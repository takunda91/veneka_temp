﻿-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_external_systems]
@external_system_id int ,
@external_system_type_id int,
@system_name nvarchar(100),
@external_system_fields as dbo.[product_external_fields_array] READONLY,	
@audit_user_id bigint,
@audit_workstation varchar(100),
@ResultCode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		BEGIN TRANSACTION [UPATE_EXTERNAL_SYSTEMS]
		BEGIN TRY 
			BEGIN
						UPDATE  external_systems
						SET external_system_type_id = @external_system_type_id,
						 system_name=@system_name
						 WHERE external_system_id=@external_system_id

						 	DECLARE @RCExternal int
				EXECUTE @RCExternal = usp_modify_external_system_fields @external_system_id, @external_system_fields, @audit_user_id, @audit_workstation

				set @ResultCode=0

				Declare @audit_msg nvarchar(500)
						 --Add audit for dist batch creation								
				SET @audit_msg = 'Update: ' + CAST(@external_system_id AS varchar(max))
									
								   
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										9,
										NULL, 
										@audit_workstation, 
										@audit_msg, 
										NULL, NULL, NULL, NULL
										
										
			COMMIT TRANSACTION [UPATE_EXTERNAL_SYSTEMS]	
			END
									
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPATE_EXTERNAL_SYSTEMS]
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