USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_issuer]    Script Date: 2015/04/23 05:50:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo, Setenane
-- Create date: 2014/03/20
-- Description:	Create a new issuer and returns its ID if the create
--				was successful, else return the error message.				
-- =============================================

ALTER PROCEDURE [dbo].[sp_create_issuer]
	@issuer_status_id int,
	@country_id int,
	@issuer_name varchar(50),
	@issuer_code varchar(10),
	@auto_create_dist_batch bit,
	@instant_card_issue_YN bit,
	@pin_mailer_printing_YN bit,
	@pin_mailer_reprint_YN bit,
	@delete_pin_file_YN bit,
	@delete_card_file_YN bit,
	@account_validation_YN bit,
	@maker_checker_YN bit,
	@enable_instant_pin_YN bit,
	@authorise_pin_issue_YN bit,
	@authorise_pin_reissue_YN bit,
	@enable_card_file_loader_YN bit,
	@license_file varchar(100) = NULL,
	@license_key varchar(1000) = NULL,
	@cards_file_location varchar(100) = NULL,
	@card_file_type varchar(20) = NULL,
	@pin_file_location varchar(100) = NULL,
	@pin_encrypted_ZPK varchar(40) = NULL,
	@pin_mailer_file_type varchar(20) = NULL,
	@pin_printer_name varchar(50) = NULL,
	@pin_encrypted_PWK varchar(40) = NULL,
	@language_id int = NULL,
	@card_ref_preference bit,
	@classic_card_issue_YN bit,
	@prod_interface_parameters_list AS dbo.bikey_value_array READONLY,
	@issue_interface_parameters_list AS dbo.bikey_value_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@new_issuer_id int OUTPUT,
	@ResultCode int OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [INSERT_ISSUER_TRAN]
		BEGIN TRY 

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [issuer] WHERE [issuer].[issuer_name] = @issuer_name) > 0
				BEGIN
					SET @new_issuer_id = 0
					SET @ResultCode = 200						
				END
			ELSE IF (SELECT COUNT(*) FROM [issuer] WHERE ([issuer].[issuer_code] = @issuer_code)) > 0
				BEGIN
					SET @new_issuer_id = 0
					SET @ResultCode = 201
				END
			ELSE			
			BEGIN

				INSERT INTO [dbo].[issuer]
					   ([issuer_status_id],[country_id],[issuer_name],[issuer_code],[auto_create_dist_batch]
					   ,[instant_card_issue_YN],[pin_mailer_printing_YN],[delete_pin_file_YN]
					   ,[delete_card_file_YN],[account_validation_YN],[maker_checker_YN],[license_file]
					   ,[license_key],[cards_file_location],[card_file_type],[pin_file_location]
					   ,[pin_encrypted_ZPK],[pin_mailer_file_type],[pin_printer_name],[pin_encrypted_PWK]
					   ,[language_id],[card_ref_preference],[classic_card_issue_YN],[enable_instant_pin_YN],
					   [authorise_pin_issue_YN],[authorise_pin_reissue_YN],[pin_mailer_reprint_YN], EnableCardFileLoader_YN)
				 VALUES
					   (@issuer_status_id, @country_id, @issuer_name, @issuer_code, @auto_create_dist_batch,
						@instant_card_issue_YN, @pin_mailer_printing_YN, @delete_pin_file_YN,
						@delete_card_file_YN, @account_validation_YN, @maker_checker_YN,
						@license_file, @license_key, @cards_file_location, @card_file_type,
						@pin_file_location, @pin_encrypted_ZPK,	@pin_mailer_file_type,
						@pin_printer_name, @pin_encrypted_PWK, @language_id,@card_ref_preference,
						@classic_card_issue_YN,@enable_instant_pin_YN, @authorise_pin_issue_YN,
						@authorise_pin_reissue_YN, @pin_mailer_reprint_YN, @enable_card_file_loader_YN)

				SET @new_issuer_id = SCOPE_IDENTITY();

				INSERT INTO [issuer_interface] (issuer_id, interface_type_id, connection_parameter_id, interface_guid, interface_area)
				SELECT @new_issuer_id, key1, key2, value, 0
				FROM @prod_interface_parameters_list

				INSERT INTO [issuer_interface] (issuer_id, interface_type_id, connection_parameter_id, interface_guid, interface_area)
				SELECT @new_issuer_id, key1, key2, value, 1
				FROM @issue_interface_parameters_list

				--IF (@instant_card_issue_YN = 1)
				--BEGIN

				--	IF (@account_validation_YN = 1)
				--	BEGIN
				--		--INSERT ACCOUNT INTERFACE
				--		INSERT INTO [issuer_interface]
				--			([issuer_id], [interface_type_id], [connection_parameter_id])
				--		VALUES (@new_issuer_id, 0, @account_connection_id)
				--	END

				--	--INSERT CORE BANKING INTERFACE
				--	INSERT INTO [issuer_interface]
				--		([issuer_id], [interface_type_id], [connection_parameter_id])
				--	VALUES (@new_issuer_id, 1, @corebanking_connection_id)
				--END

				--log the audit record
				DECLARE @audit_description varchar(max) = '',
						@country_name varchar(100),
						@country_code varchar(50),
						@issuer_status varchar(50)

				SELECT @country_name = country_name, @country_code = country_code
				FROM [country]
				WHERE country_id = @country_id

				SELECT @issuer_status = issuer_status_name
				FROM [issuer_statuses]
				WHERE issuer_status_id = @issuer_status_id


				SET @audit_description = 'Create: id: ' + CAST(@new_issuer_id AS VARCHAR(max))	+ 
										 ', name: ' + COALESCE(@issuer_name, 'UNKNOWN') +
										 ', code: ' + COALESCE(@issuer_code, 'UNKNOWN') +
										 ', country: ' + COALESCE(@country_code, 'UNKNOWN') + ';' + COALESCE(@country_name, 'UNKNOWN') +
										 ', status: ' + COALESCE(@issuer_status, 'UNKNOWN')
										 	
				EXEC sp_insert_audit @audit_user_id, 
									 4,---IssuerAdmin
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @new_issuer_id, NULL, NULL, NULL


				--INSERT DEFAULT USER GROUPS
				DECLARE @group_name varchar(50),
						@DataTable AS dbo.branch_id_array,
						@new_user_group_id int,
						@ResultCode2 int

				SELECT @group_name = @issuer_code + '_' + user_role
				FROM [user_roles]
				WHERE user_role_id = 4

				EXEC sp_create_user_group @group_name, 4, 
										  @new_issuer_id, 1, 1, 1, 1, 
										  @DataTable, -2, 'SYSTEM',
										  @new_user_group_id OUTPUT,
										  @ResultCode2 OUTPUT

				--Insert Default user groups
				--INSERT INTO [user_group]
				--	(all_branch_access, can_create, can_delete, can_read, can_update, issuer_id,
				--		user_group_name, user_role_id)
				--VALUES 
				--	(1, 1, 1, 1, 1, @new_issuer_id, @group_name, 4)

				SELECT @group_name = @issuer_code + '_' + user_role
				FROM [user_roles]
				WHERE user_role_id = 5

				EXEC sp_create_user_group @group_name, 5, 
									      @new_issuer_id, 1, 1, 1, 1, 
										  @DataTable, -2, 'SYSTEM',
										  @new_user_group_id OUTPUT,
										  @ResultCode2 OUTPUT
				--INSERT INTO [user_group]
				--	(all_branch_access, can_create, can_delete, can_read, can_update, issuer_id,
				--		user_group_name, user_role_id)
				--VALUES 
				--	(1, 1, 1, 1, 1, @new_issuer_id, @group_name, 5)	

				SET @ResultCode = 0		
					
			END

			COMMIT TRANSACTION [INSERT_ISSUER_TRAN]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_ISSUER_TRAN]
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







