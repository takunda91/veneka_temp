-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_external_system_fields]
@external_system_field_id int,
@external_system_id int ,
@field_name nvarchar(100),
@audit_user_id bigint,
@audit_workstation varchar(100),
@ResultCode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		BEGIN TRANSACTION [UPATE_EXTERNAL_SYSTEM_FIELDS]
		BEGIN TRY 
			BEGIN
						UPDATE  external_system_fields
						SET external_system_id = @external_system_id,
						 field_name=@field_name
						 WHERE external_system_field_id=@external_system_field_id
				set @ResultCode=0
				
				Declare @audit_msg nvarchar(500)
						 --Add audit for dist batch creation								
				SET @audit_msg = 'Update: ' + CAST(@external_system_field_id AS varchar(max))
									
								   
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										9,
										NULL, 
										@audit_workstation, 
										@audit_msg, 
										NULL, NULL, NULL, NULL
										
										
			COMMIT TRANSACTION [UPATE_EXTERNAL_SYSTEM_FIELDS]	
			END
									
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPATE_EXTERNAL_SYSTEM_FIELDS]
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