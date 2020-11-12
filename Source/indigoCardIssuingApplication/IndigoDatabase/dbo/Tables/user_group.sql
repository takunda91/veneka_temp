CREATE TABLE [dbo].[user_group] (
    [user_group_id]     INT          IDENTITY (1, 1) NOT NULL,
    [user_role_id]      INT          NOT NULL,
    [issuer_id]         INT          NOT NULL,
    [can_create]        BIT          CONSTRAINT [DF_user_group_can_create] DEFAULT ((0)) NOT NULL,
    [can_read]          BIT          CONSTRAINT [DF_user_group_can_read] DEFAULT ((0)) NOT NULL,
    [can_update]        BIT          CONSTRAINT [DF_user_group_can_update] DEFAULT ((0)) NOT NULL,
    [can_delete]        BIT          CONSTRAINT [DF_user_group_can_delete] DEFAULT ((0)) NOT NULL,
    [all_branch_access] BIT          CONSTRAINT [DF_user_group_all_branch_access] DEFAULT ((0)) NOT NULL,
    [user_group_name]   VARCHAR (50) NOT NULL,
    [mask_screen_pan]   BIT          NOT NULL,
    [mask_report_pan]   BIT          NOT NULL,
    CONSTRAINT [PK_user_group] PRIMARY KEY CLUSTERED ([user_group_id] ASC),
    CONSTRAINT [FK_user_group_issuer] FOREIGN KEY ([issuer_id]) REFERENCES [dbo].[issuer] ([issuer_id]),
    CONSTRAINT [FK_user_group_user_roles] FOREIGN KEY ([user_role_id]) REFERENCES [dbo].[user_roles] ([user_role_id])
);

