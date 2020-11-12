

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_file_load_list]
	@date_start DATETIMEoffset,
	@date_end DATETIMEoffset,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,	
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @StartRow INT, @EndRow INT;			
	
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	IF @date_end IS NOT NULL
		SET @date_end = DATEADD(DAY, 1, @date_end);

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY file_load_start ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
		  SELECT        file_load_id,CAST(SWITCHOFFSET([file_load].file_load_start,@UserTimezone)as datetime) as file_load_start , CAST(SWITCHOFFSET([file_load].file_load_end,@UserTimezone)as datetime) as file_load_end , user_id, files_to_process
FROM            file_load
				 WHERE SWITCHOFFSET([file_load].file_load_start,@UserTimezone)  BETWEEN @date_start AND @date_end
						
			)
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY file_load_start ASC
END