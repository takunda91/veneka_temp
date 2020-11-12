CREATE TABLE [dbo].[dist_batch_status_audit] (
    [dist_batch_status_id]   BIGINT        IDENTITY (1, 1) NOT NULL,
    [dist_batch_id]          BIGINT        NOT NULL,
    [dist_batch_statuses_id] INT           NOT NULL,
    [user_id]                BIGINT        NOT NULL,
    [status_date]            DATETIMEOFFSET      NOT NULL,
    [status_notes]           VARCHAR (150) NULL,
    CONSTRAINT [PK_dist_batch_status_audit] PRIMARY KEY CLUSTERED ([dist_batch_status_id] ASC)
);

