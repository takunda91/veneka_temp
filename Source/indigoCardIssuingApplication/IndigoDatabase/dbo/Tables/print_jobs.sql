CREATE TABLE [dbo].[print_jobs] (
    [print_job_id]      BIGINT             IDENTITY (1, 1) NOT NULL,
    [printer_id]        BIGINT             NOT NULL,
    [status_date]       DATETIMEOFFSET (7) NOT NULL,
    [print_statuses_id] INT                NOT NULL,
    [comments]          NVARCHAR (100)     NULL,
    [audit_user_id]     INT                NOT NULL,
    CONSTRAINT [PK__print_jo__63B34ED900F41754] PRIMARY KEY CLUSTERED ([print_job_id] ASC),
    CONSTRAINT [FK_print_jobs_printer] FOREIGN KEY ([printer_id]) REFERENCES [dbo].[printer] ([printer_id])
);


GO
ALTER TABLE [dbo].[print_jobs] NOCHECK CONSTRAINT [FK_print_jobs_printer];


