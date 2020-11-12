/*
RAB: Have modified the script to only pull through lookup tables and write unicode to file

===================================================
	Generate MERGE Statements for All Lookups
===================================================
Copyright:	Eitan Blumin (C) 2012
Email:		eitan@madeira.co.il
Source:		www.madeira.co.il
Disclaimer:
	The author is not responsible for any damage this
	script or any of its variations may cause.
	Do not execute it or any variations of it on production
	environments without first verifying its validity
	on controlled testing and/or QA environments.
	You may use this script at your own risk and may change it
	to your liking, as long as you leave this disclaimer header
	fully intact and unchanged.

Description:
	This script finds all the tables in the database (only such with a primary key)
	and generates a .sql file for each table, which contains a MERGE statement
	using all the records currently present in the table.
	Upon execution of these generated scripts, they will "initialize" the table
	to only contain the records that were present at the time of the script generation.
	
Instructions:
	There's no need to change anything except the values in
	the Configuration Area at the top of this script.
	Any other changes could change the behaviour of this script
	and will be done at your own risk.
	In any case, this script is commented as much as possible
	to help you understand how it works.
	
Known Issues:
	Due to the use of MERGE statements and VALUES constructor, these scripts will only
	work in SQL Server version 2008 or higher.
	
	If a MERGE statement is executed for a table which tries to delete rows, and these rows
	are referenced by another table using a foreign key WITHOUT ON DELETE CASCADE,
	then the execution will fail. For these scripts to work properly, all foreign keys
	must have ON DELETE CASCADE turned on.
	
	If at the time of the MERGE script generation, any tables have no rows,
	The script file contents for them will be a single DELETE statement which deletes all records.
	
	Columns with the IMAGE data type may cause conversion errors.
*/

---------------- Start of Configuration Area ----------------

DECLARE
	-- Target path where to save your .sql files (must be ended with a slash \):
	@OutputFolderPath	NVARCHAR(4000)	= N'C:\veneka\Temp\',

	-- Set the DB Project folder for Lookup data
	@LookupDataPath NVARCHAR(4000) = N':r .\',
	
	-- Run order bcp command. Change the database!
	@bcpCommand NVARCHAR(1000) = 'bcp "SELECT script FROM IndigoDatabaseRAB.dbo.run_order ORDER BY order_id" queryout ',

	-- BCP template for executing the MERGE statement generation procedure:
	-- Change this to connect to the desired server using a working username and password (or use -T for trusted connection)
	-- For more information on BCP please visit: http://msdn.microsoft.com/en-us/library/ms162802.aspx
	/* This "template" uses placeholders for several changing variables (later replaced with values using REPLACE).
	Each of these placeholders must not be removed or changed from this template:
		{CurrTable}
		{CurrSchema}
		{FileName}
	*/
	@BCPTemplate		NVARCHAR(4000)	= N'bcp "EXEC IndigoDatabaseRAB.dbo.usp_Generate_Merge_For_Table ''{CurrTable}'', ''{CurrSchema}''" queryout "{FileName}" -T -S -w'


---------------- End of Configuration Area ----------------
-------- Changes below this line are at your own risk --------

SET NOCOUNT ON;

-- Variable declaration
DECLARE
	@CurrSchema		SYSNAME,
	@CurrTable		SYSNAME,
	@CurrMergeStmnt	NVARCHAR(MAX),
	@FileName		NVARCHAR(4000),
	@CMD			NVARCHAR(4000)

-- Initialization of a cursor to traverse all the tables in the database
-- The tables are selected using a hierarchical query to conform with correct order
-- that the data should be inserted due to foreign key constraints.
-- (parent table will be generated before child table)
DECLARE ProcessTables CURSOR FOR
WITH reftree
AS
(
		SELECT
			ObjectId			= ReferencingTables.object_id ,
			SchemaName			= OBJECT_SCHEMA_NAME(ReferencingTables.object_id) ,
			TableName			= ReferencingTables.name ,
			Depth				= 1
		FROM
			sys.tables AS ReferencingTables
		LEFT OUTER JOIN
			sys.foreign_keys AS ForeignKeys
		ON
			ReferencingTables.object_id = ForeignKeys.parent_object_id
		AND
			ReferencingTables.object_id != ForeignKeys.referenced_object_id
		WHERE
			ForeignKeys.object_id IS NULL
		AND
			ReferencingTables.is_ms_shipped = 0
		-- Only get tables with a primary key
		AND EXISTS (
			SELECT NULL
			FROM sys.indexes AS ind
			WHERE
				ind.object_id = ReferencingTables.object_id
			AND ind.is_primary_key = 1
		)
			
		UNION ALL
		
		SELECT
			ObjectId			= ReferencingTables.object_id ,
			SchemaName			= OBJECT_SCHEMA_NAME(ReferencingTables.object_id) ,
			TableName			= ReferencingTables.name ,
			Depth				= TableHierarchy.Depth + 1
		FROM
			sys.tables AS ReferencingTables
		INNER JOIN
			sys.foreign_keys AS ForeignKeys
		ON
			ReferencingTables.object_id = ForeignKeys.parent_object_id
		AND
			ReferencingTables.object_id != ForeignKeys.referenced_object_id
		INNER JOIN
			reftree AS TableHierarchy
		ON
			ForeignKeys.referenced_object_id = TableHierarchy.ObjectId
		WHERE 
		-- Only get tables with a primary key
		EXISTS (
			SELECT NULL
			FROM sys.indexes AS ind
			WHERE
				ind.object_id = ReferencingTables.object_id
			AND ind.is_primary_key = 1
			)
)
SELECT
	SchemaName, TableName
FROM reftree
WHERE ObjectId IN (SELECT ep.major_id FROM sys.extended_properties as ep WHERE ep.name = 'VENEKA_TABLE_TYPE' AND ep.value = 'lookup')
GROUP BY
	ObjectId, SchemaName, TableName
ORDER BY
	MAX(Depth) ASC
;

-- Declare script run order table
IF OBJECT_ID('dbo.run_order', 'U') IS NOT NULL
BEGIN
	PRINT 'Dropping existing run_order...'
	DROP TABLE dbo.run_order
END

CREATE TABLE [dbo].[run_order]
(
    [order_id] INT IDENTITY(1, 1) NOT NULL,
    [script] NVARCHAR(MAX) NOT NULL
);

--Add Disclaimer to run order
INSERT INTO [dbo].[run_order] VALUES ('/*')
INSERT INTO [dbo].[run_order] VALUES ('Post-Deployment Lookup Data Table Script')
INSERT INTO [dbo].[run_order] VALUES ('--------------------------------------------------------------------------------------')
INSERT INTO [dbo].[run_order] VALUES ('Scripts and Order was generated by scrip ''gen_merge_for_all_lookups.sql''')
INSERT INTO [dbo].[run_order] VALUES ('Run order is based on primary key hierarchy. ')
INSERT INTO [dbo].[run_order] VALUES ('Manually changing or adding order could cause errors during deployment')
INSERT INTO [dbo].[run_order] VALUES ('--------------------------------------------------------------------------------------')
INSERT INTO [dbo].[run_order] VALUES ('*/')

DECLARE @RunOrder AS TABLE(Script NVARCHAR(MAX));

-- Open cursor
OPEN ProcessTables

-- Fetch first row from cursor
FETCH NEXT FROM ProcessTables INTO @CurrSchema, @CurrTable

-- While a row is returned from the cursor
WHILE @@FETCH_STATUS = 0
BEGIN
	-- Display a message with the current table name
	PRINT ''
	RAISERROR(N'-------------- %s --------------', 0, 1, @CurrTable) WITH NOWAIT;
	PRINT ''
	
	-- Initialize full file path based on table and schema names
	SET @FileName = @OutputFolderPath + @CurrSchema + '.' + @CurrTable + '.sql'
	
	-- Replace placeholders with current settings
	SET @CMD = REPLACE(@BCPTemplate, '{CurrTable}',REPLACE(@CurrTable,'''',''''''))
	SET @CMD = REPLACE(@CMD, '{CurrSchema}', REPLACE(@CurrSchema,'''',''''''))
	SET @CMD = REPLACE(@CMD, '{FileName}', @FileName)
	
	PRINT 'Saving to ' + @FileName
	PRINT @CMD
	
	DECLARE @Output AS TABLE(Msg NVARCHAR(MAX));
	DECLARE @Msg NVARCHAR(MAX);
	
	-- Execute the BCP command using CMDSHELL and save output in a variable table
	INSERT INTO @Output
	EXEC xp_cmdshell @CMD
	
	-- Insert Script into run order
	INSERT INTO dbo.run_order VALUES (@LookupDataPath + @CurrSchema + '.' + @CurrTable + '.sql')

	-- Concatenate the output from the table into a string variable and print it
	SELECT
		-- This will concatenate all messages with a new line (char(10)) between them.
		@Msg = ISNULL(@Msg + CHAR(10),'') + Msg
	FROM
		@Output
	WHERE
		Msg IS NOT NULL
	
	RAISERROR(@Msg,0,1) WITH NOWAIT;
	
	-- Reset output table and variable
	DELETE @Output;
	SET @Msg = NULL;
	
	-- Fetch the next row from cursor
	FETCH NEXT FROM ProcessTables INTO @CurrSchema, @CurrTable
END

-- Close and destroy the cursor object
CLOSE ProcessTables
DEALLOCATE ProcessTables


-- Write run order out
DECLARE @OutputFile NVARCHAR(100) ,    @FilePath NVARCHAR(100)
SET @OutputFile = '0_run_order.sql'
SET @bcpCommand = @bcpCommand + @OutputFolderPath + @OutputFile + ' -c -t, -T -S'+ @@servername

DECLARE @Output2 AS TABLE(Msg NVARCHAR(MAX));
DECLARE @Msg2 NVARCHAR(MAX);

INSERT INTO @Output2
EXEC xp_cmdshell @bcpCommand

-- Display a message with the current table name
PRINT ''
RAISERROR(N'-------------- run_order.sql --------------', 0, 1) WITH NOWAIT;
PRINT ''
-- Concatenate the output from the table into a string variable and print it
SELECT
	-- This will concatenate all messages with a new line (char(10)) between them.
	@Msg2 = ISNULL(@Msg2 + CHAR(10),'') + Msg
FROM
	@Output2
WHERE
	Msg IS NOT NULL
	
RAISERROR(@Msg2,0,1) WITH NOWAIT;

IF OBJECT_ID('dbo.run_order', 'U') IS NOT NULL
BEGIN
	PRINT 'Dropping existing run_order...'
	DROP TABLE dbo.run_order
END

--select * from run_order