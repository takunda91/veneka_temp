USE [indigo_database_group]
GO
/****** Object:  UserDefinedFunction [dbo].[MAC]    Script Date: 2014/07/31 05:03:27 PM ******/
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
/****** Object:  UserDefinedFunction [dbo].[MarkString]    Script Date: 2014/07/31 05:03:27 PM ******/
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
/****** Object:  UserDefinedFunction [dbo].[MaskString]    Script Date: 2014/07/31 05:03:27 PM ******/
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
/****** Object:  UserDefinedFunction [dbo].[next_Monday]    Script Date: 2014/07/31 05:03:27 PM ******/
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
