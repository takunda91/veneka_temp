-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_issue_card_complete] 
	@card_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [ISSUE_CARD_COMPLETE_TRAN]
		BEGIN TRY 
			
			DECLARE @current_card_status_id int,
					@branch_id int,
					@status_date datetimeoffset,
					@operator_user_id bigint
					

			--get the current status and operator for the card
			SELECT @current_card_status_id = branch_card_statuses_id, @operator_user_id = operator_user_id
			FROM branch_card_status_current
			WHERE card_id = @card_id
										  
			--Check that someone hasn't already updated the card
			IF(@current_card_status_id = 8 OR @current_card_status_id = 9)				
				BEGIN

					SET @status_date = SYSDATETIMEOFFSET()

					SELECT @branch_id = branch_id
					FROM [cards]
					WHERE card_id = @card_id

					--Update the cards status.
					UPDATE branch_card_status 
					SET branch_id = @branch_id, 
						branch_card_statuses_id = 6, 
						status_date = @status_date, 
						[user_id] = @audit_user_id, 
						operator_user_id = @audit_user_id,
						branch_card_code_id = NULL,
						comments = NULL,
						pin_auth_user_id = NULL
					OUTPUT Deleted.* INTO branch_card_status_audit
					WHERE card_id = @card_id	

					--INSERT branch_card_status
					--		(card_id, branch_id, branch_card_statuses_id, status_date, [user_id], operator_user_id)
					--VALUES (@card_id, @branch_id, 6, @status_date, @audit_user_id, @operator_user_id) 

					OPEN SYMMETRIC KEY Indigo_Symmetric_Key
					DECRYPTION BY CERTIFICATE Indigo_Certificate

					DECLARE @cardnumber varchar(50),
							@branch_card_status_name varchar(50),
							@audit_msg varchar(max)

					SELECT @cardnumber = CONVERT(VARCHAR,DECRYPTBYKEY(cards.card_number)) 
					FROM cards 
					WHERE cards.card_id = @card_id

					SELECT @branch_card_status_name = branch_card_statuses_name
					FROM [branch_card_statuses]
					WHERE branch_card_statuses_id = 6

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

					--If the card must be updated remotely add pending record
					INSERT INTO [dbo].[remote_update_status] (card_id, comments, remote_component, remote_update_statuses_id, remote_updated_time, status_date, [user_id])
					SELECT TOP 1 [dbo].[cards].card_id, 'Auto Insert', 'INDIGO_SYSTEM', 0, null, SYSDATETIMEOFFSET(), @audit_user_id
					FROM [dbo].[cards] INNER JOIN [dbo].[issuer_product]
						ON [cards].product_id = [issuer_product].product_id
					WHERE [cards].card_id = @card_id AND 
							[issuer_product].remote_cms_update_YN = 1

					--NOTIFICATION
					EXEC usp_notification_branch_add @card_id, 6

					SET @audit_msg = '' + COALESCE(@branch_card_status_name, 'UNKNOWN') + 
									 ', ' + dbo.MaskString(@cardnumber, 6, 4)
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
										 3,
										 NULL, 
										 @audit_workstation, 
										 @audit_msg, 
										 NULL, NULL, NULL, NULL

					SET @ResultCode = 0					
				END
			ELSE
				BEGIN
					SET @ResultCode = 100
				END
			

				COMMIT TRANSACTION [ISSUE_CARD_COMPLETE_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_COMPLETE_TRAN]
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