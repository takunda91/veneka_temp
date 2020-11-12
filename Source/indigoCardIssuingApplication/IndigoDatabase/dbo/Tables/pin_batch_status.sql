CREATE TABLE [dbo].[pin_batch_status] (    
    [pin_batch_id]          BIGINT        NOT NULL,
    [pin_batch_statuses_id] INT           NOT NULL,
    [user_id]               BIGINT        NOT NULL,
    [status_date]           DATETIMEOFFSET      NOT NULL,
    [status_notes]          VARCHAR (250) NULL,
    CONSTRAINT [PK_pin_batch_status] PRIMARY KEY CLUSTERED ([pin_batch_id] ASC),
    CONSTRAINT [FK_pin_batch_status_pin_batch_status] FOREIGN KEY ([pin_batch_id]) REFERENCES [dbo].[pin_batch] ([pin_batch_id]),
    CONSTRAINT [FK_pin_batch_status_pin_batch_statuses] FOREIGN KEY ([pin_batch_statuses_id]) REFERENCES [dbo].[pin_batch_statuses] ([pin_batch_statuses_id]),
    CONSTRAINT [FK_pin_batch_status_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user] ([user_id])
);

