CREATE TABLE [dbo].[user_password_history] (
    [user_id]          BIGINT          NOT NULL,
    [password_history] VARBINARY (MAX) NOT NULL,
    [date_changed]     DATETIMEOFFSET        NOT NULL,
    CONSTRAINT [FK_user_password_history_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user] ([user_id]), 
    CONSTRAINT [PK_user_password_history] PRIMARY KEY ([user_id], [date_changed])
);

