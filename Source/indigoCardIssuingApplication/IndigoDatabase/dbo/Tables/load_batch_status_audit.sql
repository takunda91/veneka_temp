CREATE TABLE [dbo].[load_batch_status_audit] (
    [load_batch_status_id]   BIGINT        IDENTITY (1, 1) NOT NULL,
    [load_batch_id]          BIGINT        NOT NULL,
    [load_batch_statuses_id] INT           NOT NULL,
    [user_id]                BIGINT        NOT NULL,
    [status_date]            DATETIMEOFFSET      NOT NULL,
    [status_notes]           VARCHAR (150) NULL,
    CONSTRAINT [PK_load_batch_status_audit] PRIMARY KEY CLUSTERED ([load_batch_status_id] ASC)    
);

