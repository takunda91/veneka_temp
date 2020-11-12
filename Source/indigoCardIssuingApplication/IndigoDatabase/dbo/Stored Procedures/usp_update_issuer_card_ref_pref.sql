-- =============================================
-- Author:		Nduvho Mukhavhuli
-- Create date: 2014/09/29	
-- =============================================

CREATE PROCEDURE [dbo].[usp_update_issuer_card_ref_pref]
	@issuer_id int,
	@selectedOption int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_ISSUER_CARD_REF_PREF]
		BEGIN TRY 			
			BEGIN
				UPDATE [dbo].[issuer]
				SET [card_ref_preference] = @selectedOption					
				WHERE [issuer_id] = @issuer_id

				SELECT @ResultCode = 0
				COMMIT TRANSACTION [UPDATE_ISSUER_CARD_REF_PREF]				
			END
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_ISSUER_CARD_REF_PREF]
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