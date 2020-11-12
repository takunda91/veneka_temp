-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_fee_scheme] 
	-- Add the parameters for the stored procedure here
	@fee_scheme_id int,
	@issuer_id int,
	@fee_accounting_id int,
	@fee_scheme_name varchar(100),
	@fee_detail_list as dbo.fee_detail_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [UPDATE_PRODUCT_FEE_SCHEME_TRAN]
		BEGIN TRY 			

			IF (SELECT COUNT(*) FROM [product_fee_scheme] 
					WHERE fee_scheme_name = @fee_scheme_name AND issuer_id = @issuer_id AND fee_scheme_id != @fee_scheme_id) > 0
				BEGIN
					SET @ResultCode = 226						
				END		
			ELSE IF (SELECT COUNT(*) FROM
						(SELECT fee_detail_name FROM @fee_detail_list fdl GROUP BY fdl.fee_detail_name
						HAVING COUNT(*) > 1) AS tb1) > 0
				BEGIN
					SET @ResultCode = 227
				END
			ELSE
				BEGIN
					DECLARE @effective_from DATETIME = SYSDATETIMEOFFSET()


					UPDATE [product_fee_scheme]
					SET fee_scheme_name = @fee_scheme_name
						, fee_accounting_id = @fee_accounting_id
					WHERE fee_scheme_id = @fee_scheme_id	
			
					--DELETE DELATAILS AND LINKED CHARGES FOR THOSE NO LONGER IN THE LIST
					DELETE FROM [product_fee_charge]
					WHERE fee_detail_id IN (
						SELECT fee_detail_id
						FROM [product_fee_detail]
						WHERE fee_scheme_id = @fee_scheme_id 
							AND fee_detail_id NOT IN (SELECT dl.fee_detail_id FROM @fee_detail_list dl WHERE dl.fee_detail_id > 0)
					)

					DELETE FROM [product_fee_detail]
					WHERE fee_scheme_id = @fee_scheme_id
							AND fee_detail_id NOT IN (SELECT dl.fee_detail_id FROM @fee_detail_list dl WHERE dl.fee_detail_id > 0)

					--UPDATE THOSE WITH VALID ID"S
					UPDATE [product_fee_detail]
					SET fee_detail_name = dl.fee_detail_name, 
						fee_editable_YN = dl.fee_editable_TN, 
						fee_waiver_YN = dl.fee_waiver_YN
					FROM [product_fee_detail]
							INNER JOIN @fee_detail_list dl
								ON [product_fee_detail].fee_detail_id = dl.fee_detail_id
					WHERE [product_fee_detail].fee_scheme_id = @fee_scheme_id
							AND dl.fee_detail_id > 0

					--INSERT ANY NEW DETAILS
					INSERT INTO [product_fee_detail] (fee_scheme_id, fee_detail_name, fee_editable_YN, fee_waiver_YN, 
														effective_from, effective_to, deleted_yn)
					SELECT @fee_scheme_id, dl.fee_detail_name, dl.fee_editable_TN, dl.fee_waiver_YN, 
							@effective_from, null, 0
					FROM @fee_detail_list dl
					WHERE dl.fee_detail_id < 0

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
					

					SET @audit_description = 'Fee Scheme Update: ' + @fee_scheme_name
												+' , Fee Scheme Id :'+  CAST(@fee_scheme_id AS VARCHAR(max)) 
												+' , Fee Bands : ' + COALESCE(@fee_details, 'NONE')
										 	
					EXEC usp_insert_audit @audit_user_id, 
											4,
											NULL, 
											@audit_workstation, 
											@audit_description, 
											@issuer_id, NULL, NULL, NULL
										
					SET @ResultCode = 0
			END

			COMMIT TRANSACTION [UPDATE_PRODUCT_FEE_SCHEME_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_PRODUCT_FEE_SCHEME_TRAN]
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



