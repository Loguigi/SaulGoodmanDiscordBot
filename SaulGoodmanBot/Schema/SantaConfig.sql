CREATE TABLE [dbo].[SantaConfig] (
    [GuildId] BIGINT NOT NULL,
    [ParticipationDeadline] DATETIME NOT NULL,
    [ExchangeDate] DATETIME NOT NULL,
    [ExchangeLocation] NVARCHAR(30) NOT NULL,
    [ExchangeAddress] NVARCHAR(100) NULL,
    [PriceLimit] DECIMAL NULL,
    [LockedIn] BIT NOT NULL,
)