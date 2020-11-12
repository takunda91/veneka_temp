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

INSERT [dbo].[product_fields] ([product_id], [field_name], [print_field_type_id], [X], [Y], [width], [height], [font], [font_size], [mapped_name], [editable], [deleted], [label], [max_length]) 
SELECT [product_id], N'CashLimit', 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Arial', 10, N'CashLimit', 0, 0, N'', 25
FROM [dbo].[issuer_product]
GO