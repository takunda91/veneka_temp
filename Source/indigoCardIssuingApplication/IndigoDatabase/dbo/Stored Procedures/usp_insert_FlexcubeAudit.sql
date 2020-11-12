-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_FlexcubeAudit] 
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100),
	@audit_description varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	EXEC usp_insert_audit @audit_user_id, 
						 3,
						 NULL, 
						 @audit_workstation, 
						 @audit_description, 
						 NULL, NULL, NULL, NULL
END