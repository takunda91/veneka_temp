-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Insert link between users group and branches
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_user_group_branches] 
	-- Add the parameters for the stored procedure here
	@user_group_id int,
	@branch_list AS dbo.branch_id_array READONLY,
	@audit_user_id BIGINT,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--BEGIN TRANSACTION [INSERT_USER_GROUP_BRANCHES_TRAN]

	--	BEGIN TRY

			INSERT INTO user_groups_branches
						(user_group_id, branch_id)
			SELECT @user_group_id, bl.branch_id
			FROM @branch_list bl

			--Insert audit train
			--EXEC usp_insert_audit @audit_user_id, 
			--						 21,
			--						 NULL, 
			--						 @audit_workstation, 
			--						 'Linking branches to user group.', 
			--						 NULL, NULL, NULL, NULL

	--		COMMIT TRANSACTION [INSERT_USER_GROUP_BRANCHES_TRAN]

	--	END TRY
	--BEGIN CATCH
	--	ROLLBACK TRANSACTION [INSERT_USER_GROUP_BRANCHES_TRAN]
	--	DECLARE @ErrorMessage NVARCHAR(4000);
	--	DECLARE @ErrorSeverity INT;
	--	DECLARE @ErrorState INT;

	--	SELECT 
	--		@ErrorMessage = ERROR_MESSAGE(),
	--		@ErrorSeverity = ERROR_SEVERITY(),
	--		@ErrorState = ERROR_STATE();

	--	RAISERROR (@ErrorMessage, -- Message text.
	--			   @ErrorSeverity, -- Severity.
	--			   @ErrorState -- State.
	--			   );
	--END CATCH 	

END