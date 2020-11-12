Create PROCEDURE [dbo].[usp_print_batch_request_status_change]
@print_batch_id bigint,
@print_batch_statuses_id int,
@Successful bit,
@status_notes nvarchar(50)=null,
@request_list AS dbo.[request_id_array] READONLY,
@card_to_be_spoiled as [dbo].[card_numbers_array] READONLY,
@audit_user_id bigint,
@audit_workstation varchar(100),
@ResultCode int OUTPUT
AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT ON;

--Update the print batch status.
BEGIN TRANSACTION [UPDATE_REQUEST_TRAN]
BEGIN TRY 
		DECLARE @new_print_batch_statuses_id as int

		IF(@Successful=1)
		set @new_print_batch_statuses_id=3
		ELSE
		set @new_print_batch_statuses_id=4


			IF(dbo.[PrintBatchInCorrectStatus](@print_batch_statuses_id, @new_print_batch_statuses_id, @print_batch_id) = 1)
				BEGIN
				SET @ResultCode = 100
				END
			ELSE IF(@Successful=1)
				BEGIN

						DECLARE @temp_card_id TABLE (	
						[card_id] [bigint] NOT NULL,
						request_id [bigint] NOT NULL)
						
				OPEN SYMMETRIC KEY Indigo_Symmetric_Key
				DECRYPTION BY CERTIFICATE Indigo_Certificate;

					--- finding card_id for cardnumbers
					insert into @temp_card_id(card_id,request_id)
					select cards.card_id,r.request_id from cards
					inner join @request_list as r ON 
					[dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4)= 
					[dbo].[MaskString](r.card_number,6,4)
					inner join branch_card_status on branch_card_status.card_id=cards.card_id
					where r.[request_statues_id]=1 and branch_card_status.branch_card_statuses_id=0

					-- sending failed requests back to the stock
					update h
					set h.[hybrid_request_statuses_id]=1,
					h.[status_date] =SYSDATETIMEOFFSET(),
					h.[comments] ='print failed for these cards. putting them back to the to cardstock.',
					h.[user_id]= @audit_user_id
					OUTPUT Deleted.* INTO hybrid_request_status_audit
					from [hybrid_request_status] as h
					inner join print_batch_requests as p on p.request_id=h.request_id
					where p.print_batch_id=@print_batch_id and p.request_id not in (select request_id from @request_list)
									

					Declare @requestcount int,@cardscount int;
					select @cardscount= count(*) from @temp_card_id
					select @requestcount= count(*) from @request_list

						if(@cardscount>0and @cardscount=@requestcount)
						BEGIN

						UPDATE print_batch_status
						SET [print_batch_statuses_id] = @new_print_batch_statuses_id, 
						[user_id] = @audit_user_id, 
						[status_date] = SYSDATETIMEOFFSET(), 
						[status_notes] = @status_notes
						OUTPUT Deleted.* INTO pin_batch_status_audit
						WHERE [print_batch_id] = @print_batch_id

						update c set c.card_priority_id=r.card_priority_id,
						c.ordering_branch_id=r.ordering_branch_id,
						c.delivery_branch_id=r.delivery_branch_id,
						c.branch_id=r.branch_id,
						c.origin_branch_id=r.origin_branch_id,
						c.fee_id=r.fee_id
						from cards as c
						inner join @temp_card_id as t on t.card_id= c.card_id 
						inner join hybrid_requests as r on r.request_id=t.request_id

						---inserting customer_account_cards
						insert customer_account_cards(card_id,customer_account_id)
						select t.card_id,customer_account_requests.customer_account_id
						from @temp_card_id as t
						inner join hybrid_requests as r on r.request_id=t.request_id
						inner join customer_account_requests on customer_account_requests.request_id=r.request_id


						UPDATE c 
						SET branch_id = c.branch_id, 
						branch_card_statuses_id = 16, --- new status to "ASSIGN_TO_Request"
						status_date = SYSDATETIMEOFFSET(), 
						[user_id] = @audit_user_id, 
						branch_card_code_id = c.branch_card_code_id,
						comments = 'Assigned to request ',
						operator_user_id = NULL,
						pin_auth_user_id = NULL
						OUTPUT Deleted.* INTO branch_card_status_audit
						From branch_card_status as c
						inner join cards on c.card_id =cards.card_id
						inner join @temp_card_id as temp on temp.card_id =cards.card_id


						UPDATE c 
						SET branch_id = c.branch_id, 
						branch_card_statuses_id = 7, 
						status_date = SYSDATETIMEOFFSET(), 
						[user_id] = @audit_user_id, 
						branch_card_code_id = c.branch_card_code_id,
						comments = 'spoil card ',
						operator_user_id = NULL,
						pin_auth_user_id = NULL
						OUTPUT Deleted.* INTO branch_card_status_audit
						From branch_card_status as c
						inner join cards on c.card_id =cards.card_id
						inner join @card_to_be_spoiled as s on [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4)= 
					[dbo].[MaskString](s.card_number,6,4) and c.branch_card_statuses_id=0

						set @ResultCode=0

		END
	ELSE
	BEGIN
		RAISERROR (N'requestes in  printbatch (%i) dont match those for the cards count (%i).', 15, 1,@requestcount, @cardscount);

	END
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END
			ELSE
			BEGIN
			-- sending failed requests back to the stock
					update h
					set h.[hybrid_request_statuses_id]=1,
					h.[status_date] =SYSDATETIMEOFFSET(),
					h.[comments] ='print failed for these cards. putting them back to the to cardstock.',
					h.[user_id]= @audit_user_id
					OUTPUT Deleted.* INTO hybrid_request_status_audit
					from [hybrid_request_status] as h
					inner join print_batch_requests as p on p.request_id=h.request_id
					where p.print_batch_id=3 and p.request_id not in (select request_id from @request_list)

					UPDATE print_batch_status
						SET [print_batch_statuses_id] = @new_print_batch_statuses_id, 
						[user_id] = @audit_user_id, 
						[status_date] = SYSDATETIMEOFFSET(), 
						[status_notes] = @status_notes
						OUTPUT Deleted.* INTO pin_batch_status_audit
						WHERE [print_batch_id] = @print_batch_id

						set @ResultCode=0
			END

COMMIT TRANSACTION [UPDATE_REQUEST_TRAN]
END TRY
BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_REQUEST_TRAN]
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
