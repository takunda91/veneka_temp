USE [indigo_database_main_dev]
GO

--Create card priority Table
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[card_priority](
	[card_priority_id] [int] NOT NULL,
	[card_priority_order] [int] NOT NULL,
	[card_priority_name] [varchar](50) NOT NULL,
	[default_selection] [bit] NOT NULL,
 CONSTRAINT [PK_card_priority] PRIMARY KEY CLUSTERED 
(
	[card_priority_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

--Add card statuses
INSERT INTO [card_priority] (card_priority_id, card_priority_order, card_priority_name)
	VALUES (0,1, 'HIGH',0)
INSERT INTO [card_priority] (card_priority_id, card_priority_order, card_priority_name)
	VALUES (1,2, 'NORMAL',1)
INSERT INTO [card_priority] (card_priority_id, card_priority_order, card_priority_name)
	VALUES (2,3, 'LOW',0)
GO

CREATE TABLE [card_priority_language]
	([card_priority_id] int NOT NULL
		REFERENCES [card_priority]([card_priority_id]),
	 [language_id] int NOT NULL
		REFERENCES [languages](id),
	 [language_text] nvarchar(max) NOT NULL,
	 PRIMARY KEY ([card_priority_id], [language_id]))

GO 
--id	language_name
--0	English
--1	French
--2	Portuguese
--3	Spanish

INSERT INTO [card_priority_language]
	([card_priority_id], [language_id], [language_text])
SELECT [card_priority_id], 0, [card_priority_name]
FROM [card_priority] 

GO

INSERT INTO [card_priority_language]
	([card_priority_id], [language_id], [language_text])
SELECT [card_priority_id], 1, [card_priority_name] + '_fr'
FROM [card_priority] 
GO

INSERT INTO [card_priority_language]
	([card_priority_id], [language_id], [language_text])
SELECT [card_priority_id], 2, [card_priority_name] + '_pt'
FROM [card_priority] 
GO

INSERT INTO [card_priority_language]
	([card_priority_id], [language_id], [language_text])
SELECT [card_priority_id], 3, [card_priority_name] + '_sp'
FROM [card_priority] 
GO
------------------------------------------------------------------------------------------------------------
---------------------------------------- UPDATE CARDS TABLE COLUMNS ----------------------------------------
------------------------------------------------------------------------------------------------------------
ALTER TABLE [cards]
	ADD [card_priority_id] int NOT NULL
		REFERENCES [card_priority] ([card_priority_id])