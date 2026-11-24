# AnalisisOpiniones.ETL

#### 1. Levantar DB
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
