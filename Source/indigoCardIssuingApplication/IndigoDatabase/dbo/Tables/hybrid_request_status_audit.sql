CREATE TABLE [dbo].[hybrid_request_status_audit]
(
	[hybrid_request_status_id] [int] NOT NULL,
	[request_id] [bigint] NOT NULL,
	[hybrid_request_statuses_id] [int] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[comments] [varchar](1000) NULL,
	[branch_id] [int] NOT NULL,
	[operator_user_id] [bigint] NOT NULL 
)
