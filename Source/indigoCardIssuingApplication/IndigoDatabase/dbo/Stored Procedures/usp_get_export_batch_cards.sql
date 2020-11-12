-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch all cards linked to distribution batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_export_batch_cards] 
	@export_batch_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_EXPORT_BATCH_CARDS]
		BEGIN TRY 
			DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				SELECT distinct
					CASE 
						WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
						ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
					END AS 'card_number'
					, cards.card_request_reference AS card_reference_number
					, '' as [export_batch_statuses_name]				   
				FROM [cards]
								INNER JOIN [export_batch] ON cards.export_batch_id = [export_batch].export_batch_id
						--INNER JOIN [export_batch_status]
						--	ON [export_batch].export_batch_id = [export_batch_status].export_batch_id
						--INNER JOIN [export_batch_statuses]
						--	ON [export_batch_status].export_batch_statuses_id = [export_batch_statuses].export_batch_statuses_id
					INNER JOIN [branch] ON [branch].branch_id = cards.branch_id
					INNER JOIN [issuer] ON branch.issuer_id = issuer.issuer_id
				WHERE 
				[cards].export_batch_id = @export_batch_id
					
														   
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting cards for distribution batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_EXPORT_BATCH_CARDS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_EXPORT_BATCH_CARDS]
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