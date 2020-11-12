USE [DB_NAME]

EXEC [indigo_database_group].[dbo].[sp_open_keys]
EXEC [dbo].[sp_open_keys]

DECLARE @objid INT

SET @objid = object_id('[indigo_database_group].[dbo].[cards]')
DECLARE @key varbinary(100) = null
SELECT @key = CONVERT(VARBINARY(MAX), [indigo_database_group].[dbo].[fn_decrypt_value](mac_key, 'cert_ProtectIndexingKeys'))
FROM [indigo_database_group].[dbo].[mac_index_keys]
WHERE table_id = @objid


DECLARE @Start INT,
	    @End INT,
		@total_count INT,
		@count INT,
		@index INT = 0,
		@rows INT = 1000

SELECT @Start = 0, 
	   @End = @rows;


SELECT @total_count = COUNT([cards].[card_id])
FROM [indigo_database_group].[dbo].[cards] [cards]
	LEFT JOIN [indigo_database_group].[dbo].[issuer_product] [product]
		ON [cards].[product_id] = [product].[product_id]
WHERE [product].[issuer_id] = @selected_issuer_id


SET @count = @total_count / @rows

SET IDENTITY_INSERT [dbo].[cards] ON

WHILE @index <= @count
BEGIN

	IF @index <> 0
	BEGIN
		SET @Start = @index * @rows
		SET @end = @start + @rows
	END

	
	;WITH [cards_list] AS 
	 ( 
		SELECT [cards].[card_id], [cards].[product_id], [cards].[branch_id], [cards].[card_number], [cards].[card_sequence], [cards].[card_index]
			,ROW_NUMBER() OVER (ORDER BY [cards].[card_id]) AS RowNumber
	    FROM [indigo_database_group].[dbo].[cards]  [cards]
	        LEFT JOIN [indigo_database_group].[dbo].[issuer_product] [product]
		        ON [cards].[product_id] = [product].[product_id]
	    WHERE [product].[issuer_id] = @selected_issuer_id
	    GROUP BY [cards].[card_id], [cards].[product_id], [cards].[branch_id], [cards].[card_number], [cards].[card_sequence], [cards].[card_index]
	 )

	 
	INSERT INTO [dbo].[cards]
           ([card_id]
		   ,[product_id]
           ,[branch_id]
           ,[card_number]
           ,[card_sequence]
           ,[card_index]
           ,[card_issue_method_id]
           ,[card_priority_id]
           ,[card_request_reference]
           ,[card_production_date]
           ,[card_expiry_date]
           ,[card_activation_date]
           ,[pvv]
           ,[fee_charged]
           ,[fee_waiver_YN]
           ,[fee_editable_YN]
           ,[fee_overridden_YN]
           ,[fee_reference_number]
           ,[fee_reversal_ref_number]
           ,[origin_branch_id]
           ,[export_batch_id])

	 SELECT [card_id]
		  ,[product_id]
		  ,[branch_id]
		  ,[dbo].[fn_encrypt_value]([indigo_database_group].[dbo].[fn_decrypt_value]([card_number], DEFAULT))
		  ,[card_sequence]
		  ,CONVERT(varbinary(24), HashBytes( N'SHA1', CONVERT(varbinary(8000), 
									CONVERT(nvarchar(4000),RIGHT([indigo_database_group].[dbo].[fn_decrypt_value]([card_number], DEFAULT), 4))) + @key )) [card_index]
		  ,1
		  ,1
		  ,[dbo].[fn_encrypt_value]([indigo_database_group].[dbo].[fn_decrypt_value]([card_number], DEFAULT))
		  ,NULL
		  ,NULL
		  ,NULL
		  ,NULL
		  ,NULL
		  ,NULL
		  ,NULL
		  ,NULL
		  ,NULL
		  ,NULL
		  ,[branch_id]
		  ,NULL
	FROM [cards_list]
	WHERE RowNumber > @Start AND RowNumber <= @End
	ORDER BY [card_id]


	SET @index = @index + 1
END
	
SET IDENTITY_INSERT [dbo].[cards] OFF


EXEC [indigo_database_group].[dbo].[sp_close_keys]
EXEC [dbo].[sp_close_keys]