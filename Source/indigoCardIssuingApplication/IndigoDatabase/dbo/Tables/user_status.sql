CREATE TABLE [dbo].[user_status] (
    [user_status_id]   INT          NOT NULL,
    [user_status_text] VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_user_status] PRIMARY KEY CLUSTERED ([user_status_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'user_status'