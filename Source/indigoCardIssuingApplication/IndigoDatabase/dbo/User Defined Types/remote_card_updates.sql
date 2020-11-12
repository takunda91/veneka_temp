CREATE TYPE [dbo].[remote_card_updates] AS TABLE
(
	card_id bigint, 
	successful bit,
	comment nvarchar(max),
	time_update  DATETIMEOFFSET (7)	
)
