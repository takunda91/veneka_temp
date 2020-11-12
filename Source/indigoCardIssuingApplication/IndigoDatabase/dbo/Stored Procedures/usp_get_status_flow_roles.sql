-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_status_flow_roles] 
	@role_list AS dbo.key_value_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [dist_batch_statuses_flow].*, [dist_batch_status_flow].*
	FROM [dist_batch_status_flow] INNER JOIN [dist_batch_statuses_flow]
			ON [dist_batch_status_flow].dist_batch_status_flow_id = [dist_batch_statuses_flow].dist_batch_status_flow_id
		INNER JOIN @role_list roles
			ON  [dist_batch_statuses_flow].user_role_id = roles.[key]
END