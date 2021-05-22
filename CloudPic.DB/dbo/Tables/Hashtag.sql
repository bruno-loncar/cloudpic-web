CREATE TABLE [dbo].[Hashtag] (
    [Id]      INT           IDENTITY (1, 1) NOT NULL,
    [Name]    NVARCHAR (50) NOT NULL,
    [PhotoId] INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([PhotoId]) REFERENCES [dbo].[Photo] ([Id])
);

