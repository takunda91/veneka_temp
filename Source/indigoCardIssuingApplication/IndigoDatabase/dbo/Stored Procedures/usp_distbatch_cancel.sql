-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_distbatch_cancel]
	@dist_batch_id bigint,
	@dist_batch_status_id int,
	@distbatch_type_id int,
	@cardissuemethod int ,
	@status_notes varchar(150) = null,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Declare @dist_batch_statusflowid int
	Declare @previous_dis_batch_status_id int

	select @dist_batch_statusflowid = [dist_batch_status_flow_id]
		from  [dist_batch_status_flow]
		where [dist_batch_type_id]=@distbatch_type_id
		   and [card_issue_method_id]=@cardissuemethod

	select @previous_dis_batch_status_id = [dist_batch_statuses_id]
		from  [dist_batch_statuses_flow]
		where [dist_batch_status_flow_id]=@dist_batch_statusflowid and flow_dist_batch_statuses_id=@dist_batch_status_id


		BEGIN TRANSACTION [BATCH_STATUS_CHANGE]
		BEGIN TRY 
			UPDATE dist_batch_status 
					SET [dist_batch_statuses_id] = @previous_dis_batch_status_id, 
						[user_id] = @audit_user_id, 
						[status_date] = SYSDATETIMEOFFSET(), 
						[status_notes] = @status_notes	
			OUTPUT Deleted.* INTO dist_batch_status_audit
			WHERE [dist_batch_id] = @dist_batch_id

			--INSERT [dist_batch_status]
			--				([dist_batch_id], [dist_batch_statuses_id], [user_id], [status_date], [status_notes])
			--		VALUES (@dist_batch_id, @previous_dis_batch_status_id, @audit_user_id, SYSDATETIMEOFFSET(), @status_notes)


					UPDATE dist_batch_cards
						SET dist_card_status_id = @previous_dis_batch_status_id
						WHERE dist_batch_id = @dist_batch_id

						set @ResultCode=0
			COMMIT TRANSACTION [BATCH_STATUS_CHANGE]

			EXEC usp_get_dist_batch @dist_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [BATCH_STATUS_CHANGE]
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

GO

