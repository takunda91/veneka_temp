INSERT INTO report_fields
                         (reportfieldid, reportfieldname)
VALUES        (24,'Card Reference Number')

INSERT INTO report_reportfields
                         (reportid, reportfieldid)
VALUES        (1,24)
INSERT INTO report_reportfields
                         (reportid, reportfieldid)
VALUES        (2,24)
INSERT INTO report_reportfields
                         (reportid, reportfieldid)
VALUES        (3,24)

INSERT INTO reportfields_language
                         (reportfieldid, language_id, language_text)
VALUES        (24,0,'Card Reference Number')
INSERT INTO reportfields_language
                         (reportfieldid, language_id, language_text)
VALUES        (24,1,'Card Reference Number_fr')
INSERT INTO reportfields_language
                         (reportfieldid, language_id, language_text)
VALUES        (24,2,'Card Reference Number_pt')
INSERT INTO reportfields_language
                         (reportfieldid, language_id, language_text)
VALUES        (24,3,'Card Reference Number_sp')