CREATE PROCEDURE [dbo].[usp_get_workstation_key]
	@workstation nvarchar(500)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	OPEN Symmetric Key Indigo_Symmetric_Key
			DECRYPTION BY Certificate Indigo_Certificate;
    -- Insert statements for procedure here
	SELECT   CONVERT(VARCHAR(100),DECRYPTBYKEY([key])) as 'key' ,
	   CONVERT(VARCHAR(100),DECRYPTBYKEY(additional_data)) as 'aData'
	   --,CONVERT(VARCHAR(100),DECRYPTBYKEY(workstation)) as 'workstation' 
	   from workstation_keys
	where CONVERT(VARCHAR(100),DECRYPTBYKEY(workstation))=@workstation
		CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END
