USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_branchcardstock_report]    Script Date: 2015/05/11 03:46:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [sp_get_branchcardstock_report] null,3,0

ALTER PROCEDURE [dbo].[sp_get_branchcardstock_report]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(@issuer_id = -1 or @issuer_id = 0)
	 set @issuer_id=null

	if(@branch_id  =0)
		set @branch_id = null
    -- Insert statements for procedure here
	 OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT distinct 
		[branch].branch_id,branch.branch_code
			,[dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) AS 'card_number'
		, [cards].card_request_reference as 'card_request_reference'
		, Convert(datetime, CONVERT(varchar,DECRYPTBYKEY([cards].card_expiry_date))) as 'card_expiry_date'
		,[dist_batch].date_created as'card_production_date'
	FROM 
		[branch_card_status_current] 
		INNER JOIN [cards] ON [branch_card_status_current].card_id = [cards].card_id
		INNER JOIN [dist_batch_cards]
				ON [cards].card_id = [dist_batch_cards].card_id
			INNER JOIN [dist_batch]
				ON [dist_batch_cards].dist_batch_id = [dist_batch].dist_batch_id
					AND [dist_batch].dist_batch_type_id = 0
		INNER JOIN [branch] ON [cards].branch_id = [branch].branch_id
		INNER JOIN [issuer] ON issuer.issuer_id = branch.issuer_id
	WHERE
		([branch_card_status_current].branch_card_statuses_id = 1 
			OR [branch_card_status_current].branch_card_statuses_id = 0)
		AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
		AND [branch].branch_id = COALESCE(null,  [branch].branch_id)	
	ORDER BY
		[branch].branch_id
		
			
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END
