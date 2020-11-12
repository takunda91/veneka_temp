CREATE TABLE [dbo].[export_batch_status] (    
    [export_batch_id]          BIGINT        NOT NULL,
    [export_batch_statuses_id] INT           NOT NULL,
    [user_id]                  BIGINT        NOT NULL,
    [status_date]              DATETIMEOFFSET NOT NULL,
    [comments]                 VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_export_batch_status] PRIMARY KEY CLUSTERED ([export_batch_id] ASC),
    CONSTRAINT [FK_export_batch_status_export_batch] FOREIGN KEY ([export_batch_id]) REFERENCES [dbo].[export_batch] ([export_batch_id]),
    CONSTRAINT [FK_export_batch_status_export_batch_statuses] FOREIGN KEY ([export_batch_statuses_id]) REFERENCES [dbo].[export_batch_statuses] ([export_batch_statuses_id]),
    CONSTRAINT [FK_export_batch_status_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user] ([user_id])
);

