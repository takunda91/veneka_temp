
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_fee_accounting] 
	-- Add the parameters for the stored procedure here
	@fee_accounting_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;


	SELECT [fee_accounting_id]
      ,[fee_accounting_name]
      ,[issuer_id]      
      ,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([fee_revenue_account_no])) as [fee_revenue_account_no]
      ,[fee_revenue_account_type_id]
	  ,[fee_revenue_branch_code]
      ,[fee_revenue_narration_en]
      ,[fee_revenue_narration_fr]
      ,[fee_revenue_narration_pt]
      ,[fee_revenue_narration_es]
      ,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([vat_account_no])) as [vat_account_no]
      ,[vat_account_type_id]
      ,[vat_account_branch_code]
      ,[vat_narration_en]
      ,[vat_narration_fr]
      ,[vat_narration_pt]
      ,[vat_narration_es]
	  ,0 as TOTAL_ROWS
	  ,CONVERT(bigint, 0) AS ROW_NO
	  ,0 as TOTAL_PAGES
	FROM [product_fee_accounting]
	WHERE fee_accounting_id = @fee_accounting_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END