CREATE TABLE [dbo].[user_groups_branches] (
    [user_group_id] INT NOT NULL,
    [branch_id]     INT NOT NULL,
    CONSTRAINT [FK_user_groups_branches_branch] FOREIGN KEY ([branch_id]) REFERENCES [dbo].[branch] ([branch_id]),
    CONSTRAINT [FK_user_groups_branches_user_group] FOREIGN KEY ([user_group_id]) REFERENCES [dbo].[user_group] ([user_group_id]), 
    CONSTRAINT [PK_user_groups_branches] PRIMARY KEY ([user_group_id], [branch_id])
);

