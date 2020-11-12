

/****** Object:  StoredProcedure [dbo].[sp_get_reportfields_Labels]    Script Date: 2/8/2016 10:58:34 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

alter PROCEDURE [dbo].[sp_get_reportfields_Labels]
	@languageId int=null,
	@ReportId int=null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
select Reportid,col1,col2,col3,col4,col5,col6,col7,col8,col9,col10,col11,col12,col13,col14,col15,col16,col17,col18,col19,col20,col21
from
(
SELECT       reports.Reportid, reportfields_language.language_text ,
'col'+CAST( Row_Number() over (partition by  reports.Reportid order by  report_reportfields.[reportfieldorderno]) as varchar(10)) as columnsequence
FROM            reports INNER JOIN
                         report_reportfields ON reports.Reportid = report_reportfields.reportid INNER JOIN
                         report_fields ON report_reportfields.reportfieldid = report_fields.reportfieldid INNER JOIN
                         reportfields_language ON report_reportfields.reportfieldid = reportfields_language.reportfieldid

						 where reportfields_language.[language_id]=@languageId and  reports.Reportid=@ReportId
) 	 Temp
				
				
PIVOT
(
		max(language_text) 
		for columnsequence in (col1,col2,col3,col4,col5,col6,col7,col8,col9,col10,col11,col12,col13,col14,col15,col16,col17,col18,col19,col20,col21)

)PIV
END

GO


