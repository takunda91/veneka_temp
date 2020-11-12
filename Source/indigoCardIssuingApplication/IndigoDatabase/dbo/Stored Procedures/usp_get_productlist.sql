-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_productlist]
	@issuer_id int =null,
	@card_issue_method_id int = null,
	@Deleted_YN bit = NULL,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY product_name ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
		SELECT DISTINCT issuer_product.product_code,
			   issuer_product.product_name,
			   issuer_product.product_id,
			   issuer_product.product_bin_code,			   
			   issuer_product.issuer_id, 
			   issuer_product.pan_length,
			   issuer_product.sub_product_code,
			   issuer_product.pin_calc_method_id,
			   issuer_product.auto_approve_batch_YN,
			   issuer_product.account_validation_YN,
			   issuer_product.name_on_card_top,                
			   issuer_product.name_on_card_left, 
			   issuer_product.Name_on_card_font_size, 
			   issuer_product.font_id, 
			   Issuer_product_font.font_name, 
               Issuer_product_font.resource_path,
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.PVK)) as PVK, 
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.PVKI)) as PVKI, 
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.CVKB)) as CVKB, 			   
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.CVKA)) as CVKA,
			   src1_id, src2_id, src3_id,
			   issuer_product.expiry_months,
			   issuer_product.fee_scheme_id,
			   issuer_product.enable_instant_pin_YN,
			   issuer_product.min_pin_length,
			   issuer_product.max_pin_length,
			   issuer_product.enable_instant_pin_reissue_YN,
			   issuer_product.cms_exportable_YN,
			   issuer_product.product_load_type_id,
			   issuer_product.pin_mailer_printing_YN,
			   issuer_product.pin_mailer_reprint_YN,
			   issuer_product.card_issue_method_id,
			   issuer_product.charge_fee_to_issuing_branch_YN,
			   issuer_product.production_dist_batch_status_flow,
			   issuer_product.distribution_dist_batch_status_flow,
			     CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.decimalisation_table)) as decimalisation_table, 
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.pin_validation_data)) as pin_validation_data,
			   pin_block_formatid,
			   issuer_product.DeletedYN,
			   issuer_product.print_issue_card_YN,
			   [issuer_product].allow_manual_uploaded_YN,
			   [issuer_product].allow_reupload_YN,
			   [issuer_product].remote_cms_update_YN,
			   			 case when [issuer_product].charge_fee_at_cardrequest is null then cast(0 as bit) else [issuer_product].charge_fee_at_cardrequest end as charge_fee_at_cardrequest,
			   			 case when issuer_product.e_pin_request_YN is null then cast(0 as bit) else [issuer_product].e_pin_request_YN end as e_pin_request_YN
						   
		FROM issuer_product 
				INNER JOIN Issuer_product_font 
					ON issuer_product.font_id = Issuer_product_font.font_id	
		WHERE issuer_product.issuer_id= @issuer_id 
				AND issuer_product.DeletedYN = COALESCE(@Deleted_YN, issuer_product.DeletedYN)
				AND card_issue_method_id = COALESCE(@card_issue_method_id, card_issue_method_id)
						
		)
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY product_name ASC
END
