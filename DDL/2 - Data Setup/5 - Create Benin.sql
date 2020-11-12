USE [indigo_database_group]
GO

DECLARE @RC int
DECLARE @new_issuer_id int
DECLARE @ResultCode int
DECLARE @new_branch_id int
DECLARE @new_product_id int

--Create Issuer
EXECUTE @RC = [dbo].[sp_create_issuer] 
   0, 5, 'Benin', 'EBJ', 1, 1, 0, 0, 0, 1, 1, null, null, 'C:\veneka\indigo_group\card_files\', '', '', '', '', '', '', 0, 1, 2, -2, 'SYSTEM'
  ,@new_issuer_id OUTPUT
  ,@ResultCode OUTPUT


--Create Flex
INSERT INTO [dbo].[flex_affiliate_codes]
           ([issuer_id], [affiliate_code])
     VALUES (@new_issuer_id, 'EBJ')


--Create Branches
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'010', 'SEME', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'030', 'CNHU', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'013', 'CADJEHOUN', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'014', 'MARO MILITAIRE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'015', 'ZOGBO', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'017', 'SEGBEYA', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'018', 'DEGAKON', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'024', 'EBB LOKOSSA', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'025', 'EBB SAVALOU', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'026', 'GBEDJROMEDE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'001', 'PRINCIPALE GANHI', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'002', 'SAINT MICHEL', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'003', 'AKPAKPA', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'004', 'FIDJROSSE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'005', 'STEINMETZ', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'006', 'ETOILE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'007', 'GODOMEY', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'009', 'PORTO NOVO', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'011', 'BOHICON', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'021', 'GODOMEY ROUTE DE OUIDAH', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'022', 'UNIVERSITE DE CALAVI', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'027', 'OUANDO', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'028', 'GBEGAMEY', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'029', 'DJOUGOU', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'012', 'NATITINGOU', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT



--Create Products
INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('BRP02', '_Benin Regional Platinum', '605715203', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '030')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)


INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('BRA02', '_Benin Regional Azur', '605715201', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '010')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)


INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('BBC02', 'Benin_Black_Card', '605715206', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '004')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)


INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('BNVS01', 'VISA Electron Debit', '484677', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '001')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)


INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('BRG02', '_Benin Regional Gold', '605715202', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '020')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)



--CMS DETAILS - BANK 02
INSERT INTO [dbo].[mod_interface_account_params]
           ([BANK_C],[GROUPC],[issuer_id],[STAT_CHANGE],[LIM_INTR],[NON_REDUCE_BAL],[CRD],[CYCLE],[DEST_ACCNT_TYPE],[REP_LANG])
     VALUES
           ('02','02', @new_issuer_id,'1' ,0 ,0 ,0 , '1ST' , 1 ,'A')
GO

