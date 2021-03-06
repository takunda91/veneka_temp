USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_search_branch_cards]    Script Date: 2014/08/15 02:05:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch cards at a branch based on search criterial
-- =============================================
ALTER PROCEDURE [dbo].[sp_search_branch_cards] 
	@branch_id int,
	@product_id int = NULL,
	@card_number varchar(20) = NULL,
	@branch_card_statuses_id int = NULL,
	@operator_user_id bigint = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [BRANCH_CARD_SEARCH_TRAN]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			DECLARE @objid int,
					@card_last_four_digits char(4)
			SET @objid = object_id('cards')

			IF LEN(@card_number) = 4
				BEGIN
					SET @card_last_four_digits = CONVERT(char(4), @card_number)
					SET @card_number = NULL
				END

			SELECT [cards].card_id, CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) AS 'card_number', [cards].product_id,
				   [branch_card_status_current].branch_card_statuses_id, [branch_card_statuses].branch_card_statuses_name,
				   [branch_card_status_current].operator_user_id, [branch_card_status_current].status_date, 
				   CONVERT(VARCHAR,DECRYPTBYKEY([user].username)) AS operator_username, [issuer_product].product_bin_code
			FROM [cards]
				INNER JOIN [issuer_product]
					ON [cards].product_id = [issuer_product].product_id
				INNER JOIN [branch_card_status_current]
					ON [cards].card_id = [branch_card_status_current].card_id
				INNER JOIN [branch_card_statuses]
					ON [branch_card_status_current].branch_card_statuses_id = [branch_card_statuses].branch_card_statuses_id
				INNER JOIN [branch]
					ON [branch].branch_id = [cards].branch_id
				LEFT OUTER JOIN [user]
					ON [branch_card_status_current].operator_user_id = [user].[user_id]
			WHERE [cards].branch_id = @branch_id AND
				  [cards].product_id = COALESCE(@product_id, [cards].product_id) AND
				  ((@card_number IS NULL) OR (DECRYPTBYKEY([cards].card_number) LIKE @card_number))	 AND
				  [branch_card_status_current].branch_card_statuses_id = COALESCE(@branch_card_statuses_id, [branch_card_status_current].branch_card_statuses_id) AND
				  ISNULL([branch_card_status_current].operator_user_id, -999) = COALESCE(@operator_user_id, [branch_card_status_current].operator_user_id, -999) AND
				  [branch].branch_status_id = 0 
				  AND ((@card_last_four_digits IS NULL) OR ([cards].[card_index] = [dbo].[MAC] (@card_last_four_digits, @objid)))
				  --[branch_card_status].status_date = (SELECT MAX(bcs2.status_date)
						--							  FROM [branch_card_status] bcs2
						--							  WHERE bcs2.card_id = [cards].card_id)
			
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

			--log the audit record		
			--EXEC sp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 'Getting cards for branch card search.', 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [BRANCH_CARD_SEARCH_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [BRANCH_CARD_SEARCH_TRAN]
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




