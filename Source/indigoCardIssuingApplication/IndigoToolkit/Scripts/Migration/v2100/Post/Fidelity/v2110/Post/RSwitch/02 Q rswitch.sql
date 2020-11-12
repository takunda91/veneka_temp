USE [{DATABASE_NAME}]
GO

--Update sequence
INSERT INTO [dbo].[integration_bellid_batch_sequence] ([file_generation_date],[batch_sequence_number])
SELECT [file_generation_date],[batch_sequence_number]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[integration_bellid_batch_sequence]
GO

SET IDENTITY_INSERT [dbo].[rswitch_crf_bank_codes] ON; 

INSERT INTO [dbo].[rswitch_crf_bank_codes] ([bank_id], [issuer_id], [bank_code])
SELECT [bank_id], [issuer_id], [bank_code]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[rswitch_crf_bank_codes]

SET IDENTITY_INSERT [dbo].[rswitch_crf_bank_codes] OFF; 
GO

UPDATE [trg]
SET [trg].[fee_accounting_id] = [src].[fee_accounting_id]
FROM [{DATABASE_NAME}].[dbo].[product_fee_scheme] [trg] INNER JOIN [{DATABASE_NAME}].[dbo].[product_fee_accounting] [src]
		ON [trg].[issuer_id] = [src].[issuer_id]
GO

UPDATE [dbo].[issuer_product]
SET [production_dist_batch_status_flow] = 2,
	[distribution_dist_batch_status_flow] = 5
GO