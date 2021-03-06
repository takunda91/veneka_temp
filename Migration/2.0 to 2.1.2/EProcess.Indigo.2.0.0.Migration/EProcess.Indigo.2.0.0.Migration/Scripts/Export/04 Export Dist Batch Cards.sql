USE [DB_NAME]

DECLARE @Start INT,
	    @End INT,
		@total_count INT,
		@count INT,
		@index INT = 0,
		@rows INT = 1000

SELECT @Start = 0, 
	   @End = @rows;


SELECT @total_count = COUNT([d_batch_cards].[dist_batch_id])
FROM [indigo_database_group].[dbo].[dist_batch_cards] [d_batch_cards]
	LEFT JOIN [indigo_database_group].[dbo].[cards] [cards]
		ON [d_batch_cards].[card_id] = [cards].[card_id]
	       LEFT JOIN [indigo_database_group].[dbo].[issuer_product] [product]
		       ON [cards].[product_id] = [product].[product_id]
WHERE [product].[issuer_id] = @selected_issuer_id


SET @count = @total_count / @rows

WHILE @index <= @count
BEGIN

	IF @index <> 0
	BEGIN
		SET @Start = @index * @rows
		SET @end = @start + @rows
	END

	
	;WITH [dist_batch_cards_list] AS 
	 ( SELECT [d_batch_cards].[dist_batch_id], [d_batch_cards].[card_id], [d_batch_cards].[dist_card_status_id]
	   ,ROW_NUMBER() OVER (ORDER BY [d_batch_cards].[dist_batch_id]) AS RowNumber
	   FROM [indigo_database_group].[dbo].[dist_batch_cards] [d_batch_cards]
	       LEFT JOIN [indigo_database_group].[dbo].[cards] [cards]
		       ON [d_batch_cards].[card_id] = [cards].[card_id]
	       LEFT JOIN [indigo_database_group].[dbo].[issuer_product] [product]
		       ON [product].[product_id] = [cards].[product_id]
	   WHERE [product].[issuer_id] = @selected_issuer_id
	   GROUP BY [d_batch_cards].[dist_batch_id], [d_batch_cards].[card_id], [d_batch_cards].[dist_card_status_id]
	 )

	INSERT INTO [dbo].[dist_batch_cards]
			   ([dist_batch_id]
			   ,[card_id]
			   ,[dist_card_status_id])
	SELECT [dist_batch_id]
		  ,[card_id]
		  ,[dist_card_status_id]
	FROM [dist_batch_cards_list]
	WHERE RowNumber > @Start AND RowNumber <= @End
	ORDER BY [dist_batch_id]


	SET @index = @index + 1
END

EXEC [indigo_database_group].[dbo].[sp_close_keys]
EXEC [dbo].[sp_close_keys]