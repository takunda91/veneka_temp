CREATE TABLE [dbo].[export_batch_status_audit] (
    [export_batch_status_id]   BIGINT        IDENTITY (1, 1) NOT NULL,
    [export_batch_id]          BIGINT        NOT NULL,
    [export_batch_statuses_id] INT           NOT NULL,
    [user_id]                  BIGINT        NOT NULL,
    [status_date]              DATETIMEOFFSET NOT NULL,
    [comments]                 VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_export_batch_status_audit] PRIMARY KEY CLUSTERED ([export_batch_status_id] ASC)
);

