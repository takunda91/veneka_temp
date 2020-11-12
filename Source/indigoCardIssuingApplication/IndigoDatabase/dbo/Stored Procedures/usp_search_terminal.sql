-- =============================================
-- Author:		LTladi
-- Create date: 20150210
-- Description:	Search for terminal by branch or masterkey
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_terminal]
	@issuer_id int,
	@branch_id INT,
	@terminal_name nvarchar(50),
	@device_id nvarchar(50),	
	@terminal_model nvarchar(50),
		@PageIndex INT = 1
	,@RowsPerPage INT = 20
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN
		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate; 
		DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	WITH PAGE_ROWS
	AS
	( 
		SELECT ROW_NUMBER() OVER(ORDER BY [terminal_id] ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
		FROM(
			SELECT  
					DISTINCT t.[terminal_id]
					, t.[terminal_name]
					, t.[terminal_model]
					, i.[issuer_name],b.branch_name,CONVERT(varchar,DECRYPTBYKEY(device_id)) as deviceid
	FROM
		[terminals] t
		inner join [branch] b on b.branch_id=t.branch_id
		inner join [issuer] i on i.issuer_id= b.issuer_id
		where 		
		 (ISNULL(t.terminal_name, '')=ISNULL(@terminal_name, ISNULL(t.terminal_name, '')) or  
		 t.terminal_name like '%'+@terminal_name+'%') 
			AND 
		  (ISNULL(t.[terminal_model], '')=ISNULL(@terminal_model, ISNULL(t.terminal_name, '')) or  
		 t.terminal_name like '%'+@terminal_model+'%') 
		 AND 		 
		 (ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(t.device_id)), '')=ISNULL(@device_id, ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(t.device_id)), '')) or  
		 CONVERT(VARCHAR(max),DECRYPTBYKEY(t.device_id)) like '%'+@device_id+'%') 
		 AND t.branch_id = COALESCE(@branch_id, t.branch_id) and i.issuer_id=COALESCE(@issuer_id,i.issuer_id))
		 AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY [terminal_Id] ASC
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

	END

END