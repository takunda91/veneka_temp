--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[issuer]
EXCEPT
SELECT [issuer_id],[issuer_status_id],[country_id],[issuer_name],[issuer_code],[instant_card_issue_YN],[maker_checker_YN],[license_file],[license_key]
      ,[language_id],[card_ref_preference],[classic_card_issue_YN],[enable_instant_pin_YN],[authorise_pin_issue_YN],[authorise_pin_reissue_YN],[back_office_pin_auth_YN]
FROM [{DATABASE_NAME}].[dbo].[issuer]