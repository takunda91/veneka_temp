--=============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_report_fields]
@reportid int=null,
@languageid int=null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
SELECT        report_fields.reportfieldid, reportfields_language.language_text
FROM            reports INNER JOIN
                         report_reportfields ON reports.Reportid = report_reportfields.reportid INNER JOIN
                         report_fields ON report_reportfields.reportfieldid = report_fields.reportfieldid INNER JOIN
                         reportfields_language ON report_reportfields.reportfieldid = reportfields_language.reportfieldid

						 where reportfields_language.[language_id]=@languageid and reports.[Reportid]=@reportid
  

END









GO


