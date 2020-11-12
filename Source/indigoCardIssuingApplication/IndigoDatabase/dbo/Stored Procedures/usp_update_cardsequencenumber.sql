-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_cardsequencenumber]
	@product_id int,
	@newSequenceNumber int,
	@auditUserId bigint,
	@auditWorkStation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

       OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	   DECRYPTION BY CERTIFICATE Indigo_Certificate;

			UPDATE integration_cardnumbers 
			SET  card_sequence_number= ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@newSequenceNumber))
			WHERE product_id = @product_id

		CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END