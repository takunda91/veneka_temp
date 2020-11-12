UPDATE card_issue_method SET card_issue_method_name = 'CENTRALISED' WHERE card_issue_method_id = 0
UPDATE card_issue_method_language SET language_text = 'CENTRALISED' WHERE language_id = 0 AND card_issue_method_id = 0
UPDATE card_issue_method_language SET language_text = 'CENTRALISED_fr' WHERE language_id = 1 AND card_issue_method_id = 0
UPDATE card_issue_method_language SET language_text = 'CENTRALISED_pt' WHERE language_id = 2 AND card_issue_method_id = 0
UPDATE card_issue_method_language SET language_text = 'CENTRALISED_sp' WHERE language_id = 3 AND card_issue_method_id = 0