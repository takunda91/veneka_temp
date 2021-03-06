

MERGE INTO [dbo].[customer_title_language] AS trgt
USING	(VALUES
		(0,0,'MR'),
		(0,1,'Mr.'),
		(0,2,'MR_pt'),
		(0,3,'MR_sp'),
		(1,0,'MRS'),
		(1,1,'Me.'),
		(1,2,'MRS_pt'),
		(1,3,'MRS_sp'),
		(2,0,'MISS'),
		(2,1,'Mlle.'),
		(2,2,'MISS_pt'),
		(2,3,'MISS_sp'),
		(3,0,'MS'),
		(3,1,'Me.'),
		(3,2,'MS_pt'),
		(3,3,'MS_sp'),
		(4,0,'PROF'),
		(4,1,'Prof.'),
		(4,2,'PROF_pt'),
		(4,3,'PROF_sp'),
		(5,0,'DR'),
		(5,1,'Dr.'),
		(5,2,'DR_pt'),
		(5,3,'DR_sp'),
		(6,0,'REV'),
		(6,1,'Rev.'),
		(6,2,'REV_pt'),
		(6,3,'REV_sp'),
		(7,0,'OTHER'),
		(7,1,'Autres'),
		(7,2,'OTHER_pt'),
		(7,3,'OTHER_sp')
		) AS src([customer_title_id],[language_id],[language_text])
ON
	trgt.[customer_title_id] = src.[customer_title_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[customer_title_id] = src.[customer_title_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([customer_title_id],[language_id],[language_text])
	VALUES ([customer_title_id],[language_id],[language_text])

;

