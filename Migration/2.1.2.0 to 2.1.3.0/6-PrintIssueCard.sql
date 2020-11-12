USE [indigo_database_main_dev]
GO

ALTER TABLE [issuer_product]
ADD print_issue_card_YN bit not null default 1
GO