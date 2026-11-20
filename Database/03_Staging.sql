USE [AnalisisOpiniones]
GO
IF SCHEMA_ID(N'stg') IS NULL
    EXEC(N'CREATE SCHEMA [stg]');
GO

IF OBJECT_ID(N'stg.Encuestas', N'U') IS NULL
CREATE TABLE [stg].[Encuestas](
    [IdOpinion]           NVARCHAR(50)  NULL,
    [IdCliente]           NVARCHAR(50)  NULL,
    [IdProducto]          NVARCHAR(50)  NULL,
    [Fecha]               NVARCHAR(50)  NULL,
    [Comentario]          NVARCHAR(MAX) NULL,
    [Clasificacion]       NVARCHAR(50)  NULL,
    [PuntajeSatisfaccion] NVARCHAR(20)  NULL,
    [Fuente]              NVARCHAR(50)  NULL,
    [FechaExtraccion]     DATETIME2     NOT NULL
);
GO

IF OBJECT_ID(N'stg.ResenasWeb', N'U') IS NULL
CREATE TABLE [stg].[ResenasWeb](
    [IdReview]        INT           NOT NULL,
    [IdCliente]       INT           NOT NULL,
    [IdProducto]      INT           NOT NULL,
    [Fecha]           DATE          NULL,
    [Comentario]      NVARCHAR(MAX) NULL,
    [Rating]          DECIMAL(2,1)  NULL,
    [IdFuente]        INT           NULL,
    [FechaExtraccion] DATETIME2     NOT NULL
);
GO

IF OBJECT_ID(N'stg.ComentariosSociales', N'U') IS NULL
CREATE TABLE [stg].[ComentariosSociales](
    [IdComment]       INT           NOT NULL,
    [IdCliente]       INT           NOT NULL,
    [IdProducto]      INT           NOT NULL,
    [Canal]           NVARCHAR(100) NULL,
    [Fecha]           DATE          NULL,
    [Comentario]      NVARCHAR(MAX) NULL,
    [FechaExtraccion] DATETIME2     NOT NULL
);
GO

IF OBJECT_ID(N'stg.Clientes', N'U') IS NULL
CREATE TABLE [stg].[Clientes](
    [IdCliente]       INT           NOT NULL,
    [Nombre]          NVARCHAR(150) NULL,
    [Email]           NVARCHAR(255) NULL,
    [FechaExtraccion] DATETIME2     NOT NULL
);
GO

IF OBJECT_ID(N'stg.Productos', N'U') IS NULL
CREATE TABLE [stg].[Productos](
    [IdProducto]      INT           NOT NULL,
    [Nombre]          NVARCHAR(200) NULL,
    [Categoria]       NVARCHAR(100) NULL,
    [FechaExtraccion] DATETIME2     NOT NULL
);
GO

IF OBJECT_ID(N'stg.FuentesDatos', N'U') IS NULL
CREATE TABLE [stg].[FuentesDatos](
    [IdFuente]        INT           NOT NULL,
    [TipoFuente]      NVARCHAR(100) NULL,
    [FechaCarga]      DATE          NULL,
    [FechaExtraccion] DATETIME2     NOT NULL
);
GO

IF OBJECT_ID(N'stg.Clasificaciones', N'U') IS NULL
CREATE TABLE [stg].[Clasificaciones](
    [IdClasificacion] INT           NOT NULL,
    [Nombre]          NVARCHAR(50)  NULL,
    [FechaExtraccion] DATETIME2     NOT NULL
);
GO

IF OBJECT_ID(N'stg.Canales', N'U') IS NULL
CREATE TABLE [stg].[Canales](
    [IdCanal]         INT           NOT NULL,
    [Nombre]          NVARCHAR(100) NULL,
    [FechaExtraccion] DATETIME2     NOT NULL
);
GO
