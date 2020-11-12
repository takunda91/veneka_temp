CREATE TABLE [dbo].[branch_card_status] (    
    [card_id]                 BIGINT         NOT NULL,
    [branch_card_statuses_id] INT            NOT NULL,
    [status_date]             DATETIMEOFFSET       NOT NULL,
    [user_id]                 BIGINT         NOT NULL,
    [operator_user_id]        BIGINT         NULL,
    [branch_card_code_id]     INT            NULL,
    [comments]                VARCHAR (1000) NULL,
    [pin_auth_user_id]        BIGINT         NULL,
    [branch_id]               INT            NOT NULL,
    CONSTRAINT [PK_branch_card_status] PRIMARY KEY CLUSTERED ([card_id] ASC),
    CONSTRAINT [FK_branch_card_status_branch_card_codes] FOREIGN KEY ([branch_card_code_id]) REFERENCES [dbo].[branch_card_codes] ([branch_card_code_id]),
    CONSTRAINT [FK_branch_card_status_branch_card_statuses] FOREIGN KEY ([branch_card_statuses_id]) REFERENCES [dbo].[branch_card_statuses] ([branch_card_statuses_id]),
    CONSTRAINT [FK_branch_card_status_branch_id] FOREIGN KEY ([branch_id]) REFERENCES [dbo].[branch] ([branch_id]),
    CONSTRAINT [FK_branch_card_status_cards] FOREIGN KEY ([card_id]) REFERENCES [dbo].[cards] ([card_id]),
    CONSTRAINT [FK_branch_card_status_user] FOREIGN KEY ([operator_user_id]) REFERENCES [dbo].[user] ([user_id]),
    CONSTRAINT [FK_branch_card_status_user1] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user] ([user_id])
);


GO
CREATE NONCLUSTERED INDEX [INDEX_CARD_STATUS_DATE]
    ON [dbo].[branch_card_status]([card_id] ASC)
    INCLUDE([status_date]);


GO
CREATE NONCLUSTERED INDEX [INDEX_CARD_OPERATOR]
    ON [dbo].[branch_card_status]([branch_card_statuses_id] ASC, [status_date] ASC)
    INCLUDE([card_id], [operator_user_id]);

