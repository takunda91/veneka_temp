CREATE TABLE [dbo].[issuer_statuses] (
    [issuer_status_id]   INT          NOT NULL,
    [issuer_status_name] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_issuer_statuses] PRIMARY KEY CLUSTERED ([issuer_status_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'issuer_statuses'