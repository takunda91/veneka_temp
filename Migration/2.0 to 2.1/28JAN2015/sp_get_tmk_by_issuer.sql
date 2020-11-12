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
-- Create date: 20150213
-- Description:	Get Masterkeys for issuer
-- =============================================
CREATE PROCEDURE sp_get_tmk_by_issuer
	@issuer_id INT
	, @branch_id INT
AS
BEGIN

	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT CONVERT(VARCHAR,DECRYPTBYKEY([masterkeys].masterkey)) as 'masterkey',
				[masterkeys].issuer_id
		FROM [masterkeys]
				INNER JOIN [issuer]
					ON [issuer].issuer_id = [masterkeys].issuer_id
						AND [issuer].issuer_status_id = 0
		WHERE [masterkeys].issuer_id = @issuer_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END
GO
