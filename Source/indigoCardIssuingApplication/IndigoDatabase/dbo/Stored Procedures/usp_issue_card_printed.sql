-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_issue_card_printed] 
	@card_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [ISSUE_CARD_PRINTED_TRAN]
		BEGIN TRY 
			
			DECLARE @maker_checker bit,
			        @current_card_status_id int,
					@card_issue_method_id int,
					@branch_id int,
					@status_date datetimeoffset
					

			--get the current status for the card
			SELECT @current_card_status_id = branch_card_statuses_id,
					@card_issue_method_id = [cards].card_issue_method_id
			FROM branch_card_status_current INNER JOIN [cards]
				ON branch_card_status_current.card_id = [cards].card_id
			WHERE [cards].card_id = @card_id

			SELECT @maker_checker = [issuer].maker_checker_YN
			FROM [issuer] INNER JOIN [branch]
					ON [issuer].issuer_id = [branch].issuer_id
					INNER JOIN [cards]
					ON [branch].branch_id = [cards].branch_id
			WHERE [cards].card_id = @card_id
										  
			--Check that someone hasn't already updated the card
							
			IF ( (@card_issue_method_id = 1 AND @current_card_status_id = 2 AND @maker_checker = 0) OR 
				 (@card_issue_method_id = 1 AND @current_card_status_id = 3 AND @maker_checker = 1) OR
				 (@card_issue_method_id = 0 AND @current_card_status_id = 0))
				BEGIN

					SET @status_date = SYSDATETIMEOFFSET()

					SELECT @branch_id = branch_id
					FROM [cards]
					WHERE card_id = @card_id

					--Update the cards status.
					UPDATE branch_card_status 
					SET branch_id = @branch_id, 
						branch_card_statuses_id = 4, 
						status_date = @status_date, 
						[user_id] = @audit_user_id, 
						operator_user_id = @audit_user_id,
						branch_card_code_id = NULL,
						comments = NULL,
						pin_auth_user_id = NULL
					OUTPUT Deleted.* INTO branch_card_status_audit
					WHERE card_id = @card_id					 

					--INSERT branch_card_status
					--		(card_id, branch_id, branch_card_statuses_id, status_date, [user_id], operator_user_id, branch_card_code_id)
					--VALUES (@card_id, @branch_id, 4, @status_date, @audit_user_id, @audit_user_id, 0) 


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
					WHERE branch_card_statuses_id = 4

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

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

				COMMIT TRANSACTION [ISSUE_CARD_PRINTED_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_PRINTED_TRAN]
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