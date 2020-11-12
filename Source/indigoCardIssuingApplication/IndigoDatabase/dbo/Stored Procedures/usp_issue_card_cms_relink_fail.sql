-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_issue_card_cms_relink_fail] 
	@card_id bigint,	
	@error varchar(1000), 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION [ISSUE_CARD_CMS_RELINK_FAIL_TRAN]
		BEGIN TRY 

			DECLARE @branch_card_code_id int,
					@current_card_status_id int,
					@branch_id int,
					@status_date datetimeoffset

			--try find the error in response mapping as to link to code.
			--SELECT TOP 1 @branch_card_code_id = branch_card_code_id
			--FROM [mod_response_mapping]
			--WHERE @error LIKE '%' + response_contains + '%'

			--Check if a valid code was found for error, if not set as UNKNOWN Error
			IF @branch_card_code_id IS NULL
				SET @branch_card_code_id = 7

			SELECT @branch_id = branch_id
			FROM [cards]
			WHERE card_id = @card_id

			--Update the cards status.
			UPDATE branch_card_status 
			SET branch_id = @branch_id, 
				branch_card_statuses_id = 9, 
				status_date = @status_date, 
				[user_id] = @audit_user_id, 
				operator_user_id = @audit_user_id,
				branch_card_code_id = @branch_card_code_id,
				comments = @error,
				pin_auth_user_id = NULL
			OUTPUT Deleted.* INTO branch_card_status_audit
			WHERE card_id = @card_id

			--INSERT branch_card_status
			--		(card_id, branch_id, branch_card_statuses_id, status_date, [user_id], operator_user_id, 
			--		 branch_card_code_id, comments)
			--VALUES (@card_id, @branch_id, 9, @status_date, @audit_user_id, @audit_user_id, @branch_card_code_id, @error) 


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
			WHERE branch_card_statuses_id = 9

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

			SET @audit_msg = '' + COALESCE(@branch_card_status_name, 'UNKNOWN') + 
							 ', ' + dbo.MaskString(@cardnumber, 6, 4)+
							 ', ' + @error
								
			--log the audit record		
			EXEC usp_insert_audit @audit_user_id, 
									3,
									NULL, 
									@audit_workstation, 
									@audit_msg, 
									NULL, NULL, NULL, NULL

			

			COMMIT TRANSACTION [ISSUE_CARD_CMS_RELINK_FAIL_TRAN]

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_CMS_RELINK_FAIL_TRAN]
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