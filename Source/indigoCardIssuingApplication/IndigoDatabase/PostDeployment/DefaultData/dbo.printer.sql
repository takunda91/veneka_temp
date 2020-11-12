
SET IDENTITY_INSERT [dbo].[printer] ON;
DECLARE @serial_no NVARCHAR(100)='Unassigned'
IF NOT EXISTS (SELECT serial_no FROM [dbo].[printer] WHERE serial_no = @serial_no)
BEGIN
INSERT INTO [dbo].[printer]

           (printer_id,[serial_no]
           ,[manufacturer]
           ,[model]
           ,[firmware_version]
           ,[branch_id]
           ,[total_prints]
           ,[next_clean]
           ,[audit_user_id]
           ,[last_update_date])
     VALUES
           (-1,'Unassigned','','','',-1,'','',-1,SYSDATETIMEOFFSET())
		   END

SET IDENTITY_INSERT [dbo].[printer] OFF;

