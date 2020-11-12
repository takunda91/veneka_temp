CREATE TABLE [dbo].[user_roles] (
    [user_role_id]         INT          NOT NULL,
    [user_role]            VARCHAR (50) NOT NULL,
    [allow_multiple_login] BIT          NOT NULL,
    [enterprise_only]      BIT          NOT NULL,
    CONSTRAINT [PK_user_roles] PRIMARY KEY CLUSTERED ([user_role_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'user_roles'