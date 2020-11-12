CREATE TABLE [dbo].[file_statuses_language] (
    [file_status_id] INT           NOT NULL,
    [language_id]    INT           NOT NULL,
    [language_text]  VARCHAR (100) NOT NULL,
   CONSTRAINT [FK_file_statuses_language_file_status_id] FOREIGN KEY ([file_status_id]) REFERENCES [dbo].[file_statuses] ([file_status_id]),
   CONSTRAINT [FK_file_statuses_language_language_id]  FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]), 
    CONSTRAINT [PK_file_statuses_language] PRIMARY KEY ([file_status_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'file_statuses_language'