CREATE TABLE [dbo].[dist_batch_statuses] (
    [dist_batch_statuses_id]          INT          NOT NULL,
    [dist_batch_status_name]          VARCHAR (50) NOT NULL,
    [dist_batch_expected_statuses_id] INT          NULL,
    CONSTRAINT [PK_dist_batch_statuses] PRIMARY KEY CLUSTERED ([dist_batch_statuses_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'dist_batch_statuses'