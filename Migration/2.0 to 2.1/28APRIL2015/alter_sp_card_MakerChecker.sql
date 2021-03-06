USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_card_MakerChecker]    Script Date: 2015/04/28 02:48:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
ALTER PROCEDURE [dbo].[sp_card_MakerChecker] 
	@card_id bigint,
	@approve bit,
	@notes varchar(1000),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [MAKER_CHECKER_TRAN]
		BEGIN TRY 

			DECLARE @current_status int					

			SELECT @current_status = branch_card_statuses_id
			FROM [branch_card_status_current]
			WHERE card_id = @card_id			

			IF(@current_status != 2)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN
					OPEN SYMMETRIC KEY Indigo_Symmetric_Key
					DECRYPTION BY CERTIFICATE Indigo_Certificate

					DECLARE @status_date datetime,
							@new_status_id int,
							@operator_username varchar(100),
							@card_issue_method_id int

					--Operator username needed for audit
					SELECT @operator_username = CONVERT(VARCHAR,DECRYPTBYKEY(username))
					FROM [branch_card_status_current]
							INNER JOIN [user]
								ON [branch_card_status_current].operator_user_id = [user].[user_id]
					WHERE [branch_card_status_current].card_id = @card_id

					--Get the issue method of the card
					SELECT @card_issue_method_id = card_issue_method_id
					FROM [cards]
					where card_id = @card_id
					

					SET @status_date = GETDATE()

					IF(@approve = 1)
						SET @new_status_id = 3
					ELSE
						BEGIN
							IF (@card_issue_method_id = 1)
								BEGIN
									SET @new_status_id = 1

									--Delete the customer information, it is not needed.
									DELETE FROM customer_account
									WHERE card_id = @card_id
								END
							ELSE
								SET @new_status_id = 11
						END

					--Update Branch Cards status with checked out cards
					INSERT INTO [branch_card_status]
									(card_id, operator_user_id, status_date, [user_id], branch_card_statuses_id, comments)
					SELECT @card_id, [branch_card_status_current].operator_user_id, @status_date, @audit_user_id, @new_status_id, @notes
					FROM [branch_card_status_current]
					WHERE [branch_card_status_current].card_id = @card_id


					--log the audit record
					DECLARE @audit_description varchar(max),
					        @branchcardstatus  varchar(50),
							@cardnumber varchar(50)	,
							@cardreferencenumber varchar(50)			

					SELECT  @branchcardstatus =  branch_card_statuses_name
					FROM    branch_card_statuses 
					WHERE	branch_card_statuses_id = @new_status_id

					
					 
					SELECT	@cardnumber = 
						CASE WHEN [issuer].card_ref_preference = 1 
						THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4)
						ELSE CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) END
						, @cardreferencenumber = [cards].card_request_reference
					FROM
						cards 				
						INNER JOIN [branch] ON [cards].branch_id = [branch].branch_id
						INNER JOIN [issuer] ON [branch].issuer_id = [issuer].issuer_id	
					WHERE	card_id = @card_id					

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

					IF(@approve = 0)
						BEGIN
							SET @audit_description = COALESCE(@branchcardstatus, 'UNKNOWN') + '(reject)' + 
													', ' + @cardnumber + 
													', ' + @cardreferencenumber +
													', to ' + COALESCE(@operator_username, 'UNKOWN')
						END
					ELSE
						BEGIN
							SET @audit_description = COALESCE(@branchcardstatus, 'UNKNOWN') +
													', ' + @cardnumber + ', ' + @cardreferencenumber
						END					
						
					EXEC sp_insert_audit @audit_user_id, 
										 3, ---IssueCard
										 @status_date, 
										 @audit_workstation, 
										 @audit_description, 
										 NULL, NULL, NULL, NULL

					SET @ResultCode = 0
				END

			COMMIT TRANSACTION [MAKER_CHECKER_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [MAKER_CHECKER_TRAN]
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






