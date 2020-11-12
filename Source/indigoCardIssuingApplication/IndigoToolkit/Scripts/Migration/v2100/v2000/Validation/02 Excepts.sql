--These need table valued functions because of their encrypted columns
--cards
SELECT * FROM [indigo_database_group].[dbo].[source_cards]( )
	EXCEPT
SELECT * FROM [indigo_database_v213].[dbo].[target_cards]( )

--customers
SELECT * FROM [indigo_database_group].[dbo].[source_customers]( )
	EXCEPT
SELECT * FROM [indigo_database_v213].[dbo].[target_customers]( )

--users
SELECT * FROM [indigo_database_group].[dbo].[source_users]( )
	EXCEPT
SELECT * FROM [indigo_database_v213].[dbo].[target_users]( )

--These dont need table valued functions
--issuers
SELECT [issuer_id],[issuer_status_id],[country_id],[issuer_name],[issuer_code],[instant_card_issue_YN],[maker_checker_YN],[license_file],[license_key],[language_id] 
FROM [indigo_database_group].[dbo].[issuer]
	EXCEPT
SELECT [issuer_id],[issuer_status_id],[country_id],[issuer_name],[issuer_code],[instant_card_issue_YN],[maker_checker_YN],[license_file],[license_key],[language_id] 
FROM [indigo_database_v213].[dbo].[issuer]

--branch
SELECT [branch_id],[branch_status_id],[issuer_id],[branch_code],[branch_name],[location],[contact_person],[contact_email],[card_centre]
FROM [indigo_database_group].[dbo].[branch]
	EXCEPT
SELECT [branch_id],[branch_status_id],[issuer_id],[branch_code],[branch_name],[location],[contact_person],[contact_email],[card_centre]
FROM [indigo_database_v213].[dbo].[branch]

--products
--NOTE BIN and subproduct split in new version, so must split for compare
SELECT [product_id],[product_code],[product_name]
	  ,SUBSTRING([product_bin_code], 1, 6) AS [product_bin_code]
	  ,SUBSTRING([product_bin_code], 7, LEN([product_bin_code])) as [sub_product_code]
	  ,[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN]    
FROM [indigo_database_group].[dbo].[issuer_product]
	EXCEPT
SELECT [product_id],[product_code],[product_name]
	  ,[product_bin_code]
	  ,[sub_product_code]
	  ,[issuer_id],[name_on_card_top],[name_on_card_left],[Name_on_card_font_size],[font_id],[DeletedYN]    
FROM [indigo_database_v213].[dbo].[issuer_product]

--File load
SELECT [file_load_id], [file_load_start], [file_load_end], [user_id], [files_to_process] FROM [indigo_database_group].[dbo].[file_load]
	EXCEPT
SELECT [file_load_id], [file_load_start], [file_load_end], [user_id], [files_to_process] FROM [indigo_database_v213].[dbo].[file_load]

--File history
SELECT [file_id],[issuer_id],[file_status_id],[file_type_id],[name_of_file],[file_created_date],[file_size],[load_date],[file_directory],[number_successful_records],[number_failed_records],[file_load_comments],[file_load_id] FROM [indigo_database_group].[dbo].[file_history]
	EXCEPT
SELECT [file_id],[issuer_id],[file_status_id],[file_type_id],[name_of_file],[file_created_date],[file_size],[load_date],[file_directory],[number_successful_records],[number_failed_records],[file_load_comments],[file_load_id] FROM [indigo_database_v213].[dbo].[file_history]

--Load Batch
SELECT [load_batch_id],[file_id],[load_batch_status_id],[no_cards],[load_date],[load_batch_reference] FROM [indigo_database_group].[dbo].[load_batch]
	EXCEPT
SELECT [load_batch_id],[file_id],[load_batch_status_id],[no_cards],[load_date],[load_batch_reference] FROM [indigo_database_v213].[dbo].[load_batch]

--Load batch cards
SELECT [load_batch_id], [card_id], [load_card_status_id] FROM [indigo_database_group].[dbo].[load_batch_cards]
	EXCEPT
SELECT [load_batch_id], [card_id], [load_card_status_id] FROM [indigo_database_v213].[dbo].[load_batch_cards]

--[load_batch_status]
SELECT [load_batch_status_id],[load_batch_id],[load_batch_statuses_id],[user_id],[status_date],[status_notes] FROM [indigo_database_group].[dbo].[load_batch_status]
	EXCEPT
SELECT [load_batch_status_id],[load_batch_id],[load_batch_statuses_id],[user_id],[status_date],[status_notes] FROM [indigo_database_v213].[dbo].[load_batch_status]

--[audit_control]
SELECT [audit_id],[audit_action_id],[user_id],[audit_date],[workstation_address],[action_description],[issuer_id],[data_changed],[data_before],[data_after] FROM [indigo_database_group].[dbo].[audit_control]
	EXCEPT
SELECT [audit_id],[audit_action_id],[user_id],[audit_date],[workstation_address],[action_description],[issuer_id],[data_changed],[data_before],[data_after] FROM [indigo_database_v213].[dbo].[audit_control]

--[dist_batch]
SELECT [dist_batch_id],[branch_id],[no_cards],[date_created],[dist_batch_reference] FROM [indigo_database_group].[dbo].[dist_batch]
	EXCEPT
SELECT [dist_batch_id],[branch_id],[no_cards],[date_created],[dist_batch_reference] FROM [indigo_database_v213].[dbo].[dist_batch]

--[dist_batch_cards]
SELECT [dist_batch_id], [card_id], [dist_card_status_id] FROM [indigo_database_group].[dbo].[dist_batch_cards]
	EXCEPT
SELECT [dist_batch_id], [card_id], [dist_card_status_id] FROM [indigo_database_v213].[dbo].[dist_batch_cards]

--[dist_batch_cards]
SELECT [dist_batch_status_id],[dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes] FROM [indigo_database_group].[dbo].[dist_batch_status]
	EXCEPT
SELECT [dist_batch_status_id],[dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes] FROM [indigo_database_v213].[dbo].[dist_batch_status]

--[branch_card_status]
--NOTE: Manually loaded card status changed from ID 10 to 16, therefor we check on the name of the status and not the ID
SELECT [branch_card_status_id],[card_id],[status_date],[user_id],[operator_user_id],[branch_card_code_id],[comments],[branch_card_statuses_name]
	FROM [indigo_database_group].[dbo].[branch_card_status] LEFT OUTER JOIN [indigo_database_group].[dbo].[branch_card_statuses]
		ON [indigo_database_group].[dbo].[branch_card_status].[branch_card_statuses_id] = [indigo_database_group].[dbo].[branch_card_statuses].[branch_card_statuses_id]
	EXCEPT
SELECT [branch_card_status_id],[card_id],[status_date],[user_id],[operator_user_id],[branch_card_code_id],[comments], [branch_card_statuses_name]
	FROM [indigo_database_v213].[dbo].[branch_card_status] LEFT OUTER JOIN [indigo_database_v213].[dbo].[branch_card_statuses]
		ON [indigo_database_v213].[dbo].[branch_card_status].[branch_card_statuses_id] = [indigo_database_v213].[dbo].[branch_card_statuses].[branch_card_statuses_id]

--[users_to_users_groups]
SELECT [user_id], [user_group_id] FROM [indigo_database_group].[dbo].[users_to_users_groups]
	EXCEPT
SELECT [user_id], [user_group_id] FROM [indigo_database_v213].[dbo].[users_to_users_groups]

--[user_group]
SELECT [user_group_id],[user_role_id],[issuer_id],[can_create],[can_read],[can_update],[can_delete],[all_branch_access],[user_group_name] FROM [indigo_database_group].[dbo].[user_group]
	EXCEPT
SELECT [user_group_id],[user_role_id],[issuer_id],[can_create],[can_read],[can_update],[can_delete],[all_branch_access],[user_group_name] FROM [indigo_database_v213].[dbo].[user_group]

--[user_groups_branches]
SELECT [user_group_id], [branch_id] FROM [indigo_database_group].[dbo].[user_groups_branches]
	EXCEPT
SELECT [user_group_id], [branch_id] FROM [indigo_database_v213].[dbo].[user_groups_branches]