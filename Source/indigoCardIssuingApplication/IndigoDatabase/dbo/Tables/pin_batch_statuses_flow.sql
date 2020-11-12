CREATE TABLE [dbo].[pin_batch_statuses_flow] (
    [pin_batch_type_id]            INT      NOT NULL,
    [pin_batch_statuses_id]        INT      NOT NULL,
    [card_issue_method_id]         INT      NOT NULL,
    [user_role_id]                 INT      NOT NULL,
    [flow_pin_batch_statuses_id]   INT      NOT NULL,
    [flow_pin_batch_type_id]       INT      NOT NULL,
    [main_menu_id]                 SMALLINT NULL,
    [sub_menu_id]                  SMALLINT NULL,
    [sub_menu_order]               SMALLINT NULL,
    [reject_pin_batch_statuses_id] INT      NULL,
    [reject_pin_card_statuses_id]  INT      NULL,
    [flow_pin_card_statuses_id]    INT      NULL,
    CONSTRAINT [PK_pin_batch_statuses_flow] PRIMARY KEY CLUSTERED ([pin_batch_type_id] ASC, [pin_batch_statuses_id] ASC, [card_issue_method_id] ASC, [flow_pin_batch_statuses_id] ASC),
    CONSTRAINT [FK_pin_batch_statuses_flow_card_issue_method] FOREIGN KEY ([card_issue_method_id]) REFERENCES [dbo].[card_issue_method] ([card_issue_method_id]),
    CONSTRAINT [FK_pin_batch_statuses_flow_pin_batch_type] FOREIGN KEY ([pin_batch_type_id]) REFERENCES [dbo].[pin_batch_type] ([pin_batch_type_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'pin_batch_statuses_flow'