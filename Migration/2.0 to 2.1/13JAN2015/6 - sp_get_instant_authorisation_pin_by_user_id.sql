USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_authpin_by_user_id]    Script Date: 2015/01/12 11:51:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Lebo Tladi
-- Create date:
-- Description:	Returns decrypted instant authorisation pin
-- =============================================
 CREATE PROCEDURE [dbo].[sp_get_authpin_by_user_id] 
	@user_id bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		--For the current password add 99 days to show it's the latest. old passwords should be in the past.
		SELECT CONVERT(VARCHAR,DECRYPTBYKEY([user].instant_authorisation_pin)) as 'Instant Auth Pin', DATEADD(DAY, 99, GETDATE()) as 'date'						
		FROM [user]
		WHERE [user].[user_id] = @user_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END







