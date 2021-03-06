

MERGE INTO [dbo].[pin_batch_statuses_flow] AS trgt
USING	(VALUES
		(0,0,0,13,1,0,NULL,NULL,NULL,NULL,NULL,NULL),
		(0,1,0,14,2,1,NULL,NULL,NULL,5,NULL,2),
		(0,5,0,13,1,0,NULL,NULL,NULL,NULL,NULL,NULL),
		(1,0,0,14,3,1,NULL,NULL,NULL,NULL,NULL,NULL),
		(1,3,0,15,4,1,NULL,NULL,NULL,6,NULL,4),
		(1,6,0,14,3,1,NULL,NULL,NULL,NULL,NULL,NULL),
		(2,0,0,4,11,2,NULL,NULL,NULL,NULL,NULL,NULL),
		(2,1,0,14,2,1,NULL,NULL,NULL,NULL,NULL,NULL),
		(2,7,0,13,8,2,NULL,NULL,NULL,NULL,NULL,NULL),
		(2,8,0,12,9,2,NULL,NULL,NULL,NULL,NULL,NULL),
		(2,9,0,12,1,2,NULL,NULL,NULL,NULL,NULL,NULL),
		(2,11,0,13,7,2,NULL,NULL,NULL,NULL,NULL,NULL)
		) AS src([pin_batch_type_id],[pin_batch_statuses_id],[card_issue_method_id],[user_role_id],[flow_pin_batch_statuses_id],[flow_pin_batch_type_id],[main_menu_id],[sub_menu_id],[sub_menu_order],[reject_pin_batch_statuses_id],[reject_pin_card_statuses_id],[flow_pin_card_statuses_id])
ON
	trgt.[pin_batch_type_id] = src.[pin_batch_type_id]
AND trgt.[pin_batch_statuses_id] = src.[pin_batch_statuses_id]
AND trgt.[card_issue_method_id] = src.[card_issue_method_id]
AND trgt.[flow_pin_batch_statuses_id] = src.[flow_pin_batch_statuses_id]
WHEN MATCHED THEN
	UPDATE SET
		[pin_batch_type_id] = src.[pin_batch_type_id]
		, [pin_batch_statuses_id] = src.[pin_batch_statuses_id]
		, [card_issue_method_id] = src.[card_issue_method_id]
		, [user_role_id] = src.[user_role_id]
		, [flow_pin_batch_statuses_id] = src.[flow_pin_batch_statuses_id]
		, [flow_pin_batch_type_id] = src.[flow_pin_batch_type_id]
		, [main_menu_id] = src.[main_menu_id]
		, [sub_menu_id] = src.[sub_menu_id]
		, [sub_menu_order] = src.[sub_menu_order]
		, [reject_pin_batch_statuses_id] = src.[reject_pin_batch_statuses_id]
		, [reject_pin_card_statuses_id] = src.[reject_pin_card_statuses_id]
		, [flow_pin_card_statuses_id] = src.[flow_pin_card_statuses_id]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([pin_batch_type_id],[pin_batch_statuses_id],[card_issue_method_id],[user_role_id],[flow_pin_batch_statuses_id],[flow_pin_batch_type_id],[main_menu_id],[sub_menu_id],[sub_menu_order],[reject_pin_batch_statuses_id],[reject_pin_card_statuses_id],[flow_pin_card_statuses_id])
	VALUES ([pin_batch_type_id],[pin_batch_statuses_id],[card_issue_method_id],[user_role_id],[flow_pin_batch_statuses_id],[flow_pin_batch_type_id],[main_menu_id],[sub_menu_id],[sub_menu_order],[reject_pin_batch_statuses_id],[reject_pin_card_statuses_id],[flow_pin_card_statuses_id])

;

