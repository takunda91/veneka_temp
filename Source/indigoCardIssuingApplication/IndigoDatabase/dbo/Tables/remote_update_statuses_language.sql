CREATE TABLE [dbo].[remote_update_statuses_language]
(
	[remote_update_statuses_id] INT NOT NULL , 
    [language_id] INT NOT NULL, 
    [language_text] NVARCHAR(100) NOT NULL, 
    CONSTRAINT [PK_remote_update_statuses_language] PRIMARY KEY ([remote_update_statuses_id], [language_id]), 
    CONSTRAINT [FK_remote_update_statuses_language_languages] FOREIGN KEY ([language_id]) REFERENCES [languages]([id]), 
    CONSTRAINT [FK_remote_update_statuses_language_remote_update_statuses] FOREIGN KEY ([remote_update_statuses_id]) REFERENCES [remote_update_statuses]([remote_update_statuses_id])
)

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'remote_update_statuses_language'