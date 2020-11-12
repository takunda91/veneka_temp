insert into [reports] ([Reportid],[ReportName]) values(9,'Issued cards summary report')


INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (53, 'Issued cards summary report')

INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(53,0, 'Issued cards summary report ')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(53,1, 'Issued cards summary reportt_fr ')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(53,2, 'Issued cards summary reportt_pt ')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(53,3, 'Issued cards summary report_es ')

 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (9,53,1) 
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (9,31,2) 
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (9,32,3) 
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (9, 33,4)
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (9, 35,5) 
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (9, 29,7)    
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (9, 45,8) 
	INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (9,43, 9) 
	INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (9,44, 10) 