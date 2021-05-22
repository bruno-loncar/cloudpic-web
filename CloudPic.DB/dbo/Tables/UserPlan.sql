CREATE TABLE [dbo].[UserPlan] (
    [Id]       INT      IDENTITY (1, 1) NOT NULL,
    [UserId]   INT      NOT NULL,
    [PlanId]   INT      NOT NULL,
    [DateFrom] DATETIME NOT NULL,
    [DateTo]   DATETIME NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([PlanId]) REFERENCES [dbo].[Plan] ([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id])
);

