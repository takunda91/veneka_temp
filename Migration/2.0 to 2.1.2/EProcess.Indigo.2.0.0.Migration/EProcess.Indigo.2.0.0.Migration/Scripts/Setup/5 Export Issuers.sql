

SET IDENTITY_INSERT [dbo].[issuer] ON 

INSERT INTO [dbo].[issuer]
           ([issuer_id]
		   ,[issuer_status_id]
           ,[country_id]
           ,[issuer_name]
           ,[issuer_code]
           ,[instant_card_issue_YN]
           ,[maker_checker_YN]
           ,[license_file]
           ,[license_key]
           ,[language_id]
           ,[card_ref_preference]
           ,[classic_card_issue_YN]
           ,[enable_instant_pin_YN]
           ,[authorise_pin_issue_YN]
           ,[authorise_pin_reissue_YN]
           ,[back_office_pin_auth_YN])
	SELECT [issuer_id]
		  ,[issuer_status_id]
		  ,[country_id]
		  ,[issuer_name]
		  ,[issuer_code]
		  ,[instant_card_issue_YN]
		  ,[maker_checker_YN]
		  ,[license_file]
		  ,[license_key]
		  ,[language_id]
		  ,0
		  ,0
		  ,0
		  ,0
		  ,0
		  ,0
	FROM [indigo_database_group].[dbo].[issuer]
	ORDER BY [issuer_id] ASC

SET IDENTITY_INSERT [dbo].[issuer] OFF 


---- MOVE THE SEED VALUE FOR SAFTY SAKE
--DECLARE @issuers_current_seed INT,
--		@issuers_new_seed INT

--SET @issuers_current_seed = 
--(
--	SELECT TOP 1 [issuer].[issuer_id]
--	FROM [dbo].[issuer]
--	ORDER BY [issuer].[issuer_id] DESC
--)


--SET @issuers_new_seed = (@issuers_current_seed * 1.5)

--DBCC CHECKIDENT('[dbo].[issuer]', RESEED, @issuers_new_seed);
