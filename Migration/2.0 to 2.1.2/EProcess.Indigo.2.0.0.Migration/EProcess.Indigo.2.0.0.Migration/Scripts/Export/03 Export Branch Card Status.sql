USE [DB_NAME]

DECLARE @Start INT,
	    @End INT,
		@total_count INT,
		@count INT,
		@index INT = 0,
		@rows INT = 1000

SELECT @Start = 0, 
	   @End = @rows;


SELECT @total_count = COUNT([card_status].[branch_card_status_id])
FROM [indigo_database_group].[dbo].[branch_card_status] [card_status]
	LEFT JOIN [indigo_database_group].[dbo].[cards] [cards]
		ON [card_status].[card_id] = [cards].[card_id]
	       LEFT JOIN [indigo_database_group].[dbo].[issuer_product] [product]
		       ON [cards].[product_id] = [product].[product_id]
WHERE [product].[issuer_id] = @selected_issuer_id


SET @count = @total_count / @rows


SET IDENTITY_INSERT [dbo].[branch_card_status] ON

WHILE @index <= @count
BEGIN

	IF @index <> 0
	BEGIN
		SET @Start = @index * @rows
		SET @end = @start + @rows
	END
	

	;WITH [branch_card_status_list] AS 
	 ( SELECT [card_status].[branch_card_status_id], [card_status].[card_id], [card_status].[branch_card_statuses_id], [card_status].[status_date], [card_status].[user_id], [card_status].[operator_user_id], [card_status].[branch_card_code_id], [card_status].[comments]
	   ,ROW_NUMBER() OVER (ORDER BY [card_status].[branch_card_status_id]) AS RowNumber
	   FROM [indigo_database_group].[dbo].[branch_card_status] [card_status]
	   LEFT JOIN [indigo_database_group].[dbo].[cards] [cards]
	       ON [card_status].[card_id] = [cards].[card_id]
		       LEFT JOIN [indigo_database_group].[dbo].[issuer_product] [product]
			       ON [cards].[product_id] = [product].[product_id]
	   WHERE [product].[issuer_id] = @selected_issuer_id 
	   GROUP BY [card_status].[branch_card_status_id], [card_status].[card_id], [card_status].[branch_card_statuses_id], [card_status].[status_date], [card_status].[user_id], [card_status].[operator_user_id], [card_status].[branch_card_code_id], [card_status].[comments]
	 )	

	 INSERT INTO [dbo].[branch_card_status]
           ([branch_card_status_id]
		   ,[card_id]
           ,[branch_card_statuses_id]
           ,[status_date]
           ,[user_id]
           ,[operator_user_id]
           ,[branch_card_code_id]
           ,[comments]
           ,[pin_auth_user_id])

	SELECT [branch_card_status_id]
		  ,[card_id]
		  ,[branch_card_statuses_id]
		  ,[status_date]
		  ,[user_id]
		  ,[operator_user_id]
		  ,[branch_card_code_id]
		  ,[comments]
		  ,NULL
	FROM [branch_card_status_list]
	WHERE RowNumber > @Start AND RowNumber <= @End
	ORDER BY [branch_card_status_id]


	SET @index = @index + 1
END

SET IDENTITY_INSERT [dbo].[branch_card_status] OFF