

SET IDENTITY_INSERT  [dbo].[branch] ON

INSERT INTO [dbo].[branch]
           ([branch_id]
		   ,[branch_status_id]
           ,[issuer_id]
           ,[branch_code]
           ,[branch_name]
           ,[location]
           ,[contact_person]
           ,[contact_email]
           ,[card_centre]
           ,[card_centre_branch_YN])
	SELECT [branch_id]
			,[branch_status_id]
			,[issuer_id]
			,[branch_code]
			,[branch_name]
			,[location]
			,[contact_person]
			,[contact_email]
			,[card_centre]
			,0
		FROM [indigo_database_group].[dbo].[branch]
		ORDER BY [branch_id] ASC

SET IDENTITY_INSERT  [dbo].[branch] OFF