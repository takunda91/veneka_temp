SET IDENTITY_INSERT [dbo].[branch] ON;

MERGE INTO [dbo].[branch] AS trgt
USING	(VALUES
		(-1,0,-1,'','Enterprise Branch','','','','',NULL,0,NULL)
		) AS src([branch_id],[branch_status_id],[issuer_id],[branch_code],[branch_name],[location],[contact_person],[contact_email],[card_centre],[emp_branch_code],[branch_type_id],[main_branch_id])
ON
	trgt.[branch_id] = src.[branch_id]
WHEN MATCHED THEN
	UPDATE SET
		[branch_status_id] = src.[branch_status_id]
		, [issuer_id] = src.[issuer_id]
		, [branch_code] = src.[branch_code]
		, [branch_name] = src.[branch_name]
		, [location] = src.[location]
		, [contact_person] = src.[contact_person]
		, [contact_email] = src.[contact_email]
		, [card_centre] = src.[card_centre]
		, [emp_branch_code] = src.[emp_branch_code]
		, [branch_type_id] = src.[branch_type_id]
		, [main_branch_id] = src.[main_branch_id]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([branch_id],[branch_status_id],[issuer_id],[branch_code],[branch_name],[location],[contact_person],[contact_email],[card_centre],[emp_branch_code],[branch_type_id],[main_branch_id])
	VALUES ([branch_id],[branch_status_id],[issuer_id],[branch_code],[branch_name],[location],[contact_person],[contact_email],[card_centre],[emp_branch_code],[branch_type_id],[main_branch_id])

;
SET IDENTITY_INSERT [dbo].[branch] OFF;
