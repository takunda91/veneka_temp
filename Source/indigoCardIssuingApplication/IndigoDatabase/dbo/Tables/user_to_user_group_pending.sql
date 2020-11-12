CREATE TABLE [dbo].[user_to_user_group_pending] (
    [pending_user_id] BIGINT NOT NULL,
    [user_group_id]   INT    NOT NULL,
    CONSTRAINT [FK_users_to_users_groups_user_group_pending] FOREIGN KEY ([user_group_id]) REFERENCES [dbo].[user_group] ([user_group_id]), 
    CONSTRAINT [PK_user_to_user_group_pending] PRIMARY KEY ([pending_user_id], [user_group_id])
);



