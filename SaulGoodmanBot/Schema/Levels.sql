CREATE TABLE [dbo].[Levels] (
    [GuildId] BIGINT NOT NULL,
    [UserId] BIGINT NOT NULL,
    [Level] INT NOT NULL,
    [Experience] INT NOT NULL,
    [MsgLastSent] DATETIME NOT NULL
);