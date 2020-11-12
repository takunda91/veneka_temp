

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_font]
@font_id int,
@font_name nvarchar(50),
@resource_path nvarchar(max),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
@ResultCode int =null OUTPUT 

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		BEGIN TRANSACTION [UPDATE_FONT_TRAN]
		BEGIN TRY 
    -- Insert statements for procedure here
			DECLARE @dup_check int
			SELECT @dup_check = COUNT(*) 
			FROM [dbo].[Issuer_product_font]  
			WHERE  font_name= @font_name AND font_id!=@font_id
			IF @dup_check > 0
				BEGIN
					SELECT @ResultCode = 69							
				END
				
			ELSE
			BEGIN
			
			UPDATE Issuer_product_font set font_name=@font_name,resource_path=@resource_path
			where font_id=@font_id


				--DECLARE @audit_description nvarchar(500)
				--SELECT @audit_description = 'Font updated: ' + @font_name  
																	
				--EXEC usp_insert_audit @audit_user_id, 
				--					 0,
				--					 NULL, 
				--					 @audit_workstation, 
				--					 @audit_description, 
				--					 NULL, NULL, NULL, NULL

				SELECT @ResultCode = 0	
				COMMIT TRANSACTION [UPDATE_FONT_TRAN]
				END
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_FONT_TRAN]
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