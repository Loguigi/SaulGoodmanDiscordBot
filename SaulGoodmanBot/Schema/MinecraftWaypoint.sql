CREATE TABLE [dbo].[MinecraftWaypoint] (
    [GuildId] BIGINT NOT NULL,
    [Dimension] NVARCHAR(30) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [XCord] INT NOT NULL,
    [YCord] INT NOT NULL,
    [ZCord] INT NOT NULL
);