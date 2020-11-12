CREATE PROCEDURE [dbo].[usp_remote_log_request]
	@remote_address varchar(150),
	@token varchar(max),
	@request varchar(max),
	@method_call_id int
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [dbo].[remote_component_logging] (method_call_id, remote_address, request, token)
	VALUES (@method_call_id, @remote_address, @request, @token)
