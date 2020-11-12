USE [indigo_database_2.1.4.0]
GO

/****** Object:  StoredProcedure [dbo].[usp_funds_load_approve]    Script Date: 2019/11/06 16:08:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Andrew Kudumba
-- Create date: 
-- Description:	approve entry for funds_load
-- =============================================
CREATE PROCEDURE [dbo].[usp_funds_load_approve] 
	-- Add the parameters for the stored procedure here
	@funds_load_id bigint,
	@approver_id	int,
	@approve_date	datetime,
	@approve_accepted bit, 
	@status int, 
	@ResultCode	int output
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_FUNDS_LOAD]
		BEGIN TRY 
		
			update funds_load
				set approve_accepted=@approve_accepted,
				approve_date=@approve_date,
				approver_id=@approver_id,
				[status]=@status
			where funds_load_id = @funds_load_id

			SET @ResultCode = 0			
	
			COMMIT TRANSACTION [UPDATE_FUNDS_LOAD]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_FUNDS_LOAD]
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


