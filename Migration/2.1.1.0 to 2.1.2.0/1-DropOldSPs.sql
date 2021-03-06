USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_validate_cards_ordered]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_validate_cards_ordered]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_useradminsettings]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_useradminsettings]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_user_language]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_user_language]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_user_group]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_user_group]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_user]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_user]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_terminal]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_terminal]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_sub_product]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_sub_product]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_product_print_fields]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_product_print_fields]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_product]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_product]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_printfieldvalues]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_printfieldvalues]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_masterkey]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_masterkey]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_ldap]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_ldap]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_issuer_card_ref_pref]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_issuer_card_ref_pref]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_issuer]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_issuer]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_instant_authorisation_pin]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_instant_authorisation_pin]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_font]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_font]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_fee_scheme]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_fee_scheme]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_fee_charge]    Script Date: 2016-03-16 02:50:49 PM ******/
DROP PROCEDURE [dbo].[sp_update_fee_charge]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_customer_details]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_update_customer_details]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_cardsequencenumber]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_update_cardsequencenumber]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_card_numbers_bikey]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_update_card_numbers_bikey]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_card_numbers]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_update_card_numbers]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_card_fee_reversal_ref]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_update_card_fee_reversal_ref]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_card_fee_reference]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_update_card_fee_reference]
GO
/****** Object:  StoredProcedure [dbo].[sp_update_branch]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_update_branch]
GO
/****** Object:  StoredProcedure [dbo].[sp_sys_encrypt]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_sys_encrypt]
GO
/****** Object:  StoredProcedure [dbo].[sp_sys_decrypt]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_sys_decrypt]
GO
/****** Object:  StoredProcedure [dbo].[sp_search_user]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_search_user]
GO
/****** Object:  StoredProcedure [dbo].[sp_search_terminal]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_search_terminal]
GO
/****** Object:  StoredProcedure [dbo].[sp_search_pin_reissue]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_search_pin_reissue]
GO
/****** Object:  StoredProcedure [dbo].[sp_search_pin_mailer_reprint]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_search_pin_mailer_reprint]
GO
/****** Object:  StoredProcedure [dbo].[sp_search_issuer_by_id]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_search_issuer_by_id]
GO
/****** Object:  StoredProcedure [dbo].[sp_search_issuer]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_search_issuer]
GO
/****** Object:  StoredProcedure [dbo].[sp_search_export_batches_paged]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_search_export_batches_paged]
GO
/****** Object:  StoredProcedure [dbo].[sp_search_export_batches]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_search_export_batches]
GO
/****** Object:  StoredProcedure [dbo].[sp_search_cards]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_search_cards]
GO
/****** Object:  StoredProcedure [dbo].[sp_search_branch_cards]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_search_branch_cards]
GO
/****** Object:  StoredProcedure [dbo].[sp_search_branch]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_search_branch]
GO
/****** Object:  StoredProcedure [dbo].[sp_rswitch_update_card_numbers]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_rswitch_update_card_numbers]
GO
/****** Object:  StoredProcedure [dbo].[sp_rswitch_hsm_pin_printed]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_rswitch_hsm_pin_printed]
GO
/****** Object:  StoredProcedure [dbo].[sp_reset_user_password]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_reset_user_password]
GO
/****** Object:  StoredProcedure [dbo].[sp_request_pin_reissue]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_request_pin_reissue]
GO
/****** Object:  StoredProcedure [dbo].[sp_request_pin_mailer_reprints]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_request_pin_mailer_reprints]
GO
/****** Object:  StoredProcedure [dbo].[sp_request_create_dist_batch]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_request_create_dist_batch]
GO
/****** Object:  StoredProcedure [dbo].[sp_request_card_stock]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_request_card_stock]
GO
/****** Object:  StoredProcedure [dbo].[sp_request_card_for_customer]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_request_card_for_customer]
GO
/****** Object:  StoredProcedure [dbo].[sp_prod_to_pin]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_prod_to_pin]
GO
/****** Object:  StoredProcedure [dbo].[sp_prod_to_dist_batch]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_prod_to_dist_batch]
GO
/****** Object:  StoredProcedure [dbo].[sp_pin_reissue_reject]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_pin_reissue_reject]
GO
/****** Object:  StoredProcedure [dbo].[sp_pin_reissue_expired]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_pin_reissue_expired]
GO
/****** Object:  StoredProcedure [dbo].[sp_pin_reissue_complete]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_pin_reissue_complete]
GO
/****** Object:  StoredProcedure [dbo].[sp_pin_reissue_approve]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_pin_reissue_approve]
GO
/****** Object:  StoredProcedure [dbo].[sp_pin_prod_to_pin_batch]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_pin_prod_to_pin_batch]
GO
/****** Object:  StoredProcedure [dbo].[sp_pin_mailer_reprint_request]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_pin_mailer_reprint_request]
GO
/****** Object:  StoredProcedure [dbo].[sp_pin_mailer_reprint_report]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_pin_mailer_reprint_report]
GO
/****** Object:  StoredProcedure [dbo].[sp_pin_mailer_reprint_reject]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_pin_mailer_reprint_reject]
GO
/****** Object:  StoredProcedure [dbo].[sp_pin_mailer_reprint_approve]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_pin_mailer_reprint_approve]
GO
/****** Object:  StoredProcedure [dbo].[sp_pin_mailer_report]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_pin_mailer_report]
GO
/****** Object:  StoredProcedure [dbo].[sp_pin_batch_status_reject]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_pin_batch_status_reject]
GO
/****** Object:  StoredProcedure [dbo].[sp_pin_batch_status_change]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_pin_batch_status_change]
GO
/****** Object:  StoredProcedure [dbo].[sp_lookup_response_message]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lookup_response_message]
GO
/****** Object:  StoredProcedure [dbo].[sp_load_issuer_license]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_load_issuer_license]
GO
/****** Object:  StoredProcedure [dbo].[sp_load_batch_reject]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_load_batch_reject]
GO
/****** Object:  StoredProcedure [dbo].[sp_load_batch_approve]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_load_batch_approve]
GO
/****** Object:  StoredProcedure [dbo].[sp_list_branch_card_codes]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_list_branch_card_codes]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_user_status]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_user_status]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_user_roles]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_user_roles]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_product_load_type]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_product_load_type]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_print_field_types]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_print_field_types]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_pin_reissue_statuses]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_pin_reissue_statuses]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_pin_batch_statuses]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_pin_batch_statuses]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_load_card_statuses]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_load_card_statuses]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_load_batch_statuses]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_load_batch_statuses]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_issuer_statuses]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_issuer_statuses]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_interface_types]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_interface_types]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_file_statuses]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_file_statuses]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_file_encryption_type]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_file_encryption_type]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_export_batch_statuses]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_export_batch_statuses]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_dist_card_statuses]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_dist_card_statuses]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_dist_batch_statuses]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_dist_batch_statuses]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_customer_type]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_customer_type]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_customer_title]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_customer_title]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_customer_residency]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_customer_residency]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_customer_account_types]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_customer_account_types]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_connection_parameter_type]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_connection_parameter_type]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_card_issue_reasons]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_card_issue_reasons]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_card_issue_method]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_card_issue_method]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_branch_statuses]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_branch_statuses]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_branch_card_statuses]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_branch_card_statuses]
GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_audit_actions]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_lang_lookup_audit_actions]
GO
/****** Object:  StoredProcedure [dbo].[sp_issue_card_to_customer]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_issue_card_to_customer]
GO
/****** Object:  StoredProcedure [dbo].[sp_issue_card_spoil]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_issue_card_spoil]
GO
/****** Object:  StoredProcedure [dbo].[sp_issue_card_printed]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_issue_card_printed]
GO
/****** Object:  StoredProcedure [dbo].[sp_issue_card_print_error]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_issue_card_print_error]
GO
/****** Object:  StoredProcedure [dbo].[sp_issue_card_PIN_captured]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_issue_card_PIN_captured]
GO
/****** Object:  StoredProcedure [dbo].[sp_issue_card_complete]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_issue_card_complete]
GO
/****** Object:  StoredProcedure [dbo].[sp_issue_card_cms_success]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_issue_card_cms_success]
GO
/****** Object:  StoredProcedure [dbo].[sp_issue_card_cms_relink_fail]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_issue_card_cms_relink_fail]
GO
/****** Object:  StoredProcedure [dbo].[sp_issue_card_cms_fail]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_issue_card_cms_fail]
GO
/****** Object:  StoredProcedure [dbo].[sp_issue_card_cms_edit_fail]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_issue_card_cms_edit_fail]
GO
/****** Object:  StoredProcedure [dbo].[sp_issue_card_check_account_balance]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_issue_card_check_account_balance]
GO
/****** Object:  StoredProcedure [dbo].[sp_integration_get_response_values]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_integration_get_response_values]
GO
/****** Object:  StoredProcedure [dbo].[sp_integration_get_response_fields]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_integration_get_response_fields]
GO
/****** Object:  StoredProcedure [dbo].[sp_integration_get_default_values]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_integration_get_default_values]
GO
/****** Object:  StoredProcedure [dbo].[sp_integration_bellid_get_sequence]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_integration_bellid_get_sequence]
GO
/****** Object:  StoredProcedure [dbo].[sp_insert_user_group_branches]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_insert_user_group_branches]
GO
/****** Object:  StoredProcedure [dbo].[sp_insert_user]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_insert_user]
GO
/****** Object:  StoredProcedure [dbo].[sp_insert_product_currency]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_insert_product_currency]
GO
/****** Object:  StoredProcedure [dbo].[sp_insert_product]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_insert_product]
GO
/****** Object:  StoredProcedure [dbo].[sp_insert_pin_reissue]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_insert_pin_reissue]
GO
/****** Object:  StoredProcedure [dbo].[sp_insert_load_batch_status]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_insert_load_batch_status]
GO
/****** Object:  StoredProcedure [dbo].[sp_insert_load_batch]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_insert_load_batch]
GO
/****** Object:  StoredProcedure [dbo].[sp_insert_font]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_insert_font]
GO
/****** Object:  StoredProcedure [dbo].[sp_insert_FlexcubeAudit]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_insert_FlexcubeAudit]
GO
/****** Object:  StoredProcedure [dbo].[sp_insert_file_history]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_insert_file_history]
GO
/****** Object:  StoredProcedure [dbo].[sp_insert_branch]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_insert_branch]
GO
/****** Object:  StoredProcedure [dbo].[sp_insert_audit]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_insert_audit]
GO
/****** Object:  StoredProcedure [dbo].[sp_getproductcode]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_getproductcode]
GO
/****** Object:  StoredProcedure [dbo].[sp_getProductbyProductid]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_getProductbyProductid]
GO
/****** Object:  StoredProcedure [dbo].[sp_getFontsList]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_getFontsList]
GO
/****** Object:  StoredProcedure [dbo].[sp_getFont_by_fontid]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_getFont_by_fontid]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_zone_key]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_zone_key]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_users_by_branch_admin]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_users_by_branch_admin]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_users_by_branch]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_users_by_branch]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_userroles_enterprise]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_userroles_enterprise]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_usergroup_auditreport]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_usergroup_auditreport]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_usergroup]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_usergroup]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_userbyroles_auditreport]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_userbyroles_auditreport]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_useradminsettingslist]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_useradminsettingslist]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_user_roles_for_user]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_user_roles_for_user]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_user_groups_by_issuer]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_user_groups_by_issuer]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_user_groups_admin]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_user_groups_admin]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_user_for_login]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_user_for_login]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_user_by_username]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_user_by_username]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_user_by_user_id]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_user_by_user_id]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_user]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_user]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_unassigned_users]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_unassigned_users]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_tmk_by_terminal]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_tmk_by_terminal]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_tmk_by_issuer]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_tmk_by_issuer]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_terminals_list]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_terminals_list]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_terminal_parameters]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_terminal_parameters]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_terminal]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_terminal]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_status_flow_roles]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_status_flow_roles]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_Spoilcardsummaryreport]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_Spoilcardsummaryreport]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_spoilcardsreport]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_spoilcardsreport]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_sequencenumber]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_sequencenumber]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_reportfields_Labels]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_reportfields_Labels]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_report_fields]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_report_fields]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_products_by_productcode]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_products_by_productcode]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_products_by_bincode]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_products_by_bincode]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_productlist]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_productlist]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_service_request_code3]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_product_service_request_code3]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_service_request_code2]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_product_service_request_code2]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_service_request_code1]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_product_service_request_code1]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_print_fields_value]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_product_print_fields_value]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_print_fields_by_code]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_product_print_fields_by_code]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_print_fields]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_product_print_fields]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_issue_reasons]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_product_issue_reasons]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_interfaces]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_product_interfaces]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_fee_scheme_list]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_product_fee_scheme_list]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_fee_scheme]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_product_fee_scheme]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_fee_details]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_product_fee_details]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_fee_charges]    Script Date: 2016-03-16 02:50:50 PM ******/
DROP PROCEDURE [dbo].[sp_get_product_fee_charges]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_details_by_product]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_product_details_by_product]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_by_interfaces]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_product_by_interfaces]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_base_currency]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_product_base_currency]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_account_types]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_product_account_types]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_printfields_byproductid]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_printfields_byproductid]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_pinreissue_report]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_pinreissue_report]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_pin_reissue]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_pin_reissue]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_pin_mailer_reprint_requests]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_pin_mailer_reprint_requests]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_pin_block_format]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_pin_block_format]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_pin_batches_for_user]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_pin_batches_for_user]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_pin_batch_history]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_pin_batch_history]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_pin_batch_cards]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_pin_batch_cards]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_pin_batch_card_details]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_pin_batch_card_details]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_pin_batch]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_pin_batch]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_passwordshistorystatus]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_passwordshistorystatus]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_passwords_by_user_id]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_passwords_by_user_id]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_parameters_product_interface]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_parameters_product_interface]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_parameters_issuer_interface]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_parameters_issuer_interface]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_operator_cards_inprogress]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_operator_cards_inprogress]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_next_sequence]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_next_sequence]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_masterkey]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_masterkey]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_load_batches]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_load_batches]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_load_batch_history]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_load_batch_history]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_load_batch_cards]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_load_batch_cards]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_load_batch]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_load_batch]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_license_issuers]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_license_issuers]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_ldap_settings]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_ldap_settings]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_ldap_setting]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_ldap_setting]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_languages]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_languages]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuerdetails]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_issuerdetails]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuer_interfaces]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_issuer_interfaces]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuer_interface_details]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_issuer_interface_details]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuer_interface_conn]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_issuer_interface_conn]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuer_by_product_and_branchcode]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_issuer_by_product_and_branchcode]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuer_by_interface]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_issuer_by_interface]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuer_by_code]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_issuer_by_code]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuer_by_branch]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_issuer_by_branch]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuer]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_issuer]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuedcardsreport]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_issuedcardsreport]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuecardsummaryreport]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_issuecardsummaryreport]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_invetorysummaryreport]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_invetorysummaryreport]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_interface_Rowaccountbaseinfo]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_interface_Rowaccountbaseinfo]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_groups_roles_for_user]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_groups_roles_for_user]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_fonts_list]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_fonts_list]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_fonts_ist]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_fonts_ist]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_fileloaderlog]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_fileloaderlog]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_file_load_list]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_file_load_list]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_file_historys]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_file_historys]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_file_history]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_file_history]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_feerevenue_report]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_feerevenue_report]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_export_batch_history]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_export_batch_history]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_export_batch_cards]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_export_batch_cards]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_export_batch_card_details]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_export_batch_card_details]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_export_batch]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_export_batch]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_dist_batches_for_user]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_dist_batches_for_user]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_dist_batches]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_dist_batches]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_dist_batch_history]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_dist_batch_history]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_dist_batch_cards]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_dist_batch_cards]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_dist_batch_card_details_paged]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_dist_batch_card_details_paged]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_dist_batch_card_details]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_dist_batch_card_details]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_dist_batch]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_dist_batch]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_customercardsearch_list]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_customercardsearch_list]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_customer_details]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_customer_details]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_current_product_fee]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_current_product_fee]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_currenies_product]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_currenies_product]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_currency_list]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_currency_list]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_currency_iso_numeric_code]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_currency_iso_numeric_code]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_currency_id]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_currency_id]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_currency]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_currency]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_connection_params]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_connection_params]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_connection_parameter]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_connection_parameter]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_cms_parameters]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_cms_parameters]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_cms_bank_parameters]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_cms_bank_parameters]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_centercardstock_report]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_centercardstock_report]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_cards_in_error]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_cards_in_error]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_cardrequests]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_cardrequests]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_cardinventorysummaryreport]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_cardinventorysummaryreport]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_card_priority_list]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_card_priority_list]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_card_object]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_card_object]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_card_history_status]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_card_history_status]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_card_history_reference]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_card_history_reference]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_card_centre_card_count]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_card_centre_card_count]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_card]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_card]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_burnrate_report]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_burnrate_report]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_branches_with_load_card_count]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_branches_with_load_card_count]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_branches_for_userroles]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_branches_for_userroles]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_branches_for_user_group]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_branches_for_user_group]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_branches_for_user_admin]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_branches_for_user_admin]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_branches_for_user]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_branches_for_user]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_branches_for_issuer]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_branches_for_issuer]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_branchcardstock_report]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_branchcardstock_report]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_branch_card_count]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_branch_card_count]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_branch_by_id]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_branch_by_id]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_branch]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_branch]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_brachesperusergroup_auditreport]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_brachesperusergroup_auditreport]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_bank_codes]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_bank_codes]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_authpin_by_user_id]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_authpin_by_user_id]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_authenticationtypes]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_authenticationtypes]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_auditdata]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_auditdata]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_all_issuers_for_role]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_all_issuers_for_role]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_all_issuers]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_all_issuers]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_all_countries]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_get_all_countries]
GO
/****** Object:  StoredProcedure [dbo].[sp_generate_audit_report]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_generate_audit_report]
GO
/****** Object:  StoredProcedure [dbo].[sp_find_reissue_cards]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_find_reissue_cards]
GO
/****** Object:  StoredProcedure [dbo].[sp_find_issuer_by_status]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_find_issuer_by_status]
GO
/****** Object:  StoredProcedure [dbo].[sp_find_file_info_by_filename]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_find_file_info_by_filename]
GO
/****** Object:  StoredProcedure [dbo].[sp_find_distinct_load_requests]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_find_distinct_load_requests]
GO
/****** Object:  StoredProcedure [dbo].[sp_find_distinct_load_cards]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_find_distinct_load_cards]
GO
/****** Object:  StoredProcedure [dbo].[sp_finalise_logout]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_finalise_logout]
GO
/****** Object:  StoredProcedure [dbo].[sp_finalise_login_failed]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_finalise_login_failed]
GO
/****** Object:  StoredProcedure [dbo].[sp_finalise_login]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_finalise_login]
GO
/****** Object:  StoredProcedure [dbo].[sp_file_load_update]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_file_load_update]
GO
/****** Object:  StoredProcedure [dbo].[sp_file_load_create]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_file_load_create]
GO
/****** Object:  StoredProcedure [dbo].[sp_export_batch_status_request]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_export_batch_status_request]
GO
/****** Object:  StoredProcedure [dbo].[sp_export_batch_status_reject]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_export_batch_status_reject]
GO
/****** Object:  StoredProcedure [dbo].[sp_export_batch_status_exported]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_export_batch_status_exported]
GO
/****** Object:  StoredProcedure [dbo].[sp_export_batch_status_approve]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_export_batch_status_approve]
GO
/****** Object:  StoredProcedure [dbo].[sp_dist_batch_to_vault]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_dist_batch_to_vault]
GO
/****** Object:  StoredProcedure [dbo].[sp_dist_batch_status_reject]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_dist_batch_status_reject]
GO
/****** Object:  StoredProcedure [dbo].[sp_dist_batch_status_change]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_dist_batch_status_change]
GO
/****** Object:  StoredProcedure [dbo].[sp_dist_batch_reject_production]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_dist_batch_reject_production]
GO
/****** Object:  StoredProcedure [dbo].[sp_delete_user_group]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_delete_user_group]
GO
/****** Object:  StoredProcedure [dbo].[sp_delete_terminaldetails]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_delete_terminaldetails]
GO
/****** Object:  StoredProcedure [dbo].[sp_delete_subproduct]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_delete_subproduct]
GO
/****** Object:  StoredProcedure [dbo].[sp_delete_product]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_delete_product]
GO
/****** Object:  StoredProcedure [dbo].[sp_delete_masterkey]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_delete_masterkey]
GO
/****** Object:  StoredProcedure [dbo].[sp_delete_ldap]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_delete_ldap]
GO
/****** Object:  StoredProcedure [dbo].[sp_delete_font]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_delete_font]
GO
/****** Object:  StoredProcedure [dbo].[sp_delete_fee_scheme]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_delete_fee_scheme]
GO
/****** Object:  StoredProcedure [dbo].[sp_delete_connenction_params]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_delete_connenction_params]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_useradminsettings]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_create_useradminsettings]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_user_group]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_create_user_group]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_terminal]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_create_terminal]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_product_print_fields]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_create_product_print_fields]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_masterkey]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_create_masterkey]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_load_batch_bulk_card_request]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_create_load_batch_bulk_card_request]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_load_batch]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_create_load_batch]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_ldap]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_create_ldap]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_issuer]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_create_issuer]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_fee_scheme]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_create_fee_scheme]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_export_batches]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_create_export_batches]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_dist_batch]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_create_dist_batch]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_customer_fields]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_create_customer_fields]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_cms_upload_batch]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_create_cms_upload_batch]
GO
/****** Object:  StoredProcedure [dbo].[sp_connection_parameter_update]    Script Date: 2016-03-16 02:50:51 PM ******/
DROP PROCEDURE [dbo].[sp_connection_parameter_update]
GO
/****** Object:  StoredProcedure [dbo].[sp_connection_parameter_create]    Script Date: 2016-03-16 02:50:52 PM ******/
DROP PROCEDURE [dbo].[sp_connection_parameter_create]
GO
/****** Object:  StoredProcedure [dbo].[sp_cms_search_for_error]    Script Date: 2016-03-16 02:50:52 PM ******/
DROP PROCEDURE [dbo].[sp_cms_search_for_error]
GO
/****** Object:  StoredProcedure [dbo].[sp_cards_checkInOut]    Script Date: 2016-03-16 02:50:52 PM ******/
DROP PROCEDURE [dbo].[sp_cards_checkInOut]
GO
/****** Object:  StoredProcedure [dbo].[sp_card_pvv]    Script Date: 2016-03-16 02:50:52 PM ******/
DROP PROCEDURE [dbo].[sp_card_pvv]
GO
/****** Object:  StoredProcedure [dbo].[sp_card_production_report]    Script Date: 2016-03-16 02:50:52 PM ******/
DROP PROCEDURE [dbo].[sp_card_production_report]
GO
/****** Object:  StoredProcedure [dbo].[sp_card_object_details]    Script Date: 2016-03-16 02:50:52 PM ******/
DROP PROCEDURE [dbo].[sp_card_object_details]
GO
/****** Object:  StoredProcedure [dbo].[sp_card_MakerChecker]    Script Date: 2016-03-16 02:50:52 PM ******/
DROP PROCEDURE [dbo].[sp_card_MakerChecker]
GO
/****** Object:  StoredProcedure [dbo].[sp_card_expiry_report]    Script Date: 2016-03-16 02:50:52 PM ******/
DROP PROCEDURE [dbo].[sp_card_expiry_report]
GO
/****** Object:  StoredProcedure [dbo].[sp_card_dispatch_report]    Script Date: 2016-03-16 02:50:52 PM ******/
DROP PROCEDURE [dbo].[sp_card_dispatch_report]
GO
/****** Object:  StoredProcedure [dbo].[sp_card_branch_spoil]    Script Date: 2016-03-16 02:50:52 PM ******/
DROP PROCEDURE [dbo].[sp_card_branch_spoil]
GO
/****** Object:  StoredProcedure [dbo].[sp_branch_order_report]    Script Date: 2016-03-16 02:50:52 PM ******/
DROP PROCEDURE [dbo].[sp_branch_order_report]
GO
/****** Object:  StoredProcedure [dbo].[sp_activate_product]    Script Date: 2016-03-16 02:50:52 PM ******/
DROP PROCEDURE [dbo].[sp_activate_product]
GO
/****** Object:  StoredProcedure [dbo].[mod_sp_insert_Flexcube_audit]    Script Date: 2016-03-16 02:50:52 PM ******/
DROP PROCEDURE [dbo].[mod_sp_insert_Flexcube_audit]
GO
/****** Object:  StoredProcedure [dbo].[mod_sp_get_flex_parameter]    Script Date: 2016-03-16 02:50:52 PM ******/
DROP PROCEDURE [dbo].[mod_sp_get_flex_parameter]
GO
/****** Object:  StoredProcedure [dbo].[GenerateAuditReport]    Script Date: 2016-03-16 02:50:52 PM ******/
DROP PROCEDURE [dbo].[GenerateAuditReport]
GO
/****** Object:  StoredProcedure [dbo].[AddMacForTable]    Script Date: 2016-03-16 02:50:52 PM ******/
DROP PROCEDURE [dbo].[AddMacForTable]
GO
