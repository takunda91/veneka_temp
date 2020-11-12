CREATE TABLE [dbo].[dist_batch_status_flow] (
    [dist_batch_status_flow_id]   INT           NOT NULL,
    [dist_batch_status_flow_name] VARCHAR (150) NOT NULL,
    [dist_batch_type_id]          INT           NOT NULL,
    [card_issue_method_id]        INT           NOT NULL,
    CONSTRAINT [FK_dist_batch_status_flow_card_issue_method] FOREIGN KEY ([card_issue_method_id]) REFERENCES [dbo].[card_issue_method] ([card_issue_method_id]),
    CONSTRAINT [FK_dist_batch_status_flow_dist_batcht_ype] FOREIGN KEY ([dist_batch_type_id]) REFERENCES [dbo].[dist_batch_type] ([dist_batch_type_id]), 
    CONSTRAINT [PK_dist_batch_status_flow] PRIMARY KEY ([dist_batch_status_flow_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'dist_batch_status_flow'