-- =============================================
-- Author:		LTladi
-- Create date: 20150210
-- Description:	Search for terminal by branch or masterkey
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_terminal]
	@terminal_id INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN
		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate; 

    SELECT 
		[terminal_name]
		, [terminal_model]
		, CONVERT(varchar(max),DECRYPTBYKEY(terminals.device_id)) AS 'device_Id'
		, b.[branch_id]
		, CONVERT(varchar(max),DECRYPTBYKEY(m.masterkey)) AS 'masterkey'
		, i.issuer_id,m.masterkey_id
		, CONVERT(varchar(max),DECRYPTBYKEY(terminals.password)) AS 'password'
		,IsMacUsed
	FROM
		[terminals]
		INNER JOIN [masterkeys] m ON m.masterkey_id = terminal_masterkey_id
		INNER JOIN [branch] b ON b.[branch_id] = terminals.branch_id
		INNER JOIN [issuer] i ON i.issuer_id = b.issuer_id
	WHERE
		[terminal_id] = @terminal_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

	END

END