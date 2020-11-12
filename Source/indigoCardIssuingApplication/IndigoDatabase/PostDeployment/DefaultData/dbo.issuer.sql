SET IDENTITY_INSERT [dbo].[issuer] ON;

MERGE INTO [dbo].[issuer] AS trgt
USING	(VALUES
		(-1,0,0,'Enterprise','',0,0,'','',0,0,0,0,0,0,0,'B22B189F-46B7-4E00-8E38-6A433E565841',NULL,0)
		) AS src([issuer_id],[issuer_status_id],[country_id],[issuer_name],[issuer_code],[instant_card_issue_YN],[maker_checker_YN],[license_file],[license_key],[language_id],[card_ref_preference],[classic_card_issue_YN],[enable_instant_pin_YN],[authorise_pin_issue_YN],[authorise_pin_reissue_YN],[back_office_pin_auth_YN],[remote_token],[remote_token_expiry],[allow_branches_to_search_cards])
ON
	trgt.[issuer_id] = src.[issuer_id]
WHEN MATCHED THEN
	UPDATE SET
		[issuer_status_id] = src.[issuer_status_id]
		, [country_id] = src.[country_id]
		, [issuer_name] = src.[issuer_name]
		, [issuer_code] = src.[issuer_code]
		, [instant_card_issue_YN] = src.[instant_card_issue_YN]
		, [maker_checker_YN] = src.[maker_checker_YN]
		, [license_file] = src.[license_file]
		, [license_key] = src.[license_key]
		, [language_id] = src.[language_id]
		, [card_ref_preference] = src.[card_ref_preference]
		, [classic_card_issue_YN] = src.[classic_card_issue_YN]
		, [enable_instant_pin_YN] = src.[enable_instant_pin_YN]
		, [authorise_pin_issue_YN] = src.[authorise_pin_issue_YN]
		, [authorise_pin_reissue_YN] = src.[authorise_pin_reissue_YN]
		, [back_office_pin_auth_YN] = src.[back_office_pin_auth_YN]
		--, [remote_token] = src.[remote_token]
		--, [remote_token_expiry] = src.[remote_token_expiry]
		, [allow_branches_to_search_cards] = src.[allow_branches_to_search_cards]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([issuer_id],[issuer_status_id],[country_id],[issuer_name],[issuer_code],[instant_card_issue_YN],[maker_checker_YN],[license_file],[license_key],[language_id],[card_ref_preference],[classic_card_issue_YN],[enable_instant_pin_YN],[authorise_pin_issue_YN],[authorise_pin_reissue_YN],[back_office_pin_auth_YN],[remote_token],[remote_token_expiry],[allow_branches_to_search_cards])
	VALUES ([issuer_id],[issuer_status_id],[country_id],[issuer_name],[issuer_code],[instant_card_issue_YN],[maker_checker_YN],[license_file],[license_key],[language_id],[card_ref_preference],[classic_card_issue_YN],[enable_instant_pin_YN],[authorise_pin_issue_YN],[authorise_pin_reissue_YN],[back_office_pin_auth_YN],[remote_token],[remote_token_expiry],[allow_branches_to_search_cards])

;
SET IDENTITY_INSERT [dbo].[issuer] OFF;
