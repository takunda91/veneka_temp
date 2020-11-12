
CREATE PROCEDURE [dbo].[usp_insert_font]
@font_name nvarchar(50),
@resource_path nvarchar(max),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
@ResultCode int =null OUTPUT ,
@new_font_id int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		BEGIN TRANSACTION [INSERT_FONT_TRAN]
		BEGIN TRY 
    -- Insert statements for procedure here
			DECLARE @dup_check int
			SELECT @dup_check = COUNT(*) 
			FROM [dbo].[Issuer_product_font]  
			WHERE  font_name= @font_name 
			IF @dup_check > 0
				BEGIN
					SELECT @ResultCode = 69							
				END
				
			ELSE
			BEGIN
			
			INSERT INTO  dbo.[Issuer_product_font] (font_name,resource_path) Values(@font_name,@resource_path)
		

			set @new_font_id=SCOPE_IDENTITY();

				--						DECLARE @audit_description nvarchar(500)
				--SELECT @audit_description = 'Font Inserted: ' + @font_name  
																	
				--EXEC usp_insert_audit @audit_user_id, 
				--					 0,
				--					 NULL, 
				--					 @audit_workstation, 
				--					 @audit_description, 
				--					 NULL, NULL, NULL, NULL

				SELECT @ResultCode = 0	
				COMMIT TRANSACTION [INSERT_FONT_TRAN]
				END
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_FONT_TRAN]
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