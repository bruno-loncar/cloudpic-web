CREATE TABLE [dbo].[Photo] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Guid]        NVARCHAR (36) NOT NULL,
    [UserId]      INT           NOT NULL,
    [UploadDate]  DATETIME      NOT NULL,
    [Description] NVARCHAR (36) NOT NULL,
    [Format]      NVARCHAR (10) NOT NULL,
    [SizeInMb]    DECIMAL (18)  NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id])
);

