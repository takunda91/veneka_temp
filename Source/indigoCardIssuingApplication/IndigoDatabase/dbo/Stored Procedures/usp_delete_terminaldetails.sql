-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_terminaldetails]
	@terminalid int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@resultcode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   
	BEGIN TRANSACTION [DELETE_TERMINAL_TRAN]
		BEGIN TRY 

		DECLARE @terminal_name varchar(max) = '', @issuer_id int, @audit_branch_name varchar(max), 
						@audit_branch_code varchar(max)


		SELECT @terminal_name = [terminal_name], @issuer_id = issuer_id, @audit_branch_name = branch_name,
						@audit_branch_code = branch_code
		FROM terminals
				INNER JOIN [branch]
				ON [branch].branch_id = terminals.branch_id
		WHERE  terminal_id=@terminalid
		
		delete dbo.terminals where terminal_id=@terminalid
		set @resultcode=0

		DECLARE @audit_description varchar(max) 
		
		SET @audit_description = 'Terminal Deleted: '+ COALESCE(@terminal_name, 'UNKNOWN') 
										  + ' , Terminal Id: ' + CAST(@terminalid as varchar(max))
										  + ' , branch code: ' + @audit_branch_code
										  + ' , branch name: ' + @audit_branch_name		
										 	
			EXEC usp_insert_audit @audit_user_id, 
									9,
									NULL, 
									@audit_workstation, 
									@audit_description, 
									@issuer_id, NULL, NULL, NULL

		COMMIT TRANSACTION [DELETE_TERMINAL_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_TERMINAL_TRAN]
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