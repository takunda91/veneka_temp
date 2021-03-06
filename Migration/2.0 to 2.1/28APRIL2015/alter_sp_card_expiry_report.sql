USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_card_expiry_report]    Script Date: 2015/04/28 02:44:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sp_card_expiry_report] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = NULL,
	@branch_id int = NULL,
	@date_from datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @date_from = DATEADD(M, 1, @date_from)

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT 
			DISTINCT  
				CASE WHEN [issuer].card_ref_preference = 1 
				THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4)
				ELSE CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) 
				END AS 'card_number'
			, cards.card_request_reference AS card_reference_number
			, CONVERT(DATETIME, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_expiry_date))) AS card_expiry_date
			, [issuer].issuer_name
			, [issuer].issuer_code
			, [branch].branch_name
			, branch.branch_code
		FROM [cards]								
				INNER JOIN [branch]
					ON [cards].branch_id = [branch].branch_id
				INNER JOIN [issuer]
					ON [branch].issuer_id = [issuer].issuer_id	
		WHERE [cards].card_issue_method_id = 0
				AND [issuer].issuer_id = COALESCE(@issuer_id, [issuer].issuer_id)
				AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
				AND DATEPART(m, CONVERT(DATETIME, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_expiry_date)))) = DATEPART(m, @date_from)
				AND DATEPART(yy, CONVERT(DATETIME, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_expiry_date)))) = DATEPART(yy, @date_from)
		ORDER BY issuer_name, issuer_code, branch_name, branch_code, card_expiry_date

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END
