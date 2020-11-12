USE [indigo_database_2.1.4.0]
GO

/****** Object:  StoredProcedure [dbo].[usp_funds_load_load]    Script Date: 2019/11/06 16:08:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Andrew Kudumba
-- Create date: 
-- Description:	load entry for funds_load
-- =============================================
CREATE PROCEDURE [dbo].[usp_funds_load_load] 
	-- Add the parameters for the stored procedure here
	@funds_load_id bigint,
	@loader_id	int,
	@load_date	datetime,
	@status int, 
	@ResultCode	int output
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_FUNDS_LOAD]
		BEGIN TRY 
		
			update funds_load
				set load_date=@load_date,
				loader_id=@loader_id,
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


