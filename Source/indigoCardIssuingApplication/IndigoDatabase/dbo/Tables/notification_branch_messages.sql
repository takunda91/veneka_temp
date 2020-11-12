CREATE TABLE [dbo].[notification_branch_messages] (
    [issuer_id]               INT           NOT NULL,
    [branch_card_statuses_id] INT           NOT NULL,
    [card_issue_method_id]    INT           NOT NULL,
    [language_id]             INT           NOT NULL,
    [channel_id]              INT           NOT NULL,
    [notification_text]       VARCHAR (MAX) NOT NULL,
    [subject_text]            VARCHAR (MAX) NOT NULL,
    [from_address] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [PK_notification_branch_messages] PRIMARY KEY NONCLUSTERED ([issuer_id] ASC, [branch_card_statuses_id] ASC, [card_issue_method_id] ASC, [language_id] ASC, [channel_id] ASC),
    CONSTRAINT [FK_notification_branch_messages_branch_card_statuses] FOREIGN KEY ([branch_card_statuses_id]) REFERENCES [dbo].[branch_card_statuses] ([branch_card_statuses_id]),
    CONSTRAINT [FK_notification_branch_messages_card_issue_method] FOREIGN KEY ([card_issue_method_id]) REFERENCES [dbo].[card_issue_method] ([card_issue_method_id]),
    CONSTRAINT [FK_notification_branch_messages_issuer_id] FOREIGN KEY ([issuer_id]) REFERENCES [dbo].[issuer] ([issuer_id]),
    CONSTRAINT [FK_notification_branch_messages_languages] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id])
);

