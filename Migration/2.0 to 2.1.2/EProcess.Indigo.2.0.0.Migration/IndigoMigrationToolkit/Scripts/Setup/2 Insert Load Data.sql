

INSERT [dbo].[languages] ([id], [language_name], [language_name_fr], [language_name_pt], [language_name_sp]) VALUES (0, N'English', N'Anglais', N'English_pt', N'English_sp')
INSERT [dbo].[languages] ([id], [language_name], [language_name_fr], [language_name_pt], [language_name_sp]) VALUES (1, N'French', N'Français', N'French_pt', N'French_sp')
INSERT [dbo].[languages] ([id], [language_name], [language_name_fr], [language_name_pt], [language_name_sp]) VALUES (2, N'Portuguese', N'Portugais', N'Portuguese_pt', N'Portuguese_sp')
INSERT [dbo].[languages] ([id], [language_name], [language_name_fr], [language_name_pt], [language_name_sp]) VALUES (3, N'Spanish', N'Espagnol', N'Spanish_pt', N'Spanish_sp')
GO	


INSERT [dbo].[customer_residency] ([resident_id], [residency_name]) VALUES (0, N'RESIDENT')
INSERT [dbo].[customer_residency] ([resident_id], [residency_name]) VALUES (1, N'NONRESIDENT')
GO	


INSERT [dbo].[customer_residency_language] ([resident_id], [language_id], [language_text]) VALUES (0, 0, N'RESIDENT')
INSERT [dbo].[customer_residency_language] ([resident_id], [language_id], [language_text]) VALUES (0, 1, N'Résident')
INSERT [dbo].[customer_residency_language] ([resident_id], [language_id], [language_text]) VALUES (0, 2, N'RESIDENT_pt')
INSERT [dbo].[customer_residency_language] ([resident_id], [language_id], [language_text]) VALUES (0, 3, N'RESIDENT_sp')
INSERT [dbo].[customer_residency_language] ([resident_id], [language_id], [language_text]) VALUES (1, 0, N'NONRESIDENT')
INSERT [dbo].[customer_residency_language] ([resident_id], [language_id], [language_text]) VALUES (1, 1, N'Non résident')
INSERT [dbo].[customer_residency_language] ([resident_id], [language_id], [language_text]) VALUES (1, 2, N'NONRESIDENT_pt')
INSERT [dbo].[customer_residency_language] ([resident_id], [language_id], [language_text]) VALUES (1, 3, N'NONRESIDENT_sp')
GO	

INSERT [dbo].[customer_title] ([customer_title_id], [customer_title_name]) VALUES (0, N'MR')
INSERT [dbo].[customer_title] ([customer_title_id], [customer_title_name]) VALUES (1, N'MRS')
INSERT [dbo].[customer_title] ([customer_title_id], [customer_title_name]) VALUES (2, N'MISS')
INSERT [dbo].[customer_title] ([customer_title_id], [customer_title_name]) VALUES (3, N'MS')
INSERT [dbo].[customer_title] ([customer_title_id], [customer_title_name]) VALUES (4, N'PROF')
INSERT [dbo].[customer_title] ([customer_title_id], [customer_title_name]) VALUES (5, N'DR')
INSERT [dbo].[customer_title] ([customer_title_id], [customer_title_name]) VALUES (6, N'REV')
INSERT [dbo].[customer_title] ([customer_title_id], [customer_title_name]) VALUES (7, N'OTHER')
GO	

INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (0, 0, N'MR')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (0, 1, N'Mr.')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (0, 2, N'MR_pt')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (0, 3, N'MR_sp')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (1, 0, N'MRS')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (1, 1, N'Me.')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (1, 2, N'MRS_pt')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (1, 3, N'MRS_sp')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (2, 0, N'MISS')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (2, 1, N'Mlle.')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (2, 2, N'MISS_pt')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (2, 3, N'MISS_sp')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (3, 0, N'MS')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (3, 1, N'Me.')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (3, 2, N'MS_pt')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (3, 3, N'MS_sp')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (4, 0, N'PROF')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (4, 1, N'Prof.')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (4, 2, N'PROF_pt')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (4, 3, N'PROF_sp')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (5, 0, N'DR')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (5, 1, N'Dr.')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (5, 2, N'DR_pt')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (5, 3, N'DR_sp')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (6, 0, N'REV')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (6, 1, N'Rev.')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (6, 2, N'REV_pt')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (6, 3, N'REV_sp')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (7, 0, N'OTHER')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (7, 1, N'Autres')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (7, 2, N'OTHER_pt')
INSERT [dbo].[customer_title_language] ([customer_title_id], [language_id], [language_text]) VALUES (7, 3, N'OTHER_sp')
GO	

INSERT [dbo].[pin_reissue_statuses] ([pin_reissue_statuses_id], [pin_reissue_statuses_name]) VALUES (0, N'REQUESTED')
INSERT [dbo].[pin_reissue_statuses] ([pin_reissue_statuses_id], [pin_reissue_statuses_name]) VALUES (1, N'APPROVED')
INSERT [dbo].[pin_reissue_statuses] ([pin_reissue_statuses_id], [pin_reissue_statuses_name]) VALUES (2, N'REJECTED')
INSERT [dbo].[pin_reissue_statuses] ([pin_reissue_statuses_id], [pin_reissue_statuses_name]) VALUES (3, N'UPLOADED')
INSERT [dbo].[pin_reissue_statuses] ([pin_reissue_statuses_id], [pin_reissue_statuses_name]) VALUES (4, N'EXPIRED')
GO	

INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (0, 0, N'REQUESTED')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (0, 1, N'REQUESTED_fr')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (0, 2, N'REQUESTED_pt')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (0, 3, N'REQUESTED_es')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (1, 0, N'APPROVED')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (1, 1, N'APPROVED_fr')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (1, 2, N'APPROVED_pt')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (1, 3, N'APPROVED_es')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (2, 0, N'REJECTED')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (2, 1, N'REJECTED_fr')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (2, 2, N'REJECTED_pt')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (2, 3, N'REJECTED_es')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (3, 0, N'UPLOADED')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (3, 1, N'UPLOADED_fr')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (3, 2, N'UPLOADED_pt')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (3, 3, N'UPLOADED_es')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (4, 0, N'EXPIRED')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (4, 1, N'EXPIRED_fr')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (4, 2, N'EXPIRED_pt')
INSERT [dbo].[pin_reissue_statuses_language] ([pin_reissue_statuses_id], [language_id], [language_text]) VALUES (4, 3, N'EXPIRED_es')
GO	

INSERT [dbo].[customer_type] ([customer_type_id], [customer_type_name]) VALUES (0, N'PRIVATE')
INSERT [dbo].[customer_type] ([customer_type_id], [customer_type_name]) VALUES (1, N'CORPORATE')
GO	


INSERT [dbo].[customer_type_language] ([customer_type_id], [language_id], [language_text]) VALUES (0, 0, N'PRIVATE')
INSERT [dbo].[customer_type_language] ([customer_type_id], [language_id], [language_text]) VALUES (0, 1, N'Privé')
INSERT [dbo].[customer_type_language] ([customer_type_id], [language_id], [language_text]) VALUES (0, 2, N'PRIVATE_pt')
INSERT [dbo].[customer_type_language] ([customer_type_id], [language_id], [language_text]) VALUES (0, 3, N'PRIVATE_sp')
INSERT [dbo].[customer_type_language] ([customer_type_id], [language_id], [language_text]) VALUES (1, 0, N'CORPORATE')
INSERT [dbo].[customer_type_language] ([customer_type_id], [language_id], [language_text]) VALUES (1, 1, N'Entreprise')
INSERT [dbo].[customer_type_language] ([customer_type_id], [language_id], [language_text]) VALUES (1, 2, N'CORPORATE_pt')
INSERT [dbo].[customer_type_language] ([customer_type_id], [language_id], [language_text]) VALUES (1, 3, N'CORPORATE_sp')
GO	


INSERT [dbo].[connection_parameter_type] ([connection_parameter_type_id], [connection_parameter_type_name]) VALUES (0, N'WEBSERVICE')
INSERT [dbo].[connection_parameter_type] ([connection_parameter_type_id], [connection_parameter_type_name]) VALUES (1, N'FILE_SYSTEM')
INSERT [dbo].[connection_parameter_type] ([connection_parameter_type_id], [connection_parameter_type_name]) VALUES (2, N'THALESHSM')
GO	


INSERT [dbo].[connection_parameter_type_language] ([connection_parameter_type_id], [language_id], [language_text]) VALUES (0, 0, N'WEBSERVICE')
INSERT [dbo].[connection_parameter_type_language] ([connection_parameter_type_id], [language_id], [language_text]) VALUES (0, 1, N'WEBSERVICE_fr')
INSERT [dbo].[connection_parameter_type_language] ([connection_parameter_type_id], [language_id], [language_text]) VALUES (0, 2, N'WEBSERVICE_pt')
INSERT [dbo].[connection_parameter_type_language] ([connection_parameter_type_id], [language_id], [language_text]) VALUES (0, 3, N'WEBSERVICE_es')
INSERT [dbo].[connection_parameter_type_language] ([connection_parameter_type_id], [language_id], [language_text]) VALUES (1, 0, N'FILE_SYSTEM')
INSERT [dbo].[connection_parameter_type_language] ([connection_parameter_type_id], [language_id], [language_text]) VALUES (1, 1, N'FILE_SYSTEM_fr')
INSERT [dbo].[connection_parameter_type_language] ([connection_parameter_type_id], [language_id], [language_text]) VALUES (1, 2, N'FILE_SYSTEM_pt')
INSERT [dbo].[connection_parameter_type_language] ([connection_parameter_type_id], [language_id], [language_text]) VALUES (1, 3, N'FILE_SYSTEM_es')
INSERT [dbo].[connection_parameter_type_language] ([connection_parameter_type_id], [language_id], [language_text]) VALUES (2, 0, N'THALESHSM')
INSERT [dbo].[connection_parameter_type_language] ([connection_parameter_type_id], [language_id], [language_text]) VALUES (2, 1, N'THALESHSM_fr')
INSERT [dbo].[connection_parameter_type_language] ([connection_parameter_type_id], [language_id], [language_text]) VALUES (2, 2, N'THALESHSM_pt')
INSERT [dbo].[connection_parameter_type_language] ([connection_parameter_type_id], [language_id], [language_text]) VALUES (2, 3, N'THALESHSM_es')
GO	


INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (0, N'CREATED', NULL)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (1, N'APPROVED', 0)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (2, N'DISPATCHED', 1)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (3, N'RECEIVED_AT_BRANCH', 2)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (4, N'REJECTED_AT_BRANCH', 2)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (5, N'REJECT_AND_REISSUE', 0)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (6, N'REJECT_AND_CANCEL', 0)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (7, N'INVALID', NULL)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (8, N'REJECTED', NULL)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (9, N'APPROVED_FOR_PRODUCTION', NULL)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (10, N'SENT_TO_CMS', NULL)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (11, N'PROCESSED_IN_CMS', NULL)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (12, N'AT_CARD_PRODUCTION', NULL)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (13, N'CARDS_PRODUCED', NULL)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (14, N'RECEIVED_AT_CARD_CENTER', NULL)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (15, N'FAILED_IN_CMS', NULL)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (16, N'REJECTED_AT_CARD_CENTER', NULL)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (17, N'SENT_TO_PRINTER', NULL)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (18, N'PIN_PRINTED', NULL)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (19, N'DISPATCHED_TO_CC', NULL)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (20, N'LOAD_PENDING', NULL)
INSERT [dbo].[dist_batch_statuses] ([dist_batch_statuses_id], [dist_batch_status_name], [dist_batch_expected_statuses_id]) VALUES (21, N'LOAD_COMPLETE', NULL)
GO	


INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (0, 0, N'CREATED')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (0, 1, N'CRÉÉ')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (0, 2, N'CREATED_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (0, 3, N'CREATED_sp')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (1, 0, N'APPROVED')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (1, 1, N'APPROUVÉ')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (1, 2, N'APPROVED_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (1, 3, N'APPROVED_sp')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (2, 0, N'DISPATCHED')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (2, 1, N'DISTRIBUÉ')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (2, 2, N'DISPATCHED_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (2, 3, N'DISPATCHED_sp')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (3, 0, N'RECEIVED_AT_BRANCH')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (3, 1, N'REÇU EN AGENCE')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (3, 2, N'RECEIVED_AT_BRANCH_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (3, 3, N'RECEIVED_AT_BRANCH_sp')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (4, 0, N'REJECTED_AT_BRANCH')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (4, 1, N'REJETÉ EN AGENCE')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (4, 2, N'REJECTED_AT_BRANCH_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (4, 3, N'REJECTED_AT_BRANCH_sp')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (5, 0, N'REJECT_AND_REISSUE')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (5, 1, N'REJET ET REEMISSION')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (5, 2, N'REJECT_AND_REISSUE_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (5, 3, N'REJECT_AND_REISSUE_sp')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (6, 0, N'REJECT_AND_CANCEL')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (6, 1, N'REJET ET ANNULATION')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (6, 2, N'REJECT_AND_CANCEL_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (6, 3, N'REJECT_AND_CANCEL_sp')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (7, 0, N'INVALID')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (7, 1, N'INVALID')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (7, 2, N'INVALID_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (7, 3, N'INVALID_sp')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (8, 0, N'REJECTED')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (8, 1, N'REJETÉ')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (8, 2, N'REJECTED_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (8, 3, N'REJECTED_sp')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (9, 0, N'APPROVED_FOR_PRODUCTION')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (9, 1, N'APPROVED_FOR_PRODUCTION_fr')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (9, 2, N'APPROVED_FOR_PRODUCTION_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (9, 3, N'APPROVED_FOR_PRODUCTION_es')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (10, 0, N'SENT_TO_CMS')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (10, 1, N'SENT_TO_CMS_fr')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (10, 2, N'SENT_TO_CMS_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (10, 3, N'SENT_TO_CMS_es')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (11, 0, N'PROCESSED_IN_CMS')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (11, 1, N'PROCESSED_IN_CMS_fr')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (11, 2, N'PROCESSED_IN_CMS_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (11, 3, N'PROCESSED_IN_CMS_es')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (12, 0, N'AT_CARD_PRODUCTION')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (12, 1, N'AT_CARD_PRODUCTION_fr')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (12, 2, N'AT_CARD_PRODUCTION_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (12, 3, N'AT_CARD_PRODUCTION_es')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (13, 0, N'CARDS_PRODUCED')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (13, 1, N'CARDS_PRODUCED_fr')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (13, 2, N'CARDS_PRODUCED_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (13, 3, N'CARDS_PRODUCED_es')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (14, 0, N'RECEIVED_AT_CARD_CENTER')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (14, 1, N'RECEIVED_AT_CARD_CENTER_fr')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (14, 2, N'RECEIVED_AT_CARD_CENTER_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (14, 3, N'RECEIVED_AT_CARD_CENTER_es')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (15, 0, N'FAILED_IN_CMS')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (15, 1, N'FAILED_IN_CMS_fr')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (15, 2, N'FAILED_IN_CMS_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (15, 3, N'FAILED_IN_CMS_es')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (16, 0, N'REJECTED_AT_CARD_CENTER')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (16, 1, N'REJECTED_AT_CARD_CENTER_fr')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (16, 2, N'REJECTED_AT_CARD_CENTER_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (16, 3, N'REJECTED_AT_CARD_CENTER_es')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (17, 0, N'SENT_TO_PRINTER')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (17, 1, N'SENT_TO_PRINTER_fr')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (17, 2, N'SENT_TO_PRINTER_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (17, 3, N'SENT_TO_PRINTER_es')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (18, 0, N'PIN_PRINTED')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (18, 1, N'PIN_PRINTED_fr')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (18, 2, N'PIN_PRINTED_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (18, 3, N'PIN_PRINTED_es')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (19, 0, N'DISPATCHED_TO_CC')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (19, 1, N'DISPATCHED_TO_CC_fr')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (19, 2, N'DISPATCHED_TO_CC_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (19, 3, N'DISPATCHED_TO_CC_es')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (20, 0, N'LOAD_PENDING')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (20, 1, N'LOAD_PENDING_fr')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (20, 2, N'LOAD_PENDING_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (20, 3, N'LOAD_PENDING_es')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (21, 0, N'LOAD_COMPLETE')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (21, 1, N'LOAD_COMPLETE_fr')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (21, 2, N'LOAD_COMPLETE_pt')
INSERT [dbo].[dist_batch_statuses_language] ([dist_batch_statuses_id], [language_id], [language_text]) VALUES (21, 3, N'LOAD_COMPLETE_es')
GO	

INSERT [dbo].[user_status] ([user_status_id], [user_status_text]) VALUES (0, N'ACTIVE')
INSERT [dbo].[user_status] ([user_status_id], [user_status_text]) VALUES (1, N'INACTIVE')
INSERT [dbo].[user_status] ([user_status_id], [user_status_text]) VALUES (2, N'DELETED')
INSERT [dbo].[user_status] ([user_status_id], [user_status_text]) VALUES (3, N'ACCOUNT_LOCKED')
GO	


INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (0, 0, N'ACTIVE')
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (0, 1, N'ACTIVE')
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (0, 2, N'ACTIVE_pt')
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (0, 3, N'ACTIVE_sp')
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (1, 0, N'INACTIVE')
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (1, 1, N'INACTIF')
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (1, 2, N'INACTIVE_pt')
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (1, 3, N'INACTIVE_sp')
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (2, 0, N'DELETED')
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (2, 1, N'SUPPRIME')
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (2, 2, N'DELETED_pt')
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (2, 3, N'DELETED_sp')
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (3, 0, N'ACCOUNT_LOCKED')
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (3, 1, N'COMPTE BLOQUE')
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (3, 2, N'ACCOUNT_LOCKED_pt')
INSERT [dbo].[user_status_language] ([user_status_id], [language_id], [language_text]) VALUES (3, 3, N'ACCOUNT_LOCKED_sp')
GO	


INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (0, N'ALLOCATED_TO_BRANCH')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (1, N'AVAILABLE_FOR_ISSUE')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (2, N'RECEIVED_AT_BRANCH')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (3, N'ALLOCATED_TO_CUST')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (4, N'CARD_PRINTED')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (5, N'PIN_CAPTURED')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (6, N'ISSUED')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (7, N'REJECTED')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (8, N'CANCELLED')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (9, N'INVALID')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (10, N'LINKED_TO_ACCOUNT')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (11, N'SPOILED')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (12, N'CREATED')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (13, N'PAN_GENERATED')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (14, N'SECURITY_GENERATED')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (15, N'PIN_MAILER_PRINTED')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (16, N'CARD_PRODUCED')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (17, N'PIN_PRINTED')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (18, N'RECEIVED_AT_CARD_CENTRE')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (19, N'ALLOCATED_TO_CARD_CENTRE')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (20, N'LOAD_PENDING')
INSERT [dbo].[dist_card_statuses] ([dist_card_status_id], [dist_card_status_name]) VALUES (21, N'LOAD_COMPLETE')
GO	

	
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (0, 0, N'ALLOCATED_TO_BRANCH')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (0, 1, N'ALLOCATED_TO_BRANCH_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (0, 2, N'ALLOCATED_TO_BRANCH_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (0, 3, N'ALLOCATED_TO_BRANCH_sp')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (1, 0, N'AVAILABLE_FOR_ISSUE')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (1, 1, N'AVAILABLE_FOR_ISSUE_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (1, 2, N'AVAILABLE_FOR_ISSUE_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (1, 3, N'AVAILABLE_FOR_ISSUE_sp')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (2, 0, N'RECEIVED_AT_BRANCH')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (2, 1, N'RECEIVED_AT_BRANCH_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (2, 2, N'RECEIVED_AT_BRANCH_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (2, 3, N'RECEIVED_AT_BRANCH_sp')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (3, 0, N'ALLOCATED_TO_CUST')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (3, 1, N'ALLOCATED_TO_CUST_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (3, 2, N'ALLOCATED_TO_CUST_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (3, 3, N'ALLOCATED_TO_CUST_sp')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (4, 0, N'CARD_PRINTED')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (4, 1, N'CARD_PRINTED_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (4, 2, N'CARD_PRINTED_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (4, 3, N'CARD_PRINTED_sp')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (5, 0, N'PIN_CAPTURED')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (5, 1, N'PIN_CAPTURED_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (5, 2, N'PIN_CAPTURED_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (5, 3, N'PIN_CAPTURED_sp')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (6, 0, N'ISSUED')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (6, 1, N'ISSUED_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (6, 2, N'ISSUED_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (6, 3, N'ISSUED_sp')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (7, 0, N'REJECTED')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (7, 1, N'REJECTED_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (7, 2, N'REJECTED_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (7, 3, N'REJECTED_sp')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (8, 0, N'CANCELLED')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (8, 1, N'CANCELLED_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (8, 2, N'CANCELLED_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (8, 3, N'CANCELLED_sp')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (9, 0, N'INVALID')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (9, 1, N'INVALID_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (9, 2, N'INVALID_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (9, 3, N'INVALID_sp')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (10, 0, N'LINKED_TO_ACCOUNT')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (10, 1, N'LINKED_TO_ACCOUNT_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (10, 2, N'LINKED_TO_ACCOUNT_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (10, 3, N'LINKED_TO_ACCOUNT_sp')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (11, 0, N'SPOILED')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (11, 1, N'SPOILED_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (11, 2, N'SPOILED_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (11, 3, N'SPOILED_sp')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (12, 0, N'CREATED')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (12, 1, N'CREATED_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (12, 2, N'CREATED_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (12, 3, N'CREATEDes')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (13, 0, N'PAN_GENERATED')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (13, 1, N'PAN_GENERATED_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (13, 2, N'PAN_GENERATED_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (13, 3, N'PAN_GENERATEDes')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (14, 0, N'SECURITY_GENERATED')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (14, 1, N'SECURITY_GENERATED_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (14, 2, N'SECURITY_GENERATED_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (14, 3, N'SECURITY_GENERATEDes')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (15, 0, N'PIN_MAILER_PRINTED')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (15, 1, N'PIN_MAILER_PRINTED_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (15, 2, N'PIN_MAILER_PRINTED_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (15, 3, N'PIN_MAILER_PRINTEDes')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (16, 0, N'CARD_PRODUCED')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (16, 1, N'CARD_PRODUCED_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (16, 2, N'CARD_PRODUCED_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (16, 3, N'CARD_PRODUCEDes')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (17, 0, N'PIN_PRINTED')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (17, 1, N'PIN_PRINTED_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (17, 2, N'PIN_PRINTED_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (17, 3, N'PIN_PRINTED_es')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (18, 0, N'RECEIVED_AT_CARD_CENTRE')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (18, 1, N'RECEIVED_AT_CARD_CENTRE_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (18, 2, N'RECEIVED_AT_CARD_CENTRE_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (18, 3, N'RECEIVED_AT_CARD_CENTRE_es')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (19, 0, N'ALLOCATED_TO_CARD_CENTRE')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (19, 1, N'ALLOCATED_TO_CARD_CENTRE_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (19, 2, N'ALLOCATED_TO_CARD_CENTRE_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (19, 3, N'ALLOCATED_TO_CARD_CENTRE_es')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (20, 0, N'LOAD_PENDING')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (20, 1, N'LOAD_PENDING_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (20, 2, N'LOAD_PENDING_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (20, 3, N'LOAD_PENDING_es')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (21, 0, N'LOAD_COMPLETE')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (21, 1, N'LOAD_COMPLETE_fr')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (21, 2, N'LOAD_COMPLETE_pt')
INSERT [dbo].[dist_card_statuses_language] ([dist_card_status_id], [language_id], [language_text]) VALUES (21, 3, N'LOAD_COMPLETE_es')
GO	


INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (0, N'READ')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (1, N'VALID_CARDS')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (2, N'PROCESSED')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (3, N'DUPLICATE_FILE')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (4, N'LOAD_FAIL')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (5, N'FILE_CORRUPT')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (6, N'PARTIAL_LOAD')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (7, N'INVALID_FORMAT')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (8, N'INVALID_NAME')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (9, N'ISSUER_NOT_FOUND')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (10, N'INVALID_ISSUER_LICENSE')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (11, N'NO_ACTIVE_BRANCH_FOUND')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (12, N'DUPLICATE_CARDS_IN_FILE')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (13, N'DUPLICATE_CARDS_IN_DATABASE')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (14, N'FILE_DECRYPTION_FAILED')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (15, N'UNLICENSED_BIN')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (16, N'NO_PRODUCT_FOUND_FOR_CARD')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (17, N'UNLICENSED_ISSUER')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (18, N'BRANCH_PRODUCT_NOT_FOUND')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (19, N'CARD_FILE_INFO_READ_ERROR')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (20, N'CARDS_NOT_ORDERED')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (21, N'ORDERED_CARD_REF_MISSING')
GO		
INSERT [dbo].[file_statuses] ([file_status_id], [file_status]) VALUES (22, N'ORDERED_CARD_PRODUCT_MISS_MATCH')
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
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (20, 0, N'CARDS_NOT_ORDERED')
GO		
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (20, 1, N'CARDS_NOT_ORDERED_fr')
GO		
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (20, 2, N'CARDS_NOT_ORDERED_pt')
GO		
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (20, 3, N'CARDS_NOT_ORDERED_es')
GO		
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (21, 0, N'ORDERED_CARD_REF_MISSING')
GO		
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (21, 1, N'ORDERED_CARD_REF_MISSING_fr')
GO		
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (21, 2, N'ORDERED_CARD_REF_MISSING_pt')
GO		
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (21, 3, N'ORDERED_CARD_REF_MISSING_es')
GO		
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (22, 0, N'ORDERED_CARD_PRODUCT_MISS_MATCH')
GO		
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (22, 1, N'ORDERED_CARD_PRODUCT_MISS_MATCH_fr')
GO		
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (22, 2, N'ORDERED_CARD_PRODUCT_MISS_MATCH_pt')
GO		
INSERT [dbo].[file_statuses_language] ([file_status_id], [language_id], [language_text]) VALUES (22, 3, N'ORDERED_CARD_PRODUCT_MISS_MATCH_es')
GO		
INSERT [dbo].[export_batch_statuses] ([export_batch_statuses_id], [export_batch_statuses_name]) VALUES (0, N'CREATED')
GO		
INSERT [dbo].[export_batch_statuses] ([export_batch_statuses_id], [export_batch_statuses_name]) VALUES (1, N'APPROVED')
GO		
INSERT [dbo].[export_batch_statuses] ([export_batch_statuses_id], [export_batch_statuses_name]) VALUES (2, N'EXPORTED')
GO		
INSERT [dbo].[export_batch_statuses] ([export_batch_statuses_id], [export_batch_statuses_name]) VALUES (3, N'REQUEST_EXPORT')
GO		
INSERT [dbo].[export_batch_statuses] ([export_batch_statuses_id], [export_batch_statuses_name]) VALUES (4, N'REJECTED')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (0, 0, N'CREATED')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (0, 1, N'CREATED_fr')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (0, 2, N'CREATED_pt')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (0, 3, N'CREATED_es')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (1, 0, N'APPROVED')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (1, 1, N'APPROVED_fr')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (1, 2, N'APPROVED_pt')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (1, 3, N'APPROVED_es')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (2, 0, N'EXPORTED')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (2, 1, N'EXPORTED_fr')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (2, 2, N'EXPORTED_pt')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (2, 3, N'EXPORTED_es')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (3, 0, N'REQUEST_EXPORT')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (3, 1, N'REQUEST_EXPORT_fr')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (3, 2, N'REQUEST_EXPORT_pt')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (3, 3, N'REQUEST_EXPORT_es')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (4, 0, N'REJECTED')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (4, 1, N'REJECTED_fr')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (4, 2, N'REJECTED_pt')
GO		
INSERT [dbo].[export_batch_statuses_language] ([export_batch_statuses_id], [language_id], [language_text]) VALUES (4, 3, N'REJECTED_es')
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
INSERT [dbo].[audit_action] ([audit_action_id], [audit_action_name]) VALUES (9, N'Administration')
GO		
INSERT [dbo].[audit_action] ([audit_action_id], [audit_action_name]) VALUES (10, N'PinReissue')
GO		
INSERT [dbo].[audit_action] ([audit_action_id], [audit_action_name]) VALUES (11, N'ExportBatch')
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
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (9, 0, N'Administration')
GO		
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (9, 1, N'Administration_fr')
GO		
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (9, 2, N'Administration_pt')
GO		
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (9, 3, N'Administration_es')
GO		
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (10, 0, N'PinReissue')
GO		
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (10, 1, N'PinReissue_fr')
GO		
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (10, 2, N'PinReissue_pt')
GO		
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (10, 3, N'PinReissue_es')
GO		
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (11, 0, N'ExportBatch')
GO		
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (11, 1, N'ExportBatch_fr')
GO		
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (11, 2, N'ExportBatch_pt')
GO		
INSERT [dbo].[audit_action_language] ([audit_action_id], [language_id], [language_text]) VALUES (11, 3, N'ExportBatch_es')
GO		
INSERT [dbo].[interface_type] ([interface_type_id], [interface_type_name]) VALUES (0, N'CBS')
GO		
INSERT [dbo].[interface_type] ([interface_type_id], [interface_type_name]) VALUES (1, N'CMS')
GO		
INSERT [dbo].[interface_type] ([interface_type_id], [interface_type_name]) VALUES (2, N'HSM')
GO		
INSERT [dbo].[interface_type] ([interface_type_id], [interface_type_name]) VALUES (3, N'CARD_PRODUCTION')
GO		
INSERT [dbo].[interface_type] ([interface_type_id], [interface_type_name]) VALUES (4, N'FILE_LOADER')
GO		
INSERT [dbo].[interface_type] ([interface_type_id], [interface_type_name]) VALUES (5, N'FEE_SCHEME')
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
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (2, 0, N'HSM')
GO		
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (2, 1, N'HSM_fr')
GO		
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (2, 2, N'HSM_pt')
GO		
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (2, 3, N'HSM_sp')
GO		
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (3, 0, N'CARD_PRODUCTION')
GO		
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (3, 1, N'CARD_PRODUCTION_fr')
GO		
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (3, 2, N'CARD_PRODUCTION_pt')
GO		
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (3, 3, N'CARD_PRODUCTION_sp')
GO		
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (4, 0, N'FILE_LOADER')
GO		
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (4, 1, N'FILE_LOADER_fr')
GO		
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (4, 2, N'FILE_LOADER_pt')
GO		
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (4, 3, N'FILE_LOADER_sp')
GO		
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (5, 0, N'FEE_SCHEME')
GO		
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (5, 1, N'FEE_SCHEME_fr')
GO		
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (5, 2, N'FEE_SCHEME_pt')
GO		
INSERT [dbo].[interface_type_language] ([interface_type_id], [language_id], [language_text]) VALUES (5, 3, N'FEE_SCHEME_sp')
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
INSERT [dbo].[branch_card_statuses] ([branch_card_statuses_id], [branch_card_statuses_name]) VALUES (10, N'REQUESTED')
GO	
INSERT [dbo].[branch_card_statuses] ([branch_card_statuses_id], [branch_card_statuses_name]) VALUES (11, N'MAKERCHECKER_REJECT')
GO	
INSERT [dbo].[branch_card_statuses] ([branch_card_statuses_id], [branch_card_statuses_name]) VALUES (12, N'CARD_REQUEST_DELETED')
GO	
INSERT [dbo].[branch_card_statuses] ([branch_card_statuses_id], [branch_card_statuses_name]) VALUES (13, N'REDISTRIBUTED')
GO	
INSERT [dbo].[branch_card_statuses] ([branch_card_statuses_id], [branch_card_statuses_name]) VALUES (14, N'PIN_AUTHORISED')
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
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (10, 0, N'REQUESTED')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (10, 1, N'REQUESTED_fr')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (10, 2, N'REQUESTED_pt')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (10, 3, N'REQUESTED_es')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (11, 0, N'MAKERCHECKER_REJECT')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (11, 1, N'MAKERCHECKER_REJECT_fr')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (11, 2, N'MAKERCHECKER_REJECT_pt')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (11, 3, N'MAKERCHECKER_REJECT_es')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (12, 0, N'CARD_REQUEST_DELETED')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (12, 1, N'CARD_REQUEST_DELETED_fr')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (12, 2, N'CARD_REQUEST_DELETED_pt')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (12, 3, N'CARD_REQUEST_DELETED_es')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (13, 0, N'REDISTRIBUTED')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (13, 1, N'REDISTRIBUTED_fr')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (13, 2, N'REDISTRIBUTED_pt')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (13, 3, N'REDISTRIBUTED_es')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (14, 0, N'PIN_AUTHORISED')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (14, 1, N'PIN_AUTHORISED_fr')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (14, 2, N'PIN_AUTHORISED_pt')
GO	
INSERT [dbo].[branch_card_statuses_language] ([branch_card_statuses_id], [language_id], [language_text]) VALUES (14, 3, N'PIN_AUTHORISED_es')
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
INSERT [dbo].[branch_statuses] ([branch_status_id], [branch_status]) VALUES (0, N'ACTIVE')
GO	
INSERT [dbo].[branch_statuses] ([branch_status_id], [branch_status]) VALUES (1, N'INACTIVE')
GO	
INSERT [dbo].[branch_statuses] ([branch_status_id], [branch_status]) VALUES (2, N'DELETED')
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
INSERT [dbo].[product_load_type] ([product_load_type_id], [product_load_type_name]) VALUES (0, N'NO_LOAD')
GO	
INSERT [dbo].[product_load_type] ([product_load_type_id], [product_load_type_name]) VALUES (1, N'LOAD_TO_PROD')
GO	
INSERT [dbo].[product_load_type] ([product_load_type_id], [product_load_type_name]) VALUES (2, N'LOAD_TO_DIST')
GO	
INSERT [dbo].[product_load_type] ([product_load_type_id], [product_load_type_name]) VALUES (3, N'LOAD_TO_CENTRE')
GO	
INSERT [dbo].[product_load_type] ([product_load_type_id], [product_load_type_name]) VALUES (4, N'LOAD_TO_EXISTING')
GO	
INSERT [dbo].[product_load_type] ([product_load_type_id], [product_load_type_name]) VALUES (5, N'LOAD_REQUESTS')
GO	
INSERT [dbo].[product_load_type] ([product_load_type_id], [product_load_type_name]) VALUES (6, N'LOAD_REQUESTS_TO_DIST')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (0, 0, N'NO_LOAD')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (0, 1, N'NO_LOAD_fr')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (0, 2, N'NO_LOAD_es')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (0, 3, N'NO_LOAD_pt')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (1, 0, N'LOAD_TO_PROD')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (1, 1, N'LOAD_TO_PROD_fr')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (1, 2, N'LOAD_TO_PROD_es')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (1, 3, N'LOAD_TO_PROD_pt')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (2, 0, N'LOAD_TO_DIST')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (2, 1, N'LOAD_TO_DIST_fr')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (2, 2, N'LOAD_TO_DIST_es')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (2, 3, N'LOAD_TO_DIST_pt')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (3, 0, N'LOAD_TO_CENTRE')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (3, 1, N'LOAD_TO_CENTRE_fr')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (3, 2, N'LOAD_TO_CENTRE_es')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (3, 3, N'LOAD_TO_CENTRE_pt')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (4, 0, N'LOAD_TO_EXISTING')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (4, 1, N'LOAD_TO_EXISTING_fr')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (4, 2, N'LOAD_TO_EXISTING_pt')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (4, 3, N'LOAD_TO_EXISTING_es')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (5, 0, N'LOAD_REQUESTS')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (5, 1, N'LOAD_REQUESTS_fr')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (5, 2, N'LOAD_REQUESTS_es')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (5, 3, N'LOAD_REQUESTS_pt')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (6, 0, N'LOAD_REQUESTS_TO_DIST')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (6, 1, N'LOAD_REQUESTS_TO_DIST_fr')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (6, 2, N'LOAD_REQUESTS_TO_DIST_es')
GO	
INSERT [dbo].[product_load_type_language] ([product_load_type_id], [language_id], [language_text]) VALUES (6, 3, N'LOAD_REQUESTS_TO_DIST_pt')
GO	
INSERT [dbo].[card_issue_method] ([card_issue_method_id], [card_issue_method_name]) VALUES (0, N'CENTRALISED')
GO	
INSERT [dbo].[card_issue_method] ([card_issue_method_id], [card_issue_method_name]) VALUES (1, N'INSTANT')
GO	
INSERT [dbo].[card_issue_method_language] ([card_issue_method_id], [language_id], [language_text]) VALUES (0, 0, N'CENTRALISED')
GO	
INSERT [dbo].[card_issue_method_language] ([card_issue_method_id], [language_id], [language_text]) VALUES (0, 1, N'CENTRALISED_fr')
GO	
INSERT [dbo].[card_issue_method_language] ([card_issue_method_id], [language_id], [language_text]) VALUES (0, 2, N'CENTRALISED_pt')
GO	
INSERT [dbo].[card_issue_method_language] ([card_issue_method_id], [language_id], [language_text]) VALUES (0, 3, N'CENTRALISED_sp')
GO	
INSERT [dbo].[card_issue_method_language] ([card_issue_method_id], [language_id], [language_text]) VALUES (1, 0, N'INSTANT')
GO	
INSERT [dbo].[card_issue_method_language] ([card_issue_method_id], [language_id], [language_text]) VALUES (1, 1, N'INSTANT_fr')
GO	
INSERT [dbo].[card_issue_method_language] ([card_issue_method_id], [language_id], [language_text]) VALUES (1, 2, N'INSTANT_pt')
GO	
INSERT [dbo].[card_issue_method_language] ([card_issue_method_id], [language_id], [language_text]) VALUES (1, 3, N'INSTANT_sp')
GO	
INSERT [dbo].[load_batch_statuses] ([load_batch_statuses_id], [load_batch_status_name]) VALUES (0, N'LOADED')
GO	
INSERT [dbo].[load_batch_statuses] ([load_batch_statuses_id], [load_batch_status_name]) VALUES (1, N'APPROVED')
GO	
INSERT [dbo].[load_batch_statuses] ([load_batch_statuses_id], [load_batch_status_name]) VALUES (2, N'REJECTED')
GO	
INSERT [dbo].[load_batch_statuses] ([load_batch_statuses_id], [load_batch_status_name]) VALUES (3, N'INVALID')
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
INSERT [dbo].[load_card_statuses] ([load_card_status_id], [load_card_status]) VALUES (0, N'LOADED')
GO	
INSERT [dbo].[load_card_statuses] ([load_card_status_id], [load_card_status]) VALUES (1, N'AVAILABLE')
GO	
INSERT [dbo].[load_card_statuses] ([load_card_status_id], [load_card_status]) VALUES (2, N'ALLOCATED')
GO	
INSERT [dbo].[load_card_statuses] ([load_card_status_id], [load_card_status]) VALUES (3, N'REJECTED')
GO	
INSERT [dbo].[load_card_statuses] ([load_card_status_id], [load_card_status]) VALUES (4, N'CANCELLED')
GO	
INSERT [dbo].[load_card_statuses] ([load_card_status_id], [load_card_status]) VALUES (5, N'INVALID')
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
INSERT [dbo].[pin_mailer_reprint_statuses] ([pin_mailer_reprint_status_id], [pin_mailer_reprint_status_name]) VALUES (0, N'REQUESTED')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses] ([pin_mailer_reprint_status_id], [pin_mailer_reprint_status_name]) VALUES (1, N'APPROVED')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses] ([pin_mailer_reprint_status_id], [pin_mailer_reprint_status_name]) VALUES (2, N'PROCESSING')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses] ([pin_mailer_reprint_status_id], [pin_mailer_reprint_status_name]) VALUES (3, N'COMPLETE')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses] ([pin_mailer_reprint_status_id], [pin_mailer_reprint_status_name]) VALUES (4, N'REJECTED')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (0, 0, N'REQUESTED')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (0, 1, N'REQUESTED_fr')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (0, 2, N'REQUESTED_pt')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (0, 3, N'REQUESTED_es')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (1, 0, N'APPROVED')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (1, 1, N'APPROVED_fr')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (1, 2, N'APPROVED_pt')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (1, 3, N'APPROVED_es')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (2, 0, N'PROCESSING')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (2, 1, N'PROCESSING_fr')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (2, 2, N'PROCESSING_pt')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (2, 3, N'PROCESSING_es')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (3, 0, N'COMPLETE')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (3, 1, N'COMPLETE_fr')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (3, 2, N'COMPLETE_pt')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (3, 3, N'COMPLETE_es')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (4, 0, N'REJECTED')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (4, 1, N'REJECTED_fr')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (4, 2, N'REJECTED_pt')
GO	
INSERT [dbo].[pin_mailer_reprint_statuses_language] ([pin_mailer_reprint_status_id], [language_id], [language_text]) VALUES (4, 3, N'REJECTED_es')
GO	
INSERT [dbo].[customer_account_type] ([account_type_id], [account_type_name], [active_YN]) VALUES (0, N'CURRENT', 1)
GO	
INSERT [dbo].[customer_account_type] ([account_type_id], [account_type_name], [active_YN]) VALUES (1, N'SAVINGS', 1)
GO	
INSERT [dbo].[customer_account_type] ([account_type_id], [account_type_name], [active_YN]) VALUES (2, N'CHEQUE', 1)
GO	
INSERT [dbo].[customer_account_type] ([account_type_id], [account_type_name], [active_YN]) VALUES (3, N'CREDIT', 1)
GO	
INSERT [dbo].[customer_account_type] ([account_type_id], [account_type_name], [active_YN]) VALUES (4, N'UNIVERSAL', 1)
GO	
INSERT [dbo].[customer_account_type] ([account_type_id], [account_type_name], [active_YN]) VALUES (5, N'INVESTMENT', 1)
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
INSERT [dbo].[pin_batch_statuses] ([pin_batch_statuses_id], [pin_batch_statuses_name]) VALUES (0, N'CREATED')
GO	
INSERT [dbo].[pin_batch_statuses] ([pin_batch_statuses_id], [pin_batch_statuses_name]) VALUES (1, N'DISPATCHED_TO_CC')
GO	
INSERT [dbo].[pin_batch_statuses] ([pin_batch_statuses_id], [pin_batch_statuses_name]) VALUES (2, N'RECIEVED_AT_CC')
GO	
INSERT [dbo].[pin_batch_statuses] ([pin_batch_statuses_id], [pin_batch_statuses_name]) VALUES (3, N'DISPATCHED_TO_BRANCH')
GO	
INSERT [dbo].[pin_batch_statuses] ([pin_batch_statuses_id], [pin_batch_statuses_name]) VALUES (4, N'RECIEVED_AT_BRANCH')
GO	
INSERT [dbo].[pin_batch_statuses] ([pin_batch_statuses_id], [pin_batch_statuses_name]) VALUES (5, N'REJECTED_AT_CC')
GO	
INSERT [dbo].[pin_batch_statuses] ([pin_batch_statuses_id], [pin_batch_statuses_name]) VALUES (6, N'REJECTED_AT_BRANCH')
GO	
INSERT [dbo].[pin_batch_statuses] ([pin_batch_statuses_id], [pin_batch_statuses_name]) VALUES (7, N'SENT_TO_PRINTER')
GO	
INSERT [dbo].[pin_batch_statuses] ([pin_batch_statuses_id], [pin_batch_statuses_name]) VALUES (8, N'PIN_PRINTED')
GO	
INSERT [dbo].[pin_batch_statuses] ([pin_batch_statuses_id], [pin_batch_statuses_name]) VALUES (9, N'SENT_TO_CMS')
GO	
INSERT [dbo].[pin_batch_statuses] ([pin_batch_statuses_id], [pin_batch_statuses_name]) VALUES (10, N'PROCESSED_IN_CMS')
GO	
INSERT [dbo].[pin_batch_statuses] ([pin_batch_statuses_id], [pin_batch_statuses_name]) VALUES (11, N'APPROVED')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (0, 0, N'CREATED')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (0, 1, N'CREATED_fr')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (0, 2, N'CREATED_pt')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (0, 3, N'CREATED_es')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (1, 0, N'DISPATCHED_TO_CC')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (1, 1, N'DISPATCHED_TO_CC_fr')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (1, 2, N'DISPATCHED_TO_CC_pt')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (1, 3, N'DISPATCHED_TO_CC_es')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (2, 0, N'RECIEVED_AT_CC')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (2, 1, N'RECIEVED_AT_CC_fr')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (2, 2, N'RECIEVED_AT_CC_pt')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (2, 3, N'RECIEVED_AT_CC_es')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (3, 0, N'DISPATCHED_TO_BRANCH')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (3, 1, N'DISPATCHED_TO_BRANCH_fr')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (3, 2, N'DISPATCHED_TO_BRANCH_pt')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (3, 3, N'DISPATCHED_TO_BRANCH_es')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (4, 0, N'RECIEVED_AT_BRANCH')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (4, 1, N'RECIEVED_AT_BRANCH_fr')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (4, 2, N'RECIEVED_AT_BRANCH_pt')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (4, 3, N'RECIEVED_AT_BRANCH_es')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (5, 0, N'REJECTED_AT_CC')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (5, 1, N'REJECTED_AT_CC_fr')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (5, 2, N'REJECTED_AT_CC_pt')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (5, 3, N'REJECTED_AT_CC_es')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (6, 0, N'REJECTED_AT_BRANCH')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (6, 1, N'REJECTED_AT_BRANCH_fr')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (6, 2, N'REJECTED_AT_BRANCH_pt')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (6, 3, N'REJECTED_AT_BRANCH_es')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (7, 0, N'SENT_TO_PRINTER')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (7, 1, N'SENT_TO_PRINTER_fr')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (7, 2, N'SENT_TO_PRINTER_pt')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (7, 3, N'SENT_TO_PRINTER_es')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (8, 0, N'PIN_PRINTED')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (8, 1, N'PIN_PRINTED_fr')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (8, 2, N'PIN_PRINTED_pt')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (8, 3, N'PIN_PRINTED_es')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (9, 0, N'SENT_TO_CMS')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (9, 1, N'SENT_TO_CMS_fr')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (9, 2, N'SENT_TO_CMS_pt')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (9, 3, N'SENT_TO_CMS_es')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (10, 0, N'PROCESSED_IN_CMS')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (10, 1, N'PROCESSED_IN_CMS_fr')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (10, 2, N'PROCESSED_IN_CMS_pt')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (10, 3, N'PROCESSED_IN_CMS_es')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (11, 0, N'APPROVED')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (11, 1, N'APPROVED_fr')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (11, 2, N'APPROVED_pt')
GO	
INSERT [dbo].[pin_batch_statuses_language] ([pin_batch_statuses_id], [language_id], [language_text]) VALUES (11, 3, N'APPROVED_es')
GO	
INSERT [dbo].[card_priority] ([card_priority_id], [card_priority_order], [card_priority_name], [default_selection]) VALUES (0, 1, N'HIGH', 0)
GO	
INSERT [dbo].[card_priority] ([card_priority_id], [card_priority_order], [card_priority_name], [default_selection]) VALUES (1, 2, N'NORMAL', 1)
GO	
INSERT [dbo].[card_priority] ([card_priority_id], [card_priority_order], [card_priority_name], [default_selection]) VALUES (2, 3, N'LOW', 0)
GO	
INSERT [dbo].[card_priority_language] ([card_priority_id], [language_id], [language_text]) VALUES (0, 0, N'HIGH')
GO	
INSERT [dbo].[card_priority_language] ([card_priority_id], [language_id], [language_text]) VALUES (0, 1, N'HIGH_fr')
GO	
INSERT [dbo].[card_priority_language] ([card_priority_id], [language_id], [language_text]) VALUES (0, 2, N'HIGH_pt')
GO	
INSERT [dbo].[card_priority_language] ([card_priority_id], [language_id], [language_text]) VALUES (0, 3, N'HIGH_sp')
GO	
INSERT [dbo].[card_priority_language] ([card_priority_id], [language_id], [language_text]) VALUES (1, 0, N'NORMAL')
GO	
INSERT [dbo].[card_priority_language] ([card_priority_id], [language_id], [language_text]) VALUES (1, 1, N'NORMAL_fr')
GO	
INSERT [dbo].[card_priority_language] ([card_priority_id], [language_id], [language_text]) VALUES (1, 2, N'NORMAL_pt')
GO	
INSERT [dbo].[card_priority_language] ([card_priority_id], [language_id], [language_text]) VALUES (1, 3, N'NORMAL_sp')
GO	
INSERT [dbo].[card_priority_language] ([card_priority_id], [language_id], [language_text]) VALUES (2, 0, N'LOW')
GO		
INSERT [dbo].[card_priority_language] ([card_priority_id], [language_id], [language_text]) VALUES (2, 1, N'LOW_fr')
GO		
INSERT [dbo].[card_priority_language] ([card_priority_id], [language_id], [language_text]) VALUES (2, 2, N'LOW_pt')
GO		
INSERT [dbo].[card_priority_language] ([card_priority_id], [language_id], [language_text]) VALUES (2, 3, N'LOW_sp')
GO		

SET IDENTITY_INSERT [dbo].[country] ON 
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (1, N'COTE D''IVOIRE', N'CIV', N'Yamoussoukro')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (2, N'GHANA', N'GHA', N'ACCRA')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (3, N'GUINEA-BISSAU', N'GNB', N'Bissau')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (4, N'TOGO', N'TGO', N'Lome')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (5, N'BENIN', N'BEN', N'Porto-Novo')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (9, N'MALI', N'MAL', N'Bamako')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (10, N'KENYA', N'KEN', N'Nairobi')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (11, N'BURKINA FASO', N'BFA', N'Ouagadougou')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (12, N'TANZANIA', N'TZA', N'Dar Es Salaam')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (36, N'BURUNDI', N'BDI', N'Bujumbura')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (37, N'CAMEROUN', N'CMR', N'Yaoundé')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (38, N'CAPE VERDE', N'CPV', N'Praia')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (39, N'CENTRAL AFRICAN REPUBLIC', N'CAF', N'Bangui')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (40, N'CHAD', N'TCD', N'N’Djamena')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (41, N'CONGO BRAZZA', N'COG', N'Brazzaville')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (42, N'Democratic Republic of Congo', N'COD', N'Kinshasa')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (43, N'EQUATORIAL GUINEA', N'GNQ', N'Malabo')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (44, N'GABON', N'GAB', N'Libreville')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (45, N'GAMBIA', N'GMB', N'Banjul')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (46, N'GUINEA CONAKRY', N'GIN', N'Conakry')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (47, N'LIBERIA', N'LBR', N'Monrovia')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (48, N'MALAWI', N'MWI', N'Lilongwe')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (49, N'MOZAMBIQUE', N'MOZ', N'Maputo')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (50, N'NIGER', N'NER', N'Niamey')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (51, N'RWANDA', N'RWA', N'Kigali')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (52, N'SAO TOME', N'STP', N'Sao Tome')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (53, N'SIERRA LEONE', N'SLE', N'Freetown')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (54, N'SENEGAL', N'SEN', N'Dakar')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (55, N'SOUTH SUDAN', N'SSD', N'Juba')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (56, N'UGANDA', N'UGA', N'Kampala')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (57, N'ZAMBIA', N'ZMB', N'Lusaka')
INSERT [dbo].[country] ([country_id], [country_name], [country_code], [country_capital_city]) VALUES (58, N'ZIMBABWE', N'ZWE', N'Harare')
SET IDENTITY_INSERT [dbo].[country] OFF
GO	

INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (0, N'GHS', N'936', 2, N'Ghana Cedi', 1)
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (1, N'USD', N'840', 2, N'US Dollar', 1)
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (2, N'GBP', N'826', 2, N'Pound Sterling', 0)
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (3, N'EUR', N'978', 2, N'Euro', 1)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (4, N'XOF', N'952', 0, N'CFA Franc BCEAO', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (5, N'BIF', N'108', 0, N'Burundi Franc', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (6, N'CDF', N'976', 2, N'Congolese Franc', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (7, N'CVE', N'132', 2, N'Cabo Verde Escudo', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (8, N'GMD', N'270', 2, N'Dalasi', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (9, N'GNF', N'324', 0, N'Guinea Franc', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (10, N'KES', N'404', 2, N'Kenyan Shilling', 1)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (11, N'LRD', N'430', 2, N'Liberian Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (12, N'MWK', N'454', 2, N'Kwacha', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (13, N'NGN', N'566', 2, N'Naira', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (14, N'RWF', N'646', 0, N'Rwanda Franc', 1)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (15, N'SLL', N'694', 2, N'Leone', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (16, N'SSP', N'728', 2, N'South Sudanese Pound', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (17, N'STD', N'678', 2, N'Dobra', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (18, N'TZS', N'834', 2, N'Tanzanian Shilling', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (19, N'UGX', N'800', 0, N'Uganda Shilling', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (20, N'XAF', N'950', 0, N'CFA Franc BEAC', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (21, N'ZMW', N'967', 2, N'Zambian Kwacha', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (22, N'AED', N'784', 2, N'UAE Dirham', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (23, N'ZAR', N'710', 2, N'Rand', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (24, N'ALL', N'008', 2, N'Lek', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (25, N'AMD', N'051', 2, N'Armenian Dram', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (26, N'ANG', N'532', 2, N'Netherlands Antillean Guilder', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (27, N'AOA', N'973', 2, N'Kwanza', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (28, N'ARS', N'032', 2, N'Argentine Peso', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (29, N'AUD', N'036', 2, N'Australian Dollar', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (30, N'AWG', N'533', 2, N'Aruban Florin', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (31, N'AZN', N'944', 2, N'Azerbaijanian Manat', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (32, N'BAM', N'977', 2, N'Convertible Mark', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (33, N'BBD', N'052', 2, N'Barbados Dollar', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (34, N'BDT', N'050', 2, N'Taka', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (35, N'BGN', N'975', 2, N'Bulgarian Lev', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (36, N'BHD', N'048', 3, N'Bahraini Dinar', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (37, N'BMD', N'060', 2, N'Bermudian Dollar', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (38, N'BND', N'096', 2, N'Brunei Dollar', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (39, N'BOB', N'068', 2, N'Boliviano', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (40, N'BOV', N'984', 2, N'Mvdol', 0)
GO		
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (41, N'BRL', N'986', 2, N'Brazilian Real', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (42, N'BSD', N'044', 2, N'Bahamian Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (43, N'BTN', N'064', 2, N'Ngultrum', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (44, N'BWP', N'072', 2, N'Pula', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (45, N'BYR', N'974', 0, N'Belarussian Ruble', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (46, N'BZD', N'084', 2, N'Belize Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (47, N'CAD', N'124', 2, N'Canadian Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (48, N'CHE', N'947', 2, N'WIR Euro', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (49, N'CHF', N'756', 2, N'Swiss Franc', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (50, N'CHW', N'948', 2, N'WIR Franc', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (51, N'CLF', N'990', 4, N'Unidad de Fomento', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (52, N'CLP', N'152', 0, N'Chilean Peso', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (53, N'CNY', N'156', 2, N'Yuan Renminbi', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (54, N'COP', N'170', 2, N'Colombian Peso', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (55, N'COU', N'970', 2, N'Unidad de Valor Real', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (56, N'CRC', N'188', 2, N'Costa Rican Colon', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (57, N'CUC', N'931', 2, N'Peso Convertible', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (58, N'CUP', N'192', 2, N'Cuban Peso', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (59, N'CZK', N'203', 2, N'Czech Koruna', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (60, N'DJF', N'262', 0, N'Djibouti Franc', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (61, N'DKK', N'208', 2, N'Danish Krone', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (62, N'DOP', N'214', 2, N'Dominican Peso', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (63, N'DZD', N'012', 2, N'Algerian Dinar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (64, N'EGP', N'818', 2, N'Egyptian Pound', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (65, N'ERN', N'232', 2, N'Nakfa', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (66, N'ETB', N'230', 2, N'Ethiopian Birr', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (67, N'FJD', N'242', 2, N'Fiji Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (68, N'FKP', N'238', 2, N'Falkland Islands Pound', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (69, N'GEL', N'981', 2, N'Lari', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (70, N'GIP', N'292', 2, N'Gibraltar Pound', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (71, N'GTQ', N'320', 2, N'Quetzal', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (72, N'GYD', N'328', 2, N'Guyana Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (73, N'HKD', N'344', 2, N'Hong Kong Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (74, N'HNL', N'340', 2, N'Lempira', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (75, N'HRK', N'191', 2, N'Croatian Kuna', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (76, N'HTG', N'332', 2, N'Gourde', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (77, N'HUF', N'348', 2, N'Forint', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (78, N'IDR', N'360', 2, N'Rupiah', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (79, N'ILS', N'376', 2, N'New Israeli Sheqel', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (80, N'INR', N'356', 2, N'Indian Rupee', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (81, N'INR', N'356', 2, N'Indian Rupee', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (82, N'IQD', N'368', 3, N'Iraqi Dinar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (83, N'IRR', N'364', 2, N'Iranian Rial', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (84, N'ISK', N'352', 0, N'Iceland Krona', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (85, N'JMD', N'388', 2, N'Jamaican Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (86, N'JOD', N'400', 3, N'Jordanian Dinar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (87, N'JPY', N'392', 0, N'Yen', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (88, N'KGS', N'417', 2, N'Som', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (89, N'KHR', N'116', 2, N'Riel', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (90, N'KMF', N'174', 0, N'Comoro Franc', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (91, N'KPW', N'408', 2, N'North Korean Won', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (92, N'KRW', N'410', 0, N'Won', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (93, N'KWD', N'414', 3, N'Kuwaiti Dinar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (94, N'KYD', N'136', 2, N'Cayman Islands Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (95, N'KZT', N'398', 2, N'Tenge', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (96, N'LAK', N'418', 2, N'Kip', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (97, N'LBP', N'422', 2, N'Lebanese Pound', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (98, N'LKR', N'144', 2, N'Sri Lanka Rupee', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (99, N'LSL', N'426', 2, N'Loti', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (100, N'LYD', N'434', 3, N'Libyan Dinar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (101, N'MAD', N'504', 2, N'Moroccan Dirham', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (102, N'MDL', N'498', 2, N'Moldovan Leu', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (103, N'MGA', N'969', 2, N'Malagasy Ariary', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (104, N'MKD', N'807', 2, N'Denar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (105, N'MMK', N'104', 2, N'Kyat', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (106, N'MNT', N'496', 2, N'Tugrik', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (107, N'MOP', N'446', 2, N'Pataca', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (108, N'MRO', N'478', 2, N'Ouguiya', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (109, N'MUR', N'480', 2, N'Mauritius Rupee', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (110, N'MVR', N'462', 2, N'Rufiyaa', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (111, N'MXN', N'484', 2, N'Mexican Peso', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (112, N'MXV', N'979', 2, N'Mexican Unidad de Inversion (UDI)', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (113, N'MYR', N'458', 2, N'Malaysian Ringgit', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (114, N'MZN', N'943', 2, N'Mozambique Metical', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (115, N'NAD', N'516', 2, N'Namibia Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (116, N'NIO', N'558', 2, N'Cordoba Oro', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (117, N'NOK', N'578', 2, N'Norwegian Krone', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (118, N'NOK', N'578', 2, N'Norwegian Krone', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (119, N'NOK', N'578', 2, N'Norwegian Krone', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (120, N'NPR', N'524', 2, N'Nepalese Rupee', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (121, N'NZD', N'554', 2, N'New Zealand Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (122, N'OMR', N'512', 3, N'Rial Omani', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (123, N'PAB', N'590', 2, N'Balboa', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (124, N'PEN', N'604', 2, N'Nuevo Sol', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (125, N'PGK', N'598', 2, N'Kina', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (126, N'PHP', N'608', 2, N'Philippine Peso', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (127, N'PKR', N'586', 2, N'Pakistan Rupee', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (128, N'PLN', N'985', 2, N'Zloty', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (129, N'PYG', N'600', 0, N'Guarani', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (130, N'QAR', N'634', 2, N'Qatari Rial', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (131, N'RON', N'946', 2, N'New Romanian Leu', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (132, N'RSD', N'941', 2, N'Serbian Dinar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (133, N'RUB', N'643', 2, N'Russian Ruble', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (134, N'SAR', N'682', 2, N'Saudi Riyal', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (135, N'SBD', N'090', 2, N'Solomon Islands Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (136, N'SCR', N'690', 2, N'Seychelles Rupee', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (137, N'SDG', N'938', 2, N'Sudanese Pound', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (138, N'SEK', N'752', 2, N'Swedish Krona', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (139, N'SGD', N'702', 2, N'Singapore Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (140, N'SHP', N'654', 2, N'Saint Helena Pound', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (141, N'SOS', N'706', 2, N'Somali Shilling', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (142, N'SRD', N'968', 2, N'Surinam Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (143, N'SVC', N'222', 2, N'El Salvador Colon', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (144, N'SYP', N'760', 2, N'Syrian Pound', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (145, N'SZL', N'748', 2, N'Lilangeni', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (146, N'THB', N'764', 2, N'Baht', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (147, N'TJS', N'972', 2, N'Somoni', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (148, N'TMT', N'934', 2, N'Turkmenistan New Manat', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (149, N'TND', N'788', 3, N'Tunisian Dinar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (150, N'TOP', N'776', 2, N'Pa’anga', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (151, N'TRY', N'949', 2, N'Turkish Lira', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (152, N'TTD', N'780', 2, N'Trinidad and Tobago Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (153, N'TWD', N'901', 2, N'New Taiwan Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (154, N'UAH', N'980', 2, N'Hryvnia', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (155, N'USN', N'997', 2, N'US Dollar(Next Day)', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (156, N'UYI', N'940', 0, N'Uruguay Peso en Unidades Indexadas (URUIURUI)', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (157, N'UYU', N'858', 2, N'Peso Uruguayo', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (158, N'UZS', N'860', 2, N'Uzbekistan Sum', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (159, N'VEF', N'937', 2, N'Bolivar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (160, N'VND', N'704', 0, N'Dong', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (161, N'VUV', N'548', 0, N'Vatu', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (162, N'WST', N'882', 2, N'Tala', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (163, N'XAG', N'961', NULL, N'Silver', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (164, N'XAU', N'959', NULL, N'Gold', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (165, N'XBA', N'955', NULL, N'Bond Markets Unit European Composite Unit (EURCO)', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (166, N'XBB', N'956', NULL, N'Bond Markets Unit European Monetary Unit (E.M.U.-6)', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (167, N'XBC', N'957', NULL, N'Bond Markets Unit European Unit of Account 9 (E.U.A.-9)', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (168, N'XBD', N'958', NULL, N'Bond Markets Unit European Unit of Account 17 (E.U.A.-17)', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (169, N'XCD', N'951', 2, N'East Caribbean Dollar', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (170, N'XDR', N'960', NULL, N'SDR (Special Drawing Right)', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (171, N'XPD', N'964', NULL, N'Palladium', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (172, N'XPF', N'953', 0, N'CFP Franc', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (173, N'XPT', N'962', NULL, N'Platinum', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (174, N'XSU', N'994', NULL, N'Sucre', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (175, N'XTS', N'963', NULL, N'Codes specifically reserved for testing purposes', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (176, N'XUA', N'965', NULL, N'ADB Unit of Account', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (177, N'XXX', N'999', NULL, N'The codes assigned for transactions where no currency is involved', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (178, N'YER', N'886', 2, N'Yemeni Rial', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (179, N'AFN', N'971', 2, N'Afghani', 0)
GO	
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (180, N'ZWL', N'932', 2, N'Zimbabwe Dollar', 0)
GO	
INSERT [dbo].[dist_batch_type] ([dist_batch_type_id], [dist_batch_type_name]) VALUES (0, N'PRODUCTION')
GO	
INSERT [dbo].[dist_batch_type] ([dist_batch_type_id], [dist_batch_type_name]) VALUES (1, N'DISTRIBUTION')
GO	


--SET IDENTITY_INSERT [dbo].[file_encryption_type] ON 

INSERT INTO [dbo].[file_encryption_type] ([file_encryption_type_id], [file_encryption_type], [file_encryption_typeid])
     VALUES (0, N'None', NULL),
			(1, N'PGP', NULL)

--SET IDENTITY_INSERT [dbo].[file_encryption_type] OFF
GO	


INSERT [dbo].[file_types] ([file_type_id], [file_type]) VALUES (0, N'PIN_MAILER')
GO	
INSERT [dbo].[file_types] ([file_type_id], [file_type]) VALUES (1, N'CARD_IMPORT')
GO	
INSERT [dbo].[file_types] ([file_type_id], [file_type]) VALUES (3, N'UNKNOWN')
GO	
INSERT [dbo].[load_batch_types] ([load_batch_type_id], [load_batch_type]) VALUES (1, N'CARD FILE')
GO	
INSERT [dbo].[load_batch_types] ([load_batch_type_id], [load_batch_type]) VALUES (2, N'CARD REQUEST FILE')
GO	
INSERT [dbo].[pin_batch_card_statuses] ([pin_batch_card_statuses_id], [pin_batch_card_statuses_name]) VALUES (0, N'CREATED')
GO	
INSERT [dbo].[pin_batch_card_statuses] ([pin_batch_card_statuses_id], [pin_batch_card_statuses_name]) VALUES (1, N'ALLOCATED')
GO	
INSERT [dbo].[pin_batch_card_statuses] ([pin_batch_card_statuses_id], [pin_batch_card_statuses_name]) VALUES (2, N'RECEIVED_AT_CC')
GO	
INSERT [dbo].[pin_batch_card_statuses] ([pin_batch_card_statuses_id], [pin_batch_card_statuses_name]) VALUES (3, N'ALLOCATED_TO_BRANCH')
GO	
INSERT [dbo].[pin_batch_card_statuses] ([pin_batch_card_statuses_id], [pin_batch_card_statuses_name]) VALUES (4, N'RECEIVED_AT_BRANCH')
GO	
INSERT [dbo].[pin_batch_card_statuses] ([pin_batch_card_statuses_id], [pin_batch_card_statuses_name]) VALUES (5, N'REJECTED')
GO	
INSERT [dbo].[pin_batch_type] ([pin_batch_type_id], [pin_batch_type_name]) VALUES (0, N'PRODUCTION')
GO	
INSERT [dbo].[pin_batch_type] ([pin_batch_type_id], [pin_batch_type_name]) VALUES (1, N'DISTRIBUTION')
GO	
INSERT [dbo].[pin_batch_type] ([pin_batch_type_id], [pin_batch_type_name]) VALUES (2, N'REPRINT')
GO	
INSERT [dbo].[print_field_types] ([print_field_type_id], [print_field_name]) VALUES (0, N'None')
GO	
INSERT [dbo].[print_field_types] ([print_field_type_id], [print_field_name]) VALUES (1, N'Image')
GO	
INSERT [dbo].[product_service_requet_code1] ([src1_id], [name]) VALUES (1, N'International card')
GO	
INSERT [dbo].[product_service_requet_code1] ([src1_id], [name]) VALUES (2, N'International card - integrated circuit facilities')
GO	
INSERT [dbo].[product_service_requet_code1] ([src1_id], [name]) VALUES (5, N'National use only')
GO	
INSERT [dbo].[product_service_requet_code1] ([src1_id], [name]) VALUES (6, N'National use only - integrated circuit facilities')
GO	
INSERT [dbo].[product_service_requet_code1] ([src1_id], [name]) VALUES (9, N'Test card - online authorization mandatory')
GO	
INSERT [dbo].[product_service_requet_code2] ([src2_id], [name]) VALUES (0, N'Normal authorization')
GO	
INSERT [dbo].[product_service_requet_code2] ([src2_id], [name]) VALUES (2, N'Online authorization mandatory')
GO	
INSERT [dbo].[product_service_requet_code2] ([src2_id], [name]) VALUES (4, N'Online authorization mandatory')
GO	
INSERT [dbo].[product_service_requet_code3] ([src3_id], [name]) VALUES (0, N'PIN required')
GO	
INSERT [dbo].[product_service_requet_code3] ([src3_id], [name]) VALUES (1, N'No restrictions - normal cardholder verification')
GO	
INSERT [dbo].[product_service_requet_code3] ([src3_id], [name]) VALUES (2, N'Goods and services only')
GO	
INSERT [dbo].[product_service_requet_code3] ([src3_id], [name]) VALUES (3, N'PIN required, ATM only')
GO	
INSERT [dbo].[product_service_requet_code3] ([src3_id], [name]) VALUES (5, N'PIN required, goods and services only at POS, cash at ATM')
GO	
INSERT [dbo].[product_service_requet_code3] ([src3_id], [name]) VALUES (6, N'PIN required if PIN pad present')
GO	
INSERT [dbo].[product_service_requet_code3] ([src3_id], [name]) VALUES (7, N'PIN required if PIN pad present, goods and services only at POS, cash at ATM')
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
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (14, N'Notes')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (15, N'TRANSACTION REFERENCE')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (16, N'TRANSACTION DESCRIPTION')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (17, N'MAKER ID')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (18, N'MAKER DATE')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (19, N'MAKER TIME')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (20, N'MAKER IP ADDRESS')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (21, N'ACTIVITY')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (22, N'OLD VALUE')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (23, N'NEW VALUE')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (24, N'Card Reference Number')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (25, N'Number Of Cards:')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (26, N'Line Item')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (27, N'Pin Batch Report')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (28, N'Branch Name')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (29, N'Branch Code')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (30, N'Export Batch Report')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (31, N'Generated By: ')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (32, N'Issuer: ')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (33, N'Branch: ')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (34, N'User: ')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (35, N'Date Range: ')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (36, N'Spoiled By ')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (37, N'Customer Names ')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (38, N'PAN')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (39, N'Card Reference')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (40, N'Reason')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (41, N'Spoiled Date')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (42, N'Spoiled cards report')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (43, N'* End of report')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (44, N'***Confidential Report***')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (45, N'Total')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (46, N'Issued By')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (47, N'Account Number')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (48, N'Approved User')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (49, N'Scenario')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (50, N'Issued Date')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (51, N'Fee Charged')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (52, N'Issued cards report')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (53, N'Issued cards summary report')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (55, N'Inventory Summary Report')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (56, N'Audit Report – Branches Per User Group')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (57, N'User Role')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (58, N'User Group Name')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (59, N'All Branch Access')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (60, N'Execution Time')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (61, N'Audit Report – Users Per User Role')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (62, N'Mask Screen PAN')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (63, N'Mask Report PAN')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (64, N'Update')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (65, N'Create')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (66, N'Delete')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (67, N'Read')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (68, N'User Name')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (69, N'Audit Report – User Groups')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (70, N'Customer Account')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (71, N'Batch Reference')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (72, N'Date Created')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (73, N'Pin Mailer Report')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (75, N'PIN Re-issue Report')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (76, N'Requested Date Time')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (77, N'Approved DateTime')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (78, N'Supervisor')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (79, N'CardDispatchReport')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (80, N'Customer Full Name')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (81, N'Name On Card')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (82, N'Expiry')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (83, N'Card Expiry Report')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (84, N'Card Production Report')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (85, N'BrachOrderReport')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (86, N'Mobile Number')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (87, N'Branch Card Stock Management Report')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (88, N'Production Date')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (89, N'Full Fee')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (90, N'Amended Fee')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (91, N'Zero / No Fee')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (92, N'180 d')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (93, N'90 d')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (94, N'Week 4')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (95, N'Week 3')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (96, N'Week 2')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (97, N'Week 1')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (98, N'Current Cards Stock')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (99, N'Weeks Remaining')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (100, N'Fee Revenue Report')
GO	
INSERT [dbo].[report_fields] ([reportfieldid], [reportfieldname]) VALUES (101, N'Burn Rate Report')
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (1, 0, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (1, 1, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (1, 2, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (1, 3, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (1, 4, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (1, 5, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (1, 6, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (1, 7, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (1, 9, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (1, 24, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (1, 25, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (1, 26, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (1, 28, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (1, 29, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (2, 0, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (2, 1, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (2, 2, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (2, 3, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (2, 4, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (2, 5, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (2, 6, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (2, 7, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (2, 10, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (2, 14, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (2, 24, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (2, 25, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (2, 26, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (2, 28, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (2, 29, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (3, 0, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (3, 2, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (3, 3, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (3, 4, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (3, 7, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (3, 8, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (3, 11, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (3, 12, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (3, 13, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (3, 24, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (3, 25, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (3, 26, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (4, 15, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (4, 16, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (4, 17, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (4, 18, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (4, 19, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (4, 20, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (4, 21, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (4, 22, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (4, 23, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (5, 0, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (5, 1, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (5, 2, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (5, 3, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (5, 4, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (5, 5, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (5, 6, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (5, 7, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (5, 24, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (5, 25, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (5, 26, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (5, 27, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (5, 28, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (5, 29, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (6, 0, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (6, 1, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (6, 2, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (6, 3, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (6, 4, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (6, 5, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (6, 6, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (6, 7, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (6, 14, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (6, 24, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (6, 25, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (6, 26, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (6, 30, NULL)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (7, 29, 7)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (7, 31, 2)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (7, 32, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (7, 33, 4)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (7, 34, 5)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (7, 35, 6)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (7, 36, 8)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (7, 37, 9)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (7, 38, 10)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (7, 39, 11)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (7, 40, 12)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (7, 41, 13)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (7, 42, 1)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (7, 43, 15)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (7, 44, 16)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (7, 45, 14)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 31, 2)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 32, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 33, 4)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 34, 5)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 35, 6)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 37, 8)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 38, 10)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 39, 11)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 43, 17)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 44, 18)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 45, 16)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 46, 7)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 47, 9)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 48, 12)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 49, 13)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 50, 14)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 51, 15)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (8, 52, 1)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (9, 29, 7)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (9, 31, 2)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (9, 32, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (9, 33, 4)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (9, 35, 5)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (9, 43, 9)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (9, 44, 10)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (9, 45, 8)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (9, 53, 1)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (11, 29, 7)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (11, 31, 2)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (11, 32, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (11, 33, 4)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (11, 35, 5)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (11, 43, 9)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (11, 44, 10)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (11, 45, 8)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (11, 54, 1)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (11, 55, 1)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (12, 31, 2)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (12, 32, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (12, 33, 7)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (12, 43, 9)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (12, 44, 10)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (12, 56, 1)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (12, 57, 4)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (12, 58, 5)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (12, 59, 8)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (12, 60, 6)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (13, 31, 2)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (13, 32, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (13, 43, 15)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (13, 44, 16)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (13, 57, 4)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (13, 58, 7)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (13, 59, 14)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (13, 60, 5)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (13, 61, 1)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (13, 62, 8)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (13, 63, 9)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (13, 64, 10)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (13, 65, 11)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (13, 66, 12)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (13, 67, 13)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (13, 68, 6)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (14, 31, 2)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (14, 32, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (14, 43, 14)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (14, 44, 15)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (14, 57, 4)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (14, 58, 6)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (14, 59, 13)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (14, 60, 5)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (14, 62, 7)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (14, 63, 8)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (14, 64, 9)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (14, 65, 10)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (14, 66, 11)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (14, 67, 12)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (14, 69, 1)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (15, 31, 2)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (15, 32, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (15, 33, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (15, 43, 8)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (15, 44, 9)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (15, 45, 7)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (15, 70, 4)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (15, 71, 5)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (15, 72, 6)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (15, 73, 1)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (17, 8, 7)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (17, 31, 2)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (17, 32, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (17, 33, 4)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (17, 34, 5)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (17, 35, 6)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (17, 38, 8)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (17, 40, 9)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (17, 43, 14)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (17, 44, 15)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (17, 45, 13)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (17, 75, 1)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (17, 76, 10)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (17, 77, 12)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (17, 78, 11)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (18, 32, 2)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (18, 33, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (18, 38, 7)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (18, 39, 8)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (18, 43, 12)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (18, 44, 13)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (18, 45, 11)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (18, 70, 5)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (18, 71, 9)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (18, 72, 10)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (18, 79, 1)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (18, 80, 4)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (18, 81, 6)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (19, 32, 2)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (19, 33, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (19, 38, 4)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (19, 39, 5)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (19, 43, 8)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (19, 44, 9)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (19, 45, 7)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (19, 82, 6)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (19, 84, 1)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (20, 32, 2)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (20, 33, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (20, 38, 7)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (20, 39, 9)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (20, 43, 12)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (20, 44, 13)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (20, 45, 11)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (20, 70, 5)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (20, 71, 8)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (20, 72, 10)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (20, 80, 4)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (20, 81, 6)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (20, 84, 1)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (21, 32, 2)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (21, 33, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (21, 43, 11)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (21, 44, 12)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (21, 45, 10)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (21, 70, 5)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (21, 71, 8)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (21, 72, 9)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (21, 80, 4)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (21, 81, 7)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (21, 85, 1)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (21, 86, 6)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (22, 29, 6)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (22, 31, 2)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (22, 32, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (22, 33, 4)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (22, 38, 8)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (22, 39, 7)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (22, 43, 12)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (22, 44, 13)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (22, 45, 11)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (22, 60, 5)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (22, 82, 10)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (22, 87, 1)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (22, 88, 9)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (23, 29, 6)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (23, 31, 2)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (23, 32, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (23, 33, 4)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (23, 43, 12)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (23, 44, 13)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (23, 45, 11)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (23, 51, 10)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (23, 60, 5)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (23, 89, 7)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (23, 90, 8)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (23, 91, 9)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (23, 100, 1)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 29, 6)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 31, 2)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 32, 3)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 33, 4)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 43, 16)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 44, 17)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 45, 15)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 60, 5)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 92, 7)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 93, 8)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 94, 9)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 95, 10)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 96, 11)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 97, 12)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 98, 13)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 99, 14)
GO	
INSERT [dbo].[report_reportfields] ([reportid], [reportfieldid], [reportfieldorderno]) VALUES (24, 101, 1)
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
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (14, 0, N'Notes')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (14, 1, N'Notes_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (14, 2, N'Notes_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (14, 3, N'Notes_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (15, 0, N'TRANSACTION REFERENCE')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (15, 1, N'TRANSACTION REFERENCE_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (15, 2, N'TRANSACTION REFERENCE_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (15, 3, N'TRANSACTION REFERENCE_sp')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (16, 0, N'TRANSACTION Description')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (16, 1, N'TRANSACTION Description_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (16, 2, N'TRANSACTION Description_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (16, 3, N'TRANSACTION Description_sp')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (17, 0, N'MAKER ID')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (17, 1, N'MAKER ID_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (17, 2, N'MAKER ID_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (17, 3, N'MAKER ID_sp')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (18, 0, N'MAKER DATE')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (18, 1, N'MAKER DATE_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (18, 2, N'MAKER DATE_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (18, 3, N'MAKER DATE_sp')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (19, 0, N'MAKER TIME')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (19, 1, N'MAKER TIME_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (19, 2, N'MAKER TIME_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (19, 3, N'MAKER TIME_sp')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (20, 0, N'MAKER IP Address')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (20, 1, N'MAKER IP Address_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (20, 2, N'MAKER IP Address_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (20, 3, N'MAKER IP Address_sp')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (21, 0, N'ACTIVITY')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (21, 1, N'ACTIVITY_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (21, 2, N'ACTIVITY_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (21, 3, N'ACTIVITY_sp')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (22, 0, N'OLD VALUE')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (22, 1, N'OLD VALUE_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (22, 2, N'OLD VALUE_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (22, 3, N'OLD VALUE_sp')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (23, 0, N'NEW VALUE')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (23, 1, N'NEW VALUE_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (23, 2, N'NEW VALUE_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (23, 3, N'NEW VALUE_sp')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (24, 0, N'Card Reference Number')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (24, 1, N'Card Reference Number_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (24, 2, N'Card Reference Number_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (24, 3, N'Card Reference Number_sp')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (25, 0, N'Number Of Cards:')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (25, 1, N'Number Of Cards:')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (25, 2, N'Number Of Cards:')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (25, 3, N'Number Of Cards:')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (26, 0, N'Line Item')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (26, 1, N'Line Item')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (26, 2, N'Line Item')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (26, 3, N'Line Item')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (27, 0, N'Pin Batch Report')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (27, 1, N'Pin Batch Report_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (27, 2, N'Pin Batch Report_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (27, 3, N'Pin Batch Report_sp')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (28, 0, N'Branch Name')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (28, 1, N'Branch Name_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (28, 2, N'Branch Name_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (28, 3, N'Branch Name_sp')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (29, 0, N'Branch Code')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (29, 1, N'Branch Code_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (29, 2, N'Branch Code_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (29, 3, N'Branch Code_sp')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (30, 0, N'Export Batch Report')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (30, 1, N'Export Batch Report_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (30, 2, N'Export Batch Report_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (30, 3, N'Export Batch Report_sp')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (31, 0, N'Generated By: ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (31, 1, N'Generated By:_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (31, 2, N'Generated By:_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (31, 3, N'Generated By:_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (32, 0, N'Issuer: ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (32, 1, N'Issuer:_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (32, 2, N'Issuer:_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (32, 3, N'Issuer:_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (33, 0, N'Branch: ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (33, 1, N'Branch:_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (33, 2, N'Branch:_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (33, 3, N'Branch:_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (34, 0, N'User: ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (34, 1, N'User:_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (34, 2, N'User:_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (34, 3, N'User:_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (35, 0, N'Date Range: ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (35, 1, N'Date Range:_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (35, 2, N'Date Range:_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (35, 3, N'Date Range:_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (36, 0, N'Spoiled By ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (36, 1, N'Spoiled By_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (36, 2, N'Spoiled By_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (36, 3, N'Spoiled By_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (37, 0, N'Customer Names ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (37, 1, N'Customer Names_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (37, 2, N'Customer Names_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (37, 3, N'Customer Names_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (38, 0, N'PAN')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (38, 1, N'PAN_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (38, 2, N'PAN_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (38, 3, N'PAN_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (39, 0, N'Card Reference')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (39, 1, N'Card Reference_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (39, 2, N'Card Reference_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (39, 3, N'Card Reference_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (40, 0, N'Reason')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (40, 1, N'Reason_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (40, 2, N'Reason_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (40, 3, N'Reason_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (41, 0, N'Spoiled Date')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (41, 1, N'Spoiled Date')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (41, 2, N'Spoiled Date')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (41, 3, N'Spoiled Date')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (42, 0, N'Spoiled cards report')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (42, 1, N'Spoiled cards report_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (42, 2, N'Spoiled cards report_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (42, 3, N'Spoiled cards report_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (43, 0, N'* End of report')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (43, 1, N'* End of report_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (43, 2, N'* End of report_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (43, 3, N'* End of report_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (44, 0, N'***Confidential Report***')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (44, 1, N'***Confidential Report***_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (44, 2, N'***Confidential Report***_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (44, 3, N'***Confidential Report***_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (45, 0, N'Total')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (45, 1, N'Total_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (45, 2, N'Total_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (45, 3, N'Total_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (46, 0, N'Issued By ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (46, 1, N'Issued By_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (46, 2, N'Issued By_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (46, 3, N'Issued By_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (47, 0, N'Account Number ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (47, 1, N'Account Number_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (47, 2, N'Account Number_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (47, 3, N'Account Number_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (48, 0, N'Approved User ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (48, 1, N'Approved User_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (48, 2, N'Approved User_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (48, 3, N'Approved User_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (49, 0, N'Scenario')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (49, 1, N'Scenario_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (49, 2, N'Scenario_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (49, 3, N'Scenario_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (50, 0, N'Issued Date')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (50, 1, N'Issued Date_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (50, 2, N'Issued Date_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (50, 3, N'Issued Date_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (51, 0, N'Fee Charged')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (51, 1, N'Fee Charged_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (51, 2, N'Fee Charged_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (51, 3, N'Fee Charged_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (52, 0, N'Issued cards report')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (52, 1, N'Issued cards report_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (52, 2, N'Issued cards report_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (52, 3, N'Issued cards report_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (53, 0, N'Issued cards summary report ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (53, 1, N'Issued cards summary reportt_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (53, 2, N'Issued cards summary reportt_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (53, 3, N'Issued cards summary report_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (54, 0, N'Spoil cards summary report ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (54, 1, N'Spoil cards summary reportt_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (54, 2, N'Spoil cards summary reportt_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (54, 3, N'Spoil cards summary report_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (55, 0, N'Inventory Summary Report ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (55, 1, N'Inventory Summary Report_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (55, 2, N'Inventory Summary Report_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (55, 3, N'Inventory Summary Report_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (56, 0, N'Audit Report – Branches Per User Group ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (56, 1, N'Audit Report – Branches Per User Group_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (56, 2, N'Audit Report – Branches Per User Group_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (56, 3, N'Audit Report – Branches Per User Group_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (57, 0, N'User Role')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (57, 1, N'User Role_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (57, 2, N'User Role_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (57, 3, N'User Role_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (58, 0, N'User Group Name')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (58, 1, N'User Group Name_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (58, 2, N'User Group Name_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (58, 3, N'User Group Name_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (59, 0, N'All Branch Access')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (59, 1, N'All Branch Access_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (59, 2, N'All Branch Access_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (59, 3, N'All Branch Access_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (60, 0, N'Execution Time')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (60, 1, N'Execution Time_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (60, 2, N'Execution Time_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (60, 3, N'Execution Time_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (61, 0, N'Audit Report – Users Per User Role')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (61, 1, N'Audit Report – Users Per User Role_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (61, 2, N'Audit Report – Users Per User Role_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (61, 3, N'Audit Report – Users Per User Role_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (62, 0, N'Mask Screen PAN')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (62, 1, N'Mask Screen PAN_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (62, 2, N'Mask Screen PAN_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (62, 3, N'Mask Screen PAN_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (63, 0, N'Mask Report PAN')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (63, 1, N'Mask Report PAN_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (63, 2, N'Mask Report PAN_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (63, 3, N'Mask Report PAN_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (64, 0, N'Update')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (64, 1, N'Update_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (64, 2, N'Update_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (64, 3, N'Update_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (65, 0, N'Create')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (65, 1, N'Create_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (65, 2, N'Create_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (65, 3, N'Create_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (66, 0, N'Delete')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (66, 1, N'Delete_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (66, 2, N'Delete_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (66, 3, N'Delete_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (67, 0, N'Read')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (67, 1, N'Read_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (67, 2, N'Read_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (67, 3, N'Read_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (68, 0, N'User Name')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (68, 1, N'User Name_fr ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (68, 2, N'User Name_pt ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (68, 3, N'User Name_es ')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (69, 0, N'Audit Report – User Groups')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (69, 1, N'Audit Report – User Groups_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (69, 2, N'Audit Report – User Groups_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (69, 3, N'Audit Report – User Groups_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (70, 0, N'Customer Account')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (70, 1, N'Customer Account_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (70, 2, N'Customer Account_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (70, 3, N'Customer Account_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (71, 0, N'Batch Reference')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (71, 1, N'Batch Reference_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (71, 2, N'Batch Reference_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (71, 3, N'Batch Reference_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (72, 0, N'Date Created')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (72, 1, N'Date Created_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (72, 2, N'Date Created_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (72, 3, N'Date Created_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (73, 0, N'Pin Mailer Report')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (73, 1, N'Pin Mailer Report_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (73, 2, N'Pin Mailer Report_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (73, 3, N'Pin Mailer Report_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (75, 0, N'PIN Re-issue Report')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (75, 1, N'PIN Re-issue Report_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (75, 2, N'PIN Re-issue Report_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (75, 3, N'PIN Re-issue Report_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (76, 0, N'Requested Date Time')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (76, 1, N'Requested Date Time_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (76, 2, N'Requested Date Time_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (76, 3, N'Requested Date Time_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (77, 0, N'Approved DateTime')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (77, 1, N'Approved DateTime_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (77, 2, N'Approved DateTime_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (77, 3, N'Approved DateTime_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (78, 0, N'Supervisor')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (78, 1, N'Supervisor_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (78, 2, N'Supervisor_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (78, 3, N'Supervisor_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (79, 0, N'Card Dispatch Report')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (79, 1, N'CardDispatchReport_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (79, 2, N'CardDispatchReport_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (79, 3, N'CardDispatchReport_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (80, 0, N'Customer Full Name')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (80, 1, N'Customer Full Name_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (80, 2, N'Customer Full Name_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (80, 3, N'Customer Full Name_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (81, 0, N'Name On Card')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (81, 1, N'Name On Card_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (81, 2, N'Name On Card_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (81, 3, N'Name On Card_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (82, 0, N'Expiry')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (82, 1, N'Expiry_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (82, 2, N'Expiry_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (82, 3, N'Expiry_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (83, 0, N'Card Expiry Report')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (83, 1, N'Card Expiry Report_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (83, 2, N'Card Expiry Report_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (83, 3, N'Card Expiry Report_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (84, 0, N'Card Production Report')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (84, 1, N'Card Production Report_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (84, 2, N'Card Production Report_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (84, 3, N'Card Production Report_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (85, 0, N'Branch Order Report')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (85, 1, N'Branch Order Report_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (85, 2, N'Branch Order Report_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (85, 3, N'Branch Order Report_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (86, 0, N'Mobile Number')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (86, 1, N'Mobile Number_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (86, 2, N'Mobile Number_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (86, 3, N'Mobile Number_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (87, 0, N'Branch Card Stock Management Report')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (87, 1, N'Branch Card Stock Management Report_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (87, 2, N'Branch Card Stock Management Report_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (87, 3, N'Branch Card Stock Management Report_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (88, 0, N'Production Date')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (88, 1, N'Production Date_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (88, 2, N'Production Date_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (88, 3, N'Production Date_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (89, 0, N'Full Fee')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (89, 1, N'Full Fee_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (89, 2, N'Full Fee_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (89, 3, N'Full Fee_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (90, 0, N'Amended Fee')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (90, 1, N'Amended Fee_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (90, 2, N'Amended Fee_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (90, 3, N'Amended Fee_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (91, 0, N'Zero / No Fee')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (91, 1, N'Zero / No Fee_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (91, 2, N'Zero / No Fee_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (91, 3, N'Zero / No Fee_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (92, 0, N'180 d')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (92, 1, N'180 d_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (92, 2, N'180 d_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (92, 3, N'180 d_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (93, 0, N'90 d')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (93, 1, N'90 d_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (93, 2, N'90 d_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (93, 3, N'90 d_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (94, 0, N'Week 4')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (94, 1, N'Week 4_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (94, 2, N'Week 4_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (94, 3, N'Week 4_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (95, 0, N'Week 3')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (95, 1, N'Week 3_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (95, 2, N'Week 3_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (95, 3, N'Week 3_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (96, 0, N'Week 2')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (96, 1, N'Week 2_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (96, 2, N'Week 2_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (96, 3, N'Week 2_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (97, 0, N'Week 1')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (97, 1, N'Week 1_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (97, 2, N'Week 1_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (97, 3, N'Week 1_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (98, 0, N'Current Cards Stock')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (98, 1, N'Current Cards Stock_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (98, 2, N'Current Cards Stock_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (98, 3, N'Current Cards Stock_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (99, 0, N'Weeks Remaining')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (99, 1, N'Weeks Remaining_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (99, 2, N'Weeks Remaining_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (99, 3, N'Weeks Remaining_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (100, 0, N'Fee Revenue Report')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (100, 1, N'Fee Revenue Report_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (100, 2, N'Fee Revenue Report_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (100, 3, N'Fee Revenue Report_es')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (101, 0, N'Burn Rate Report')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (101, 1, N'Burn Rate Report_fr')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (101, 2, N'Burn Rate Report_pt')
GO	
INSERT [dbo].[reportfields_language] ([reportfieldid], [language_id], [language_text]) VALUES (101, 3, N'Burn Rate Report_es')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (1, N'Load Batch Report')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (2, N'Distribution Batch Report')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (3, N'Card Report')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (4, N'Audit Log Report')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (5, N'Pin Batch Report')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (6, N'Export Batch Report')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (7, N'Spoiled cards report')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (8, N'Issued cards report')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (9, N'Issued cards summary report')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (11, N'Inventory Summary Report')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (12, N'Audit Report – Branches Per User Group')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (13, N'Audit Report – Users Per User Role')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (14, N'Audit Report – User Groups')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (15, N'PinMailerReport')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (17, N'PIN Re-issue Report')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (18, N'CardDispatchReport')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (19, N'CardExpiryReport')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (20, N'CardProductionReport')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (21, N'BranchOrderReport')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (22, N'Branch Card Stock Management Report')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (23, N'Fee Revenue Report')
GO	
INSERT [dbo].[reports] ([Reportid], [ReportName]) VALUES (24, N'Burn Rate Report')
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
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (0, 31, N'Card has been marked as PRINTED. <br />Please click ''Upload Card Details'' to finish issuing card."', N'Cette carte a  été  marquée imprimée. Cliquez sur ''Charger les détails de la carte'' pour achever l''émission. ', N'Card has been marked as PRINTED. <br />Pleace click ''Upload Card Details'' to finish issuing card._pt', N'Card has been marked as PRINTED. <br />Pleace click ''Upload Card Details'' to finish issuing card._sp')
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
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (72, 0, N'The Authorisation Pin captured is incorrect. Please try again.', N'L''autorisation Pin capturé est incorrect . Se il vous plaît essayer à nouveau.', N'The Authorisation Pin captured is incorrect. Please try again._pt', N'The Authorisation Pin captured is incorrect. Please try again._esp')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (100, 0, N'Batch not in correct status', N'Batch not in correct status_fr', N'Batch not in correct status_pt', N'Batch not in correct status_es')
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
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (216, 0, N'Cannot delete user group, users are still linked to user group.', N'Cannot delete user group, users are still linked to user group._fr', N'Cannot delete user group, users are still linked to user group._pt', N'Cannot delete user group, users are still linked to user group._es')
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
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (223, 25, N'Duplicate sub product code,please change sub product code.', N'Duplicate sub product code,please sub product code_fr.', N'Duplicate sub product code,please change sub product code_pt', N'Duplicate sub product code,please change sub product code_es')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (223, 26, N'Duplicate sub product code,please change sub product code.', N'Duplicate sub product code,please sub product code_fr.', N'Duplicate sub product code,please change sub product code_pt', N'Duplicate sub product code,please change sub product code_es')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (224, 25, N'Duplicate sub product id , please change sub product id', N'Duplicate sub productid,please change sub product id_fr', N'Duplicate sub product id,Please change sub product id_pt', N'Duplicate sub product id,please change sub product id _es.')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (224, 26, N'Duplicate sub product id , please change sub product id', N'Duplicate sub productid,please change sub product id_fr', N'Duplicate sub product id,Please change sub product id_pt', N'Duplicate sub product id,please change sub product id _es.')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (225, 0, N'Duplicate LDAP Setting name, please change the name.', N'Nom du réglage LDAP est dupliqué. Veuillez changer le nom.', N'Duplicate LDAP Setting name, please change the name_pt', N'Duplicate LDAP Setting name, please change the name_sp')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (226, 0, N'Duplicate Fee Scheme Name, please change name.', N'Duplicate Fee Scheme Name, please change name._fr', N'Duplicate Fee Scheme Name, please change name._pt', N'Duplicate Fee Scheme Name, please change name._es')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (226, 25, N'Duplicate sub product name , please change sub product name', N'Duplicate sub productid,please change sub product name_fr', N'Duplicate sub product id,Please change sub product name_pt', N'Duplicate sub product id,please change sub product name_es.')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (226, 26, N'Duplicate sub product name , please change sub product name', N'Duplicate sub productid,please change sub product name_fr', N'Duplicate sub product id,Please change sub product name_pt', N'Duplicate sub product id,please change sub product name_es.')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (227, 0, N'Duplicate Fee Detail Name, please change name..', N'Duplicate Fee Detail Name, please change name._fr', N'Duplicate Fee Detail Name, please change name._pt', N'Duplicate Fee Detail Name, please change name._es')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (227, 25, N'selected product linked to cards ,you cannot add sub product.', N'selected product linked to cards ,you cannot add sub product_fr', N'selected product linked to cards ,you cannot add sub product_pt', N'selected product linked to cards ,you cannot add sub product__sp')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (228, 0, N'Parameter already in use on another file interface, please use a different parameter.', N'Parameter already in use on another file interface, please use a different parameter._fr', N'Parameter already in use on another file interface, please use a different parameter._pt', N'Parameter already in use on another file interface, please use a different parameter._es')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (228, 25, N'Parameter already in use on another file interface, please use a different parameter.', N'Parameter already in use on another file interface, please use a different parameter._fr', N'Parameter already in use on another file interface, please use a different parameter._pt', N'Parameter already in use on another file interface, please use a different parameter._es')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (228, 26, N'Parameter already in use on another file interface, please use a different parameter.', N'Parameter already in use on another file interface, please use a different parameter._fr', N'Parameter already in use on another file interface, please use a different parameter._pt', N'Parameter already in use on another file interface, please use a different parameter._es')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (400, 0, N'There are no card requests available to create the batch.', N'There are no card requests available to create the batch._fr', N'There are no card requests available to create the batch._pt', N'There are no card requests available to create the batch._es')
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
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (507, 0, N'The customer has insuffiecient funds for card fees.', N'Le client dispose de fonds insuffiecient pour les frais de carte.', N'The customer has insuffiecient funds for card fees.', N'The customer has insuffiecient funds for card fees.')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (600, 0, N'Issuer does not have a valid licence.', N'La filliale n''a pas de licence valide.', N'Issuer does not have a valid licence_pt', N'Issuer does not have a valid licence_sp')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (601, 0, N'There was a problem processing the issuers licence, it may be invalid.', N'Une erreur est survenue lors du traitement de la licence de la filliale. Il se peut qu''elle soit invalide.', N'There was a problem processing the issuers licence, it may be invalid_pt', N'There was a problem processing the issuers licence, it may be invalid_sp')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (602, 0, N'Issuer licence has expired.', N'Licence de la filliale a expiré.', N'Issuer licence has expired_pt', N'Issuer licence has expired_sp')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (603, 0, N'An error has occured while processing the issuers licence, please contact support.', N'Une erreur est survenue lors du traitement de la license de la filliale. Contactez l''équipe de support.', N'An error has occured while processing the issuers licence, please contact support_pt', N'An error has occured while processing the issuers licence, please contact support_sp')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (604, 0, N'Duplicate Terminal name', N'Duplicate Terminal name_fr', N'Duplicate Terminal name_pt', N'Duplicate Terminal name_es')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (605, 0, N'Duplicate Device Number', N'Duplicate Device Number_fr', N'Duplicate Device Number_pt', N'Duplicate Device Number_es')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (606, 0, N'Duplicate Master Key', N'Duplicate Master Key_fr', N'Duplicate Master Key_pt', N'Duplicate Master Key_es')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (607, 0, N'Duplicate Master Key Name', N'Duplicate Master Key Name_fr', N'Duplicate Master Key Name_pt', N'Duplicate Master Key Name_es')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (608, 0, N'Cannot delete master key. Key in use on terminals.', N'Cannot delete master key. Key is used on terminal_fr.', N'Cannot delete master key. Key is used on terminal._pt', N'Cannot delete master key. Key is used on terminal._sp')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (702, 0, N'Cannot delete fee scheme, products still linked to fee scheme.', N'Cannot delete fee scheme, products still linked to fee scheme._fr', N'Cannot delete fee scheme, products still linked to fee scheme._pt', N'Cannot delete fee scheme, products still linked to fee scheme._es')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (703, 0, N'Cannot delete fee scheme, sub-products still linked to fee scheme.', N'Cannot delete fee scheme, sub-products still linked to fee scheme._fr', N'Cannot delete fee scheme, sub-products still linked to fee scheme._pt', N'Cannot delete fee scheme, sub-products still linked to fee scheme._es')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (800, 0, N'PIN Reissue not in correct status.', N'PIN Reissue not in correct status._fr', N'PIN Reissue not in correct status._pt', N'PIN Reissue not in correct status._es')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (801, 0, N'PIN Reissue request expired, please restart pin request.', N'PIN Reissue request expired, please restart pin request._fr', N'PIN Reissue request expired, please restart pin request._pt', N'PIN Reissue request expired, please restart pin request._es')
GO	
INSERT [dbo].[response_messages] ([system_response_code], [system_area], [english_response], [french_response], [portuguese_response], [spanish_response]) VALUES (802, 0, N'PIN Reissue already requested for card, please complete request.', N'PIN Reissue already requested for card, please complete request._fr', N'PIN Reissue already requested for card, please complete request._pt', N'PIN Reissue already requested for card, please complete request._es')
GO	

SET IDENTITY_INSERT [dbo].[user_gender] ON 
INSERT [dbo].[user_gender] ([user_gender_id], [user_gender_text]) VALUES (1, N'MALE')
INSERT [dbo].[user_gender] ([user_gender_id], [user_gender_text]) VALUES (2, N'FEMALE')
INSERT [dbo].[user_gender] ([user_gender_id], [user_gender_text]) VALUES (3, N'UNSPECIFIED')
SET IDENTITY_INSERT [dbo].[user_gender] OFF
GO	


INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (0, N'CONFIG_ADMIN', 0, 0)
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
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (12, N'CMS_OPERATOR', 0, 0)
GO	
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (13, N'PIN_PRINTER_OPERATOR', 0, 0)
GO	
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (14, N'CARD_CENTRE_PIN_OFFICER', 0, 0)
GO	
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (15, N'BRANCH_PIN_OFFICER', 0, 0)
GO	
INSERT [dbo].[user_roles] ([user_role_id], [user_role], [allow_multiple_login], [enterprise_only]) VALUES (16, N'REPORT_ADMIN', 0, 0)
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (0, 0, N'CONFIG_ADMIN')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (0, 1, N'CONFIG_ADMIN_fr')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (0, 2, N'CONFIG_ADMIN_pt')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (0, 3, N'CONFIG_ADMIN_es')
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
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (12, 0, N'CMS_OPERATOR')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (12, 1, N'CMS_OPERATOR_fr')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (12, 2, N'CMS_OPERATOR_pt')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (12, 3, N'CMS_OPERATOR_es')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (13, 0, N'PIN_PRINTER_OPERATOR')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (13, 1, N'PIN_PRINTER_OPERATOR_fr')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (13, 2, N'PIN_PRINTER_OPERATOR_pt')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (13, 3, N'PIN_PRINTER_OPERATOR_es')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (14, 0, N'CARD_CENTRE_PIN_OFFICER')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (14, 1, N'CARD_CENTRE_PIN_OFFICER_fr')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (14, 2, N'CARD_CENTRE_PIN_OFFICER_pt')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (14, 3, N'CARD_CENTRE_PIN_OFFICER_es')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (15, 0, N'BRANCH_PIN_OFFICER')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (15, 1, N'BRANCH_PIN_OFFICER_fr')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (15, 2, N'BRANCH_PIN_OFFICER_pt')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (15, 3, N'BRANCH_PIN_OFFICER_es')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (16, 0, N'REPORT_ADMIN')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (16, 1, N'REPORT_ADMIN_fr')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (16, 2, N'REPORT_ADMIN_pt')
GO	
INSERT [dbo].[user_roles_language] ([user_role_id], [language_id], [language_text]) VALUES (16, 3, N'REPORT_ADMIN_es')

INSERT [dbo].[pin_calc_methods] ([pin_calc_method_id], [pin_calc_method_name]) VALUES (0, N'VISA Method')
GO	
INSERT [dbo].[pin_calc_methods] ([pin_calc_method_id], [pin_calc_method_name]) VALUES (1, N'IBM Method')
GO	





declare @objid int
SET @objid = object_id('cards')
EXEC	[dbo].[AddMacForTable]
		@Table_id = @objid
GO	

declare @objid int
SET @objid = object_id('user')
EXEC	[dbo].[AddMacForTable]
		@Table_id = @objid
GO	