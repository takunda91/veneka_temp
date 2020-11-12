-- =============================================
-- Author:		LTladi	
-- Create date: 20150220
-- Description:	Get masterkey
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_masterkey]
	@masterkey_id INT
AS
BEGIN
	SET NOCOUNT ON;
	
		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate; 

		SELECT
			[masterkey_name]
			, CONVERT(varchar(max),DECRYPTBYKEY([masterkey])) AS 'masterkey'
			,[issuer_id]
		FROM 
			[masterkeys]
		WHERE 
			[masterkey_id] = @masterkey_id

		CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END