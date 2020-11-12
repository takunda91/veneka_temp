USE [DB_NAME]

EXEC [dbo].[sp_open_keys]

SELECT [cards].[card_id], 
	   [dbo].[fn_decrypt_value]([cards].[card_number], DEFAULT) AS [card_number],
	   [branch].[branch_name], 
	   [product].[product_name]
FROM [dbo].[cards]
	LEFT JOIN [dbo].[issuer_product] [product]
		ON [cards].[product_id] = [product].[product_id]
	LEFT JOIN [dbo].[branch]
		ON [cards].[branch_id] = [branch].[branch_id]
WHERE [product].[issuer_id] = @selected_issuer_id

EXEC [dbo].[sp_close_keys]