

MERGE INTO [dbo].[file_statuses] AS trgt
USING	(VALUES
		(0,'READ'),
		(1,'VALID_CARDS'),
		(2,'PROCESSED'),
		(3,'DUPLICATE_FILE'),
		(4,'LOAD_FAIL'),
		(5,'FILE_CORRUPT'),
		(6,'PARTIAL_LOAD'),
		(7,'INVALID_FORMAT'),
		(8,'INVALID_NAME'),
		(9,'ISSUER_NOT_FOUND'),
		(10,'INVALID_ISSUER_LICENSE'),
		(11,'NO_ACTIVE_BRANCH_FOUND'),
		(12,'DUPLICATE_CARDS_IN_FILE'),
		(13,'DUPLICATE_CARDS_IN_DATABASE'),
		(14,'FILE_DECRYPTION_FAILED'),
		(15,'UNLICENSED_BIN'),
		(16,'NO_PRODUCT_FOUND_FOR_CARD'),
		(17,'UNLICENSED_ISSUER'),
		(18,'BRANCH_PRODUCT_NOT_FOUND'),
		(19,'CARD_FILE_INFO_READ_ERROR'),
		(20,'CARDS_NOT_ORDERED'),
		(21,'ORDERED_CARD_REF_MISSING'),
		(22,'ORDERED_CARD_PRODUCT_MISS_MATCH'),
		(23,'MULTIPLE_PRODUCTS_IN_FILE'),
		(24,'PRODUCT_NOT_ACTIVE'),
		(25,'NO_LOAD_FOR_PRODUCT'),
		(26,'ISSUER_NOT_ACTIVE'),
		(27,'ISSUER_LICENCE_EXPIRED')
		) AS src([file_status_id],[file_status])
ON
	trgt.[file_status_id] = src.[file_status_id]
WHEN MATCHED THEN
	UPDATE SET
		[file_status_id] = src.[file_status_id]
		, [file_status] = src.[file_status]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([file_status_id],[file_status])
	VALUES ([file_status_id],[file_status])

;

