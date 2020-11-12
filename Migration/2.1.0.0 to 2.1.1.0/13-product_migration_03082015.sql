USE [indigo_database_main_dev]
GO

--Sub product
ALTER TABLE [issuer_product]
ADD sub_product_id int
GO
ALTER TABLE [issuer_product]
ADD master_product_id int
GO

--SELECT product_id, COUNT(product_id)
--FROM [sub_product] 
--GROUP BY product_id
--HAVING COUNT(product_id) = 1

--Create new products from sub products
INSERT INTO [issuer_product]
([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left]
,[Name_on_card_font_size],[font_id],[DeletedYN],[src1_id],[src2_id],[src3_id],[PVKI],[PVK],[CVKA],[CVKB]
,[expiry_months],[fee_scheme_id],[enable_instant_pin_YN],[min_pin_length],[max_pin_length]
,[enable_instant_pin_reissue_YN],[cms_exportable_YN],[product_load_type_id],[sub_product_code]
,[pin_calc_method_id] ,[file_delete_YN],[auto_approve_batch_YN],[account_validation_YN],[pan_length]
,[pin_mailer_printing_YN],[pin_mailer_reprint_YN], sub_product_id_length, [card_issue_method_id], sub_product_id, master_product_id)
SELECT [sub_product].sub_product_code, [sub_product].sub_product_name, [product_bin_code], [issuer_id],[name_on_card_top],[name_on_card_left]
,[Name_on_card_font_size],[font_id],[DeletedYN],[src1_id],[src2_id],[src3_id],[PVKI],[PVK],[CVKA],[CVKB]
,[expiry_months],[sub_product].[fee_scheme_id],[enable_instant_pin_YN],[min_pin_length],[max_pin_length]
,[enable_instant_pin_reissue_YN],[cms_exportable_YN],[product_load_type_id], RIGHT('000'+CAST(ISNULL([sub_product].sub_product_id,'') AS VARCHAR(3)),[issuer_product].sub_product_id_length)
,[pin_calc_method_id] ,[file_delete_YN],[auto_approve_batch_YN],[account_validation_YN],[pan_length]
,[pin_mailer_printing_YN],[pin_mailer_reprint_YN], 0, [sub_product].card_issue_method_id
,[sub_product].sub_product_id, [sub_product].product_id
FROM [sub_product] INNER JOIN [issuer_product]
		ON [sub_product].product_id = [issuer_product].product_id
GO

ALTER TABLE [cards]
DROP CONSTRAINT SubProductCheck
GO

ALTER TABLE [cards]
DROP CONSTRAINT sub_product_id_fk
GO

--SELECT card_id, product_id, sub_product_id FROM [cards] WHERE sub_product_id IS NOT NULL
--Link cards created with sub products to the new products
SELECT c.card_id, c.product_id, c.sub_product_id, [sub_product].sub_product_code
FROM [cards] c INNER JOIN [issuer_product]
		ON c.product_id = [issuer_product].master_product_id
			AND c.sub_product_id = [issuer_product].sub_product_id
		INNER JOIN [sub_product]
			ON c.sub_product_id = [sub_product].sub_product_id
			AND c.product_id = [sub_product].product_id
WHERE c.sub_product_id IS NOT NULL

UPDATE c
SET c.product_id = [issuer_product].product_id
FROM [cards] c INNER JOIN [issuer_product]
		ON c.product_id = [issuer_product].master_product_id
			AND c.sub_product_id = [issuer_product].sub_product_id
WHERE c.sub_product_id IS NOT NULL
GO

SELECT c.card_id, c.product_id, c.sub_product_id, [issuer_product].product_code
FROM [cards] c INNER JOIN [issuer_product]
		ON c.product_id = [issuer_product].product_id
WHERE c.sub_product_id IS NOT NULL
--Done till here

--Inactivate original product of the sub product
SELECT product_id, COUNT(product_id)
FROM [sub_product] 
GROUP BY product_id

SELECT * FROM [issuer_product] WHERE DeletedYN = 1

SELECT * FROM [issuer_product] WHERE product_id IN (SELECT product_id FROM [issuer_product] WHERE master_product_id IS NOT NULL) 

UPDATE [issuer_product]
SET DeletedYN = 1
WHERE product_id IN (SELECT master_product_id FROM [issuer_product] WHERE master_product_id IS NOT NULL) 
GO

--Remove the sub_product table  
DROP TABLE [sub_product]
DROP INDEX [_dta_index_cards_5_1842105603__K3_K17_K6_K1_2_4_7_8_9] ON [dbo].[cards]
DROP STATISTICS [cards]._dta_stat_1842105603_17_6
DROP STATISTICS [cards]._dta_stat_1842105603_1_3_17
DROP STATISTICS [cards]._dta_stat_1842105603_1_17_6

ALTER TABLE [cards]
DROP COLUMN sub_product_id

ALTER TABLE [issuer_product]
DROP COLUMN sub_product_id_length

--issue method
--INSERT INTO [products_card_issue_methods] (product_id, card_issue_method_id)
--SELECT product_id, card_issue_method_id
--FROM [issuer_product]
--GO

--ALTER TABLE [issuer_product]
--DROP CONSTRAINT FK_product_issue_method
--GO 
--ALTER TABLE [issuer_product]
--DROP COLUMN card_issue_method_id
--GO

--account validation, delete card file, auto create batch
UPDATE p
SET 
	p.account_validation_YN = i.account_validation_YN,
	p.file_delete_YN = i.delete_card_file_YN,
	p.auto_approve_batch_YN = i.auto_create_dist_batch,
	p.pin_mailer_printing_YN = i.pin_mailer_printing_YN,
	p.pin_mailer_reprint_YN = i.pin_mailer_reprint_YN
FROM [issuer_product] p
	INNER JOIN [issuer] i
		ON p.issuer_id = i.issuer_id
GO
ALTER TABLE [issuer]
DROP COLUMN account_validation_YN
GO
ALTER TABLE [issuer]
DROP COLUMN delete_card_file_YN
GO
ALTER TABLE [issuer]
DROP COLUMN auto_create_dist_batch
GO
ALTER TABLE [issuer]
DROP COLUMN pin_mailer_printing_YN
GO
ALTER TABLE [issuer]
DROP COLUMN pin_mailer_reprint_YN
GO

--interfaces
INSERT INTO [product_interface] (product_id, interface_type_id, interface_area, connection_parameter_id, interface_guid)
SELECT [issuer_product].product_id, [issuer_interface].interface_type_id, [issuer_interface].interface_area, 
		[issuer_interface].connection_parameter_id, [issuer_interface].interface_guid
FROM [issuer_interface] INNER JOIN [issuer_product] ON [issuer_product].issuer_id = [issuer_interface].issuer_id
WHERE [issuer_interface].interface_type_id != 2

DELETE FROM [issuer_interface]
WHERE [issuer_interface].interface_type_id != 2
GO

--General cleanup
ALTER TABLE [issuer]
DROP COLUMN delete_pin_file_YN
GO
ALTER TABLE [issuer]
DROP COLUMN cards_file_location
GO
ALTER TABLE [issuer]
DROP COLUMN card_file_type
GO
ALTER TABLE [issuer]
DROP COLUMN pin_file_location
GO
ALTER TABLE [issuer]
DROP COLUMN pin_encrypted_ZPK
GO
ALTER TABLE [issuer]
DROP COLUMN pin_mailer_file_type
GO
ALTER TABLE [issuer]
DROP COLUMN pin_printer_name
GO
ALTER TABLE [issuer]
DROP COLUMN pin_encrypted_PWK
GO
ALTER TABLE [issuer]
DROP CONSTRAINT DF__issuer__EnableCa__4341E1B1
GO
ALTER TABLE [issuer]
DROP COLUMN EnableCardFileLoader_YN
GO