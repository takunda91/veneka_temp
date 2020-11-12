CREATE PROCEDURE [dbo].[usp_sys_encrypt] @clearText varchar(max)
As
BEGIN
OPEN Symmetric Key Indigo_Symmetric_Key
DECRYPTION BY Certificate Indigo_Certificate;
SELECT CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@clearText)))
CLOSE Symmetric Key Indigo_Symmetric_Key;
END