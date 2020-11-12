insert into [reports] ([Reportid],[ReportName]) values(11,'Inventory Summary Report')


INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (55, 'Inventory Summary Report')


INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(55,0, 'Inventory Summary Report ')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(55,1, 'Inventory Summary Report_fr ')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(55,2, 'Inventory Summary Report_pt ')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(55,3, 'Inventory Summary Report_es ')

 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (11,55,1) 
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (11,31,2) 
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (11,32,3) 
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (11, 33,4)
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (11, 35,5) 
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (11, 29,6)    
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (11, 45,7) 
	INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (11,43, 8) 
	INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (11,44, 9) 