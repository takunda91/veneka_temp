insert into [reports] ([Reportid],[ReportName]) values(21,'BranchOrderReport')

INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (85, 'BrachOrderReport')
INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (86, 'Mobile Number')


INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(85,0, 'Branch Order Report')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(85,1, 'Branch Order Report_fr')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(85,2, 'Branch Order Report_pt')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(85,3, 'Branch Order Report_es')


INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(86,0, 'Mobile Number')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(86,1, 'Mobile Number_fr')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(86,2, 'Mobile Number_pt')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(86,3, 'Mobile Number_es')



 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (21,85,1) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (21,32,2)  
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (21,33,3) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (21,80,4) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (21,70,5) 
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (21,86,6) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (21,81,7)
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (21,71,8) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (21,72,9) 
INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (21,45, 10) 
INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (21,43, 11) 
INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (21,44, 12) 
