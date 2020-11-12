CREATE PROCEDURE [dbo].[usp_get_terminal_parameters] 
	-- Add the parameters for the stored procedure here
	@product_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT product_bin_code, product_name, product_code, product_id, min_pin_length, max_pin_length, 
			[issuer_product].enable_instant_pin_YN as product_enable_instant_pin_YN,
			[issuer_product].enable_instant_pin_reissue_YN, DeletedYN, 
			[issuer].issuer_status_id , 
			[issuer].enable_instant_pin_YN as issuer_enable_instant_pin_YN
	FROM [issuer_product]
		INNER JOIN [issuer]
			ON [issuer_product].issuer_id = [issuer].issuer_id
	WHERE product_id = @product_id
END