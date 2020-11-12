CREATE TABLE [dbo].[branch_type]
(
	[branch_type_id] [int] NOT NULL,
	[branch_type_name] [nvarchar](50) NULL, 
    CONSTRAINT [PK_branch_type] PRIMARY KEY ([branch_type_id]),
)

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'branch_type'