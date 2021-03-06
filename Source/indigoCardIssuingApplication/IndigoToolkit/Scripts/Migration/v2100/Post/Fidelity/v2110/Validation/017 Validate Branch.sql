--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[branch]
EXCEPT
SELECT [branch_id],[branch_status_id],[issuer_id],[branch_code],[branch_name],[location],[contact_person],[contact_email],[card_centre],[card_centre_branch_YN]
FROM [{DATABASE_NAME}].[dbo].[branch]