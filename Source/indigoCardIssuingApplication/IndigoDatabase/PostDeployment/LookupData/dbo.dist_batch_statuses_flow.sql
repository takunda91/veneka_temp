

MERGE INTO [dbo].[dist_batch_statuses_flow] AS trgt
USING	(VALUES
		(-1,5,0,0,1,NULL,0,NULL,NULL,NULL,NULL,NULL,1),
		(0,4,9,0,1,NULL,1,8,NULL,NULL,NULL,NULL,1),
		(9,12,10,0,1,NULL,4,NULL,NULL,NULL,NULL,NULL,1),
		(10,12,11,0,1,NULL,5,NULL,NULL,NULL,NULL,NULL,1),
		(11,4,14,1,1,NULL,8,16,18,NULL,NULL,NULL,1),
		(16,11,13,0,1,NULL,9,NULL,NULL,NULL,NULL,NULL,1),
		(-1,5,0,0,1,NULL,0,NULL,NULL,NULL,NULL,NULL,2),
		(0,4,9,0,1,NULL,1,8,NULL,NULL,NULL,NULL,2),
		(9,13,17,0,1,NULL,2,NULL,NULL,NULL,NULL,NULL,2),
		(10,12,11,0,1,NULL,5,NULL,NULL,NULL,NULL,NULL,2),
		(11,11,12,0,1,NULL,6,NULL,NULL,NULL,NULL,NULL,2),
		(12,11,13,0,1,NULL,7,NULL,NULL,NULL,NULL,NULL,2),
		(13,4,14,1,1,NULL,8,16,18,NULL,NULL,NULL,2),
		(16,11,13,0,1,NULL,9,NULL,NULL,NULL,NULL,NULL,2),
		(17,13,18,0,1,NULL,3,NULL,NULL,NULL,NULL,NULL,2),
		(18,12,10,0,1,NULL,4,NULL,NULL,NULL,NULL,NULL,2),
		(20,-1,21,0,2,NULL,6,NULL,NULL,NULL,NULL,NULL,2),
		(-1,5,0,0,2,NULL,0,NULL,NULL,NULL,NULL,NULL,3),
		(0,4,9,0,2,NULL,1,6,NULL,8,NULL,NULL,3),
		(1,5,14,0,2,NULL,5,NULL,18,NULL,NULL,NULL,3),
		(9,12,10,0,2,NULL,2,NULL,NULL,NULL,NULL,NULL,3),
		(10,12,11,0,2,NULL,3,NULL,NULL,NULL,NULL,NULL,3),
		(11,4,1,0,2,NULL,4,NULL,NULL,NULL,NULL,NULL,3),
		(-1,5,0,0,2,NULL,0,NULL,NULL,NULL,NULL,NULL,4),
		(0,4,9,0,2,NULL,1,6,NULL,8,NULL,NULL,4),
		(1,5,14,0,2,NULL,5,NULL,18,NULL,NULL,NULL,4),
		(9,12,10,0,2,NULL,2,NULL,NULL,NULL,NULL,NULL,4),
		(10,12,20,0,2,NULL,3,NULL,20,NULL,NULL,NULL,4),
		(21,5,14,0,2,NULL,5,NULL,18,NULL,NULL,NULL,4),
		(0,4,1,1,-1,1,1,NULL,NULL,NULL,NULL,NULL,5),
		(1,5,2,1,-1,1,2,NULL,NULL,NULL,NULL,NULL,5),
		(2,2,3,1,-1,1,3,4,NULL,NULL,NULL,NULL,5),
		(4,5,2,1,-1,1,4,NULL,NULL,NULL,NULL,NULL,5),
		(-1,2,19,1,3,NULL,1,NULL,NULL,NULL,NULL,NULL,6),
		(0,4,1,1,-1,1,4,8,NULL,18,NULL,NULL,6),
		(1,5,2,1,-1,1,5,NULL,NULL,NULL,NULL,NULL,6),
		(2,2,3,1,-1,1,6,4,NULL,NULL,NULL,NULL,6),
		(4,4,8,1,-1,1,4,6,18,11,NULL,NULL,6),
		(16,2,8,1,3,NULL,4,NULL,7,NULL,0,NULL,6),
		(19,5,14,1,3,801,2,16,18,NULL,NULL,NULL,6),
		(-1,5,0,0,2,NULL,0,NULL,NULL,NULL,NULL,NULL,13),
		(0,4,9,0,2,NULL,1,6,NULL,8,NULL,NULL,13),
		(1,5,14,0,2,NULL,5,NULL,18,NULL,NULL,NULL,13),
		(9,12,10,0,2,NULL,2,NULL,NULL,NULL,NULL,NULL,13),
		(10,-1,20,0,2,NULL,3,NULL,20,NULL,NULL,NULL,13),
		(21,5,14,0,2,NULL,5,NULL,18,NULL,NULL,NULL,13)
		) AS src([dist_batch_statuses_id],[user_role_id],[flow_dist_batch_statuses_id],[flow_dist_batch_type_id],[main_menu_id],[sub_menu_id],[sub_menu_order],[reject_dist_batch_statuses_id],[flow_dist_card_statuses_id],[reject_dist_card_statuses_id],[branch_card_statuses_id],[reject_branch_card_statuses_id],[dist_batch_status_flow_id])
ON
	trgt.[dist_batch_statuses_id] = src.[dist_batch_statuses_id]
AND trgt.[flow_dist_batch_statuses_id] = src.[flow_dist_batch_statuses_id]
AND trgt.[dist_batch_status_flow_id] = src.[dist_batch_status_flow_id]
WHEN MATCHED THEN
	UPDATE SET
		[dist_batch_statuses_id] = src.[dist_batch_statuses_id]
		, [user_role_id] = src.[user_role_id]
		, [flow_dist_batch_statuses_id] = src.[flow_dist_batch_statuses_id]
		, [flow_dist_batch_type_id] = src.[flow_dist_batch_type_id]
		, [main_menu_id] = src.[main_menu_id]
		, [sub_menu_id] = src.[sub_menu_id]
		, [sub_menu_order] = src.[sub_menu_order]
		, [reject_dist_batch_statuses_id] = src.[reject_dist_batch_statuses_id]
		, [flow_dist_card_statuses_id] = src.[flow_dist_card_statuses_id]
		, [reject_dist_card_statuses_id] = src.[reject_dist_card_statuses_id]
		, [branch_card_statuses_id] = src.[branch_card_statuses_id]
		, [reject_branch_card_statuses_id] = src.[reject_branch_card_statuses_id]
		, [dist_batch_status_flow_id] = src.[dist_batch_status_flow_id]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([dist_batch_statuses_id],[user_role_id],[flow_dist_batch_statuses_id],[flow_dist_batch_type_id],[main_menu_id],[sub_menu_id],[sub_menu_order],[reject_dist_batch_statuses_id],[flow_dist_card_statuses_id],[reject_dist_card_statuses_id],[branch_card_statuses_id],[reject_branch_card_statuses_id],[dist_batch_status_flow_id])
	VALUES ([dist_batch_statuses_id],[user_role_id],[flow_dist_batch_statuses_id],[flow_dist_batch_type_id],[main_menu_id],[sub_menu_id],[sub_menu_order],[reject_dist_batch_statuses_id],[flow_dist_card_statuses_id],[reject_dist_card_statuses_id],[branch_card_statuses_id],[reject_branch_card_statuses_id],[dist_batch_status_flow_id])

;

