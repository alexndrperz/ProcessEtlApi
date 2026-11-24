USE [master]
GO
IF DB_ID(N'DataWarehouse') IS NULL
    CREATE DATABASE [DataWarehouse];
GO

USE [DataWarehouse]
GO

IF OBJECT_ID(N'dbo.DimCliente', N'U') IS NULL
CREATE TABLE [dbo].[DimCliente](
    [ClienteKey]         INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_DimCliente] PRIMARY KEY,
    [IdCliente]          INT               NOT NULL CONSTRAINT [AK_DimCliente_IdCliente] UNIQUE,
    [Nombre]             NVARCHAR(150)     NOT NULL,
    [Email]              NVARCHAR(255)     NOT NULL,
    [FechaInsercion]     DATETIME2         NOT NULL CONSTRAINT [DF_DimCliente_FechaInsercion] DEFAULT SYSDATETIME(),
    [FechaActualizacion] DATETIME2         NULL
);
GO

IF OBJECT_ID(N'dbo.DimProducto', N'U') IS NULL
CREATE TABLE [dbo].[DimProducto](
    [ProductoKey]        INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_DimProducto] PRIMARY KEY,
    [IdProducto]         INT               NOT NULL CONSTRAINT [AK_DimProducto_IdProducto] UNIQUE,
    [Nombre]             NVARCHAR(200)     NOT NULL,
    [Categoria]          NVARCHAR(100)     NOT NULL,
    [FechaInsercion]     DATETIME2         NOT NULL CONSTRAINT [DF_DimProducto_FechaInsercion] DEFAULT SYSDATETIME(),
    [FechaActualizacion] DATETIME2         NULL
);
GO

IF OBJECT_ID(N'dbo.DimFuente', N'U') IS NULL
CREATE TABLE [dbo].[DimFuente](
    [FuenteKey]          INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_DimFuente] PRIMARY KEY,
    [IdFuente]           INT               NOT NULL CONSTRAINT [AK_DimFuente_IdFuente] UNIQUE,
    [TipoFuente]         NVARCHAR(100)     NOT NULL,
    [FechaCarga]         DATE              NULL,
    [FechaInsercion]     DATETIME2         NOT NULL CONSTRAINT [DF_DimFuente_FechaInsercion] DEFAULT SYSDATETIME(),
    [FechaActualizacion] DATETIME2         NULL
);
GO

IF OBJECT_ID(N'dbo.DimCanal', N'U') IS NULL
CREATE TABLE [dbo].[DimCanal](
    [CanalKey]           INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_DimCanal] PRIMARY KEY,
    [IdCanal]            INT               NOT NULL CONSTRAINT [AK_DimCanal_IdCanal] UNIQUE,
    [Nombre]             NVARCHAR(100)     NOT NULL,
    [FechaInsercion]     DATETIME2         NOT NULL CONSTRAINT [DF_DimCanal_FechaInsercion] DEFAULT SYSDATETIME(),
    [FechaActualizacion] DATETIME2         NULL
);
GO

IF OBJECT_ID(N'dbo.DimClasificacion', N'U') IS NULL
CREATE TABLE [dbo].[DimClasificacion](
    [ClasificacionKey]   INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_DimClasificacion] PRIMARY KEY,
    [IdClasificacion]    INT               NOT NULL CONSTRAINT [AK_DimClasificacion_IdClasificacion] UNIQUE,
    [Nombre]             NVARCHAR(50)      NOT NULL,
    [FechaInsercion]     DATETIME2         NOT NULL CONSTRAINT [DF_DimClasificacion_FechaInsercion] DEFAULT SYSDATETIME(),
    [FechaActualizacion] DATETIME2         NULL
);
GO

IF OBJECT_ID(N'dbo.DimFecha', N'U') IS NULL
CREATE TABLE [dbo].[DimFecha](
    [FechaKey]       INT          NOT NULL CONSTRAINT [PK_DimFecha] PRIMARY KEY,
    [Fecha]          DATE         NOT NULL CONSTRAINT [AK_DimFecha_Fecha] UNIQUE,
    [Anio]           SMALLINT     NOT NULL,
    [Trimestre]      TINYINT      NOT NULL,
    [Mes]            TINYINT      NOT NULL,
    [NombreMes]      NVARCHAR(20) NOT NULL,
    [Dia]            TINYINT      NOT NULL,
    [DiaSemana]      TINYINT      NOT NULL,
    [NombreDia]      NVARCHAR(20) NOT NULL,
    [EsFinDeSemana]  BIT          NOT NULL,
    [FechaInsercion] DATETIME2    NOT NULL CONSTRAINT [DF_DimFecha_FechaInsercion] DEFAULT SYSDATETIME()
);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DimCliente WHERE ClienteKey = -1)
BEGIN
    SET IDENTITY_INSERT dbo.DimCliente ON;
    INSERT INTO dbo.DimCliente (ClienteKey, IdCliente, Nombre, Email) VALUES (-1, -1, N'Desconocido', N'Desconocido');
    SET IDENTITY_INSERT dbo.DimCliente OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DimProducto WHERE ProductoKey = -1)
BEGIN
    SET IDENTITY_INSERT dbo.DimProducto ON;
    INSERT INTO dbo.DimProducto (ProductoKey, IdProducto, Nombre, Categoria) VALUES (-1, -1, N'Desconocido', N'Desconocido');
    SET IDENTITY_INSERT dbo.DimProducto OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DimFuente WHERE FuenteKey = -1)
BEGIN
    SET IDENTITY_INSERT dbo.DimFuente ON;
    INSERT INTO dbo.DimFuente (FuenteKey, IdFuente, TipoFuente) VALUES (-1, -1, N'Desconocido');
    SET IDENTITY_INSERT dbo.DimFuente OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DimCanal WHERE CanalKey = -1)
BEGIN
    SET IDENTITY_INSERT dbo.DimCanal ON;
    INSERT INTO dbo.DimCanal (CanalKey, IdCanal, Nombre) VALUES (-1, -1, N'Desconocido');
    SET IDENTITY_INSERT dbo.DimCanal OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DimClasificacion WHERE ClasificacionKey = -1)
BEGIN
    SET IDENTITY_INSERT dbo.DimClasificacion ON;
    INSERT INTO dbo.DimClasificacion (ClasificacionKey, IdClasificacion, Nombre) VALUES (-1, -1, N'Desconocido');
    SET IDENTITY_INSERT dbo.DimClasificacion OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DimFecha WHERE FechaKey = -1)
    INSERT INTO dbo.DimFecha (FechaKey, Fecha, Anio, Trimestre, Mes, NombreMes, Dia, DiaSemana, NombreDia, EsFinDeSemana)
    VALUES (-1, '19000101', 1900, 1, 1, N'Desconocido', 1, 1, N'Desconocido', 0);
GO
