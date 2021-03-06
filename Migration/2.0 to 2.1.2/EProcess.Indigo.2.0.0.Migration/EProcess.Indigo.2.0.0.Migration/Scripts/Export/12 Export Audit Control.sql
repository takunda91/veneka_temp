USE [DB_NAME]


SET IDENTITY_INSERT  [dbo].[audit_control] ON 

DECLARE @Start INT,
	    @End INT,
		@total_count INT,
		@count INT,
		@index INT = 0,
		@rows INT = 1000

SELECT @Start = 0, 
	   @End = @rows;


SELECT @total_count = COUNT([audit_id])
FROM [indigo_database_group].[dbo].[audit_control]

SET @count = @total_count / @rows

WHILE @index <= @count
BEGIN

	IF @index <> 0
	BEGIN
		SET @Start = @index * @rows
		SET @end = @start + @rows
	END

	
	;WITH [audit_control_list] AS 
	 ( SELECT [audit_id], [audit_action_id],[user_id], [audit_date], [workstation_address], [action_description], [issuer_id], [data_changed], [data_before],[data_after]
	   ,ROW_NUMBER() OVER (ORDER BY [audit_id]) AS RowNumber
	   FROM [indigo_database_group].[dbo].[audit_control] 
	   GROUP BY [audit_id], [audit_action_id],[user_id], [audit_date], [workstation_address], [action_description], [issuer_id], [data_changed], [data_before],[data_after]
	 )
  	 INSERT INTO [dbo].[audit_control]
           ([audit_id]
		   ,[audit_action_id]
           ,[user_id]
           ,[audit_date]
           ,[workstation_address]
           ,[action_description]
           ,[issuer_id]
           ,[data_changed]
           ,[data_before]
           ,[data_after])
	SELECT [audit_id]
      ,[audit_action_id]
      ,[user_id]
      ,[audit_date]
      ,[workstation_address]
      ,[action_description]
      ,[issuer_id]
      ,[data_changed]
      ,[data_before]
      ,[data_after]
	 FROM [audit_control_list]
	 WHERE RowNumber > @Start AND RowNumber <= @End

	 ORDER BY [audit_id]


	SET @index = @index + 1
END

EXEC [indigo_database_group].[dbo].[sp_close_keys]
EXEC [dbo].[sp_close_keys]

SET IDENTITY_INSERT  [dbo].[audit_control] OFF