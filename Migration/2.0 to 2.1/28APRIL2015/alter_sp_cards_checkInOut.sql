USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_cards_checkInOut]    Script Date: 2015/04/28 02:59:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Mark list of cards as checked in or out
-- =============================================
ALTER PROCEDURE [dbo].[sp_cards_checkInOut] 
	-- Add the parameters for the stored procedure here
	@operator_user_id bigint,
	@branch_id int,
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

			DECLARE @status_date DATETIME
			SET @status_date = GETDATE()

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
			SELECT 1, @audit_user_id, GETDATE(), @audit_workstation, 
				COALESCE(@branch_card_status_name , 'UNKNWON') + ' (check out)' +
				', ' + dbo.MaskString(CONVERT(VARCHAR,DECRYPTBYKEY(card_number)),6,4) +
				', to ' + COALESCE(@operator_name, 'UNKNOWN')
				, NULL, NULL, NULL, NULL
				FROM @card_id_array cardsArray 
					INNER JOIN [branch_card_status_current]
						ON cardsArray.card_id = [branch_card_status_current].card_id
					INNER JOIN [cards]
						ON [cards].card_id = cardsArray.card_id
				WHERE cardsArray.branch_card_statuses_id = 1 
					  AND [branch_card_status_current].branch_card_statuses_id = 0


			--Update Branch Cards status with checked out cards
			INSERT INTO [branch_card_status]
							(card_id, operator_user_id, status_date, [user_id], branch_card_statuses_id)
			SELECT cardsArray.card_id, @operator_user_id, @status_date, @audit_user_id, cardsArray.branch_card_statuses_id
			FROM @card_id_array cardsArray INNER JOIN [branch_card_status]
					ON cardsArray.card_id = [branch_card_status].card_id
			WHERE cardsArray.branch_card_statuses_id = 1 
			      AND [branch_card_status].branch_card_statuses_id = 0
				  AND [branch_card_status].status_date = (SELECT MAX(bcs2.status_date)
														  FROM [branch_card_status] bcs2
														  WHERE [branch_card_status].card_id = bcs2.card_id)
		

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
				, GETDATE()
				, @audit_workstation
				, COALESCE(@branch_card_status_name , 'UNKNWON') + 
					' (check in)' +
					', ' + CASE WHEN [issuer].card_ref_preference = 1 
							THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4)
							ELSE CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) 
						   END +
					', ' + cards.card_request_reference +
					', from ' + COALESCE(@operator_name, 'UNKNOWN')
				, NULL, NULL, NULL, NULL
			FROM 
				@card_id_array cardsArray 
				INNER JOIN [branch_card_status_current] ON cardsArray.card_id = [branch_card_status_current].card_id
				INNER JOIN [cards] ON [cards].card_id = cardsArray.card_id			
				INNER JOIN [branch] ON [cards].branch_id = [branch].branch_id
				INNER JOIN [issuer] ON [branch].issuer_id = [issuer].issuer_id	
			WHERE 
				cardsArray.branch_card_statuses_id = 0 
				AND [branch_card_status_current].branch_card_statuses_id = 1

			--Update Branch Cards status with checked in cards
			INSERT INTO [branch_card_status]
				(card_id, operator_user_id, status_date, [user_id], branch_card_statuses_id)
			SELECT cardsArray.card_id, null, @status_date, @audit_user_id, cardsArray.branch_card_statuses_id
			FROM @card_id_array cardsArray INNER JOIN [branch_card_status]
					ON cardsArray.card_id = [branch_card_status].card_id
			WHERE cardsArray.branch_card_statuses_id = 0 
			      AND [branch_card_status].branch_card_statuses_id = 1
				  AND [branch_card_status].status_date = (SELECT MAX(bcs2.status_date)
														  FROM [branch_card_status] bcs2
														  WHERE [branch_card_status].card_id = bcs2.card_id) 			
			
			--return list of problem cards.
			SELECT 
				[cards].card_id, '0' as card_number
				, [cards].card_request_reference AS card_reference_number
				, [branch_card_status].branch_card_statuses_id
				, [branch_card_statuses].branch_card_statuses_name
				, [branch_card_status].operator_user_id
				, [branch_card_status].status_date			   
			FROM [cards]
				INNER JOIN [branch_card_status]
					ON [cards].card_id = [branch_card_status].card_id
				INNER JOIN [branch_card_statuses]
					ON [branch_card_status].branch_card_statuses_id = [branch_card_statuses].branch_card_statuses_id
				INNER JOIN [branch]
					ON [branch].branch_id = [cards].branch_id
				INNER JOIN @card_id_array ca
					ON ca.card_id = [cards].card_id				
			WHERE [branch_card_status].branch_card_statuses_id != 0
				  AND ca.branch_card_statuses_id = 0				  				  		  
				  AND [branch_card_status].status_date = (SELECT MAX(bcs2.status_date)
													      FROM [branch_card_status] bcs2
													      WHERE bcs2.card_id = [cards].card_id)
			UNION
			SELECT 
				[cards].card_id
				, '0' as card_number
				, [cards].card_request_reference AS card_reference_number
				, [branch_card_status].branch_card_statuses_id
				, [branch_card_statuses].branch_card_statuses_name
				, [branch_card_status].operator_user_id
				, [branch_card_status].status_date			   
			FROM [cards]
				INNER JOIN [branch_card_status]
					ON [cards].card_id = [branch_card_status].card_id
				INNER JOIN [branch_card_statuses]
					ON [branch_card_status].branch_card_statuses_id = [branch_card_statuses].branch_card_statuses_id
				INNER JOIN [branch]
					ON [branch].branch_id = [cards].branch_id
				INNER JOIN @card_id_array ca
					ON ca.card_id = [cards].card_id
				LEFT OUTER JOIN [user]
					ON [user].[user_id] = [branch_card_status].operator_user_id
			WHERE ca.branch_card_statuses_id = 1
				  AND([branch_card_status].branch_card_statuses_id != 1 OR 
				     ([branch_card_status].branch_card_statuses_id = 1 AND [branch_card_status].operator_user_id != @operator_user_id))				   
				  --AND [branch_card_status].[user_id] != @audit_user_id					  		  
				  AND [branch_card_status].status_date = (SELECT MAX(bcs2.status_date)
													      FROM [branch_card_status] bcs2
													      WHERE bcs2.card_id = [cards].card_id)


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







