EXEC sp_rename 'remote_update_status', 'remote_update_status_audit'
GO


--DROP contraints
ALTER TABLE remote_update_status_audit DROP CONSTRAINT [FK_remote_update_status_cards];
ALTER TABLE remote_update_status_audit DROP CONSTRAINT [FK_remote_update_status_user]; 
ALTER TABLE remote_update_status_audit DROP CONSTRAINT [FK_remote_update_status_remote_component_statuses]; 
GO

--Creat current table
CREATE TABLE [dbo].[remote_update_status]
(	
    [remote_update_statuses_id] INT NOT NULL, 
    [card_id] BIGINT NOT NULL PRIMARY KEY, 
    [status_date] DATETIMEOFFSET NOT NULL, 
    [comments] NVARCHAR(MAX) NOT NULL, 
    [remote_component] VARCHAR(250) NOT NULL, 
    [user_id] BIGINT NOT NULL, 
    [remote_updated_time] DATETIMEOFFSET NULL, 
    CONSTRAINT [FK_remote_update_status_remote_component_statuses] FOREIGN KEY ([remote_update_statuses_id]) REFERENCES [remote_update_statuses]([remote_update_statuses_id]), 
    CONSTRAINT [FK_remote_update_status_cards] FOREIGN KEY ([card_id]) REFERENCES [cards]([card_id]), 
    CONSTRAINT [FK_remote_update_status_user] FOREIGN KEY ([user_id]) REFERENCES [user]([user_id])
);
GO

--Load current records
INSERT INTO [remote_update_status] ([remote_update_statuses_id], [card_id], [status_date], [comments], [remote_component], [user_id], [remote_updated_time])
SELECT [dbo].[remote_update_status_audit].remote_update_statuses_id, [dbo].[remote_update_status_audit].card_id, [dbo].[remote_update_status_audit].status_date, [dbo].[remote_update_status_audit].comments,
		[dbo].[remote_update_status_audit].remote_component, [dbo].[remote_update_status_audit].[user_id], [dbo].[remote_update_status_audit].remote_updated_time
FROM [dbo].[remote_update_status_audit] INNER JOIN
		(SELECT card_id, MAX(status_date) AS MaxDate
		FROM [dbo].[remote_update_status_audit]
		GROUP BY card_id) AS T 
	ON [dbo].[remote_update_status_audit].[card_id] = T.card_id AND [dbo].[remote_update_status_audit].[status_date] = T.MaxDate

GO
-- Delete current status from audit - THIS MUST ONLY BE RUN ONCE
DELETE FROM dbo.[remote_update_status_audit]
WHERE [remote_update_status_id] IN (
SELECT [remote_update_status_id]
FROM [dbo].[remote_update_status_audit] INNER JOIN
		(SELECT card_id, MAX(status_date) AS MaxDate
		FROM [dbo].[remote_update_status_audit]
		GROUP BY card_id) AS T 
	ON [dbo].[remote_update_status_audit].[card_id] = T.card_id AND [dbo].[remote_update_status_audit].[status_date] = T.MaxDate)