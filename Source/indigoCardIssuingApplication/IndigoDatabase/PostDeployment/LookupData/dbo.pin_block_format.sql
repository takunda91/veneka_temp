

MERGE INTO [dbo].[pin_block_format] AS trgt
USING	(VALUES
		(0,N'ISO9564-Format 0'),
		(1,N'ISO9564-Format 1')
		) AS src([pin_block_formatid],[pin_block_format])
ON
	trgt.[pin_block_formatid] = src.[pin_block_formatid]
WHEN MATCHED THEN
	UPDATE SET
		[pin_block_formatid] = src.[pin_block_formatid]
		, [pin_block_format] = src.[pin_block_format]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([pin_block_formatid],[pin_block_format])
	VALUES ([pin_block_formatid],[pin_block_format])

;

