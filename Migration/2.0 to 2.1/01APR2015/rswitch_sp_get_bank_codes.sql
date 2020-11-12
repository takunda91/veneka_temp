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
-- Create date: 20150401	
-- Description:	Get bank codes
-- =============================================
CREATE PROCEDURE sp_get_bank_codes
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
GO
