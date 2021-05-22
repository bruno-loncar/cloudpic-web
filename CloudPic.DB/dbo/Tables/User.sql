CREATE TABLE [dbo].[User] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Email]         NVARCHAR (50)  NOT NULL,
    [Password]      NVARCHAR (150) NOT NULL,
    [Name]          NVARCHAR (50)  NOT NULL,
    [LoginProvider] NVARCHAR (10)  NOT NULL,
    [SsoToken]      NVARCHAR (150) NOT NULL,
    [RegisterDate]  DATETIME       NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

