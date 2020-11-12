CREATE TABLE [dbo].[threed_secure_batch_statuses_language]
(
	[threed_batch_statuses_id] INT NOT NULL , 
    [language_id] INT NOT NULL, 
    [language_text] VARCHAR(100) NOT NULL, 
    CONSTRAINT [PK_threed_secure_batch_statuses_language] PRIMARY KEY ([language_id], [threed_batch_statuses_id]), 
    CONSTRAINT [FK_threed_secure_batch_statuses_language_to_language] FOREIGN KEY ([language_id]) REFERENCES [languages]([id])
)

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'threed_secure_batch_statuses_language'