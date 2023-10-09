CREATE TABLE [dbo].[Schedules] (
    [GuildId] BIGINT NOT NULL,
    [UserId] BIGINT NOT NULL,
    [LastUpdated] DATETIME NULL,
    [RecurringSchedule] BIT NOT NULL,
    [Sunday] NVARCHAR(100) NULL,
    [Monday] NVARCHAR(100) NULL,
    [Tuesday] NVARCHAR(100) NULL,
    [Wednesday] NVARCHAR(100) NULL,
    [Thursday] NVARCHAR(100) NULL,
    [Friday] NVARCHAR(100) NULL,
    [Saturday] NVARCHAR(100) NULL,
    [PictureUrl] NVARCHAR(3000) NULL, 
);