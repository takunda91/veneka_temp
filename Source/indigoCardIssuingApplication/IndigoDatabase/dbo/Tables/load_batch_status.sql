CREATE TABLE [dbo].[load_batch_status] (    
    [load_batch_id]          BIGINT        NOT NULL,
    [load_batch_statuses_id] INT           NOT NULL,
    [user_id]                BIGINT        NOT NULL,
    [status_date]            DATETIMEOFFSET      NOT NULL,
    [status_notes]           VARCHAR (150) NULL,
    CONSTRAINT [PK_load_batch_status] PRIMARY KEY CLUSTERED ([load_batch_id] ASC),
    CONSTRAINT [FK_load_batch_status_batch_statuses] FOREIGN KEY ([load_batch_statuses_id]) REFERENCES [dbo].[load_batch_statuses] ([load_batch_statuses_id]),
    CONSTRAINT [FK_load_batch_status_load_batch] FOREIGN KEY ([load_batch_id]) REFERENCES [dbo].[load_batch] ([load_batch_id]),
    CONSTRAINT [FK_load_batch_status_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user] ([user_id])
);

