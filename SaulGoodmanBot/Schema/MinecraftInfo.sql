CREATE TABLE [dbo].[MinecraftInfo] (
    [GuildId] BIGINT NOT NULL,
    [WorldName] NVARCHAR(100) NOT NULL,
    [WorldDescription] NVARCHAR(1000) NULL,
    [IPAddress] NVARCHAR(30) NULL,
    [MaxPlayers] INT NULL,
    [Whitelist] BIT NOT NULL
);