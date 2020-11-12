USE [DB_NAME]

SELECT branch_id,
	   branch_code,
	   branch_name,
	   contact_person,
	   contact_email
FROM [dbo].[branch]
WHERE [branch].[issuer_id] = @selected_issuer_id