CREATE TABLE [dbo].[branch_type_language]
(
	[branch_type_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](1000) NULL, 
    CONSTRAINT [PK_branch_type_language] PRIMARY KEY ([language_id], [branch_type_id]), 
    CONSTRAINT [FK_branch_type_language_language] FOREIGN KEY ([language_id]) REFERENCES [languages]([id]), 
    CONSTRAINT [FK_branch_type_language_branch_type] FOREIGN KEY ([branch_type_id]) REFERENCES [branch_type]([branch_type_id])
)

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'branch_type_language'