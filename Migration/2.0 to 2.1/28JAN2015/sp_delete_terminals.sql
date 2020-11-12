-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE sp_delete_terminal
	@terminal_id int
	, @audit_user_id bigint
	, @audit_workstation varchar(100)
	, @ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
				DELETE FROM [dbo].[terminals]
				  WHERE [terminal_id] = @terminal_id
				  --log the audit record


				DECLARE @audit_description varchar(max) = ''

				SET @audit_description = 'Delete terminal: id: ' + CAST(@terminal_id AS VARCHAR(max))

				EXEC sp_insert_audit @audit_user_id, 
									 0,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @terminal_id, NULL, NULL, NULL

									 SET @ResultCode = 0		
					
			END

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
GO
