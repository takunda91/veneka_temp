insert into [reports] ([Reportid],[ReportName]) values(15,'PinMailerReport')



INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (70, 'Customer Account')

INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (71, 'Batch Reference')
INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (72, 'Date Created')
INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (73, 'Pin Mailer Report')




INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(70,0, 'Customer Account')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(70,1, 'Customer Account_fr')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(70,2, 'Customer Account_pt')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(70,3, 'Customer Account_es')


INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(71,0, 'Batch Reference')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(71,1, 'Batch Reference_fr')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(71,2, 'Batch Reference_pt')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(71,3, 'Batch Reference_es')


INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(72,0, 'Date Created')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(72,1, 'Date Created_fr')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(72,2, 'Date Created_pt')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(72,3, 'Date Created_es')

INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(73,0, 'Pin Mailer Report')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(73,1, 'Pin Mailer Report_fr')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(73,2, 'Pin Mailer Report_pt')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(73,3, 'Pin Mailer Report_es')

INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (15,73,1)
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (15,32,2) 
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (15,33,3) 
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (15,70,4) 
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (15, 71,5)
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (15, 72,6)
  INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (15,45,7) 
INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (15,43,8) 
INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (15,44, 9) 