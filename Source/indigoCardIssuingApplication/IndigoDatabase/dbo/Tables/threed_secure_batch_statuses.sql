CREATE TABLE [dbo].[threed_secure_batch_statuses]
(
	[threed_batch_statuses_id] INT NOT NULL , 
    [threed_batch_statuses_name] NVARCHAR(200) NOT NULL, 
    CONSTRAINT [PK_threed_secure_batch_statuses] PRIMARY KEY ([threed_batch_statuses_id])
)

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'threed_secure_batch_statuses'