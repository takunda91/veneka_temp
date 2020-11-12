insert into [reports] ([Reportid],[ReportName]) values(18,'CardDispatchReport')
insert into [reports] ([Reportid],[ReportName]) values(19,'CardExpiryReport')
insert into [reports] ([Reportid],[ReportName]) values(20,'CardProductionReport')


INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (79, 'CardDispatchReport')
INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (80, 'Customer Full Name')
INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (81, 'Name On Card')
INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (82, 'Expiry')
INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (83, 'Card Expiry Report')
INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (84, 'Card Production Report')


INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(79,0, 'Card Dispatch Report')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(79,1, 'CardDispatchReport_fr')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(79,2, 'CardDispatchReport_pt')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(79,3, 'CardDispatchReport_es')


INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(80,0, 'Customer Full Name')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(80,1, 'Customer Full Name_fr')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(80,2, 'Customer Full Name_pt')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(80,3, 'Customer Full Name_es')


INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(81,0, 'Name On Card')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(81,1, 'Name On Card_fr')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(81,2, 'Name On Card_pt')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(81,3, 'Name On Card_es')


INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(82,0, 'Expiry')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(82,1, 'Expiry_fr')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(82,2, 'Expiry_pt')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(82,3, 'Expiry_es')



INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(83,0, 'Card Expiry Report')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(83,1, 'Card Expiry Report_fr')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(83,2, 'Card Expiry Report_pt')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(83,3, 'Card Expiry Report_es')

INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(84,0, 'Card Production Report')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(84,1, 'Card Production Report_fr')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(84,2, 'Card Production Report_pt')
INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES(84,3, 'Card Production Report_es')

 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (18,79,1) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (18,32,2)  
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (18,33,3) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (18,80,4) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (18,70,5) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (18,81,6) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (18,38,7) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (18,39,8) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (18,71,9) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (18,72,10) 
INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (18,45, 11) 
INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (18,43, 12) 
INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (18,44, 13) 



 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (19,83,1) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (19,32,2)  
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (19,33,3)
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (19,38,4) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (19,39,5) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (19,82,6)
INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (19,45, 7) 
INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (19,43, 8) 
INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (19,44, 9) 


 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (20,84,1) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (20,32,2)  
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (20,33,3) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (20,80,4) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (20,70,5) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (20,81,6) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (20,38,7)   
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (20,71,8)
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (20,39,9) 
 INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (20,72,10) 
INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (20,45, 11) 
INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (20,43, 12) 
INSERT INTO [report_reportfields] (reportid, reportfieldid,[reportfieldorderno]) VALUES (20,44, 13)



 