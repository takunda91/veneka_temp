-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_external_system_fields]
	@external_system_field_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
BEGIN TRANSACTION [DELETE_EXTERNAL_SYSTEM_FIELDS]
		BEGIN TRY 

			
				BEGIN
					DELETE FROM external_system_fields
					WHERE external_system_field_id=@external_system_field_id
					
					SET @ResultCode = 0
				END


			COMMIT TRANSACTION [DELETE_EXTERNAL_SYSTEM_FIELDS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_EXTERNAL_SYSTEM_FIELDS]
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