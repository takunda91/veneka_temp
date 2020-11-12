CREATE PROCEDURE [dbo].[usp_get_product_accounttypes_mappings]
	@product_id int = NULL,
	@cbs_account_type varchar(50)=NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    
	SELECT [cbs_account_type],indigo_account_type,[cms_account_type]
	FROM product_account_types_mapping
	WHERE product_id = COALESCE(@product_id, product_id) 
	AND [cbs_account_type] = COALESCE(@cbs_account_type, [cbs_account_type])
	 

END
