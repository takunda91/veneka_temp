CREATE PROCEDURE [dbo].[usp_sys_decrypt] @cipheredText varbinary(max)
AS
BEGIN
OPEN SYMMETRIC KEY Indigo_Symmetric_Key
DECRYPTION BY CERTIFICATE Indigo_Certificate; 
--SELECT CONVERT(VARCHAR,DECRYPTBYKEY(''))
select CONVERT(varchar,DECRYPTBYKEY(@cipheredText))
CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END