CREATE TABLE [dbo].[notification_branch_outbox] (
    [branch_message_id]       UNIQUEIDENTIFIER NOT NULL,
    [added_time]              DATETIMEOFFSET         NOT NULL,
    [card_id]                 BIGINT           NOT NULL,
    [issuer_id]               INT              NOT NULL,
    [branch_card_statuses_id] INT              NOT NULL,
    [card_issue_method_id]    INT              NOT NULL,
    [language_id]             INT              NOT NULL,
    [channel_id]              INT              NOT NULL
);

