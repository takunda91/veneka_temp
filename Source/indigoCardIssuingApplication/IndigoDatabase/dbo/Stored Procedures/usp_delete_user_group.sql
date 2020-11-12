-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_user_group] 
	-- Add the parameters for the stored procedure here
	@user_group_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [DELETE_USER_GROUP_TRAN]
		BEGIN TRY 

			IF((SELECT COUNT(*) FROM [users_to_users_groups] WHERE user_group_id = @user_group_id) > 0)
				BEGIN
					SET @ResultCode = 216
				END
			ELSE
				BEGIN
					DELETE FROM [user_groups_branches]
					WHERE user_group_id = @user_group_id

					DELETE FROM [users_to_users_groups]
					WHERE user_group_id = @user_group_id

					DELETE FROM [user_group]
					WHERE user_group_id = @user_group_id
					SET @ResultCode = 0
				END


			COMMIT TRANSACTION [DELETE_USER_GROUP_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_USER_GROUP_TRAN]
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();

		RAISERROR (@ErrorMessage, -- Message text.
				   @ErrorSeverity, -- Severity.
				   @ErrorState -- State.
				   );
	END CATCH 
END