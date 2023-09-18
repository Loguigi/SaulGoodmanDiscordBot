CREATE TABLE [dbo].[Schedules] (
    [GuildId] BIGINT NOT NULL,
    [UserId] BIGINT NOT NULL,
    [LastUpdated] DATETIME NOT NULL,
    [RecurringSchedule] BIT NOT NULL,
    [Monday] NVARCHAR(100) NULL,
    [Tuesday] NVARCHAR(100) NULL,
    [Wednesday] NVARCHAR(100) NULL,
    [Thursday] NVARCHAR(100) NULL,
    [Friday] NVARCHAR(100) NULL,
    [Saturday] NVARCHAR(100) NULL,
    [Sunday] NVARCHAR(100) NULL,
    [PictureUrl] NVARCHAR(3000) NULL, 
);