CREATE TABLE [dbo].[SantaConfig] (
    [GuildId] BIGINT NOT NULL,
    [ParticipationDeadline] DATETIME NOT NULL,
    [ExchangeDate] DATETIME NOT NULL,
    [ExchangeLocation] NVARCHAR(30) NOT NULL,
    [PriceLimit] DECIMAL NULL,
    [LockedIn] BIT NOT NULL,
)