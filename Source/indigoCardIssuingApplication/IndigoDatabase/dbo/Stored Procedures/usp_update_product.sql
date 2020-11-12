-- =============================================
-- Author:		Sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE  PROCEDURE [dbo].[usp_update_product]
	-- Add the parameters for the stored procedure here
	@product_id int,
	@product_name varchar(100),
	@product_code varchar(50),
	@product_bin_code varchar(9),
	@issuer_id  int,
	@pan_length smallint,
	@sub_product_code varchar(4),
	@expiry_months int,
	@fee_scheme_id int = null,
	@charge_fee_to_issuing_branch_YN bit,
	@charge_fee_at_cardrequest bit,
	@card_issue_method_id int,
	@print_issue_card_YN bit,

	@name_on_card_top decimal(8,2) =0 ,
	@name_on_card_left decimal(8,2)=0,
	@Name_on_card_font_size int=0,
	@font_id int=1,

	@src1_id int,
	@src2_id int,
	@src3_id int,
	@PVKI varchar(100),
	@PVK varchar(100),
	@CVKA varchar(100),
	@CVKB varchar(100),
	
	@enable_instant_pin_YN bit,
	@enable_instant_pin_reissue_YN bit,
	@pin_calc_method_id int,
	@min_pin_length int,
	@max_pin_length int,	

	@cms_exportable_YN bit,
	@product_load_type_id int,
	@auto_approve_batch_YN bit,
	@account_validation_YN bit,

	@pin_mailer_printing_YN bit,
	@pin_mailer_reprint_YN bit,
	@e_pin_request_YN bit,

	@card_issue_reasons_list as dbo.key_value_array READONLY,
	@account_types_list as dbo.key_value_array READONLY,
	@currencylist AS dbo.product_currency_array READONLY,
	@external_system_fields as dbo.[product_external_fields_array] READONLY,			
	@prod_interface_parameters_list AS dbo.bikey_value_array READONLY,
	@issue_interface_parameters_list AS dbo.bikey_value_array READONLY,
	@accounttype_interface_parameters_list AS dbo.product_account_types READONLY,

	@decimalisation_table varchar(100),
	@pin_validation_data varchar(100),
	@pin_block_formatid int,
	

	@production_dist_batch_status_flow_id int, 
	@distribution_dist_batch_status_flow_id int,

	@allow_manual_uploaded_YN bit,
	@allow_reupload_YN bit,
	@remote_cms_update_YN bit,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int =null OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	BEGIN TRANSACTION [UPDATE_Product_TRAN]
		BEGIN TRY 			

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate
			--Check for duplicate's
			IF EXISTS(SELECT * FROM [issuer_product] WHERE ([product_code] = @product_code AND [product_id] != @product_id AND issuer_id = @issuer_id))
				BEGIN
					SET @ResultCode = 221						
				END
			IF EXISTS(SELECT * FROM [issuer_product] WHERE ([product_name] = @product_name AND [product_id] != @product_id))
				BEGIN
					SET @ResultCode = 220
				END
			ELSE IF dbo.ProductValidation(@product_id, @product_bin_code, @sub_product_code) = 0
				BEGIN
					SET @ResultCode = 222
				END
			ELSE IF 0 = ANY(SELECT dbo.FileParameterValidation(key2, value, key1) FROM @prod_interface_parameters_list)
				BEGIN
				
					SET @ResultCode = 228
				END
			ELSE			
			BEGIN

				UPDATE [dbo].[issuer_product]
					SET [product_code] = @product_code,
						[product_name] = @product_name,
						[product_bin_code] = @product_bin_code,
						[issuer_id] = @issuer_id,
						[font_id] = @font_id,
						[name_on_card_top] = @name_on_card_top,
						[name_on_card_left] = @name_on_card_left,
						[Name_on_card_font_size] = @Name_on_card_font_size,
						[src1_id] = @src1_id,
						[src2_id] = @src2_id,
						[src3_id] = @src3_id,
						[PVKI] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@PVKI)),
						[PVK] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@PVK)),
						[CVKA] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@CVKA)),
						[CVKB] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@CVKB)),
						[expiry_months] = @expiry_months,						
						[fee_scheme_id] = @fee_scheme_id,
						[charge_fee_to_issuing_branch_YN] = @charge_fee_to_issuing_branch_YN,
						[charge_fee_at_cardrequest]=@charge_fee_at_cardrequest,
						[enable_instant_pin_YN] = @enable_instant_pin_YN,
						[min_pin_length] = @min_pin_length,
						[max_pin_length] = @max_pin_length,
						[enable_instant_pin_reissue_YN] = @enable_instant_pin_reissue_YN,
						[cms_exportable_YN] = @cms_exportable_YN,
						[product_load_type_id] = @product_load_type_id,
						[pan_length] = @pan_length,
						[sub_product_code] = @sub_product_code,
						[pin_calc_method_id] = @pin_calc_method_id,
						[auto_approve_batch_YN] = @auto_approve_batch_YN,
						[account_validation_YN] = @account_validation_YN,
						[pin_mailer_printing_YN] = @pin_mailer_printing_YN,
						[pin_mailer_reprint_YN] = @pin_mailer_reprint_YN,
						[card_issue_method_id] = @card_issue_method_id,
						[decimalisation_table]=ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@decimalisation_table)), 
						[pin_validation_data]=ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@pin_validation_data)),
						[pin_block_formatid]=@pin_block_formatid,
						[production_dist_batch_status_flow] = @production_dist_batch_status_flow_id, 
						[distribution_dist_batch_status_flow] = @distribution_dist_batch_status_flow_id,
						[print_issue_card_YN] = @print_issue_card_YN
						, [allow_manual_uploaded_YN] = @allow_manual_uploaded_YN
					    , [allow_reupload_YN] = @allow_reupload_YN
						, [remote_cms_update_YN] = @remote_cms_update_YN
						,[e_pin_request_YN]=@e_pin_request_YN
					WHERE [product_id] = @product_id



				--insert issuing reason
				DELETE FROM [product_issue_reason] WHERE product_id = @product_id

				INSERT INTO [product_issue_reason] (product_id, card_issue_reason_id)
				SELECT @product_id, value
				FROM @card_issue_reasons_list	

				--insert account types
				DELETE FROM [products_account_types] WHERE product_id = @product_id

				INSERT INTO [products_account_types] (product_id, account_type_id)
				SELECT @product_id, value
				FROM @account_types_list

				--Insert interfaces
				DELETE FROM [product_interface]
				WHERE [product_id] = @product_id

				INSERT INTO [product_interface] (product_id, interface_type_id, connection_parameter_id, interface_guid, interface_area)
				SELECT @product_id, key1, key2, value, 0
				FROM @prod_interface_parameters_list

				INSERT INTO [product_interface] (product_id, interface_type_id, connection_parameter_id, interface_guid, interface_area)
				SELECT @product_id, key1, key2, value, 1
				FROM @issue_interface_parameters_list

				Delete from product_account_types_mapping
				WHERE [product_id] = @product_id


			INSERT INTO product_account_types_mapping(product_id, cbs_account_type,indigo_account_type, cms_account_type)
				SELECT @product_id, cbs_account_type, indigo_account_type,cms_account_type
				FROM @accounttype_interface_parameters_list 

				--Update the products currency
				DELETE FROM product_currency
				WHERE [product_id] = @product_id

				Declare  @RC as int
				EXECUTE @RC = [usp_insert_product_currency] @product_id, @currencylist, @audit_user_id, @audit_workstation


					--Update the products currency
				DELETE FROM product_external_system
				WHERE [product_id] = @product_id

				DECLARE @RCExternal int
				EXECUTE @RCExternal = [usp_insert_product_external_systems] @product_id, @external_system_fields, @audit_user_id, @audit_workstation

										
				
				DECLARE @audit_description varchar(500)
				SELECT @audit_description = 'Product updated: ' + @product_code  + ', Product Name: ' + @product_name + 
																     ', bin code: ' + @product_bin_code 
																	 + ', Product Id: ' + CAST(@product_id as varchar(max))
																	
				EXEC usp_insert_audit @audit_user_id, 
									 4,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @Issuer_id, NULL, NULL, NULL

				SELECT @ResultCode = 0				
			END
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
			COMMIT TRANSACTION [UPDATE_Product_TRAN]

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_Product_TRAN]
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
