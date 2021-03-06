

MERGE INTO [dbo].[dist_batch_status_flow] AS trgt
USING	(VALUES
		(1,'DEFAULT_CENTRALISED_PRODUCTION',0,0),
		(2,'DEFAULT_CENTRALISED_PRODUCTION_WITH_PINMAILER',0,0),
		(3,'DEFAULT_INSTANT_PRODUCTION',0,1),
		(4,'DEFAULT_INSTANT_EMP_PRODUCTION',0,1),
		(5,'DEFAULT_CENTRALISED_DISTRIBUTION',1,0),
		(6,'DEFAULT_INSTANT_DISTRIBUTION',1,1),
		(13,'DEFAULT_INSTANT_ASYNC_PRODUCTION',0,1)
		) AS src([dist_batch_status_flow_id],[dist_batch_status_flow_name],[dist_batch_type_id],[card_issue_method_id])
ON
	trgt.[dist_batch_status_flow_id] = src.[dist_batch_status_flow_id]
WHEN MATCHED THEN
	UPDATE SET
		[dist_batch_status_flow_id] = src.[dist_batch_status_flow_id]
		, [dist_batch_status_flow_name] = src.[dist_batch_status_flow_name]
		, [dist_batch_type_id] = src.[dist_batch_type_id]
		, [card_issue_method_id] = src.[card_issue_method_id]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([dist_batch_status_flow_id],[dist_batch_status_flow_name],[dist_batch_type_id],[card_issue_method_id])
	VALUES ([dist_batch_status_flow_id],[dist_batch_status_flow_name],[dist_batch_type_id],[card_issue_method_id])

;

