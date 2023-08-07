CREATE TABLE [dbo].[Roles] (
    [GuildId] BIGINT NOT NULL,
    [RoleId] BIGINT NOT NULL,
    [Description] NVARCHAR(200) NULL,
    [RoleEmoji] NVARCHAR(100) NULL,
);