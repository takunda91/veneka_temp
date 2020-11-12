CREATE TABLE [dbo].[report_reportfields] (
    [reportid]           INT NOT NULL,
    [reportfieldid]      INT NOT NULL,
    [reportfieldorderno] INT NULL,
    CONSTRAINT [PK_report_fields] PRIMARY KEY CLUSTERED ([reportid] ASC, [reportfieldid] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'report_reportfields'