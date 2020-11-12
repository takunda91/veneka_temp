-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_fee_scheme] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@fee_scheme_name varchar(100),
	@fee_accounting_id int,
	@fee_detail_list as dbo.fee_detail_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@new_fee_scheme_id int OUTPUT,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [CREATE_PRODUCT_FEE_SCHEME_TRAN]
		BEGIN TRY 			
			IF (SELECT COUNT(*) FROM [product_fee_scheme] 
					WHERE fee_scheme_name = @fee_scheme_name AND issuer_id = @issuer_id) > 0
				BEGIN
					SET @new_fee_scheme_id = 0
					SET @ResultCode = 226						
				END		
			ELSE
				BEGIN
					DECLARE @effective_from DATETIME = GETDATE()

					INSERT INTO product_fee_scheme (fee_scheme_name, issuer_id, fee_accounting_id, deleted_yn)
						VALUES (@fee_scheme_name, @issuer_id, @fee_accounting_id, 0)

					SET @new_fee_scheme_id = SCOPE_IDENTITY();

					INSERT INTO [product_fee_detail] (fee_scheme_id, fee_detail_name, fee_editable_YN, fee_waiver_YN, 
														effective_from, effective_to, deleted_yn)
					SELECT @new_fee_scheme_id, dl.fee_detail_name, dl.fee_editable_TN, dl.fee_waiver_YN, 
							@effective_from, null, 0
					FROM @fee_detail_list dl

					--log the audit record
			DECLARE @audit_description varchar(max), @fee_details varchar(max)

			SELECT  @fee_details = STUFF(
							(SELECT ' Fee Band Name: ' + dl.fee_detail_name + ', editable: ' +  CAST(dl.fee_editable_TN AS VARCHAR(MAX)) 
								+ ', waiver: ' +  CAST(dl.fee_waiver_YN AS VARCHAR(MAX)) + ';' 
								FROM @fee_detail_list dl
								FOR XML PATH(''))
							, 1
							, 1
							, '')
					

			SET @audit_description = 'Fee Scheme Create: ' + @fee_scheme_name	
										+ ' , Fee Scheme Id: ' + CAST(@new_fee_scheme_id  AS VARCHAR(max))
										+' , Fee Bands : ' + COALESCE(@fee_details, 'NONE')
										 	
			EXEC usp_insert_audit @audit_user_id, 
									4,
									NULL, 
									@audit_workstation, 
									@audit_description, 
									@issuer_id, NULL, NULL, NULL
					
					SET @ResultCode = 0
				END

				COMMIT TRANSACTION [CREATE_PRODUCT_FEE_SCHEME_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_PRODUCT_FEE_SCHEME_TRAN]
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