CREATE TABLE [dbo].[threed_secure_batch_status]
(
	[threed_batch_id] BIGINT NOT NULL , 
    [threed_batch_statuses_id] INT NOT NULL, 
    [user_id] BIGINT NOT NULL, 
    [status_date] DATETIMEOFFSET NOT NULL, 
    [status_note] NVARCHAR(200) NOT NULL, 
    CONSTRAINT [PK_threed_secure_batch_status] PRIMARY KEY ([threed_batch_id]), 
    CONSTRAINT [FK_threed_secure_batch_status_to_batch] FOREIGN KEY ([threed_batch_id]) REFERENCES [threed_secure_batch]([threed_batch_id]), 
    CONSTRAINT [FK_threed_secure_batch_status_to_statuses] FOREIGN KEY ([threed_batch_statuses_id]) REFERENCES [threed_secure_batch_statuses]([threed_batch_statuses_id]), 
    CONSTRAINT [FK_threed_secure_batch_status_to_user] FOREIGN KEY ([user_id]) REFERENCES [user]([user_id]) 
)
