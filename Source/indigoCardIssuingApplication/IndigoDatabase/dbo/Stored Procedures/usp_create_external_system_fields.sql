-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_external_system_fields]
@external_system_id int,
@field_name nvarchar(100),
@audit_user_id bigint,
@audit_workstation varchar(100),
@external_system_field_id int output,
@ResultCode int output
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		BEGIN TRANSACTION [CREATE_EXTERNAL_SYSTEM_FIELD]
		BEGIN TRY 
			BEGIN
						INSERT INTO external_system_fields ( external_system_id, field_name)
						VALUES        (@external_system_id,@field_name)
		
				SET @external_system_field_id=SCOPE_IDENTITY()

				set @ResultCode=0
				
				Declare @audit_msg nvarchar(500)
						 --Add audit for dist batch creation								
				SET @audit_msg = 'CREATE: ' + CAST(@external_system_field_id AS varchar(max))+
									'FIELD NAME:'+@field_name
									
								   
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										9,
										NULL, 
										@audit_workstation, 
										@audit_msg, 
										NULL, NULL, NULL, NULL
										
										
			COMMIT TRANSACTION [CREATE_EXTERNAL_SYSTEM_FIELD]	
			END
									
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_EXTERNAL_SYSTEM_FIELD]
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;
		SET @external_system_field_id=0
		
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
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;



END