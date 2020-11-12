CREATE TABLE [dbo].[notification_branch_log] (
    [added_time]              DATETIMEOFFSET        NOT NULL,
    [card_id]                 BIGINT          NOT NULL,
    [issuer_id]               INT             NOT NULL,
    [branch_card_statuses_id] INT             NOT NULL,
    [channel_id]              INT             NOT NULL,
    [notification_text]       VARBINARY (MAX) NOT NULL
);

