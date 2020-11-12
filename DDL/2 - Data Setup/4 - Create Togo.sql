USE [indigo_database_group]
GO

DECLARE @RC int
DECLARE @new_issuer_id int
DECLARE @ResultCode int
DECLARE @new_branch_id int
DECLARE @new_product_id int

--Create Issuer
EXECUTE @RC = [dbo].[sp_create_issuer] 
   0, 4, 'Togo', 'ETG', 1, 1, 0, 0, 0, 1, 1, null, null, 'C:\veneka\indigo_group\card_files\', '', '', '', '', '', '', 0, 1, 2, -2, 'SYSTEM'
  ,@new_issuer_id OUTPUT
  ,@ResultCode OUTPUT


--Create Flex
INSERT INTO [dbo].[flex_affiliate_codes]
           ([issuer_id], [affiliate_code])
     VALUES (@new_issuer_id, 'ETG')

--Create Branches
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'702', 'ASSIVITO', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'703', 'AKODESSEWA', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'701', 'PRINCIPALE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'704', 'TOKOIN', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'705', 'BAGUIDA', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'721', 'MINI-DIRECTION DU PORT', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'710', 'ROND POINT PORT', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'717', 'MINI-ADIDOGOME', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'718', 'MINI-AGOE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'719', 'MINI-GRAND SEMINAIRE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'709', 'NUKAFU', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT


--Create Products
INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('TRG02', '_Togo Regional Gold', '605714202', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '004')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)

INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('TRA02', '_Togo Regional Azur', '605714201', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '003')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)

INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('debit new', 'debit new', '444444', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '002')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)

INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('TRP02', '_TOGO Regional Platinum', '605714203', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '005')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)

INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('Togo Visa', 'Togo Visa Electron', '484685', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '001')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)

INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('TBC02', '_Togo Black Card', '605714206', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '006')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)



--CMS DETAILS - BANK 01
INSERT INTO [dbo].[mod_interface_account_params]
           ([BANK_C],[GROUPC],[issuer_id],[STAT_CHANGE],[LIM_INTR],[NON_REDUCE_BAL],[CRD],[CYCLE],[DEST_ACCNT_TYPE],[REP_LANG])
     VALUES
           ('01','02', @new_issuer_id,'1' ,0 ,0 ,0 , '1' , 1 ,'A')
GO

