-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Persist new branch to db
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_branch] 
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
	@new_branch_id int OUTPUT,
	@ResultCode int OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	

		

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [branch] WHERE ([branch_code] = @branch_code AND [issuer_id] = @issuer_id)) > 0
				BEGIN
					SET @new_branch_id = 0
					SET @ResultCode = 211						
				END
			ELSE IF (SELECT COUNT(*) FROM [branch] WHERE ([branch_name] = @branch_name AND [issuer_id] = @issuer_id)) > 0
				BEGIN
					SET @new_branch_id = 0
					SET @ResultCode = 210
				END
			ELSE
			BEGIN	
			
			BEGIN TRANSACTION [INSERT_BRANCH_TRAN]
				BEGIN TRY 		

				INSERT INTO [branch]
						([branch_status_id],[issuer_id],[branch_code],[branch_name],[location]
						,[contact_person],[contact_email],[card_centre],[emp_branch_code],[branch_type_id],[main_branch_id])
					VALUES
						(@branch_status_id, @issuer_id, @branch_code, @branch_name, @location,
						@contact_person, @contact_email, @card_centre,@emp_branch_code,@branch_type_id,@main_branch_id)

				SET @new_branch_id = SCOPE_IDENTITY();

				if(@satellite_branch_id <>' ')
				BEGIN
				EXECUTE('update [branch] set branch_type_id=2 ,main_branch_id='+@new_branch_id+' where branch_id in ('+@satellite_branch_id+')')
				END

				DECLARE @issuer_code varchar(10)
				SELECT @issuer_code = issuer_code
				FROM issuer
				WHERE issuer_id = @issuer_id

				DECLARE @group_name varchar(50),
						@new_group_id int
				SET @group_name =  @issuer_code + '_' + @branch_code + '_CUSTODIAN'

				--Insert Default user groups
				INSERT INTO [user_group]
					(all_branch_access, can_create, can_delete, can_read, can_update, issuer_id,
						user_group_name, user_role_id, mask_report_pan, mask_screen_pan)
				VALUES 
					(0, 1, 1, 1, 1, @issuer_id, @group_name, 2, 1, 1)

				SET @new_group_id = SCOPE_IDENTITY();

				INSERT INTO [user_groups_branches]
					(branch_id, user_group_id)
				VALUES 
					(@new_branch_id, @new_group_id)

				SET @group_name =  @issuer_code + '_' + @branch_code + '_OPERATOR'
				INSERT INTO [user_group]
					(all_branch_access, can_create, can_delete, can_read, can_update, issuer_id,
						user_group_name, user_role_id, mask_report_pan, mask_screen_pan)
				VALUES 
					(0, 1, 1, 1, 1, @issuer_id, @group_name, 3, 1, 1)

				SET @new_group_id = SCOPE_IDENTITY();

				INSERT INTO [user_groups_branches]
					(branch_id, user_group_id)
				VALUES 
					(@new_branch_id, @new_group_id)

				--log the audit record
				DECLARE @audit_description varchar(500)
				DECLARE @branchstatus  varchar(50)

				SELECT @branchstatus = branch_statuses.[branch_status]
				FROM branch_statuses 
				WHERE branch_statuses.branch_status_id = @branch_status_id

				SELECT @audit_description = 'Create: ID ' + CAST(@new_branch_id AS varchar(max))	+ ', [' + CAST(@issuer_id as varchar(100)) + ';' + @issuer_code + '], [' +
											@branch_code + ';' + @branch_name + ', ' + @branchstatus + ']'

				EXEC usp_insert_audit @audit_user_id, 
									 0,--BranchAdmin
									 NULL,
									 @audit_workstation, 
									 @audit_description, 
									 @issuer_id, NULL, NULL, NULL

				SET @ResultCode = 0

				COMMIT TRANSACTION [INSERT_BRANCH_TRAN]			
		END TRY
	BEGIN CATCH		
		ROLLBACK TRANSACTION [INSERT_BRANCH_TRAN]
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
END
