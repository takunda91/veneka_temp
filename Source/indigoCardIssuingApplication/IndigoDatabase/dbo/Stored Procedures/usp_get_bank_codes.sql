Create PROCEDURE [dbo].[usp_get_bank_codes]
	@issuer_id INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
		bank_code 
	FROM 
		rswitch_crf_bank_codes 
	WHERE 
		issuer_id = @issuer_id
END