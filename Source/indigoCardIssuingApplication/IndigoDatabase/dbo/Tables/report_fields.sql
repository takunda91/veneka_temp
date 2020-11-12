CREATE TABLE [dbo].[report_fields] (
    [reportfieldid]   INT           NOT NULL,
    [reportfieldname] NVARCHAR (50) NULL,
    CONSTRAINT [PK_report_fields_1] PRIMARY KEY CLUSTERED ([reportfieldid] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'report_fields'