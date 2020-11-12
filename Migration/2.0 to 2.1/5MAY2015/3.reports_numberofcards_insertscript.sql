INSERT INTO report_fields
                         (reportfieldid, reportfieldname)
VALUES        (25,'Number Of Cards: ')

INSERT INTO report_fields
                         (reportfieldid, reportfieldname)
VALUES        (26,'Line Item')


INSERT INTO report_reportfields
                         (reportid, reportfieldid)
VALUES        (1,25)
INSERT INTO report_reportfields
                         (reportid, reportfieldid)
VALUES        (2,25)
INSERT INTO report_reportfields
                         (reportid, reportfieldid)
VALUES        (3,25)

INSERT INTO report_reportfields
                         (reportid, reportfieldid)
VALUES        (1,26)
INSERT INTO report_reportfields
                         (reportid, reportfieldid)
VALUES        (2,26)
INSERT INTO report_reportfields
                         (reportid, reportfieldid)
VALUES        (3,26)


INSERT INTO reportfields_language
                         (reportfieldid, language_id, language_text)
VALUES        (25,0,'Number Of Cards:')
INSERT INTO reportfields_language
                         (reportfieldid, language_id, language_text)
VALUES        (25,1,'Number Of Cards_fr: ')
INSERT INTO reportfields_language
                         (reportfieldid, language_id, language_text)
VALUES        (25,2,'Number Of Cards_pt: ')
INSERT INTO reportfields_language
                         (reportfieldid, language_id, language_text)
VALUES        (25,3,'Number Of Cards_sp:')

INSERT INTO reportfields_language
                         (reportfieldid, language_id, language_text)
VALUES        (26,0,'Line Item')
INSERT INTO reportfields_language
                         (reportfieldid, language_id, language_text)
VALUES        (26,1,'Line Item_fr')
INSERT INTO reportfields_language
                         (reportfieldid, language_id, language_text)
VALUES        (26,2,'Line Item_pt')
INSERT INTO reportfields_language
                         (reportfieldid, language_id, language_text)
VALUES        (26,3,'Line Item_sp')



