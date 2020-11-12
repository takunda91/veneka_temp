USE [{DATABASE_NAME}]
GO
-- Allow all account type linking
INSERT [dbo].[products_account_types] ([product_id], [account_type_id])
SELECT [product_id], [account_type_id]
FROM [dbo].[issuer_product], [dbo].[customer_account_type]
ORDER BY [product_id], [account_type_id]
GO

-- Allow all card issuing reasons
INSERT INTO [dbo].[product_issue_reason] ([product_id], [card_issue_reason_id])
SELECT [product_id], [card_issue_reason_id]
FROM [dbo].[issuer_product], [dbo].[card_issue_reason]
ORDER BY [product_id], [card_issue_reason_id]
GO


-- Set fee charged status based on reference number
--TODO
SELECT *
FROM [cards] INNER JOIN [branch_card_status_current]
	ON [cards].card_id = [branch_card_status_current].card_id
		AND [branch_card_status_current].branch_card_statuses_id = 6
WHERE [fee_reference_number] != NULL

DECLARE @interface_areas TABLE ([interface_area_id] int not null)
INSERT INTO @interface_areas ([interface_area_id]) VALUES (0),(1)
-- Set issuer interface
--HSM
--INSERT INTO [dbo].[issuer_interface]
--           ([interface_type_id]
--           ,[issuer_id]
--           ,[connection_parameter_id]
--           ,[interface_area]
--           ,[interface_guid])
SELECT 4 as [interface_type_id]
      ,[product_id]
      ,[connection_parameter_id]
      ,0 as [interface_area]
      ,'8DC86A77-BCC7-45C4-8C37-71D7982D6543' as [interface_guid]
FROM [dbo].[issuer] INNER JOIN [dbo].[connection_parameters]
	ON [dbo].[issuer].[cards_file_location] = [dbo].[connection_parameters].[path]
		INNER JOIN [dbo].[issuer_product]
	ON [dbo].[issuer_product].[issuer_id] = [dbo].[issuer].[issuer_id]

-- Set product interfaces
--CPS linking
--INSERT INTO [dbo].[product_interface]
--           ([interface_type_id]
--           ,[product_id]
--           ,[connection_parameter_id]
--           ,[interface_area]
--           ,[interface_guid])
SELECT 3 as [interface_type_id]
      ,[product_id]
      ,[connection_parameter_id]
      ,0 as [interface_area]
      ,'01D78A39-465A-41FD-B27D-907A57B5AA2C' as [interface_guid]
FROM [dbo].[issuer] INNER JOIN [dbo].[connection_parameters]
	ON [dbo].[connection_parameters].connection_name = 'Bankworld'
		INNER JOIN [dbo].[issuer_product]
	ON [dbo].[issuer_product].[issuer_id] = [dbo].[issuer].[issuer_id]

--NS linking
--INSERT INTO [dbo].[product_interface]
--           ([interface_type_id]
--           ,[product_id]
--           ,[connection_parameter_id]
--           ,[interface_area]
--           ,[interface_guid])
--SELECT 7 as [interface_type_id]
--      ,[product_id]
--      ,[connection_parameter_id]
--      ,[interface_area].[interface_area_id] as [interface_area]
--      ,'D163EDB8-3302-4E37-8DA7-2E427AD22072' as [interface_guid]
--FROM [{SOURCE_DATABASE_NAME}].[dbo].[issuer] INNER JOIN [dbo].[connection_parameters]
--	ON [{SOURCE_DATABASE_NAME}].[dbo].[issuer].[cards_file_location] = [dbo].[connection_parameters].[path]
--		INNER JOIN [dbo].[issuer_product]
--	ON [dbo].[issuer_product].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[issuer].[issuer_id]

--Core Banking linking
INSERT INTO [dbo].[product_interface]
           ([interface_type_id]
           ,[product_id]
           ,[connection_parameter_id]
           ,[interface_area]
           ,[interface_guid])
SELECT 0 as [interface_type_id]
      ,[product_id]
      ,[dbo].[connection_parameters].[connection_parameter_id]
      ,[interface_area].[interface_area_id] as [interface_area]
      ,'AFF650BD-3DF7-49C8-B858-699338F79F33' as [interface_guid]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[issuer] INNER JOIN [{SOURCE_DATABASE_NAME}].[dbo].[issuer_interface]
		ON [dbo].[issuer].[issuer_id] = [dbo].[issuer_interface].[issuer_id]
			AND [dbo].[issuer_interface].[interface_type_id] = 0
	INNER JOIN [{SOURCE_DATABASE_NAME}].[dbo].[connection_parameters]
		ON [dbo].[connection_parameters].[connection_parameter_id] = [dbo].[issuer_interface].[connection_parameter_id]
	INNER JOIN [{SOURCE_DATABASE_NAME}].[dbo].[issuer_product]
		ON [dbo].[issuer_product].[issuer_id] = [dbo].[issuer].[issuer_id],
	@interface_areas as [interface_area]
	
--CMS linking
INSERT INTO [dbo].[product_interface]
           ([interface_type_id]
           ,[product_id]
           ,[connection_parameter_id]
           ,[interface_area]
           ,[interface_guid])
SELECT 1 as [interface_type_id]
      ,[product_id]
      ,[dbo].[connection_parameters].[connection_parameter_id]
      ,[interface_area].[interface_area_id] as [interface_area]
      ,'9B424A7D-D2C9-4D4E-8317-870C8889D7CF' as [interface_guid]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[issuer] INNER JOIN [{SOURCE_DATABASE_NAME}].[dbo].[issuer_interface]
		ON [{SOURCE_DATABASE_NAME}].[dbo].[issuer].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[issuer_interface].[issuer_id]
			AND [{SOURCE_DATABASE_NAME}].[dbo].[issuer_interface].[interface_type_id] = 1
	INNER JOIN [dbo].[connection_parameters]
		ON [dbo].[connection_parameters].[connection_parameter_id] = [{SOURCE_DATABASE_NAME}].[dbo].[issuer_interface].[connection_parameter_id]
	INNER JOIN [dbo].[issuer_product]
		ON [dbo].[issuer_product].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[issuer].[issuer_id],
	@interface_areas as [interface_area]


--User AD linking
UPDATE [new_user]
SET [new_user].[connection_parameter_id] = [dbo].[connection_parameters].[connection_parameter_id],
	[new_user].[workstation] = NULL
FROM [dbo].[user] as [new_user] INNER JOIN [dbo].[connection_parameters]
	ON [new_user].[workstation] = [dbo].[connection_parameters].[connection_name]