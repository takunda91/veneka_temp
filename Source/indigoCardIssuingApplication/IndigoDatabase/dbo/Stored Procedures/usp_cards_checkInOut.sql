-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Mark list of cards as checked in or out
-- =============================================
CREATE PROCEDURE [dbo].[usp_cards_checkInOut] 
	-- Add the parameters for the stored procedure here
	@operator_user_id bigint,
	@branch_id int,
	@product_id int,
	@card_id_array AS card_id_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

        BEGIN TRANSACTION [CARD_CHECKINOUT_TRAN]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			DECLARE @status_date DATETIMEOFFSET
			SET @status_date = SYSDATETIMEOFFSET()

			DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)

			--DECLARE @product_id int

			--SELECT @product_id = product_id 
			--FROM [cards]
			--WHERE card_id = (SELECT TOP 1 card_id FROM @card_id_array)

			--Temp table for cards to be checked out
			DECLARE @check_out_cards table(card_id bigint, branch_id int)

			--Temp table for cards to be check in
			DECLARE @check_in_cards table(card_id bigint, branch_id int)			

			--Find card to move from checked-in to checked out
			INSERT INTO @check_out_cards (card_id, branch_id)
			SELECT card_id, branch_id
			FROM [branch_card_status_current]
			WHERE  [branch_card_status_current].branch_card_statuses_id = 0
					AND [card_id] IN (SELECT card_id FROM @card_id_array)
					AND [product_id] = @product_id


			--Find cards to move from checked out to checked in
			INSERT INTO @check_in_cards (card_id, branch_id)
			SELECT card_id, branch_id
			FROM [branch_card_status_current]
			WHERE [branch_card_status_current].operator_user_id = @operator_user_id
					AND [branch_card_status_current].branch_card_statuses_id = 1
					AND [card_id] NOT IN (SELECT card_id FROM @card_id_array)
					AND [product_id] = @product_id


			--Audit Checked out cards MUST HAPPEN BEFORE ACTUAL INSERT OR NOTHING WILL SHOW IN AUDIT
			DECLARE @audit_description varchar(max),
					@branch_card_status_name varchar(50),
					@operator_name varchar(100)

			SELECT @operator_name = CONVERT(VARCHAR,DECRYPTBYKEY(username))
			FROM [user]
			WHERE [user_id] = @operator_user_id

			SELECT @branch_card_status_name = branch_card_statuses_name
			FROM branch_card_statuses
			WHERE branch_card_statuses_id = 1			

			INSERT INTO [audit_control]
				([audit_action_id], [user_id], [audit_date], [workstation_address], [action_description]
				,[issuer_id], [data_changed], [data_before], [data_after])
			SELECT 1, @audit_user_id, SYSDATETIMEOFFSET(), @audit_workstation, 
				COALESCE(@branch_card_status_name , 'UNKNWON') + ' (check out)' +
				', ' + dbo.MaskString(CONVERT(VARCHAR(max),DECRYPTBYKEY(card_number)),6,4) +
				', to ' + COALESCE(@operator_name, 'UNKNOWN')
				, NULL, NULL, NULL, NULL
				FROM @check_out_cards cardsArray 
					INNER JOIN [branch_card_status_current]
						ON cardsArray.card_id = [branch_card_status_current].card_id
					INNER JOIN [cards]
						ON [cards].card_id = cardsArray.card_id


			--Update Branch Cards status with checked out cards
			UPDATE t
			SET t.branch_id = s.branch_id, 
				t.branch_card_statuses_id = 1, 
				t.status_date = @status_date, 
				t.[user_id] = @audit_user_id, 
				t.operator_user_id = @operator_user_id,
				t.branch_card_code_id = NULL,
				t.comments = NULL,
				t.pin_auth_user_id = NULL
			OUTPUT Deleted.* INTO branch_card_status_audit
			FROM branch_card_status t INNER JOIN @check_out_cards s
					ON t.card_id = s.card_id
						

			--INSERT INTO [branch_card_status]
			--				(card_id, branch_id, operator_user_id, status_date, [user_id], branch_card_statuses_id)
			--SELECT cardsArray.card_id, cardsArray.branch_id, @operator_user_id, @status_date, @audit_user_id, 1
			--FROM @check_out_cards cardsArray
		

			--Audit Checked in cards MUST HAPPEN BEFORE ACTUAL INSERT OR NOTHING WILL SHOW IN AUDIT
			SELECT @branch_card_status_name = branch_card_statuses_name
			FROM branch_card_statuses
			WHERE branch_card_statuses_id = 0		

			INSERT INTO [audit_control]
				([audit_action_id], [user_id], [audit_date], [workstation_address], [action_description]
				,[issuer_id], [data_changed], [data_before], [data_after])
			SELECT 
				1
				, @audit_user_id
				, SYSDATETIMEOFFSET()
				, @audit_workstation
				, COALESCE(@branch_card_status_name , 'UNKNWON') + 
					' (check in)' +
					', ' + [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4)
						 +
					', ' + cards.card_request_reference +
					', from ' + COALESCE(@operator_name, 'UNKNOWN')
				, NULL, NULL, NULL, NULL
			FROM 
				@check_in_cards cardsArray 
				INNER JOIN [branch_card_status_current] ON cardsArray.card_id = [branch_card_status_current].card_id
				INNER JOIN [cards] ON [cards].card_id = cardsArray.card_id			
				INNER JOIN [branch] ON [cards].branch_id = [branch].branch_id
				INNER JOIN [issuer] ON [branch].issuer_id = [issuer].issuer_id	
			--WHERE  [branch_card_status_current].branch_card_statuses_id = 1

			--Update Branch Cards status with checked in cards
			UPDATE t
			SET t.branch_id = s.branch_id, 
				t.branch_card_statuses_id = 0, 
				t.status_date = @status_date, 
				t.[user_id] = @audit_user_id, 
				t.operator_user_id = NULL,
				t.branch_card_code_id = NULL,
				t.comments = NULL,
				t.pin_auth_user_id = NULL
			OUTPUT Deleted.* INTO branch_card_status_audit
			FROM branch_card_status t INNER JOIN @check_in_cards s
					ON t.card_id = s.card_id

			--INSERT INTO [branch_card_status]
			--	(card_id, branch_id, operator_user_id, status_date, [user_id], branch_card_statuses_id)
			--SELECT cardsArray.card_id, cardsArray.branch_id, null, @status_date, @audit_user_id, 0
			--FROM @check_in_cards cardsArray			
			
			--return list of cards that had been checked in and/or out.
			SELECT 
				[cards].card_id, 
				CASE 
					WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
					ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
				END AS 'card_number'
				, [cards].card_request_reference AS card_reference_number
				, [branch_card_status_current].branch_card_statuses_id
				, [branch_card_statuses].branch_card_statuses_name
				, [branch_card_status_current].operator_user_id
				, [branch_card_status_current].status_date			   
			FROM [cards]
				INNER JOIN [branch_card_status_current]
					ON [cards].card_id = [branch_card_status_current].card_id
				INNER JOIN [branch_card_statuses]
					ON 	[branch_card_statuses].branch_card_statuses_id = [branch_card_status_current].branch_card_statuses_id
			WHERE [cards].card_id IN (SELECT card_id FROM @check_out_cards)
					OR
				  [cards].card_id IN (SELECT card_id FROM @check_in_cards)
			

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

			COMMIT TRANSACTION [CARD_CHECKINOUT_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CARD_CHECKINOUT_TRAN]
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