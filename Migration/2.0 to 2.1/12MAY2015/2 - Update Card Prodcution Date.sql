USE [indigo_database]
GO

OPEN SYMMETRIC KEY Indigo_Symmetric_Key
DECRYPTION BY CERTIFICATE Indigo_Certificate;



	UPDATE [cards]
	SET	[cards].card_production_date = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),[dist_batch_status].status_date))
	FROM [cards] 
		INNER JOIN [dist_batch_cards]
			ON [cards].card_id = [dist_batch_cards].card_id
		INNER JOIN [dist_batch_status]
			ON [dist_batch_status].dist_batch_id = [dist_batch_cards].dist_batch_id
	WHERE [dist_batch_status].dist_batch_statuses_id = 11
		AND [cards].card_production_date IS NULL



CLOSE SYMMETRIC KEY Indigo_Symmetric_Key