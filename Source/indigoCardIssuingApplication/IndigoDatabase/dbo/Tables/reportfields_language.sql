CREATE TABLE [dbo].[reportfields_language] (
    [reportfieldid] INT           NOT NULL,
    [language_id]   INT           NOT NULL,
    [language_text] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_reportfields_language] PRIMARY KEY CLUSTERED ([reportfieldid] ASC, [language_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'reportfields_language'