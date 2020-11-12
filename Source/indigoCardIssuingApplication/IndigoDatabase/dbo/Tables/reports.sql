CREATE TABLE [dbo].[reports] (
    [Reportid]   INT           NOT NULL,
    [ReportName] NVARCHAR (50) NULL,
    CONSTRAINT [PK_Reports] PRIMARY KEY CLUSTERED ([Reportid] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'reports'