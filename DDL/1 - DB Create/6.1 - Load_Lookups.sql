USE [indigo_database_group]
GO
INSERT [dbo].[customer_residency] ([resident_id], [residency_name]) VALUES (0, N'RESIDENT')
GO
INSERT [dbo].[customer_residency] ([resident_id], [residency_name]) VALUES (1, N'NONRESIDENT')
GO
INSERT [dbo].[languages] ([id], [language_name], [language_name_fr], [language_name_pt], [language_name_sp]) VALUES (0, N'English', N'Anglais', N'English_pt', N'English_sp')
GO
INSERT [dbo].[languages] ([id], [language_name], [language_name_fr], [language_name_pt], [language_name_sp]) VALUES (1, N'French', N'Français', N'French_pt', N'French_sp')
GO
INSERT [dbo].[languages] ([id], [language_name], [language_name_fr], [language_name_pt], [language_name_sp]) VALUES (2, N'Portuguese', N'Portugais', N'Portuguese_pt', N'Portuguese_sp')
GO
INSERT [dbo].[languages] ([id], [language_name], [language_name_fr], [language_name_pt], [language_name_sp]) VALUES (3, N'Spanish', N'Espagnol', N'Spanish_pt', N'Spanish_sp')
GO
INSERT [dbo].[customer_residency_language] ([resident_id], [language_id], [language_text]) VALUES (0, 0, N'RESIDENT')
GO
INSERT [dbo].[customer_residency_language] ([resident_id], [language_id], [language_text]) VALUES (0, 1, N'Résident')
GO
INSERT [dbo].[customer_residency_language] ([resident_id], [language_id], [language_text]) VALUES (0, 2, N'RESIDENT_pt')
GO
INSERT [dbo].[customer_residency_language] ([resident_id], [language_id], [language_text]) VALUES (0, 3, N'RESIDENT_sp')
GO
INSERT [dbo].[customer_residency_language] ([resident_id], [language_id], [language_text]) VALUES (1, 0, N'NONRESIDENT')
GO
INSERT [dbo].[customer_residency_language] ([resident_id], [language_id], [language_text]) VALUES (1, 1, N'Non résident')
GO
INSERT [dbo].[customer_residency_language] ([resident_id], [language_id], [language_text]) VALUES (1, 2, N'NONRESIDENT_pt')
GO
INSERT [dbo].[customer_residency_language] ([resident_id], [language_id], [language_text]) VALUES (1, 3, N'NONRESIDENT_sp')
GO
INSERT [dbo].[branch_card_statuses] ([branch_card_statuses_id], [branch_card_statuses_name]) VALUES (0, N'CHECKED_IN')
GO
INSERT [dbo].[branch_card_statuses] ([branch_card_statuses_id], [branch_card_statuses_name]) VALUES (1, N'AVAILABLE_FOR_ISSUE')
GO
INSERT [dbo].[branch_card_statuses] ([branch_card_statuses_id], [branch_card_statuses_name]) VALUES (2, N'ALLOCATED_TO_CUST')
GO
INSERT [dbo].[branch_card_statuses] ([branch_card_statuses_id], [branch_card_statuses_name]) VALUES (3, N'APPROVED_FOR_ISSUE')
GO
INSERT [dbo].[branch_card_statuses] ([branch_card_statuses_id], [branch_card_statuses_name]) VALUES (4, N'CARD_PRINTED')
GO
INSERT [dbo].[branch_card_statuses] ([branch_card_statuses_id], [branch_card_statuses_name]) VALUES (5, N'PIN_CAPTURED')
GO
INSERT [dbo].[branch_card_statuses] ([branch_card_statuses_id], [branch_card_statuses_name]) VALUES (6, N'ISSUED')
GO
INSERT [dbo].[branch_card_statuses] ([branch_card_statuses_id], [branch_card_statuses_name]) VALUES (7, N'SPOILED')
GO
INSERT [dbo].[branch_card_statuses] ([branch_card_statuses_id], [branch_card_statuses_name]) VALUES (8, N'PRINT_ERROR')
GO
INSERT [dbo].[branch_card_statuses] ([branch_card_statuses_id], [branch_card_statuses_name]) VALUES (9, N'CMS_ERROR')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (0, 0, N'CHECKED_IN')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (0, 1, N'DESALLOUÉES')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (0, 2, N'CHECKED_IN_pt')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (0, 3, N'CHECKED_IN_sp')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (1, 0, N'AVAILABLE_FOR_ISSUE')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (1, 1, N'PRÊTES POUR EMISSION')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (1, 2, N'AVAILABLE_FOR_ISSUE_pt')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (1, 3, N'AVAILABLE_FOR_ISSUE_sp')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (2, 0, N'ALLOCATED_TO_CUST')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (2, 1, N'EMISES AUX CLIENTS')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (2, 2, N'ALLOCATED_TO_CUST_pt')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (2, 3, N'ALLOCATED_TO_CUST_sp')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (3, 0, N'APPROVED_FOR_ISSUE')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (3, 1, N'APPROUVÉES POUR EMISSION')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (3, 2, N'APPROVED_FOR_ISSUE_pt')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (3, 3, N'APPROVED_FOR_ISSUE_sp')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (4, 0, N'CARD_PRINTED')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (4, 1, N'CARTES IMPRIMEES')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (4, 2, N'CARD_PRINTED_pt')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (4, 3, N'CARD_PRINTED_sp')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (5, 0, N'PIN_CAPTURED')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (5, 1, N'PIN ENREGISTRÉS')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (5, 2, N'PIN_CAPTURED_pt')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (5, 3, N'PIN_CAPTURED_sp')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (6, 0, N'ISSUED')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (6, 1, N'EMISES')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (6, 2, N'ISSUED_pt')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (6, 3, N'ISSUED_sp')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (7, 0, N'SPOILED')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (7, 1, N'DETRUITES')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (7, 2, N'SPOILED_pt')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (7, 3, N'SPOILED_sp')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (8, 0, N'PRINT_ERROR')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (8, 1, N'ERREUR D''IMPRESSION')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (8, 2, N'PRINT_ERROR_pt')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (8, 3, N'PRINT_ERROR_sp')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (9, 0, N'CMS_ERROR')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (9, 1, N'ERREUR CMS')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (9, 2, N'CMS_ERROR_pt')
GO
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (9, 3, N'CMS_ERROR_sp')
GO
INSERT [dbo].[branch_statuses] ([branch_status_id], [branch_status]) VALUES (0, N'ACTIVE')
GO
INSERT [dbo].[branch_statuses] ([branch_status_id], [branch_status]) VALUES (2, N'DELETED')
GO
INSERT [dbo].[branch_statuses] ([branch_status_id], [branch_status]) VALUES (1, N'INACTIVE')
GO
INSERT [dbo].[branch_statuses_language] ([branch_status_id], [language_id], [language_text]) VALUES (0, 0, N'ACTIVE')
GO
INSERT [dbo].[branch_statuses_language] ([branch_status_id], [language_id], [language_text]) VALUES (0, 1, N'ACTIVE')
GO
INSERT [dbo].[branch_statuses_language] ([branch_status_id], [language_id], [language_text]) VALUES (0, 2, N'ACTIVE_pt')
GO
INSERT [dbo].[branch_statuses_language] ([branch_status_id], [language_id], [language_text]) VALUES (0, 3, N'ACTIVE_sp')
GO
INSERT [dbo].[branch_statuses_language] ([branch_status_id], [language_id], [language_text]) VALUES (1, 0, N'INACTIVE')
GO
INSERT [dbo].[branch_statuses_language] ([branch_status_id], [language_id], [language_text]) VALUES (1, 1, N'INACTIF')
GO
INSERT [dbo].[branch_statuses_language] ([branch_status_id], [language_id], [language_text]) VALUES (1, 2, N'INACTIVE_pt')
GO
INSERT [dbo].[branch_statuses_language] ([branch_status_id], [language_id], [language_text]) VALUES (1, 3, N'INACTIVE_sp')
GO
INSERT [dbo].[branch_statuses_language] ([branch_status_id], [language_id], [language_text]) VALUES (2, 0, N'DELETED')
GO
INSERT [dbo].[branch_statuses_language] ([branch_status_id], [language_id], [language_text]) VALUES (2, 1, N'SUPPRIME')
GO
INSERT [dbo].[branch_statuses_language] ([branch_status_id], [language_id], [language_text]) VALUES (2, 2, N'DELETED_pt')
GO
INSERT [dbo].[branch_statuses_language] ([branch_status_id], [language_id], [language_text]) VALUES (2, 3, N'DELETED_sp')
GO
INSERT [dbo].[card_issue_reason] ([card_issue_reason_id], [card_issuer_reason_name]) VALUES (0, N'NEW ACCOUNT - NEW CUSTOMER')
GO
INSERT [dbo].[card_issue_reason] ([card_issue_reason_id], [card_issuer_reason_name]) VALUES (1, N'NEW ACCOUNT - EXISTING CUSTOMER')
GO
INSERT [dbo].[card_issue_reason] ([card_issue_reason_id], [card_issuer_reason_name]) VALUES (2, N'CARD RENEWAL')
GO
INSERT [dbo].[card_issue_reason] ([card_issue_reason_id], [card_issuer_reason_name]) VALUES (3, N'CARD REPLACEMENT')
GO
INSERT [dbo].[card_issue_reason] ([card_issue_reason_id], [card_issuer_reason_name]) VALUES (4, N'SUPPLEMENTARY CARD')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (0, 0, N'NEW ACCOUNT - NEW CUSTOMER')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (0, 1, N'NOUVEAU COMPTE - NOUVEAU CLIENT')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (0, 2, N'NEW ACCOUNT - NEW CUSTOMER_pt')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (0, 3, N'NEW ACCOUNT - NEW CUSTOMER_sp')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (1, 0, N'NEW ACCOUNT - EXISTING CUSTOMER')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (1, 1, N'NOUVEAU COMPTE - CLIENT EXISTANT')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (1, 2, N'NEW ACCOUNT - EXISTING CUSTOMER_pt')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (1, 3, N'NEW ACCOUNT - EXISTING CUSTOMER_sp')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (2, 0, N'CARD RENEWAL')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (2, 1, N'RENOUVELLEMENT DE CARTE')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (2, 2, N'CARD RENEWAL_pt')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (2, 3, N'CARD RENEWAL_sp')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (3, 0, N'CARD REPLACEMENT')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (3, 1, N'REMPLACEMENT DE CARTE')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (3, 2, N'CARD REPLACEMENT_pt')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (3, 3, N'CARD REPLACEMENT_sp')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (4, 0, N'SUPPLEMENTARY CARD')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (4, 1, N'CARTE SUPPLMENTAIRE')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (4, 2, N'SUPPLEMENTARY CARD_pt')
GO
INSERT [dbo].[card_issue_reason_language] ([card_issue_reason_id], [language_id], [language_text]) VALUES (4, 3, N'SUPPLEMENTARY CARD_sp')
GO
INSERT [dbo].[customer_title] ([customer_title_id], [customer_title_name]) VALUES (0, N'MR')
GO
INSERT [dbo].[customer_title] ([customer_title_id], [customer_title_name]) VALUES (1, N'MRS')
GO
INSERT [dbo].[customer_title] ([customer_title_id], [customer_title_name]) VALUES (2, N'MISS')
GO
INSERT [dbo].[customer_title] ([customer_title_id], [customer_title_name]) VALUES (3, N'MS')
GO
INSERT [dbo].[customer_title] ([customer_title_id], [customer_title_name]) VALUES (4, N'PROF')
GO
INSERT [dbo].[customer_title] ([customer_title_id], [customer_title_name]) VALUES (5, N'DR')
GO
INSERT [dbo].[customer_title] ([customer_title_id], [customer_title_name]) VALUES (6, N'REV')
GO
INSERT [dbo].[customer_title] ([customer_title_id], [customer_title_name]) VALUES (7, N'OTHER')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (0, 0, N'MR')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (0, 1, N'Mr.')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (0, 2, N'MR_pt')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (0, 3, N'MR_sp')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (1, 0, N'MRS')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (1, 1, N'Me.')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (1, 2, N'MRS_pt')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (1, 3, N'MRS_sp')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (2, 0, N'MISS')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (2, 1, N'Mlle.')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (2, 2, N'MISS_pt')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (2, 3, N'MISS_sp')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (3, 0, N'MS')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (3, 1, N'Me.')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (3, 2, N'MS_pt')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (3, 3, N'MS_sp')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (4, 0, N'PROF')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (4, 1, N'Prof.')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (4, 2, N'PROF_pt')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (4, 3, N'PROF_sp')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (5, 0, N'DR')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (5, 1, N'Dr.')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (5, 2, N'DR_pt')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (5, 3, N'DR_sp')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (6, 0, N'REV')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (6, 1, N'Rev.')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (6, 2, N'REV_pt')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (6, 3, N'REV_sp')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (7, 0, N'OTHER')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (7, 1, N'Autres')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (7, 2, N'OTHER_pt')
GO
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (7, 3, N'OTHER_sp')
GO
INSERT [dbo].[customer_type] ([customer_type_id], [customer_type_name]) VALUES (0, N'PRIVATE')
GO
INSERT [dbo].[customer_type] ([customer_type_id], [customer_type_name]) VALUES (1, N'CORPORATE')
GO
INSERT [dbo].[customer_type_language] ([customer_type_id], [language_id], [language_text]) VALUES (0, 0, N'PRIVATE')
GO
INSERT [dbo].[customer_type_language] ([customer_type_id], [language_id], [language_text]) VALUES (0, 1, N'Privé')
GO
INSERT [dbo].[customer_type_language] ([customer_type_id], [language_id], [language_text]) VALUES (0, 2, N'PRIVATE_pt')
GO
INSERT [dbo].[customer_type_language] ([customer_type_id], [language_id], [language_text]) VALUES (0, 3, N'PRIVATE_sp')
GO
INSERT [dbo].[customer_type_language] ([customer_type_id], [language_id], [language_text]) VALUES (1, 0, N'CORPORATE')
GO
INSERT [dbo].[customer_type_language] ([customer_type_id], [language_id], [language_text]) VALUES (1, 1, N'Entreprise')
GO
INSERT [dbo].[customer_type_language] ([customer_type_id], [language_id], [language_text]) VALUES (1, 2, N'CORPORATE_pt')
GO
INSERT [dbo].[customer_type_language] ([customer_type_id], [language_id], [language_text]) VALUES (1, 3, N'CORPORATE_sp')
GO
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (0, N'CREATED', NULL)
GO
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (1, N'APPROVED', 0)
GO
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (2, N'DISPATCHED', 1)
GO
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (3, N'RECEIVED_AT_BRANCH', 2)
GO
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (4, N'REJECTED_AT_BRANCH', 2)
GO
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (5, N'REJECT_AND_REISSUE', 0)
GO
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (6, N'REJECT_AND_CANCEL', 0)
GO
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (7, N'INVALID', NULL)
GO
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (8, N'REJECTED', NULL)
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (0, 0, N'CREATED')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (0, 1, N'CRÉÉ')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (0, 2, N'CREATED_pt')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (0, 3, N'CREATED_sp')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (1, 0, N'APPROVED')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (1, 1, N'APPROUVÉ')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (1, 2, N'APPROVED_pt')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (1, 3, N'APPROVED_sp')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (2, 0, N'DISPATCHED')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (2, 1, N'DISTRIBUÉ')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (2, 2, N'DISPATCHED_pt')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (2, 3, N'DISPATCHED_sp')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (3, 0, N'RECEIVED_AT_BRANCH')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (3, 1, N'REÇU EN AGENCE')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (3, 2, N'RECEIVED_AT_BRANCH_pt')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (3, 3, N'RECEIVED_AT_BRANCH_sp')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (4, 0, N'REJECTED_AT_BRANCH')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (4, 1, N'REJETÉ EN AGENCE')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (4, 2, N'REJECTED_AT_BRANCH_pt')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (4, 3, N'REJECTED_AT_BRANCH_sp')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (5, 0, N'REJECT_AND_REISSUE')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (5, 1, N'REJET ET REEMISSION')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (5, 2, N'REJECT_AND_REISSUE_pt')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (5, 3, N'REJECT_AND_REISSUE_sp')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (6, 0, N'REJECT_AND_CANCEL')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (6, 1, N'REJET ET ANNULATION')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (6, 2, N'REJECT_AND_CANCEL_pt')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (6, 3, N'REJECT_AND_CANCEL_sp')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (7, 0, N'INVALID')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (7, 1, N'INVALID')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (7, 2, N'INVALID_pt')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (7, 3, N'INVALID_sp')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (8, 0, N'REJECTED')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (8, 1, N'REJETÉ')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (8, 2, N'REJECTED_pt')
GO
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (8, 3, N'REJECTED_sp')
GO
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (0, N'ALLOCATED_TO_BRANCH')
GO
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (1, N'AVAILABLE_FOR_ISSUE')
GO
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (2, N'RECEIVED_AT_BRANCH')
GO
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (3, N'ALLOCATED_TO_CUST')
GO
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (4, N'CARD_PRINTED')
GO
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (5, N'PIN_CAPTURED')
GO
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (6, N'ISSUED')
GO
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (7, N'REJECTED')
GO
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (8, N'CANCELLED')
GO
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (9, N'INVALID')
GO
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (10, N'LINKED_TO_ACCOUNT')
GO
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (11, N'SPOILED')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (0, 0, N'ALLOCATED_TO_BRANCH')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (0, 1, N'ALLOCATED_TO_BRANCH_fr')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (0, 2, N'ALLOCATED_TO_BRANCH_pt')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (0, 3, N'ALLOCATED_TO_BRANCH_sp')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (1, 0, N'AVAILABLE_FOR_ISSUE')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (1, 1, N'AVAILABLE_FOR_ISSUE_fr')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (1, 2, N'AVAILABLE_FOR_ISSUE_pt')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (1, 3, N'AVAILABLE_FOR_ISSUE_sp')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (2, 0, N'RECEIVED_AT_BRANCH')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (2, 1, N'RECEIVED_AT_BRANCH_fr')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (2, 2, N'RECEIVED_AT_BRANCH_pt')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (2, 3, N'RECEIVED_AT_BRANCH_sp')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (3, 0, N'ALLOCATED_TO_CUST')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (3, 1, N'ALLOCATED_TO_CUST_fr')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (3, 2, N'ALLOCATED_TO_CUST_pt')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (3, 3, N'ALLOCATED_TO_CUST_sp')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (4, 0, N'CARD_PRINTED')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (4, 1, N'CARD_PRINTED_fr')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (4, 2, N'CARD_PRINTED_pt')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (4, 3, N'CARD_PRINTED_sp')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (5, 0, N'PIN_CAPTURED')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (5, 1, N'PIN_CAPTURED_fr')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (5, 2, N'PIN_CAPTURED_pt')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (5, 3, N'PIN_CAPTURED_sp')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (6, 0, N'ISSUED')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (6, 1, N'ISSUED_fr')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (6, 2, N'ISSUED_pt')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (6, 3, N'ISSUED_sp')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (7, 0, N'REJECTED')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (7, 1, N'REJECTED_fr')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (7, 2, N'REJECTED_pt')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (7, 3, N'REJECTED_sp')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (8, 0, N'CANCELLED')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (8, 1, N'CANCELLED_fr')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (8, 2, N'CANCELLED_pt')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (8, 3, N'CANCELLED_sp')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (9, 0, N'INVALID')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (9, 1, N'INVALID_fr')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (9, 2, N'INVALID_pt')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (9, 3, N'INVALID_sp')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (10, 0, N'LINKED_TO_ACCOUNT')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (10, 1, N'LINKED_TO_ACCOUNT_fr')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (10, 2, N'LINKED_TO_ACCOUNT_pt')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (10, 3, N'LINKED_TO_ACCOUNT_sp')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (11, 0, N'SPOILED')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (11, 1, N'SPOILED_fr')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (11, 2, N'SPOILED_pt')
GO
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (11, 3, N'SPOILED_sp')
GO
INSERT [dbo].[interface_type] ([interface_type_id], [interface_type_name]) VALUES (0, N'ACCOUNT_VALIDATION')
GO
INSERT [dbo].[interface_type] ([interface_type_id], [interface_type_name]) VALUES (1, N'CORE_BANKING')
GO
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (0, 0, N'ACCOUNT_VALIDATION')
GO
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (0, 1, N'VALIDATION DU COMPTE')
GO
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (0, 2, N'ACCOUNT_VALIDATION_pt')
GO
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (0, 3, N'ACCOUNT_VALIDATION_sp')
GO
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (1, 0, N'CORE_BANKING')
GO
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (1, 1, N'SERVICE BANCAIRES DE BASE')
GO
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (1, 2, N'CORE_BANKING_pt')
GO
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (1, 3, N'CORE_BANKING_sp')
GO
INSERT [dbo].[branch_card_code_type] ([branch_card_code_type_id], [branch_card_code_name]) VALUES (0, N'PRINTER')
GO
INSERT [dbo].[branch_card_code_type] ([branch_card_code_type_id], [branch_card_code_name]) VALUES (1, N'CMS')
GO
INSERT [dbo].[branch_card_codes] ([branch_card_code_id], [branch_card_code_type_id], [branch_card_code_name], [branch_card_code_enabled], [spoil_only], [is_exception]) VALUES (0, 0, N'PRINT_SUCCESS', 1, 0, 0)
GO
INSERT [dbo].[branch_card_codes] ([branch_card_code_id], [branch_card_code_type_id], [branch_card_code_name], [branch_card_code_enabled], [spoil_only], [is_exception]) VALUES (1, 0, N'PRINTER_JAMMED', 1, 1, 1)
GO
INSERT [dbo].[branch_card_codes] ([branch_card_code_id], [branch_card_code_type_id], [branch_card_code_name], [branch_card_code_enabled], [spoil_only], [is_exception]) VALUES (2, 0, N'CARD_INSERTED_INCORRECTLY', 1, 1, 1)
GO
INSERT [dbo].[branch_card_codes] ([branch_card_code_id], [branch_card_code_type_id], [branch_card_code_name], [branch_card_code_enabled], [spoil_only], [is_exception]) VALUES (3, 0, N'PRINTER_NO_INK', 1, 1, 1)
GO
INSERT [dbo].[branch_card_codes] ([branch_card_code_id], [branch_card_code_type_id], [branch_card_code_name], [branch_card_code_enabled], [spoil_only], [is_exception]) VALUES (4, 1, N'CMS_SUCCESS', 1, 0, 0)
GO
INSERT [dbo].[branch_card_codes] ([branch_card_code_id], [branch_card_code_type_id], [branch_card_code_name], [branch_card_code_enabled], [spoil_only], [is_exception]) VALUES (5, 1, N'CARD_NOT_FOUND', 1, 1, 1)
GO
INSERT [dbo].[branch_card_codes] ([branch_card_code_id], [branch_card_code_type_id], [branch_card_code_name], [branch_card_code_enabled], [spoil_only], [is_exception]) VALUES (6, 1, N'ACCOUNT_NOT_FOUND', 1, 1, 1)
GO
INSERT [dbo].[branch_card_codes] ([branch_card_code_id], [branch_card_code_type_id], [branch_card_code_name], [branch_card_code_enabled], [spoil_only], [is_exception]) VALUES (7, 1, N'UNKNOWN_ERROR', 1, 1, 1)
GO
INSERT [dbo].[branch_card_codes] ([branch_card_code_id], [branch_card_code_type_id], [branch_card_code_name], [branch_card_code_enabled], [spoil_only], [is_exception]) VALUES (8, 1, N'RELINK_FAILED', 1, 0, 1)
GO
INSERT [dbo].[branch_card_codes] ([branch_card_code_id], [branch_card_code_type_id], [branch_card_code_name], [branch_card_code_enabled], [spoil_only], [is_exception]) VALUES (9, 1, N'EDIT_FAILED', 1, 0, 1)
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (0, 0, N'PRINT_SUCCESS')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (1, 0, N'PRINTER_JAMMED')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (2, 0, N'CARD_INSERTED_INCORRECTLY')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (3, 0, N'PRINTER_NO_INK')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (4, 0, N'CMS_SUCCESS')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (5, 0, N'CARD_NOT_FOUND')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (6, 0, N'ACCOUNT_NOT_FOUND')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (7, 0, N'UNKNOWN_ERROR')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (8, 0, N'RELINK_FAILED')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (9, 0, N'EDIT_FAILED')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (0, 1, N'Impression a été faite avec succès')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (1, 1, N'IMPRIMANTE EN BOURRAGE')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (2, 1, N'CARTE MAL INSEREE')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (3, 1, N'ANKRE INSUFFISANT')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (4, 1, N'OPERATIONS')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (5, 1, N'CARTE INTROUVABLE')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (6, 1, N'COMPTE INTROUVABLE')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (7, 1, N'ERREUR ETRANGE')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (8, 1, N'RELIAGE A ECHOUE')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (9, 1, N'MODIFICATION A ECHOUEE')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (0, 2, N'PRINTER_JAMMED_fr')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (1, 2, N'PRINTER_JAMMED_pt')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (2, 2, N'CARD_INSERTED_INCORRECTLY_pt')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (3, 2, N'PRINTER_NO_INK_pt')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (4, 2, N'CMS_SUCCESS_pt')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (5, 2, N'CARD_NOT_FOUND_pt')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (6, 2, N'ACCOUNT_NOT_FOUND_pt')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (7, 2, N'UNKNOWN_ERROR_pt')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (8, 2, N'RELINK_FAILED_pt')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (9, 2, N'EDIT_FAILED_pt')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (0, 3, N'PRINT_SUCCESS_sp')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (2, 3, N'CARD_INSERTED_INCORRECTLY_sp')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (3, 3, N'PRINTER_NO_INK_sp')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (4, 3, N'CMS_SUCCESS_sp')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (5, 3, N'CARD_NOT_FOUND_sp')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (6, 3, N'ACCOUNT_NOT_FOUND_sp')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (7, 3, N'UNKNOWN_ERROR_sp')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (8, 3, N'RELINK_FAILED_sp')
GO
INSERT [dbo].[branch_card_codes_language] ([branch_card_code_id], [language_id], [language_text]) VALUES (9, 3, N'EDIT_FAILED_sp')
GO
INSERT [dbo].[issuer_statuses] ([issuer_status_id], [issuer_status_name]) VALUES (0, N'ACTIVE')
GO
INSERT [dbo].[issuer_statuses] ([issuer_status_id], [issuer_status_name]) VALUES (1, N'INACTIVE')
GO
INSERT [dbo].[issuer_statuses] ([issuer_status_id], [issuer_status_name]) VALUES (2, N'DELETED')
GO
INSERT [dbo].[issuer_statuses_language] ([issuer_status_id], [language_id], [language_text]) VALUES (0, 0, N'ACTIVE')
GO
INSERT [dbo].[issuer_statuses_language] ([issuer_status_id], [language_id], [language_text]) VALUES (0, 1, N'ACTIVE')
GO
INSERT [dbo].[issuer_statuses_language] ([issuer_status_id], [language_id], [language_text]) VALUES (0, 2, N'ACTIVE_pt')
GO
INSERT [dbo].[issuer_statuses_language] ([issuer_status_id], [language_id], [language_text]) VALUES (0, 3, N'ACTIVE_sp')
GO
INSERT [dbo].[issuer_statuses_language] ([issuer_status_id], [language_id], [language_text]) VALUES (1, 0, N'INACTIVE')
GO
INSERT [dbo].[issuer_statuses_language] ([issuer_status_id], [language_id], [language_text]) VALUES (1, 1, N'INACTIF')
GO
INSERT [dbo].[issuer_statuses_language] ([issuer_status_id], [language_id], [language_text]) VALUES (1, 2, N'INACTIVE_pt')
GO
INSERT [dbo].[issuer_statuses_language] ([issuer_status_id], [language_id], [language_text]) VALUES (1, 3, N'INACTIVE_sp')
GO
INSERT [dbo].[issuer_statuses_language] ([issuer_status_id], [language_id], [language_text]) VALUES (2, 0, N'DELETED')
GO
INSERT [dbo].[issuer_statuses_language] ([issuer_status_id], [language_id], [language_text]) VALUES (2, 1, N'SUPPRIME')
GO
INSERT [dbo].[issuer_statuses_language] ([issuer_status_id], [language_id], [language_text]) VALUES (2, 2, N'DELETED_pt')
GO
INSERT [dbo].[issuer_statuses_language] ([issuer_status_id], [language_id], [language_text]) VALUES (2, 3, N'DELETED_sp')
GO
INSERT [dbo].[customer_account_type] ([account_type_id], [account_type_name]) VALUES (0, N'CURRENT')
GO
INSERT [dbo].[customer_account_type] ([account_type_id], [account_type_name]) VALUES (1, N'SAVINGS')
GO
INSERT [dbo].[customer_account_type] ([account_type_id], [account_type_name]) VALUES (2, N'CHEQUE')
GO
INSERT [dbo].[customer_account_type] ([account_type_id], [account_type_name]) VALUES (3, N'CREDIT')
GO
INSERT [dbo].[customer_account_type] ([account_type_id], [account_type_name]) VALUES (4, N'UNIVERSAL')
GO
INSERT [dbo].[customer_account_type] ([account_type_id], [account_type_name]) VALUES (5, N'INVESTMENT')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (0, 0, N'CURRENT')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (0, 1, N'COURANT')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (0, 2, N'CURRENT_pt')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (0, 3, N'CURRENT_sp')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (1, 0, N'SAVINGS')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (1, 1, N'EPARGNE')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (1, 2, N'SAVINGS_pt')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (1, 3, N'SAVINGS_sp')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (2, 0, N'CHEQUE')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (2, 1, N'CHEQUE')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (2, 2, N'CHEQUE_pt')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (2, 3, N'CHEQUE_sp')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (3, 0, N'CREDIT')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (3, 1, N'CREDIT')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (3, 2, N'CREDIT_pt')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (3, 3, N'CREDIT_sp')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (4, 0, N'UNIVERSAL')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (4, 1, N'UNIVERSEL')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (4, 2, N'UNIVERSAL_pt')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (4, 3, N'UNIVERSAL_sp')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (5, 0, N'INVESTMENT')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (5, 1, N'INVESTISSEMENT')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (5, 2, N'INVESTMENT_pt')
GO
INSERT [dbo].[customer_account_type_language] ([account_type_id], [language_id], [language_text]) VALUES (5, 3, N'INVESTMENT_sp')
GO
INSERT [dbo].[audit_action] ([audit_action_id], [audit_action_name]) VALUES (0, N'BranchAdmin')
GO
INSERT [dbo].[audit_action] ([audit_action_id], [audit_action_name]) VALUES (1, N'CardManagement')
GO
INSERT [dbo].[audit_action] ([audit_action_id], [audit_action_name]) VALUES (2, N'DistributionBatch')
GO
INSERT [dbo].[audit_action] ([audit_action_id], [audit_action_name]) VALUES (3, N'IssueCard')
GO
INSERT [dbo].[audit_action] ([audit_action_id], [audit_action_name]) VALUES (4, N'IssuerAdmin')
GO
INSERT [dbo].[audit_action] ([audit_action_id], [audit_action_name]) VALUES (5, N'LoadBatch')
GO
INSERT [dbo].[audit_action] ([audit_action_id], [audit_action_name]) VALUES (6, N'Logon')
GO
INSERT [dbo].[audit_action] ([audit_action_id], [audit_action_name]) VALUES (7, N'UserAdmin')
GO
INSERT [dbo].[audit_action] ([audit_action_id], [audit_action_name]) VALUES (8, N'UserGroupAdmin')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (0, 0, N'BranchAdmin')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (0, 1, N'Admin_Agence')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (0, 2, N'BranchAdmin_pt')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (0, 3, N'BranchAdmin_sp')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (1, 0, N'CardManagement')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (1, 1, N'Gestion des cartes')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (1, 2, N'CardManagement_pt')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (1, 3, N'CardManagement_sp')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (2, 0, N'DistributionBatch')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (2, 1, N'Batch des distributions')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (2, 2, N'DistributionBatch_pt')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (2, 3, N'DistributionBatch_sp')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (3, 0, N'IssueCard')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (3, 1, N'Emettre carte')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (3, 2, N'IssueCard_pt')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (3, 3, N'IssueCard_sp')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (4, 0, N'IssuerAdmin')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (4, 1, N'Admin_Emetteur')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (4, 2, N'IssuerAdmin_pt')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (4, 3, N'IssuerAdmin_sp')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (5, 0, N'LoadBatch')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (5, 1, N'Batch chargé')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (5, 2, N'LoadBatch_pt')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (5, 3, N'LoadBatch_sp')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (6, 0, N'Logon')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (6, 1, N'Se connecter')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (6, 2, N'Logon_pt')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (6, 3, N'Logon_sp')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (7, 0, N'UserAdmin')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (7, 1, N'Utilisateur Admin')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (7, 2, N'UserAdmin_pt')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (7, 3, N'UserAdmin_sp')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (8, 0, N'UserGroupAdmin')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (8, 1, N'Utilisateur Admin Groupe')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (8, 2, N'UserGroupAdmin_pt')
GO
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (8, 3, N'UserGroupAdmin_sp')
GO
INSERT [dbo].[user_status] ([user_status_id], [user_status_text]) VALUES (3, N'ACCOUNT_LOCKED')
GO
INSERT [dbo].[user_status] ([user_status_id], [user_status_text]) VALUES (0, N'ACTIVE')
GO
INSERT [dbo].[user_status] ([user_status_id], [user_status_text]) VALUES (2, N'DELETED')
GO
INSERT [dbo].[user_status] ([user_status_id], [user_status_text]) VALUES (1, N'INACTIVE')
GO
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (0, 0, N'ACTIVE')
GO
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (0, 1, N'ACTIVE')
GO
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (0, 2, N'ACTIVE_pt')
GO
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (0, 3, N'ACTIVE_sp')
GO
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (1, 0, N'INACTIVE')
GO
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (1, 1, N'INACTIF')
GO
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (1, 2, N'INACTIVE_pt')
GO
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (1, 3, N'INACTIVE_sp')
GO
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (2, 0, N'DELETED')
GO
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (2, 1, N'SUPPRIME')
GO
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (2, 2, N'DELETED_pt')
GO
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (2, 3, N'DELETED_sp')
GO
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (3, 0, N'ACCOUNT_LOCKED')
GO
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (3, 1, N'COMPTE BLOQUE')
GO
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (3, 2, N'ACCOUNT_LOCKED_pt')
GO
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (3, 3, N'ACCOUNT_LOCKED_sp')
GO
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (0, N'ADMINISTRATOR', 0, 0)
GO
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (1, N'AUDITOR', 0, 0)
GO
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (2, N'BRANCH_CUSTODIAN', 0, 0)
GO
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (3, N'BRANCH_OPERATOR', 0, 0)
GO
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (4, N'CENTER_MANAGER', 0, 0)
GO
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (5, N'CENTER_OPERATOR', 0, 0)
GO
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (6, N'ISSUER_ADMIN', 0, 0)
GO
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (7, N'PIN_OPERATOR', 0, 0)
GO
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (8, N'USER_GROUP_ADMIN', 0, 1)
GO
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (9, N'USER_ADMIN', 1, 1)
GO
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (10, N'BRANCH_ADMIN', 0, 0)
GO
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (11, N'CARD_PRODUCTION', 0, 0)
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (0, 0, N'ADMINISTRATOR')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (0, 1, N'ADMINISTRATEUR')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (0, 2, N'ADMINISTRATOR_pt')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (0, 3, N'ADMINISTRATOR_sp')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (1, 0, N'AUDITOR')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (1, 1, N'AUDITEUR')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (1, 2, N'AUDITOR_pt')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (1, 3, N'AUDITOR_sp')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (2, 0, N'BRANCH_CUSTODIAN')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (2, 1, N'SUPERVISEUR AGENCE')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (2, 2, N'BRANCH_CUSTODIAN_pt')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (2, 3, N'BRANCH_CUSTODIAN_sp')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (3, 0, N'BRANCH_OPERATOR')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (3, 1, N'OPERATEUR AGENCE')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (3, 2, N'BRANCH_OPERATOR_pt')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (3, 3, N'BRANCH_OPERATOR_sp')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (4, 0, N'CENTER_MANAGER')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (4, 1, N'CHEF CARD CENTER')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (4, 2, N'CENTER_MANAGER_pt')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (4, 3, N'CENTER_MANAGER_sp')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (5, 0, N'CENTER_OPERATOR')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (5, 1, N'OPERATEUR CARD CENTER')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (5, 2, N'CENTER_OPERATOR_pt')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (5, 3, N'CENTER_OPERATOR_sp')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (6, 0, N'ISSUER_ADMIN')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (6, 1, N'ADMIN EMETTEUR')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (6, 2, N'ISSUER_ADMIN_pt')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (6, 3, N'ISSUER_ADMIN_sp')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (7, 0, N'PIN_OPERATOR')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (7, 1, N'OPERATEUR PIN')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (7, 2, N'PIN_OPERATOR_pt')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (7, 3, N'PIN_OPERATOR_sp')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (8, 0, N'USER_GROUP_ADMIN')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (8, 1, N'ADMIN GROUPES D''UTILISATEURS')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (8, 2, N'USER_GROUP_ADMIN_pt')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (8, 3, N'USER_GROUP_ADMIN_sp')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (9, 0, N'USER_ADMIN')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (9, 1, N'ADMIN UTILISATEURS')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (9, 2, N'USER_ADMIN_pt')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (9, 3, N'USER_ADMIN_sp')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (10, 0, N'BRANCH_ADMIN')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (10, 1, N'ADMIN AGENCE')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (10, 2, N'BRANCH_ADMIN_pt')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (10, 3, N'BRANCH_ADMIN_sp')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (11, 0, N'CARD_PRODUCTION')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (11, 1, N'CARD_PRODUCTION_fr')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (11, 2, N'CARD_PRODUCTION_pt')
GO
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (11, 3, N'CARD_PRODUCTION_es')
GO
INSERT [dbo].[load_card_statuses] ([load_card_status_id], [load_card_status]) VALUES (2, N'ALLOCATED')
GO
INSERT [dbo].[load_card_statuses] ([load_card_status_id], [load_card_status]) VALUES (1, N'AVAILABLE')
GO
INSERT [dbo].[load_card_statuses] ([load_card_status_id], [load_card_status]) VALUES (4, N'CANCELLED')
GO
INSERT [dbo].[load_card_statuses] ([load_card_status_id], [load_card_status]) VALUES (5, N'INVALID')
GO
INSERT [dbo].[load_card_statuses] ([load_card_status_id], [load_card_status]) VALUES (0, N'LOADED')
GO
INSERT [dbo].[load_card_statuses] ([load_card_status_id], [load_card_status]) VALUES (3, N'REJECTED')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (0, 0, N'LOADED')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (0, 1, N'LOADED_fr')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (0, 2, N'LOADED_pt')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (0, 3, N'LOADED_sp')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (1, 0, N'AVAILABLE')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (1, 1, N'AVAILABLE_fr')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (1, 2, N'AVAILABLE_pt')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (1, 3, N'AVAILABLE_sp')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (2, 0, N'ALLOCATED')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (2, 1, N'ALLOCATED_fr')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (2, 2, N'ALLOCATED_pt')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (2, 3, N'ALLOCATED_sp')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (3, 0, N'REJECTED')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (3, 1, N'REJECTED_fr')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (3, 2, N'REJECTED_pt')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (3, 3, N'REJECTED_sp')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (4, 0, N'CANCELLED')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (4, 1, N'CANCELLED_fr')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (4, 2, N'CANCELLED_pt')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (4, 3, N'CANCELLED_sp')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (5, 0, N'INVALID')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (5, 1, N'INVALID_fr')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (5, 2, N'INVALID_pt')
GO
INSERT [dbo].[load_card_statuses_language] ([load_card_status_id], [language_id], [language_text]) VALUES (5, 3, N'INVALID_sp')
GO
INSERT [dbo].[load_batch_statuses] ([load_batch_statuses_id], [load_batch_status_name]) VALUES (1, N'APPROVED')
GO
INSERT [dbo].[load_batch_statuses] ([load_batch_statuses_id], [load_batch_status_name]) VALUES (3, N'INVALID')
GO
INSERT [dbo].[load_batch_statuses] ([load_batch_statuses_id], [load_batch_status_name]) VALUES (0, N'LOADED')
GO
INSERT [dbo].[load_batch_statuses] ([load_batch_statuses_id], [load_batch_status_name]) VALUES (2, N'REJECTED')
GO
INSERT [dbo].[load_batch_statuses_language] ([load_batch_statuses_id], [language_id], [language_text]) VALUES (0, 0, N'LOADED')
GO
INSERT [dbo].[load_batch_statuses_language] ([load_batch_statuses_id], [language_id], [language_text]) VALUES (0, 1, N'CHARGÉ')
GO
INSERT [dbo].[load_batch_statuses_language] ([load_batch_statuses_id], [language_id], [language_text]) VALUES (0, 2, N'LOADED_pt')
GO
INSERT [dbo].[load_batch_statuses_language] ([load_batch_statuses_id], [language_id], [language_text]) VALUES (0, 3, N'LOADED_sp')
GO
INSERT [dbo].[load_batch_statuses_language] ([load_batch_statuses_id], [language_id], [language_text]) VALUES (1, 0, N'APPROVED')
GO
INSERT [dbo].[load_batch_statuses_language] ([load_batch_statuses_id], [language_id], [language_text]) VALUES (1, 1, N'APPROUVÉ')
GO
INSERT [dbo].[load_batch_statuses_language] ([load_batch_statuses_id], [language_id], [language_text]) VALUES (1, 2, N'APPROVED_pt')
GO
INSERT [dbo].[load_batch_statuses_language] ([load_batch_statuses_id], [language_id], [language_text]) VALUES (1, 3, N'APPROVED_sp')
GO
INSERT [dbo].[load_batch_statuses_language] ([load_batch_statuses_id], [language_id], [language_text]) VALUES (2, 0, N'REJECTED')
GO
INSERT [dbo].[load_batch_statuses_language] ([load_batch_statuses_id], [language_id], [language_text]) VALUES (2, 1, N'REJETÉ')
GO
INSERT [dbo].[load_batch_statuses_language] ([load_batch_statuses_id], [language_id], [language_text]) VALUES (2, 2, N'REJECTED_pt')
GO
INSERT [dbo].[load_batch_statuses_language] ([load_batch_statuses_id], [language_id], [language_text]) VALUES (2, 3, N'REJECTED_sp')
GO
INSERT [dbo].[load_batch_statuses_language] ([load_batch_statuses_id], [language_id], [language_text]) VALUES (3, 0, N'INVALID')
GO
INSERT [dbo].[load_batch_statuses_language] ([load_batch_statuses_id], [language_id], [language_text]) VALUES (3, 1, N'INVALID')
GO
INSERT [dbo].[load_batch_statuses_language] ([load_batch_statuses_id], [language_id], [language_text]) VALUES (3, 2, N'INVALID_pt')
GO
INSERT [dbo].[load_batch_statuses_language] ([load_batch_statuses_id], [language_id], [language_text]) VALUES (3, 3, N'INVALID_sp')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (18, N'BRANCH_PRODUCT_NOT_FOUND')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (19, N'CARD_FILE_INFO_READ_ERROR')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (13, N'DUPLICATE_CARDS_IN_DATABASE')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (12, N'DUPLICATE_CARDS_IN_FILE')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (3, N'DUPLICATE_FILE')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (5, N'FILE_CORRUPT')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (14, N'FILE_DECRYPTION_FAILED')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (7, N'INVALID_FORMAT')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (10, N'INVALID_ISSUER_LICENSE')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (8, N'INVALID_NAME')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (9, N'ISSUER_NOT_FOUND')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (4, N'LOAD_FAIL')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (11, N'NO_ACTIVE_BRANCH_FOUND')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (16, N'NO_PRODUCT_FOUND_FOR_CARD')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (6, N'PARTIAL_LOAD')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (2, N'PROCESSED')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (0, N'READ')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (15, N'UNLICENSED_BIN')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (17, N'UNLICENSED_ISSUER')
GO
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (1, N'VALID_CARDS')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (0, 0, N'READ')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (0, 1, N'LIRE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (0, 2, N'READ_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (0, 3, N'READ_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (1, 0, N'VALID_CARDS')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (1, 1, N'CARTES VALIDES')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (1, 2, N'VALID_CARDS_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (1, 3, N'VALID_CARDS_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (2, 0, N'PROCESSED')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (2, 1, N'TRAITE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (2, 2, N'PROCESSED_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (2, 3, N'PROCESSED_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (3, 0, N'DUPLICATE_FILE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (3, 1, N'DOSSIER DUPLIQUE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (3, 2, N'DUPLICATE_FILE_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (3, 3, N'DUPLICATE_FILE_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (4, 0, N'LOAD_FAIL')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (4, 1, N'LE CHARGEMENT A ECHOUE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (4, 2, N'LOAD_FAIL_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (4, 3, N'LOAD_FAIL_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (5, 0, N'FILE_CORRUPT')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (5, 1, N'FILE CORROMPUE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (5, 2, N'FILE_CORRUPT_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (5, 3, N'FILE_CORRUPT_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (6, 0, N'PARTIAL_LOAD')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (6, 1, N'CHARGEMENT PARTIEL')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (6, 2, N'PARTIAL_LOAD_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (6, 3, N'PARTIAL_LOAD_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (7, 0, N'INVALID_FORMAT')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (7, 1, N'FORMAT INVALIDE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (7, 2, N'INVALID_FORMAT_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (7, 3, N'INVALID_FORMAT_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (8, 0, N'INVALID_NAME')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (8, 1, N'NOM INVALIDE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (8, 2, N'INVALID_NAME_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (8, 3, N'INVALID_NAME_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (9, 0, N'ISSUER_NOT_FOUND')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (9, 1, N'Emetteur INTROUVABLE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (9, 2, N'ISSUER_NOT_FOUND_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (9, 3, N'ISSUER_NOT_FOUND_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (10, 0, N'INVALID_ISSUER_LICENSE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (10, 1, N'LICENSE DE Emetteur INVALIDE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (10, 2, N'INVALID_ISSUER_LICENSE_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (10, 3, N'INVALID_ISSUER_LICENSE_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (11, 0, N'NO_ACTIVE_BRANCH_FOUND')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (11, 1, N'AUCUNE AGENCE ACTIVE TROUVEE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (11, 2, N'NO_ACTIVE_BRANCH_FOUND_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (11, 3, N'NO_ACTIVE_BRANCH_FOUND_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (12, 0, N'DUPLICATE_CARDS_IN_FILE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (12, 1, N'CARTES DUPLIQUEES DANS LE FICHIER')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (12, 2, N'DUPLICATE_CARDS_IN_FILE_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (12, 3, N'DUPLICATE_CARDS_IN_FILE_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (13, 0, N'DUPLICATE_CARDS_IN_DATABASE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (13, 1, N'CARTES DUPLIQUEES DANS LA BASE DE DONNEES')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (13, 2, N'DUPLICATE_CARDS_IN_DATABASE_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (13, 3, N'DUPLICATE_CARDS_IN_DATABASE_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (14, 0, N'FILE_DECRYPTION_FAILED')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (14, 1, N'DECRYPTAGE DE FICHIER A ECHOUE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (14, 2, N'FILE_DECRYPTION_FAILED_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (14, 3, N'FILE_DECRYPTION_FAILED_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (15, 0, N'UNLICENSED_BIN')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (15, 1, N'BIN NON AUTORISE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (15, 2, N'UNLICENSED_BIN_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (15, 3, N'UNLICENSED_BIN_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (16, 0, N'NO_PRODUCT_FOUND_FOR_CARD')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (16, 1, N'AUCUN PRODUIT TROUVE POUR LA CARTE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (16, 2, N'NO_PRODUCT_FOUND_FOR_CARD_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (16, 3, N'NO_PRODUCT_FOUND_FOR_CARD_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (17, 0, N'UNLICENSED_ISSUER')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (17, 1, N'EMETTEUR NON AUTORISE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (17, 2, N'UNLICENSED_ISSUER_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (17, 3, N'UNLICENSED_ISSUER_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (18, 0, N'BRANCH_PRODUCT_NOT_FOUND')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (18, 1, N'PRODUIT D''AGENCE INTROUVABLE')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (18, 2, N'BRANCH_PRODUCT_NOT_FOUND_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (18, 3, N'BRANCH_PRODUCT_NOT_FOUND_sp')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (19, 0, N'CARD_FILE_INFO_READ_ERROR')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (19, 1, N'ERREUR DE  LECTURE DES INFORMATION DU FICHIER DES CARTES')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (19, 2, N'CARD_FILE_INFO_READ_ERROR_pt')
GO
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (19, 3, N'CARD_FILE_INFO_READ_ERROR_sp')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (0, N'GHS')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (1, N'USD')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (2, N'GBP')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (3, N'EUR')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (4, N'XOF')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (5, N'BIF')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (6, N'CDF')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (7, N'CVE')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (8, N'GMD')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (9, N'GNF')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (10, N'KES')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (11, N'LRD')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (12, N'MWK')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (13, N'NGN')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (14, N'RWF')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (15, N'SLL')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (16, N'SSP')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (17, N'STD')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (18, N'TZS')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (19, N'UGX')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (20, N'XAF')
GO
INSERT [dbo].[currency] ([currency_id], [currency_code]) VALUES (21, N'ZMW')
GO
INSERT [dbo].[file_types] ([file_type_id], [file_type]) VALUES (1, N'CARD_IMPORT')
GO
INSERT [dbo].[file_types] ([file_type_id], [file_type]) VALUES (0, N'PIN_MAILER')
GO
INSERT [dbo].[file_types] ([file_type_id], [file_type]) VALUES (3, N'UNKNOWN')
GO
INSERT [dbo].[Issuer_product_font] ([font_id], [font_name], [resource_path], [DeletedYN]) VALUES (1, N'Lucida Console', NULL, 0)
GO
INSERT [dbo].[Issuer_product_font] ([font_id], [font_name], [resource_path], [DeletedYN]) VALUES (2, N'Arial', NULL, 0)
GO
INSERT [dbo].[Issuer_product_font] ([font_id], [font_name], [resource_path], [DeletedYN]) VALUES (3, N'Courier New', NULL, 0)
GO
INSERT [dbo].[Issuer_product_font] ([font_id], [font_name], [resource_path], [DeletedYN]) VALUES (4, N'Times New Roman', NULL, 0)
GO
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (0, N'Indigo Instant Card Issuing System')
GO
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (1, N'Current batch status:')
GO
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (2, N'Date printed: ')
GO
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (3, N'Report generated by: ')
GO
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (4, N'Date')
GO
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (5, N'Batch Status ')
GO
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (6, N'User')
GO
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (7, N'Card Number')
GO
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (8, N'Operator: ')
GO
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (9, N'Load Batch Report')
GO
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (10, N'Distribution Batch Report')
GO
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (11, N'Card Report')
GO
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (12, N'Checked-In Cards report')
GO
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (13, N'Checked-Out Cards report')
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (1, 0)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (1, 1)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (1, 2)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (1, 3)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (1, 4)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (1, 5)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (1, 6)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (1, 7)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (1, 9)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (2, 0)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (2, 1)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (2, 2)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (2, 3)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (2, 4)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (2, 5)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (2, 6)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (2, 7)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (2, 10)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (3, 0)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (3, 2)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (3, 3)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (3, 4)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (3, 7)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (3, 8)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (3, 11)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (3, 12)
GO
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid]) VALUES (3, 13)
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (0, 0, N'Indigo Instant Card Issuing System')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (0, 1, N'Système d''émission instantanée de carte Indigo')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (0, 2, N'Indigo Instant Card Issuing System_pt')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (0, 3, N'Indigo Instant Card Issuing System_sp')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (1, 0, N'Current batch status:')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (1, 1, N'Status actuel du batch:')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (1, 2, N'Current batch status_pt:')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (1, 3, N'Current batch status_sp:')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (2, 0, N'Date printed: ')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (2, 1, N'Date d''impression:')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (2, 2, N'Date printed_pt:')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (2, 3, N'Date printed_sp:')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (3, 0, N'Report generated by: ')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (3, 1, N'Rapport généré par:')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (3, 2, N'Report generated by_pt:')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (3, 3, N'Report generated by_sp:')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (4, 0, N'Date')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (4, 1, N'Date')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (4, 2, N'Date_pt')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (4, 3, N'Date_sp')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (5, 0, N'Batch Status ')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (5, 1, N'Status du batch')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (5, 2, N'Batch Status_pt')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (5, 3, N'Batch Status_sp')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (6, 0, N'User')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (6, 1, N'Utilisateur')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (6, 2, N'User_pt')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (6, 3, N'User_sp')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (7, 0, N'Card Number')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (7, 1, N'Numéro de carte')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (7, 2, N'Card Number_pt')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (7, 3, N'Card Number_sp')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (8, 0, N'Operator: ')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (8, 1, N'Opérateur')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (8, 2, N'Operator_pt:')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (8, 3, N'Operator_sp:')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (9, 0, N'Load Batch Report')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (9, 1, N'Rapport des batch chargés')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (9, 2, N'Load Batch Report_pt')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (9, 3, N'Load Batch Report_sp')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (10, 0, N'Distribution Batch Report')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (10, 1, N'Rapport des batchs de distribution')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (10, 2, N'Distribution Batch Report_pt')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (10, 3, N'Distribution Batch Report_sp')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (11, 0, N'Card Report')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (11, 1, N'Rapport des cartes')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (11, 2, N'Card Report_pt')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (11, 3, N'Card Report_sp')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (12, 0, N'Checked-In Cards report')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (12, 1, N'Rapport d''allocation')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (12, 2, N'Checked-In Cards report_pt')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (12, 3, N'Checked-In Cards report_sp')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (13, 0, N'Checked-Out Cards report')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (13, 1, N'Rapport de desallocation')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (13, 2, N'Checked-Out Cards report_pt')
GO
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (13, 3, N'Checked-Out Cards report_sp')
GO
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (1, N'Load Batch Report')
GO
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (2, N'Distribution Batch Report')
GO
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (3, N'Card Report')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (0, 0, N'Action was successful.', N'Action reussie', N'Action was successful_pt', N'Action was successful_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (0, 16, N'Load batch has been approved.', N'Le batch chargé a été aprouvé.', N'Load batch has been approved._pt', N'Load batch has been approved._sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (0, 17, N'Load batch has been rejected.', N'Le batch chargé a été rejeté.', N'Load batch has been rejected._pt', N'Load batch has been rejected._sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (0, 25, N'Successfully Inserted Porduct.', N'Carte inserrée avec succès', N'Successfully Inserted Porduct_pt', N'Successfully Inserted Porduct_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (0, 26, N'Successfully Updated Product.', N'Carte mise á jour avec succès', N'Successfully Updated Product_pt', N'Successfully Updated Product_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (0, 30, N'Successfully Updated Card Status', N'Le status de la carte a été mis á jour avec succès', N'Successfully Updated Card_pt', N'Successfully Updated Card_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (0, 31, N'Card has been marked as PRINTED. <br />Pleace click ''Upload Card Details'' to finish issuing card."', N'Cette carte a  été  marquée imprimée. Cliquez sur ''Charger les détails de la carte'' pour achever l''émission. ', N'Card has been marked as PRINTED. <br />Pleace click ''Upload Card Details'' to finish issuing card._pt', N'Card has been marked as PRINTED. <br />Pleace click ''Upload Card Details'' to finish issuing card._sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (0, 32, N'Card has been approved for Printing.', N'La carte a  été aprouvée pour impression. ', N'Card has been approved for Printing._pt', N'Card has been approved for Printing._sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (0, 33, N'Card reserved for customer, please have the branch custodian approve the card for issue.', N'Cette carte est reservée au client, demandez l''aprobation du Superviseur agence  pour  initier l''impression.', N'Card reserved for customer, please have the branch custodian approve the card for issue._pt', N'Card reserved for customer, please have the branch custodian approve the card for issue._sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (0, 34, N'Card marked with printing error.', N'Cette carte a été  marquée erronnée dû á une erreur d''impression.', N'Card marked with printing error_pt', N'Card marked with printing error_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (0, 35, N'PIN successfully captured.', N'Code PIN validé.', N'PIN successfully captured_pt', N'PIN successfully captured_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (0, 36, N'Card has been rejected.', N'La carte a  été rejetée. ', N'Card has been rejected._pt', N'Card has been rejected.sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (1, 0, N'Action was not successful.', N'Action échouée', N'Action was not successful_pt', N'Action was not successful_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (1, 16, N'An unknown error has occured, please try again or contact support.', N'Une erreur étrange est survenue, veuillez réssayer ou contacter l''équipe de support.', N'An unknown error has occured, please try again or contact support._pt', N'An unknown error has occured, please try again or contact support._sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (1, 17, N'An unknown error has occured, please try again or contact support.', N'Une erreur étrange est survenue, veuillez réssayer ou contacter l''equipe de support.', N'An unknown error has occured, please try again or contact support._pt', N'An unknown error has occured, please try again or contact support._sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (1, 25, N'Failed to insert product.', N'Insertion De carte échouée.', N'Failed to insert product_pt', N'Failed to insert product_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (1, 26, N'Failed to Updated Product.', N'Echec de la  tentative de mise á jour du produit', N'Failed to Updated Product_pt', N'Failed to Updated Product_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (1, 30, N'Failed to Update Card.', N'Echec de la  tentative de mise á jour de la carte', N'Failed to Update Card_pt', N'Failed to Update Card_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (1, 31, N'Failed to Update Card.', N'Echec de la  tentative de mise á jour de la carte', N'Failed to Update Card_pt', N'Failed to Update Card_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (1, 32, N'Failed to Update Card.', N'Echec de la  tentative de mise á jour de la carte', N'Failed to Update Card_pt', N'Failed to Update Card_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (1, 33, N'Failed to Update Card.', N'Echec de la  tentative de mise á jour de la carte', N'Failed to Update Card_pt', N'Failed to Update Card_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (1, 34, N'Failed to Update Card.', N'Echec de la  tentative de mise á jour de la carte', N'Failed to Update Card_pt', N'Failed to Update Card_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (1, 35, N'Failed to Update card status. Unknown Error, please contact support.', N'Echec de la  tentative de mise á jour du status de la carte. Erreur étrange, Contactez l''équipe de support.', N'Failed to Update card status. Unknown Error, please contact support_pt', N'Failed to Update card status. Unknown Error, please contact support_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (1, 36, N'Failed to Update card status. Unknown Error, please contact support.', N'Echec de la  tentative de mise á jour du status de la carte. Erreur étrange, Contactez l''équipe de support.', N'Failed to Update card status. Unknown Error, please contact support_pt', N'Failed to Update card status. Unknown Error, please contact support_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (2, 0, N'Could not connect to service, it may be down. Please try again or contact support.', N'Service inaccessible! Réessayer ou contactez l''équipe de support.', N'Could not connect to service, it may be down. Please try again or contact support._pt', N'Could not connect to service, it may be down. Please try again or contact support._sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (4, 0, N'Parameter is missing or incorrect, please call support.', N'Paramètre introuvable ou incorrect. Contactez l''équipe de support.', N'Parameter is missing or incorrect, please call support._pt', N'Parameter is missing or incorrect, please call support._sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (20, 0, N'Could not connect to service, it may be down. Please try again or contact support.', N'Service inaccessible! Réessayer ou contactez l''équipe de support.', N'Could not connect to service, it may be down. Please try again or contact support.__pt', N'Could not connect to service, it may be down. Please try again or contact support.__sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (69, 0, N'Duplicate username found, please change username.', N'Le nom d''utilisateur est dupliqué. Veuillez changer le nom d''utilisateur.', N'Duplicate username found, please change username_pt', N'Duplicate username found, please change username_es')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (70, 0, N'Password should be a minimum of 8 charecters comprised of atleast 1 numeric value and 1 special charecter.', N'Le mot de passe doit contenir au minimum 8 caractères avec au moins un chiffre et un caractère spécial.', N'Password should be a minimum of 8 charecters comprised of atleast 1 numeric value and 1 special charecter_pt', N'Password should be a minimum of 8 charecters comprised of atleast 1 numeric value and 1 special charecter_es')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (71, 0, N'New password matches previous password.', N'Le nouveau mot de passe correspond á l''ancien mot de passe.', N'New password matches previous password_pt', N'New password matches previous password_es')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (100, 16, N'Load batch is not in the correct status, it may have been updated by someone else.', N'Le status du batch chargé est incorrect. Un autre utilisateur l''a peut être modifié.', N'Load batch is not in the correct status, it may have been updated by someone else._pt', N'Load batch is not in the correct status, it may have been updated by someone else._sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (100, 17, N'Load batch is not in the correct status, it may have been updated by someone else.', N'Le status du batch chargé est incorrect. Un autre utilisateur l''a peut être modifié.', N'Load batch is not in the correct status, it may have been updated by someone else._pt', N'Load batch is not in the correct status, it may have been updated by someone else._sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (100, 30, N'Card not in correct status.', N'La carte a un status incorrect.', N'Card not in correct status_pt', N'Card not in correct status_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (100, 31, N'Card not in correct status.', N'La carte a un status incorrect.', N'Card not in correct status_pt', N'Card not in correct status_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (100, 32, N'Card not in correct status.', N'La carte a un status incorrect.', N'Card not in correct status_pt', N'Card not in correct status_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (100, 33, N'Card not in correct status.', N'La carte a un status incorrect.', N'Card not in correct status_pt', N'Card not in correct status_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (100, 34, N'Card not in correct status.', N'La carte a un status incorrect.', N'Card not in correct status_pt', N'Card not in correct status_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (100, 35, N'Card not in correct status.', N'La carte a un status incorrect.', N'Card not in correct status_pt', N'Card not in correct status_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (100, 36, N'Card not in correct status.', N'La carte a un status incorrect.', N'Card not in correct status._pt', N'Card not in correct status.sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (200, 0, N'Duplicate issuer name found, please change issuer name.', N'Le nom de la filliale est dupliqué. Veuillez changer le nom de la filliale.', N'Duplicate issuer name found, please change issuer name._pt', N'Duplicate issuer name found, please change issuer name._es')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (201, 0, N'Duplicate issuer code found, please change issuer code.', N'Le Code de la filliale est dupliqué. Veuillez chnager le code de la filliale.', N'Duplicate issuer code found, please change issuer code._pt', N'Duplicate issuer code found, please change issuer code._es')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (210, 0, N'Duplicate branch name, please change name.', N'Le nom de l''agence est dupliqué. Veuillez changer le nom de l''agence.', N'Duplicate branch name, please change name_pt', N'Duplicate branch name, please change name_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (211, 0, N'Duplicate branch code, please change code.', N'Le code de l''agence est dupliqué. Veuillez chnager le code', N'Duplicate branch code, please change code_pt', N'Duplicate branch code, please change code_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (215, 0, N'Duplicate user group name, please change user group name.', N'Le nom du groupe d''utilisateurs est dupliqué. ', N'Duplicate user group name, please change user group name_pt', N'Duplicate user group name, please change user group name_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (220, 25, N'Duplicate product name, please change name.', N'Le nom du produit est dupliqué. Veuillez changer le nom du produit', N'Duplicate product name, please change name_pt', N'Duplicate product name, please change name_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (220, 26, N'Duplicate product name, please change name.', N'Le nom du produit est dupliqué. Veuillez changer le nom du produit', N'Duplicate product name, please change name_pt', N'Duplicate product name, please change name_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (221, 0, N'Duplicate branch code, please change code.', N'Le Code agence est dupliqué. Veuillez changer le code', N'Duplicate branch code, please change code_pt', N'Duplicate branch code, please change code_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (221, 25, N'Duplicate product code, please change code.', N'Le code du produit est dupliqué. Veuillez changer le code.', N'Duplicate product code, please change code_pt', N'Duplicate product code, please change code_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (221, 26, N'Duplicate product code, please change code.', N'Le code du produit est dupliqué. Veuillez changer le code.', N'Duplicate product code, please change code_pt', N'Duplicate product code, please change code_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (222, 25, N'Duplicate product bin, please change bin.', N'BIN du produit est dupliqué. Veuillez changer le BIN', N'Duplicate product bin, please change bin_pt', N'Duplicate product bin, please change bin_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (222, 26, N'Duplicate product bin, please change bin.', N'BIN du produit est dupliqué. Veuillez changer le BIN', N'Duplicate product bin, please change bin_pt', N'Duplicate product bin, please change bin_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (225, 0, N'Duplicate LDAP Setting name, please change the name.', N'Nom du réglage LDAP est dupliqué. Veuillez changer le nom.', N'Duplicate LDAP Setting name, please change the name_pt', N'Duplicate LDAP Setting name, please change the name_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (500, 0, N'Card successfully linked to Customers Account.', N'La carte a été reliée au compte du client avec succès.', N'Card successfully linked to Customers Account_pt', N'Card successfully linked to Customers Account_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (501, 0, N'An error has occured while linked card, please contact support.', N'Une erreur est survenue lors de la liaison de la carte. Contactez  l''équipe de support.', N'An error has occured while linked card, please contact support_pt', N'An error has occured while linked card, please contact support_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (502, 0, N'Parameters for CMS are missing or incorrect, please contact support.', N'Les paramètres CMS introuvables ou incorrects. Contactez  l''équipe de support.', N'Parameters for CMS are missing or incorrect, please contact support_pt', N'Parameters for CMS are missing or incorrect, please contact support_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (503, 0, N'Failed on New Customer and aggreement.', N'Echec d''émission pour le type Nouveau client et Accord', N'Failed on New Customer and aggreement.pt', N'Failed on New Customer and aggreement.sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (504, 0, N'Failed on existing Customer new aggreement.', N'Echec d''émission pour le type Client existant et Accord', N'Failed on existing Customer new aggreement._pt', N'Failed on existing Customer new aggreement._sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (505, 0, N'Failed on ReLink.', N'Echec  lors de la liason á Relink', N'Failed on ReLink._pt', N'Failed on ReLink._sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (506, 0, N'Failed on EditCard.', N'Echec lors de la liason á EditCard', N'Failed on EditCard._pt', N'Failed on EditCard._sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (600, 0, N'Issuer does not have a valid licence.', N'La filliale n''a pas de licence valide.', N'Issuer does not have a valid licence_pt', N'Issuer does not have a valid licence_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (601, 0, N'There was a problem processing the issuers licence, it may be invalid.', N'Une erreur est survenue lors du traitement de la licence de la filliale. Il se peut qu''elle soit invalide.', N'There was a problem processing the issuers licence, it may be invalid_pt', N'There was a problem processing the issuers licence, it may be invalid_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (602, 0, N'Issuer licence has expired.', N'Licence de la filliale a expiré.', N'Issuer licence has expired_pt', N'Issuer licence has expired_sp')
GO
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (603, 0, N'An error has occured while processing the issuers licence, please contact support.', N'Une erreur est survenue lors du traitement de la license de la filliale. Contactez l''équipe de support.', N'An error has occured while processing the issuers licence, please contact support_pt', N'An error has occured while processing the issuers licence, please contact support_sp')
GO
SET IDENTITY_INSERT [dbo].[user_gender] ON 

GO
INSERT [dbo].[user_gender] ([user_gender_id], [user_gender_text]) VALUES (2, N'FEMALE')
GO
INSERT [dbo].[user_gender] ([user_gender_id], [user_gender_text]) VALUES (1, N'MALE')
GO
INSERT [dbo].[user_gender] ([user_gender_id], [user_gender_text]) VALUES (3, N'UNSPECIFIED')
GO
SET IDENTITY_INSERT [dbo].[user_gender] OFF
GO

  INSERT INTO [response_messages]
  ([system_response_code]
  ,[system_area]
  ,[english_response]
  ,[french_response]
  ,[portuguese_response]
  ,[spanish_response] )
  VALUES (507
	, 0
	, 'The customer has insuffiecient funds for card fees.'
	, 'Le client dispose de fonds insuffiecient pour les frais de carte.'
	, 'The customer has insuffiecient funds for card fees.'
	, 'The customer has insuffiecient funds for card fees.')
GO

  INSERT INTO [response_messages]
  ([system_response_code]
  ,[system_area]
  ,[english_response]
  ,[french_response]
  ,[portuguese_response]
  ,[spanish_response] )
  VALUES (72
	, 0
	, 'The Authorisation Pin captured is incorrect. Please try again.'
	, 'L''autorisation Pin capturé est incorrect . Se il vous plaît essayer à nouveau.'
	, 'The Authorisation Pin captured is incorrect. Please try again._pt'
	, 'The Authorisation Pin captured is incorrect. Please try again._esp')
GO