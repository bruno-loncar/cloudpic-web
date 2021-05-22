CREATE TABLE [dbo].[Log] (
    [Id]        INT      IDENTITY (1, 1) NOT NULL,
    [UserId]    INT      NOT NULL,
    [LogTypeId] INT      NOT NULL,
    [ObjectId]  INT      NOT NULL,
    [LogDate]   DATETIME NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([LogTypeId]) REFERENCES [dbo].[LogType] ([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id])
);

