CREATE TABLE [dbo].[dist_batch_statuses_flow] (
    [dist_batch_statuses_id]         INT      NOT NULL,
    [user_role_id]                   INT      NOT NULL,
    [flow_dist_batch_statuses_id]    INT      NOT NULL,
    [flow_dist_batch_type_id]        INT      NOT NULL,
    [main_menu_id]                   SMALLINT NULL,
    [sub_menu_id]                    SMALLINT NULL,
    [sub_menu_order]                 SMALLINT NULL,
    [reject_dist_batch_statuses_id]  INT      NULL,
    [flow_dist_card_statuses_id]     INT      NULL,
    [reject_dist_card_statuses_id]   INT      NULL,
    [branch_card_statuses_id]        INT      NULL,
    [reject_branch_card_statuses_id] INT      NULL,
    [dist_batch_status_flow_id]      INT      NOT NULL,
    CONSTRAINT [PK_DistStatusesFlow] PRIMARY KEY CLUSTERED ([dist_batch_status_flow_id] ASC, [dist_batch_statuses_id] ASC, [flow_dist_batch_statuses_id] ASC),
    CONSTRAINT [FK_dist_batch_status_flow_dist_batch_statuses_flow] FOREIGN KEY ([dist_batch_status_flow_id]) REFERENCES [dbo].[dist_batch_status_flow] ([dist_batch_status_flow_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'dist_batch_statuses_flow'