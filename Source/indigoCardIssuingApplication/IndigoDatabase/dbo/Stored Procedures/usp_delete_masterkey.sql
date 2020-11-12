-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_masterkey] 
	@masterkeyid int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@resultcode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
		
		BEGIN TRY 
		
		IF((select count(*) from terminals where terminal_masterkey_id=@masterkeyid)> 0)
		BEGIN
			set @resultcode=608
		END
		ELSE 
		BEGIN
		BEGIN TRANSACTION [DELETE_MASTERKEY_TRAN]

		DECLARE @masterkey_name varchar(max) = '', @issuer_id int
		SELECT @masterkey_name = [masterkey_name], @issuer_id = issuer_id
				FROM masterkeys
				WHERE  masterkey_id=@masterkeyid

		delete dbo.masterkeys where masterkey_id=@masterkeyid
			set @resultcode=0
		
		DECLARE @audit_description varchar(max)
		
		SET @audit_description = 'MasterKey Delete: '+ COALESCE(@masterkey_name, 'UNKNOWN') 
									+ 'MasterKey Id: ' + CAST(@masterkeyid AS varchar(max))
										 	
		EXEC usp_insert_audit @audit_user_id, 
								9,
								NULL, 
								@audit_workstation, 
								@audit_description, 
								@issuer_id, NULL, NULL, NULL

		COMMIT TRANSACTION [DELETE_MASTERKEY_TRAN]
		END
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_MASTERKEY_TRAN]
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