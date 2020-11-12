USE [indigo_database_group]
GO

DECLARE @RC int
DECLARE @new_issuer_id int
DECLARE @ResultCode int
DECLARE @new_branch_id int
DECLARE @new_product_id int

--Create Issuer
EXECUTE @RC = [dbo].[sp_create_issuer] 
   0, 1, 'Cote D''Ivoire', 'ECI', 1, 1, 0, 0, 0, 1, 1, null, null, 'C:\veneka\indigo_group\card_files\', '', '', '', '', '', '', 0, 1, 2, -2, 'SYSTEM'
  ,@new_issuer_id OUTPUT
  ,@ResultCode OUTPUT


--Create Flex
INSERT INTO [dbo].[flex_affiliate_codes]
           ([issuer_id], [affiliate_code])
     VALUES (@new_issuer_id, 'ECI')

--Create Branches
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'001', 'PRINCIPALE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'044', 'COCODY VALON', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'004', 'DES  II PLATEAUX', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'006', ' SAN PEDRO', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'008', 'YOPOUGON', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'011', 'COCODY ANONO', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'013', 'DALOA', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'015', 'COCODY ST JEAN', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'016', 'BASSAM', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'019', 'SOUBRE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'020', 'AGCE SAN PEDRO BARDOT', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'021', 'ADZOPE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'022', 'ABENGOUROU', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'023', 'BONDOUKOU', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'025', 'MEAGUI', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'026', 'ROSIERS', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'027', 'YOP NIANGON', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'028', 'AGCE TREICHVILLE ARRAS', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'032', 'AGCE ANGRE DJIBI', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'033', 'NOE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'038', 'YAMOUSSOUKRO', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'040', 'FIGAYO', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'045', 'YOPOUGON BEL AIR', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'002', 'DE BOUAKE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT



--Create Products
INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('CIVBC02', '_Cote D''Ivoire Black Card', '605718206', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '006')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)

INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('CVRG02', '_Cote D''voire Regional Gold', '605718202', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '004')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)

INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('CVRA', '_Cote D''voire Regional Azur', '605718201', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '003')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)

INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('CIV001', 'CIV VISA Electron', '484674', @new_issuer_id, 175, 40, 13, 1, 0 )
	SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '001')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)

INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('CVRP02', '_COTE D''IVOIRE REGIONAL PLATINUM', '605718203', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '005')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)


--BANK 05
INSERT INTO [dbo].[mod_interface_account_params]
           ([BANK_C],[GROUPC],[issuer_id],[STAT_CHANGE],[LIM_INTR],[NON_REDUCE_BAL],[CRD],[CYCLE],[DEST_ACCNT_TYPE],[REP_LANG])
     VALUES
           ('05','02', @new_issuer_id,'1' ,0 ,0 ,0 , '1st' , 1 ,'A')
GO

