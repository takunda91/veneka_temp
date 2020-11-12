CREATE TABLE [dbo].[dist_batch_status] (    
    [dist_batch_id]          BIGINT        NOT NULL,
    [dist_batch_statuses_id] INT           NOT NULL,
    [user_id]                BIGINT        NOT NULL,
    [status_date]            DATETIMEOFFSET      NOT NULL,
    [status_notes]           VARCHAR (150) NULL,
    CONSTRAINT [PK_dist_batch_status] PRIMARY KEY CLUSTERED ([dist_batch_id] ASC),
    CONSTRAINT [FK_dist_batch_status_dist_batch_statuses] FOREIGN KEY ([dist_batch_statuses_id]) REFERENCES [dbo].[dist_batch_statuses] ([dist_batch_statuses_id]),
    CONSTRAINT [FK_dist_batch_status_distribution_batch] FOREIGN KEY ([dist_batch_id]) REFERENCES [dbo].[dist_batch] ([dist_batch_id]),
    CONSTRAINT [FK_dist_batch_status_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user] ([user_id])
);

