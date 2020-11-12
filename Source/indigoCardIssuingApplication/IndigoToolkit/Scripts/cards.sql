USE [indigo_database_v213]
GO

CREATE PROCEDURE [dbo].[export_cardsData]
AS
BEGIN
	EXEC [indigo_database_group].[dbo].[sp_open_keys]
	EXEC [dbo].[sp_open_keys]

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT [card_id]
			  ,[product_id]
			  ,[branch_id]
			  ,[dbo].[fn_encrypt_value]([indigo_database_group].[dbo].[fn_decrypt_value]([card_number], DEFAULT)) as [card_number]
			  ,[card_sequence]
			  ,CONVERT(varbinary(24), 'HelloPeople') as [card_index]
			  --,CONVERT(varbinary(24), HashBytes( N'SHA1', CONVERT(varbinary(8000), 
					--					CONVERT(nvarchar(4000),RIGHT([indigo_database_group].[dbo].[fn_decrypt_value]([card_number], DEFAULT), 4))) + @key )) [card_index]
			  ,1 as [card_issue_method_id]
			  ,1 as [card_priority_id]
			  ,[indigo_database_group].[dbo].[fn_decrypt_value]([card_number], DEFAULT) as [card_request_reference]
			  ,NULL as [card_production_date]
			  ,NULL as [card_expiry_date]
			  ,NULL as [card_activation_date]
			  ,NULL as [pvv]
			  ,NULL as [fee_charged]
			  ,NULL as [fee_waiver_YN]
			  ,NULL as [fee_editable_YN]
			  ,NULL as [fee_overridden_YN]
			  ,NULL as [fee_reference_number]
			  ,NULL as [fee_reversal_ref_number]
			  ,[branch_id] as [origin_branch_id]
			  ,NULL as [export_batch_id]
			  ,[branch_id] as [ordering_branch_id]
			  ,[branch_id] as [delivery_branch_id]			  
		FROM [indigo_database_group].[dbo].[cards]
	EXEC [indigo_database_group].[dbo].[sp_close_keys]
	EXEC [dbo].[sp_close_keys]

END
GO