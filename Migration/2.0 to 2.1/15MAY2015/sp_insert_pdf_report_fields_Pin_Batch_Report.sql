USE indigo_database
GO 

INSERT INTO reports (Reportid , ReportName) VALUES (5, 'Pin Batch Report') 

INSERT INTO report_fields (reportfieldid, reportfieldname) VALUES (27 , 'Pin Batch Report') 

INSERT INTO report_reportfields (reportfieldid, reportid) VALUES (0, 5)
INSERT INTO report_reportfields (reportfieldid, reportid) VALUES (1, 5)
INSERT INTO report_reportfields (reportfieldid, reportid) VALUES (2, 5)
INSERT INTO report_reportfields (reportfieldid, reportid) VALUES (3, 5)
INSERT INTO report_reportfields (reportfieldid, reportid) VALUES (4, 5)
INSERT INTO report_reportfields (reportfieldid, reportid) VALUES (5, 5)
INSERT INTO report_reportfields (reportfieldid, reportid) VALUES (6, 5)
INSERT INTO report_reportfields (reportfieldid, reportid) VALUES (7, 5)
INSERT INTO report_reportfields (reportfieldid, reportid) VALUES (24,5)
INSERT INTO report_reportfields (reportfieldid, reportid) VALUES (25,5)
INSERT INTO report_reportfields (reportfieldid, reportid) VALUES (26,5)
INSERT INTO report_reportfields (reportfieldid, reportid) VALUES (27,5)

INSERT INTO reportfields_language (reportfieldid, language_id, language_text) VALUES (27,0,'Pin Batch Report')
INSERT INTO reportfields_language (reportfieldid, language_id, language_text) VALUES (27,1,'Pin Batch Report_fr')
INSERT INTO reportfields_language (reportfieldid, language_id, language_text) VALUES (27,2,'Pin Batch Report_pt')
INSERT INTO reportfields_language (reportfieldid, language_id, language_text) VALUES (27,3,'Pin Batch Report_sp')