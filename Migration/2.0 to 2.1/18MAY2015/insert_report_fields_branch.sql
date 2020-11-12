  INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (28, 'Branch Name: ')
  INSERT INTO [report_fields] (reportfieldid, reportfieldname) VALUES (29, 'Branch Code: ')

  INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES (28, 0, 'Branch Name: ')
  INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES (28, 1, 'Branch Name_fr: ')
  INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES (28, 2, 'Branch Name_pt: ')
  INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES (28, 3, 'Branch Name_sp: ')

  INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES (29, 0, 'Branch Code: ')
  INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES (29, 1, 'Branch Code_fr: ')
  INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES (29, 2, 'Branch Code_pt: ')
  INSERT INTO [reportfields_language] (reportfieldid, language_id, language_text) VALUES (29, 3, 'Branch Code_sp: ')

  INSERT INTO [report_reportfields] (reportid, reportfieldid) VALUES (1, 28) -- Load Batch Report 
  INSERT INTO [report_reportfields] (reportid, reportfieldid) VALUES (2, 28) -- Dist Batch Report
  INSERT INTO [report_reportfields] (reportid, reportfieldid) VALUES (5, 28) -- Pin Batch Report
  
  INSERT INTO [report_reportfields] (reportid, reportfieldid) VALUES (1, 29) -- Load Batch Report 
  INSERT INTO [report_reportfields] (reportid, reportfieldid) VALUES (2, 29) -- Dist Batch Report
  INSERT INTO [report_reportfields] (reportid, reportfieldid) VALUES (5, 29) -- Pin Batch Report