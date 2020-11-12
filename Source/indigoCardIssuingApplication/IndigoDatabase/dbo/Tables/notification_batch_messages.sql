CREATE TABLE [dbo].[notification_batch_messages] (
    [issuer_id]              INT           NOT NULL,
    [dist_batch_type_id]     INT           NOT NULL,
    [dist_batch_statuses_id] INT           NOT NULL,
    [language_id]            INT           NOT NULL,
    [channel_id]             INT           NOT NULL,
    [notification_text]      VARCHAR (MAX) NOT NULL,
    [subject_text]           VARCHAR (MAX) NOT NULL,
    [from_address] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [PK_notification_messages] PRIMARY KEY NONCLUSTERED ([issuer_id] ASC, [dist_batch_statuses_id] ASC, [language_id] ASC, [channel_id] ASC),
    CONSTRAINT [FK_dist_batch_statuses] FOREIGN KEY ([dist_batch_statuses_id]) REFERENCES [dbo].[dist_batch_statuses] ([dist_batch_statuses_id]),
    CONSTRAINT [FK_issuer_id] FOREIGN KEY ([issuer_id]) REFERENCES [dbo].[issuer] ([issuer_id]),
    CONSTRAINT [FK_languages] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]),
    CONSTRAINT [FK_notification_batch_messages_dist_batch_type_id] FOREIGN KEY ([dist_batch_type_id]) REFERENCES [dbo].[dist_batch_type] ([dist_batch_type_id])
);

