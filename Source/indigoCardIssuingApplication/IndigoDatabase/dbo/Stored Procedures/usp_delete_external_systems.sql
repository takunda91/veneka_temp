-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_external_systems]
	@external_system_id INT,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
BEGIN TRANSACTION [DELETE_EXTERNAL_SYSTEMS]
		BEGIN TRY 

			--IF((SELECT COUNT(*) FROM external_system_fields WHERE external_system_id = @external_system_id) > 0)
			--	BEGIN
			--		SET @ResultCode = 803
			--	END
			--ELSE
				BEGIN
					DELETE FROM external_systems
					WHERE external_system_id=@external_system_id
					
					DELETE FROM external_system_fields
					WHERE external_system_id=@external_system_id

					SET @ResultCode = 0
				END


			COMMIT TRANSACTION [DELETE_EXTERNAL_SYSTEMS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_EXTERNAL_SYSTEMS]
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