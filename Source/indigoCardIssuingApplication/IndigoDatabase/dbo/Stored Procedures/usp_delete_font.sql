

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_font]
@font_id int,
		@audit_user_id bigint,
	@audit_workstation varchar(100),
	@result_code int =null output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION [DELETE_FONT_TRAN]
	BEGIN TRY 
    -- Insert statements for procedure here
	update Issuer_product_font set [DeletedYN]=1 where font_id=@font_id

	DECLARE @audit_description varchar(500)
				--SELECT @audit_description = 'Font Deleted: ' + CAST(@font_id as nvarchar(100))
																	
				--EXEC usp_insert_audit @audit_user_id, 
				--					 0,
				--					 NULL, 
				--					 @audit_workstation, 
				--					 @audit_description, 
				--					 NULL, NULL, NULL, NULL
	 COMMIT TRANSACTION [DELETE_FONT_TRAN]

	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_FONT_TRAN]
		
	END CATCH 

END