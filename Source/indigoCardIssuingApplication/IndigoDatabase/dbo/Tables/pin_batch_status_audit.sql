CREATE TABLE [dbo].[pin_batch_status_audit] (
    [pin_batch_status_id]   BIGINT        IDENTITY (1, 1) NOT NULL,
    [pin_batch_id]          BIGINT        NOT NULL,
    [pin_batch_statuses_id] INT           NOT NULL,
    [user_id]               BIGINT        NOT NULL,
    [status_date]           DATETIMEOFFSET      NOT NULL,
    [status_notes]          VARCHAR (250) NULL,
    CONSTRAINT [PK_pin_batch_status_audit] PRIMARY KEY CLUSTERED ([pin_batch_status_id] ASC)
);

