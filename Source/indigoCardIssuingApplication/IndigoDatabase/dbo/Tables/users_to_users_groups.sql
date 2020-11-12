CREATE TABLE [dbo].[users_to_users_groups] (
    [user_id]       BIGINT NOT NULL,
    [user_group_id] INT    NOT NULL,
    CONSTRAINT [FK_users_to_users_groups_application_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user] ([user_id]),
    CONSTRAINT [FK_users_to_users_groups_user_group] FOREIGN KEY ([user_group_id]) REFERENCES [dbo].[user_group] ([user_group_id]), 
    CONSTRAINT [PK_users_to_users_groups] PRIMARY KEY ([user_id], [user_group_id])
);

