USE [indigo_database_main_dev]
GO

--Create card priority Table
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[connection_parameter_type](
	[connection_parameter_type_id] [int] NOT NULL,
	[connection_parameter_type_name] [varchar](50) NOT NULL
 CONSTRAINT [PK_connection_parameter_type] PRIMARY KEY CLUSTERED 
(
	[connection_parameter_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

--Add card statuses
INSERT INTO [connection_parameter_type] (connection_parameter_type_id, connection_parameter_type_name)
	VALUES (0, 'WEBSERVICE')
INSERT INTO [connection_parameter_type] (connection_parameter_type_id, connection_parameter_type_name)
	VALUES (1, 'FILE')

GO

CREATE TABLE [connection_parameter_type_language]
	([connection_parameter_type_id] int NOT NULL
		REFERENCES [connection_parameter_type]([connection_parameter_type_id]),
	 [language_id] int NOT NULL
		REFERENCES [languages](id),
	 [language_text] nvarchar(max) NOT NULL,
	 PRIMARY KEY ([connection_parameter_type_id], [language_id]))

GO 
--id	language_name
--0	English
--1	French
--2	Portuguese
--3	Spanish

INSERT INTO [connection_parameter_type_language]
	([connection_parameter_type_id], [language_id], [language_text])
SELECT [connection_parameter_type_id], 0, [connection_parameter_type_name]
FROM [connection_parameter_type] 

GO

INSERT INTO [connection_parameter_type_language]
	([connection_parameter_type_id], [language_id], [language_text])
SELECT [connection_parameter_type_id], 1, [connection_parameter_type_name] + '_fr'
FROM [connection_parameter_type] 
GO

INSERT INTO [connection_parameter_type_language]
	([connection_parameter_type_id], [language_id], [language_text])
SELECT [connection_parameter_type_id], 2, [connection_parameter_type_name] + '_pt'
FROM [connection_parameter_type] 
GO

INSERT INTO [connection_parameter_type_language]
	([connection_parameter_type_id], [language_id], [language_text])
SELECT [connection_parameter_type_id], 3, [connection_parameter_type_name] + '_sp'
FROM [connection_parameter_type] 
GO


------------------------------------------------------------------------------------------------------------
---------------------------------------- UPDATE CARDS TABLE COLUMNS ----------------------------------------
------------------------------------------------------------------------------------------------------------
ALTER TABLE [connection_parameters]
	ADD [connection_parameter_type_id] int NULL
		REFERENCES [connection_parameter_type] ([connection_parameter_type_id])