-- =============================================
-- Author:		LTladi
-- Create date: 20151116
-- Description:	Create product print fields
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_product_print_fields]
	@product_id int
	, @field_name varchar(100)
	, @print_field_type_id int
	, @X float
	, @Y float
	, @width float
	, @height float
	, @font varchar(50)
	, @font_size int
	, @mapped_name varbinary(50)
	, @editable bit
	, @deleted bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [dbo].[product_fields]
           (product_id
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
           ,[deleted])
     VALUES
           (@product_id 
			, @field_name 
			, @print_field_type_id 
			, @X 
			, @Y 
			, @width 
			, @height 
			, @font 
			, @font_size 
			, @mapped_name 
			, @editable 
			, @deleted )

END