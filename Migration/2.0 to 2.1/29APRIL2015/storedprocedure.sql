USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_terminal]    Script Date: 2015/04/30 02:47:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi
-- Create date: 20150210
-- Description:	Search for terminal by branch or masterkey
-- =============================================
ALTER PROCEDURE [dbo].[sp_get_terminal]
	@terminal_id INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN
		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate; 

    SELECT 
		[terminal_name]
		, [terminal_model]
		, CONVERT(varchar(max),DECRYPTBYKEY(terminals.device_id)) AS 'device_Id'
		, b.[branch_id]
		, CONVERT(varchar(max),DECRYPTBYKEY(m.masterkey)) AS 'masterkey'
		, i.issuer_id,m.masterkey_id
	FROM
		[terminals]
		INNER JOIN [masterkeys] m ON m.masterkey_id = terminal_masterkey_id
		INNER JOIN [branch] b ON b.[branch_id] = terminals.branch_id
		INNER JOIN [issuer] i ON i.issuer_id = b.issuer_id
	WHERE
		[terminal_id] = @terminal_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

	END

END
GO

ALTER PROCEDURE [dbo].[sp_create_dist_batch] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@branch_id int,	
	@to_branch_id int,
	@card_issue_method_id int,
	@product_id int,
	@sub_product_id int = NULL,
	@batch_card_size int = NULL,
	@create_batch_option int,
	@start_ref varchar(100),
	@end_ref varchar(100),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT,
	@dist_batchid int OUTPUT,
	@dist_batch_refnumber varchar(50) OUTPUT
AS
BEGIN

	BEGIN TRANSACTION [CREATE_DIST_BATCH]
		BEGIN TRY 

			DECLARE @number_of_dist_cards int = 0,
					@start_card_id bigint,
					@end_card_id bigint,
					@cards_total int = 0,
					@dist_batch_id int,
					@status_date datetime = GETDATE(),
					@audit_msg varchar,
					@card_centre bit


			--Determin direction of batch
			SELECT @card_centre = card_centre_branch_YN
			FROM [branch]
			WHERE branch_id  = @branch_id


			IF(@create_batch_option = 2)
			BEGIN
				--Get the start card id
				SELECT @start_card_id = card_id 
				FROM [cards]				
				WHERE [cards].card_request_reference  = @start_ref
						AND [cards].product_id = @product_id
						AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
						AND [cards].card_issue_method_id = @card_issue_method_id
						AND [cards].branch_id = @branch_id
			
				--Get the end card if
				SELECT @end_card_id = card_id 
				FROM [cards]				
				WHERE [cards].card_request_reference  = @end_ref
						AND [cards].product_id = @product_id
						AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
						AND [cards].card_issue_method_id = @card_issue_method_id
						AND [cards].branch_id = @branch_id
		

				--Validations
				--Make sure the cards references are correct
				IF(@start_card_id IS NULL OR @end_card_id IS NULL)
				BEGIN
					SET @ResultCode = 4
					SET @dist_batchid=0
					SET @dist_batch_refnumber=0
					ROLLBACK TRANSACTION [CREATE_DIST_BATCH]
					RETURN;
				END
				--TODO make sure start ref is smaller than end ref
				IF(@start_card_id > @end_card_id)
				BEGIN
					SET @ResultCode = 1
					SET @dist_batchid=0
					SET @dist_batch_refnumber=0
					ROLLBACK TRANSACTION [CREATE_DIST_BATCH]
					RETURN;
				END			

				IF(@card_centre = 1)
				BEGIN				
					SELECT @batch_card_size = COUNT([cards].card_id)
					FROM [cards]
							INNER JOIN [avail_cc_and_load_cards]
								ON [cards].card_id = [avail_cc_and_load_cards].card_id						
					WHERE [cards].branch_id = @branch_id
							AND [cards].product_id = @product_id
							AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
							AND [cards].card_issue_method_id = @card_issue_method_id
							AND [cards].card_id >= @start_card_id AND [cards].card_id <= @end_card_id									
				
				END
				ELSE
				BEGIN
				
					SELECT @batch_card_size = COUNT([cards].card_id)
					FROM [cards]
							INNER JOIN [branch_card_status_current]
								ON [cards].card_id = [branch_card_status_current].card_id
					WHERE [cards].branch_id = @branch_id
							AND [cards].product_id = @product_id
							AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
							AND [branch_card_status_current].branch_card_statuses_id = 0
							AND [cards].card_issue_method_id = @card_issue_method_id
							AND [cards].card_id >= @start_card_id AND [cards].card_id <= @end_card_id				
					
				END				
			END

			IF(@batch_card_size = 0)
				BEGIN
					SET @ResultCode = 1
					SET @dist_batchid=0
					set @dist_batch_refnumber=0
					ROLLBACK TRANSACTION [CREATE_DIST_BATCH]
					RETURN;
				END	

			--create the distribution batch
			INSERT INTO [dist_batch]
				([branch_id], [no_cards],[date_created],[dist_batch_reference], [card_issue_method_id],
					[dist_batch_type_id], [issuer_id])
			VALUES (@to_branch_id, 0, @status_date, @status_date, @card_issue_method_id, 1, @issuer_id)

			SET @dist_batch_id = SCOPE_IDENTITY();

			IF(@card_centre = 1)
			BEGIN
				--Add cards to distribution batch from card centre
				INSERT INTO [dist_batch_cards]
					([dist_batch_id],[card_id],[dist_card_status_id])
				SELECT TOP(@batch_card_size)	@dist_batch_id, [cards].card_id, 0
				FROM [cards]
						INNER JOIN [avail_cc_and_load_cards]
							ON [cards].card_id = [avail_cc_and_load_cards].card_id						
				WHERE [cards].branch_id = @branch_id
						AND [cards].product_id = @product_id
						AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
						AND [cards].card_issue_method_id = @card_issue_method_id
						AND (((@create_batch_option = 2) AND ([cards].card_id >= @start_card_id AND [cards].card_id <= @end_card_id))
								OR @create_batch_option = 1)
				ORDER BY [cards].card_id
				
			END
			ELSE
			BEGIN
				--Add cards to distribution batch from branch
				INSERT INTO [dist_batch_cards]
					([dist_batch_id],[card_id],[dist_card_status_id])
				SELECT TOP(@batch_card_size)
						@dist_batch_id, 
						[cards].card_id, 
						0
				FROM [cards]
						INNER JOIN [branch_card_status_current]
							ON [cards].card_id = [branch_card_status_current].card_id
				WHERE [cards].branch_id = @branch_id
						AND [cards].product_id = @product_id
						AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
						AND [branch_card_status_current].branch_card_statuses_id = 0
						AND [cards].card_issue_method_id = @card_issue_method_id
						AND (((@create_batch_option = 2) AND ([cards].card_id >= @start_card_id AND [cards].card_id <= @end_card_id))
								OR @create_batch_option = 1)
				ORDER BY [cards].card_id
			END
							
			--Get the number of cards inserted
			SELECT @number_of_dist_cards = @@ROWCOUNT										

			--Make sure we've insered enough cards
			IF(@number_of_dist_cards = @batch_card_size)
			BEGIN
				IF(@card_centre = 1)
				BEGIN
					--add dist batch status of created
					INSERT INTO [dbo].[dist_batch_status]
						([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
					VALUES(@dist_batch_id, 0, @audit_user_id, @status_date, 'Dist Batch Create')
				END
				ELSE
				BEGIN
					--add dist batch status of created
					INSERT INTO [dbo].[dist_batch_status]
						([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
					VALUES(@dist_batch_id, 19, @audit_user_id, @status_date, 'Dist Batch Create')
				END

				--Generate dist batch reference
				DECLARE @dist_batch_ref varchar(50)
				SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
										  [branch].branch_code + '' + 
										  CONVERT(VARCHAR(8), @status_date, 112) + '' +
										  CAST(@dist_batch_id AS varchar(max))
				FROM [branch] INNER JOIN [issuer]
					ON [branch].issuer_id = [issuer].issuer_id
				WHERE [branch].branch_id = @branch_id

				--UPDATE dist batch with reference and number of cards
				UPDATE [dist_batch]
				SET [dist_batch_reference] = @dist_batch_ref,
					[no_cards] = @number_of_dist_cards
				WHERE [dist_batch].dist_batch_id = @dist_batch_id

				----UPDATE the load batch cards status to allocated							
				--UPDATE [load_batch_cards]
				--SET [load_batch_cards].load_card_status_id = 2
				--WHERE [load_batch_cards].card_id IN 
				--		(SELECT [dist_batch_cards].card_id
				--		 FROM [dist_batch_cards]
				--		 WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id)

				IF(@card_centre = 1)
				BEGIN
					UPDATE [dist_batch_cards]
					SET [dist_batch_cards].dist_card_status_id = 0
					FROM [dist_batch_cards]
						INNER JOIN [cards]
							ON [cards].card_id = [dist_batch_cards].card_id
								AND [dist_batch_cards].dist_card_status_id = 18
						INNER JOIN [dist_batch_cards] batch_cards
							ON [cards].card_id = batch_cards.card_id
					WHERE batch_cards.dist_batch_id = @dist_batch_id

					UPDATE [load_batch_cards]
					SET [load_batch_cards].load_card_status_id = 2
					FROM [load_batch_cards]
						INNER JOIN [cards]
							ON [cards].card_id = [load_batch_cards].card_id
								AND [load_batch_cards].load_card_status_id = 1
						INNER JOIN [dist_batch_cards]
							ON [dist_batch_cards].card_id = [cards].card_id
					WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id	

				END
				ELSE
				BEGIN
					INSERT INTO [branch_card_status] (card_id, branch_card_statuses_id, comments, status_date, [user_id])
					SELECT card_id, 13, '', GETDATE(), @audit_user_id
					FROM [dist_batch_cards]
					WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id
				END

				--Update the cards to the new destination branch.
				UPDATE [cards]
				SET branch_id = @to_branch_id
				FROM [cards]
						INNER JOIN [dist_batch_cards]
							ON [cards].card_id = [dist_batch_cards].card_id
				WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id


				--UPDATE [dist_batch_cards]
				--SET [dist_batch_cards].dist_batch_id = 0
				--FROM [cards]
				--	INNER JOIN [dist_batch_cards]
				--		ON [cards].card_id = [dist_batch_cards].card_id
				--	INNER JOIN [dist_batch_cards] prod_batch_cards
				--		ON [cards].card_id = prod_batch_cards.card_id
				--			AND .dist_card_status_id = 18
				--	INNER JOIN [dist_batch]
				--		ON [dist_batch_cards].dist_batch_id = [dist_batch].dist_batch_id
				--	INNER JOIN [dist_batch_status_current]
				--		ON [dist_batch].dist_batch_id = [dist_batch_status_current].dist_batch_id
				--WHERE [cards].branch_id = @branch_id
				--		AND [cards].product_id = @product_id
				--		AND [cards].sub_product_id = @sub_product_id
				--		AND [dist_batch].dist_batch_type_id = 0
				--		AND [dist_batch_status_current].dist_batch_statuses_id = 14
				--		AND [dist_batch_cards].dist_card_status_id = 18
				--		AND [cards].card_issue_method_id = COALESCE(@card_issue_method_id, [cards].card_issue_method_id)

				
				DECLARE @dist_batch_status_name varchar(50)
				SELECT @dist_batch_status_name =  dist_batch_status_name
				FROM dist_batch_statuses
				WHERE dist_batch_statuses_id = 0

				--Add audit for dist batch creation								
				SET @audit_msg = 'Create: ' + CAST(@dist_batch_id AS varchar(max)) +
									', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
									', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
				--log the audit record		
				EXEC sp_insert_audit @audit_user_id, 
										2,
										NULL, 
										@audit_workstation, 
										@audit_msg, 
										NULL, NULL, NULL, NULL

				SET @ResultCode = 0
				SET @dist_batchid=@dist_batch_id
				SET @dist_batch_refnumber=@dist_batch_ref

				COMMIT TRANSACTION [CREATE_DIST_BATCH]	
			END
			ELSE
			BEGIN
				--Size fo cards for batch doesnt match number of records inserted.
				SET @ResultCode = 70
				SET @dist_batchid=0
				SET @dist_batch_refnumber=0
				ROLLBACK TRANSACTION [CREATE_DIST_BATCH]
			END						
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_DIST_BATCH]
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

ALTER PROCEDURE [dbo].[sp_create_terminal]
	@terminal_name varchar(250)
	, @terminal_model varchar(250)
	, @device_id varchar(max)
	, @branch_id int
	, @terminal_masterkey_id int
	, @audit_user_id bigint
	, @audit_workstation varchar(100)
	, @new_terminal_id int OUTPUT
	, @ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
			OPEN Symmetric Key Indigo_Symmetric_Key
			DECRYPTION BY Certificate Indigo_Certificate;

	BEGIN TRANSACTION [INSERT_TERMINAL_TRAN]
		BEGIN TRY 

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [terminals] WHERE [terminals].[terminal_name] = @terminal_name) > 0
				BEGIN
					SET @new_terminal_id = 0
					SET @ResultCode = 604						
				END
			ELSE IF (SELECT COUNT(*) FROM [terminals] WHERE (CONVERT(VARCHAR(max),DECRYPTBYKEY([terminals].[device_id])) = @device_id)) > 0
				BEGIN
					SET @new_terminal_id = 0
					SET @ResultCode = 605
				END
			ELSE			
			BEGIN

		

				INSERT INTO [dbo].[terminals]
					   ( [terminal_name]
					   , [terminal_model]
					   , [device_id]
					   , [branch_id]
					   , [terminal_masterkey_id]
					   , [workstation]
					   , [date_created]
					   , [date_changed])
				 VALUES
					   ( @terminal_name
					   , @terminal_model
					   , CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@device_id)))
					   , @branch_id
					   , @terminal_masterkey_id
					   , @audit_workstation
					   , GETDATE()
					   , GETDATE())

				SET @new_terminal_id = SCOPE_IDENTITY();
			SET @ResultCode = 0	
		

				--log the audit record
				DECLARE @audit_description varchar(max) = '',
						@branch_name varchar(100)

				SELECT @branch_name = branch_name
				FROM [branch]
				WHERE branch_id = @branch_id

				SET @audit_description = 'Create: id: ' + CAST(@new_terminal_id AS VARCHAR(max))	+ 
										 ', name: ' + COALESCE(@terminal_name, 'UNKNOWN') +
										 ', model: ' + COALESCE(@terminal_model, 'UNKNOWN') +
										 ', branch: ' + COALESCE(@branch_name, 'UNKNOWN')
										 	
				EXEC sp_insert_audit @audit_user_id, 
									 0,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @new_terminal_id, NULL, NULL, NULL

									 SET @ResultCode = 0		
					
			END

			COMMIT TRANSACTION [INSERT_TERMINAL_TRAN]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_TERMINAL_TRAN]
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
		CLOSE Symmetric Key Indigo_Symmetric_Key;	
END
GO

ALTER PROCEDURE [dbo].[sp_update_terminal]
	@terminal_id int
	, @terminal_name varchar(250)
	, @terminal_model varchar(250)
	, @device_id varchar(max)
	, @branch_id int
	, @terminal_masterkey_id int
	, @audit_user_id bigint
	, @audit_workstation varchar(100)
	, @ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		OPEN Symmetric Key Indigo_Symmetric_Key
			DECRYPTION BY Certificate Indigo_Certificate;
	BEGIN TRANSACTION [UPDATE_TERMINAL_TRAN]
		BEGIN TRY 

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [terminals] WHERE ([terminals].[terminal_name] = @terminal_name AND [terminals].[terminal_id] != @terminal_id)) > 0
				BEGIN					
					SET @ResultCode = 604						
				END
			ELSE IF (SELECT COUNT(*) FROM [terminals] WHERE (CONVERT(VARCHAR(max),DECRYPTBYKEY([terminals].[device_id]))  = @device_id AND [terminals].[terminal_id] != @terminal_id)) > 0
				BEGIN
					SET @ResultCode = 605
				END
			ELSE			
			BEGIN
			
		

				UPDATE [dbo].[terminals]
				   SET [terminal_name] = @terminal_name
					  ,[terminal_model] = @terminal_model
					  ,[device_id] = CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@device_id)))
					  ,[branch_id] = @branch_id
					  ,[terminal_masterkey_id] = @terminal_masterkey_id
					  ,[workstation] = @audit_workstation
					  ,[date_changed] = GETDATE()
				 WHERE [terminal_id] = @terminal_id
				 SET @ResultCode = 0
			

				--log the audit record
				DECLARE @audit_description varchar(max) = '',
						@branch_name varchar(100)

				SELECT @branch_name = branch_name
				FROM [branch]
				WHERE branch_id = @branch_id

				SET @audit_description = 'Create: id: ' + CAST(@terminal_id AS VARCHAR(max))	+ 
										 ', name: ' + COALESCE(@terminal_name, 'UNKNOWN') +
										 ', model: ' + COALESCE(@terminal_model, 'UNKNOWN') +
										 ', branch: ' + COALESCE(@branch_name, 'UNKNOWN')
										 	
				EXEC sp_insert_audit @audit_user_id, 
									 0,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @terminal_id, NULL, NULL, NULL

									 SET @ResultCode = 0		
					
			END

			COMMIT TRANSACTION [UPDATE_TERMINAL_TRAN]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_TERMINAL_TRAN]
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
	CLOSE Symmetric Key Indigo_Symmetric_Key;	
END
GO

ALTER PROCEDURE [dbo].[sp_get_terminal]
	@terminal_id INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN
		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate; 

    SELECT 
		[terminal_name]
		, [terminal_model]
		, CONVERT(varchar(max),DECRYPTBYKEY(terminals.device_id)) AS 'device_Id'
		, b.[branch_id]
		, CONVERT(varchar(max),DECRYPTBYKEY(m.masterkey)) AS 'masterkey'
		, i.issuer_id,m.masterkey_id
	FROM
		[terminals]
		INNER JOIN [masterkeys] m ON m.masterkey_id = terminal_masterkey_id
		INNER JOIN [branch] b ON b.[branch_id] = terminals.branch_id
		INNER JOIN [issuer] i ON i.issuer_id = b.issuer_id
	WHERE
		[terminal_id] = @terminal_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

	END

END
GO





