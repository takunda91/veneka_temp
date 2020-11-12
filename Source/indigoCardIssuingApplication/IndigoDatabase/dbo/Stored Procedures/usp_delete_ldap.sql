
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_ldap]
	@ldap_setting_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   BEGIN TRANSACTION [DELETE_LDAP_TRAN]
		BEGIN TRY 
			Delete from  [dbo].[ldap_setting]
				WHERE ldap_setting_id = @ldap_setting_id

		--DECLARE @audit_description varchar(500)
		--		SELECT @audit_description = 'Deleted ldap: ' + CONVERT(NVARCHAR, @ldap_setting_id)	+ ')'			
		--		EXEC usp_insert_audit @audit_user_id, 
		--							 2,
		--							 NULL, 
		--							 @audit_workstation, 
		--							 @audit_description, 
		--							 NULL, NULL, NULL, NULL	


				COMMIT TRANSACTION [DELETE_LDAP_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_LDAP_TRAN]
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