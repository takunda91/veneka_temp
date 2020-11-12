CREATE TABLE [dbo].[threed_secure_batch_status_audit]
(
	[threed_batch_id] BIGINT NOT NULL , 
    [threed_batch_statuses_id] INT NOT NULL, 
    [user_id] BIGINT NOT NULL, 
    [status_date] DATETIMEOFFSET NOT NULL, 
    [status_note] VARCHAR(150) NOT NULL 
)
