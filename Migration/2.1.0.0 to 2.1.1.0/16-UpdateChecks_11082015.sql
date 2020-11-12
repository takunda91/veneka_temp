USE [indigo_database_main_dev]
GO

DROP FUNCTION [CheckCardsForProduct]
GO
DROP FUNCTION [CheckCardSubProductId]
GO

/****** Object:  UserDefinedFunction [dbo].[ProductValidation]    Script Date: 2015-08-11 03:41:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Validates that BIN and Subproduct code are valid. True= Validation Passed, False= Validation Failed
-- =============================================
CREATE FUNCTION [dbo].[ProductValidation] 
(
	-- Add the parameters for the function here
	@product_id int = null,
	@product_bin char(6),
	@sub_product_code varchar(3)
)
RETURNS BIT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result BIT = 0

	IF(@product_bin IS NULL OR LEN(@product_bin) != 6)
		RETURN 0

	IF(@sub_product_code IS NULL)
		SET @sub_product_code = ''

	--Validation ONE: Product ID supplied and bin and/or sub product code changed, check if cards linked
	IF (@product_id IS NOT NULL)
		IF EXISTS(SELECT * FROM [issuer_product] WHERE product_id = @product_id AND product_bin_code = @product_bin
				AND ISNULL(sub_product_code, '') = @sub_product_code) --Product & Subproduct have not changed, dont go further.
				RETURN 1
		ELSE --Details have changed, check if any cards linked to the product.
			SET @Result = CAST(CASE WHEN EXISTS(SELECT * FROM [cards] WHERE product_id = @product_id) THEN 0 ELSE 1 END AS BIT)
	ELSE
		SET @Result = 1

	IF(@Result = 1)
	BEGIN
		--VALIDATION TWO: Is there already a product that doesn't have sub-product code, no new products with sub_products can be added.
		IF EXISTS(SELECT * FROM [issuer_product] WHERE product_bin_code = @product_bin AND sub_product_code IS NULL
					AND @sub_product_code IS NOT NULL)
			RETURN 0
		--Validation Three: make sure that the BIN + product or BIN plus part of the sun product has been used already
		ELSE IF EXISTS(SELECT * FROM [issuer_product] WHERE 
				((product_bin_code = @product_bin AND ISNULL(sub_product_code, '') = @sub_product_code)
				OR (product_bin_code = @product_bin AND ISNULL(sub_product_code, '') LIKE @sub_product_code+'%')
				OR (product_bin_code = @product_bin AND @sub_product_code LIKE ISNULL(sub_product_code, '')+'%'))
				AND ((@product_id IS NULL) OR (product_id != @product_id))
		)
			RETURN 0
	END

	-- Return the result of the function
	RETURN @Result
END
GO

ALTER TABLE [issuer_product]
    WITH CHECK ADD CONSTRAINT CK_BIN_SUBPRODUCT
    CHECK (dbo.ProductValidation(product_id, product_bin_code, sub_product_code) = 1)
GO

ALTER TABLE [issuer_product]
	WITH CHECK ADD CONSTRAINT CK_BIN_LENGTH
	CHECK ((product_bin_code IS NOT NULL) AND (LEN(product_bin_code) = 6))
GO

--SELECT product_id, product_bin_code, LEN(product_bin_code)
--FROM [issuer_product]

--SELECT product_id, product_name, product_bin_code, sub_product_id, DeletedYN, dbo.ProductValidation(product_id, product_bin_code, sub_product_code) 
--FROM [issuer_product]
--ORDER BY product_bin_code
--WHERE dbo.ProductValidation(product_id, product_bin_code, sub_product_code) = 0