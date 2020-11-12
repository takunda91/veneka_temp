-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_product_print_fields]
	-- Add the parameters for the stored procedure here
	@product_id int,
	@product_fields_list as dbo.[product_print_fields] READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [UPDATE_PRODUCT_FIELDS_TRAN]
		BEGIN TRY 			

			IF(SELECT COUNT(*) FROM
						(SELECT field_name FROM @product_fields_list fdl WHERE product_id = @product_id GROUP BY fdl.field_name
						HAVING COUNT(*) > 1) AS tb1) > 0
				BEGIN
					SET @ResultCode = 227
				END
			ELSE
				BEGIN
					DECLARE @effective_from DATETIMEOFFSET = SYSDATETIMEOFFSET()
					
					MERGE product_fields AS [target]
						USING @product_fields_list AS [source]
						ON ([target].product_id = [source].product_id AND [target].product_field_id = [source].product_field_id) 
						WHEN NOT MATCHED BY TARGET 
							THEN 
								INSERT 
								   ([product_id]
								   ,[field_name]
								   ,[print_field_type_id]
								   ,[X]
								   ,[Y]
								   ,[width]
								   ,[height]
								   ,[font]
								   ,[font_size]
								   ,[mapped_name]
								   ,[editable]
								   ,[deleted]
								   ,[label]
								   ,[max_length]
								   ,[printable]
								   ,[printside])
							 VALUES
								   ([source].product_id
								   ,[source].field_name
								   ,[source].print_field_type_id
								   ,[source].X
								   ,[source].Y
								   ,[source].width
								   ,[source].height
								   ,[source].font
								   ,[source].font_size
								   ,[source].mapped_name
								   ,[source].editable
								   ,[source].deleted
								   ,[source].label
								   ,[source].max_length
								   ,[source].printable
								   ,[source].printside

								   )
						WHEN MATCHED 
							THEN UPDATE SET 
								 [product_id] = [source].product_id 
								  ,[field_name] = [source].field_name 
								  ,[print_field_type_id] = [source].print_field_type_id 
								  ,[X] = [source].X 
								  ,[Y] = [source].Y 
								  ,[width] = [source].width 
								  ,[height] = [source].height 
								  ,[font] = [source].font 
								  ,[font_size] = [source].font_size 
								  ,[mapped_name] = [source].mapped_name 
								  ,[editable] = [source].editable 
								  ,[deleted] = [source].deleted 
								  ,[label] = [source].label 
								  ,[max_length] = [source].max_length
								   ,[printable] = [source].printable 
								  ,[printside] = [source].printside
						OUTPUT $action, inserted.*, deleted.*;
																
					SET @ResultCode = 0


					DECLARE @product_field_id int

					DECLARE PrintFieldTypes_cursor CURSOR FOR 
							SELECT DISTINCT product_fields.product_field_id
							FROM @product_fields_list p
							INNER JOIN product_fields on p.[product_id]=product_fields.[product_id]
							WHERE p.[deleted]=1

					OPEN PrintFieldTypes_cursor

						FETCH NEXT FROM PrintFieldTypes_cursor 
						INTO @product_field_id

						WHILE @@FETCH_STATUS = 0
						BEGIN
							FETCH NEXT FROM PrintFieldTypes_cursor 
							INTO @product_field_id
							--- if the print field is not using in customer_fields consider as hard delte.
							if((SELECT count(*) from customer_fields 
								WHERE customer_fields.product_field_id= @product_field_id)<=0 and (SELECT count(*) from customer_image_fields 
								WHERE customer_image_fields.product_field_id= @product_field_id)<=0 )

									BEGIN

									Delete from product_fields where product_fields.product_field_id=@product_field_id

									END

							
							END 
						CLOSE PrintFieldTypes_cursor;
						DEALLOCATE PrintFieldTypes_cursor;

			END

			COMMIT TRANSACTION [UPDATE_PRODUCT_FIELDS_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_PRODUCT_FIELDS_TRAN]
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