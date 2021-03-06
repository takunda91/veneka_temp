

MERGE INTO [dbo].[reports] AS trgt
USING	(VALUES
		(1,N'Load Batch Report'),
		(2,N'Distribution Batch Report'),
		(3,N'Card Report'),
		(4,N'Audit Log Report'),
		(5,N'Pin Batch Report'),
		(6,N'Export Batch Report'),
		(7,N'Spoiled cards report'),
		(8,N'Issued cards report'),
		(9,N'Issued cards summary report'),
		(10,N'Spoil cards summary report'),
		(11,N'Inventory Summary Report'),
		(12,N'Audit Report – Branches Per User Group'),
		(13,N'Audit Report – Users Per User Role'),
		(14,N'Audit Report – User Groups'),
		(15,N'PinMailerReport'),
		(16,N'Pin Mailer Re-Print Report'),
		(17,N'PIN Re-issue Report'),
		(18,N'CardDispatchReport'),
		(19,N'CardExpiryReport'),
		(20,N'CardProductionReport'),
		(21,N'BranchOrderReport'),
		(22,N'Branch Card Stock Management Report'),
		(23,N'Fee Revenue Report'),
		(24,N'Burn Rate Report'),
		(25,N'Cards InProgress Report'),
		(26,N'CMS Error Report'),
		(27,N'Fee Status Report'),
		(28,N'Print Batch Report')
		) AS src([Reportid],[ReportName])
ON
	trgt.[Reportid] = src.[Reportid]
WHEN MATCHED THEN
	UPDATE SET
		[Reportid] = src.[Reportid]
		, [ReportName] = src.[ReportName]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Reportid],[ReportName])
	VALUES ([Reportid],[ReportName])

;

