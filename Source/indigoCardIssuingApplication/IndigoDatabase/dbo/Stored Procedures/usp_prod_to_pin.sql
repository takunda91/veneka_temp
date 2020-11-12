-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_prod_to_pin] 
	-- Add the parameters for the stored procedure here
	@dist_batch_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    --Check if the issuer prints pins, if it does create pin batch
	IF(1 = ANY (SELECT [issuer_product].pin_mailer_printing_YN 
			FROM [issuer_product] 
			INNER JOIN [dist_batch] 
			ON [issuer_product].issuer_id = [dist_batch].issuer_id 
			WHERE [dist_batch].dist_batch_id = @dist_batch_id) )
	BEGIN
		DECLARE @pin_batch_id bigint,
				@pin_status_date datetimeoffset = SYSDATETIMEOFFSET(),
				@pin_batch_ref varchar(100)

		--CREATE the pin batch from the dist batch
		INSERT INTO [pin_batch] (branch_id, card_issue_method_id, date_created, issuer_id, 
									no_cards, pin_batch_reference, pin_batch_type_id)
		SELECT branch_id, card_issue_method_id, @pin_status_date, issuer_id,
				no_cards, dist_batch_reference + 'P', 0
		FROM [dist_batch]
		WHERE dist_batch_id = @dist_batch_id

		SET @pin_batch_id = SCOPE_IDENTITY();

		--Link the cards to the batch
		INSERT INTO [pin_batch_cards] (card_id, pin_batch_cards_statuses_id, pin_batch_id)
		SELECT card_id, 0, @pin_batch_id
		FROM [dist_batch_cards]
		WHERE dist_batch_id = @dist_batch_id

		--Add status of the pin batch
		INSERT INTO [pin_batch_status] (pin_batch_id, pin_batch_statuses_id, status_date, status_notes, [user_id])
			VALUES (@pin_batch_id, 0, @pin_status_date, 'PIN Batch Created', @audit_user_id)

		--Audit
		DECLARE @audit_msg varchar(max)

		SELECT @pin_batch_ref = pin_batch_reference
		FROM [pin_batch]
		WHERE pin_batch_id = @pin_batch_id

		SET @audit_msg = 'Create: ' + CAST(@pin_batch_id AS varchar(max)) +
						', ' + COALESCE(@pin_batch_ref, 'UNKNOWN') +
						', CREATE' 
								   
		--log the audit record		
		EXEC usp_insert_audit @audit_user_id, 
							2,
							NULL, 
							@audit_workstation, 
							@audit_msg, 
							NULL, NULL, NULL, NULL
	END
END