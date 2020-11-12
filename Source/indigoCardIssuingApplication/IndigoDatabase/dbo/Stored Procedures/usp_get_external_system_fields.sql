-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_external_system_fields]
	@external_system_field_id int,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @StartRow INT, @EndRow INT;	
	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;


	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY external_system_field_id ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM( 

		SELECT      external_system_field_id, external_system_fields.external_system_id, field_name,system_name 
		FROM            external_system_fields		
		INNER JOIN 	external_systems ON external_systems.external_system_id=external_system_fields.external_system_id
		WHERE external_system_field_id= COALESCE(@external_system_field_id, external_system_field_id) 

	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY external_system_field_id ASC
														   
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;		

END