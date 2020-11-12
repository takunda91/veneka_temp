-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_pin_reissue]
	-- Add the parameters for the stored procedure here
	@pin_reissue_id bigint,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate

	SELECT        
		pin_reissue.issuer_id, 
		pin_reissue.branch_id, 
		pin_reissue.product_id, 
		pin_reissue.pan, 
		SWITCHOFFSET(pin_reissue.reissue_date, @UserTimezone) AS reissue_date, 
		pin_reissue.operator_user_id, 
		pin_reissue.authorise_user_id,
		pin_reissue.failed, 
		pin_reissue.notes, 
		pin_reissue.pin_reissue_id, 
		pin_reissue.primary_index_number, 
		SWITCHOFFSET(pin_reissue.request_expiry, @UserTimezone) AS request_expiry, 
		CASE WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX), DECRYPTBYKEY([pin_reissue].pan)), 6, 4) ELSE CONVERT(VARCHAR(MAX), DECRYPTBYKEY([pin_reissue].pan)) END AS 'card_number', 
		pin_reissue_status_current.pin_reissue_statuses_id, 
		SWITCHOFFSET(pin_reissue_status_current.status_date, @UserTimezone) AS status_date, 
		pin_reissue_status_current.[user_id], 
		pin_reissue_status_current.comments, 
		pin_reissue_statuses_language.language_text AS pin_reissue_statuses_name, 
		CONVERT(VARCHAR(MAX), DECRYPTBYKEY([user].username)) AS operator_usename, 
		CONVERT(VARCHAR(MAX), DECRYPTBYKEY(authoriser.username)) AS authorise_username, 
		issuer.issuer_name, 
		issuer.issuer_code, 
		branch.branch_code, 
		branch.branch_name, 
		issuer_product.product_code, 
		issuer_product.product_name, 
		issuer.authorise_pin_reissue_YN, 
		@UserTimezone as user_time_zone,
		[pin_reissue].pin_reissue_type_id	,	
	CONVERT(VARCHAR(max),DECRYPTBYKEY([pin_reissue].mobile_number)) as mobile_number,
		CAST(1 AS BIGINT) AS ROW_NO, 1 AS TOTAL_ROWS, 
		1 AS TOTAL_PAGES
	FROM    pin_reissue INNER JOIN
            pin_reissue_status_current ON pin_reissue.pin_reissue_id = pin_reissue_status_current.pin_reissue_id INNER JOIN
            pin_reissue_statuses_language ON pin_reissue_status_current.pin_reissue_statuses_id = pin_reissue_statuses_language.pin_reissue_statuses_id AND 
            pin_reissue_statuses_language.language_id = @language_id INNER JOIN
            [user] ON [user].user_id = pin_reissue.operator_user_id INNER JOIN
            issuer ON issuer.issuer_id = pin_reissue.issuer_id INNER JOIN
            branch ON branch.branch_id = pin_reissue.branch_id INNER JOIN
            issuer_product ON issuer_product.product_id = pin_reissue.product_id LEFT OUTER JOIN
            [user] AS authoriser ON authoriser.user_id = pin_reissue.authorise_user_id
				
	WHERE [pin_reissue].pin_reissue_id = @pin_reissue_id
			
CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key	

END
GO

