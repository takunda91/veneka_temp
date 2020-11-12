

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getFont_by_fontid]
@fontid int 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

		Select font_id,font_name,resource_path,DeletedYN,0 as TOTAL_ROWS, ROW_NUMBER() OVER(ORDER BY font_name ASC) AS ROW_NO,0 as TOTAL_PAGES from Issuer_product_font where font_id=@fontid

END