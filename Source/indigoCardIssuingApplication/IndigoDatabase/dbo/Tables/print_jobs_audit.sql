CREATE TABLE [dbo].[print_jobs_audit]
(
	[print_job_id] BIGINT NOT NULL ,
	[printer_id] BIGINT NOT NULL  ,
	[status_date] DATETIMEOFFSET NOT NULL,	
	[print_statuses_id] INT NOT NULL,
	[comments] NVARCHAR(100),
	[audit_user_id] INT NOT NULL	
	
)
