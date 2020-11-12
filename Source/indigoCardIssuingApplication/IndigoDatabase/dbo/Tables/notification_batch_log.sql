CREATE TABLE [dbo].[notification_batch_log] (
    [added_time]             DATETIMEOFFSET        NOT NULL,
    [issuer_id]              INT             NOT NULL,
    [dist_batch_id]          INT             NOT NULL,
    [dist_batch_statuses_id] INT             NOT NULL,
    [user_id]                BIGINT          NOT NULL,
    [channel_id]             INT             NOT NULL,
    [notification_text]      VARBINARY (MAX) NOT NULL
);

