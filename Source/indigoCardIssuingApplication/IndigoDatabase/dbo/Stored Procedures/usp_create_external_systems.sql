-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_external_systems]
@external_system_type_id int,
@system_name nvarchar(100),
@external_system_fields as dbo.[product_external_fields_array] READONLY,	
@audit_user_id bigint,
@audit_workstation varchar(100),
@external_system_id int output,
@ResultCode int output
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		BEGIN TRANSACTION [CREATE_EXTERNAL_SYSTEMS]
		BEGIN TRY 
			BEGIN
						 INSERT INTO external_systems
                         ( external_system_type_id, system_name)
VALUES        (@external_system_type_id,@system_name)

		SET @external_system_id=SCOPE_IDENTITY()

				DECLARE @RCExternal int
				EXECUTE @RCExternal = usp_modify_external_system_fields @external_system_id, @external_system_fields, @audit_user_id, @audit_workstation
				
				Declare @audit_msg nvarchar(500)
						 --Add audit for dist batch creation								
				SET @audit_msg = 'CREATE: ' + CAST(@external_system_id AS varchar(max))+
									'SYSTEM NAME:'+@system_name
									
					set @ResultCode=0			   
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										9,
										NULL, 
										@audit_workstation, 
										@audit_msg, 
										NULL, NULL, NULL, NULL
										
										
			COMMIT TRANSACTION [CREATE_EXTERNAL_SYSTEMS]	
			END
									
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_EXTERNAL_SYSTEMS]
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;
		SET @external_system_id=0
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