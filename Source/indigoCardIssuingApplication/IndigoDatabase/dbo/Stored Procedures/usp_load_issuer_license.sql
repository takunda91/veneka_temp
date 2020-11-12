-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Loads license information for issuer
-- =============================================
CREATE PROCEDURE [dbo].[usp_load_issuer_license] 
	@issuer_name varchar(50),
	@issuer_code varchar(10),
	@license_key varchar(1000),
	@xml_license_file varbinary,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    BEGIN TRANSACTION [LOAD_ISSUER_LICENSE_TRAN]
		BEGIN TRY 

		   DECLARE @issuer_id int

		   SELECT @issuer_id = issuer_id
		   FROM [issuer]
		   WHERE issuer_name = @issuer_name AND
				 issuer_code = @issuer_code
			
			IF(@issuer_id > 0)
				BEGIN
					UPDATE [issuer]
					SET license_key = @license_key,
						license_file = ''
					WHERE issuer_name = @issuer_name AND
							issuer_code = @issuer_code

					--log the audit record
					--DECLARE @audit_description varchar(500)
					--SELECT @audit_description = 'Load issuer license information (issuer_id = ' + CONVERT(NVARCHAR, @issuer_id)	+ ')'			
					--EXEC usp_insert_audit @audit_user_id, 
					--					 2,
					--					 NULL, 
					--					 @audit_workstation, 
					--					 @audit_description, 
					--					 NULL, NULL, NULL, NULL

					SET @ResultCode = 0
				END
			ELSE
				BEGIN
					SET @ResultCode = 71
				END

			COMMIT TRANSACTION [LOAD_ISSUER_LICENSE_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [LOAD_ISSUER_LICENSE_TRAN]
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