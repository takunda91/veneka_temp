CREATE TABLE [dbo].[notification_batch_outbox] (
    [batch_message_id]       UNIQUEIDENTIFIER NOT NULL,
    [added_time]             DATETIMEOFFSET         NOT NULL,
    [dist_batch_id]          BIGINT           NOT NULL,
    [issuer_id]              INT              NOT NULL,
    [dist_batch_statuses_id] INT              NOT NULL,
    [dist_batch_type_id]     INT              NOT NULL,
    [language_id]            INT              NOT NULL,
    [channel_id]             INT              NOT NULL
);

