CREATE TABLE [dbo].[audit_action_language] (
    [audit_action_id] INT           NOT NULL,
    [language_id]     INT           NOT NULL,
    [language_text]   VARCHAR (100) NOT NULL,
	CONSTRAINT [FK_audit_action_language_audit_action]  FOREIGN KEY ([audit_action_id]) REFERENCES [dbo].[audit_action] ([audit_action_id]),
	CONSTRAINT [FK_audit_action_language_user]  FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]), 
    CONSTRAINT [PK_audit_action_language] PRIMARY KEY ([audit_action_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'audit_action_language'