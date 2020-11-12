USE [indigo_database_main_dev]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_issue_card_check_account_balance]
	@fee_charged decimal(10,4) = NULL,
	@accountbalance decimal(10,4) = NULL,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [ISSUE_CARD_CHECK_ACCOUNT_BALANCE]
		BEGIN TRY 
				IF (@accountbalance - @fee_charged > 0)
					 BEGIN
						SET @ResultCode = 1
					END
			ELSE
				BEGIN
					SET @ResultCode = 507
				END


				COMMIT TRANSACTION [ISSUE_CARD_CHECK_ACCOUNT_BALANCE]
		END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_CHECK_ACCOUNT_BALANCE]
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
