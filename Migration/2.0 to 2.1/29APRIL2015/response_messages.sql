update response_messages set english_response='Duplicate Fee Scheme Name, please change name.',
french_response='Duplicate Fee Scheme Name, please change name._fr',
portuguese_response='Duplicate Fee Scheme Name, please change name._pt',
spanish_response='Duplicate Fee Scheme Name, please change name._es'
where system_response_code= 226 and system_area=0

update response_messages set english_response='Duplicate Fee Detail Name, please change name.',
french_response='Duplicate Fee Detail Name, please change name._fr',
portuguese_response='Duplicate Fee Detail Name, please change name._pt',
spanish_response='Duplicate Fee Detail Name, please change name._es'
where system_response_code= 227 and system_area=0


INSERT INTO response_messages
                         (system_response_code, system_area, english_response, french_response, portuguese_response, spanish_response)
VALUES        (604,0,'Duplicate Terminal name.','Duplicate Terminal name_fr.','Duplicate Terminal name_pt.','Duplicate Terminal name_sp.')

INSERT INTO response_messages
                         (system_response_code, system_area, english_response, french_response, portuguese_response, spanish_response)
VALUES        (605,0,'Duplicate Device Number.','Duplicate Device Number_fr.','Duplicate Device Number_pt.','Duplicate Device Number_sp.')

INSERT INTO response_messages
                         (system_response_code, system_area, english_response, french_response, portuguese_response, spanish_response)
VALUES        (606,0,'Duplicate Master Key.','Duplicate Master Key_fr.','Duplicate Master Key_pt.','Duplicate Master Key_sp.')

INSERT INTO response_messages
                         (system_response_code, system_area, english_response, french_response, portuguese_response, spanish_response)
VALUES        (607,0,'Duplicate Master Key Name.','Duplicate Master Key Name_fr.','Duplicate Master Key Name_pt.','Duplicate Master Key Name_sp.')