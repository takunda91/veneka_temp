USE [indigo_database_main_dev]
GO

--------------------------------------------------------------------------------------------------------------------------
--------- UPDATE dist batch to indicate which issue method it is for and which cards may be contained within it ----------
--------------------------------------------------------------------------------------------------------------------------
ALTER TABLE [dist_batch]
	ADD card_issue_method_id int NOT NULL
		REFERENCES [card_issue_method] (card_issue_method_id)

GO