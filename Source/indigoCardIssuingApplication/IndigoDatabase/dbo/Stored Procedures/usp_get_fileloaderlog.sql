-- =============================================
-- Author:		sandhya konduru
-- Create date: 19 may 2014
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_fileloaderlog]
	@file_load_id int = null,
	@file_status_id int = NULL,
	@name_of_file varchar(60) = NULL,
	@issuer_id int = NULL,
	@date_from  datetimeoffset = null,	
	@date_to datetimeoffset = null,
	@languageId int =null,
	@PageIndex int = 1,
	@RowsPerPage int = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @StartRow INT, @EndRow INT;			
	if(@issuer_id=0)
	set @issuer_id=null

	if(@name_of_file <>'' or @name_of_file <> null)
	set @name_of_file= '%'+@name_of_file+'%'
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY load_date DESC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
		SELECT ISNULL(fh.issuer_id, 0) AS issuer_id, fh.[file_id], fsl.language_text as file_status , fh.number_successful_records, fh.number_failed_records, 
               fh.file_load_comments,fh.name_of_file,CAST(SWITCHOFFSET( fh.[file_created_date],@UserTimezone) AS datetime) as file_created_date,fh.file_size,CAST(SWITCHOFFSET( fh.load_date,@UserTimezone) AS datetime) as load_date,fh.file_directory,ft.file_type
		FROM file_history fh 
				INNER JOIN file_statuses fs 
					ON fh.file_status_id = fs.file_status_id
				INNER JOIN file_types ft 
					ON fh.file_type_id = ft.file_type_id
					inner join file_statuses_language fsl on fsl.file_status_id=fs.file_status_id
		WHERE fh.file_load_id = COALESCE(@file_load_id, fh.file_load_id)
			  AND fh.file_status_id = COALESCE(@file_status_id, fh.file_status_id)
			  AND ((@date_from IS NULL) OR (SWITCHOFFSET(fh.load_date,@UserTimezone)  BETWEEN @date_from AND DATEADD(day, 1, @date_to)))
			  AND ((@name_of_file IS NULL) OR (fh.name_of_file LIKE @name_of_file))
			  AND ((@issuer_id IS NULL) OR (fh.issuer_id = @issuer_id))
			  AND fsl.language_id=@languageId	 
		)
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY load_date DESC	

END