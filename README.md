# AnalisisOpiniones.ETL

Worker Service en .NET 8 que realiza la Extracción hacia staging y la Carga de dimensiones hacia la base `DataWarehouse` del Sistema de Análisis de Opiniones de Clientes.

> **IMPORTANTE: antes de ejecutar este proyecto se debe correr el `docker compose` de este repositorio.**
> El compose levanta SQL Server y crea las bases de datos `AnalisisOpiniones` (tablas de origen con sus datos y tablas staging) y `DataWarehouse` (dimensiones y procedimientos almacenados de carga).
> Sin ese paso el proyecto no puede ejecutarse, porque la base de datos no existe.

## Requisitos

- Docker Desktop
- .NET 8 SDK

## Pasos para ejecutar

### 1. Levantar la base de datos con docker compose

Desde la carpeta raíz de este repositorio (donde está `docker-compose.yml`):

```bash
docker compose up -d
```

El servicio `db-init` ejecuta en orden los scripts de la carpeta `Database`:

| Script | Contenido |
|---|---|
| `00_BaseDatos.sql` | Crea la base de datos `AnalisisOpiniones`. |
| `01_Origen.sql` | Crea las tablas de origen (clientes, productos, reseñas web, comentarios sociales, catálogos). |
| `02_Datos_Origen.sql` | Inserta los datos de origen. |
| `03_Staging.sql` | Crea el esquema `stg` con las tablas staging. |
| `04_DataWarehouse.sql` | Crea la base `DataWarehouse`, las tablas de dimensión y el miembro `Desconocido` (clave -1) de cada una. |
| `05_Procedimientos_Dimensiones.sql` | Crea los tipos tabla y los procedimientos almacenados `usp_CargarDim*` que cargan cada dimensión sin duplicados. |

Para recrear la base de datos desde cero:

```bash
docker compose down -v
docker compose up -d
```

Por defecto SQL Server queda en el puerto `1433` con la contraseña configurada en `appsettings.json`. Se pueden cambiar con las variables `SQL_PORT` y `MSSQL_SA_PASSWORD` (por ejemplo en un archivo `.env`); en ese caso hay que actualizar también las cadenas de conexión de `appsettings.json`.

### 2. Levantar la API REST (terminal 1)

```bash
dotnet run --project AnalisisOpiniones.Api
```

La API queda disponible en `http://localhost:5080/api/social-comments?page=1&pageSize=5`.

### 3. Ejecutar el Worker ETL (terminal 2)

```bash
dotnet run --project AnalisisOpiniones.Etl.Worker
```

El Worker ejecuta dos fases:

**Extracción**, en dos etapas:

1. Catálogos desde la base de datos: clientes, productos, fuentes de datos, clasificaciones y canales.
2. Opiniones: encuestas (CSV), reseñas web (base de datos) y comentarios sociales (API REST).

**Carga de dimensiones**, en paralelo: lee el staging con ADO.NET, limpia los datos, elimina duplicados y envía cada dimensión como parámetro tipo tabla a su procedimiento almacenado. Cada procedimiento usa `MERGE`: inserta los registros nuevos, actualiza solo los que cambiaron y no vuelve a insertar los existentes, por lo que el proceso se puede ejecutar varias veces sin duplicar datos.

| Dimensión | Origen | Procedimiento |
|---|---|---|
| `DimCliente` | `stg.Clientes` | `dbo.usp_CargarDimCliente` |
| `DimProducto` | `stg.Productos` | `dbo.usp_CargarDimProducto` |
| `DimFuente` | `stg.FuentesDatos` | `dbo.usp_CargarDimFuente` |
| `DimCanal` | `stg.Canales` | `dbo.usp_CargarDimCanal` |
| `DimClasificacion` | `stg.Clasificaciones` | `dbo.usp_CargarDimClasificacion` |
| `DimFecha` | Rango de fechas del staging (años completos) | `dbo.usp_CargarDimFecha` |

La carga se activa o desactiva con `Carga:Habilitado` en `appsettings.json`.

### 4. Verificar el staging

```sql
SELECT 'stg.Encuestas' AS Tabla, COUNT(*) AS Filas FROM stg.Encuestas
UNION ALL SELECT 'stg.ResenasWeb', COUNT(*) FROM stg.ResenasWeb
UNION ALL SELECT 'stg.ComentariosSociales', COUNT(*) FROM stg.ComentariosSociales
UNION ALL SELECT 'stg.Clientes', COUNT(*) FROM stg.Clientes
UNION ALL SELECT 'stg.Productos', COUNT(*) FROM stg.Productos
UNION ALL SELECT 'stg.FuentesDatos', COUNT(*) FROM stg.FuentesDatos
UNION ALL SELECT 'stg.Clasificaciones', COUNT(*) FROM stg.Clasificaciones
UNION ALL SELECT 'stg.Canales', COUNT(*) FROM stg.Canales;
```

### 5. Verificar las dimensiones

```sql
USE DataWarehouse;

SELECT 'DimCliente' AS Tabla, COUNT(*) AS Filas FROM dbo.DimCliente
UNION ALL SELECT 'DimProducto', COUNT(*) FROM dbo.DimProducto
UNION ALL SELECT 'DimFuente', COUNT(*) FROM dbo.DimFuente
UNION ALL SELECT 'DimCanal', COUNT(*) FROM dbo.DimCanal
UNION ALL SELECT 'DimClasificacion', COUNT(*) FROM dbo.DimClasificacion
UNION ALL SELECT 'DimFecha', COUNT(*) FROM dbo.DimFecha;
```
