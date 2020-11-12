-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Updates user group
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_user_group] 
	@user_group_id int,
	@user_group_name varchar(50),
	@user_role_id int,
	@can_read bit,
	@can_create bit,
	@can_update bit,
	@mask_screen_pan bit = 1,
	@mask_report_pan bit = 1,
	@issuer_id int,
	@all_branch_access bit,
	@branch_list AS dbo.branch_id_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Declare @checkoutcards int
	select @checkoutcards=count(cards.card_id) from cards
						INNER JOIN [branch_card_status_current]
						ON [cards].card_id = [branch_card_status_current].card_id
						 where  [cards].branch_id  in (select [user_groups_branches].branch_id
													FROM [user_groups_branches] 
													where 	[user_groups_branches].user_group_id=@user_group_id)
							and  [branch_card_status_current].branch_card_statuses_id =1
							and   [cards].card_issue_method_id =1
	

			--Check for duplicate's
	if (@checkoutcards>0)
		BEGIN
			SET @ResultCode = 803						
		END
	IF (SELECT COUNT(*) FROM [user_group] WHERE ([user_group_name] = @user_group_name AND [issuer_id] = @issuer_id) AND user_group_id != @user_group_id) > 0
		BEGIN
			SET @ResultCode = 215						
		END
	ELSE
		BEGIN
			BEGIN TRANSACTION [UPDATE_USER_GROUP_TRAN]
			BEGIN TRY 

		DECLARE @RC int
		UPDATE [user_group]
        SET [user_role_id] = @user_role_id, 
			[issuer_id] = @issuer_id, 
			[can_create] = @can_create, 
			[can_read] = @can_read, 
			[can_update] = @can_update, 
			[mask_screen_pan] = @mask_screen_pan,
			[mask_report_pan] = @mask_report_pan,
			[can_delete] = 0,
            [all_branch_access] = @all_branch_access, 
			[user_group_name] = @user_group_name
		WHERE user_group_id = @user_group_id

		--Delete any linked branches so they may be inserted in the next step.
		DELETE FROM [user_groups_branches]
		WHERE user_group_id = @user_group_id

		--Link branches to user group
		EXECUTE @RC = [usp_insert_user_group_branches] @user_group_id, @branch_list, @audit_user_id, @audit_workstation
		

		--Insert audit
		DECLARE @branches varchar(max),
				@user_role_name varchar(50),
				@issuer_code varchar(10)

		IF (@all_branch_access = 0)
			BEGIN
				SELECT  @branches = STUFF(
									(SELECT ', ' +b.[branch_code] + ';' + cast(b.[branch_id] as varchar(max)) 
									 FROM user_groups_branches ug
										INNER JOIN [branch] b 
											ON ug.[branch_id] = b.[branch_id]
										WHERE ug.user_group_id = @user_group_id
										FOR XML PATH(''))
								   , 1
								   , 1
								   , '')
			END
		ELSE
			BEGIN
				SELECT  @branches = STUFF(
									(SELECT ', ' + [branch_code] + ';' + cast([branch_id] as varchar(max))
									 FROM [branch]
									 WHERE issuer_id = @issuer_id
									 FOR XML PATH(''))
								   , 1
								   , 1
								   , '')
			END

		SELECT @user_role_name = user_role
		FROM [user_roles]
		WHERE @user_role_id = @user_role_id

		SELECT @issuer_code = issuer_code
		FROM [issuer]
		WHERE issuer_id = @issuer_id

		DECLARE @audit_description varchar(max)
		SET @audit_description = 'Update: ' + COALESCE(@user_group_name, 'UNKNOWN') +
								 ', iss:' + COALESCE(@issuer_code, 'UNKNOWN') + ';' + COALESCE(CAST(@issuer_id as varchar(max)), 'UNKNOWN') + 
								 ', read: ' + COALESCE(CAST(@can_read as varchar(1)), 'UNKNOWN') + 
								 ', create: ' + COALESCE(CAST(@can_create as varchar(1)), 'UNKNOWN') +
								 ', update: ' + COALESCE(CAST(@can_update as varchar(1)), 'UNKNOWN') +
								 ', branches: ' + COALESCE(@branches, 'UNKNOWN')

		EXEC usp_insert_audit @audit_user_id, 
							 8,----UserGroupAdmin
							 NULL, 
							 @audit_workstation, 
							 @audit_description, 
							 @issuer_id, NULL, NULL, NULL

		SET @ResultCode = 0
		COMMIT TRANSACTION [UPDATE_USER_GROUP_TRAN]

	END TRY
		BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_USER_GROUP_TRAN]
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
END