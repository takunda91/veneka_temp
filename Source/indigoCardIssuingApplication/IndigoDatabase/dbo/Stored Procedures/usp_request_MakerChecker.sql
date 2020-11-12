CREATE PROCEDURE [dbo].[usp_request_MakerChecker] 
	@request_id bigint,
	@approve bit,
	@notes varchar(1000),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@card_issue_method_id int OUTPUT,
	@ResultCode int OUTPUT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [MAKER_CHECKER_TRAN]
		BEGIN TRY 

			DECLARE @current_status int,
					@branch_id int				

			--Get the issue method of the card
			SELECT @card_issue_method_id = card_issue_method_id, @branch_id = branch_id
			FROM [hybrid_requests]
			where request_id = @request_id

			SELECT @current_status = hybrid_request_statuses_id
			FROM [hybrid_request_status_current]
			where request_id = @request_id			

			IF(@current_status !=0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN
					OPEN SYMMETRIC KEY Indigo_Symmetric_Key
					DECRYPTION BY CERTIFICATE Indigo_Certificate

					DECLARE @status_date datetimeoffset,
							@new_status_id int,
							@operator_username varchar(100)

					--Operator username needed for audit
					SELECT @operator_username = CONVERT(VARCHAR,DECRYPTBYKEY(username))
					FROM [hybrid_request_status_current]
							INNER JOIN [user]
								ON [hybrid_request_status_current].operator_user_id = [user].[user_id]
					WHERE [hybrid_request_status_current].request_id = @request_id

					SET @status_date = SYSDATETIMEOFFSET()

					IF(@approve = 1)
						SET @new_status_id = 1
					ELSE
						SET @new_status_id = 3---rejected

					--Update Branch Cards status with checked out cards
					UPDATE hybrid_request_status 
					SET branch_id = @branch_id, 
						hybrid_request_statuses_id = @new_status_id, 
						status_date = @status_date, 
						[user_id] = @audit_user_id, 
						operator_user_id = operator_user_id,
						comments = @notes
						
					OUTPUT Deleted.* INTO hybrid_request_status_audit
					WHERE request_id = @request_id	

					

					--log the audit record
					DECLARE @audit_description varchar(max),
					        @requestcardstatus  varchar(50),
							
							@referencenumber varchar(50)			

					SELECT  @requestcardstatus =  hybrid_request_statuses
					FROM    hybrid_request_statuses 
					WHERE	hybrid_request_statuses_id = @new_status_id

					
					 
					SELECT	@referencenumber = [hybrid_requests].request_reference
					FROM
						[hybrid_requests] 				
						INNER JOIN [branch] ON [hybrid_requests].branch_id = [branch].branch_id
						INNER JOIN [issuer] ON [branch].issuer_id = [issuer].issuer_id	
					WHERE	request_id = @request_id					

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;


					--NOTIFICATION for approval
					--exec usp_notification_branch_add @card_id, @new_status_id

					IF(@approve = 0)
						BEGIN
							SET @audit_description = COALESCE(@requestcardstatus, 'UNKNOWN') + '(reject)' + 													
													', ' + @referencenumber +
													', to ' + COALESCE(@operator_username, 'UNKOWN')
						END
					ELSE
						BEGIN
							SET @audit_description = COALESCE(@requestcardstatus, 'UNKNOWN') +
													 ', ' + @referencenumber
						END					
						
					EXEC usp_insert_audit @audit_user_id, 
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
