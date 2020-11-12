CREATE TABLE [dbo].[remote_update_statuses]
(
	[remote_update_statuses_id] INT NOT NULL , 
    [remote_update_statuses_name] VARCHAR(100) NOT NULL, 
    CONSTRAINT [PK_remote_update_statuses] PRIMARY KEY ([remote_update_statuses_id])
)

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'remote_update_statuses'