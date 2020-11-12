-- =============================================
-- Author:		Selebalo, Setenane
-- Create date: 2014/04/10
-- Description:	Updates a issuer' and returns its zero(0) if the update
--				was successful, else return the error message.				
-- =============================================

CREATE PROCEDURE [dbo].[usp_update_issuer]
	@issuer_id int,
	@issuer_status_id int,
	@country_id int,
	@issuer_name varchar(50),
	@issuer_code varchar(10),
	@instant_card_issue_YN bit,
	@maker_checker_YN bit,
	@back_office_pin_auth_YN bit,	
	@language_id int = NULL,
	@card_ref_preference bit,
	@classic_card_issue_YN bit,
	@enable_instant_pin_YN bit,
	@authorise_pin_issue_YN bit,
	@authorise_pin_reissue_YN bit,	
	@allow_branches_to_search_cards_YN bit,
	@prod_interface_parameters_list AS dbo.bikey_value_array READONLY,
	@issue_interface_parameters_list AS dbo.bikey_value_array READONLY,
	@remote_token uniqueidentifier,
	@remote_token_expiry DATETIMEOFFSET =null,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_ISSUER_TRAN]
		BEGIN TRY 

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [issuer] WHERE ([issuer].[issuer_name] = @issuer_name AND [issuer].issuer_id != @issuer_id)) > 0
				BEGIN					
					SET @ResultCode = 200						
				END
			ELSE IF (SELECT COUNT(*) FROM [issuer] WHERE ([issuer].[issuer_code] = @issuer_code AND [issuer].issuer_id != @issuer_id)) > 0
				BEGIN
					SET @ResultCode = 201
				END
			ELSE			
			BEGIN

				UPDATE [dbo].[issuer]
				SET [issuer_status_id] = @issuer_status_id,
					[country_id] = @country_id,
					[issuer_name] = @issuer_name,
					[issuer_code] = @issuer_code,					
					[instant_card_issue_YN] = @instant_card_issue_YN,					
					[maker_checker_YN] = @maker_checker_YN,					
					[language_id] = @language_id,
					[card_ref_preference] = @card_ref_preference,
					[classic_card_issue_YN] = @classic_card_issue_YN,
					[enable_instant_pin_YN] = @enable_instant_pin_YN,
					[authorise_pin_issue_YN] = @authorise_pin_issue_YN,
					[authorise_pin_reissue_YN] = @authorise_pin_reissue_YN ,					
					[back_office_pin_auth_YN] = @back_office_pin_auth_YN,
					[allow_branches_to_search_cards]=@allow_branches_to_search_cards_YN,
					[remote_token]=@remote_token ,
					[remote_token_expiry]=@remote_token_expiry
				WHERE [issuer_id] = @issuer_id

				DELETE FROM [issuer_interface]
				WHERE [issuer_id] = @issuer_id

				IF (@issuer_status_id = 0)
				BEGIN

					INSERT INTO [issuer_interface] (issuer_id, interface_type_id, connection_parameter_id, interface_guid, interface_area)
					SELECT @issuer_id, key1, key2, value, 0
					FROM @prod_interface_parameters_list

					INSERT INTO [issuer_interface] (issuer_id, interface_type_id, connection_parameter_id, interface_guid, interface_area)
					SELECT @issuer_id, key1, key2, value, 1
					FROM @issue_interface_parameters_list				
				END
				ELSE
				BEGIN
					-- Clean up the product tables related to this issuer.
					DELETE FROM product_interface 
					WHERE product_id IN (SELECT product_id FROM issuer_product WHERE issuer_id = @issuer_id)
					
					UPDATE issuer_product SET DeletedYN = 1 
					WHERE issuer_id = @issuer_id

				END

				--IF (@instant_card_issue_YN = 1)
				--BEGIN
				--	IF (@account_validation_YN = 1)
				--	BEGIN
				--		--INSERT ACCOUNT INTERFACE
				--		INSERT INTO [issuer_interface]
				--			([issuer_id], [interface_type_id], [connection_parameter_id])
				--		VALUES (@issuer_id, 0, @account_connection_id)
				--	END

				--	--INSERT CORE BANKING INTERFACE
				--	INSERT INTO [issuer_interface]
				--		([issuer_id], [interface_type_id], [connection_parameter_id])
				--	VALUES (@issuer_id, 1, @corebanking_connection_id)
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


				SET @audit_description = 'Update: id: ' + CAST(@issuer_id AS VARCHAR(max))	+ 
										 ', name: ' + COALESCE(@issuer_name, 'UNKNOWN') +
										 ', code: ' + COALESCE(@issuer_code, 'UNKNOWN') +
										 ', country: ' + COALESCE(@country_code, 'UNKNOWN') + ';' + COALESCE(@country_name, 'UNKNOWN') +
										 ', status: ' + COALESCE(@issuer_status, 'UNKNOWN')
										 	
				EXEC usp_insert_audit @audit_user_id, 
									 4,---IssuerAdmin
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @issuer_id, NULL, NULL, NULL

				SELECT @ResultCode = 0				
			END

			COMMIT TRANSACTION [UPDATE_ISSUER_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_ISSUER_TRAN]
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







GO


