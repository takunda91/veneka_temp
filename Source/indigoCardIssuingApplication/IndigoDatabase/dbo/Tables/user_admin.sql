CREATE TABLE [dbo].[user_admin] (
    [user_admin_id]                  INT      IDENTITY (1, 1) NOT NULL,
    [PasswordValidPeriod]            INT      NOT NULL,
    [PasswordMinLength]              INT      NOT NULL,
    [PasswordMaxLength]              INT      NOT NULL,
    [PreviousPasswordsCount]         INT      NOT NULL,
    [maxInvalidPasswordAttempts]     INT      NOT NULL,
    [PasswordAttemptLockoutDuration] INT      NOT NULL,
    [CreatedBy]                      INT      NULL,
    [CreatedDateTime]                 DATETIMEOFFSET(7)  NULL,
    CONSTRAINT [PK_user_admin] PRIMARY KEY CLUSTERED ([user_admin_id] ASC),
    CONSTRAINT [CK_user_admin] CHECK ([PasswordMaxLength]>[PasswordMinLength])
);

