

/****** Object:  StoredProcedure [dbo].[sp_update_product_print_fields]    Script Date: 2/29/2016 11:17:52 AM ******/
DROP PROCEDURE [dbo].[sp_update_product_print_fields]
GO

/****** Object:  StoredProcedure [dbo].[sp_update_product_print_fields]    Script Date: 2/29/2016 11:17:52 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_update_product_print_fields]
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
					DECLARE @effective_from DATETIME = GETDATE()
					
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
								   ,[max_length])
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
								   ,[source].max_length)
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



GO


/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_print_field_types]    Script Date: 2/29/2016 11:17:44 AM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_print_field_types]
GO

/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_print_field_types]    Script Date: 2/29/2016 11:17:44 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_print_field_types]
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT print_field_type_id AS lookup_id, language_text
	FROM print_field_types_language
	WHERE language_id = @language_id

END

GO




/****** Object:  StoredProcedure [dbo].[sp_get_product_print_fields_value]    Script Date: 2/29/2016 11:17:34 AM ******/
DROP PROCEDURE [dbo].[sp_get_product_print_fields_value]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_product_print_fields_value]    Script Date: 2/29/2016 11:17:34 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		LTladi 
-- Create date: 20151116
-- Description:	Get product print fields
-- =============================================
--exec sp_get_product_print_fields_value '87309'

CREATE PROCEDURE [dbo].[sp_get_product_print_fields_value]
	@card_id bigint
AS
BEGIN
	SET NOCOUNT ON;
	
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT
		product_fields.product_field_id
		, product_fields.product_id
		, product_fields.field_name
		, product_fields.print_field_type_id
		, product_fields.X
		, product_fields.Y
		, product_fields.width
		, product_fields.height
		, product_fields.font
		, product_fields.font_size
		, product_fields.mapped_name
		, product_fields.editable
		, product_fields.deleted		
		, product_fields.label
		, product_fields.max_length
		, CAST(DECRYPTBYKEY(customer_fields.value) as image) value,
		print_field_types.print_field_name
	FROM
		product_fields
		 INNER JOIN print_field_types ON product_fields.print_field_type_id = print_field_types.print_field_type_id
		inner JOIN customer_fields ON customer_fields.product_field_id = product_fields.product_field_id
		inner JOIN customer_account ON customer_account.customer_account_id = customer_fields.customer_account_id
		--LEFT JOIN cards ON cards.card_id = customer_account.card_id
	WHERE customer_account.card_id = @card_id
	UNION ALL
	SELECT
		product_fields.product_field_id
		, product_fields.product_id
		, product_fields.field_name
		, product_fields.print_field_type_id
		, product_fields.X
		, product_fields.Y
		, product_fields.width
		, product_fields.height
		, product_fields.font
		, product_fields.font_size
		, product_fields.mapped_name
		, product_fields.editable
		, product_fields.deleted		
		, product_fields.label
		, product_fields.max_length
		, customer_image_fields.value,
		print_field_types.print_field_name
	FROM
		product_fields
		 INNER JOIN print_field_types ON product_fields.print_field_type_id = print_field_types.print_field_type_id

		inner JOIN customer_image_fields ON customer_image_fields.product_field_id = product_fields.product_field_id
		inner JOIN customer_account ON customer_account.customer_account_id = customer_image_fields.customer_account_id
		--LEFT JOIN cards ON cards.card_id = customer_account.card_id
	WHERE customer_account.card_id = @card_id
	
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END

GO


/****** Object:  StoredProcedure [dbo].[sp_get_product_print_fields_by_code]    Script Date: 2/29/2016 11:17:25 AM ******/
DROP PROCEDURE [dbo].[sp_get_product_print_fields_by_code]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_product_print_fields_by_code]    Script Date: 2/29/2016 11:17:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		LTladi 
-- Create date: 20151116
-- Description:	Get product print fields by product code
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_product_print_fields_by_code]
	@product_code varchar(50),
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		product_fields.product_field_id
		, product_fields.product_id
		, product_fields.field_name
		, product_fields.print_field_type_id
		, product_fields.X
		, product_fields.Y
		, product_fields.width
		, product_fields.height
		, product_fields.font
		, product_fields.font_size
		, product_fields.mapped_name
		, product_fields.editable
		, product_fields.deleted
		, product_fields.label
		, product_fields.max_length
		, cast(null as varbinary) as value,
		print_field_types.print_field_name

	FROM
		product_fields
		 INNER JOIN print_field_types ON product_fields.print_field_type_id = print_field_types.print_field_type_id
	
		 INNER JOIN issuer_product 	ON product_fields.product_id = issuer_product.product_id
	WHERE
		issuer_product.product_code = @product_code
		AND product_fields.deleted = 0

END

GO




/****** Object:  StoredProcedure [dbo].[sp_get_product_print_fields]    Script Date: 2/29/2016 11:17:16 AM ******/
DROP PROCEDURE [dbo].[sp_get_product_print_fields]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_product_print_fields]    Script Date: 2/29/2016 11:17:16 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		LTladi 
-- Create date: 20151116
-- Description:	Get product print fields
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_product_print_fields]
	@product_id bigint 
AS
BEGIN
	SET NOCOUNT ON;
	
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT
		product_fields.product_field_id
		, product_fields.product_id
		, product_fields.field_name
		, product_fields.print_field_type_id
		, product_fields.X
		, product_fields.Y
		, product_fields.width
		, product_fields.height
		, product_fields.font
		, product_fields.font_size
		, product_fields.mapped_name
		, product_fields.editable
		, product_fields.deleted
		, product_fields.label
		, product_fields.max_length
		, cast(null as varbinary) as value,
		print_field_types.print_field_name
	FROM
		product_fields
		 INNER JOIN
                         print_field_types ON product_fields.print_field_type_id = print_field_types.print_field_type_id
	WHERE
		product_fields.product_id = @product_id
	
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END

GO





/****** Object:  StoredProcedure [dbo].[sp_get_card_object]    Script Date: 3/1/2016 11:28:55 AM ******/
DROP PROCEDURE [dbo].[sp_get_card_object]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_card_object]    Script Date: 3/1/2016 11:28:55 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch card object
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_card_object] 
	@card_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)) AS 'card_number'
		, cards.card_request_reference AS card_reference_number
		, [cards].branch_id
		, [cards].card_id
		, [cards].card_issue_method_id
		, [cards].card_priority_id
		, [cards].card_request_reference
		, [cards].card_sequence
		, [cards].product_id
		, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_activation_date)), 109) as 'card_activation_date'
		, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_expiry_date)), 109) as 'card_expiry_date'
		, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_production_date)), 109) as 'card_production_date'						
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].pvv)) as 'pvv'
		--, [dist_batch].dist_batch_reference
		--, [dist_batch_cards].dist_batch_id
		--, [dist_batch_cards].dist_card_status_id
		--, [dist_card_statuses].dist_card_status_name
		, 0 as dist_batch_id
		, 0 as dist_card_status_id
		, [customer_account].account_type_id
		, [customer_account].cms_id
		, [customer_account].contract_number
		, [customer_account].currency_id
		, [customer_account].customer_account_id
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_account_number)) as 'customer_account_number'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].name_on_card)) as 'name_on_card'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].Id_number)) as 'Id_number'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].contact_number)) as 'contact_number'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].CustomerId)) as 'CustomerId'
		, [customer_account].customer_title_id
		, [customer_account].customer_type_id
		, [customer_account].date_issued
		, [customer_account].resident_id
		, [customer_account].[user_id]
		, [issuer].issuer_id
		, [issuer].issuer_code
		, [issuer].issuer_name
		, [branch].branch_code
		, [branch].branch_name
		, [issuer_product].[product_code]
		, [issuer_product].[sub_product_code]
		, [issuer_product].[product_name]
		, [issuer_product].[product_bin_code]
		, [issuer_product].[src1_id]
		, [issuer_product].[src2_id]
		, [issuer_product].[src3_id]
		, CONVERT(INT, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVKI]))) as 'PVKI'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVK])) as 'PVK'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKA])) as 'CVKA'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKB])) as 'CVKB'
		, [issuer_product].[expiry_months]

	FROM 
		[cards]							
		INNER JOIN [branch]
			ON [cards].branch_id = [branch].branch_id
		INNER JOIN [issuer]
			ON [branch].issuer_id = [issuer].issuer_id
		INNER JOIN [issuer_product]
			ON [cards].product_id = [issuer_product].product_id
		LEFT OUTER JOIN [customer_account]
			ON [cards].card_id = [customer_account].card_id
	WHERE 
		[cards].card_id = @card_id
					
														   
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

END


GO





