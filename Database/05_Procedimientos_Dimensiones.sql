USE [DataWarehouse]
GO

DROP PROCEDURE IF EXISTS dbo.usp_CargarDimCliente;
DROP PROCEDURE IF EXISTS dbo.usp_CargarDimProducto;
DROP PROCEDURE IF EXISTS dbo.usp_CargarDimFuente;
DROP PROCEDURE IF EXISTS dbo.usp_CargarDimCanal;
DROP PROCEDURE IF EXISTS dbo.usp_CargarDimClasificacion;
DROP PROCEDURE IF EXISTS dbo.usp_CargarDimFecha;
GO

DROP TYPE IF EXISTS dbo.ClienteTipo;
DROP TYPE IF EXISTS dbo.ProductoTipo;
DROP TYPE IF EXISTS dbo.FuenteTipo;
DROP TYPE IF EXISTS dbo.CanalTipo;
DROP TYPE IF EXISTS dbo.ClasificacionTipo;
GO

CREATE TYPE dbo.ClienteTipo AS TABLE(
    [IdCliente] INT           NOT NULL PRIMARY KEY,
    [Nombre]    NVARCHAR(150) NOT NULL,
    [Email]     NVARCHAR(255) NOT NULL
);
GO

CREATE TYPE dbo.ProductoTipo AS TABLE(
    [IdProducto] INT           NOT NULL PRIMARY KEY,
    [Nombre]     NVARCHAR(200) NOT NULL,
    [Categoria]  NVARCHAR(100) NOT NULL
);
GO

CREATE TYPE dbo.FuenteTipo AS TABLE(
    [IdFuente]   INT           NOT NULL PRIMARY KEY,
    [TipoFuente] NVARCHAR(100) NOT NULL,
    [FechaCarga] DATE          NULL
);
GO

CREATE TYPE dbo.CanalTipo AS TABLE(
    [IdCanal] INT           NOT NULL PRIMARY KEY,
    [Nombre]  NVARCHAR(100) NOT NULL
);
GO

CREATE TYPE dbo.ClasificacionTipo AS TABLE(
    [IdClasificacion] INT          NOT NULL PRIMARY KEY,
    [Nombre]          NVARCHAR(50) NOT NULL
);
GO

CREATE PROCEDURE dbo.usp_CargarDimCliente
    @Filas dbo.ClienteTipo READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Cambios TABLE (Accion NVARCHAR(10));

    MERGE dbo.DimCliente AS destino
    USING @Filas AS origen
        ON destino.IdCliente = origen.IdCliente
    WHEN MATCHED AND EXISTS (SELECT origen.Nombre, origen.Email EXCEPT SELECT destino.Nombre, destino.Email) THEN
        UPDATE SET Nombre = origen.Nombre,
                   Email = origen.Email,
                   FechaActualizacion = SYSDATETIME()
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (IdCliente, Nombre, Email)
        VALUES (origen.IdCliente, origen.Nombre, origen.Email)
    OUTPUT $action INTO @Cambios;

    SELECT Insertadas   = COUNT(CASE WHEN Accion = N'INSERT' THEN 1 END),
           Actualizadas = COUNT(CASE WHEN Accion = N'UPDATE' THEN 1 END)
    FROM @Cambios;
END
GO

CREATE PROCEDURE dbo.usp_CargarDimProducto
    @Filas dbo.ProductoTipo READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Cambios TABLE (Accion NVARCHAR(10));

    MERGE dbo.DimProducto AS destino
    USING @Filas AS origen
        ON destino.IdProducto = origen.IdProducto
    WHEN MATCHED AND EXISTS (SELECT origen.Nombre, origen.Categoria EXCEPT SELECT destino.Nombre, destino.Categoria) THEN
        UPDATE SET Nombre = origen.Nombre,
                   Categoria = origen.Categoria,
                   FechaActualizacion = SYSDATETIME()
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (IdProducto, Nombre, Categoria)
        VALUES (origen.IdProducto, origen.Nombre, origen.Categoria)
    OUTPUT $action INTO @Cambios;

    SELECT Insertadas   = COUNT(CASE WHEN Accion = N'INSERT' THEN 1 END),
           Actualizadas = COUNT(CASE WHEN Accion = N'UPDATE' THEN 1 END)
    FROM @Cambios;
END
GO

CREATE PROCEDURE dbo.usp_CargarDimFuente
    @Filas dbo.FuenteTipo READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Cambios TABLE (Accion NVARCHAR(10));

    MERGE dbo.DimFuente AS destino
    USING @Filas AS origen
        ON destino.IdFuente = origen.IdFuente
    WHEN MATCHED AND EXISTS (SELECT origen.TipoFuente, origen.FechaCarga EXCEPT SELECT destino.TipoFuente, destino.FechaCarga) THEN
        UPDATE SET TipoFuente = origen.TipoFuente,
                   FechaCarga = origen.FechaCarga,
                   FechaActualizacion = SYSDATETIME()
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (IdFuente, TipoFuente, FechaCarga)
        VALUES (origen.IdFuente, origen.TipoFuente, origen.FechaCarga)
    OUTPUT $action INTO @Cambios;

    SELECT Insertadas   = COUNT(CASE WHEN Accion = N'INSERT' THEN 1 END),
           Actualizadas = COUNT(CASE WHEN Accion = N'UPDATE' THEN 1 END)
    FROM @Cambios;
END
GO

CREATE PROCEDURE dbo.usp_CargarDimCanal
    @Filas dbo.CanalTipo READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Cambios TABLE (Accion NVARCHAR(10));

    MERGE dbo.DimCanal AS destino
    USING @Filas AS origen
        ON destino.IdCanal = origen.IdCanal
    WHEN MATCHED AND EXISTS (SELECT origen.Nombre EXCEPT SELECT destino.Nombre) THEN
        UPDATE SET Nombre = origen.Nombre,
                   FechaActualizacion = SYSDATETIME()
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (IdCanal, Nombre)
        VALUES (origen.IdCanal, origen.Nombre)
    OUTPUT $action INTO @Cambios;

    SELECT Insertadas   = COUNT(CASE WHEN Accion = N'INSERT' THEN 1 END),
           Actualizadas = COUNT(CASE WHEN Accion = N'UPDATE' THEN 1 END)
    FROM @Cambios;
END
GO

CREATE PROCEDURE dbo.usp_CargarDimClasificacion
    @Filas dbo.ClasificacionTipo READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Cambios TABLE (Accion NVARCHAR(10));

    MERGE dbo.DimClasificacion AS destino
    USING @Filas AS origen
        ON destino.IdClasificacion = origen.IdClasificacion
    WHEN MATCHED AND EXISTS (SELECT origen.Nombre EXCEPT SELECT destino.Nombre) THEN
        UPDATE SET Nombre = origen.Nombre,
                   FechaActualizacion = SYSDATETIME()
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (IdClasificacion, Nombre)
        VALUES (origen.IdClasificacion, origen.Nombre)
    OUTPUT $action INTO @Cambios;

    SELECT Insertadas   = COUNT(CASE WHEN Accion = N'INSERT' THEN 1 END),
           Actualizadas = COUNT(CASE WHEN Accion = N'UPDATE' THEN 1 END)
    FROM @Cambios;
END
GO

CREATE PROCEDURE dbo.usp_CargarDimFecha
    @FechaInicio DATE,
    @FechaFin    DATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @FechaInicio IS NULL OR @FechaFin IS NULL OR @FechaInicio > @FechaFin
    BEGIN
        SELECT Insertadas = 0, Actualizadas = 0;
        RETURN;
    END;

    WITH Dias AS (
        SELECT @FechaInicio AS Fecha
        UNION ALL
        SELECT DATEADD(DAY, 1, Fecha) FROM Dias WHERE Fecha < @FechaFin
    ),
    Calendario AS (
        SELECT Fecha,
               DiaSemana = (DATEDIFF(DAY, '19000101', Fecha) % 7) + 1
        FROM Dias
    )
    INSERT INTO dbo.DimFecha (FechaKey, Fecha, Anio, Trimestre, Mes, NombreMes, Dia, DiaSemana, NombreDia, EsFinDeSemana)
    SELECT YEAR(c.Fecha) * 10000 + MONTH(c.Fecha) * 100 + DAY(c.Fecha),
           c.Fecha,
           YEAR(c.Fecha),
           DATEPART(QUARTER, c.Fecha),
           MONTH(c.Fecha),
           CHOOSE(MONTH(c.Fecha), N'Enero', N'Febrero', N'Marzo', N'Abril', N'Mayo', N'Junio',
                  N'Julio', N'Agosto', N'Septiembre', N'Octubre', N'Noviembre', N'Diciembre'),
           DAY(c.Fecha),
           c.DiaSemana,
           CHOOSE(c.DiaSemana, N'Lunes', N'Martes', N'Miércoles', N'Jueves', N'Viernes', N'Sábado', N'Domingo'),
           CASE WHEN c.DiaSemana IN (6, 7) THEN 1 ELSE 0 END
    FROM Calendario c
    WHERE NOT EXISTS (SELECT 1 FROM dbo.DimFecha d WHERE d.Fecha = c.Fecha)
    OPTION (MAXRECURSION 0);

    SELECT Insertadas = @@ROWCOUNT, Actualizadas = 0;
END
GO
