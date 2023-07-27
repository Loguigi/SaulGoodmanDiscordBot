CREATE TABLE [dbo].[Config] (
    [GuildId] BIGINT NOT NULL,

    -- Basic Config
    [WelcomeMessage] NVARCHAR(200) NULL,
    [LeaveMessage] NVARCHAR(200) NULL,

    -- Birthday Config
    [BirthdayNotifications] BIT NOT NULL,
    [PauseBdayNotifsTimer] DATETIME NULL
);