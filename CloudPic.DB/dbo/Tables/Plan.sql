CREATE TABLE [dbo].[Plan] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (50) NOT NULL,
    [PricePerDay]  DECIMAL (18)  NOT NULL,
    [PhotosPerDay] INT           NOT NULL,
    [StorageSize]  INT           NOT NULL,
    [MbPerDay]     INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

