CREATE PROCEDURE [dbo].[usp_remote_get_token]
	@remote_token uniqueidentifier,
	@audit_user_id bigint,
	@audit_workstation varchar(150)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	

	SELECT [issuer_id], [issuer_code], [issuer_name], [issuer_status_id], [remote_token_expiry]
	FROM [dbo].[issuer]
	WHERE [remote_token] = @remote_token
