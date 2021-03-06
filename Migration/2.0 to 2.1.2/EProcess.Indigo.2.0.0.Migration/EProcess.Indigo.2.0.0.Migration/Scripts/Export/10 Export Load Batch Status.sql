USE [DB_NAME]

DECLARE @Start INT,
	    @End INT,
		@total_count INT,
		@count INT,
		@index INT = 0,
		@rows INT = 1000

SELECT @Start = 0, 
	   @End = @rows;


SELECT @total_count = COUNT([load_status].[load_batch_status_id])
FROM [indigo_database_group].[dbo].[load_batch_status] [load_status]
LEFT JOIN [indigo_database_group].[dbo].[load_batch] [load_batch]
	ON [load_status].[load_batch_id] = [load_batch].[load_batch_id]
LEFT JOIN [indigo_database_group].[dbo].[file_history] [history]
	ON [load_batch].[file_id] = [history].[file_id]
WHERE [history].[issuer_id] = @selected_issuer_id


SET @count = @total_count / @rows

SET IDENTITY_INSERT [dbo].[load_batch_status] ON

WHILE @index <= @count
BEGIN

	IF @index <> 0
	BEGIN
		SET @Start = @index * @rows
		SET @end = @start + @rows
	END

	
	;WITH [load_status_list] AS 
	 (  
		SELECT [load_status].[load_batch_status_id], [load_status].[load_batch_id], [load_status].[load_batch_statuses_id], [load_status].[user_id], [load_status].[status_date],[load_status].[status_notes]
	   ,ROW_NUMBER() OVER (ORDER BY [load_status].[load_batch_status_id]) AS RowNumber
		FROM [indigo_database_group].[dbo].[load_batch_status] [load_status]
		LEFT JOIN [indigo_database_group].[dbo].[load_batch] [load_batch]
			ON [load_status].[load_batch_id] = [load_batch].[load_batch_id]
		LEFT JOIN [indigo_database_group].[dbo].[file_history] [history]
			ON [load_batch].[file_id] = [history].[file_id]
		WHERE [history].[issuer_id] = @selected_issuer_id
		GROUP BY [load_status].[load_batch_status_id], [load_status].[load_batch_id], [load_status].[load_batch_statuses_id], [load_status].[user_id], [load_status].[status_date], [load_status].[status_notes]
	)

	INSERT INTO [dbo].[load_batch_status]
           ([load_batch_status_id]
		   ,[load_batch_id]
           ,[load_batch_statuses_id]
           ,[user_id]
           ,[status_date]
           ,[status_notes])

	SELECT [load_batch_status_id]
		  ,[load_batch_id]
		  ,[load_batch_statuses_id]
		  ,[user_id]
		  ,[status_date]
		  ,[status_notes]
	FROM [load_status_list]
	WHERE RowNumber > @Start AND RowNumber <= @End
	ORDER BY [load_batch_id]


	SET @index = @index + 1
END

SET IDENTITY_INSERT [dbo].[load_batch_status] OFF