USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_dist_batch_cards]    Script Date: 2015/04/24 06:14:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch all cards linked to distribution batch
-- =============================================
ALTER PROCEDURE [dbo].[sp_get_dist_batch_cards] 
	@dist_batch_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_DIST_BATCH_CARDS]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				SELECT 
					[dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) as 'card_number', 
						[dist_card_statuses].dist_card_status_name					   
				FROM [cards]
					INNER JOIN [dist_batch_cards] 
						ON [dist_batch_cards].card_id = [cards].card_id	
					INNER JOIN [dist_card_statuses]
						ON 	[dist_batch_cards].dist_card_status_id = [dist_card_statuses].dist_card_status_id
				WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id
					
														   
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;		

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting cards for distribution batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC sp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_DIST_BATCH_CARDS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_DIST_BATCH_CARDS]
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







