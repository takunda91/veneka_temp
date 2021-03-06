

MERGE INTO [dbo].[Issuer_product_font] AS trgt
USING	(VALUES
		(1,'Lucida Console',NULL,0),
		(2,'Arial',NULL,0),
		(3,'Courier New',NULL,0),
		(4,'Times New Roman',NULL,0)
		) AS src([font_id],[font_name],[resource_path],[DeletedYN])
ON
	trgt.[font_id] = src.[font_id]
WHEN MATCHED THEN
	UPDATE SET
		[font_id] = src.[font_id]
		, [font_name] = src.[font_name]
		, [resource_path] = src.[resource_path]
		, [DeletedYN] = src.[DeletedYN]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([font_id],[font_name],[resource_path],[DeletedYN])
	VALUES ([font_id],[font_name],[resource_path],[DeletedYN])

;

