USE [{DATABASE_NAME}]
GO
/****** Object:  UserDefinedFunction [dbo].[DistBatchInCorrectStatus]    Script Date: 2016-07-07 03:54:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[DistBatchInCorrectStatus] 
(
	-- Add the parameters for the function here
	@dist_batch_statuses_id int,
	@new_dispatch_dist_batch_statuses_id int,
	@dist_batch_id int
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @current_dist_batch_status_id int,
			@flow_from_dist_batch_status_id int,
			@dist_batch_status_flow_id int,
			--@dist_batch_type_id int,
			--@card_issue_method_id int,
			@Result bit

	SET @Result = 0

	--get the current status for the distribution batch
	SELECT @current_dist_batch_status_id = [dist_batch_status_current].dist_batch_statuses_id, 
			@dist_batch_status_flow_id = [product_flow].dist_batch_status_flow_id
			--, @dist_batch_type_id = dist_batch_type_id,
			--, @card_issue_method_id = card_issue_method_id
	FROM [dist_batch_status_current]
		INNER JOIN [dist_batch]
			ON [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
		INNER JOIN [dist_batch_cards]
			ON [dist_batch].dist_batch_id = [dist_batch_cards].dist_batch_id
		INNER JOIN cards  
			ON [dist_batch_cards].card_id = cards.card_id
		INNER JOIN [issuer_product]
			ON cards.product_id = [issuer_product].product_id
		INNER JOIN [dist_batch_statuses_flow] AS [product_flow]
			ON (([dist_batch].dist_batch_type_id = 0 AND 
					[product_flow].dist_batch_status_flow_id = [issuer_product].production_dist_batch_status_flow)
				OR ([dist_batch].dist_batch_type_id = 1 AND 
					[product_flow].dist_batch_status_flow_id = [issuer_product].distribution_dist_batch_status_flow))
				AND [product_flow].dist_batch_statuses_id = [dist_batch_status_current].dist_batch_statuses_id
	WHERE dist_batch_status_current.dist_batch_id = @dist_batch_id

	--If we arent moving to a new status make sure the batch is currently in the same status
	IF(@dist_batch_statuses_id = @new_dispatch_dist_batch_statuses_id)
	BEGIN
		IF(@new_dispatch_dist_batch_statuses_id = @current_dist_batch_status_id)
			SET @Result = 1
	END
	ELSE IF(@dist_batch_statuses_id = @current_dist_batch_status_id)
	BEGIN
		--Check which status the batch must be in to flow to the new status
		SELECT @flow_from_dist_batch_status_id = dist_batch_statuses_id
		FROM dist_batch_statuses_flow
		WHERE dist_batch_status_flow_id = @dist_batch_status_flow_id
		    AND flow_dist_batch_statuses_id = @new_dispatch_dist_batch_statuses_id
			AND dist_batch_statuses_id = @dist_batch_statuses_id
			--AND dist_batch_type_id = @dist_batch_type_id
			--AND card_issue_method_id = @card_issue_method_id


		IF(@flow_from_dist_batch_status_id = @current_dist_batch_status_id)
			SET @Result = 1
	END

	-- Return the result of the function
	RETURN @Result

END

GO
/****** Object:  UserDefinedFunction [dbo].[DistBatchInCorrectStatusReject]    Script Date: 2016-07-07 03:54:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[DistBatchInCorrectStatusReject] 
(
	-- Add the parameters for the function here
	@new_dist_batch_statuses_id int,
	@dist_batch_id int
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @current_dist_batch_status_id int,
			@flow_from_dist_batch_status_id int,
			@dist_batch_status_flow_id int,
			--@dist_batch_type_id int,
			--@card_issue_method_id int,
			@Result bit

	SET @Result = 0

	--get the current status for the distribution batch
	SELECT @current_dist_batch_status_id = [dist_batch_status_current].dist_batch_statuses_id,
	       @dist_batch_status_flow_id = dist_batch_status_flow_id
			--@dist_batch_type_id = dist_batch_type_id,
			--@card_issue_method_id = card_issue_method_id
	FROM [dist_batch_status_current]
		INNER JOIN [dist_batch]
			ON [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
		INNER JOIN [dist_batch_cards]
			ON [dist_batch].dist_batch_id = [dist_batch_cards].dist_batch_id
		INNER JOIN cards  
			ON [dist_batch_cards].card_id = cards.card_id
		INNER JOIN [issuer_product]
			ON cards.product_id = [issuer_product].product_id
		INNER JOIN [dist_batch_statuses_flow] AS [product_flow]
			ON (([dist_batch].dist_batch_type_id = 0 AND 
					[product_flow].dist_batch_status_flow_id = [issuer_product].production_dist_batch_status_flow)
				OR ([dist_batch].dist_batch_type_id = 1 AND 
					[product_flow].dist_batch_status_flow_id = [issuer_product].distribution_dist_batch_status_flow))
				AND [product_flow].dist_batch_statuses_id = [dist_batch_status_current].dist_batch_statuses_id
	WHERE dist_batch_status_current.dist_batch_id = @dist_batch_id

	--Check which status the batch must be in to flow to the new status
	SELECT @flow_from_dist_batch_status_id = dist_batch_statuses_id
	FROM dist_batch_statuses_flow
	WHERE dist_batch_status_flow_id = @dist_batch_status_flow_id
		AND reject_dist_batch_statuses_id = @new_dist_batch_statuses_id
		--AND dist_batch_type_id = @dist_batch_type_id
		--AND card_issue_method_id = @card_issue_method_id


	IF(@flow_from_dist_batch_status_id = @current_dist_batch_status_id)
		SET @Result = 1

	-- Return the result of the function
	RETURN @Result

END

GO
/****** Object:  UserDefinedFunction [dbo].[FileParameterValidation]    Script Date: 2016-07-07 03:54:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Check that a file parameter does not belong to more than one file interface.
-- =============================================
CREATE FUNCTION [dbo].[FileParameterValidation]
(
	@connection_parameter_id int,
	@interface_guid char(36),
	@interface_type_id int
)
RETURNS bit
AS
BEGIN
	--Return true for all other interface types except for file type.
	IF(@interface_type_id != 4)
		RETURN 1
	
	--Make sure that the parameter associated with the interface has not been used by another interface
	RETURN CAST(CASE WHEN EXISTS(SELECT * 
								FROM [product_interface] 
								WHERE connection_parameter_id = @connection_parameter_id 
								AND interface_guid != @interface_guid) 
		THEN 0 ELSE 1 END AS BIT)
END

GO
/****** Object:  UserDefinedFunction [dbo].[GenCardReferenceNo]    Script Date: 2016-07-07 03:54:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Create card reference number
-- =============================================
CREATE FUNCTION [dbo].[GenCardReferenceNo] 
(
	-- Add the parameters for the function here
	@status_date DATETIME,
	@card_id BIGINT
)
RETURNS VARCHAR(MAX)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result VARCHAR(MAX),
			@product_code varchar(max),
			@issuer_code varchar(max)

	SELECT @issuer_code = [issuer].issuer_code,
			@product_code = [issuer_product].product_code
	FROM [issuer]
		INNER JOIN [branch]
			ON [issuer].issuer_id = [branch].issuer_id
		INNER JOIN [cards]
			ON [branch].branch_id = [cards].branch_id
		INNER JOIN [issuer_product]
			ON [issuer_product].product_id = [cards].product_id
	WHERE [cards].card_id = @card_id


	-- Add the T-SQL statements to compute the return value here
	--SELECT @Result = 'CR' + CONVERT(VARCHAR(8), @status_date, 12) + 
	--						RIGHT('000'+convert(varchar(3), @product_id), 3) +							
	--						CAST(@card_id AS varchar(max))

	SELECT @Result = 'CR' + @issuer_code + @product_code +						
							CAST(@card_id AS varchar(max))


	-- Return the result of the function
	RETURN @Result

END

GO
/****** Object:  UserDefinedFunction [dbo].[MAC]    Script Date: 2016-07-07 03:54:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[MAC]( @Message nvarchar(4000), @Table_id int )
RETURNS varbinary(24)
--WITH EXECUTE AS 'dbo'
AS
BEGIN
        declare @RetVal varbinary(24)
        declare @Key   varbinary(100)
        SET @RetVal = null
        SET @key    = null
        SELECT @Key = DecryptByKeyAutoCert( cert_id('cert_ProtectIndexingKeys'), null, mac_key) FROM mac_index_keys WHERE table_id = @Table_id
        if( @Key is not null )
               SELECT @RetVal = HashBytes( N'SHA1', convert(varbinary(8000), @Message) + @Key )
        RETURN @RetVal
END





GO
/****** Object:  UserDefinedFunction [dbo].[MarkString]    Script Date: 2016-07-07 03:54:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Masks the input string, left pad are the amount of characters from the left
-- you dont want masked, right pad are the amount of character from the right you dont want masked.
-- =============================================
CREATE FUNCTION [dbo].[MarkString]
(
	@InputStr varchar(100),
	@LeftPad int,
	@rightPad int
)
RETURNS varchar(100)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result varchar(100)

	-- Add the T-SQL statements to compute the return value here
	SELECT @Result = LEFT(@InputStr, @LeftPad) +
					 '******' +
					 RIGHT(@InputStr, @rightPad)

	-- Return the result of the function
	RETURN @Result

END




GO
/****** Object:  UserDefinedFunction [dbo].[MaskReportPAN]    Script Date: 2016-07-07 03:54:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchey
-- Create date: 
-- Description:	Determins if the users should have PAN masked for screen or not
-- =============================================
CREATE FUNCTION [dbo].[MaskReportPAN] 
(
	-- Add the parameters for the function here
	@user_id bigint
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result bit

	--Check if the user belongs to any user groups that have mask screen pan set to true
	SELECT @Result =
		CASE WHEN EXISTS (
			SELECT [user_group].mask_report_pan
			FROM [user_group]
				INNER JOIN [users_to_users_groups]
					ON [user_group].user_group_id = [users_to_users_groups].user_group_id
			WHERE [users_to_users_groups].[user_id] = @user_id
				AND [user_group].mask_report_pan = 1				
		)THEN 1 
		ELSE 0 END

	-- Return the result of the function
	RETURN @Result

END

GO
/****** Object:  UserDefinedFunction [dbo].[MaskScreen]    Script Date: 2016-07-07 03:54:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchey
-- Create date: 
-- Description:	Determins if the users should have PAN masked for screen or not
-- =============================================
CREATE FUNCTION [dbo].[MaskScreen] 
(
	-- Add the parameters for the function here
	@user_id bigint
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result bit

	--Check if the user belongs to any user groups that have mask screen pan set to true
	SELECT @Result =
		CASE WHEN EXISTS (
			SELECT [user_group].mask_screen_pan
			FROM [user_group]
				INNER JOIN [users_to_users_groups]
					ON [user_group].user_group_id = [users_to_users_groups].user_group_id
			WHERE [users_to_users_groups].[user_id] = @user_id
				AND [user_group].mask_screen_pan = 1				
		)THEN 1 
		ELSE 0 END

	-- Return the result of the function
	RETURN @Result

END

GO
/****** Object:  UserDefinedFunction [dbo].[MaskScreenPAN]    Script Date: 2016-07-07 03:54:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchey
-- Create date: 
-- Description:	Determins if the users should have PAN masked for screen or not
-- =============================================
CREATE FUNCTION [dbo].[MaskScreenPAN] 
(
	-- Add the parameters for the function here
	@user_id bigint
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result bit

	--Check if the user belongs to any user groups that have mask screen pan set to true
	SELECT @Result =
		CASE WHEN EXISTS (
			SELECT [user_group].mask_screen_pan
			FROM [user_group]
				INNER JOIN [users_to_users_groups]
					ON [user_group].user_group_id = [users_to_users_groups].user_group_id
			WHERE [users_to_users_groups].[user_id] = @user_id
				AND [user_group].mask_screen_pan = 1				
		)THEN 1 
		ELSE 0 END

	-- Return the result of the function
	RETURN @Result

END

GO
/****** Object:  UserDefinedFunction [dbo].[MaskString]    Script Date: 2016-07-07 03:54:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Masks the input string, left pad are the amount of characters from the left
-- you dont want masked, right pad are the amount of character from the right you dont want masked.
-- =============================================
CREATE FUNCTION [dbo].[MaskString]
(
	@InputStr varchar(100),
	@LeftPad int,
	@rightPad int
)
RETURNS varchar(100)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result varchar(100)

	-- Add the T-SQL statements to compute the return value here
	SELECT @Result = LEFT(@InputStr, @LeftPad) +
					 '******' +
					 RIGHT(@InputStr, @rightPad)

	-- Return the result of the function
	RETURN @Result

END





GO
/****** Object:  UserDefinedFunction [dbo].[next_Monday]    Script Date: 2016-07-07 03:54:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [dbo].[next_Monday] ( @myDate DATETIME )  
returns datetime
as
BEGIN
 set @myDate = dateadd(mm, 6, @myDate)  
 while datepart(dw,@myDate) <> 2
  begin  
   set @myDate = dateadd(dd, 1, @myDate)  
  end  
   return @myDate
 end




GO
/****** Object:  UserDefinedFunction [dbo].[PinBatchInCorrectStatus]    Script Date: 2016-07-07 03:54:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[PinBatchInCorrectStatus] 
(
	@pin_batch_statuses_id int,
	@new_pin_batch_statuses_id int,
	@pin_batch_id int
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @current_pin_batch_status_id int,
			@flow_from_pin_batch_status_id int,
			@pin_batch_type_id int,
			@card_issue_method_id int,
			@Result bit

	SET @Result = 0

	--get the current status for the distribution batch
	SELECT @current_pin_batch_status_id = pin_batch_statuses_id,
			@pin_batch_type_id = pin_batch_type_id,
			@card_issue_method_id = card_issue_method_id
	FROM pin_batch_status_current
		INNER JOIN pin_batch
			ON pin_batch_status_current.pin_batch_id = pin_batch.pin_batch_id
	WHERE pin_batch_status_current.pin_batch_id = @pin_batch_id


	IF(@pin_batch_statuses_id = @new_pin_batch_statuses_id)
	BEGIN
		IF(@new_pin_batch_statuses_id = @current_pin_batch_status_id)
			SET @Result = 1
	END
	ELSE IF(@pin_batch_statuses_id = @current_pin_batch_status_id)
	BEGIN
		--Check which status the batch must be in to flow to the new status
		SELECT @flow_from_pin_batch_status_id = pin_batch_statuses_id
		FROM pin_batch_statuses_flow
		WHERE flow_pin_batch_statuses_id = @new_pin_batch_statuses_id
			AND pin_batch_statuses_id = @pin_batch_statuses_id
			AND pin_batch_type_id = @pin_batch_type_id
			AND card_issue_method_id = @card_issue_method_id


		IF(@flow_from_pin_batch_status_id = @current_pin_batch_status_id)
			SET @Result = 1
	END

	-- Return the result of the function
	RETURN @Result

END


GO
/****** Object:  UserDefinedFunction [dbo].[PinBatchInCorrectStatusReject]    Script Date: 2016-07-07 03:54:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[PinBatchInCorrectStatusReject] 
(
	-- Add the parameters for the function here
	@new_pin_batch_statuses_id int,
	@pin_batch_id int
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @current_pin_batch_status_id int,
			@flow_from_pin_batch_status_id int,
			@pin_batch_type_id int,
			@card_issue_method_id int,
			@Result bit

	SET @Result = 0

	--get the current status for the pin batch
	SELECT @current_pin_batch_status_id = pin_batch_statuses_id,
			@pin_batch_type_id = pin_batch_type_id,
			@card_issue_method_id = card_issue_method_id
	FROM pin_batch_status_current
		INNER JOIN pin_batch
			ON pin_batch_status_current.pin_batch_id = pin_batch.pin_batch_id
	WHERE pin_batch_status_current.pin_batch_id = @pin_batch_id

	--Check which status the batch must be in to flow to the new status
	SELECT @flow_from_pin_batch_status_id = pin_batch_statuses_id
	FROM pin_batch_statuses_flow
	WHERE reject_pin_batch_statuses_id = @new_pin_batch_statuses_id
		AND pin_batch_type_id = @pin_batch_type_id
		AND card_issue_method_id = @card_issue_method_id


	IF(@flow_from_pin_batch_status_id = @current_pin_batch_status_id)
		SET @Result = 1

	-- Return the result of the function
	RETURN @Result

END

GO
/****** Object:  UserDefinedFunction [dbo].[ProductValidation]    Script Date: 2016-07-07 03:54:10 PM ******/
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
ALTER TABLE [dbo].[issuer_product]  WITH CHECK ADD  CONSTRAINT [CK_BIN_SUBPRODUCT] CHECK  (([dbo].[ProductValidation]([product_id],[product_bin_code],[sub_product_code])=(1)))
GO
ALTER TABLE [dbo].[issuer_product] CHECK CONSTRAINT [CK_BIN_SUBPRODUCT]
GO
ALTER TABLE [dbo].[product_interface]  WITH CHECK ADD  CONSTRAINT [CK_FILE_PARAMETER] CHECK  (([dbo].[FileParameterValidation]([connection_parameter_id],[interface_guid],[interface_type_id])=(1)))
GO
ALTER TABLE [dbo].[product_interface] CHECK CONSTRAINT [CK_FILE_PARAMETER]
GO