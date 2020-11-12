USE [indigo_database_main_dev]
GO

INSERT INTO report_fields (reportfieldid, reportfieldname)
	VALUES(14, 'Notes')

GO

INSERT INTO reportfields_language ([reportfieldid], [language_id], [language_text])
	VALUES(14, 0, 'Notes')

INSERT INTO reportfields_language ([reportfieldid], [language_id], [language_text])
	VALUES(14, 1, 'Notes_fr')

INSERT INTO reportfields_language ([reportfieldid], [language_id], [language_text])
	VALUES(14, 2, 'Notes_pt')

INSERT INTO reportfields_language ([reportfieldid], [language_id], [language_text])
	VALUES(14, 3, 'Notes_es')

GO

INSERT INTO report_reportfields (reportid, reportfieldid)
	VALUES (2, 14)
GO