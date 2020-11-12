CREATE PROCEDURE [dbo].[usp_pin_reissue_expiry_check]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE t
		SET t.pin_reissue_statuses_id = 4, 
		t.[user_id] = -2, 
		t.audit_workstation = 'SYSTEM', 
		t.comments = 'Expired', 
		t.status_date = SYSDATETIMEOFFSET()
	OUTPUT Deleted.* INTO [pin_reissue_status_audit]
	FROM [pin_reissue_status] AS t
		INNER JOIN [pin_reissue] s ON t.pin_reissue_id = s.pin_reissue_id
	WHERE s.request_expiry <= SYSDATETIMEOFFSET()
		AND t.pin_reissue_statuses_id NOT IN ( 2, 3, 4)
END
