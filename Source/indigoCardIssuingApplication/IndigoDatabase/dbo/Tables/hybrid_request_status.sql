CREATE TABLE [dbo].[hybrid_request_status]
(
	[hybrid_request_status_id] [int] IDENTITY(1,1) NOT NULL,
	[request_id] [bigint] NOT NULL,
	[hybrid_request_statuses_id] [int] NOT NULL,
	[status_date] [datetimeoffset](7) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[comments] [varchar](1000) NULL,
	[branch_id] [int] NOT NULL,
	[operator_user_id] [bigint] NOT NULL, 
    CONSTRAINT [PK_hybrid_request_status] PRIMARY KEY ([hybrid_request_status_id]), 
    CONSTRAINT [FK_hybrid_request_status_hybrid_request_statuses] FOREIGN KEY (hybrid_request_statuses_id) REFERENCES [hybrid_request_statuses](hybrid_request_statuses_id), 
    CONSTRAINT [FK_hybrid_request_status_users] FOREIGN KEY ([user_id]) REFERENCES [user]([user_id]),
    CONSTRAINT [FK_hybrid_request_status_users_1] FOREIGN KEY ([operator_user_id]) REFERENCES [user]([user_id]), 
    CONSTRAINT [FK_hybrid_request_status_branch] FOREIGN KEY ([branch_id]) REFERENCES [branch]([branch_id]), 
    CONSTRAINT [FK_hybrid_request_status_hybrid_requests] FOREIGN KEY ([request_id]) REFERENCES [hybrid_requests]([request_id])

)
