USE [AnalisisOpiniones]
GO

IF OBJECT_ID(N'dbo.Categorias', N'U') IS NULL
CREATE TABLE [dbo].[Categorias](
    [IdCategoria] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Categorias] PRIMARY KEY,
    [Nombre]      VARCHAR(100)      NOT NULL CONSTRAINT [AK_Categorias_Nombre] UNIQUE
);
GO

IF OBJECT_ID(N'dbo.TiposFuente', N'U') IS NULL
CREATE TABLE [dbo].[TiposFuente](
    [IdTipoFuente] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_TiposFuente] PRIMARY KEY,
    [Nombre]       VARCHAR(100)      NOT NULL CONSTRAINT [AK_TiposFuente_Nombre] UNIQUE
);
GO

IF OBJECT_ID(N'dbo.Clasificaciones', N'U') IS NULL
CREATE TABLE [dbo].[Clasificaciones](
    [IdClasificacion] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Clasificaciones] PRIMARY KEY,
    [Nombre]          VARCHAR(50)       NOT NULL CONSTRAINT [AK_Clasificaciones_Nombre] UNIQUE
);
GO

IF OBJECT_ID(N'dbo.Canales', N'U') IS NULL
CREATE TABLE [dbo].[Canales](
    [IdCanal] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Canales] PRIMARY KEY,
    [Nombre]  VARCHAR(100)      NOT NULL CONSTRAINT [AK_Canales_Nombre] UNIQUE
);
GO

IF OBJECT_ID(N'dbo.Clients', N'U') IS NULL
CREATE TABLE [dbo].[Clients](
    [IdCliente] INT          NOT NULL CONSTRAINT [PK_Clients] PRIMARY KEY,
    [Nombre]    VARCHAR(150) NULL,
    [Email]     VARCHAR(255) NULL CONSTRAINT [AK_Clients_Email] UNIQUE
);
GO

IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
CREATE TABLE [dbo].[Products](
    [IdProducto]  INT          NOT NULL CONSTRAINT [PK_Products] PRIMARY KEY,
    [Nombre]      VARCHAR(200) NULL,
    [IdCategoria] INT          NULL CONSTRAINT [FK_Products_Categorias] REFERENCES [dbo].[Categorias]([IdCategoria])
);
GO

IF OBJECT_ID(N'dbo.FuenteDatos', N'U') IS NULL
CREATE TABLE [dbo].[FuenteDatos](
    [IdFuente]     INT  NOT NULL CONSTRAINT [PK_FuenteDatos] PRIMARY KEY,
    [IdTipoFuente] INT  NULL CONSTRAINT [FK_FuenteDatos_TiposFuente] REFERENCES [dbo].[TiposFuente]([IdTipoFuente]),
    [FechaCarga]   DATE NULL
);
GO

IF OBJECT_ID(N'dbo.WebReview', N'U') IS NULL
CREATE TABLE [dbo].[WebReview](
    [IdReview]   INT          NOT NULL CONSTRAINT [PK_WebReview] PRIMARY KEY,
    [IdCliente]  INT          NOT NULL CONSTRAINT [FK_WebReview_Clients] REFERENCES [dbo].[Clients]([IdCliente]),
    [IdProducto] INT          NOT NULL CONSTRAINT [FK_WebReview_Products] REFERENCES [dbo].[Products]([IdProducto]),
    [Fecha]      DATE         NULL,
    [Comentario] TEXT         NULL,
    [Rating]     DECIMAL(2,1) NULL,
    [IdFuente]   INT          NULL CONSTRAINT [FK_WebReview_FuenteDatos] REFERENCES [dbo].[FuenteDatos]([IdFuente])
);
GO

IF OBJECT_ID(N'dbo.Surveys', N'U') IS NULL
CREATE TABLE [dbo].[Surveys](
    [IdOpinion]           INT          NOT NULL CONSTRAINT [PK_Surveys] PRIMARY KEY,
    [IdCliente]           INT          NOT NULL CONSTRAINT [FK_Surveys_Clients] REFERENCES [dbo].[Clients]([IdCliente]),
    [IdProducto]          INT          NOT NULL CONSTRAINT [FK_Surveys_Products] REFERENCES [dbo].[Products]([IdProducto]),
    [Fecha]               DATE         NULL,
    [Comentario]          TEXT         NULL,
    [IdClasificacion]     INT          NULL CONSTRAINT [FK_Surveys_Clasificaciones] REFERENCES [dbo].[Clasificaciones]([IdClasificacion]),
    [PuntajeSatisfaccion] DECIMAL(3,1) NULL,
    [IdCanal]             INT          NULL CONSTRAINT [FK_Surveys_Canales] REFERENCES [dbo].[Canales]([IdCanal]),
    [IdFuente]            INT          NULL CONSTRAINT [FK_Surveys_FuenteDatos] REFERENCES [dbo].[FuenteDatos]([IdFuente])
);
GO

IF OBJECT_ID(N'dbo.SocialComments', N'U') IS NULL
CREATE TABLE [dbo].[SocialComments](
    [IdComment]  INT  NOT NULL CONSTRAINT [PK_SocialComments] PRIMARY KEY,
    [IdCliente]  INT  NOT NULL,
    [IdProducto] INT  NOT NULL,
    [IdCanal]    INT  NULL CONSTRAINT [FK_SocialComments_Canales] REFERENCES [dbo].[Canales]([IdCanal]),
    [IdFuente]   INT  NULL,
    [Fecha]      DATE NULL,
    [Comentario] TEXT NULL
);
GO
