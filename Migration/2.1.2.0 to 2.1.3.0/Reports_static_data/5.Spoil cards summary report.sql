insert into [reports] ([Reportid],[ReportName]) values(10,'Spoil cards summary report')


INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (54, 'Spoil cards summary report')


INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(54,0, 'Spoil cards summary report ')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(54,1, 'Spoil cards summary reportt_fr ')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(54,2, 'Spoil cards summary reportt_pt ')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(54,3, 'Spoil cards summary report_es ')

 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (10,54,1) 
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (10,31,2) 
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (10,32,3) 
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (10, 33,4)
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (10, 35,5) 
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (10, 29,6)    
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (10, 45,7) 
	INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (10,43, 8) 
	INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (10,44, 9) 