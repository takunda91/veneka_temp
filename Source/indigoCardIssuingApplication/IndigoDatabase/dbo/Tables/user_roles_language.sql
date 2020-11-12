CREATE TABLE [dbo].[user_roles_language] (
    [user_role_id]  INT           NOT NULL,
    [language_id]   INT           NOT NULL,
    [language_text] VARCHAR (100) NOT NULL,
   CONSTRAINT [FK_user_roles_language_language_id] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]),
   CONSTRAINT [FK_user_roles_language_user_role_id] FOREIGN KEY ([user_role_id]) REFERENCES [dbo].[user_roles] ([user_role_id]), 
    CONSTRAINT [PK_user_roles_language] PRIMARY KEY ([user_role_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'user_roles_language'