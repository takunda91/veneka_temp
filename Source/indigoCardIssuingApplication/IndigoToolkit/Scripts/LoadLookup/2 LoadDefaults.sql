USE [{DATABASE_NAME}]
GO

declare @objid int
SET @objid = object_id('cards')
EXEC	[dbo].[AddMacForTable]
		@Table_id = @objid
GO

declare @objid int
SET @objid = object_id('user')
EXEC	[dbo].[AddMacForTable]
		@Table_id = @objid
GO