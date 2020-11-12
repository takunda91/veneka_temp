CREATE TABLE [dbo].[remote_update_status_audit]
(
	[remote_update_status_id] INT NOT NULL  IDENTITY, 
    [remote_update_statuses_id] INT NOT NULL, 
    [card_id] BIGINT NOT NULL, 
    [status_date] DATETIMEOFFSET NOT NULL, 
    [comments] NVARCHAR(MAX) NOT NULL, 
    [remote_component] VARCHAR(250) NOT NULL, 
    [user_id] BIGINT NOT NULL, 
    [remote_updated_time] DATETIMEOFFSET NULL, 
    CONSTRAINT [PK_remote_update_status_audit] PRIMARY KEY ([remote_update_status_id])
)
