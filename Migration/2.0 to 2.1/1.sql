USE [indigo_database_main_dev]
GO

-- USE THESE SCRIPTS TO UPDATE DATABASE FROM VERSION 2.0.0.0 TO 2.1.0.0


---------------------------------------------------------------------------------------------------------------
----------------------------------------    ADD NEW BRANCH CARD STATUS ----------------------------------------
---------------------------------------------------------------------------------------------------------------
INSERT INTO branch_card_statuses (branch_card_statuses_id, branch_card_statuses_name)
	VALUES (10, 'REQUESTED')
GO

--id	language_name
--0	English
--1	French
--2	Portuguese
--3	Spanish

INSERT INTO [branch_card_statuses_language]	(branch_card_statuses_id, [language_id], [language_text])
	VALUES (10, 0, 'REQUESTED')
INSERT INTO [branch_card_statuses_language]	(branch_card_statuses_id, [language_id], [language_text])
	VALUES (10, 1, 'REQUESTED_fr')
INSERT INTO [branch_card_statuses_language]	(branch_card_statuses_id, [language_id], [language_text])
	VALUES (10, 2, 'REQUESTED_pt')
INSERT INTO [branch_card_statuses_language]	(branch_card_statuses_id, [language_id], [language_text])
	VALUES (10, 3, 'REQUESTED_es')
GO

----------------------------------------------------------------------------------------------------------------
---------------------------------------- CREATE CARD_ISSUE_METHOD TABLE ----------------------------------------
----------------------------------------------------------------------------------------------------------------
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[card_issue_method](
	[card_issue_method_id] [int] NOT NULL,
	[card_issue_method_name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_card_issue_method] PRIMARY KEY CLUSTERED 
(
	[card_issue_method_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

INSERT INTO [card_issue_method] ([card_issue_method_id], [card_issue_method_name])
	VALUES (0, 'CLASSIC')
INSERT INTO [card_issue_method] ([card_issue_method_id], [card_issue_method_name])
	VALUES (1, 'INSTANT')
GO

CREATE TABLE [card_issue_method_language]
	([card_issue_method_id] int NOT NULL
		REFERENCES [card_issue_method]([card_issue_method_id]),
	 [language_id] int NOT NULL
		REFERENCES [languages](id),
	 [language_text] nvarchar(max) NOT NULL,
	 PRIMARY KEY ([card_issue_method_id], [language_id]))

GO 
--id	language_name
--0	English
--1	French
--2	Portuguese
--3	Spanish

INSERT INTO [card_issue_method_language]
	([card_issue_method_id], [language_id], [language_text])
SELECT [card_issue_method_id], 0, [card_issue_method_name]
FROM [card_issue_method] 

GO

INSERT INTO [card_issue_method_language]
	([card_issue_method_id], [language_id], [language_text])
SELECT [card_issue_method_id], 1, [card_issue_method_name] + '_fr'
FROM [card_issue_method] 
GO

INSERT INTO [card_issue_method_language]
	([card_issue_method_id], [language_id], [language_text])
SELECT [card_issue_method_id], 2, [card_issue_method_name] + '_pt'
FROM [card_issue_method] 
GO

INSERT INTO [card_issue_method_language]
	([card_issue_method_id], [language_id], [language_text])
SELECT [card_issue_method_id], 3, [card_issue_method_name] + '_sp'
FROM [card_issue_method] 
GO
------------------------------------------------------------------------------------------------------------
---------------------------------------- UPDATE CARDS TABLE COLUMNS ----------------------------------------
------------------------------------------------------------------------------------------------------------
ALTER TABLE [cards]
	ADD card_issue_method_id int NOT NULL
		REFERENCES [card_issue_method] (card_issue_method_id)

ALTER TABLE [cards]
	ADD card_request_reference varchar(100) NULL
	CONSTRAINT uq_card_request_reference UNIQUE(card_request_reference) 

------------------------------------------------------------------------------------------------------------
------------------------------------------- MANY-TO-MANY PRODUCTS ------------------------------------------
------------------------------------------------------------------------------------------------------------
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[issuer_product_issue_method](
	[product_id] [int] NOT NULL,
	[card_issue_method_id] [int] NOT NULL,
 CONSTRAINT [PK_issuer_product_issue_method] PRIMARY KEY CLUSTERED 
(
	[product_id] ASC,
	[card_issue_method_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[issuer_product_issue_method]  WITH CHECK ADD  CONSTRAINT [FK_issuer_product_issue_method_issuer_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO

ALTER TABLE [dbo].[issuer_product_issue_method] CHECK CONSTRAINT [FK_issuer_product_issue_method_issuer_product]
GO

ALTER TABLE [dbo].[issuer_product_issue_method]  WITH CHECK ADD  CONSTRAINT [FK_issuer_product_issue_method_issuer_product_issue_method] FOREIGN KEY([card_issue_method_id])
REFERENCES [dbo].[card_issue_method] ([card_issue_method_id])
GO

ALTER TABLE [dbo].[issuer_product_issue_method] CHECK CONSTRAINT [FK_issuer_product_issue_method_issuer_product_issue_method]
GO
