-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_card_pvv] 
	-- Add the parameters for the stored procedure here
	@card_id bigint,
	@pvv_lmk varchar(50),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [CARD_PVV]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			UPDATE [cards]
			SET pvv = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR(max), @pvv_lmk))
			WHERE card_id = @card_id

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

			COMMIT TRANSACTION [CARD_PVV]			
		END TRY

		BEGIN CATCH
			ROLLBACK TRANSACTION [CARD_PVV]
			DECLARE @ErrorMessage NVARCHAR(4000);
			DECLARE @ErrorSeverity INT;
			DECLARE @ErrorState INT;

			SELECT 
				@ErrorMessage = ERROR_MESSAGE(),
				@ErrorSeverity = ERROR_SEVERITY(),
				@ErrorState = ERROR_STATE();

			RAISERROR (@ErrorMessage, -- Message text.
					   @ErrorSeverity, -- Severity.
					   @ErrorState -- State.
					   );
		END CATCH	

END