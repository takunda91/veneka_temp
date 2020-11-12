CREATE TABLE [dbo].[audit_action] (
    [audit_action_id]   INT           NOT NULL,
    [audit_action_name] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_audit_action] PRIMARY KEY CLUSTERED ([audit_action_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'audit_action'