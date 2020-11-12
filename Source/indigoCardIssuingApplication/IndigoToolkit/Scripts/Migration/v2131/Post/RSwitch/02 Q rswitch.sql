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