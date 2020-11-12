CREATE TABLE [dbo].[remote_update_status]
(	
    [remote_update_statuses_id] INT NOT NULL, 
    [card_id] BIGINT NOT NULL , 
    [status_date] DATETIMEOFFSET NOT NULL, 
    [comments] NVARCHAR(MAX) NOT NULL, 
    [remote_component] VARCHAR(250) NOT NULL, 
    [user_id] BIGINT NOT NULL, 
    [remote_updated_time] DATETIMEOFFSET NULL, 
    CONSTRAINT [FK_remote_update_status_remote_component_statuses] FOREIGN KEY ([remote_update_statuses_id]) REFERENCES [remote_update_statuses]([remote_update_statuses_id]), 
    CONSTRAINT [FK_remote_update_status_cards] FOREIGN KEY ([card_id]) REFERENCES [cards]([card_id]), 
    CONSTRAINT [FK_remote_update_status_user] FOREIGN KEY ([user_id]) REFERENCES [user]([user_id]), 
    CONSTRAINT [PK_remote_update_status] PRIMARY KEY ([card_id])
)
