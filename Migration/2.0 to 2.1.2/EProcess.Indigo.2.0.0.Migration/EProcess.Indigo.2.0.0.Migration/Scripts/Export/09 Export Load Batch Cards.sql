USE [DB_NAME]

DECLARE @Start INT,
	    @End INT,
		@total_count INT,
		@count INT,
		@index INT = 0,
		@rows INT = 1000

SELECT @Start = 0, 
	   @End = @rows;


SELECT @total_count = COUNT([load_cards].[load_batch_id])
FROM [indigo_database_group].[dbo].[load_batch_cards] [load_cards]
	LEFT JOIN [indigo_database_group].[dbo].[load_batch] [load_batch]
		ON [load_cards].[load_batch_id] = [load_batch].[load_batch_id]
	LEFT JOIN [indigo_database_group].[dbo].[file_history] [history]
		ON [load_batch].[file_id] = [history].[file_id]
WHERE [history].[issuer_id] = @selected_issuer_id 

SET @count = @total_count / @rows

WHILE @index <= @count
BEGIN

	IF @index <> 0
	BEGIN
		SET @Start = @index * @rows
		SET @end = @start + @rows
	END

	
	;WITH [load_batch_cards_list] AS 
	 (  SELECT [load_cards].[load_batch_id], [load_cards].[card_id], [load_cards].[load_card_status_id]
	   ,ROW_NUMBER() OVER (ORDER BY [load_cards].[load_batch_id]) AS RowNumber
	  
		FROM [indigo_database_group].[dbo].[load_batch_cards] [load_cards]
			LEFT JOIN [indigo_database_group].[dbo].[load_batch] [load_batch]
				ON [load_cards].[load_batch_id] = [load_batch].[load_batch_id]
			LEFT JOIN [indigo_database_group].[dbo].[file_history] [history]
				ON [load_batch].[file_id] = [history].[file_id]
		WHERE [history].[issuer_id] = @selected_issuer_id 
		GROUP BY [load_cards].[load_batch_id], [load_cards].[card_id], [load_cards].[load_card_status_id]
	 )

	INSERT INTO [dbo].[load_batch_cards]
			   ([load_batch_id]
			   ,[card_id]
			   ,[load_card_status_id])
	SELECT [load_batch_id]
		  ,[card_id]
		  ,[load_card_status_id]
	  FROM load_batch_cards_list
	WHERE RowNumber > @Start AND RowNumber <= @End
	ORDER BY [load_batch_id]


	SET @index = @index + 1
END

EXEC [indigo_database_group].[dbo].[sp_close_keys]
EXEC [dbo].[sp_close_keys]