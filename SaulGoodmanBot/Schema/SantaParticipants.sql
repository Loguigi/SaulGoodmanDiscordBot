CREATE TABLE [dbo].[SantaParticipants] (
    [GuildId] BIGINT NOT NULL,
    [UserId] BIGINT NOT NULL,
    [FirstName] NVARCHAR(20) NOT NULL,
    [GifteeId] BIGINT NULL,
    [SOId] BIGINT NULL
)