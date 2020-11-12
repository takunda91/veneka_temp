-- =============================================
-- Author:		Lebo Tladi
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_products_for_export] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@only_active_records bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

    SELECT [issuer_product].product_id,
			[issuer_product].product_code,
			[issuer_product].product_name,
			[issuer_product].product_bin_code,
			ISNULL([issuer_product].sub_product_code, '') as sub_product_code,
			[issuer_product].card_issue_method_id,
			[issuer_product].product_load_type_id,
			[issuer_product].DeletedYN,
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([issuer_product].CVKA)) as CVKA, 
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([issuer_product].CVKB)) as CVKB, 
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([issuer_product].PVK)) as PVK, 
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([issuer_product].PVKI)) as PVKI,
			[issuer_product].src1_id, [issuer_product].src2_id, [issuer_product].src3_id,
			[issuer_product].issuer_id
	FROM [issuer_product]
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [issuer_product].issuer_id					
	WHERE [issuer_product].issuer_id = COALESCE(@issuer_id, [issuer_product].issuer_id)
			AND [issuer_product].cms_exportable_YN = 1
			AND ((@only_active_records = 0) OR (@only_active_records = 1
												AND [issuer_product].DeletedYN = 0
												AND [issuer].issuer_status_id = 0))

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END