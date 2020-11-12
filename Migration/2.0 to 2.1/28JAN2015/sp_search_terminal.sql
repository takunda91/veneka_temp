-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi
-- Create date: 20150210
-- Description:	Search for terminal by branch or masterkey
-- =============================================
CREATE PROCEDURE sp_search_terminal
	@terminal_id INT
	, @branch_id INT
	, @masterkey_id INT
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
		, CONVERT(varchar,DECRYPTBYKEY(terminals.device_id)) AS 'device_Id'
		, [branch_id]
		, CONVERT(varchar,DECRYPTBYKEY(m.masterkey)) AS 'masterkey'
	FROM
		[terminals]
		INNER JOIN [masterkeys] m ON m.masterkey_id = terminal_masterkey_id
	WHERE
		[terminal_id] = @terminal_id
		OR [branch_id] = @branch_id
		OR [masterkey_id] = @masterkey_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

	END

END
GO
