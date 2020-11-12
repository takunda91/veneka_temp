-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Persist changes to the branch to the DB
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_branch] 
	@branch_id int,
	@branch_status_id int,
	@issuer_id int,
	@branch_code varchar(10),
	@branch_name varchar(30),
	@location varchar(20),
	@contact_person varchar(30),
	@contact_email varchar(30),
	@card_centre varchar(10),	
	@emp_branch_code varchar(10),
	@satellite_branch_id varchar(500), 
	@branch_type_id int ,
	@main_branch_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	

	BEGIN TRANSACTION [UPDATE_BRANCH_TRAN]
		BEGIN TRY 

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [branch] WHERE ([branch_code] = @branch_code AND [issuer_id] = @issuer_id) AND branch_id != @branch_id) > 0
				BEGIN
					SET @ResultCode = 211						
				END
			ELSE IF (SELECT COUNT(*) FROM [branch] WHERE ([branch_name] = @branch_name AND [issuer_id] = @issuer_id) AND branch_id != @branch_id) > 0
				BEGIN
					SET @ResultCode = 210
				END			
			ELSE IF (SELECT count(*) fROM branch where @branch_type_id =2 and main_branch_id = @branch_id AND [issuer_id] = @issuer_id AND branch_id != @branch_id) > 0
				BEGIN
					SET @ResultCode = 807
				END
			ELSE
			BEGIN

				UPDATE [branch]
				SET [branch_status_id] = @branch_status_id,
					[issuer_id] = @issuer_id,
					[branch_code] = @branch_code,
					[branch_name] = @branch_name,
					[location] = @location,
					[contact_person] = @contact_person,
					[contact_email] = @contact_email,
					[card_centre] = @card_centre,
					[branch_type_id]=@branch_type_id,
					[main_branch_id]=@main_branch_id,
					[emp_branch_code]=@emp_branch_code
				WHERE branch_id = @branch_id

				if(@satellite_branch_id <>' ')
				BEGIN
				
				EXECUTE('update [branch] set branch_type_id=2 ,main_branch_id='+@branch_id+' where branch_id in ('+@satellite_branch_id+')')
				
				EXECUTE('update [branch] set branch_type_id=2 ,main_branch_id=-1 where branch_id not in ('+@satellite_branch_id+') and main_branch_id='+@branch_id)
				 --set @satellite_branch_id = null
				END

				--log the audit record
				DECLARE @audit_description varchar(500),
						@branchstatus  varchar(50),
				        @issuer_code varchar(10)

				SELECT @issuer_code = issuer_code
				FROM issuer
				WHERE issuer_id = @issuer_id

				SELECT @branchstatus = branch_statuses.[branch_status]
				FROM branch_statuses 
				WHERE branch_statuses.branch_status_id = @branch_status_id

				SELECT @audit_description = 'Update: ID ' + CAST(@branch_id AS varchar(max)) + '; [' + CAST(@issuer_id as varchar(100)) + ',' + @issuer_code + ']; [' +
											@branch_code + ',' + @branch_name + ',' + @branchstatus + ']'

				EXEC usp_insert_audit @audit_user_id, 
									 0,--BranchAdmin
									 NULL,
									 @audit_workstation, 
									 @audit_description, 
									 @issuer_id, NULL, NULL, NULL

				SET @ResultCode  = 0				
			END

			COMMIT TRANSACTION [UPDATE_BRANCH_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_BRANCH_TRAN]
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
