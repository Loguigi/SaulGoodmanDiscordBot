CREATE TABLE [dbo].[Config] (
    [GuildId] BIGINT NOT NULL,

    -- Basic Config
    [WelcomeMessage] NVARCHAR(200) NULL,
    [LeaveMessage] NVARCHAR(200) NULL,
    [DefaultChannel] BIGINT NOT NULL,

    -- Birthday Config
    [BirthdayNotifications] BIT NOT NULL,
    [PauseBdayNotifsTimer] DATE NOT NULL,
    [BirthdayMessage] NVARCHAR(200) NOT NULL,

    -- Role Config
    [ServerRolesName] NVARCHAR(100) NULL,
    [ServerRolesDescription] NVARCHAR(1000) NULL,
    [AllowMultipleRoles] BIT NOT NULL,

    -- Level Config
    [EnableLevels] BIT NOT NULL,
    [LevelUpMessage] NVARCHAR(200) NOT NULL,
);