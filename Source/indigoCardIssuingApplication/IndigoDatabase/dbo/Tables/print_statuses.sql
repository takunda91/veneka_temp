CREATE TABLE [dbo].[print_statuses]
(
	[print_statuses_id] INT NOT NULL PRIMARY KEY,
	[print_statuses] NVARCHAR(100) NOT NULL

);
GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'print_statuses'
