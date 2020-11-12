CREATE TABLE [dbo].[user_status_language] (
    [user_status_id] INT           NOT NULL,
    [language_id]    INT           NOT NULL,
    [language_text]  VARCHAR (100) NOT NULL,
  CONSTRAINT [FK_user_status_language_language_id]  FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]),
   CONSTRAINT [FK_user_status_language_user_status_id] FOREIGN KEY ([user_status_id]) REFERENCES [dbo].[user_status] ([user_status_id]), 
    CONSTRAINT [PK_user_status_language] PRIMARY KEY ([user_status_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'user_status_language'