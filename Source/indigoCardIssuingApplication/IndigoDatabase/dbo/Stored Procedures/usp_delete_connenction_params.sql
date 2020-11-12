
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_connenction_params]
	@connection_parameter_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   BEGIN TRANSACTION [DELETE_CONNECTIONPARAMS_TRAN]
		BEGIN TRY 
			--Log Audit
			DECLARE @audit_description varchar(max), @connection_name varchar(max)

			SELECT @connection_name = connection_name
			FROM connection_parameters
			WHERE connection_parameter_id = @connection_parameter_id 
			
			SELECT @audit_description = 'Parameter Deleted: ' + @connection_name
											+ ' , Paramter Id: ' + CAST(@connection_parameter_id AS varchar(max))
			EXEC usp_insert_audit @audit_user_id, 
						9,
						NULL, 
						@audit_workstation, 
						@audit_description, 
						NULL, NULL, NULL, NULL	

	Delete  from [connection_parameters_additionaldata] where connection_parameter_id= @connection_parameter_id 

			delete from [dbo].connection_parameters
				WHERE connection_parameter_id = @connection_parameter_id
		--DECLARE @audit_description varchar(500)
		--		SELECT @audit_description = 'Deleted Connection Parameters : ' + CONVERT(NVARCHAR, @connection_parameter_id)	+ ')'			
		--		EXEC usp_insert_audit @audit_user_id, 
		--							 2,
		--							 NULL, 
		--							 @audit_workstation, 
		--							 @audit_description, 
		--							 NULL, NULL, NULL, NULL	


				COMMIT TRANSACTION [DELETE_CONNECTIONPARAMS_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_CONNECTIONPARAMS_TRAN]
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