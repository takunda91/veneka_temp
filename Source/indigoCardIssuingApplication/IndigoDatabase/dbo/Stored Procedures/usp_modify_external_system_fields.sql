-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_modify_external_system_fields]
	@external_system_id int,
	@external_system_fields as dbo.[product_external_fields_array] READONLY,	
	@audit_user_id BIGINT,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--BEGIN TRANSACTION [CRATE_EXTERNAL_FILEDS_TRANS]
		BEGIN TRY 
							DECLARE @external_system_field_id int,
							 @fieldname nvarchar(100)

							CREATE TABLE #Temp ( external_system_field_id  integer );
							 
				     DECLARE External_Systems_Field CURSOR FOR 
								SELECT esx.external_system_field_id,esx.field_name
							FROM @external_system_fields esx
						

							OPEN External_Systems_Field

							FETCH NEXT FROM External_Systems_Field 
							INTO @external_system_field_id,@fieldname

							WHILE @@FETCH_STATUS = 0
							BEGIN

							IF ((SELECT count(*) FROM external_system_fields WHERE external_system_field_id = @external_system_field_id)=0) 
								BEGIN 
									 INSERT INTO external_system_fields(external_system_id, field_name) 
									 values(@external_system_id,@fieldname)
									 
									 insert #Temp(external_system_field_id) values(SCOPE_IDENTITY())
									 

								END 
								ELSE 
								BEGIN 
								
									 UPDATE external_system_fields
									SET external_system_id=@external_system_id,
									  field_name=@fieldname
									  where external_system_field_id=@external_system_field_id
									
								END 				
													
							FETCH NEXT FROM External_Systems_Field 
							INTO @external_system_field_id,@fieldname
								END 
								
							
							CLOSE External_Systems_Field;
							DEALLOCATE External_Systems_Field;



							DELETE FROM external_system_fields 
							WHERE external_system_field_id NOT IN(SELECT external_system_field_id FROM @external_system_fields   )
											AND external_system_id=@external_system_id
											AND external_system_field_id NOT IN(select external_system_field_id from #Temp)

											
							--COMMIT TRANSACTION [CRATE_EXTERNAL_FILEDS_TRANS]
					

		END TRY
	BEGIN CATCH
		--ROLLBACK TRANSACTION [CRATE_EXTERNAL_FILEDS_TRANS]
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