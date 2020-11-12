USE [indigo_database_group]
GO

DECLARE @RC int
DECLARE @new_issuer_id int
DECLARE @ResultCode int
DECLARE @new_branch_id int
DECLARE @new_product_id int

--Create Issuer
EXECUTE @RC = [dbo].[sp_create_issuer] 
   0, 11, 'Burkina Faso', 'EBF', 1, 1, 0, 0, 0, 1, 1, null, null, 'C:\veneka\indigo_group\card_files\', '', '', '', '', '', '', 0, 1, 2, -2, 'SYSTEM'
  ,@new_issuer_id OUTPUT
  ,@ResultCode OUTPUT


--Create Flex
INSERT INTO [dbo].[flex_affiliate_codes]
           ([issuer_id], [affiliate_code])
     VALUES (@new_issuer_id, 'EBF')

--Create Branches
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'010', 'GOUNGHIN 1', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'009', 'CASH POINT TAMPOUY', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'012', 'PATTE D''OIE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'013', 'DE LEO', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'014', 'PO', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'015', 'OUAGA 2000', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'016', 'CASH POINT ROOD WOKO', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'017', '1200 LOGEMENTS', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'018', 'KOULOUBA', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'019', 'CASH POINT WAYALGHIN', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'020', 'DE GOUNGHIN 2', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'021', 'DE BOBO DIOULASSO 1', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'022', 'CASH POINT KUA', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'024', 'DE BOBO DIOULASSO 2', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'025', 'BUREAU DE GAOUA', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'026', 'DE ORODARA', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'027', 'DE DIEBOUGOU', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'028', 'DE N''DOROLA', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'029', 'DE HOUNDE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'030', 'DE BOROMO', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'031', 'DE SOLENZO', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'032', 'DE NOUNA', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'033', 'DE DEDOUGOU', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'035', 'DE DIAPAGA', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'037', 'DE ZORGHO', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'038', 'DE POUYTENGA', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'040', 'DE BOGANDE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'002', 'DE KAYA', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'003', 'DORI', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'004', 'OUAHIGOUYA', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'005', 'DE ZINIARE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'006', 'CASH POINT TANGHIN', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'007', 'SANKARYARE', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'008', 'OUIDI', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT
EXECUTE [dbo].[sp_insert_branch] 0, @new_issuer_id,'011', 'KWAME NKRUMAH', '', '' ,'' ,'', -2, 'SYSTEM' , @new_branch_id OUTPUT, @ResultCode OUTPUT



--Create Products
INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('BFRG02', '_Burkina Regional Gold', '605716202', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '003')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)

INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('BFRP02', '_Burkina Regional Platinum', '605716203', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '004')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)

INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('VE001', 'Burkina VISA Electron', '484676', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '001')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)

INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('BFBC02', '_Burkina Black Card', '605716206', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '005')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)

INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('BFRA02', '_Burkina Regional Azur', '605716201', @new_issuer_id, 175, 40, 13, 1, 0 )
	SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '002')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)

INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('DB', 'DB', '413135', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '999')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)

INSERT INTO [dbo].[issuer_product]           
 ([product_code],[product_name],[product_bin_code],[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN])
     VALUES ('PA', 'PA', '903703', @new_issuer_id, 175, 40, 13, 1, 0 )
SET @new_product_id = SCOPE_IDENTITY();
INSERT INTO [dbo].[mod_interface_cond_accnt] ([product_id] ,[CCY],[COND_SET])
     VALUES  (@new_product_id, 'XOF', '888')
INSERT INTO [dbo].[product_currency]
           ([product_id] ,[currency_id])
     VALUES (@new_product_id, 4)


--BANK 03
INSERT INTO [dbo].[mod_interface_account_params]
           ([BANK_C],[GROUPC],[issuer_id],[STAT_CHANGE],[LIM_INTR],[NON_REDUCE_BAL],[CRD],[CYCLE],[DEST_ACCNT_TYPE],[REP_LANG])
     VALUES
           ('03','02', @new_issuer_id,'1' ,0 ,0 ,0 , '1' , 1 ,'A')
GO

