USE [indigo_database_group]
GO

/****** Object:  StoredProcedure [dbo].[sp_insert_audit]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_insert_audit]
	@audit_user_id bigint,
	@audit_action_id int,
	@audit_date datetime = NULL,
	@workstation_address varchar(100),
	@action_description varchar(max),
	@issuer_id int = NULL,
	@data_changed varchar(30) = NULL,
	@data_before varchar(450) = NULL,
	@data_after varchar(450) = NULL

AS
BEGIN
	INSERT INTO [audit_control]
           ([audit_action_id]
           ,[user_id]
           ,[audit_date]
           ,[workstation_address]
           ,[action_description]
           ,[issuer_id]
           ,[data_changed]
           ,[data_before]
           ,[data_after])
     VALUES
           (@audit_action_id
           ,@audit_user_id
           ,COALESCE(@audit_date, GETDATE())
           ,@workstation_address
           ,@action_description
           ,@issuer_id
           ,@data_changed
           ,@data_before
           ,@data_after)
	
END

GO
/****** Object:  StoredProcedure [dbo].[sp_get_dist_batch]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Retreive a distribution batch
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_dist_batch] 
	@dist_batch_id bigint,
	@language_id INT,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_DIST_BATCH]
		BEGIN TRY 

			SELECT CAST(1 as BIGINT) as ROW_NO, 1 AS TOTAL_ROWS, 1 as TOTAL_PAGES, 
				   [dist_batch].dist_batch_id, [dist_batch].date_created, [dist_batch].dist_batch_reference, 
				   [dist_batch].no_cards, [dist_batch_status].dist_batch_statuses_id, 
				   [dist_batch_status].status_notes, [dist_batch_statuses_language].language_text AS 'dist_batch_status_name', 
				   [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code, [issuer].auto_create_dist_batch,
				   [branch].branch_name, [branch].branch_code
			FROM [dist_batch] 
				INNER JOIN [dist_batch_status]
					ON [dist_batch_status].dist_batch_id = [dist_batch].dist_batch_id
				INNER JOIN [dist_batch_statuses_language]
					ON [dist_batch_status].dist_batch_statuses_id = [dist_batch_statuses_language].dist_batch_statuses_id
						
				INNER JOIN [branch]
					ON [branch].branch_id = [dist_batch].branch_id				
				INNER JOIN [issuer]
					ON [issuer].issuer_id = [branch].issuer_id				
			WHERE 
				[dist_batch].dist_batch_id = @dist_batch_id	
				AND [dist_batch_status].status_date = (SELECT MAX(dbs2.status_date)
													   FROM [dist_batch_status] dbs2
													   WHERE dbs2.dist_batch_id = [dist_batch].dist_batch_id)
				AND [dist_batch_statuses_language].language_id = @language_id		

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting dist batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC sp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_DIST_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_DIST_BATCH]
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
/****** Object:  StoredProcedure [dbo].[sp_insert_product_currency]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya
-- Create date: 
-- Description:	Insert link between users group and branches
-- =============================================
CREATE PROCEDURE [dbo].[sp_insert_product_currency] 
	-- Add the parameters for the stored procedure here
	@product_id int,
	 @currencylist as dbo.currency_id_array READONLY,
	
	@audit_user_id BIGINT,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--BEGIN TRANSACTION [INSERT_USER_GROUP_BRANCHES_TRAN]

	--	BEGIN TRY

			INSERT INTO product_currency
						(product_id, currency_id)
			SELECT @product_id, cl.currency_id
			FROM @currencylist cl

			--Insert audit train
			--EXEC sp_insert_audit @audit_user_id, 
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





GO
/****** Object:  StoredProcedure [dbo].[AddMacForTable]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[AddMacForTable] @Table_id int
--WITH EXECUTE AS 'dbo'
AS
        declare @Key    varbinary(100)
        declare @KeyGuid uniqueidentifier
        SET @KeyGuid = key_guid('key_Indexing')
        -- Open the encryption key
        -- Make sure the key is closed before doing any operation
		-- that may end the module, otherwise the key will
		-- remain opened after the store-procedure execution ends
        OPEN SYMMETRIC KEY key_Indexing DECRYPTION BY CERTIFICATE cert_ProtectIndexingKeys
 
        -- The new MAC key is derived from an encryption
		-- of a newly created GUID. As the encryption function
		-- is not deterministic, the output is random
        -- After getting this cipher, we calculate a SHA1 Hash for it.
        SELECT @Key = HashBytes( N'SHA1', ENCRYPTBYKEY( @KeyGuid, convert(varbinary(100), newid())) )
 
		-- Protect the new MAC key
        SET @KEY = ENCRYPTBYKEY( @KeyGuid, @Key )
 
        -- Closing the encryption key
        CLOSE SYMMETRIC KEY key_Indexing
        -- As we have closed the key we opened,
		-- it is safe to return from the SP at any time
 
        if @Key is null
        BEGIN
               RAISERROR( 'Failed to create new key.', 16, 1)
        END
        INSERT INTO mac_index_keys VALUES( @Table_id, @Key )




GO
/****** Object:  StoredProcedure [dbo].[GenerateAuditReport]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GenerateAuditReport]
	-- Add the parameters for the stored procedure here
	@User		varchar(30) = null,
	@ActionType varchar(30) = null,	
	@IssuerID int = 0,
	@DateFrom		datetime = null,
	@DateTo		datetime = null,
	@Keyword  varchar (50) = null

AS
BEGIN

IF (@User = '') OR (@User = 'All') OR (@User like 'null')
	BEGIN
	 SET @User =NULL
	END

IF (@ActionType = '' ) OR (@ActionType = 'All' ) OR (@ActionType like 'null')
	BEGIN
		 SET @ActionType =NULL
	END


IF(@DateFrom = '1900-01-01 00:00:00.000') OR (@DateFrom=null)
		BEGIN			
			SET @DateFrom = NULL
		END

IF(@DateTo= '1900-01-01 00:00:00.000') OR (@DateTo=null)
		BEGIN			
			SET @DateTo = NULL
		END

	IF(@IssuerID = 0)
		BEGIN			
			SET @IssuerID = NULL
		END

IF (@Keyword = '')  OR (@Keyword like 'null')
	BEGIN
		 SET @Keyword =''
	END

	SELECT   
	       audit_id,
			audit_date ,
			user_audit  , 
			workstation_address  , 
			user_action  ,
			action_description , 
		   COALESCE( (Select issuer_name FROM issuer where issuer.issuer_id =  audit.issuer_id   ),'-') AS 'Issuer',
			data_before  ,
			data_after ,
			audit.issuer_id

	FROM            audit_controll audit

	WHERE 	audit.user_action  =  COALESCE(@ActionType ,audit.user_action)
			AND	audit.user_audit  =  COALESCE(@User ,audit.user_audit)
			AND audit.issuer_id =  @IssuerID 
			AND audit.audit_date >= COALESCE(@DateFrom, audit.audit_date) 
			AND audit.audit_date <= COALESCE(DATEADD(dd,1,@DateTo), audit.audit_date)
			AND audit.data_after like '%'+@Keyword + '%'

END



GO
/****** Object:  StoredProcedure [dbo].[sp_card_branch_spoil]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_card_branch_spoil] 
	@card_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	EXEC sp_issue_card_spoil @card_id, 
							 @audit_user_id,
							 @audit_workstation,
							 @ResultCode
END



GO
/****** Object:  StoredProcedure [dbo].[sp_card_MakerChecker]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_card_MakerChecker] 
	@card_id bigint,
	@approve bit,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [MAKER_CHECKER_TRAN]
		BEGIN TRY 

			DECLARE @current_status int					

			SELECT @current_status = branch_card_statuses_id
			FROM [branch_card_status_current]
			WHERE card_id = @card_id			

			IF(@current_status != 2)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN
					OPEN SYMMETRIC KEY Indigo_Symmetric_Key
					DECRYPTION BY CERTIFICATE Indigo_Certificate

					DECLARE @status_date datetime,
							@new_status_id int,
							@operator_username varchar(100)

					--Operator username needed for audit
					SELECT @operator_username = CONVERT(VARCHAR,DECRYPTBYKEY(username))
					FROM [branch_card_status_current]
							INNER JOIN [user]
								ON [branch_card_status_current].operator_user_id = [user].[user_id]
					WHERE [branch_card_status_current].card_id = @card_id

					

					SET @status_date = GETDATE()

					IF(@approve = 1)
						SET @new_status_id = 3
					ELSE
						BEGIN
							SET @new_status_id = 1

							--Delete the customer information, it is not needed.
							DELETE FROM customer_account
							WHERE card_id = @card_id
						END

					--Update Branch Cards status with checked out cards
					INSERT INTO [branch_card_status]
									(card_id, operator_user_id, status_date, [user_id], branch_card_statuses_id)
					SELECT @card_id, [branch_card_status_current].operator_user_id, @status_date, @audit_user_id, @new_status_id
					FROM [branch_card_status_current]
					WHERE [branch_card_status_current].card_id = @card_id


					--log the audit record
					DECLARE @audit_description varchar(max),
					        @branchcardstatus  varchar(50),
							@cardnumber varchar(50)							

					SELECT  @branchcardstatus =  branch_card_statuses_name
					FROM    branch_card_statuses 
					WHERE	branch_card_statuses_id = @new_status_id

					
					 
					SELECT	@cardnumber = CONVERT(VARCHAR,DECRYPTBYKEY(card_number)) 
					FROM	cards 
					WHERE	card_id = @card_id					

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

					IF(@approve = 0)
						BEGIN
							SET @audit_description = COALESCE(@branchcardstatus, 'UNKNOWN') + '(reject)' + 
													', ' + dbo.MaskString(@cardnumber, 6, 4) +
													', to ' + COALESCE(@operator_username, 'UNKOWN')
						END
					ELSE
						BEGIN
							SET @audit_description = COALESCE(@branchcardstatus, 'UNKNOWN') +
													', ' + dbo.MaskString(@cardnumber, 6, 4)
						END
						
					EXEC sp_insert_audit @audit_user_id, 
										 3, ---IssueCard
										 @status_date, 
										 @audit_workstation, 
										 @audit_description, 
										 NULL, NULL, NULL, NULL

					SET @ResultCode = 0
				END

			COMMIT TRANSACTION [MAKER_CHECKER_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [MAKER_CHECKER_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_cards_checkInOut]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Mark list of cards as checked in or out
-- =============================================
CREATE PROCEDURE [dbo].[sp_cards_checkInOut] 
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
			SELECT 1, @audit_user_id, GETDATE(), @audit_workstation, 
				COALESCE(@branch_card_status_name , 'UNKNWON') + ' (check in)' +
				', ' + dbo.MaskString(CONVERT(VARCHAR,DECRYPTBYKEY(card_number)),6,4) +
				', from ' + COALESCE(@operator_name, 'UNKNOWN')
				, NULL, NULL, NULL, NULL
				FROM @card_id_array cardsArray 
					INNER JOIN [branch_card_status_current]
						ON cardsArray.card_id = [branch_card_status_current].card_id
					INNER JOIN [cards]
						ON [cards].card_id = cardsArray.card_id
				WHERE cardsArray.branch_card_statuses_id = 0 
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
			SELECT [cards].card_id, '0' as card_number,
				   [branch_card_status].branch_card_statuses_id, [branch_card_statuses].branch_card_statuses_name,
				   [branch_card_status].operator_user_id, [branch_card_status].status_date			   
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
			SELECT [cards].card_id, '0' as card_number,
				   [branch_card_status].branch_card_statuses_id, [branch_card_statuses].branch_card_statuses_name,
				   [branch_card_status].operator_user_id, [branch_card_status].status_date			   
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




GO
/****** Object:  StoredProcedure [dbo].[sp_cms_search_for_error]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_cms_search_for_error] 
	-- Add the parameters for the stored procedure here
	@error varchar(1000), 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM [mod_response_mapping]
	WHERE @error LIKE '%' + response_contains + '%'
END



GO
/****** Object:  StoredProcedure [dbo].[sp_connection_parameter_create]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_connection_parameter_create] 
	@connection_name varchar(100),
	@address varchar(200),
	@port int,
	@path varchar(200),
	@protocol varchar(50),
	@auth_type int,
	@auth_username VARCHAR(100),
	@auth_password VARCHAR(100),
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [INSERT_CANN_PARAM_TRAN]
		BEGIN TRY 

			DECLARE @connection_parameter_id int

			IF @auth_username IS NULL
				SET @auth_username = ''

			IF @auth_password IS NULL
				SET @auth_password = ''

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

				INSERT INTO [dbo].[connection_parameters]
					   ([connection_name],[address],[port],[path],[protocol],[auth_type],[username],[password])
				VALUES
					   (@connection_name, @address, @port, @path, @protocol, @auth_type, 
					    ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@auth_username)),
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@auth_password)))		


				SET @connection_parameter_id = SCOPE_IDENTITY();

				CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

				--log the audit record
				--DECLARE @audit_description varchar(500)
				--SELECT @audit_description = 'Inserted Connection parameter with id = ' + CONVERT(NVARCHAR, @connection_parameter_id)	+ ')'			
				--EXEC sp_insert_audit @audit_user_id, 
				--					 0,
				--					 NULL, 
				--					 @audit_workstation, 
				--					 @audit_description, 
				--					 NULL, NULL, NULL, NULL		


				COMMIT TRANSACTION [INSERT_CANN_PARAM_TRAN]

				SELECT @connection_parameter_id

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_CANN_PARAM_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_connection_parameter_update]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_connection_parameter_update]
	@connection_parameter_id int,
	@connection_name varchar(100),
	@address varchar(200),
	@port int,
	@path varchar(200),
	@protocol varchar(50),
	@auth_type int,
	@auth_username VARCHAR(100) = NULL,
	@auth_password VARCHAR(100) = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_CONN_PARAM_TRAN]
		BEGIN TRY 

			IF @auth_username IS NULL
				SET @auth_username = ''

			IF @auth_password IS NULL
				SET @auth_password = ''

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

				UPDATE [dbo].[connection_parameters]
				SET [connection_name] = @connection_name,
					[address] = @address,
					[port] = @port,
					[path] = @path,
					[protocol] = @protocol,
					[auth_type] = @auth_type,
					[username] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@auth_username)),
					[password] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@auth_password))
				WHERE [connection_parameter_id] = @connection_parameter_id


				CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

				--log the audit record
				--DECLARE @audit_description varchar(500)
				--SELECT @audit_description = 'Updated Connection parameter with id = ' + CONVERT(NVARCHAR, @connection_parameter_id)	+ ')'			
				--EXEC sp_insert_audit @audit_user_id, 
				--					 2,
				--					 NULL, 
				--					 @audit_workstation, 
				--					 @audit_description, 
				--					 NULL, NULL, NULL, NULL		


				COMMIT TRANSACTION [UPDATE_CONN_PARAM_TRAN]

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_CONN_PARAM_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_count_distinct_load_cards]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 5 April 2014
-- Description:	Find all distinct card numbers from load cards based on supplied card numbers list
--				and returns the number of matched cards.
-- =============================================
CREATE PROCEDURE [dbo].[sp_count_distinct_load_cards] 
	-- Add the parameters for the stored procedure here
	@card_list dbo.DistBatchCards READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT COUNT(DISTINCT CONVERT(VARCHAR,DECRYPTBYKEY(load_card.card_number)))
		FROM load_card
		WHERE CONVERT(VARCHAR,DECRYPTBYKEY(load_card.card_number)) IN (
				SELECT cardlist.card_number
				FROM @card_list cardlist )
      
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[sp_create_dist_batch]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Manually create a new distribution batch.
-- =============================================
CREATE PROCEDURE [dbo].[sp_create_dist_batch] 
	-- Add the parameters for the stored procedure here
	@branch_id int, 
	@batch_card_size int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN

	BEGIN TRANSACTION [CREATE_DIST_BATCH]
		BEGIN TRY 

			DECLARE @number_of_dist_cards int = 0,
					@cards_total int = 0,
					@dist_batch_id int,
					@audit_msg varchar


			--create the distribution batch
			INSERT INTO [dist_batch]
				([branch_id], [no_cards],[date_created],[dist_batch_reference])
			VALUES (@branch_id, 0, GETDATE(), GETDATE())

			SET @dist_batch_id = SCOPE_IDENTITY();

			--Add cards to distribution batch
			INSERT INTO [dist_batch_cards]
				([dist_batch_id],[card_id],[dist_card_status_id])
			SELECT top(@batch_card_size)
				@dist_batch_id, [cards].card_id, 0
			FROM [cards] INNER JOIN [load_batch_cards]
				ON [cards].card_id = [load_batch_cards].card_id
			WHERE [load_batch_cards].load_card_status_id = 1
				  AND [cards].branch_id = @branch_id
							
			--Get the number of cards inserted
			SELECT @number_of_dist_cards = @@ROWCOUNT										

			--Make sure we've insered enough cards
			IF(@number_of_dist_cards = @batch_card_size)
			BEGIN
				--add dist batch status of created
				INSERT INTO [dbo].[dist_batch_status]
					([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
				VALUES(@dist_batch_id, 0, @audit_user_id, GETDATE(), 'Dist Batch Create')

				--Generate dist batch reference
				DECLARE @dist_batch_ref varchar(50)
				SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
										  [branch].branch_code + '' + 
										  CONVERT(VARCHAR(8), GETDATE(), 112) + '' +
										  CAST(@dist_batch_id AS varchar(max))
				FROM [branch] INNER JOIN [issuer]
					ON [branch].issuer_id = [issuer].issuer_id
				WHERE [branch].branch_id = @branch_id

				--UPDATE dist batch with reference and number of cards
				UPDATE [dist_batch]
				SET [dist_batch_reference] = @dist_batch_ref,
					[no_cards] = @number_of_dist_cards
				WHERE [dist_batch].dist_batch_id = @dist_batch_id

				--UPDATE the load batch cards status to allocated							
				UPDATE [load_batch_cards]
				SET [load_batch_cards].load_card_status_id = 2
				WHERE [load_batch_cards].card_id IN 
						(SELECT [dist_batch_cards].card_id
						 FROM [dist_batch_cards]
						 WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id)
				
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

				SELECT @ResultCode = 0

				COMMIT TRANSACTION [CREATE_DIST_BATCH]	
			END
			ELSE
			BEGIN
				--Size fo cards for batch doesnt match number of records inserted.
				SELECT @ResultCode = 70
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
/****** Object:  StoredProcedure [dbo].[sp_create_issuer]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo, Setenane
-- Create date: 2014/03/20
-- Description:	Create a new issuer and returns its ID if the create
--				was successful, else return the error message.				
-- =============================================

CREATE PROCEDURE [dbo].[sp_create_issuer]
	@issuer_status_id int,
	@country_id int,
	@issuer_name varchar(50),
	@issuer_code varchar(10),
	@auto_create_dist_batch bit,
	@instant_card_issue_YN bit,
	@pin_mailer_printing_YN bit,
	@delete_pin_file_YN bit,
	@delete_card_file_YN bit,
	@account_validation_YN bit,
	@maker_checker_YN bit,
	@license_file varchar(100) = NULL,
	@license_key varchar(1000) = NULL,
	@cards_file_location varchar(100) = NULL,
	@card_file_type varchar(20) = NULL,
	@pin_file_location varchar(100) = NULL,
	@pin_encrypted_ZPK varchar(40) = NULL,
	@pin_mailer_file_type varchar(20) = NULL,
	@pin_printer_name varchar(50) = NULL,
	@pin_encrypted_PWK varchar(40) = NULL,
	@language_id int = NULL,
	@account_connection_id int = NULL,
	@corebanking_connection_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@new_issuer_id int OUTPUT,
	@ResultCode int OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [INSERT_ISSUER_TRAN]
		BEGIN TRY 

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [issuer] WHERE [issuer].[issuer_name] = @issuer_name) > 0
				BEGIN
					SET @new_issuer_id = 0
					SET @ResultCode = 200						
				END
			ELSE IF (SELECT COUNT(*) FROM [issuer] WHERE ([issuer].[issuer_code] = @issuer_code)) > 0
				BEGIN
					SET @new_issuer_id = 0
					SET @ResultCode = 201
				END
			ELSE			
			BEGIN

				INSERT INTO [dbo].[issuer]
					   ([issuer_status_id],[country_id],[issuer_name],[issuer_code],[auto_create_dist_batch]
					   ,[instant_card_issue_YN],[pin_mailer_printing_YN],[delete_pin_file_YN]
					   ,[delete_card_file_YN],[account_validation_YN],[maker_checker_YN],[license_file]
					   ,[license_key],[cards_file_location],[card_file_type],[pin_file_location]
					   ,[pin_encrypted_ZPK],[pin_mailer_file_type],[pin_printer_name],[pin_encrypted_PWK]
					   ,[language_id])
				 VALUES
					   (@issuer_status_id, @country_id, @issuer_name, @issuer_code, @auto_create_dist_batch,
						@instant_card_issue_YN, @pin_mailer_printing_YN, @delete_pin_file_YN,
						@delete_card_file_YN, @account_validation_YN, @maker_checker_YN,
						@license_file, @license_key, @cards_file_location, @card_file_type,
						@pin_file_location, @pin_encrypted_ZPK,	@pin_mailer_file_type,
						@pin_printer_name, @pin_encrypted_PWK, @language_id)

				SET @new_issuer_id = SCOPE_IDENTITY();

				IF (@instant_card_issue_YN = 1)
				BEGIN

					IF (@account_validation_YN = 1)
					BEGIN
						--INSERT ACCOUNT INTERFACE
						INSERT INTO [issuer_interface]
							([issuer_id], [interface_type_id], [connection_parameter_id])
						VALUES (@new_issuer_id, 0, @account_connection_id)
					END

					--INSERT CORE BANKING INTERFACE
					INSERT INTO [issuer_interface]
						([issuer_id], [interface_type_id], [connection_parameter_id])
					VALUES (@new_issuer_id, 1, @corebanking_connection_id)
				END


				--log the audit record
				DECLARE @audit_description varchar(max) = '',
						@country_name varchar(100),
						@country_code varchar(50),
						@issuer_status varchar(50)

				SELECT @country_name = country_name, @country_code = country_code
				FROM [country]
				WHERE country_id = @country_id

				SELECT @issuer_status = issuer_status_name
				FROM [issuer_statuses]
				WHERE issuer_status_id = @issuer_status_id


				SET @audit_description = 'Create: id: ' + CAST(@new_issuer_id AS VARCHAR(max))	+ 
										 ', name: ' + COALESCE(@issuer_name, 'UNKNOWN') +
										 ', code: ' + COALESCE(@issuer_code, 'UNKNOWN') +
										 ', country: ' + COALESCE(@country_code, 'UNKNOWN') + ';' + COALESCE(@country_name, 'UNKNOWN') +
										 ', status: ' + COALESCE(@issuer_status, 'UNKNOWN')
										 	
				EXEC sp_insert_audit @audit_user_id, 
									 4,---IssuerAdmin
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @new_issuer_id, NULL, NULL, NULL


				--INSERT DEFAULT USER GROUPS
				DECLARE @group_name varchar(50),
						@DataTable AS dbo.branch_id_array,
						@new_user_group_id int,
						@ResultCode2 int

				SELECT @group_name = @issuer_code + '_' + user_role
				FROM [user_roles]
				WHERE user_role_id = 4

				EXEC sp_create_user_group @group_name, 4, 
										  @new_issuer_id, 1, 1, 1, 1, 
										  @DataTable, -2, 'SYSTEM',
										  @new_user_group_id OUTPUT,
										  @ResultCode2 OUTPUT

				--Insert Default user groups
				--INSERT INTO [user_group]
				--	(all_branch_access, can_create, can_delete, can_read, can_update, issuer_id,
				--		user_group_name, user_role_id)
				--VALUES 
				--	(1, 1, 1, 1, 1, @new_issuer_id, @group_name, 4)

				SELECT @group_name = @issuer_code + '_' + user_role
				FROM [user_roles]
				WHERE user_role_id = 5

				EXEC sp_create_user_group @group_name, 5, 
									      @new_issuer_id, 1, 1, 1, 1, 
										  @DataTable, -2, 'SYSTEM',
										  @new_user_group_id OUTPUT,
										  @ResultCode2 OUTPUT
				--INSERT INTO [user_group]
				--	(all_branch_access, can_create, can_delete, can_read, can_update, issuer_id,
				--		user_group_name, user_role_id)
				--VALUES 
				--	(1, 1, 1, 1, 1, @new_issuer_id, @group_name, 5)	

				SET @ResultCode = 0		
					
			END

			COMMIT TRANSACTION [INSERT_ISSUER_TRAN]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_ISSUER_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_create_ldap]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo Setenane
-- Create date: 2014/04/03
-- Description:	Creates a new LDAP record associated and returns the new record's ID
-- =============================================
CREATE PROCEDURE [dbo].[sp_create_ldap]
	@ldap_setting_name varchar(100),
	@hostname_or_ip varchar(200),
	@path varchar(200),
	@domain_name varchar(100) = NULL,
	@auth_username varchar(200) = NULL,
	@auth_password varchar(200) = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@new_ldap_id int OUTPUT,
	@ResultCode int OUTPUT
		
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Check for duplicate's
	IF (SELECT COUNT(*) FROM [ldap_setting] WHERE [ldap_setting_name] = @ldap_setting_name) > 0
		BEGIN
			SET @new_ldap_id = 0
			SET @ResultCode = 225						
		END
	ELSE
	BEGIN 

		BEGIN TRANSACTION [INSERT_LDAP_TRAN]
			BEGIN TRY 
				IF @auth_username IS NULL
					SET @auth_username = ''

				IF @auth_password IS NULL
					SET @auth_password = ''

				OPEN SYMMETRIC KEY Indigo_Symmetric_Key
				DECRYPTION BY CERTIFICATE Indigo_Certificate

					INSERT INTO [dbo].[ldap_setting]
						([ldap_setting_name],[hostname_or_ip],[domain_name],[path],[username],[password])
					VALUES
							(@ldap_setting_name, @hostname_or_ip, @domain_name, @path, 
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@auth_username)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@auth_password)))		


					SET @new_ldap_id = SCOPE_IDENTITY();

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

					--log the audit record
					--DECLARE @audit_description varchar(500)
					--SELECT @audit_description = 'Inserted ldap ' + CONVERT(NVARCHAR, @new_ldap_id)	+ ')'			
					--EXEC sp_insert_audit @audit_user_id, 
					--						0,
					--						NULL, 
					--						@audit_workstation, 
					--						@audit_description, 
					--						NULL, NULL, NULL, NULL		

					SET @ResultCode = 0
					COMMIT TRANSACTION [INSERT_LDAP_TRAN]

			END TRY
		BEGIN CATCH
			ROLLBACK TRANSACTION [INSERT_LDAP_TRAN]
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




GO
/****** Object:  StoredProcedure [dbo].[sp_create_load_batch]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Richard Brenchley
-- Create date: 6 March 2014
-- Description:	Create's the load batch, load batch cards, load batch status and file history within a transaction.
-- =============================================
CREATE PROCEDURE [dbo].[sp_create_load_batch] 
	@load_batch_reference varchar(50),
	@batch_status_id int,
	@user_id bigint,	
	@load_card_status_id int,
	@card_list AS dbo.load_cards_type READONLY,
	@issuer_id int,
	@file_load_id INT,
	@name_of_file varchar(60),
	@file_created_date datetime,
	@file_size int,
	@load_date datetime,
	@file_status_id int,
	@file_directory varchar(110),
	@number_successful_records int,
	@number_failed_records int,
	@file_load_comments varchar(max),
	@file_type_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET QUERY_GOVERNOR_COST_LIMIT 0;

	BEGIN TRANSACTION [CREATE_LOAD_BATCH_TRAN]
	BEGIN TRY 

		DECLARE @RC int
		DECLARE @file_id bigint
		DECLARE @load_batch_id bigint
		DECLARE @number_of_cards int
		DECLARE @load_batch_status_id bigint

		--Inserts should happen in the following order
		--1. file_history
		--2. load_batch
		--3. load_batch_status
		--4. cards
		--5. load_batch_cards

		SET @number_of_cards = (SELECT COUNT(*) FROM @card_list)

		--Insert into file history
		EXECUTE @RC = [dbo].[sp_insert_file_history] @file_load_id, @issuer_id,@name_of_file,@file_created_date
													,@file_size,@load_date,@file_status_id,@file_directory
													,@number_successful_records,@number_failed_records
													,@file_load_comments,@file_type_id,@file_id OUTPUT

		--Insert into load_batch
		EXECUTE @RC = [dbo].[sp_insert_load_batch] @load_batch_reference,@file_id,@issuer_id,@batch_status_id
												  ,@load_date,@number_of_cards,@load_batch_id OUTPUT

		--Insert into load_batch_status
		EXECUTE @RC = [dbo].[sp_insert_load_batch_status] @load_batch_id,@batch_status_id,@load_date
														 ,@user_id,@load_batch_status_id OUTPUT

		
		OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
		DECRYPTION BY CERTIFICATE Indigo_Certificate
			DECLARE @objid int
			SET @objid = object_id('cards')

			MERGE [cards] AS cardsTable
			USING @card_list AS cardsList
			ON (DECRYPTBYKEY(cardsTable.card_number) = cardsList.card_number) 
			WHEN NOT MATCHED BY TARGET
				THEN INSERT ([product_id],[branch_id],[card_number],[card_sequence],[card_index]) 
					VALUES(cardsList.product_id,
						   cardsList.branch_id
						   ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR,cardsList.card_number))
						   ,cardsList.card_sequence
						   ,[dbo].[MAC](RIGHT(cardsList.card_number, 4), @objid));


			--Insert into load_batch_cards, links cards to load batch
			INSERT INTO load_batch_cards
				([load_batch_id], [card_id], [load_card_status_id])
			SELECT @load_batch_id, cards.card_id, @load_card_status_id
			FROM cards 
			WHERE DECRYPTBYKEY(cards.card_number) IN (SELECT cards_list.card_number
													  FROM @card_list cards_list
													  WHERE cards_list.branch_id = cards.branch_id)

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
					
			--declare @audit_msg nvarchar(50)
			--SET @audit_msg = 'INSERT :' + CAST(@load_batch_reference AS varchar(max))  +', LOADED'
			----log the audit record		
			--EXEC sp_insert_audit @user_id, 
			--						5, --LoadBatch
			--						NULL, 
			--						'LOADED', 
			--						@audit_msg, 
			--						NULL, NULL, NULL, NULL		

		COMMIT TRANSACTION [CREATE_LOAD_BATCH_TRAN]

	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_LOAD_BATCH_TRAN]
		RETURN ERROR_MESSAGE()
	END CATCH 

	RETURN '0'
END







GO
/****** Object:  StoredProcedure [dbo].[sp_create_user_group]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_create_user_group]
	@user_group_name varchar(50),
	@user_role_id int,
	@issuer_id int,
	@can_read bit,
	@can_create bit,
	@can_update bit,
	@all_branch_access bit,
	@branch_list AS dbo.branch_id_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@new_user_group_id int OUTPUT,
	@ResultCode int OUTPUT
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Check for duplicate's
	IF (SELECT COUNT(*) FROM [user_group] WHERE ([user_group_name] = @user_group_name AND [issuer_id] = @issuer_id)) > 0
		BEGIN
			SET @new_user_group_id = 0
			SET @ResultCode = 215						
		END
	ELSE
		BEGIN

		BEGIN TRANSACTION [CREATE_USER_GROUP_TRAN]
		BEGIN TRY 

			DECLARE @RC int


			INSERT INTO [user_group]
			   ([user_role_id], [issuer_id], 
				[can_create], [can_read], [can_update], [can_delete],
				[all_branch_access], [user_group_name])
			VALUES
			   (@user_role_id, @issuer_id, @can_create, @can_read, @can_update, 0, @all_branch_access, @user_group_name)

			SET @new_user_group_id = SCOPE_IDENTITY();

			--Link branches to user group
			EXECUTE @RC = [sp_insert_user_group_branches] @new_user_group_id, @branch_list, @audit_user_id, @audit_workstation

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
											WHERE ug.user_group_id = @new_user_group_id
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
			SET @audit_description = 'Create: ' + COALESCE(@user_group_name, 'UNKNOWN') +
									 ', iss:' + COALESCE(@issuer_code, 'UNKNOWN') + ';' + COALESCE(CAST(@issuer_id as varchar(max)), 'UNKNOWN') + 
									 ', read: ' + COALESCE(CAST(@can_read as varchar(1)), 'UNKNOWN') + 
									 ', create: ' + COALESCE(CAST(@can_create as varchar(1)), 'UNKNOWN') +
									 ', update: ' + COALESCE(CAST(@can_update as varchar(1)), 'UNKNOWN') +
									 ', branches: ' + COALESCE(@branches, 'UNKNOWN')

			EXEC sp_insert_audit @audit_user_id, 
								 8,----UserGroupAdmin
								 NULL, 
								 @audit_workstation, 
								 @audit_description, 
								 @issuer_id, NULL, NULL, NULL

			SET @ResultCode = 0
			COMMIT TRANSACTION [CREATE_USER_GROUP_TRAN]

		END TRY
			BEGIN CATCH
			ROLLBACK TRANSACTION [CREATE_USER_GROUP_TRAN]
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



GO
/****** Object:  StoredProcedure [dbo].[sp_delete_connenction_params]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_delete_connenction_params]
	@connection_parameter_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   BEGIN TRANSACTION [DELETE_CONNECTIONPARAMS_TRAN]
		BEGIN TRY 
			delete from [dbo].connection_parameters
				WHERE connection_parameter_id = @connection_parameter_id
		--DECLARE @audit_description varchar(500)
		--		SELECT @audit_description = 'Deleted Connection Parameters : ' + CONVERT(NVARCHAR, @connection_parameter_id)	+ ')'			
		--		EXEC sp_insert_audit @audit_user_id, 
		--							 2,
		--							 NULL, 
		--							 @audit_workstation, 
		--							 @audit_description, 
		--							 NULL, NULL, NULL, NULL	


				COMMIT TRANSACTION [DELETE_CONNECTIONPARAMS_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_CONNECTIONPARAMS_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_delete_font]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_delete_font]
@font_id int,
		@audit_user_id bigint,
	@audit_workstation varchar(100),
	@result_code int =null output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION [DELETE_FONT_TRAN]
	BEGIN TRY 
    -- Insert statements for procedure here
	update Issuer_product_font set [DeletedYN]=1 where font_id=@font_id

	DECLARE @audit_description varchar(500)
				--SELECT @audit_description = 'Font Deleted: ' + CAST(@font_id as nvarchar(100))
																	
				--EXEC sp_insert_audit @audit_user_id, 
				--					 0,
				--					 NULL, 
				--					 @audit_workstation, 
				--					 @audit_description, 
				--					 NULL, NULL, NULL, NULL
	 COMMIT TRANSACTION [DELETE_FONT_TRAN]

	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_FONT_TRAN]
		
	END CATCH 

END






GO
/****** Object:  StoredProcedure [dbo].[sp_delete_ldap]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_delete_ldap]
	@ldap_setting_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   BEGIN TRANSACTION [DELETE_LDAP_TRAN]
		BEGIN TRY 
			Delete from  [dbo].[ldap_setting]
				WHERE ldap_setting_id = @ldap_setting_id

		--DECLARE @audit_description varchar(500)
		--		SELECT @audit_description = 'Deleted ldap: ' + CONVERT(NVARCHAR, @ldap_setting_id)	+ ')'			
		--		EXEC sp_insert_audit @audit_user_id, 
		--							 2,
		--							 NULL, 
		--							 @audit_workstation, 
		--							 @audit_description, 
		--							 NULL, NULL, NULL, NULL	


				COMMIT TRANSACTION [DELETE_LDAP_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_LDAP_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_delete_product]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_delete_product]
	@productid int,
		@audit_user_id bigint,
	@audit_workstation varchar(100),
	@result_code int =null output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	update issuer_product set DeletedYN=1 where product_id=@productid

	--DECLARE @audit_description varchar(500)
	--			SELECT @audit_description = 'Product Deleted: ' + CAST(@productid as nvarchar(100))
																	
	--			EXEC sp_insert_audit @audit_user_id, 
	--								 0,
	--								 NULL, 
	--								 @audit_workstation, 
	--								 @audit_description, 
	--								 NULL, NULL, NULL, NULL

END




GO
/****** Object:  StoredProcedure [dbo].[sp_delete_user_group]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_delete_user_group] 
	-- Add the parameters for the stored procedure here
	@user_group_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [DELETE_USER_GROUP_TRAN]
		BEGIN TRY 

			DELETE FROM [user_groups_branches]
			WHERE user_group_id = @user_group_id

			DELETE FROM [users_to_users_groups]
			WHERE user_group_id = @user_group_id

			DELETE FROM [user_group]
			WHERE user_group_id = @user_group_id


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


GO
/****** Object:  StoredProcedure [dbo].[sp_dist_batch_approve]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Approves the distribution Batch
-- =============================================
CREATE PROCEDURE [dbo].[sp_dist_batch_approve] 
	@dist_batch_id bigint,
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

	BEGIN TRANSACTION [APPROVE_DIST_BATCH]
		BEGIN TRY 
			
			DECLARE @current_dist_batch_status_id int,
					@audit_msg varchar(max)

			--get the current status for the distribution batch
			SELECT @current_dist_batch_status_id = dist_batch_statuses_id
			FROM dist_batch_status
			WHERE status_date = (SELECT MAX(status_date)
								 FROM dist_batch_status
								 WHERE dist_batch_id = @dist_batch_id)
				  AND dist_batch_id = @dist_batch_id
										  
			--Check that someone hasn't already updated the dist batch
			IF(@current_dist_batch_status_id != 0)
				BEGIN
					SET @ResultCode = 1
				END
			ELSE
				BEGIN
					DECLARE @approve_dist_batch_statuses_id int
					SET @approve_dist_batch_statuses_id = 1

					--Update the dist batch status.
					INSERT dist_batch_status
							([dist_batch_id], [dist_batch_statuses_id], [user_id], [status_date], [status_notes])
					VALUES (@dist_batch_id, @approve_dist_batch_statuses_id, @audit_user_id, GETDATE(), @status_notes)

					--Update the cards linked to the dist batch with the new status.
					--UPDATE dist_batch_cards
					--SET dist_card_status_id = 3
					--WHERE dist_batch_id = @dist_batch_id

					DECLARE @dist_batch_status_name varchar(50),
							@dist_batch_ref varchar(100)

					SELECT @dist_batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = @approve_dist_batch_statuses_id

					SELECT @dist_batch_ref = dist_batch_reference
					FROM dist_batch
					WHERE dist_batch_id = @dist_batch_id

					--Add audit for dist batch update								
					SET @audit_msg = 'Update: ' + CAST(@dist_batch_id AS varchar(max)) +
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
				END

				--Fetch the distribution batch with latest details
				EXEC sp_get_dist_batch @dist_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation


				COMMIT TRANSACTION [APPROVE_DIST_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [APPROVE_DIST_BATCH]
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
/****** Object:  StoredProcedure [dbo].[sp_dist_batch_dispatch]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Dispatch the distribution batch
-- =============================================
CREATE PROCEDURE [dbo].[sp_dist_batch_dispatch] 
	@dist_batch_id bigint,
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

	BEGIN TRANSACTION [DISPATCH_DIST_BATCH]
		BEGIN TRY 
			
			DECLARE @current_dist_batch_status_id int,
					@audit_msg varchar(max)

			--get the current status for the distribution batch
			SELECT @current_dist_batch_status_id = dist_batch_statuses_id
			FROM dist_batch_status
			WHERE status_date = (SELECT MAX(status_date)
								 FROM dist_batch_status
								 WHERE dist_batch_id = @dist_batch_id)
				  AND dist_batch_id = @dist_batch_id
										  
			--Check that someone hasn't already updated the dist batch
			IF(@current_dist_batch_status_id != 1)
				BEGIN
					SET @ResultCode = 1
				END
			ELSE
				BEGIN
					DECLARE @dispatch_dist_batch_statuses_id int
					SET @dispatch_dist_batch_statuses_id = 2

					--Update the dist batch status.
					INSERT dist_batch_status
							([dist_batch_id], [dist_batch_statuses_id], [user_id], [status_date], [status_notes])
					VALUES (@dist_batch_id, @dispatch_dist_batch_statuses_id, @audit_user_id, GETDATE(), @status_notes)

					--Update the cards linked to the dist batch with the new status.
					--UPDATE dist_batch_cards
					--SET dist_card_status_id = 3
					--WHERE dist_batch_id = @dist_batch_id
				
					DECLARE @dist_batch_status_name varchar(50),
							@dist_batch_ref varchar(100)

					SELECT @dist_batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = @dispatch_dist_batch_statuses_id

					SELECT @dist_batch_ref = dist_batch_reference
					FROM dist_batch
					WHERE dist_batch_id = @dist_batch_id

					--Add audit for dist batch update								
					SET @audit_msg = 'Update: ' + CAST(@dist_batch_id AS varchar(max)) +
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
				END

				--Fetch the distribution batch with latest details
				EXEC sp_get_dist_batch @dist_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [DISPATCH_DIST_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DISPATCH_DIST_BATCH]
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
/****** Object:  StoredProcedure [dbo].[sp_dist_batch_received]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Update status of distribution batch to received at branch
-- =============================================
CREATE PROCEDURE [dbo].[sp_dist_batch_received] 
	@dist_batch_id bigint,
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

	BEGIN TRANSACTION [RECEIVE_DIST_BATCH]
		BEGIN TRY 
			
			DECLARE @current_dist_batch_status_id int,
					@audit_msg varchar(max)

			--get the current status for the distribution batch
			SELECT @current_dist_batch_status_id = dist_batch_statuses_id
			FROM dist_batch_status
			WHERE status_date = (SELECT MAX(status_date)
								 FROM dist_batch_status
								 WHERE dist_batch_id = @dist_batch_id)
				  AND dist_batch_id = @dist_batch_id
										  
			--Check that someone hasn't already updated the dist batch
			IF(@current_dist_batch_status_id != 2)
				BEGIN
					SET @ResultCode = 1
				END
			ELSE
				BEGIN
					DECLARE @received_dist_batch_statuses_id int
					SET @received_dist_batch_statuses_id = 3

					--Update the dist batch status.
					INSERT dist_batch_status
							([dist_batch_id], [dist_batch_statuses_id], [user_id], [status_date], [status_notes])
					VALUES (@dist_batch_id, @received_dist_batch_statuses_id, @audit_user_id, GETDATE(), @status_notes)

					--Update the cards linked to the dist batch with the new status.
					UPDATE dist_batch_cards
					SET dist_card_status_id = 2
					WHERE dist_batch_id = @dist_batch_id


					--Insert cards into branch status as checked in.
					INSERT branch_card_status
							(card_id, branch_card_statuses_id, status_date, [user_id])
					SELECT card_id, 0, GETDATE(), @audit_user_id
					FROM dist_batch_cards
					WHERE dist_batch_id = @dist_batch_id

					--Audit record stuff
					DECLARE @dist_batch_status_name varchar(50),
							@dist_batch_ref varchar(100)

					SELECT @dist_batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = @received_dist_batch_statuses_id

					SELECT @dist_batch_ref = dist_batch_reference
					FROM dist_batch
					WHERE dist_batch_id = @dist_batch_id

					--Add audit for dist batch update								
					SET @audit_msg = 'Update: ' + CAST(@dist_batch_id AS varchar(max)) +
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
				END

				--Fetch the distribution batch with latest details
				EXEC sp_get_dist_batch @dist_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [RECEIVE_DIST_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [RECEIVE_DIST_BATCH]
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
/****** Object:  StoredProcedure [dbo].[sp_dist_batch_reject]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Reject the distribution batch
-- =============================================
CREATE PROCEDURE [dbo].[sp_dist_batch_reject] 
	@dist_batch_id bigint,
	@status_notes varchar(150),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [REJECT_DIST_BATCH]
		BEGIN TRY 
			
			DECLARE @current_dist_batch_status_id int,
					@audit_msg varchar(max)

			--get the current status for the distribution batch
			SELECT @current_dist_batch_status_id = dist_batch_statuses_id
			FROM dist_batch_status
			WHERE status_date = (SELECT MAX(status_date)
								 FROM dist_batch_status
								 WHERE dist_batch_id = @dist_batch_id)
				  AND dist_batch_id = @dist_batch_id
										  
			--Check that someone hasn't already updated the dist batch
			IF(@current_dist_batch_status_id != 0)
				BEGIN
					SET @ResultCode = 1
				END
			ELSE
				BEGIN
					DECLARE @reject_dist_batch_statuses_id int
					SET @reject_dist_batch_statuses_id = 8

					--Update the dist batch status.
					INSERT dist_batch_status
							([dist_batch_id], [dist_batch_statuses_id], [user_id], [status_date], [status_notes])
					VALUES (@dist_batch_id, @reject_dist_batch_statuses_id, @audit_user_id, GETDATE(), @status_notes)

					--Update the cards linked to the dist batch with the new status.
					UPDATE dist_batch_cards
					SET dist_card_status_id = 7
					WHERE dist_batch_id = @dist_batch_id

					
					--Audit record stuff
					DECLARE @dist_batch_status_name varchar(50),
							@dist_batch_ref varchar(100)

					SELECT @dist_batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = @reject_dist_batch_statuses_id

					SELECT @dist_batch_ref = dist_batch_reference
					FROM dist_batch
					WHERE dist_batch_id = @dist_batch_id

					--Add audit for dist batch update								
					SET @audit_msg = 'Update: ' + CAST(@dist_batch_id AS varchar(max)) +
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
				END

				--Fetch the distribution batch with latest details
				EXEC sp_get_dist_batch @dist_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [REJECT_DIST_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [REJECT_DIST_BATCH]
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
/****** Object:  StoredProcedure [dbo].[sp_dist_batch_reject_and_destroy]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Reject distribution batch and destroy cards
-- =============================================
CREATE PROCEDURE [dbo].[sp_dist_batch_reject_and_destroy] 
	@dist_batch_id bigint,
	@status_notes varchar(150),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [REJECT_AND_DESTROY_DIST_BATCH]
		BEGIN TRY 
			
			DECLARE @current_dist_batch_status_id int,
					@audit_msg varchar(max)

			--get the current status for the distribution batch
			SELECT @current_dist_batch_status_id = dist_batch_statuses_id
			FROM dist_batch_status
			WHERE status_date = (SELECT MAX(status_date)
								 FROM dist_batch_status
								 WHERE dist_batch_id = @dist_batch_id)
				  AND dist_batch_id = @dist_batch_id
										  
			--Check that someone hasn't already updated the dist batch
			IF(@current_dist_batch_status_id != 4)
				BEGIN
					SET @ResultCode = 1
				END
			ELSE
				BEGIN
					DECLARE @reject_destroy_dist_batch_statuses_id int
					SET @reject_destroy_dist_batch_statuses_id = 6

					--Update the dist batch status.
					INSERT dist_batch_status
							([dist_batch_id], [dist_batch_statuses_id], [user_id], [status_date], [status_notes])
					VALUES (@dist_batch_id, @reject_destroy_dist_batch_statuses_id, @audit_user_id, GETDATE(), @status_notes)

					--Update the cards linked to the dist batch with the new status.
					UPDATE dist_batch_cards
					SET dist_card_status_id = 7
					WHERE dist_batch_id = @dist_batch_id


					--Audit record stuff
					DECLARE @dist_batch_status_name varchar(50),
							@dist_batch_ref varchar(100)

					SELECT @dist_batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = @reject_destroy_dist_batch_statuses_id

					SELECT @dist_batch_ref = dist_batch_reference
					FROM dist_batch
					WHERE dist_batch_id = @dist_batch_id

					--Add audit for dist batch update								
					SET @audit_msg = 'Update: ' + CAST(@dist_batch_id AS varchar(max)) +
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
				END

				--Fetch the distribution batch with latest details
				EXEC sp_get_dist_batch @dist_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [REJECT_AND_DESTROY_DIST_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [REJECT_AND_DESTROY_DIST_BATCH]
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
/****** Object:  StoredProcedure [dbo].[sp_dist_batch_reject_and_reissue]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Reject the distribution batch and make cards available for distribution
-- =============================================
CREATE PROCEDURE [dbo].[sp_dist_batch_reject_and_reissue] 
	@dist_batch_id bigint,
	@status_notes varchar(150),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [REJECT_AND_REISSUE_DIST_BATCH]
		BEGIN TRY 
			
			DECLARE @current_dist_batch_status_id int,
					@audit_msg varchar(max)

			--get the current status for the distribution batch
			SELECT @current_dist_batch_status_id = dist_batch_statuses_id
			FROM dist_batch_status
			WHERE status_date = (SELECT MAX(status_date)
								 FROM dist_batch_status
								 WHERE dist_batch_id = @dist_batch_id)
				  AND dist_batch_id = @dist_batch_id
										  
			--Check that someone hasn't already updated the dist batch
			IF(@current_dist_batch_status_id != 4)
				BEGIN
					SET @ResultCode = 1
				END
			ELSE
				BEGIN
					DECLARE @reject_reissue_dist_batch_statuses_id int
					SET @reject_reissue_dist_batch_statuses_id = 5

					--Update the dist batch status.
					INSERT dist_batch_status
							([dist_batch_id], [dist_batch_statuses_id], [user_id], [status_date], [status_notes])
					VALUES (@dist_batch_id, @reject_reissue_dist_batch_statuses_id, @audit_user_id, GETDATE(), @status_notes)

					--Update the cards linked to the dist batch with rejected status.
					UPDATE [dist_batch_cards]
					SET dist_card_status_id = 7
					WHERE dist_batch_id = @dist_batch_id

					--update cards in load batch cards to available status
					--Load cards should be in an allocated status.
					UPDATE [load_batch_cards]
					SET [load_batch_cards].load_card_status_id = 1
					FROM [load_batch_cards]
							INNER JOIN [dist_batch_cards]
								ON [load_batch_cards].card_id = [dist_batch_cards].card_id
					WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id
							AND [load_batch_cards].load_card_status_id = 2


					--Audit record stuff
					DECLARE @dist_batch_status_name varchar(50),
							@dist_batch_ref varchar(100)

					SELECT @dist_batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = @reject_reissue_dist_batch_statuses_id

					SELECT @dist_batch_ref = dist_batch_reference
					FROM dist_batch
					WHERE dist_batch_id = @dist_batch_id

					--Add audit for dist batch update								
					SET @audit_msg = 'Update: ' + CAST(@dist_batch_id AS varchar(max)) +
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
				END

				--Fetch the distribution batch with latest details
				EXEC sp_get_dist_batch @dist_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [REJECT_AND_REISSUE_DIST_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [REJECT_AND_REISSUE_DIST_BATCH]
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
/****** Object:  StoredProcedure [dbo].[sp_dist_batch_reject_branch]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Set distribution batch status to rejected at branch
-- =============================================
CREATE PROCEDURE [dbo].[sp_dist_batch_reject_branch] 
	@dist_batch_id bigint,
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

	BEGIN TRANSACTION [BRANCH_REJECT_DIST_BATCH]
		BEGIN TRY 
			
			DECLARE @current_dist_batch_status_id int,
					@audit_msg varchar(max)

			--get the current status for the distribution batch
			SELECT @current_dist_batch_status_id = dist_batch_statuses_id
			FROM dist_batch_status
			WHERE status_date = (SELECT MAX(status_date)
								 FROM dist_batch_status
								 WHERE dist_batch_id = @dist_batch_id)
				  AND dist_batch_id = @dist_batch_id
										  
			--Check that someone hasn't already updated the dist batch
			IF(@current_dist_batch_status_id != 2)
				BEGIN
					SET @ResultCode = 1
				END
			ELSE
				BEGIN
					DECLARE @reject_branch_dist_batch_statuses_id int
					SET @reject_branch_dist_batch_statuses_id = 4

					--Update the dist batch status.
					INSERT dist_batch_status
							([dist_batch_id], [dist_batch_statuses_id], [user_id], [status_date], [status_notes])
					VALUES (@dist_batch_id, @reject_branch_dist_batch_statuses_id, @audit_user_id, GETDATE(), @status_notes)


					--Audit record stuff
					DECLARE @dist_batch_status_name varchar(50),
							@dist_batch_ref varchar(100)

					SELECT @dist_batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = @reject_branch_dist_batch_statuses_id

					SELECT @dist_batch_ref = dist_batch_reference
					FROM dist_batch
					WHERE dist_batch_id = @dist_batch_id

					--Add audit for dist batch update								
					SET @audit_msg = 'Update: ' + CAST(@dist_batch_id AS varchar(max)) +
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
				END

				--Fetch the distribution batch with latest details
				EXEC sp_get_dist_batch @dist_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [BRANCH_REJECT_DIST_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [BRANCH_REJECT_DIST_BATCH]
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
/****** Object:  StoredProcedure [dbo].[sp_dist_batch_resend_to_branch]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Resend distribution batch back to branch
-- =============================================
CREATE PROCEDURE [dbo].[sp_dist_batch_resend_to_branch] 
	@dist_batch_id bigint,
	@status_notes varchar(150),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [RESEND_DIST_BATCH]
		BEGIN TRY 
			
			DECLARE @current_dist_batch_status_id int,
					@audit_msg varchar(max)

			--get the current status for the distribution batch
			SELECT @current_dist_batch_status_id = dist_batch_statuses_id
			FROM dist_batch_status
			WHERE status_date = (SELECT MAX(status_date)
								 FROM dist_batch_status
								 WHERE dist_batch_id = @dist_batch_id)
				  AND dist_batch_id = @dist_batch_id
										  
			--Check that someone hasn't already updated the dist batch
			IF(@current_dist_batch_status_id != 4)
				BEGIN
					SET @ResultCode = 1
				END
			ELSE
				BEGIN
					DECLARE @resend_dist_batch_statuses_id int
					SET @resend_dist_batch_statuses_id = 1

					--Update the dist batch status.
					INSERT dist_batch_status
							([dist_batch_id], [dist_batch_statuses_id], [user_id], [status_date], [status_notes])
					VALUES (@dist_batch_id, @resend_dist_batch_statuses_id, @audit_user_id, GETDATE(), @status_notes)

					--Audit record stuff
					DECLARE @dist_batch_status_name varchar(50),
							@dist_batch_ref varchar(100)

					SELECT @dist_batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = @resend_dist_batch_statuses_id

					SELECT @dist_batch_ref = dist_batch_reference
					FROM dist_batch
					WHERE dist_batch_id = @dist_batch_id

					--Add audit for dist batch update								
					SET @audit_msg = 'Update: ' + CAST(@dist_batch_id AS varchar(max)) +
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
				END

				--Fetch the distribution batch with latest details
					EXEC sp_get_dist_batch @dist_batch_id,
										   @language_id,
										   @audit_user_id,
										   @audit_workstation

				COMMIT TRANSACTION [RESEND_DIST_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [RESEND_DIST_BATCH]
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
/****** Object:  StoredProcedure [dbo].[sp_file_load_create]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Insert new file load
-- =============================================
CREATE PROCEDURE [dbo].[sp_file_load_create] 
	@file_load_start DATETIME, 
	@file_load_end DATETIME = NULL, 
	@user_id BIGINT, 
	@files_to_process int, 
	@audit_user_id BIGINT, 
	@audit_workstation VARCHAR(100),
	@file_load_id INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [file_load]
           ([file_load_start], [file_load_end], [user_id], [files_to_process])
     VALUES
			(@file_load_start, @file_load_end, @user_id, @files_to_process)

	SET @file_load_id = SCOPE_IDENTITY();
END


GO
/****** Object:  StoredProcedure [dbo].[sp_file_load_update]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Insert new file load
-- =============================================
CREATE PROCEDURE [dbo].[sp_file_load_update]
	@file_load_id INT,
	@file_load_end DATETIME, 
	@audit_user_id BIGINT, 
	@audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE [file_load]
		SET [file_load_end] = @file_load_end
   WHERE file_load_id = @file_load_id

END


GO
/****** Object:  StoredProcedure [dbo].[sp_finalise_login]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 3 March 2014
-- Description:	This stored procedure will fanalise the login by updating last login, workstation etc.
-- =============================================
CREATE PROCEDURE [dbo].[sp_finalise_login] 
	@user_id bigint,
    @workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate;

		BEGIN TRANSACTION [FINALISE_LOGIN_TRAN]
		BEGIN TRY

			--Update workstation and last login
			UPDATE [user]
			SET workstation = @workstation,
				[online] = 1,
				last_login_attempt = GETDATE(),
				last_login_date = GETDATE(),
				number_of_incorrect_logins = 0
			WHERE [user_id] = @user_id

			--log the audit record
			DECLARE @audit_description varchar(500)
			SELECT @audit_description = 'Login success'		
			EXEC sp_insert_audit @user_id, 
									6,---Logon
									NULL, 
									@workstation, 
									@audit_description, 
									NULL, NULL, NULL, NULL

		COMMIT TRANSACTION [FINALISE_LOGIN_TRAN]

		END TRY
		BEGIN CATCH
			ROLLBACK TRANSACTION [FINALISE_LOGIN_TRAN]
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

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

	RETURN

END




GO
/****** Object:  StoredProcedure [dbo].[sp_finalise_login_failed]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Selebalo Setenane
-- Create date: 11 March 2014
-- Description:	This stored procedure will fanalise the failed login by updating last login, workstation etc.
-- =============================================

CREATE PROCEDURE [dbo].[sp_finalise_login_failed] 
    @user_id bigint,    
	@audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [FINALISE_LOGIN_FAILED_TRAN]
		BEGIN TRY
			
			DECLARE @incorrect_login_attempts int

			--Fetch number of incorrect login attempts
			SELECT @incorrect_login_attempts = number_of_incorrect_logins
			FROM [user]
			WHERE [user].[user_id] = @user_id

			--Update last login and increment the incorrect login attempts
			UPDATE [user]
			SET last_login_attempt = GETDATE(),
				number_of_incorrect_logins = (@incorrect_login_attempts + 1)
			WHERE [user].[user_id] = @user_id
		
			--log the audit record
			DECLARE @audit_description varchar(500)
			SELECT @audit_description = 'Login failed'			
			EXEC sp_insert_audit @user_id, 
									6,---Logon
									NULL, 
									@audit_workstation, 
									@audit_description, 
									NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [FINALISE_LOGIN_FAILED_TRAN]

		END TRY
		BEGIN CATCH
		  ROLLBACK TRANSACTION [FINALISE_LOGIN_FAILED_TRAN]
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
	RETURN
END




GO
/****** Object:  StoredProcedure [dbo].[sp_finalise_logout]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Update user logout status
-- =============================================
CREATE PROCEDURE [dbo].[sp_finalise_logout] 
	@user_id BIGINT,
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

        BEGIN TRANSACTION [FINALISE_LOGOUT_TRAN]
			BEGIN TRY 			

				DECLARE @workstation varchar(100),
						@username varchar(100)

				OPEN SYMMETRIC KEY Indigo_Symmetric_Key
					DECRYPTION BY CERTIFICATE Indigo_Certificate;

					SELECT @workstation = workstation, @username = CONVERT(VARCHAR,DECRYPTBYKEY(username))
					FROM [user]
					WHERE [user_id] = @user_id

				CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

				--Update workstation and last login
				UPDATE [user]
				SET workstation = '',
					[online] = 0
				WHERE [user_id] = @user_id

				--log the audit record
				IF (@user_id = @audit_user_id)
					BEGIN
						EXEC sp_insert_audit @audit_user_id, 
									 6,--Logon
									 NULL, 
									 @audit_workstation,
									 'Logged off', 									  
									 NULL, NULL, NULL, NULL	
					END
				ELSE
					BEGIN
						DECLARE @audit_description varchar(500)

						SET @audit_description = 'Resetting user login status: ' + @username + ',' + @workstation

						EXEC sp_insert_audit @audit_user_id, 
									 7,--UserAdmin
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL	
					END					

			COMMIT TRANSACTION [FINALISE_LOGOUT_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [FINALISE_LOGOUT_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_find_distinct_load_cards]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 5 April 2014
-- Description:	Find all distinct card numbers from load cards based on supplied card numbers list
--				and returns the matched cards.
--				Cards that are attached to a load batch which has been rejected are not considered
--				duplicates in the system as they are not active. When creating a new load batch
--				a merege on the card table will happen to stop duplicate card number from entereing
--				the system.
-- =============================================
CREATE PROCEDURE [dbo].[sp_find_distinct_load_cards] 
	-- Add the parameters for the stored procedure here
	@card_list dbo.DistBatchCards READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT DISTINCT CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number))
		FROM [cards] 
			INNER JOIN [load_batch_cards]
				ON [cards].card_id = [load_batch_cards].card_id
			INNER JOIN [load_batch]
				ON [load_batch_cards].load_batch_id = [load_batch].load_batch_id
			INNER JOIN [load_batch_status_current]
				ON [load_batch_status_current].load_batch_id = [load_batch].load_batch_id
		WHERE [load_batch_status_current].load_batch_statuses_id NOT IN (2, 3)
			  AND CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) IN (SELECT cardlist.card_number
																		 FROM @card_list cardlist )
																		
      
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[sp_find_file_info_by_filename]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchey
-- Create date: 5 March 2014
-- Description:	Find file info based on filename.
-- =============================================
CREATE PROCEDURE [dbo].[sp_find_file_info_by_filename]
	-- Add the parameters for the stored procedure here
	@filename varchar(60) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [file_history].*
	FROM [file_history]
		INNER JOIN [load_batch]
			ON [load_batch].[file_id] =[file_history].[file_id]
		INNER JOIN [load_batch_status_current]
			ON [load_batch_status_current].load_batch_id = [load_batch].load_batch_id
	WHERE [file_history].name_of_file = @filename
		  AND [load_batch_status_current].load_batch_statuses_id NOT IN (2, 3)

END




GO
/****** Object:  StoredProcedure [dbo].[sp_find_issuer_by_status]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 5 March 2014
-- Description:	Find issuers based on status
-- =============================================
CREATE PROCEDURE [dbo].[sp_find_issuer_by_status] 
	-- Add the parameters for the stored procedure here
	@issuer_status_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT * 
	FROM issuer 
	WHERE issuer_status_id = @issuer_status_id

END




GO
/****** Object:  StoredProcedure [dbo].[sp_find_load_card]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 5 March 2014
-- Description:	Return cards from load_card based on card number.
-- =============================================
CREATE PROCEDURE [dbo].[sp_find_load_card] 
	-- Add the parameters for the stored procedure here
	@card_number varchar(20) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
    DECRYPTION BY CERTIFICATE Indigo_Certificate;
		SELECT card_number 
		FROM load_card 
		WHERE DECRYPTBYKEY(card_number) =  @card_number
    CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END




GO
/****** Object:  StoredProcedure [dbo].[sp_find_load_cards]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 5 April 2014
-- Description:	Find all distinct card numbers from load cards based on supplied card numbers list
--				and returns the number of matched cards.
-- =============================================
CREATE PROCEDURE [dbo].[sp_find_load_cards] 
	-- Add the parameters for the stored procedure here
	@card_list dbo.DistBatchCards READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT COUNT(DISTINCT CONVERT(VARCHAR,DECRYPTBYKEY(load_card.card_number)))
		FROM load_card
		WHERE CONVERT(VARCHAR,DECRYPTBYKEY(load_card.card_number)) IN (
				SELECT cardlist.card_number
				FROM @card_list cardlist )
      
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[sp_find_reissue_cards]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 28 March 2014
-- Description:	Search for card/s based on input parameters
-- =============================================
CREATE PROCEDURE [dbo].[sp_find_reissue_cards] 
	-- Add the parameters for the stored procedure here
	@user_id bigint,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [CARD_REISSUE_SEARCH_TRAN]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			--THIS IS FOR QUICKER CARD LOOKUP
			DECLARE @objid int
			SET @objid = object_id('cards')
			
			DECLARE @StartRow INT, @EndRow INT;			
			
			SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
			SET @EndRow = @StartRow + @RowsPerPage - 1;

			--append#1
			WITH PAGE_ROWS
			AS
			(
			SELECT ROW_NUMBER() OVER(ORDER BY card_number ASC) AS ROW_NO
					, COUNT(*) OVER() AS TOTAL_ROWS			
					, *
			FROM( 				
				SELECT DISTINCT [cards].card_id, 
					   CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) AS card_number,
					   --[load_batch].load_date, 
					   --[load_batch].load_batch_id,
					   --[load_card_statuses].load_card_status, 
					  -- [dist_batch].date_created,					   
					  --[dist_batch].dist_batch_id,
					 --  [dist_card_statuses].dist_card_status_name,
					 --  [branch_card_statuses].branch_card_statuses_name,
					   
					   COALESCE([branch_card_statuses].branch_card_statuses_name, 
								[dist_card_statuses].dist_card_status_name,
								[load_card_statuses].load_card_status) as current_card_status,
					   COALESCE([branch_card_status_current].status_date, 
								[dist_batch_status_current].status_date,
								[load_batch_status_current].status_date) as status_date,
					   [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
					   [branch].branch_id, [branch].branch_name, [branch].branch_code
				FROM [cards]						
					INNER JOIN [load_batch_cards]
						ON [cards].card_id = [load_batch_cards].card_id
					INNER JOIN [load_batch]
						ON [load_batch].load_batch_id = [load_batch_cards].load_batch_id
					INNER JOIN [load_batch_status_current]
						ON [load_batch].load_batch_id = [load_batch_status_current].load_batch_id
					INNER JOIN [load_card_statuses]
						ON [load_card_statuses].load_card_status_id = [load_batch_cards].load_card_status_id
					--Dist batch joins
					LEFT OUTER JOIN [dist_batch_cards]
						ON [cards].card_id = [dist_batch_cards].card_id						
					LEFT OUTER JOIN [dist_batch]
						ON [dist_batch].dist_batch_id = [dist_batch_cards].dist_batch_id
					LEFT OUTER JOIN [dist_batch_status_current]
						ON [dist_batch].dist_batch_id = [dist_batch_status_current].dist_batch_id				
					LEFT OUTER JOIN [dist_card_statuses]
						ON [dist_card_statuses].dist_card_status_id = [dist_batch_cards].dist_card_status_id
					--branch card joins
					LEFT OUTER JOIN [branch_card_status_current]
						ON [branch_card_status_current].card_id = [cards].card_id						   
					LEFT OUTER JOIN [branch_card_statuses]
						ON [branch_card_status_current].branch_card_statuses_id = [branch_card_statuses].branch_card_statuses_id						
					--Filter out cards linked to branches the user doesnt have access to.
					INNER JOIN (SELECT DISTINCT branch_id								
								FROM [user_roles_branch] INNER JOIN [user_roles]
										ON [user_roles_branch].user_role_id = [user_roles].user_role_id		
								WHERE [user_roles_branch].[user_id] = @user_id
										AND [user_roles].user_role_id IN (3)--Only want roles that allowed to search cards										
								) as X
						ON [cards].branch_id = X.branch_id
					INNER JOIN [branch]
						ON [branch].branch_id = [cards].branch_id
					INNER JOIN [issuer]
						ON [issuer].issuer_id = [branch].issuer_id
				WHERE [branch].branch_status_id = 0	 
					  AND [issuer].issuer_status_id = 0
					  AND (([issuer].pin_mailer_printing_YN = 0 AND [branch_card_status_current].branch_card_statuses_id = 4)
							OR
						    ([issuer].pin_mailer_printing_YN = 1 AND [branch_card_status_current].branch_card_statuses_id = 5))

			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY card_number ASC

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

			--log the audit record		
			--EXEC sp_insert_audit @user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 'Getting cards for card search.', 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [CARD_REISSUE_SEARCH_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CARD_REISSUE_SEARCH_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_generate_audit_report]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_generate_audit_report]
	-- Add the parameters for the stored procedure here
	@User		varchar(30) = null,
	@ActionType varchar(30) = null,	
	@IssuerID int = 0,
	@DateFrom		datetime = null,
	@DateTo		datetime = null,
	@Keyword  varchar (50) = null

AS
BEGIN

IF (@User = '') OR (@User = 'All') OR (@User like 'null')
	BEGIN
	 SET @User =NULL
	END

IF (@ActionType = '' ) OR (@ActionType = 'All' ) OR (@ActionType like 'null')
	BEGIN
		 SET @ActionType =NULL
	END


IF(@DateFrom = '1900-01-01 00:00:00.000') OR (@DateFrom=null)
		BEGIN			
			SET @DateFrom = NULL
		END

IF(@DateTo= '1900-01-01 00:00:00.000') OR (@DateTo=null)
		BEGIN			
			SET @DateTo = NULL
		END

	IF(@IssuerID = 0)
		BEGIN			
			SET @IssuerID = NULL
		END

IF (@Keyword = '')  OR (@Keyword like 'null')
	BEGIN
		 SET @Keyword =''
	END



SELECT   *


FROM            audit_controll audit

WHERE 	audit.user_action  =  COALESCE(@ActionType ,audit.user_action)
		AND audit.user_audit = COALESCE(@User ,audit.user_audit)
		AND audit.issuer_id = COALESCE(@IssuerID ,audit.issuer_id ) 
		AND audit.audit_date >= COALESCE(@DateFrom, audit.audit_date) 
		AND audit.audit_date <= COALESCE(DATEADD(dd,1,@DateTo), audit.audit_date)
		AND audit.action_description like '%'+@Keyword+'%'

END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_all_countries]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_all_countries] 
	@audit_user_id BIGINT,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT *
	FROM [country]
END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_all_issuers]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo Setenane
-- Create date: 2014/03/19
-- Description:	Returns all the issuers and their interfaces
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_all_issuers]
	 @languageId int =null,
	 @PageIndex INT = 1,
	@RowsPerPage INT = 20
AS
BEGIN
	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY issuer_name ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
	SELECT  distinct      issuer.issuer_id, issuer.issuer_name, issuer.issuer_code, issuer_statuses_language.language_text as 'issuer_status'
FROM            issuer INNER JOIN
                issuer_statuses 
				ON issuer.issuer_status_id = issuer_statuses.issuer_status_id 
				INNER JOIN
                issuer_statuses_language ON 
			    issuer_statuses.issuer_status_id = issuer_statuses_language.issuer_status_id
				where issuer_statuses_language.[language_id]=@languageId
					 )
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY issuer_name ASC
	--SELECT 
	--[dbo].[issuer_id] AS [issuer_id], 
	--[dbo].[issuer_name] AS [issuer_name], 
	--[dbo].[issuer_code] AS [issuer_code], 
	--[dbo].[issuer_status] AS [issuer_status], 
	--[dbo].[license_file] AS [license_file], 
	--[dbo].[cards_file_location] AS [cards_file_location], 
	--[dbo].[pins_file_location] AS [pins_file_location], 
	--[dbo].[encrypted_ZPK] AS [encrypted_ZPK], 
	--[dbo].[instant_card_issue_YN] AS [instant_card_issue_YN], 
	--[dbo].[pin_mailer_printing_YN] AS [pin_mailer_printing_YN], 
	--[dbo].[delete_pin_file_YN] AS [delete_pin_file_YN], 
	--[dbo].[delete_card_file_YN] AS [delete_card_file_YN], 
	--[dbo].[pin_mailer_file_type] AS [pin_mailer_file_type], 
	--[dbo].[card_file_type] AS [card_file_type], 
	--[dbo].[pin_printer_name] AS [pin_printer_name], 
	--[dbo].[encrypted_PWK] AS [encrypted_PWK], 
	--[dbo].[C1] AS [C1], 
	--[dbo].[id] AS [id], 
	--[dbo].[interface_type_id] AS [interface_type_id], 
	--[dbo].[issuer_issuer_id] AS [issuer_issuer_id]
	--FROM ( SELECT 
	--	[dbo].[issuer_id] AS [issuer_id], 
	--	[dbo].[issuer_name] AS [issuer_name], 
	--	[dbo].[issuer_code] AS [issuer_code], 
	--	[dbo].[issuer_status] AS [issuer_status], 
	--	[dbo].[license_file] AS [license_file], 
	--	[dbo].[cards_file_location] AS [cards_file_location], 
	--	[dbo].[pins_file_location] AS [pins_file_location], 
	--	[dbo].[encrypted_ZPK] AS [encrypted_ZPK], 
	--	[dbo].[instant_card_issue_YN] AS [instant_card_issue_YN], 
	--	[dbo].[pin_mailer_printing_YN] AS [pin_mailer_printing_YN], 
	--	[dbo].[delete_pin_file_YN] AS [delete_pin_file_YN], 
	--	[dbo].[delete_card_file_YN] AS [delete_card_file_YN], 
	--	[dbo].[pin_mailer_file_type] AS [pin_mailer_file_type], 
	--	[dbo].[card_file_type] AS [card_file_type], 
	--	[dbo].[pin_printer_name] AS [pin_printer_name], 
	--	[dbo].[encrypted_PWK] AS [encrypted_PWK], 
	--	[Extent2].[id] AS [id], 
	--	[Extent2].[interface_type_id] AS [interface_type_id], 
	--	[Extent2].[issuer_issuer_id] AS [issuer_issuer_id], 
	--	CASE WHEN ([Extent2].[id] IS NULL) THEN CAST(NULL AS int) ELSE 1 END AS [C1]
	--	FROM  [dbo].[issuer] AS [dbo]
	--	LEFT OUTER JOIN [dbo].[interface] AS [Extent2] ON [dbo].[issuer_id] = [Extent2].[issuer_issuer_id]
	--)  AS [dbo]
	--ORDER BY [dbo].[issuer_id] ASC, [dbo].[C1] ASC
	END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_all_issuers_for_role]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_all_issuers_for_role] 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
		   [issuer].maker_checker_YN, [issuer].account_validation_YN, 
		   [issuer].auto_create_dist_batch
	FROM [issuer]
	WHERE issuer_status_id = 0
END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_branch]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_get_branch]
			@branchName varchar(30),
			@branchCode varchar(10),
			@issuerID int
			
	AS 
	BEGIN 
	IF (@branchName='') OR (@branchName='null')
	BEGIN
		SET @branchName=NULL
	END
	
	IF (@branchCode='') OR (@branchCode='null')
	BEGIN
		SET @branchCode=NULL
	END
	
	
	
   SELECT * FROM branch 
   WHERE branch_name = COALESCE(@branchName,branch_name)
    AND branch_code =COALESCE(@branchCode,branch_code)
    AND issuer_id =@issuerID
                           
                           
    END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_branch_by_id]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 17 March 2014
-- Description:	Get a branch by its Id.
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_branch_by_id] 
	-- Add the parameters for the stored procedure here
	@branch_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT TOP 1 *
	FROM branch
	WHERE branch_id = @branch_id;
END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_branch_card_count]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get the card count for a branch
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_branch_card_count] 
	@branch_id int,
	@load_card_status_id int = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT COUNT([load_batch_cards].card_id) AS CardCount
		FROM [cards]
			LEFT JOIN [load_batch_cards]
				ON [load_batch_cards].[card_id] = [cards].card_id
					AND [load_batch_cards].load_card_status_id = COALESCE(@load_card_status_id, [load_batch_cards].load_card_status_id)
		WHERE  [cards].branch_id = @branch_id
			
END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_branches_for_user]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 12 March 2014
-- Description:	Fetch all branches by role, issuer and user
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_branches_for_user] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = NULL, 
	@user_id bigint,
	@user_role_id int = NULL,	
	@languageId int =null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @master_user int;

	--Check if the user has enterpise wide priviliages for the specific user role.
	SELECT @master_user = COUNT(*)
	FROM [users_to_users_groups]
		INNER JOIN [user_group]
			ON [users_to_users_groups].user_group_id = [user_group].user_group_id
	WHERE [users_to_users_groups].[user_id] = @user_id
		  AND [user_group].user_role_id = @user_role_id
		  AND [user_group].issuer_id = -1


	IF @master_user > 0
		BEGIN
			SELECT [branch].branch_code, [branch].branch_name, [branch].branch_id,
				   [branch].branch_status_id, [branch].issuer_id, bsl.language_text as branch_status
			FROM [branch] INNER JOIN [branch_statuses]
				ON [branch].branch_status_id = [branch_statuses].branch_status_id
				INNER JOIN [dbo].[branch_statuses_language] bsl
				 ON bsl.branch_status_id=[branch_statuses].branch_status_id
			WHERE issuer_id = COALESCE(@issuer_id, issuer_id)
					AND [branch].branch_status_id = 0  
					AND bsl.language_id=@languageId
		END
	ELSE
		BEGIN
			SELECT DISTINCT [branch].branch_code, [branch].branch_name, [branch].branch_id,
							[branch].branch_status_id, [branch].issuer_id, bsl.language_text as branch_status
			FROM [branch] 
					INNER JOIN [user_roles_branch]
						ON [user_roles_branch].branch_id = [branch].branch_id
					INNER JOIN [branch_statuses]
						ON [branch].branch_status_id = [branch_statuses].branch_status_id
						INNER JOIN [dbo].[branch_statuses_language] bsl
				 ON bsl.branch_status_id=[branch_statuses].branch_status_id
			WHERE [user_roles_branch].[user_id] = @user_id
				  AND [user_roles_branch].user_role_id = COALESCE(@user_role_id, [user_roles_branch].user_role_id)
				  AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  AND [branch].branch_status_id = 0
				  AND bsl.language_id=@languageId
		  END
END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_branches_for_user_admin]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Description:	Fetch all branches by issuer and user for use on branch admin screen.
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_branches_for_user_admin] 
	@issuer_id int = NULL, 
	@branch_status_id int=NULL,
	@user_id bigint,
	@languageId int =null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT DISTINCT [branch].branch_code, [branch].branch_name, [branch].branch_id,
				    [branch].branch_status_id, [branch].issuer_id, bsl.language_text as branch_status
	FROM [branch] 
			INNER JOIN [branch_statuses]
				ON [branch].branch_status_id = [branch_statuses].branch_status_id
			INNER JOIN [dbo].[branch_statuses_language] bsl ON bsl.branch_status_id=[branch_statuses].branch_status_id
	WHERE issuer_id = COALESCE(@issuer_id, issuer_id) 
	AND [branch].branch_status_id =COALESCE(@branch_status_id, [branch].branch_status_id)
	AND bsl.language_id=@languageId
	AND	  branch_id IN (SELECT branch_id
						FROM user_roles_branch
						WHERE [user_id] = @user_id
								AND user_role_id = 10)

	ORDER BY [branch].branch_code

END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_branches_for_user_group]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get a list of branches for a user group.
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_branches_for_user_group] 
	-- Add the parameters for the stored procedure here
	@user_group_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT branch_id
	FROM user_groups_branches
	WHERE user_group_id = @user_group_id
END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_branches_for_userroles]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 12 March 2014
-- Description:	Fetch all branches by role, issuer and user
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_branches_for_userroles] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = NULL, 
	@user_id bigint,	
	@user_role_id_1 int = NULL,
	@user_role_id_2 int = NULL,
	@user_role_id_3 int = NULL,
	@user_role_id_4 int = NULL,		
	@languageId int =null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @master_user int;

	--Check if the user has enterpise wide priviliages for the specific user role.
	SELECT @master_user = COUNT(*)
	FROM [users_to_users_groups]
		INNER JOIN [user_group]
			ON [users_to_users_groups].user_group_id = [user_group].user_group_id
	WHERE 
	--[users_to_users_groups].[user_id] = @user_id AND
		   [user_group].user_role_id = COALESCE(@user_role_id_1, [user_group].user_role_id )
		   OR [user_group].user_role_id =COALESCE(@user_role_id_2, [user_group].user_role_id )
		     OR  [user_group].user_role_id = COALESCE(@user_role_id_3, [user_group].user_role_id )
		  AND [user_group].issuer_id = -1


	IF @master_user > 0
		BEGIN
			SELECT [branch].branch_code, [branch].branch_name, [branch].branch_id,
				   [branch].branch_status_id, [branch].issuer_id, bsl.language_text as branch_status
			FROM [branch] INNER JOIN [branch_statuses]
				ON [branch].branch_status_id = [branch_statuses].branch_status_id
					INNER JOIN [dbo].[branch_statuses_language] bsl
				 ON bsl.branch_status_id=[branch_statuses].branch_status_id
			WHERE issuer_id = COALESCE(@issuer_id, issuer_id)
					AND [branch].branch_status_id = 0
						AND bsl.language_id=@languageId
		END
	ELSE
		BEGIN
			SELECT DISTINCT [branch].branch_code, [branch].branch_name, [branch].branch_id,
							[branch].branch_status_id, [branch].issuer_id, bsl.language_text as branch_status
			FROM [branch] 
					INNER JOIN [user_roles_branch]
						ON [user_roles_branch].branch_id = [branch].branch_id
					INNER JOIN [branch_statuses]
						ON [branch].branch_status_id = [branch_statuses].branch_status_id
							INNER JOIN [dbo].[branch_statuses_language] bsl
				 ON bsl.branch_status_id=[branch_statuses].branch_status_id
			WHERE 
			--[user_roles_branch].[user_id] = @user_id AND
				   [user_roles_branch].user_role_id = COALESCE(@user_role_id_1, [user_roles_branch].user_role_id)
				   OR  [user_roles_branch].user_role_id = COALESCE(@user_role_id_2, [user_roles_branch].user_role_id)
				   OR  [user_roles_branch].user_role_id = COALESCE(@user_role_id_3, [user_roles_branch].user_role_id)
				  AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  AND [branch].branch_status_id = 0
				  	AND bsl.language_id=@languageId
		  END
END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_branches_with_load_card_count]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetches all branches for a user as well as total available cards for distribution batch creation
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_branches_with_load_card_count]
	@issuer_id int, 
	@user_id bigint,
	@user_role_id int,
	@load_card_status_id int = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

		--Get all branches belong to user_group where specific branches are listed
		SELECT [branch].branch_id, [branch].branch_name, [branch].branch_code, COUNT([load_batch_cards].card_id) AS CardCount
		FROM branch INNER JOIN user_groups_branches
				ON branch.branch_id = user_groups_branches.branch_id
			INNER JOIN user_group
				ON user_group.user_group_id = user_groups_branches.user_group_id
			INNER JOIN users_to_users_groups
				ON users_to_users_groups.user_group_id = user_group.user_group_id
			INNER JOIN [user]
				ON [user].[user_id] = users_to_users_groups.[user_id]
			LEFT OUTER JOIN [cards]
				ON [cards].branch_id = [branch].branch_id
			LEFT JOIN [load_batch_cards]
				ON [load_batch_cards].[card_id] = [cards].card_id
					AND [load_batch_cards].load_card_status_id = COALESCE(@load_card_status_id, [load_batch_cards].load_card_status_id)
		WHERE user_group.issuer_id = @issuer_id
			AND user_group.user_role_id = @user_role_id
			AND [user].[user_id] = @user_id
			AND user_group.all_branch_access = 0
			AND branch.branch_status_id = 0
			AND [user].user_status_id = 0
		GROUP BY [branch].branch_id, [branch].branch_name, [branch].branch_code		
		UNION
		--Get all branches belong to user_group where all should be listed.
		SELECT [branch].branch_id, [branch].branch_name, [branch].branch_code, COUNT([load_batch_cards].card_id) AS CardCount
		FROM branch INNER JOIN issuer
				ON branch.issuer_id = issuer.issuer_id
			INNER JOIN user_group
				ON user_group.issuer_id = issuer.issuer_id
			INNER JOIN users_to_users_groups
				ON users_to_users_groups.user_group_id = user_group.user_group_id
			INNER JOIN [user]
				ON [user].[user_id] = users_to_users_groups.[user_id]
			LEFT OUTER JOIN [cards]
				ON [cards].branch_id = [branch].branch_id
			LEFT JOIN [load_batch_cards]
				ON [load_batch_cards].[card_id] = [cards].card_id
					AND [load_batch_cards].load_card_status_id = COALESCE(@load_card_status_id, [load_batch_cards].load_card_status_id)
		WHERE user_group.issuer_id = @issuer_id
			AND user_group.user_role_id = @user_role_id
			AND [user].[user_id] = @user_id
			AND user_group.all_branch_access = 1
			AND branch.branch_status_id = 0
			AND [user].user_status_id = 0
		GROUP BY [branch].branch_id, [branch].branch_name, [branch].branch_code
END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_card]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get card info
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_card] 
	@card_id BIGINT,
	@language_id INT,
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [GET_CARD_DETAILS_TRAN]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				SELECT [cards].card_id, 
					   CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) AS 'card_number', 
					   [cards].product_id,					   
					   [issuer].issuer_name, [issuer].issuer_code, [issuer].issuer_id,
					   [issuer].instant_card_issue_YN, [issuer].pin_mailer_printing_YN,
					   [branch].branch_name, [branch].branch_code, [branch].branch_id,
					   [load_card_statuses].load_card_status,					   					   
					   [load_batch].load_batch_reference,	
					   
					   [issuer_product].product_name,
					   [issuer_product].product_code,
					   [issuer_product].Name_on_card_font_size,
					   [issuer_product].name_on_card_left,
					   [issuer_product].name_on_card_top,
					   [Issuer_product_font].font_name,	

					   [dist_card_statuses].dist_card_status_name,
					   [dist_batch].dist_batch_reference,
					   
					   [branch_card_status_current].branch_card_statuses_id,
					   [branch_card_status_current].status_date,
					   [branch_card_statuses_language].language_text AS 'branch_card_statuses_name',
					   [branch_card_status_current].branch_card_code_type_id,
					   [branch_card_status_current].branch_card_code_name,					   
					   [branch_card_status_current].spoil_only,
					   CONVERT(VARCHAR,DECRYPTBYKEY(operator.username)) as operator,

					   [customer_account].customer_account_id, 
					   CONVERT(VARCHAR,DECRYPTBYKEY([customer_account].customer_account_number)) as 'customer_account_number',
					   [customer_account].card_issue_reason_id,
					   [card_issue_reason_language].language_text AS 'card_issuer_reason_name',
					   [customer_account].account_type_id,
					   [customer_account_type_language].language_text AS 'customer_account_type_name',
					   CONVERT(VARCHAR,DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name',
					   CONVERT(VARCHAR,DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name',
					   CONVERT(VARCHAR,DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name',
					   [customer_account].customer_title_id,
					   [customer_title_language].language_text AS 'customer_title_name',
					   [customer_account].date_issued,
					   CONVERT(VARCHAR,DECRYPTBYKEY([customer_account].name_on_card)) as 'name_on_card',
					   [customer_account].[user_id],
					   CONVERT(VARCHAR,DECRYPTBYKEY([user].first_name)) as 'user_first_name',
					   CONVERT(VARCHAR,DECRYPTBYKEY([user].last_name)) as 'user_last_name',
					   CONVERT(VARCHAR,DECRYPTBYKEY([user].username)) as 'username',
					   [customer_account].cms_id,
					   [customer_account].currency_id,
					   [customer_account].resident_id,
					   [customer_residency_language].language_text AS 'customer_residency_name',
					   [customer_account].customer_type_id,
					   [customer_type_language].language_text AS 'customer_type_name',
					   [customer_account].contract_number
				FROM [cards]
					INNER JOIN [branch]
						ON [branch].branch_id = [cards].branch_id
					INNER JOIN [issuer]
						ON [issuer].issuer_id = [branch].issuer_id					
					INNER JOIN [load_batch_cards]
						ON [load_batch_cards].card_id = [cards].[card_id]
					INNER JOIN [load_batch]
						ON [load_batch].load_batch_id = [load_batch_cards].load_batch_id
					INNER JOIN [load_card_statuses]
						ON [load_card_statuses].load_card_status_id = [load_batch_cards].load_card_status_id
					INNER JOIN [issuer_product]
						ON [issuer_product].product_id = [cards].product_id
					INNER JOIN [Issuer_product_font]
						ON [issuer_product].font_id = [Issuer_product_font].font_id

					LEFT OUTER JOIN [dist_batch_cards]
						ON [cards].card_id = [dist_batch_cards].card_id
					LEFT OUTER  JOIN [dist_batch]
						ON [dist_batch].dist_batch_id = [dist_batch_cards].dist_batch_id
					LEFT OUTER  JOIN [dist_card_statuses]
						ON [dist_card_statuses].dist_card_status_id = [dist_batch_cards].dist_card_status_id

					LEFT OUTER JOIN [branch_card_status_current]
						ON [branch_card_status_current].card_id = [cards].card_id
					--LEFT OUTER JOIN [branch_card_statuses]
					--	ON [branch_card_statuses].branch_card_statuses_id = [branch_card_status_current].branch_card_statuses_id
					LEFT OUTER JOIN [user] operator
						ON [branch_card_status_current].operator_user_id = operator.[user_id]
					LEFT OUTER JOIN [branch_card_statuses_language]
						ON [branch_card_statuses_language].branch_card_statuses_id = [branch_card_status_current].branch_card_statuses_id
							AND [branch_card_statuses_language].language_id = @language_id

					LEFT OUTER JOIN [customer_account]
						ON [cards].card_id = [customer_account].card_id
					LEFT OUTER JOIN [customer_type_language]
						ON [customer_type_language].customer_type_id = [customer_account].customer_type_id
							AND [customer_type_language].language_id = @language_id
					LEFT OUTER JOIN [customer_account_type_language]
						ON [customer_account_type_language].account_type_id = [customer_account].account_type_id
							AND [customer_account_type_language].language_id = @language_id
					LEFT OUTER JOIN [customer_residency_language]
						ON [customer_residency_language].resident_id = [customer_account].resident_id
							AND [customer_residency_language].language_id = @language_id
					LEFT OUTER JOIN [customer_title_language]
						ON [customer_title_language].customer_title_id = [customer_account].customer_title_id
							AND [customer_title_language].language_id = @language_id
					LEFT OUTER JOIN [card_issue_reason_language]
						ON [card_issue_reason_language].card_issue_reason_id = [customer_account].card_issue_reason_id
							AND [card_issue_reason_language].language_id = @language_id
					LEFT OUTER  JOIN [user]
						ON [customer_account].[user_id] = [user].[user_id]
				WHERE [cards].card_id = @card_id
				

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

			--log the audit record		
			--EXEC sp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 'Getting details for card.', 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_CARD_DETAILS_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_CARD_DETAILS_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_get_cards_in_error]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_cards_in_error] 
	@user_id bigint,
	@language_id INT,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
			
	DECLARE @StartRow INT, @EndRow INT;			
			
	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	--append#1
	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY status_date DESC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS			
			, *
	FROM( 				
		SELECT DISTINCT [cards].card_id, 
				CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) AS card_number,					   
				[branch_card_statuses_language].language_text AS current_card_status,						
				[branch_card_status_current].status_date,						
				[issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
				[branch].branch_id, [branch].branch_name, [branch].branch_code
		FROM [cards]
			INNER JOIN [branch_card_status_current]
				ON [branch_card_status_current].card_id = [cards].card_id						   
			INNER JOIN [branch_card_statuses_language]
				ON [branch_card_status_current].branch_card_statuses_id = [branch_card_statuses_language].branch_card_statuses_id
					AND [branch_card_statuses_language].language_id = @language_id							
			--Filter out cards linked to branches the user doesnt have access to.
			INNER JOIN (SELECT DISTINCT branch_id								
						FROM [user_roles_branch] INNER JOIN [user_roles]
								ON [user_roles_branch].user_role_id = [user_roles].user_role_id		
						WHERE [user_roles_branch].[user_id] = @user_id
								AND [user_roles].user_role_id = 2--Only want roles that allowed to search cards
						) as X
				ON [cards].branch_id = X.branch_id
			INNER JOIN [branch]
				ON [branch].branch_id = [cards].branch_id
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [branch].issuer_id
		WHERE  [branch].branch_status_id = 0	 
				AND [issuer].issuer_status_id = 0			  
				AND [branch_card_status_current].branch_card_statuses_id IN (8 , 9) 

	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY status_date DESC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END


GO
/****** Object:  StoredProcedure [dbo].[sp_get_cms_bank_parameters]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_cms_bank_parameters] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [mod_interface_account_params].*
	FROM [mod_interface_account_params]
	WHERE [mod_interface_account_params].issuer_id = @issuer_id

END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_cms_parameters]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_cms_parameters] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@branch_id int,
	@card_id bigint,
	@CCY char(3),
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [mod_interface_account_params].*, [branch].branch_code, [issuer_product].product_code, 
			[issuer_product].product_bin_code, [mod_interface_cond_accnt].COND_SET, 
			[country].country_code, [country].country_capital_city
FROM [mod_interface_account_params]
		INNER JOIN [branch]
			ON [mod_interface_account_params].issuer_id = [branch].issuer_id
		INNER JOIN [issuer_product]
			ON [mod_interface_account_params].issuer_id = [issuer_product].issuer_id
		INNER JOIN [mod_interface_cond_accnt]
			ON [mod_interface_cond_accnt].product_id = [issuer_product].product_id
		INNER JOIN [cards]
			ON [cards].product_id = [mod_interface_cond_accnt].product_id
		INNER JOIN [issuer]
			ON [issuer].issuer_id = [mod_interface_account_params].issuer_id
		INNER JOIN [country]
			ON [country].country_id = [issuer].country_id
WHERE [mod_interface_account_params].issuer_id = @issuer_id
	  AND [branch].branch_id = @branch_id
	  AND [cards].card_id = @card_id
	  AND [mod_interface_cond_accnt].CCY = @CCY
END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_connection_parameters]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================

-- Author:		sandhya konduru

-- Create date: <Create Date,,>

-- Description:	<Description,,>

-- =============================================

CREATE PROCEDURE [dbo].[sp_get_connection_parameters]
	@issuerID int,
	@interfaceid int

AS

BEGIN

	SET NOCOUNT ON;




	SELECT X.address,
			   Y.interface_type_id,
			   Y.issuer_issuer_id,
			   Y.id,
			   X.username, 
			   X.password,
			   X.port, 
			   X.protocal, 
			   X.path, 
			   X.auth_type

		FROM connection_parameters X,
			 issuer_interface Y,
			 connection_parameters_has_interface Z

		WHERE X.id = Z.connection_parameters_id and 
			   Z.interfaces_id = Y.id and 
			   y.issuer_issuer_id = @issuerID and y.interface_type_id= @interfaceid
END


GO
/****** Object:  StoredProcedure [dbo].[sp_get_connection_params]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_connection_params] 
	-- Add the parameters for the stored procedure here
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT [connection_parameter_id],[connection_name],[address],[port],[path],[protocol],[auth_type],
			CONVERT(VARCHAR,DECRYPTBYKEY([username])) as [username],
			CONVERT(VARCHAR,DECRYPTBYKEY([password])) as [password]
		FROM connection_parameters

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_currenies_product]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_currenies_product] 
	@Productid int=null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select * from product_currency
	where product_currency.product_id=@Productid

END


GO
/****** Object:  StoredProcedure [dbo].[sp_get_dist_batch_cards]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch all cards linked to distribution batch
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_dist_batch_cards] 
	@dist_batch_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_DIST_BATCH_CARDS]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				SELECT CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) as 'card_number', 
						[dist_card_statuses].dist_card_status_name					   
				FROM [cards]
					INNER JOIN [dist_batch_cards] 
						ON [dist_batch_cards].card_id = [cards].card_id	
					INNER JOIN [dist_card_statuses]
						ON 	[dist_batch_cards].dist_card_status_id = [dist_card_statuses].dist_card_status_id
				WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id
					
														   
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;		

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting cards for distribution batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC sp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_DIST_BATCH_CARDS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_DIST_BATCH_CARDS]
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
/****** Object:  StoredProcedure [dbo].[sp_get_dist_batch_history]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch the distribution batches history
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_dist_batch_history] 
	@dist_batch_id bigint,	
	@languageId int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_DIST_BATCH_HISTORY]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				SELECT [dist_batch].dist_batch_reference, [dist_batch].no_cards, [dist_batch_status].status_date,
					   [dist_batch_statuses_language].language_text as dist_batch_status_name, 
					   CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'username',
					   CONVERT(VARCHAR,DECRYPTBYKEY([user].first_name)) as 'first_name',
					   CONVERT(VARCHAR,DECRYPTBYKEY([user].last_name)) as 'last_name'   
				FROM [dist_batch]
						INNER JOIN [dist_batch_status]
							ON [dist_batch].dist_batch_id = [dist_batch_status].dist_batch_id
						INNER JOIN [dist_batch_statuses]
							ON [dist_batch_status].dist_batch_statuses_id = [dist_batch_statuses].dist_batch_statuses_id
							INNER JOIN [dist_batch_statuses_language]
							ON [dist_batch_status].dist_batch_statuses_id = [dist_batch_statuses_language].dist_batch_statuses_id
						INNER JOIN [user]
							ON [dist_batch_status].[user_id] = [user].[user_id]
				WHERE [dist_batch].dist_batch_id = @dist_batch_id and [dist_batch_statuses_language].language_id=@languageId
				ORDER BY [dist_batch_status].status_date ASC	

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting history for distribution batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC sp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_DIST_BATCH_HISTORY]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_DIST_BATCH_HISTORY]
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
/****** Object:  StoredProcedure [dbo].[sp_get_dist_batches]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_get_dist_batches]
@batch_status varchar(25),
@branch_code varchar (20),
@issuer_id int,
@dateFrom Datetime,
@dateTo Datetime
AS
BEGIN

	IF((@batch_status=null) OR (@batch_status='') 
	OR (@batch_status='INVALID'))
	BEGIN
		SET @batch_status= NULL
	END

	IF((@branch_code=null) OR (@branch_code='') 
	OR (@branch_code='INVALID'))
	BEGIN
		SET @branch_code= NULL
	END
	
	IF(@dateFrom = '1900-01-01 00:00:00.000') OR (@dateFrom like 'null')
		BEGIN			
			SET @dateFrom = NULL
		END
		
		IF(@dateTo = '1900-01-01 00:00:00.000') OR (@dateTo like 'null')
		BEGIN			
			SET @dateTo = NULL
		END
	
	SELECT * FROM distribution_batch 
	WHERE date_created >=COALESCE(@dateFrom,date_created) AND
		  date_created <= COALESCE(@dateTo,date_created) AND
		  batch_status = COALESCE(@batch_status,batch_status)AND
		  branch_code = COALESCE (@branch_code,branch_code) AND
		  issuer_id =@issuer_id
	ORDER BY date_created;
END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_dist_batches_for_user]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 26 March 2014
-- Description:	Gets a list of distribution batches for a user
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_dist_batches_for_user] 
	-- Add the parameters for the stored procedure here
	@user_id bigint,
	@issuerId int =null,
	@dist_batch_reference VARCHAR(25) = NULL,
	@dist_batch_statuses_id int = NULL,
	@branch_id INT = NULL,
	@date_start DateTime = NULL,
	@date_end DateTime = NULL,
	@language_id INT,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_DIST_BATCH_FOR_USER_TRAN]
		BEGIN TRY 
			IF(@dist_batch_reference <>'' or @dist_batch_reference <> NULL)
				SET @dist_batch_reference = '%'+@dist_batch_reference+'%'

			IF @date_end IS NOT NULL
				SET @date_end = DATEADD(DAY, 1, @date_end)

			DECLARE @StartRow INT, @EndRow INT;			

			SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
			SET @EndRow = @StartRow + @RowsPerPage - 1;

			--append#1
			WITH PAGE_ROWS
			AS
			(
			SELECT ROW_NUMBER() OVER(ORDER BY date_created DESC, dist_batch_reference ASC) AS ROW_NO
					, COUNT(*) OVER() AS TOTAL_ROWS
					, *
			FROM( 
				SELECT Distinct [dist_batch].dist_batch_id, [dist_batch].date_created, [dist_batch].dist_batch_reference, 
					   [dist_batch].no_cards, [dist_batch_status_current].status_notes,
					   [dist_batch_status_current].dist_batch_statuses_id, [dist_batch_statuses_language].language_text AS 'dist_batch_status_name', 
					   [issuer].[issuer_id], [issuer].issuer_name, [issuer].issuer_code, [issuer].auto_create_dist_batch,
					   [branch].branch_name, [branch].branch_code
				FROM [dist_batch] 
					INNER JOIN [dist_batch_status_current]
						ON [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
					INNER JOIN [dist_batch_statuses_language]
						ON [dist_batch_status_current].dist_batch_statuses_id = [dist_batch_statuses_language].dist_batch_statuses_id
							AND [dist_batch_statuses_language].language_id = @language_id
					INNER JOIN [user_roles_branch]
						ON [dist_batch].branch_id = [user_roles_branch].branch_id
							AND [user_roles_branch].user_role_id IN ( 2, 4, 5 )
					INNER JOIN [branch]
						ON [branch].branch_id = [dist_batch].branch_id				
					INNER JOIN [issuer]
						ON [issuer].issuer_id = [branch].issuer_id				
				WHERE [user_roles_branch].[user_id] = @user_id
					AND [dist_batch].dist_batch_reference LIKE COALESCE(@dist_batch_reference, [dist_batch].dist_batch_reference)
					AND [dist_batch_status_current].dist_batch_statuses_id = COALESCE(@dist_batch_statuses_id, [dist_batch_status_current].dist_batch_statuses_id)
					AND [dist_batch].date_created >= COALESCE(@date_start, [dist_batch].date_created)
					AND [dist_batch].date_created <= COALESCE(@date_end, [dist_batch].date_created)
					AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
					AND [branch].branch_status_id = 0	 
					AND [issuer].issuer_status_id = 0
					AND [issuer].issuer_id = COALESCE(@issuerId,  [issuer].issuer_id)
			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY date_created DESC, dist_batch_reference ASC

			--log the audit record		
			--EXEC sp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 'Getting Distribution batches for user.', 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_DIST_BATCH_FOR_USER_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_DIST_BATCH_FOR_USER_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_get_file_history]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_file_history] 
	-- Add the parameters for the stored procedure here
	@file_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

        SELECT TOP 1 [file_history].*, [file_statuses].file_status, [file_types].file_type, [issuer].issuer_name, [issuer].issuer_code
		FROM [file_history]
		INNER JOIN [file_statuses]
			ON [file_history].file_status_id = [file_statuses].file_status_id
		INNER JOIN [file_types]
			ON [file_history].file_type_id = [file_types].file_type_id
		LEFT OUTER JOIN [issuer]
			ON [file_history].issuer_id = [issuer].issuer_id
		WHERE [file_history].[file_id] = @file_id
END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_file_historys]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_file_historys] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@date_from datetime,
	@date_to datetime,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [file_history].*, [file_statuses].file_status, [file_types].file_type, [issuer].issuer_name, [issuer].issuer_code
	FROM [file_history]
		INNER JOIN [file_statuses]
			ON [file_history].file_status_id = [file_statuses].file_status_id
		INNER JOIN [file_types]
			ON [file_history].file_type_id = [file_types].file_type_id
		LEFT OUTER JOIN [issuer]
			ON [file_history].issuer_id = [issuer].issuer_id
	WHERE [file_history].issuer_id = COALESCE(@issuer_id, [file_history].issuer_id)
		  AND load_date BETWEEN @date_from AND @date_to 
END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_file_load_list]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_file_load_list]
	@date_start DATETIME,
	@date_end DATETIME,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	IF @date_end IS NOT NULL
		SET @date_end = DATEADD(DAY, 1, @date_end);

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY file_load_start ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
		   SELECT * 
				 FROM [file_load]
				 WHERE [file_load].file_load_start BETWEEN @date_start AND @date_end
						
			)
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY file_load_start ASC
END






GO
/****** Object:  StoredProcedure [dbo].[sp_get_flex_parameter]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch flexcube parameters
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_flex_parameter]
	@issuer_id int,	
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_FLEX_PARAMETER]

	BEGIN TRY 
		--Increment the request id
		UPDATE [flex_parameters]
		SET request_id = (fp.request_id + 1)	
		FROM [flex_parameters] fp
		WHERE flex_parameter_id = 1

		--Fetch the parameters
		--Someone may need to look at getting parameters per connection...
		SELECT [flex_parameters].*, [flex_affiliate_codes].affiliate_code
		FROM [flex_parameters], [flex_affiliate_codes]
		WHERE [flex_parameters].flex_parameter_id = 1 AND
			  [flex_affiliate_codes].issuer_id = @issuer_id

		IF @@Rowcount = 0
			ROLLBACK TRANSACTION [GET_FLEX_PARAMETER]
		ELSE
			COMMIT TRANSACTION [GET_FLEX_PARAMETER]

	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_FLEX_PARAMETER]
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
/****** Object:  StoredProcedure [dbo].[sp_get_flex_responses]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_flex_responses]
	@responseCode varchar(20),
	@accountType varchar(20),
	@accountStatus varchar(20),
	@atmFlag varchar(20),
	@responseMessage varchar(50),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [flex_responses].flex_response_name, 
			[flex_response_values].*, [flex_response_values_language].language_text
	FROM [flex_responses] 
			INNER JOIN [flex_response_values] 		
				ON [flex_responses].flex_response_id = [flex_response_values].flex_response_id
			INNER JOIN [flex_response_values_language]
				ON [flex_response_values].flex_response_value_id = [flex_response_values_language].flex_response_value_id
					AND [flex_response_values_language].language_id = @language_id
	WHERE ([flex_responses].flex_response_id = 1 AND [flex_response_values].flex_response_value = @responseCode)
		  OR ([flex_responses].flex_response_id = 2 AND [flex_response_values].flex_response_value = @accountType)
		  OR ([flex_responses].flex_response_id = 3 AND [flex_response_values].flex_response_value = @accountStatus)
		  OR ([flex_responses].flex_response_id = 4 AND [flex_response_values].flex_response_value = @atmFlag)
		  OR ([flex_responses].flex_response_id = 5 AND [flex_response_values].flex_response_value = @responseMessage)	 

END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_fonts_ist]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_fonts_ist]
	@PageIndex INT = 1,
@RowsPerPage INT = 20
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY font_name ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
    -- Insert statements for procedure here
	select * from Issuer_product_font where DeletedYN is null or DeletedYN =0
	 )
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY font_name ASC

END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_fonts_list]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_fonts_list]
	@PageIndex INT = 1,
	@RowsPerPage INT = 20
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY font_name ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
    -- Insert statements for procedure here
	select * from Issuer_product_font where DeletedYN is null or DeletedYN =0
	 )
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY font_name ASC

END





GO
/****** Object:  StoredProcedure [dbo].[sp_get_groups_roles_for_user]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_groups_roles_for_user] 
	-- Add the parameters for the stored procedure here
	@user_id bigint,
	@languageId int =null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [user_group].user_group_id, [user_group].user_group_name, url.language_text as user_role, [issuer].issuer_code, 
		   [issuer].issuer_name
	FROM [user_group] 
			INNER JOIN [issuer]
				ON [user_group].issuer_id = [issuer].issuer_id
			INNER JOIN [users_to_users_groups]
				ON [user_group].user_group_id = [users_to_users_groups].user_group_id
			INNER JOIN [user_roles]
				ON [user_group].user_role_id = [user_roles].user_role_id
				INNER JOIN [user_roles_language] url
				ON url.user_role_id = [user_roles].user_role_id
	WHERE [users_to_users_groups].[user_id] = @user_id
	AND url.language_id=@languageId
END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_interface_Rowaccountbaseinfo]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: 20/04/2014
-- Description:	to pass values to cms webservice
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_interface_Rowaccountbaseinfo]
	@Bankcode nvarchar(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT        m.STAT_CHANGE,m.LIM_INTR, m.NON_REDUCE_BAL, m.CRD, m.Cycle,m.BANK_C
FROM            mod_interface_account_params m

where m.BANK_C =@Bankcode

END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_invetorysummaryreport]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [sp_get_invetorysummaryreport] null,1
CREATE PROCEDURE [dbo].[sp_get_invetorysummaryreport]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int =null
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	if(@issuer_id=-1 or @issuer_id=0)
	set @issuer_id=null

	if(@branch_id =0)
	set @branch_id=null

	SELECT INTERX.issuer_id,
		   branch_code,
		   INTERX.branch_id,
		   INTERX.branch_card_statuses_name,
		   INTERX.branch_card_statuses_id,
		   COUNT(INTER.card_id)	AS CardCount,issuer.issuer_name as 'issuer_name'

	FROM 	 
		  (SELECT [branch].branch_id, [branch_card_status_current].branch_card_statuses_id, [branch_card_status_current].card_id
			FROM [branch_card_status_current]
					INNER JOIN [cards]
						ON [branch_card_status_current].card_id = [cards].card_id
					INNER JOIN [branch]
						ON [cards].branch_id = [branch].branch_id
						Where    [branch].branch_status_id = 0
			) AS INTER
				RIGHT OUTER JOIN 			
			(SELECT issuer_id, branch_id, branch_code, [branch_card_statuses].branch_card_statuses_id, bcl.language_text as 'branch_card_statuses_name',bcl.language_id
			 FROM	[branch], [branch_card_statuses]
			 inner join branch_card_statuses_language bcl on [branch_card_statuses].branch_card_statuses_id=bcl.branch_card_statuses_id
			 where branch.branch_status_id=0
		 		) INTERX

			ON INTER.branch_id = INTERX.branch_id
				AND INTER.branch_card_statuses_id = INTERX.branch_card_statuses_id 				
	inner join issuer on issuer.issuer_id=INTERX.issuer_id

	WHERE INTERX.issuer_id = COALESCE(@issuer_id, INTERX.issuer_id)
		  AND INTERX.branch_id = COALESCE(@branch_id, INTERX.branch_id) and INTERX.language_id=@language_id
		  AND INTERX.branch_card_statuses_id!=6 and INTERX.branch_card_statuses_id!=7
		  AND INTERX.branch_code <> ''  
	GROUP BY INTERX.issuer_id, INTERX.branch_id, branch_code, branch_card_statuses_name, INTERX.branch_card_statuses_id,issuer_name
	ORDER BY issuer_name, INTERX.branch_code, INTERX.branch_card_statuses_id

END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_issue_cards]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Batch submitted through debugger: SQLQuery5.sql|0|0|C:\Users\mpho.majenge\AppData\Local\Temp\~vs4B50.sql
CREATE PROCEDURE [dbo].[sp_get_issue_cards]

--@user varchar(20),
--@firstname varchar(20),
--@lastname varchar(20),
--@accountNumber varchar(20),
@issuerID int,
@cardStatus varchar(20),
@dateFrom Datetime,
@dateTo Datetime

AS
BEGIN

--	IF((@accountNumber=null) OR (@accountNumber=''))
--	BEGIN
--		SET @accountNumber= '0'
--	END

	IF((@cardStatus=null) OR (@cardStatus=''))
	BEGIN
		SET @cardStatus= NULL
	END

	
	--IF(@dateFrom = CONVERT(datetime,'1900-01-01 00:00:00.000')) OR (@dateFrom = 'null')
	--BEGIN			
	--	SET @dateFrom = NULL
	--END
		
	--IF(@dateTo = CONVERT(datetime,'1900-01-01 00:00:00.000')) OR (@dateTo = 'null')
	--BEGIN			
	--	SET @dateTo = NULL
	--END
	
	--MM Changes  (PCI - DSS)
OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate
	if (@dateFrom = '' OR @dateFrom = null OR @dateTo = '' OR @dateTo = null)
	begin
		 SELECT card_id,
		 dist_cards.card_number,
		 dist_cards.seqeunce,dist_cards.dist_batch_reference,dist_cards.card_status,
		 dist_cards.date_issued,
		 dist_cards.issued_by,
		 dist_cards.customer_first_name,dist_cards.customer_last_name,
		 dist_cards.customer_account,dist_cards.account_type,
		 dist_cards.name_on_card,dist_cards.issuer_id,
		 dist_cards.branch_code,dist_cards.reason_for_issue,dist_cards.customer_title,
		 dist_cards.assigned_operator
		 FROM dist_cards inner join distribution_batch
		 ON dist_cards.dist_batch_reference = distribution_batch.batch_refrence
		 WHERE dist_cards.card_status = @cardStatus AND distribution_batch.issuer_id = @issuerID
		 AND dist_cards.date_issued >= CONVERT(datetime,@dateFrom)
		 AND dist_cards.date_issued <= CONVERT(datetime,@dateTo)
	end
	else
		begin
		 SELECT card_id,
		 dist_cards.card_number,
		 dist_cards.seqeunce,dist_cards.dist_batch_reference,dist_cards.card_status,
		 dist_cards.date_issued,
		 dist_cards.issued_by,
		 dist_cards.customer_first_name,dist_cards.customer_last_name,
		 dist_cards.customer_account,dist_cards.account_type,
		 dist_cards.name_on_card,dist_cards.issuer_id,
		 dist_cards.branch_code,dist_cards.reason_for_issue,dist_cards.customer_title,
		 dist_cards.assigned_operator
		 FROM dist_cards inner join distribution_batch
		 ON dist_cards.dist_batch_reference = distribution_batch.batch_refrence 
		 WHERE dist_cards.card_status = @cardStatus
		 AND distribution_batch.issuer_id = @issuerID
		end
	
CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuecardsummaryreport]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [sp_get_invetorysummaryreport] null,1
CREATE PROCEDURE [dbo].[sp_get_issuecardsummaryreport]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int,
	@fromdate datetime,
	@todate datetime
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	if(@issuer_id = -1 or @issuer_id = 0)
	 set @issuer_id=null

	if(@branch_id  =0)
		set @branch_id = null

	SELECT INTERX.issuer_id,
		   branch_code,
		   INTERX.branch_id,
		   INTERX.card_issue_reason_id,
		   INTERX.card_issuer_reason_name,
		   COUNT(INTER.card_id)	AS CardCount,issuer.issuer_name as 'issuer_name'
	FROM 
		-- this sub select fetches all cards belonging to a branch and currently in issued status
		(SELECT [branch].branch_id, [branch_card_status_current].card_id, [customer_account].card_issue_reason_id
			FROM [branch_card_status_current] 
					INNER JOIN [customer_account]
						ON [customer_account].card_id = [branch_card_status_current].card_id 
					INNER JOIN [cards]
						ON [branch_card_status_current].card_id = [cards].card_id
					INNER JOIN [branch]
						ON [cards].branch_id = [branch].branch_id
			WHERE [branch_card_status_current].branch_card_statuses_id = 6
				  AND [branch_card_status_current].[status_date] >= @fromdate 
				  AND [branch_card_status_current].[status_date] <= @todate
		) AS INTER						
		RIGHT OUTER JOIN 		
		--This Sub Select creates a cartesian product of branch and card issue reason	
		(SELECT issuer_id, [branch].branch_id, branch_code, [card_issue_reason].[card_issue_reason_id],
				 language_text as 'card_issuer_reason_name'
			FROM [branch],[card_issue_reason]
					INNER JOIN [card_issue_reason_language]  
						ON [card_issue_reason].[card_issue_reason_id] = [card_issue_reason_language].[card_issue_reason_id]
						   AND [card_issue_reason_language].language_id = @language_id
						   Where    [branch].branch_status_id = 0
		 	)  INTERX
		ON INTER.branch_id = INTERX.branch_id
			AND INTER.[card_issue_reason_id] = INTERX.[card_issue_reason_id] 
			inner join issuer on issuer.issuer_id=INTERX.issuer_id
	WHERE INTERX.issuer_id = COALESCE(@issuer_id, INTERX.issuer_id)
		AND INTERX.branch_id = COALESCE(@branch_id,  INTERX.branch_id)	
		AND INTERX.branch_code <> '' 
	GROUP BY INTERX.issuer_id,  INTERX.branch_id,INTERX.branch_code,  INTERX.[card_issue_reason_id],INTERX.[card_issuer_reason_name],issuer_name
	ORDER BY issuer_name, INTERX.branch_code, INTERX.[card_issue_reason_id]
END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuedcardsreport]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: 16/05/2014
-- Description:	for displaying issued card report
-- =============================================
----exec sp_get_issuedcardsreport 2,'6/14/2014 12:00:00 AM','6/27/2014 12:00:00 AM' ,25,null
CREATE PROCEDURE [dbo].[sp_get_issuedcardsreport]
	@isuerid int,
	@fromdate datetime,
	@todate datetime,
	@userid int = null,
	@brachcode nvarchar(50),
	@language_id int = null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if(@userid=0)
	set @userid=null

	if(@isuerid=0)
	set @isuerid=null
    -- Insert statements for procedure here
		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
		
SELECT [branch_code] as 'BranchCode',CONVERT(VARCHAR,DECRYPTBYKEY(u.[username])) as 'IssuedBy',
CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.customer_first_name))+' '+ CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.customer_last_name)) as 'CustomerNames',
CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.[customer_account_number])) as 'customeraccountNumber',
CONVERT(VARCHAR,DECRYPTBYKEY(cards.[card_number])) as 'cardnumber',[branch_card_statuses_name] as 'CardStatus',[status_date] as'IssuedDate',APPROVER.username as 'APPROVER USER',language_text as 'Scenario'
FROM branch_card_status_current
 INNER JOIN  cards  on branch_card_status_current.[card_id] = cards.[card_id]
 inner join branch on cards.[branch_id]=branch.[branch_id]
  left join customer_account ON cards.card_id = customer_account.card_id 
 --left JOIN
 --customer_account_type ON customer_account.customer_account_type_id = customer_account_type.customer_account_type_id 
 inner join [user] u on u.[user_id] = branch_card_status_current.[operator_user_id]
 inner join branch_card_statuses bs on bs.[branch_card_statuses_id] =branch_card_status_current.[branch_card_statuses_id]
 inner join [card_issue_reason] on customer_account.card_issue_reason_id = [card_issue_reason].card_issue_reason_id
					INNER JOIN [card_issue_reason_language]  
						ON [card_issue_reason].[card_issue_reason_id] = [card_issue_reason_language].[card_issue_reason_id]
						  
 INNER JOIN (SELECT CONVERT(VARCHAR,DECRYPTBYKEY([user].username))as 'username' , card_id
  FROM branch_card_status
  inner join [user] on [user].user_id=branch_card_status.user_id
  where branch_card_status.[branch_card_statuses_id]  =3) AS APPROVER
  ON [cards].card_id = APPROVER.card_id
  

 where
      ISNULL(branch.issuer_id, null)=ISNULL(@isuerid, ISNULL(branch.issuer_id, null)) and
  branch_card_status_current.branch_card_statuses_id = 6 and 
  ISNULL(u.[user_id],null)=ISNULL(@userid, ISNULL(u.[user_id], null))
and   
  ISNULL(branch.branch_id, null)=ISNULL(@brachcode, ISNULL(branch.branch_id, null))
   AND [card_issue_reason_language].language_id = @language_id
 and branch_card_status_current.[status_date] >=@fromdate and branch_card_status_current.[status_date]<=@todate
 order by CardStatus

 CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuer]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_issuer] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT TOP 1 [issuer].*
	FROM [issuer] 
	WHERE [issuer].issuer_id = @issuer_id

END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuer_by_branch]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_issuer_by_branch] 
	@branch_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT *
	FROM [issuer]
		INNER JOIN [branch]
			ON [issuer].issuer_id = [branch].issuer_id
	WHERE [branch].branch_id = @branch_id
END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuer_by_product_and_branchcode]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_issuer_by_product_and_branchcode] 
	-- Add the parameters for the stored procedure here
	@bin_code char(6),
	@branch_code char(3)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT DISTINCT [issuer].*
	FROM [issuer]
			INNER JOIN [issuer_product]
				ON [issuer_product].issuer_id = [issuer].issuer_id
			INNER JOIN [branch]
				ON [branch].issuer_id = [issuer].issuer_id
	WHERE [issuer_product].product_bin_code LIKE CONVERT(varchar,@bin_code + '%')
		  AND [branch].branch_code = @branch_code
END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuer_interface_conn]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_issuer_interface_conn] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@interface_type_id int,
	@auditUserId bigint,
	@auditWorkstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT TOP 1 [connection_parameters].[connection_parameter_id],[connection_name],[address],[port],[path],[protocol],[auth_type],
			CONVERT(VARCHAR,DECRYPTBYKEY([username])) as [username],
			CONVERT(VARCHAR,DECRYPTBYKEY([password])) as [password]
		FROM [connection_parameters]
			INNER JOIN [issuer_interface]
				ON [issuer_interface].connection_parameter_id = [connection_parameters].connection_parameter_id
		WHERE [issuer_interface].issuer_id = @issuer_id AND
			  [issuer_interface].interface_type_id = @interface_type_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuer_interface_details]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo, Setenane
-- Create date: 2014/03/20
-- Description:	Gets the issuer interface details based on the @interface_id
-- =============================================

CREATE PROCEDURE [dbo].[sp_get_issuer_interface_details]
	@connection_parameter_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT *
	FROM [connection_parameters]
	WHERE connection_parameter_id = @connection_parameter_id

END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuer_interfaces]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo, Setenane
-- Create date: 2014/03/25
-- Description:	Gets a list of issuer interfaces based on the @issuer_id
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_issuer_interfaces]
	@issuer_id int,
	@interface_type_id int = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    
	SELECT *
	FROM [issuer_interface]
	WHERE issuer_id = @issuer_id AND
		  interface_type_id = COALESCE(@interface_type_id, interface_type_id)


END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuerdetails]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_issuerdetails]
@issuerid varchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
--SELECT  top 1     issuer_id, issuer_city, issuer_country, language_id,issuer.issuer_code
--FROM            issuer
--where issuer.issuer_id=@issuerid
SELECT  top 1     *
FROM            issuer
where issuer.issuer_id=@issuerid

	
END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_languages]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_languages] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM [languages]
	--WHERE id = @language_id
END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_ldap_setting]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo, Setenane
-- Create date: 2014/04/02
-- Description:	Gets ldap settings for
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_ldap_setting]	
	@ldap_setting_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT [ldap_setting].domain_name, [ldap_setting].hostname_or_ip, [ldap_setting].[path],
				CONVERT(VARCHAR,DECRYPTBYKEY([ldap_setting].username)) as domain_username,
				CONVERT(VARCHAR,DECRYPTBYKEY([ldap_setting].[password])) as domain_password
		FROM [ldap_setting]
		WHERE [ldap_setting].ldap_setting_id = @ldap_setting_id
		 

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_ldap_settings]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_ldap_settings] 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT [ldap_setting_id],[ldap_setting_name], [hostname_or_ip], [domain_name],[path],
			CONVERT(VARCHAR,DECRYPTBYKEY([username])) as [username],
			CONVERT(VARCHAR,DECRYPTBYKEY([password])) as [password]
		FROM ldap_setting

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_license_issuers]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch all issuers who are licensed/unlicensed
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_license_issuers] 
	-- Add the parameters for the stored procedure here
	@issuer_licensed bit = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT issuer_id, issuer_code, issuer_name, license_file, license_key
	FROM [issuer]
	WHERE issuer_status_id = 0 AND
			(@issuer_licensed IS NULL
			OR (@issuer_licensed = 1 AND [issuer].license_key IS NOT NULL)
			OR (@issuer_licensed = 0 AND [issuer].license_key IS NULL))
END


GO
/****** Object:  StoredProcedure [dbo].[sp_get_load_batch]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Returns a single load batch
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_load_batch] 
	@load_batch_id bigint,
	@language_id INT,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_LOAD_BATCH]
		BEGIN TRY 

			SELECT CAST(1 as BIGINT) as ROW_NO, 1 AS TOTAL_ROWS, 1 as TOTAL_PAGES, 
				   [load_batch].load_batch_id, [load_batch].load_date, [load_batch].load_batch_reference, 
				   [load_batch].no_cards, [load_batch_status].load_batch_statuses_id, 
				   [load_batch_status].status_notes, [load_batch_statuses_language].language_text AS 'load_batch_status_name'
				   ,'' as 'BranchName'
			FROM [load_batch] 
				INNER JOIN [load_batch_status]
					ON [load_batch_status].load_batch_id = [load_batch].load_batch_id
				INNER JOIN [load_batch_statuses_language]
					ON [load_batch_status].load_batch_statuses_id = [load_batch_statuses_language].load_batch_statuses_id	
						AND [load_batch_statuses_language].language_id = @language_id		
			WHERE 
				[load_batch].load_batch_id = @load_batch_id	
				AND [load_batch_status].status_date = (SELECT MAX(lbs2.status_date)
													   FROM [load_batch_status] lbs2
													   WHERE lbs2.load_batch_id = [load_batch].load_batch_id)		

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting Load batch with id: ' + CAST(@load_batch_id AS varchar(max))
			----log the audit record		
			--EXEC sp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_LOAD_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_LOAD_BATCH]
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
/****** Object:  StoredProcedure [dbo].[sp_get_load_batch_cards]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch all cards for load batch
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_load_batch_cards] 
	@load_batch_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_LOAD_BATCH_CARDS]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				SELECT CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) as 'card_number', 
						[load_card_statuses].load_card_status					   
				FROM [cards]
					INNER JOIN [load_batch_cards] 
						ON [load_batch_cards].card_id = [cards].card_id	
					INNER JOIN [load_card_statuses]
						ON 	[load_batch_cards].load_card_status_id = [load_card_statuses].load_card_status_id
				WHERE [load_batch_cards].load_batch_id = @load_batch_id
					
														   
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;		

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting cards for load batch with id: ' + CAST(@load_batch_id AS varchar(max))
			----log the audit record		
			--EXEC sp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_LOAD_BATCH_CARDS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_LOAD_BATCH_CARDS]
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
/****** Object:  StoredProcedure [dbo].[sp_get_load_batch_history]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get status hostory for a load batch
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_load_batch_history] 
	@load_batch_id bigint,
	@languageId int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_LOAD_BATCH_HISTORY]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				SELECT [load_batch].load_batch_reference, [load_batch].no_cards, [load_batch_status].status_date,
					  [load_batch_statuses_language].language_text as  load_batch_status_name, 
					   CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'username',
					   CONVERT(VARCHAR,DECRYPTBYKEY([user].first_name)) as 'first_name',
					   CONVERT(VARCHAR,DECRYPTBYKEY([user].last_name)) as 'last_name'   
				FROM [load_batch]
						INNER JOIN [load_batch_status]
							ON [load_batch].load_batch_id = [load_batch_status].load_batch_id
						INNER JOIN [load_batch_statuses]
							ON [load_batch_status].load_batch_statuses_id = [load_batch_statuses].load_batch_statuses_id
							INNER JOIN [load_batch_statuses_language]
							ON [load_batch_status].load_batch_statuses_id = [load_batch_statuses_language].load_batch_statuses_id
						INNER JOIN [user]
							ON [load_batch_status].[user_id] = [user].[user_id]
				WHERE [load_batch].load_batch_id = @load_batch_id and [load_batch_statuses_language].language_id=@languageId
				ORDER BY [load_batch_status].status_date ASC	

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting history for load batch with id: ' + CAST(@load_batch_id AS varchar(max))
			----log the audit record		
			--EXEC sp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_LOAD_BATCH_HISTORY]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_LOAD_BATCH_HISTORY]
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
/****** Object:  StoredProcedure [dbo].[sp_get_load_batches]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_get_load_batches]
	@user_id bigint,
	@load_batch_reference VARCHAR(50) = NULL,
	@issuerId int,
	@load_batch_statuses_id int = NULL,
	@date_start DateTime = NULL,
	@date_end DateTime = NULL,
	@language_id INT,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if(@issuerId=0)
	set @issuerId=null

	if(@load_batch_reference <>'' or @load_batch_reference<> null)
	set @load_batch_reference ='%'+@load_batch_reference+'%'

	BEGIN TRANSACTION [GET_LOAD_BATCHES]
		BEGIN TRY 
			
			IF @date_end IS NOT NULL
				SET @date_end = DATEADD(day, 1, @date_end)
			
			DECLARE @StartRow INT, @EndRow INT;			

			SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
			SET @EndRow = @StartRow + @RowsPerPage - 1;

			--append#1
			WITH PAGE_ROWS
			AS
			(
			SELECT ROW_NUMBER() OVER(ORDER BY load_date ASC, load_batch_reference ASC) AS ROW_NO
					, COUNT(*) OVER() AS TOTAL_ROWS
					, *
			FROM( 
				SELECT DISTINCT [load_batch].load_batch_id, [load_batch].load_date, [load_batch].load_batch_reference, 
					   [load_batch].no_cards, [load_batch_status_current].load_batch_statuses_id, 
					   [load_batch_status_current].status_notes, [load_batch_statuses_language].language_text AS 'load_batch_status_name',
					   [branch].[branch_code]+' '+ [branch].branch_name as 'BranchName'
				FROM [load_batch] 
					INNER JOIN [load_batch_status_current]
						ON [load_batch_status_current].load_batch_id = [load_batch].load_batch_id
					INNER JOIN [load_batch_statuses_language]
						ON [load_batch_status_current].load_batch_statuses_id = [load_batch_statuses_language].load_batch_statuses_id
						  AND  [load_batch_statuses_language].language_id = @language_id
					INNER JOIN [load_batch_cards]
						ON [load_batch_cards].load_batch_id = [load_batch].load_batch_id
					INNER JOIN [cards]
						ON [cards].card_id = [load_batch_cards].card_id
					INNER JOIN [branch]
						ON [cards].branch_id = [branch].branch_id		
				WHERE 
					[load_batch].load_batch_reference LIKE COALESCE(@load_batch_reference, [load_batch].load_batch_reference)
					AND [branch].issuer_id = COALESCE(@issuerId,[branch].issuer_id)
					AND [load_batch_status_current].load_batch_statuses_id = COALESCE(@load_batch_statuses_id, [load_batch_status_current].load_batch_statuses_id)
					AND [load_batch].load_date BETWEEN COALESCE(@date_start, [load_batch].load_date)
													AND COALESCE(@date_end, [load_batch].load_date)
					--AND [load_batch_status].status_date = (SELECT MAX(lbs2.status_date)
					--									   FROM [load_batch_status] lbs2
					--									   WHERE lbs2.load_batch_id = [load_batch].load_batch_id)
			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY load_date ASC, load_batch_reference ASC

			--log the audit record		
			--EXEC sp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 'Getting Load batches.', 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_LOAD_BATCHES]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_LOAD_BATCHES]
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
/****** Object:  StoredProcedure [dbo].[sp_get_operator_cards_inprogress]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_operator_cards_inprogress] 
	@user_id bigint,
	@language_id INT,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
			
	DECLARE @StartRow INT, @EndRow INT;			
			
	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	--append#1
	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY status_date DESC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS			
			, *
	FROM( 				
		SELECT DISTINCT [cards].card_id, 
				CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) AS card_number,					   
				[branch_card_statuses_language].language_text AS current_card_status,						
				[branch_card_status_current].status_date,						
				[issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
				[branch].branch_id, [branch].branch_name, [branch].branch_code
		FROM [cards]
			INNER JOIN [branch_card_status_current]
				ON [branch_card_status_current].card_id = [cards].card_id						   
			INNER JOIN [branch_card_statuses_language]
				ON [branch_card_status_current].branch_card_statuses_id = [branch_card_statuses_language].branch_card_statuses_id
					AND [branch_card_statuses_language].language_id = @language_id							
			--Filter out cards linked to branches the user doesnt have access to.
			INNER JOIN (SELECT DISTINCT branch_id								
						FROM [user_roles_branch] INNER JOIN [user_roles]
								ON [user_roles_branch].user_role_id = [user_roles].user_role_id		
						WHERE [user_roles_branch].[user_id] = @user_id
								AND [user_roles].user_role_id IN (3)--Only want roles that allowed to search cards
						) as X
				ON [cards].branch_id = X.branch_id
			INNER JOIN [branch]
				ON [branch].branch_id = [cards].branch_id
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [branch].issuer_id
		WHERE  [branch].branch_status_id = 0	 
				AND [issuer].issuer_status_id = 0	
				AND [branch_card_status_current].operator_user_id = @user_id			  
				AND ([branch_card_status_current].branch_card_statuses_id = 3
					OR ([issuer].pin_mailer_printing_YN = 0 AND 
						[branch_card_status_current].branch_card_statuses_id = 4))
		UNION
		SELECT DISTINCT [cards].card_id, 
				CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) AS card_number,					   
				[branch_card_statuses_language].language_text AS current_card_status,						
				[branch_card_status_current].status_date,						
				[issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
				[branch].branch_id, [branch].branch_name, [branch].branch_code
		FROM [cards]
			INNER JOIN [branch_card_status_current]
				ON [branch_card_status_current].card_id = [cards].card_id						   
			INNER JOIN [branch_card_statuses_language]
				ON [branch_card_status_current].branch_card_statuses_id = [branch_card_statuses_language].branch_card_statuses_id							
					AND [branch_card_statuses_language].language_id = @language_id
			--Filter out cards linked to branches the user doesnt have access to.
			INNER JOIN (SELECT DISTINCT branch_id								
						FROM [user_roles_branch] INNER JOIN [user_roles]
								ON [user_roles_branch].user_role_id = [user_roles].user_role_id		
						WHERE [user_roles_branch].[user_id] = @user_id
								AND [user_roles].user_role_id = 7--Only want roles that allowed to search cards
						) as X
				ON [cards].branch_id = X.branch_id
			INNER JOIN [branch]
				ON [branch].branch_id = [cards].branch_id
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [branch].issuer_id
		WHERE  [branch].branch_status_id = 0	 
				AND [issuer].issuer_status_id = 0			  
				AND ([branch_card_status_current].branch_card_statuses_id = 5
					OR ([issuer].pin_mailer_printing_YN = 1 AND 
						[branch_card_status_current].branch_card_statuses_id = 6))
	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY status_date DESC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END


GO
/****** Object:  StoredProcedure [dbo].[sp_get_passwords_by_user_id]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 26 March 2014
-- Description:	Returns decrypted passwords
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_passwords_by_user_id] 
	@user_id bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		--For the current password add 99 days to show it's the latest. old passwords should be in the past.
		SELECT CONVERT(VARCHAR,DECRYPTBYKEY([user].[password])) as 'password', DATEADD(DAY, 99, GETDATE()) as 'date'						
		FROM [user]
		WHERE [user].[user_id] = @user_id
		UNION ALL
		SELECT CONVERT(VARCHAR,DECRYPTBYKEY([user_password_history].[password_history])) as 'password', 
			   [user_password_history].[date_changed] as 'date'						
		FROM [user_password_history]
		WHERE [user_password_history].[user_id] = @user_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_pin_batches]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_get_pin_batches]
@batch_status varchar(25),
@issuer_id int,
@dateFrom Datetime,
@dateTo Datetime
AS
BEGIN

	IF((@batch_status=null) OR (@batch_status='') 
	OR (@batch_status='INVALID'))
	BEGIN
		SET @batch_status= NULL
	END
	
	IF(@dateFrom = '1900-01-01 00:00:00.000') OR (@dateFrom like 'null')
		BEGIN			
			SET @dateFrom = NULL
		END
		
		IF(@dateTo = '1900-01-01 00:00:00.000') OR (@dateTo like 'null')
		BEGIN			
			SET @dateTo = NULL
		END
	
	SELECT * FROM pin_batch 
	WHERE loaded_date >= COALESCE(@dateFrom,loaded_date) AND
		  loaded_date <= COALESCE(@dateTo,loaded_date) AND
		  batch_status = @batch_status AND
		  issuer_id = @issuer_id
	ORDER BY loaded_date
END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_product_base_currency]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_product_base_currency] 
	-- Add the parameters for the stored procedure here
	@product_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT *
	FROM [product_currency]
	WHERE product_id = @product_id

END


GO
/****** Object:  StoredProcedure [dbo].[sp_get_productlist]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_productlist]
	
	@issuer_id int =null,
	@PageIndex INT = 1,
@RowsPerPage INT = 20
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY product_name ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
   SELECT        issuer_product.product_code, issuer_product.product_name, issuer_product.product_bin_code, issuer_product.issuer_id, issuer_product.name_on_card_top, 
                         issuer_product.product_id, issuer_product.name_on_card_left, issuer_product.Name_on_card_font_size, issuer_product.font_id, Issuer_product_font.font_name, 
                         Issuer_product_font.resource_path
FROM            issuer_product INNER JOIN
                         Issuer_product_font ON issuer_product.font_id = Issuer_product_font.font_id
						 where 
						issuer_id= @issuer_id 
						
						 )
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY product_name ASC
END






GO
/****** Object:  StoredProcedure [dbo].[sp_get_products_by_bincode]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch products for file loader
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_products_by_bincode] 
	-- Add the parameters for the stored procedure here
	@bin_code CHAR(6),
	@branch_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [issuer_product].*
	FROM [issuer_product]
		INNER JOIN [branch]
			ON [branch].issuer_id = [issuer_product].issuer_id
	WHERE [issuer_product].product_bin_code LIKE CONVERT(varchar,@bin_code + '%')
		  AND [branch].branch_id = @branch_id
END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_report_fields]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_report_fields]
@reportid int=null,
@languageid int=null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
SELECT        report_fields.reportfieldid, reportfields_language.language_text
FROM            reports INNER JOIN
                         report_reportfields ON reports.Reportid = report_reportfields.reportid INNER JOIN
                         report_fields ON report_reportfields.reportfieldid = report_fields.reportfieldid INNER JOIN
                         reportfields_language ON report_reportfields.reportfieldid = reportfields_language.reportfieldid

						 where reportfields_language.[language_id]=@languageid and reports.[Reportid]=@reportid
  

END


GO
/****** Object:  StoredProcedure [dbo].[sp_get_spoilcardsreport]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: 16/05/2014
-- Description:	for displaying issued card report
-- =============================================
----exec sp_get_issuedcardsreport 2,'6/14/2014 12:00:00 AM','6/27/2014 12:00:00 AM' ,25,null
CREATE PROCEDURE [dbo].[sp_get_spoilcardsreport]
	@isuerid int = null,
	@language_id int,
	@userid int = null,
	@branchid int = null,
	@fromdate datetime,
	@todate datetime	

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if(@userid = 0)
		set @userid = null

	if(@isuerid = 0)
		set @isuerid=null

	if(@branchid = 0)
		set @branchid = null

    -- Insert statements for procedure here
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
		
		SELECT DISTINCT [branch_code] as 'BranchCode',CONVERT(VARCHAR,DECRYPTBYKEY(u.[username])) as 'SpoliedBy','' as 'IssuedBy',
						CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.customer_first_name))+' '+ CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.customer_last_name)) as 'CustomerNames',
						CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.[customer_account_number])) as 'customeraccountNumber',
						CONVERT(VARCHAR,DECRYPTBYKEY(cards.[card_number])) as 'cardnumber',[branch_card_statuses_name] as 'CardStatus',[status_date] as'IssuedDate',bcl.[language_text] as 'Reason'
		FROM branch_card_status_current
				INNER JOIN cards  
					ON branch_card_status_current.[card_id] = cards.[card_id]
				INNER JOIN branch 
					ON cards.[branch_id] = branch.[branch_id]
				LEFT JOIN customer_account 
					ON cards.card_id = customer_account.card_id 
				INNER JOIN [user] u 
					ON u.[user_id] = branch_card_status_current.[user_id]
				INNER JOIN branch_card_statuses bs 
					ON bs.[branch_card_statuses_id] = branch_card_status_current.[branch_card_statuses_id] 
				INNER JOIN [branch_card_codes_language] bcl 
					ON branch_card_status_current.[branch_card_code_id] = bcl.[branch_card_code_id]
		  --left join (SELECT CONVERT(VARCHAR,DECRYPTBYKEY([user].username))as 'username' , card_id
		  --FROM branch_card_status 
		  --inner join [user] on [user].user_id=branch_card_status.user_id
		  --where branch_card_status.[branch_card_statuses_id]  = 6) AS Issued
		  --ON [cards].card_id = Issued.card_id
				INNER JOIN 
				(SELECT [branch_card_status].operator_user_id, [branch_card_status].card_id
				 FROM [branch_card_status]
				 WHERE [branch_card_status].branch_card_statuses_id = 2) AS Operator
					ON Operator.card_id = cards.[card_id]

		WHERE branch.issuer_id = COALESCE(@isuerid, branch.issuer_id)
				AND Operator.operator_user_id = COALESCE(@userid, Operator.operator_user_id)
				AND branch.branch_id = COALESCE(@branchid, branch.branch_id)
				AND branch_card_status_current.branch_card_statuses_id = 7  
				AND bcl.[language_id]=@language_id
				AND branch_card_status_current.[status_date] >= @fromdate 
				AND branch_card_status_current.[status_date] <= @todate 
				AND is_exception = 1
			  --ISNULL(branch.issuer_id, null) = ISNULL(@isuerid, ISNULL(branch.issuer_id, null)) and
		  --branch_card_status_current.branch_card_statuses_id = 7  and   bcl.[language_id]=@language_id
		  --and ISNULL(u.[user_id],null)=ISNULL(@userid, ISNULL(u.[user_id], null))

		  --and ISNULL(branch.branch_id, null)=ISNULL(@branchid, ISNULL(branch.branch_id, null))

		 --and branch_card_status_current.[status_date] >=@fromdate and branch_card_status_current.[status_date]<=@todate 
		--						   and  is_exception = 1
		ORDER BY CardStatus
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_Spoilcardsummaryreport]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE PROCEDURE [dbo].[sp_get_Spoilcardsummaryreport]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int,
	@fromdate datetime,
	@todate datetime
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	if(@issuer_id = -1 or @issuer_id = 0)
	 set @issuer_id=null

	if(@branch_id  =0)
		set @branch_id = null

	SELECT INTERX.issuer_id,
		   branch_code,
		   INTERX.branch_id,
		   INTERX.branch_card_code_id,
		   INTERX.Spoil_reason_name,
		   COUNT(INTER.card_id)	AS CardCount,issuer.issuer_name as 'issuer_name'
	FROM 
		-- this sub select fetches all cards belonging to a branch and currently in issued status
		(SELECT [branch].branch_id, [branch_card_status_current].card_id, [branch_card_status_current].branch_card_code_id
			FROM [branch_card_status_current] 
					INNER JOIN [customer_account]
						ON [customer_account].card_id = [branch_card_status_current].card_id 
					INNER JOIN [cards]
						ON [branch_card_status_current].card_id = [cards].card_id
					INNER JOIN [branch]
						ON [cards].branch_id = [branch].branch_id
			WHERE [branch_card_status_current].branch_card_statuses_id = 7
				  AND [branch_card_status_current].[status_date] >= @fromdate 
				  AND [branch_card_status_current].[status_date] <= @todate
		) AS INTER						
		RIGHT OUTER JOIN 		
		--This Sub Select creates a cartesian product of branch and card issue reason	
		(SELECT issuer_id, [branch].branch_id, branch_code, branch_card_codes.branch_card_code_id,
				 language_text as 'Spoil_reason_name'
			FROM [branch], branch_card_codes
					INNER JOIN branch_card_codes_language 
						ON branch_card_codes.branch_card_code_id = branch_card_codes_language.branch_card_code_id
						   AND branch_card_codes_language.language_id = @language_id 						  
						   and  branch_card_codes.is_exception = 1
						   Where    [branch].branch_status_id = 0
		 	)  INTERX
		ON INTER.branch_id = INTERX.branch_id
			AND INTER.branch_card_code_id = INTERX.branch_card_code_id 
			inner join issuer on issuer.issuer_id=INTERX.issuer_id
	WHERE INTERX.issuer_id = COALESCE(@issuer_id, INTERX.issuer_id)
		AND INTERX.branch_id = COALESCE(@branch_id,  INTERX.branch_id)	
		AND INTERX.branch_code <> '' 
	GROUP BY INTERX.issuer_id,  INTERX.branch_id,INTERX.branch_code,  INTERX.branch_card_code_id,INTERX.Spoil_reason_name,issuer_name
	ORDER BY issuer_name,  INTERX.branch_code, INTERX.branch_card_code_id
END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_unassigned_users]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get all users not assigned to any groups.
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_unassigned_users] 
	@languageId int =null,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_UNASSIGNED_USERS]
		BEGIN TRY 

		DECLARE @StartRow INT, @EndRow INT;			

		SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
		SET @EndRow = @StartRow + @RowsPerPage - 1;

		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate;

		WITH PAGE_ROWS
		AS
		(
		SELECT ROW_NUMBER() OVER(ORDER BY username ASC) AS ROW_NO
				, COUNT(*) OVER() AS TOTAL_ROWS
				, *
		FROM( 
			SELECT DISTINCT [user].[user_id]						
							,CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'username'
							,CONVERT(VARCHAR,DECRYPTBYKEY([user].[first_name])) as 'first_name'
							,CONVERT(VARCHAR,DECRYPTBYKEY([user].[last_name])) as 'last_name' 					
							,CONVERT(VARCHAR,DECRYPTBYKEY([user].[employee_id])) as 'empoyee_id'
							,usl.language_text as  [user_status_text] 
							,[user].[online]    
							,[user].[workstation],case when ([user].[ldap_setting_id] is null) then 'Indigo' else 'LDAP' end as 'AUTHENTICATION_TYPE'
				FROM [user]
					INNER JOIN user_status
						ON user_status.user_status_id = [user].user_status_id
					INNER JOIN  [dbo].[user_status_language] usl
						 ON usl.user_status_id=user_status.user_status_id 
				WHERE [user].[user_id] NOT IN (SELECT DISTINCT [user_id] FROM [users_to_users_groups])  AND usl.language_id=@languageId
			) AS Src )
		SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
			,*
		FROM PAGE_ROWS
		WHERE ROW_NO BETWEEN @StartRow AND @EndRow
		ORDER BY username ASC

		CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

		--log the audit record		
		--EXEC sp_insert_audit @audit_user_id, 
		--						1,
		--						NULL, 
		--						@audit_workstation, 
		--						'Getting unassigned users.', 
		--						NULL, NULL, NULL, NULL

		COMMIT TRANSACTION [GET_UNASSIGNED_USERS]
	END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION [GET_UNASSIGNED_USERS]
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
/****** Object:  StoredProcedure [dbo].[sp_get_user]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_get_user]
	(
		@username varchar(256),
		@user_status varchar(30)
	)
AS
	/* SET NOCOUNT ON */

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

    SELECT user_id, u.user_status_id, user_gender_id, username, first_name,
		   last_name, CONVERT(VARCHAR,DECRYPTBYKEY(password)), online, employee_id, last_login_date,
		   last_login_attempt, number_of_incorrect_logins, last_password_changed_date,
		   workstation 
	FROM [user] u,user_status us
	WHERE (DECRYPTBYKEY(username)=@username And us.user_status_text = @user_status)
	
CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;



GO
/****** Object:  StoredProcedure [dbo].[sp_get_user_by_user_id]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 24 March 2014
-- Description:	Get a user based on the users system ID.
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_user_by_user_id] 
	-- Add the parameters for the stored procedure here
	@user_id bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		--There should only be one result
		SELECT TOP 1 [user].[user_id]						
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'username'
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[first_name])) as 'first_name'
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[last_name])) as 'last_name' 					
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[employee_id])) as 'empoyee_id'
						,[user].[user_email]
						,[user].[user_status_id] 
						,[user].[ldap_setting_id]
						,[user].[language_id]
						,[user].[online]    
						,[user].[workstation]
		FROM [user]
		WHERE [user].[user_id] = @user_id		

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_user_by_username]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 14 March 2014
-- Description:	Return a user based on the username
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_user_by_username] 
	-- Add the parameters for the stored procedure here
	@username varchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		--There should only be one result
		SELECT TOP 1 [user].[user_id]						
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'username'
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[first_name])) as 'first_name'
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[last_name])) as 'last_name' 					
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[employee_id])) as 'empoyee_id'
						,[user].[user_status_id] 
						,[user].[online]    
						,[user].[workstation]
		FROM [user]
		WHERE CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) = @username		

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_user_for_login]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 20 March 2014
-- Description:	Gets a user for login purposes.
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_user_for_login] 
	-- Add the parameters for the stored procedure here
@username varchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		--There should only be one result
		SELECT TOP 1 [user].[user_id]						
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'clr_username'
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[password])) as 'clr_password'
						,[user].[online]    
						,[user].[workstation]
						,[user].last_login_attempt
						,[user].last_password_changed_date
						,[user].number_of_incorrect_logins
						,[user].language_id
						,[user].ldap_setting_id	
						,[user].user_status_id
						,[ldap_setting].hostname_or_ip as domain_hostname_or_ip
						,[ldap_setting].domain_name
						,[ldap_setting].[path] as domain_path
						,CONVERT(VARCHAR,DECRYPTBYKEY([ldap_setting].username)) as domain_username	
						,CONVERT(VARCHAR,DECRYPTBYKEY([ldap_setting].[password])) as domain_password										
		FROM [user] 
			LEFT OUTER JOIN [ldap_setting]
				ON [user].ldap_setting_id = [ldap_setting].ldap_setting_id
		WHERE CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) = @username

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_user_groups_admin]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Gets a list of user groups and indicates if the user has been assigned to it.
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_user_groups_admin] 
	-- Add the parameters for the stored procedure here
	@user_id bigint = null,
	@issuer_id int = NULL,	
	@branch_id int = NULL,
	@user_role_id int = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT ug.user_group_id, ug.user_group_name, ug.user_role_id, ug.issuer_id,
	CASE WHEN [users_to_users_groups].[user_id] IS NULL THEN 0 ELSE 1 END 'is_in_group'
	FROM [user_group] ug
		LEFT OUTER JOIN [users_to_users_groups]
					ON ug.user_group_id = [users_to_users_groups].user_group_id
						AND [users_to_users_groups].[user_id] = @user_id
	WHERE (@branch_id IS NULL OR 
		((ug.all_branch_access = 1 AND
			EXISTS (SELECT b.*
					FROM [branch] b
					WHERE b.issuer_id = ug.issuer_id
						  AND b.branch_id = COALESCE(@branch_id, b.branch_id)))
		 OR 
		  (ug.all_branch_access = 0 AND
			  EXISTS (SELECT ugb.* 
					  FROM [user_groups_branches] ugb
					  WHERE ugb.user_group_id = ug.user_group_id
							AND ugb.branch_id = COALESCE(@branch_id, ugb.branch_id)))))
		 AND ug.issuer_id = COALESCE(@issuer_id, ug.issuer_id)
		 AND ug.user_role_id = COALESCE(@user_role_id, ug.user_role_id)
		  
END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_user_groups_by_issuer]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 13 March 2014
-- Description:	Get user groups by issuer.
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_user_groups_by_issuer] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@user_role_id int = null,	
	@languageId int =null,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_USER_GROUPS]
		BEGIN TRY 

		DECLARE @StartRow INT, @EndRow INT;			

		SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
		SET @EndRow = @StartRow + @RowsPerPage - 1;

		WITH PAGE_ROWS
		AS
		(
		SELECT ROW_NUMBER() OVER(ORDER BY user_group_name ASC) AS ROW_NO
				, COUNT(*) OVER() AS TOTAL_ROWS
				, *
		FROM( 

			SELECT DISTINCT [user_group].*, url.language_text as user_role
			FROM [user_group] 
				INNER JOIN [issuer]
					ON user_group.issuer_id = issuer.issuer_id
				INNER JOIN [user_roles]
					ON [user_group].user_role_id = [user_roles].user_role_id
						INNER JOIN  [dbo].user_roles_language url 
					ON url.user_role_id=[user_roles].user_role_id 
			WHERE [user_group].issuer_id = @issuer_id
					AND [user_group].user_role_id = COALESCE(@user_role_id, [user_group].user_role_id)			
						AND url.language_id=@languageId
		) AS Src )
		SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
			,*
		FROM PAGE_ROWS
		WHERE ROW_NO BETWEEN @StartRow AND @EndRow
		ORDER BY user_group_name ASC

		CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

		--log the audit record		
		--EXEC sp_insert_audit @audit_user_id, 
		--						1,
		--						NULL, 
		--						@audit_workstation, 
		--						'Getting user groups.', 
		--						NULL, NULL, NULL, NULL

		COMMIT TRANSACTION [GET_USER_GROUPS]
	END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION [GET_USER_GROUPS]
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
/****** Object:  StoredProcedure [dbo].[sp_get_user_roles_for_user]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 11 March 2014
-- Description:	Get all the roles linked to the specified user.
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_user_roles_for_user]
	@user_id bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [user_group].user_role_id, [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
			[issuer].maker_checker_YN, [issuer].account_validation_YN, [issuer].auto_create_dist_batch,
			[user_roles].allow_multiple_login, 
			[user_group].can_read, [user_group].can_create, [user_group].can_update
	FROM [user_group] 
			INNER JOIN [users_to_users_groups] 
				ON [user_group].user_group_id = [users_to_users_groups].user_group_id
			INNER JOIN [user_roles]
				ON [user_group].user_role_id = [user_roles].user_role_id
			INNER JOIN [user] 
				ON [user].[user_id] = [users_to_users_groups].[user_id]			 
			INNER JOIN [issuer] 
				ON [user_group].issuer_id = [issuer].issuer_id					
	WHERE [user].[user_id] = @user_id
		  AND [issuer].issuer_status_id = 0
		  AND [user].user_status_id = 0
	GROUP BY [user_group].user_role_id, [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
			[issuer].maker_checker_YN, [issuer].account_validation_YN, [issuer].auto_create_dist_batch,
			[user_roles].allow_multiple_login, [user_group].can_read, [user_group].can_create, [user_group].can_update
END




GO
/****** Object:  StoredProcedure [dbo].[sp_get_usergroup]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_get_usergroup]
 @user_group_id int,
 @audit_user_id bigint,
 @audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT *
	FROM [user_group]
	WHERE user_group_id = @user_group_id

END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_userroles_enterprise]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_userroles_enterprise]
	@language_id int,
	@enterprise_only int =null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
SELECT        user_roles_language.user_role_id AS lookup_id, language_text
FROM            user_roles INNER JOIN
                         user_roles_language ON user_roles.user_role_id = user_roles_language.user_role_id
						 where user_roles_language.language_id=@language_id and enterprise_only = COALESCE(@enterprise_only, user_roles.enterprise_only)
END


GO
/****** Object:  StoredProcedure [dbo].[sp_get_users_by_branch]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 13 March 2014
-- Description:	Get all ussers linked to the specified branch.
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_users_by_branch] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@branch_id int = null,
	@branch_status_id int = null,
	@user_status_id int = null,
	@user_role_id int = null,
	@username varchar(100) = null,
	@first_name varchar(100) = null,
	@last_name varchar(100) = null,
	@languageId int =null,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @username = '' OR @username IS NULL
		SET @username = NULL
	ELSE
		SET @username = '%' + @username + '%'

	IF @first_name = '' OR @first_name IS NULL
		SET @first_name = NULL
	ELSE
		SET @first_name = '%' + @first_name + '%'

	IF @last_name = '' OR @last_name IS NULL
		SET @last_name = NULL
	ELSE
		SET @last_name = '%' + @last_name + '%'

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY username ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
		SELECT DISTINCT [user].[user_id]						
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'username'
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[first_name])) as 'first_name'
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[last_name])) as 'last_name' 					
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[employee_id])) as 'empoyee_id'
						,usl.language_text as [user_status_text] 
						,[user].[online]    
						,[user].[workstation],case when ([user].[ldap_setting_id] is null) then 'Indigo' else 'LDAP' end as 'AUTHENTICATION_TYPE'
			FROM [branch] INNER JOIN [user_roles_branch]
					ON [branch].branch_id = [user_roles_branch].branch_id
				INNER JOIN [user]
					ON [user].[user_id] = [user_roles_branch].[user_id]
				INNER JOIN user_status
					ON user_status.user_status_id = [user].user_status_id
				INNER JOIN  [dbo].[user_status_language] usl 
					ON usl.user_status_id=user_status.user_status_id 
			WHERE [user_roles_branch].issuer_id = COALESCE(@issuer_id, [user_roles_branch].issuer_id)
				AND (@username IS NULL OR CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) LIKE @username) 
				AND (@first_name IS NULL OR CONVERT(VARCHAR,DECRYPTBYKEY([user].[first_name])) LIKE @first_name) 
				AND (@last_name IS NULL OR CONVERT(VARCHAR,DECRYPTBYKEY([user].[last_name])) LIKE @last_name) 
				AND [branch].branch_id = COALESCE(@branch_id, branch.branch_id)				
				AND [branch].branch_status_id = COALESCE(@branch_status_id, branch.branch_status_id)
				AND [user].user_status_id = COALESCE(@user_status_id, [user].user_status_id)
				AND [user_roles_branch].user_role_id = COALESCE(@user_role_id, [user_roles_branch].user_role_id)
				AND usl.language_id=@languageId
		) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY username ASC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_users_by_branch_admin]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 13 March 2014
-- Description:	Get all ussers linked to the specified branch for user admin
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_users_by_branch_admin] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@branch_id int = null,
	@branch_status_id int = null,
	@user_status_id int = null,
	@user_role_id int = null,
	@username varchar(100) = null,
	@first_name varchar(100) = null,
	@last_name varchar(100) = null,
	@languageId int =null,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @username = '' OR @username IS NULL
		SET @username = NULL
	ELSE
		SET @username = '%' + @username + '%'

	IF @first_name = '' OR @first_name IS NULL
		SET @first_name = NULL
	ELSE
		SET @first_name = '%' + @first_name + '%'

	IF @last_name = '' OR @last_name IS NULL
		SET @last_name = NULL
	ELSE
		SET @last_name = '%' + @last_name + '%'

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY username ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
		SELECT DISTINCT [user].[user_id]						
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'username'
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[first_name])) as 'first_name'
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[last_name])) as 'last_name' 					
						,CONVERT(VARCHAR,DECRYPTBYKEY([user].[employee_id])) as 'empoyee_id'
						,usl.language_text as  [user_status_text] 
						,[user].[online]    
						,[user].[workstation],case when ([user].[ldap_setting_id] is null) then 'Indigo' else 'LDAP' end as 'AUTHENTICATION_TYPE'
			FROM [branch] INNER JOIN [user_group_branch_ex_ent]
					ON [branch].branch_id = [user_group_branch_ex_ent].branch_id
				INNER JOIN [user]
					ON [user].[user_id] = [user_group_branch_ex_ent].[user_id]
				INNER JOIN user_status
					ON user_status.user_status_id = [user].user_status_id
				INNER JOIN  [dbo].[user_status_language] usl 
					ON usl.user_status_id=user_status.user_status_id 
			WHERE [user_group_branch_ex_ent].issuer_id = COALESCE(@issuer_id, [user_group_branch_ex_ent].issuer_id)
				AND (@username IS NULL OR CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) LIKE @username) 
				AND (@first_name IS NULL OR CONVERT(VARCHAR,DECRYPTBYKEY([user].[first_name])) LIKE @first_name) 
				AND (@last_name IS NULL OR CONVERT(VARCHAR,DECRYPTBYKEY([user].[last_name])) LIKE @last_name) 
				AND [branch].branch_id = COALESCE(@branch_id, branch.branch_id)				
				AND [branch].branch_status_id = COALESCE(@branch_status_id, branch.branch_status_id)
				AND [user].user_status_id = COALESCE(@user_status_id, [user].user_status_id)
				AND [user_group_branch_ex_ent].user_role_id = COALESCE(@user_role_id, [user_group_branch_ex_ent].user_role_id)
				AND usl.language_id=@languageId and [branch].branch_status_id=0
		) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY username ASC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END



GO
/****** Object:  StoredProcedure [dbo].[sp_get_users_by_institution]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_get_users_by_institution]
@branchCode varchar(15),
@issuerID int
AS
BEGIN

	IF((@branchCode='NULL') OR (@branchCode='') OR (@branchCode IS NULL))
		BEGIN
			--SET @branchCode= NULL
			SELECT * FROM application_user  
			WHERE  issuerID =  @issuerID
		END
	ELSE
		BEGIN
			SELECT * FROM application_user  
			WHERE  issuerID =  @issuerID
				AND  branchCode = @branchCode
		END
	
	--SELECT * FROM application_user  
	--WHERE  issuerID =  @issuerID
	--AND  branchCode= COALESCE(@branchCode, branchCode  )
END



GO
/****** Object:  StoredProcedure [dbo].[sp_GetAuditData]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE PROCEDURE [dbo].[sp_GetAuditData]
	@audit_action_id int = NULL,
	@user_role_id int = NULL,
	@username varchar(50) = NULL,
	@issuer_id int = NULL,
	@date_from datetime ,
	@date_to datetime,	
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@aduti_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY audit_date DESC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
				SELECT CONVERT(VARCHAR,DECRYPTBYKEY([user].username)) as 'UserName', [workstation_address],
					   audit_date, [audit_action].audit_action_name, [audit_control].action_description, 
					   [audit_control].issuer_id 
				FROM [audit_control]
						INNER JOIN [audit_action] 
							ON [audit_control].audit_action_id = [audit_action].audit_action_id
						INNER JOIN [user]
							ON [user].[user_id] = [audit_control].[user_id]												
				WHERE [audit_control].audit_action_id = COALESCE(@audit_action_id, [audit_control].audit_action_id)
					AND audit_date BETWEEN @date_from AND DATEADD(day, 1, @date_to)					
					AND ((@username IS NULL) OR (CONVERT(VARCHAR,DECRYPTBYKEY([user].username)) LIKE @username))
					--AND [audit_control].issuer_id = COALESCE(@issuer_id, [audit_control].issuer_id)	
					AND [user].[user_id] IN (SELECT [user_id] 
											 FROM user_group
											 inner join users_to_users_groups on users_to_users_groups.[user_group_id]=user_group.[user_group_id]
											 WHERE user_group.user_role_id = COALESCE(@user_role_id, user_group.[user_role_id])
													AND user_group.issuer_id = COALESCE(@issuer_id, user_group.issuer_id))
						--	ON [user].[user_id] = [user_roles_branch].[user_id]	
						--	AND [user_roles_branch].user_role_id = COALESCE(@user_role_id, [user_roles_branch].user_role_id )	 
		)
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY audit_date DESC

	
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END




GO
/****** Object:  StoredProcedure [dbo].[sp_GetFileLoderLog]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: 19 may 2014
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetFileLoderLog]
	@file_load_id int = null,
	@file_status_id int = NULL,
	@name_of_file varchar(60) = NULL,
	@issuer_id int = NULL,
	@date_from  datetime = null,	
	@date_to datetime = null,
	@languageId int =null,
	@PageIndex int = 1,
	@RowsPerPage int = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @StartRow INT, @EndRow INT;			
	if(@issuer_id=0)
	set @issuer_id=null

	if(@name_of_file <>'' or @name_of_file <> null)
	set @name_of_file= '%'+@name_of_file+'%'


	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY load_date DESC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
		SELECT ISNULL(fh.issuer_id, 0) AS issuer_id, fh.[file_id], fsl.language_text as file_status , fh.number_successful_records, fh.number_failed_records, 
               fh.file_load_comments,fh.name_of_file,fh.[file_created_date],fh.file_size,fh.load_date,fh.file_directory,ft.file_type
		FROM file_history fh 
				INNER JOIN file_statuses fs 
					ON fh.file_status_id = fs.file_status_id
				INNER JOIN file_types ft 
					ON fh.file_type_id = ft.file_type_id
					inner join file_statuses_language fsl on fsl.file_status_id=fs.file_status_id
		WHERE fh.file_load_id = COALESCE(@file_load_id, fh.file_load_id)
			  AND fh.file_status_id = COALESCE(@file_status_id, fh.file_status_id)
			  AND ((@date_from IS NULL) OR (fh.load_date BETWEEN @date_from AND DATEADD(day, 1, @date_to)))
			  AND ((@name_of_file IS NULL) OR (fh.name_of_file LIKE @name_of_file))
			  AND ((@issuer_id IS NULL) OR (fh.issuer_id = @issuer_id))
			  AND fsl.language_id=@languageId	 
		)
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY load_date DESC	

END




GO
/****** Object:  StoredProcedure [dbo].[sp_getFont_by_fontid]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getFont_by_fontid]
@fontid int 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

		Select font_id,font_name,resource_path,DeletedYN,0 as TOTAL_ROWS, ROW_NUMBER() OVER(ORDER BY font_name ASC) AS ROW_NO,0 as TOTAL_PAGES from Issuer_product_font where font_id=@fontid

END





GO
/****** Object:  StoredProcedure [dbo].[sp_getFontsList]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getFontsList]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select  font_id,font_name,'' as resource_path,[DeletedYN] from Issuer_product_font where [DeletedYN]=0
END






GO
/****** Object:  StoredProcedure [dbo].[sp_getProductbyProductid]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getProductbyProductid]
	@productid int 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	 SELECT        issuer_product.product_code, issuer_product.product_name, issuer_product.product_bin_code, issuer_product.issuer_id, issuer_product.name_on_card_top, 
                         issuer_product.product_id, issuer_product.name_on_card_left, issuer_product.Name_on_card_font_size, issuer_product.font_id, Issuer_product_font.font_name, 
                         Issuer_product_font.resource_path,0 as TOTAL_ROWS, ROW_NUMBER() OVER(ORDER BY product_name ASC) AS ROW_NO,0 as TOTAL_PAGES
FROM            issuer_product INNER JOIN
                         Issuer_product_font ON issuer_product.font_id = Issuer_product_font.font_id
						 where product_id=@productid

END




GO
/****** Object:  StoredProcedure [dbo].[sp_getproductcode]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getproductcode]
	-- Add the parameters for the stored procedure here
	@issuerid int,
	@bincode varchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	select top 1 * from issuer_product
	where product_bin_code=@bincode and issuer_id=@issuerid

	--select top 1 product_code,product_name from issuer_product
	--where product_bin_code=@bincode and issuer_id=@issuerid

END


GO
/****** Object:  StoredProcedure [dbo].[sp_insert_branch]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Persist new branch to db
-- =============================================
CREATE PROCEDURE [dbo].[sp_insert_branch] 
	@branch_status_id int,
	@issuer_id int,
	@branch_code varchar(10),
	@branch_name varchar(30),
	@location varchar(20),
	@contact_person varchar(30),
	@contact_email varchar(30),
	@card_centre varchar(10),	 
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@new_branch_id int OUTPUT,
	@ResultCode int OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	

		

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [branch] WHERE ([branch_code] = @branch_code AND [issuer_id] = @issuer_id)) > 0
				BEGIN
					SET @new_branch_id = 0
					SET @ResultCode = 211						
				END
			ELSE IF (SELECT COUNT(*) FROM [branch] WHERE ([branch_name] = @branch_name AND [issuer_id] = @issuer_id)) > 0
				BEGIN
					SET @new_branch_id = 0
					SET @ResultCode = 210
				END
			ELSE
			BEGIN	
			
			BEGIN TRANSACTION [INSERT_BRANCH_TRAN]
				BEGIN TRY 		

				INSERT INTO [branch]
						([branch_status_id],[issuer_id],[branch_code],[branch_name],[location]
						,[contact_person],[contact_email],[card_centre])
					VALUES
						(@branch_status_id, @issuer_id, @branch_code, @branch_name, @location,
						@contact_person, @contact_email, @card_centre)

				SET @new_branch_id = SCOPE_IDENTITY();

				DECLARE @issuer_code varchar(10)
				SELECT @issuer_code = issuer_code
				FROM issuer
				WHERE issuer_id = @issuer_id

				DECLARE @group_name varchar(50),
						@new_group_id int
				SET @group_name =  @issuer_code + '_' + @branch_code + '_CUSTODIAN'

				--Insert Default user groups
				INSERT INTO [user_group]
					(all_branch_access, can_create, can_delete, can_read, can_update, issuer_id,
						user_group_name, user_role_id)
				VALUES 
					(0, 1, 1, 1, 1, @issuer_id, @group_name, 2)

				SET @new_group_id = SCOPE_IDENTITY();

				INSERT INTO [user_groups_branches]
					(branch_id, user_group_id)
				VALUES 
					(@new_branch_id, @new_group_id)

				SET @group_name =  @issuer_code + '_' + @branch_code + '_OPERATOR'
				INSERT INTO [user_group]
					(all_branch_access, can_create, can_delete, can_read, can_update, issuer_id,
						user_group_name, user_role_id)
				VALUES 
					(0, 1, 1, 1, 1, @issuer_id, @group_name, 3)

				SET @new_group_id = SCOPE_IDENTITY();

				INSERT INTO [user_groups_branches]
					(branch_id, user_group_id)
				VALUES 
					(@new_branch_id, @new_group_id)

				--log the audit record
				DECLARE @audit_description varchar(500)
				DECLARE @branchstatus  varchar(50)

				SELECT @branchstatus = branch_statuses.[branch_status]
				FROM branch_statuses 
				WHERE branch_statuses.branch_status_id = @branch_status_id

				SELECT @audit_description = 'Create: ID ' + CAST(@new_branch_id AS varchar(max))	+ ', [' + CAST(@issuer_id as varchar(100)) + ';' + @issuer_code + '], [' +
											@branch_code + ';' + @branch_name + ', ' + @branchstatus + ']'

				EXEC sp_insert_audit @audit_user_id, 
									 0,--BranchAdmin
									 NULL,
									 @audit_workstation, 
									 @audit_description, 
									 @issuer_id, NULL, NULL, NULL

				SET @ResultCode = 0

				COMMIT TRANSACTION [INSERT_BRANCH_TRAN]			
		END TRY
	BEGIN CATCH		
		ROLLBACK TRANSACTION [INSERT_BRANCH_TRAN]
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




GO
/****** Object:  StoredProcedure [dbo].[sp_insert_cards_forBatch]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Batch submitted through debugger: SQLQuery2.sql|0|0|C:\Users\gumani.maumela\AppData\Local\Temp\~vsA9C7.sql

CREATE PROCEDURE [dbo].[sp_insert_cards_forBatch]
@batchReference varchar(25),
--@card_numbers varchar(8000),
@card_numbers AS dbo.DistBatchCards READONLY,
@sequenceNumbers varchar(1266),
@card_status varchar(10)

AS BEGIN

	--DECLARE @pos  int,@pos_seq int
	--DECLARE @card_number varchar(19),@sequenceNumber varchar(3)

	--IF(@card_numbers = NULL OR @card_numbers='null')
	--BEGIN
	--	SET @card_numbers=''
	--END

	--IF(@sequenceNumbers = NULL OR @sequenceNumbers='null')
	--BEGIN
	--	SET @sequenceNumbers=''
	--END


	--SET @card_numbers = LTRIM(RTRIM(@card_numbers))
	--SET @sequenceNumbers = LTRIM(RTRIM(@sequenceNumbers))

	--SET @pos = CHARINDEX(',',@card_numbers)
	--SET @pos_seq = CHARINDEX(',',@sequenceNumbers)


	--MM Chages  (PCI - DSS)
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate
	
	--WHILE (@pos>0 AND @pos_seq>0) 
	--BEGIN
		--SET @card_number = SUBSTRING(@card_numbers,1,@pos-1)	
		--SET @card_numbers = LTRIM(RTRIM(SUBSTRING(@card_numbers,@pos+1,LEN(@card_numbers)- @pos)))
		--SET @pos=CHARINDEX(',',@card_numbers)
		
		--SET @sequenceNumber = SUBSTRING(@sequenceNumbers,1,@pos_seq-1)	
		--SET @sequenceNumbers = LTRIM(RTRIM(SUBSTRING(@sequenceNumbers,@pos_seq+1,LEN(@sequenceNumbers)- @pos_seq)))
		--SET @pos_seq=CHARINDEX(',',@sequenceNumbers)
		
		
	
		
		--INSERT INTO load_card
		--		   (card_number
		--		   ,card_sequence
		--		   ,load_batch_reference
		--		   ,card_status)
		--VALUES
		--		   (
		--			ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@card_number))
		--		   ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@sequenceNumber))
		--		   ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@batchReference))
		--		   ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@card_status))
		--		   )

		INSERT INTO load_card
				   (card_number
				   ,card_sequence
				   ,load_batch_reference
				   ,card_status)
		SELECT ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,card_number))
				   ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'0'))
				   ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@batchReference))
				   ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@card_status))	
		FROM @card_numbers
		WHERE card_number is not null AND
				card_number <> ''
				   
	 
	--END
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
	
END



GO
/****** Object:  StoredProcedure [dbo].[sp_insert_file_history]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 6 March 2014
-- Description:	Insert to file_history table.
-- =============================================
CREATE PROCEDURE [dbo].[sp_insert_file_history] 	
	@file_load_id INT,
	@issuer_id int = NULL,
	@name_of_file varchar(60),
	@file_created_date datetime,
	@file_size int,
	@load_date datetime,
	@file_status_id int,
	@file_directory varchar(110),
	@number_successful_records int,
	@number_failed_records int,
	@file_load_comments varchar(max),
	@file_type_id int,
	@file_id bigint = NULL OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [dbo].[file_history]
           ([issuer_id],[name_of_file],[file_created_date],[file_size],[load_date],[file_status_id],[file_directory],
		    [number_successful_records],[number_failed_records],[file_load_comments],[file_type_id], [file_load_id])
     VALUES
		    (@issuer_id, @name_of_file, @file_created_date, @file_size, @load_date, @file_status_id, @file_directory,
			 @number_successful_records, @number_failed_records, @file_load_comments, @file_type_id, @file_load_id)

	SET @file_id = SCOPE_IDENTITY();
END




GO
/****** Object:  StoredProcedure [dbo].[sp_insert_FlexcubeAudit]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_insert_FlexcubeAudit] 
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100),
	@audit_description varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	EXEC sp_insert_audit @audit_user_id, 
						 3,
						 NULL, 
						 @audit_workstation, 
						 @audit_description, 
						 NULL, NULL, NULL, NULL
END




GO
/****** Object:  StoredProcedure [dbo].[sp_insert_font]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_insert_font]
@font_name nvarchar(50),
@resource_path nvarchar(max),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
@ResultCode int =null OUTPUT ,
@new_font_id int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		BEGIN TRANSACTION [INSERT_FONT_TRAN]
		BEGIN TRY 
    -- Insert statements for procedure here
			DECLARE @dup_check int
			SELECT @dup_check = COUNT(*) 
			FROM [dbo].[Issuer_product_font]  
			WHERE  font_name= @font_name 
			IF @dup_check > 0
				BEGIN
					SELECT @ResultCode = 69							
				END
				
			ELSE
			BEGIN
			
			INSERT INTO  dbo.[Issuer_product_font] (font_name,resource_path) Values(@font_name,@resource_path)
		

			set @new_font_id=SCOPE_IDENTITY();

				--						DECLARE @audit_description nvarchar(500)
				--SELECT @audit_description = 'Font Inserted: ' + @font_name  
																	
				--EXEC sp_insert_audit @audit_user_id, 
				--					 0,
				--					 NULL, 
				--					 @audit_workstation, 
				--					 @audit_description, 
				--					 NULL, NULL, NULL, NULL

				SELECT @ResultCode = 0	
				COMMIT TRANSACTION [INSERT_FONT_TRAN]
				END
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_FONT_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_insert_load_batch]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 6 March 2014
-- Description:	Insert into load_batch table and returns the id of the inserted row.
-- =============================================
CREATE PROCEDURE [dbo].[sp_insert_load_batch] 
	@load_batch_reference varchar(50),
	@file_id bigint,
	@issuer_id int,
	@load_batch_status_id int,
	@load_date datetime,
	@number_of_cards int,
	@load_batch_id bigint = NULL OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [dbo].[load_batch]
			([load_batch_reference],[file_id],[load_batch_status_id],[load_date],[no_cards])
	VALUES
			(@load_batch_reference, @file_id, @load_batch_status_id, @load_date, @number_of_cards)

	SET @load_batch_id = SCOPE_IDENTITY();
END




GO
/****** Object:  StoredProcedure [dbo].[sp_insert_load_batch_status]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 6 March 2014
-- Description:	Insert into load_batch_status
-- =============================================
CREATE PROCEDURE [dbo].[sp_insert_load_batch_status] 	
	@load_batch_id bigint, 
	@load_batch_statuses_id int,
	@status_date datetime,
	@user_id bigint,
	@load_batch_status_id bigint = null OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [load_batch_status]
	       ([load_batch_id],[load_batch_statuses_id],[status_date],[user_id],[status_notes])
     VALUES
		    (@load_batch_id, @load_batch_statuses_id, @status_date, @user_id, '')

	SET @load_batch_status_id = SCOPE_IDENTITY();
           
END




GO
/****** Object:  StoredProcedure [dbo].[sp_insert_product]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [sp_insert_product] 'Ghana_Visa_Black_Card','GVBC02','484680',-1,50.00,50.00,15,2,null,-1,'veneka-04'
CREATE PROCEDURE [dbo].[sp_insert_product]
	-- Add the parameters for the stored procedure here
	@product_name varchar(100),
	@product_code varchar(50),
	@product_bin_code varchar(9),
	@issuer_id  int,
	@name_on_card_top decimal(8,2),
	@name_on_card_left decimal(8,2),
	@Name_on_card_font_size int,
	@font_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@currencylist AS dbo.currency_id_array READONLY,
	@ResultCode int =null OUTPUT ,
	@new_product_id int =null output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		BEGIN TRANSACTION [INSERT_Product_TRAN]
		BEGIN TRY 

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [issuer_product] WHERE [product_code] = @product_code AND issuer_id = @issuer_id) > 0
				BEGIN
					SET @new_product_id = 0
					SET @ResultCode = 221						
				END
			 ELSE
			IF (SELECT COUNT(*) FROM [issuer_product] WHERE [product_name] = @product_name) > 0
				BEGIN
					SET @new_product_id = 0
					SET @ResultCode = 220
				END
			ELSE IF (SELECT COUNT(*) FROM [issuer_product] WHERE [product_bin_code] = @product_bin_code) > 0
				BEGIN
					SET @new_product_id = 0
					SET @ResultCode = 222
				END
			ELSE			
			BEGIN

				INSERT INTO [dbo].[issuer_product]
					   ([product_code], [product_name], [product_bin_code], [issuer_id],
						[name_on_card_top], [name_on_card_left], [Name_on_card_font_size], [font_id],DeletedYN) 
				VALUES (@product_code, @product_name, @product_bin_code, @issuer_id,
						@name_on_card_top, @name_on_card_left, @Name_on_card_font_size, @font_id, 0)
			
				SET @new_product_id= SCOPE_IDENTITY()
				DECLARE @RC int
				EXECUTE @RC = [sp_insert_product_currency] @new_product_id, @currencylist, @audit_user_id, @audit_workstation
				--DECLARE @audit_description varchar(500)
				--SELECT @audit_description = 'Product Created: ' + @product_code  + ', Product Name=' + @product_name + 
																     --', bin code=' + @product_bin_code 
																	
				--EXEC sp_insert_audit @audit_user_id, 
				--					 0,
				--					 NULL, 
				--					 @audit_workstation, 
				--					 @audit_description, 
				--					 NULL, NULL, NULL, NULL

				SET @ResultCode = 0				
			END

			COMMIT TRANSACTION [INSERT_Product_TRAN]

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_Product_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_insert_user]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 20 March 2014
-- Description:	Insert a user
-- =============================================
CREATE PROCEDURE [dbo].[sp_insert_user] 
	@user_status_id int,
    @username varchar(50),
    @first_name varchar(50),
    @last_name varchar(50),
    @password varchar(50),
	@user_email varchar(100),
    @online bit = 0,
    @employee_id varchar(50),
    @last_login_date datetime = null,
    @last_login_attempt datetime = null,
    @number_of_incorrect_logins int = 0,
    @last_password_changed_date datetime = null,
    @workstation nchar(50) = '',
	@user_group_list AS user_group_id_array READONLY,	 
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ldap_setting_id int = null,
	@language_id int = null,
	@new_user_id bigint OUTPUT,
	@ResultCode int OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [INSERT_USER_TRAN]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			DECLARE @objid int
			SET @objid = object_id('user')

			--Check for duplicate username
			DECLARE @dup_check int
			SELECT @dup_check = COUNT(*) 
			FROM [user]
			WHERE CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) = @username

			IF @dup_check > 0
				BEGIN
					SET @new_user_id = 0;
					SELECT @ResultCode = 69							
				END
			ELSE
			BEGIN
					INSERT INTO [user]
						([user_status_id],[user_gender_id],[username],[first_name],[last_name],[password],[user_email],[online]
						 ,[employee_id],[last_login_date],[last_login_attempt],[number_of_incorrect_logins]
						  ,[last_password_changed_date],[workstation], [ldap_setting_id], [language_id], [username_index])
					VALUES
						   (@user_status_id,
							1,
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@username)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@first_name)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@last_name)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@password)),
							@user_email,
							@online,
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@employee_id)),
							@last_login_date,
							@last_login_attempt,
							@number_of_incorrect_logins,
							@last_password_changed_date,
							@workstation,
							@ldap_setting_id,
							@language_id,
							[dbo].[MAC] (@username, @objid))

					SET @new_user_id = SCOPE_IDENTITY();
			

				--Link user to user groups
				INSERT INTO [users_to_users_groups]
							([user_id], [user_group_id])
				SELECT @new_user_id, ugl.user_group_id
				FROM @user_group_list ugl

				
				
				--log the audit record
				DECLARE @audit_description varchar(max), 
				        @groupname varchar(max),
						@user_status_name varchar(50)

				SELECT @user_status_name = user_status_text
				FROM [user_status]
				WHERE user_status_id = @user_status_id				

				SELECT @groupname = STUFF(
									(SELECT ', ' + user_group_name + 
											';' + CAST([user_group].[user_group_id] as varchar(max)) +
											';' + CAST([user_group].issuer_id as varchar(max))
									 FROM [user_group]
											INNER JOIN [users_to_users_groups]
												ON [user_group].user_group_id = [users_to_users_groups].user_group_id
										WHERE [users_to_users_groups].[user_id] = @new_user_id
										FOR XML PATH(''))
								   , 1
								   , 1
								   , '')

				--SELECT @groupname = SUBSTRING(
				--	(
				--	select ';'+ u.[user_group_name] + '-' +cast(u.[user_group_id] as varchar(500))+'- issu:'
				--	+cast(u.issuer_id as varchar(500)) from [dbo].[user_group] u
				--	inner join [users_to_users_groups] ug on u.[user_group_id]=ug.[user_group_id]
				--	where ug.[user_id]=@new_user_id
				--	FOR XML PATH('')),2,2000)
				--	if( @groupname is null)
				--	set @groupname=''
				
				--set @audit_description = 'Created: ' +@username + ', issu:'	+CAST(@login_issuer_id as varchar(100))+', groups:'+@groupname
				set @audit_description = 'Created: id: ' + CAST(@new_user_id as varchar(max)) + 
										 ', user: ' + @username + 
										 ', status: ' + COALESCE(@user_status_name, 'UNKNOWN') +
										 ', login ldap: ' + CAST(COALESCE(@ldap_setting_id, 0) as varchar(max)) + 
										 ', groups: ' + COALESCE(@groupname, 'none-selected')
				EXEC sp_insert_audit @audit_user_id, 
									 7,---UserAdmin
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL

				SELECT @ResultCode = 0	
								
			END

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

			COMMIT TRANSACTION [INSERT_USER_TRAN]

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_USER_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_insert_user_group_branches]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Insert link between users group and branches
-- =============================================
CREATE PROCEDURE [dbo].[sp_insert_user_group_branches] 
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
			--EXEC sp_insert_audit @audit_user_id, 
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




GO
/****** Object:  StoredProcedure [dbo].[sp_issue_card_cms_edit_fail]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_issue_card_cms_edit_fail] 
	@card_id bigint,
	@error varchar(5000),	 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION [ISSUE_CARD_CMS_EDIT_FAIL_TRAN]
		BEGIN TRY 

			DECLARE @branch_card_code_id int,
					@current_card_status_id int,
					@status_date datetime

			--try find the error in response mapping as to link to code.
			SELECT TOP 1 @branch_card_code_id = branch_card_code_id
			FROM [mod_response_mapping]
			WHERE @error LIKE '%' + response_contains + '%'

			--Check if a valid code was found for error, if not set as UNKNOWN Error
			IF @branch_card_code_id IS NULL
				SET @branch_card_code_id = 7

			--Update the cards status.
			INSERT branch_card_status
					(card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id, 
					 branch_card_code_id, comments)
			VALUES (@card_id, 9, @status_date, @audit_user_id, @audit_user_id, @branch_card_code_id, @error) 


			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			DECLARE @cardnumber varchar(50),
					@branch_card_status_name varchar(50),
					@audit_msg varchar(max)

			SELECT @cardnumber = CONVERT(VARCHAR,DECRYPTBYKEY(cards.card_number)) 
			FROM cards 
			WHERE cards.card_id = @card_id

			SELECT @branch_card_status_name = branch_card_statuses_name
			FROM [branch_card_statuses]
			WHERE branch_card_statuses_id = 9

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

			SET @audit_msg = '' + COALESCE(@branch_card_status_name, 'UNKNOWN') + 
								', ' + dbo.MaskString(@cardnumber, 6, 4) +
								', ' + @error

			--log the audit record		
			EXEC sp_insert_audit @audit_user_id, 
									3,
									NULL, 
									@audit_workstation, 
									@audit_msg, 
									NULL, NULL, NULL, NULL

			

			COMMIT TRANSACTION [ISSUE_CARD_CMS_EDIT_FAIL_TRAN]

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_CMS_EDIT_FAIL_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_issue_card_cms_fail]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_issue_card_cms_fail] 
	@card_id bigint,
	@error varchar(1000), 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION [ISSUE_CARD_CMS_FAIL_TRAN]
		BEGIN TRY 

			DECLARE @branch_card_code_id int,
					@current_card_status_id int,
					@status_date datetime

			--try find the error in response mapping as to link to code.
			--SELECT TOP 1 @branch_card_code_id = branch_card_code_id
			--FROM [mod_response_mapping]
			--WHERE @error LIKE '%' + response_contains + '%'

			--Check if a valid code was found for error, if not set as UNKNOWN Error
			IF @branch_card_code_id IS NULL
				SET @branch_card_code_id = 7

			SET @status_date = GETDATE()

			--Check what status the card was last in
			--SELECT @current_card_status_id = branch_card_statuses_id
			--FROM branch_card_status_current
			--WHERE card_id = @card_id

			--Update the cards status.
			INSERT branch_card_status
					(card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id, 
					 branch_card_code_id, comments)
			VALUES (@card_id, 9, @status_date, @audit_user_id, @audit_user_id, @branch_card_code_id, @error) 


			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			DECLARE @cardnumber varchar(50),
					@branch_card_status_name varchar(50),
					@audit_msg varchar(max)

			SELECT @cardnumber = CONVERT(VARCHAR,DECRYPTBYKEY(cards.card_number)) 
			FROM cards 
			WHERE cards.card_id = @card_id

			SELECT @branch_card_status_name = branch_card_statuses_name
			FROM [branch_card_statuses]
			WHERE branch_card_statuses_id = 9

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

			SET @audit_msg = '' + COALESCE(@branch_card_status_name, 'UNKNOWN') + 
							 ', ' + dbo.MaskString(@cardnumber, 6, 4) +
							 ', ' + @error
			--log the audit record		
			EXEC sp_insert_audit @audit_user_id, 
									3,
									NULL, 
									@audit_workstation, 
									@audit_msg, 
									NULL, NULL, NULL, NULL

		

			COMMIT TRANSACTION [ISSUE_CARD_CMS_FAIL_TRAN]

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_CMS_FAIL_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_issue_card_cms_relink_fail]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_issue_card_cms_relink_fail] 
	@card_id bigint,	
	@error varchar(1000), 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION [ISSUE_CARD_CMS_RELINK_FAIL_TRAN]
		BEGIN TRY 

			DECLARE @branch_card_code_id int,
					@current_card_status_id int,
					@status_date datetime

			--try find the error in response mapping as to link to code.
			SELECT TOP 1 @branch_card_code_id = branch_card_code_id
			FROM [mod_response_mapping]
			WHERE @error LIKE '%' + response_contains + '%'

			--Check if a valid code was found for error, if not set as UNKNOWN Error
			IF @branch_card_code_id IS NULL
				SET @branch_card_code_id = 7

			--Update the cards status.
			INSERT branch_card_status
					(card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id, 
					 branch_card_code_id, comments)
			VALUES (@card_id, 9, @status_date, @audit_user_id, @audit_user_id, @branch_card_code_id, @error) 


			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			DECLARE @cardnumber varchar(50),
					@branch_card_status_name varchar(50),
					@audit_msg varchar(max)

			SELECT @cardnumber = CONVERT(VARCHAR,DECRYPTBYKEY(cards.card_number)) 
			FROM cards 
			WHERE cards.card_id = @card_id

			SELECT @branch_card_status_name = branch_card_statuses_name
			FROM [branch_card_statuses]
			WHERE branch_card_statuses_id = 9

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

			SET @audit_msg = '' + COALESCE(@branch_card_status_name, 'UNKNOWN') + 
							 ', ' + dbo.MaskString(@cardnumber, 6, 4)+
							 ', ' + @error
								
			--log the audit record		
			EXEC sp_insert_audit @audit_user_id, 
									3,
									NULL, 
									@audit_workstation, 
									@audit_msg, 
									NULL, NULL, NULL, NULL

			

			COMMIT TRANSACTION [ISSUE_CARD_CMS_RELINK_FAIL_TRAN]

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_CMS_RELINK_FAIL_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_issue_card_cms_success]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_issue_card_cms_success] 
	-- Add the parameters for the stored procedure here
	@card_id bigint, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    	BEGIN TRANSACTION [ISSUE_CARD_CMS_SUCCESS_TRAN]
		BEGIN TRY 

			DECLARE @status_date datetime
			SET @status_date = GETDATE()

			--Update the cards status.
			INSERT branch_card_status
					(card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id, 
					 branch_card_code_id)
			VALUES (@card_id, 6, @status_date, @audit_user_id, @audit_user_id, 4 ) 


			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			DECLARE @cardnumber varchar(50),
					@branch_card_status_name varchar(50),
					@audit_msg varchar(max)

			SELECT @cardnumber = CONVERT(VARCHAR,DECRYPTBYKEY(cards.card_number)) 
			FROM cards 
			WHERE cards.card_id = @card_id

			SELECT @branch_card_status_name = branch_card_statuses_name
			FROM [branch_card_statuses]
			WHERE branch_card_statuses_id = 6

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

			SET @audit_msg = '' + COALESCE(@branch_card_status_name, 'UNKNOWN') + 
								', ' + dbo.MaskString(@cardnumber, 6, 4)
			--log the audit record		
			EXEC sp_insert_audit @audit_user_id, 
									3,
									NULL, 
									@audit_workstation, 
									@audit_msg, 
									NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [ISSUE_CARD_CMS_SUCCESS_TRAN]

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_CMS_SUCCESS_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_issue_card_complete]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_issue_card_complete] 
	@card_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [ISSUE_CARD_COMPLETE_TRAN]
		BEGIN TRY 
			
			DECLARE @current_card_status_id int,
					@status_date datetime,
					@operator_user_id bigint
					

			--get the current status and operator for the card
			SELECT @current_card_status_id = branch_card_statuses_id, @operator_user_id = operator_user_id
			FROM branch_card_status_current
			WHERE card_id = @card_id
										  
			--Check that someone hasn't already updated the card
			IF(@current_card_status_id = 8 OR @current_card_status_id = 9)				
				BEGIN

					SET @status_date = GETDATE()

					--Update the cards status.
					INSERT branch_card_status
							(card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id)
					VALUES (@card_id, 6, @status_date, @audit_user_id, @operator_user_id) 

					OPEN SYMMETRIC KEY Indigo_Symmetric_Key
					DECRYPTION BY CERTIFICATE Indigo_Certificate

					DECLARE @cardnumber varchar(50),
							@branch_card_status_name varchar(50),
							@audit_msg varchar(max)

					SELECT @cardnumber = CONVERT(VARCHAR,DECRYPTBYKEY(cards.card_number)) 
					FROM cards 
					WHERE cards.card_id = @card_id

					SELECT @branch_card_status_name = branch_card_statuses_name
					FROM [branch_card_statuses]
					WHERE branch_card_statuses_id = 6

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

					SET @audit_msg = '' + COALESCE(@branch_card_status_name, 'UNKNOWN') + 
									 ', ' + dbo.MaskString(@cardnumber, 6, 4)
					--log the audit record		
					EXEC sp_insert_audit @audit_user_id, 
										 3,
										 NULL, 
										 @audit_workstation, 
										 @audit_msg, 
										 NULL, NULL, NULL, NULL

					SET @ResultCode = 0					
				END
			ELSE
				BEGIN
					SET @ResultCode = 100
				END
			

				COMMIT TRANSACTION [ISSUE_CARD_COMPLETE_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_COMPLETE_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_issue_card_PIN_captured]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_issue_card_PIN_captured] 
	@card_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [ISSUE_CARD_PIN_CAPTURED_TRAN]
		BEGIN TRY 
			
			DECLARE @current_card_status_id int,
					@status_date datetime
					

			--get the current status for the card
			SELECT @current_card_status_id = branch_card_statuses_id
			FROM branch_card_status_current
			WHERE card_id = @card_id
										  
			--Check that someone hasn't already updated the card
			IF(@current_card_status_id = 4)				
				BEGIN

					--TODO: Need to update card table the pin

					SET @status_date = GETDATE()

					--Update the cards status.
					INSERT branch_card_status
							(card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id)
					VALUES (@card_id, 5, @status_date, @audit_user_id, null) 

					OPEN SYMMETRIC KEY Indigo_Symmetric_Key
					DECRYPTION BY CERTIFICATE Indigo_Certificate

					DECLARE @cardnumber varchar(50),
							@branch_card_status_name varchar(50),
							@audit_msg varchar(max)

					SELECT @cardnumber = CONVERT(VARCHAR,DECRYPTBYKEY(cards.card_number)) 
					FROM cards 
					WHERE cards.card_id = @card_id

					SELECT @branch_card_status_name = branch_card_statuses_name
					FROM [branch_card_statuses]
					WHERE branch_card_statuses_id = 5

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

					SET @audit_msg = '' + COALESCE(@branch_card_status_name, 'UNKNOWN') + 
									 ', ' + dbo.MaskString(@cardnumber, 6, 4)
					--log the audit record		
					EXEC sp_insert_audit @audit_user_id, 
										 3,
										 NULL, 
										 @audit_workstation, 
										 @audit_msg, 
										 NULL, NULL, NULL, NULL

					SET @ResultCode = 0					
				END
			ELSE
				BEGIN
					SET @ResultCode = 100
				END
			

				COMMIT TRANSACTION [ISSUE_CARD_PIN_CAPTURED_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_PIN_CAPTURED_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_issue_card_print_error]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_issue_card_print_error] 
	@card_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [ISSUE_CARD_PRINT_ERROR_TRAN]
		BEGIN TRY 
			
			DECLARE @current_card_status_id int,
					@status_date datetime
					

			--get the current status for the card
			SELECT @current_card_status_id = branch_card_statuses_id
			FROM branch_card_status_current
			WHERE card_id = @card_id
										  
			--Check that someone hasn't already updated the card
			IF(@current_card_status_id != 3)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN

					SET @status_date = GETDATE()

					--Update the cards status.
					INSERT branch_card_status
							(card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id)
					VALUES (@card_id, 8, @status_date, @audit_user_id, @audit_user_id) 


					OPEN SYMMETRIC KEY Indigo_Symmetric_Key
					DECRYPTION BY CERTIFICATE Indigo_Certificate

					DECLARE @cardnumber varchar(50),
							@branch_card_status_name varchar(50),
							@audit_msg varchar(max)

					SELECT @cardnumber = CONVERT(VARCHAR,DECRYPTBYKEY(cards.card_number)) 
					FROM cards 
					WHERE cards.card_id = @card_id

					SELECT @branch_card_status_name = branch_card_statuses_name
					FROM [branch_card_statuses]
					WHERE branch_card_statuses_id = 8

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

					SET @audit_msg = '' + COALESCE(@branch_card_status_name, 'UNKNOWN') + 
									 ', ' + dbo.MaskString(@cardnumber, 6, 4)
					--log the audit record		
					EXEC sp_insert_audit @audit_user_id, 
										 3,
										 NULL, 
										 @audit_workstation, 
										 @audit_msg, 
										 NULL, NULL, NULL, NULL

					SET @ResultCode = 0					
				END

				COMMIT TRANSACTION [ISSUE_CARD_PRINT_ERROR_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_PRINT_ERROR_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_issue_card_printed]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_issue_card_printed] 
	@card_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [ISSUE_CARD_PRINTED_TRAN]
		BEGIN TRY 
			
			DECLARE @current_card_status_id int,
					@status_date datetime
					

			--get the current status for the card
			SELECT @current_card_status_id = branch_card_statuses_id
			FROM branch_card_status_current
			WHERE card_id = @card_id
										  
			--Check that someone hasn't already updated the card
			IF(@current_card_status_id != 3)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN

					SET @status_date = GETDATE()

					--Update the cards status.
					INSERT branch_card_status
							(card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id, branch_card_code_id)
					VALUES (@card_id, 4, @status_date, @audit_user_id, @audit_user_id, 0) 


					OPEN SYMMETRIC KEY Indigo_Symmetric_Key
					DECRYPTION BY CERTIFICATE Indigo_Certificate

					DECLARE @cardnumber varchar(50),
							@branch_card_status_name varchar(50),
							@audit_msg varchar(max)

					SELECT @cardnumber = CONVERT(VARCHAR,DECRYPTBYKEY(cards.card_number)) 
					FROM cards 
					WHERE cards.card_id = @card_id

					SELECT @branch_card_status_name = branch_card_statuses_name
					FROM [branch_card_statuses]
					WHERE branch_card_statuses_id = 4

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

					SET @audit_msg = '' + COALESCE(@branch_card_status_name, 'UNKNOWN') + 
									 ', ' + dbo.MaskString(@cardnumber, 6, 4)
					--log the audit record		
					EXEC sp_insert_audit @audit_user_id, 
										 3,
										 NULL, 
										 @audit_workstation, 
										 @audit_msg, 
										 NULL, NULL, NULL, NULL

					SET @ResultCode = 0					
				END

				COMMIT TRANSACTION [ISSUE_CARD_PRINTED_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_PRINTED_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_issue_card_spoil]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_issue_card_spoil] 
	@card_id bigint,
	@branch_card_code_id int,
	@spoil_comments varchar(1000),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [ISSUE_CARD_SPOIL_TRAN]
		BEGIN TRY 
			
			DECLARE @current_card_status_id int,
					@status_date datetime
					

			--get the current status for the card
			SELECT @current_card_status_id = branch_card_statuses_id
			FROM branch_card_status_current
			WHERE card_id = @card_id
										  
			--Check that someone hasn't already updated the card
			IF(@current_card_status_id = 8 OR @current_card_status_id = 9)
				BEGIN

					SET @status_date = GETDATE()

					--Update the cards status.
					INSERT branch_card_status
							(card_id, branch_card_statuses_id, status_date, [user_id],
							 branch_card_code_id, comments)
					VALUES (@card_id, 7, @status_date, @audit_user_id, @branch_card_code_id, @spoil_comments) 


					OPEN SYMMETRIC KEY Indigo_Symmetric_Key
					DECRYPTION BY CERTIFICATE Indigo_Certificate

					DECLARE @cardnumber varchar(50),
							@branch_card_status_name varchar(50),
							@audit_msg varchar(max)

					SELECT @cardnumber = CONVERT(VARCHAR,DECRYPTBYKEY(cards.card_number)) 
					FROM cards 
					WHERE cards.card_id = @card_id

					SELECT @branch_card_status_name = branch_card_statuses_name
					FROM [branch_card_statuses]
					WHERE branch_card_statuses_id = 7

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

					SET @audit_msg = '' + COALESCE(@branch_card_status_name, 'UNKNOWN') + 
										', ' + dbo.MaskString(@cardnumber, 6, 4)
					--log the audit record		
					EXEC sp_insert_audit @audit_user_id, 
											3,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					SET @ResultCode = 0					
				END
			ELSE
				BEGIN
					SET @ResultCode = 100
				END
			

				COMMIT TRANSACTION [ISSUE_CARD_SPOIL_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_SPOIL_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_issue_card_to_customer]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_issue_card_to_customer]
	@card_id bigint,	
    @customer_account_number varchar(27),
	@account_type_id int,
	@card_issue_reason_id int,
	@customer_first_name varchar(50),
	@customer_middle_name varchar(50),
	@customer_last_name varchar(50),
	@name_on_card varchar(30),
	@customer_title_id int,
	@cms_id varchar(50),
	@contract_number varchar(50),
	@currency_id int,
	@resident_id int,
	@customer_type_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [ISSUE_CARD_TO_CUST_TRAN]
		BEGIN TRY 

			IF @customer_middle_name IS NULL
				SET @customer_middle_name = ''
			
			DECLARE @current_card_status_id int,
					@status_date datetime

			--get the current status for the card
			SELECT @current_card_status_id = branch_card_statuses_id
			FROM branch_card_status_current
			WHERE card_id = @card_id
										  
			--Check that someone hasn't already updated the card
			IF(@current_card_status_id = 1 OR @current_card_status_id = 3)				
				BEGIN

					OPEN SYMMETRIC KEY Indigo_Symmetric_Key
					DECRYPTION BY CERTIFICATE Indigo_Certificate

					SET @status_date = GETDATE()

					--Update the cards status.
					INSERT branch_card_status
							(card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id)
					VALUES (@card_id, 2, @status_date, @audit_user_id, @audit_user_id) 

					--Save customer details
					INSERT customer_account
							([user_id], card_id, card_issue_reason_id, account_type_id, customer_account_number,
								customer_first_name, customer_middle_name, customer_last_name, name_on_card, customer_title_id, 
								date_issued, customer_type_id, currency_id, resident_id, cms_id, contract_number)
					VALUES (@audit_user_id, @card_id, @card_issue_reason_id, @account_type_id, 
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_account_number)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_first_name)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_middle_name)), 
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_last_name)), 
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@name_on_card)), 
							@customer_title_id, @status_date, @customer_type_id, @currency_id, @resident_id, @cms_id, @contract_number)					

					
					--Log audit stuff
					DECLARE @branchcardstatus  varchar(max),
							@Scenario  varchar(max),
							@audit_msg varchar(max),
							@cardnumber varchar(16)

					SELECT @cardnumber = CONVERT(VARCHAR,DECRYPTBYKEY(cards.card_number))
					FROM cards 
					WHERE cards.card_id = @card_id

					SELECT  @branchcardstatus =  branch_card_statuses.branch_card_statuses_name
					FROM    branch_card_statuses 
					WHERE	branch_card_statuses.branch_card_statuses_id = 2

					SELECT  @Scenario =  card_issue_reason.[card_issuer_reason_name]
					FROM	card_issue_reason 
					WHERE	card_issue_reason.[card_issue_reason_id] = @card_issue_reason_id

					SET @audit_msg =  COALESCE(@branchcardstatus, 'UNKNWON') +  
									  ', ' + dbo.MaskString(@cardnumber, 6, 4) + 
									  ', cust id:' + COALESCE(CAST(@cms_id as varchar(max)), 'n/a') +
									  ', a/c:' + dbo.MaskString(@customer_account_number, 3, 4) + 
									  ', ' + COALESCE(@Scenario, 'UNKNWON')

					--SET @audit_msg = 'Issued card ' + dbo.MaskString(@cardnumber, 6, 4) +
					--				 ' , acc:'+ dbo.MaskString(@customer_account_number, 3, 4) + 
					--				 ', typeid :'+CAST(@customer_account_type_id as nvarchar(50))

					--log the audit record		
					EXEC sp_insert_audit @audit_user_id, 
										 3,---IssueCard
										 NULL, 
										 @audit_workstation, 
										 @audit_msg, 
										 NULL, NULL, NULL, NULL

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
					SET @ResultCode = 0					
				END
			ELSE
				BEGIN
					SET @ResultCode = 100
				END
			
				
				COMMIT TRANSACTION [ISSUE_CARD_TO_CUST_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_TO_CUST_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_audit_actions]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_audit_actions] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT audit_action_id AS lookup_id, language_text
	FROM [audit_action_language]
	WHERE language_id = @language_id
END



GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_branch_card_statuses]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_branch_card_statuses] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT branch_card_statuses_id AS lookup_id, language_text
	FROM [branch_card_statuses_language]
	WHERE language_id = @language_id
END



GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_branch_statuses]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_branch_statuses] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT branch_status_id AS lookup_id, language_text
	FROM [branch_statuses_language]
	WHERE language_id = @language_id
	order by language_text
END



GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_card_issue_reasons]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_card_issue_reasons] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT card_issue_reason_id AS lookup_id, language_text
	FROM [card_issue_reason_language]
	WHERE language_id = @language_id
END



GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_customer_account_types]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_customer_account_types] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT account_type_id AS lookup_id, language_text
	FROM [customer_account_type_language]
	WHERE language_id = @language_id
END



GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_customer_residency]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_customer_residency] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT resident_id AS lookup_id, language_text
	FROM [customer_residency_language]
	WHERE language_id = @language_id
END


GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_customer_title]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_customer_title] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT customer_title_id AS lookup_id, language_text
	FROM [customer_title_language]
	WHERE language_id = @language_id
END


GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_customer_type]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_customer_type] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT customer_type_id AS lookup_id, language_text
	FROM [customer_type_language]
	WHERE language_id = @language_id
END


GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_dist_batch_statuses]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_dist_batch_statuses] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT dist_batch_statuses_id AS lookup_id, language_text
	FROM [dist_batch_statuses_language]
	WHERE language_id = @language_id
END



GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_dist_card_statuses]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_dist_card_statuses] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT dist_card_status_id AS lookup_id, language_text
	FROM [dist_card_statuses_language]
	WHERE language_id = @language_id
END



GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_file_statuses]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_file_statuses] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT file_status_id AS lookup_id, language_text
	FROM [file_statuses_language]
	WHERE language_id = @language_id
END



GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_interface_types]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_interface_types] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT interface_type_id AS lookup_id, language_text
	FROM [interface_type_language]
	WHERE language_id = @language_id
END



GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_issuer_statuses]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_issuer_statuses] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT issuer_status_id AS lookup_id, language_text
	FROM [issuer_statuses_language]
	WHERE language_id = @language_id
END



GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_load_batch_statuses]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_load_batch_statuses] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT load_batch_statuses_id AS lookup_id, language_text
	FROM [load_batch_statuses_language]
	WHERE language_id = @language_id
END



GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_load_card_statuses]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_load_card_statuses] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT load_card_status_id AS lookup_id, language_text
	FROM [load_card_statuses_language]
	WHERE language_id = @language_id
END



GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_user_roles]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_user_roles] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT user_role_id AS lookup_id, language_text
	FROM [user_roles_language]
	WHERE language_id = @language_id
END


GO
/****** Object:  StoredProcedure [dbo].[sp_lang_lookup_user_status]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[sp_lang_lookup_user_status] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT user_status_id AS lookup_id, language_text
	FROM [user_status_language]
	WHERE language_id = @language_id
END


GO
/****** Object:  StoredProcedure [dbo].[sp_list_branch_card_codes]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_list_branch_card_codes] 
	-- Add the parameters for the stored procedure here
	@branch_card_code_type_id int = NULL,
	@is_exception bit = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT *
	FROM [branch_card_codes]
	WHERE branch_card_code_type_id = COALESCE(@branch_card_code_type_id, branch_card_code_type_id)
		  AND is_exception = COALESCE(@is_exception, is_exception)
		  AND branch_card_code_enabled = 1
END



GO
/****** Object:  StoredProcedure [dbo].[sp_load_batch_approve]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Approve's the load batch. if issuer has auto create dist batch then create distbatch
-- =============================================
CREATE PROCEDURE [dbo].[sp_load_batch_approve] 
	@load_batch_id bigint,
	@status_notes varchar(150),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

	BEGIN TRANSACTION [APPROVE_LOAD_BATCH]
		BEGIN TRY 
			
			DECLARE @current_load_batch_status_id int

			SELECT @current_load_batch_status_id = load_batch_statuses_id
			FROM load_batch_status_current
			WHERE load_batch_id = @load_batch_id
										  
			--Check that someone hasn't already updated the load batch
			IF(@current_load_batch_status_id != 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN
					
					--Get a distinct list of branches from the load batch

					DECLARE @branch_id int,
							@cards_total int = 0,
							@dist_batch_id int,
							@audit_msg varchar(max)

					--Loop through all distinct branches for the load batch and create dist batches
					DECLARE branchId_cursor CURSOR FOR 
						SELECT DISTINCT [cards].branch_id
						FROM [cards] 
							INNER JOIN [load_batch_cards]
								ON [cards].card_id = [load_batch_cards].card_id
						WHERE [load_batch_cards].load_batch_id = @load_batch_id
						

					OPEN branchId_cursor

					FETCH NEXT FROM branchId_cursor 
					INTO @branch_id

					WHILE @@FETCH_STATUS = 0
					BEGIN
						DECLARE @number_of_dist_cards int = 0

						IF(SELECT [issuer].auto_create_dist_batch
							FROM [issuer] INNER JOIN [branch]
									ON [issuer].issuer_id = [branch].issuer_id
							WHERE [branch].branch_id = @branch_id) = 1
							BEGIN
								
								
								--create the distribution batch
								INSERT INTO [dist_batch]
									([branch_id], [no_cards],[date_created],[dist_batch_reference])
								VALUES (@branch_id, 0, GETDATE(), GETDATE())
								--SELECT
								--	@branch_id, 0, GETDATE(), GETDATE()
								--FROM [load_batch]
								--WHERE [load_batch].load_batch_id = @load_batch_id

								SET @dist_batch_id = SCOPE_IDENTITY();

								--Add cards to distribution batch
								INSERT INTO [dist_batch_cards]
									([dist_batch_id],[card_id],[dist_card_status_id])
								SELECT
									@dist_batch_id, [cards].card_id, 0
								FROM [cards] INNER JOIN [load_batch_cards]
									ON [cards].card_id = [load_batch_cards].card_id
								WHERE [load_batch_cards].load_batch_id = @load_batch_id
										AND [cards].branch_id = @branch_id
							
								--Get the number of cards inserted
								SELECT @number_of_dist_cards = @@ROWCOUNT										

								--add dist batch status of created
								INSERT INTO [dbo].[dist_batch_status]
									([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
								VALUES(@dist_batch_id, 0, @audit_user_id, GETDATE(), 'Auto Dist Batch Create')

								--Generate dist batch reference
								DECLARE @dist_batch_ref varchar(50)
								SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
														  [branch].branch_code + '' + 
														  CONVERT(VARCHAR(8), GETDATE(), 112) + '' +
														  CAST(@dist_batch_id AS varchar(max))
								FROM [branch] INNER JOIN [issuer]
									ON [branch].issuer_id = [issuer].issuer_id
								WHERE [branch].branch_id = @branch_id

								--UPDATE dist batch with reference and number of cards
								UPDATE [dist_batch]
								SET [dist_batch_reference] = @dist_batch_ref,
									[no_cards] = @number_of_dist_cards
								WHERE [dist_batch].dist_batch_id = @dist_batch_id

								--UPDATE the load batch cards status to allocated							
								UPDATE [load_batch_cards]
								SET [load_batch_cards].load_card_status_id = 2
								WHERE [load_batch_cards].card_id IN 
										(SELECT [dist_batch_cards].card_id
										 FROM [dist_batch_cards]
										 WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id)

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


								--auto add approve the distbatch
								INSERT INTO [dbo].[dist_batch_status]
									([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
								VALUES(@dist_batch_id, 1, @audit_user_id, GETDATE(), 'Auto Dist Batch Create Aproval')								

								SELECT @dist_batch_status_name =  dist_batch_status_name
								FROM dist_batch_statuses
								WHERE dist_batch_statuses_id = 1

								--Add audit for dist batch update								
								SET @audit_msg = 'Update: ' + CAST(@dist_batch_id AS varchar(max)) +
												 ', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
												 ', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
								--log the audit record		
								EXEC sp_insert_audit @audit_user_id, 
													 2,
													 NULL, 
													 @audit_workstation, 
													 @audit_msg, 
													 NULL, NULL, NULL, NULL

							END
						ELSE
							BEGIN
								--Update the cards linked to the load batch and cursors branch with the available status.
								UPDATE [load_batch_cards]
								SET load_card_status_id = 1
								FROM [load_batch_cards] INNER JOIN [cards]
									 ON [load_batch_cards].card_id = [cards].card_id
								WHERE [load_batch_cards].load_batch_id = @load_batch_id
										AND [cards].branch_id = @branch_id

								--Get the number of cards updated
								SELECT @number_of_dist_cards = @@ROWCOUNT
							END						

						SELECT @cards_total += @number_of_dist_cards

							-- Get the next branch.
						FETCH NEXT FROM branchId_cursor 
						INTO @branch_id
						END 
					CLOSE branchId_cursor;
					DEALLOCATE branchId_cursor;

					--Check that all cards for the load batch have been updated
					IF (SELECT COUNT(card_id) FROM [load_batch_cards] WHERE load_batch_id = @load_batch_id) != @cards_total
					BEGIN
						RAISERROR ('Not all cards have been approved.',
								    12,
								    12 );
					END

					--Update the load batch status.
					INSERT load_batch_status
							([load_batch_id], [load_batch_statuses_id], [user_id], [status_date], [status_notes])
					VALUES (@load_batch_id, 1, @audit_user_id, GETDATE(), @status_notes)
	
					--log the audit record for load batch
					DECLARE @load_batch_ref varchar(100),
							@load_batch_status varchar(50)

					SELECT @load_batch_ref = load_batch_reference
					FROM [load_batch]
					WHERE load_batch_id = @load_batch_id

					SELECT @load_batch_status = load_batch_status_name
					FROM [load_batch_statuses]
					WHERE load_batch_statuses_id = 1
						
					SET @audit_msg = 'Update: ' + CAST(@load_batch_id AS varchar(max)) + 
									 ', ' + COALESCE(@load_batch_ref, 'UNKNWON') +
									 ', ' + COALESCE(@load_batch_status, 'UNKNOWN')

					--log the audit record		
					EXEC sp_insert_audit @audit_user_id, 
										 5, --LoadBatch
										 NULL, 
										 @audit_workstation, 
										 @audit_msg, 
										 NULL, NULL, NULL, NULL

					SET @ResultCode = 0

					COMMIT TRANSACTION [APPROVE_LOAD_BATCH]
				END
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [APPROVE_LOAD_BATCH]
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
/****** Object:  StoredProcedure [dbo].[sp_load_batch_reject]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Reject the load batch
-- =============================================
CREATE PROCEDURE [dbo].[sp_load_batch_reject] 
	@load_batch_id bigint,
	@status_notes varchar(150),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [REJECT_LOAD_BATCH]
		BEGIN TRY 
			
			DECLARE @current_status int

			SELECT @current_status = load_batch_statuses_id
			FROM load_batch_status_current
			WHERE load_batch_id = @load_batch_id
										  
			--Check that someone hasn't already updated the load batch
			IF(@current_status <> 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN
					--Update the load batch status.
					INSERT load_batch_status
							([load_batch_id], [load_batch_statuses_id], [user_id], [status_date], [status_notes])
					VALUES (@load_batch_id, 2, @audit_user_id, GETDATE(), @status_notes)

					--Update the cards linked to the load batch with the new status.
					UPDATE load_batch_cards
					SET load_card_status_id = 3
					WHERE load_batch_id = @load_batch_id
	

					--log the audit record for load batch
					DECLARE @audit_msg varchar(max),
							@load_batch_ref varchar(100),
							@load_batch_status varchar(50)

					SELECT @load_batch_ref = load_batch_reference
					FROM [load_batch]
					WHERE load_batch_id = @load_batch_id

					SELECT @load_batch_status = load_batch_status_name
					FROM [load_batch_statuses]
					WHERE load_batch_statuses_id = 3
						
					SET @audit_msg = 'Update: ' + CAST(@load_batch_id AS varchar(max)) + 
									 ', ' + COALESCE(@load_batch_ref, 'UNKNWON') +
									 ', ' + COALESCE(@load_batch_status, 'UNKNOWN')

					--log the audit record		
					EXEC sp_insert_audit @audit_user_id, 
										 5,---LoadBatch
										 NULL, 
										 @audit_workstation, 
										 @audit_msg, 
										 NULL, NULL, NULL, NULL

					SET @ResultCode = 0

					COMMIT TRANSACTION [REJECT_LOAD_BATCH]
				END
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [REJECT_LOAD_BATCH]
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
/****** Object:  StoredProcedure [dbo].[sp_load_issuer_license]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Loads license information for issuer
-- =============================================
CREATE PROCEDURE [dbo].[sp_load_issuer_license] 
	@issuer_name varchar(50),
	@issuer_code varchar(10),
	@license_key varchar(1000),
	@xml_license_file varbinary,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    BEGIN TRANSACTION [LOAD_ISSUER_LICENSE_TRAN]
		BEGIN TRY 

		   DECLARE @issuer_id int

		   SELECT @issuer_id = issuer_id
		   FROM [issuer]
		   WHERE issuer_name = @issuer_name AND
				 issuer_code = @issuer_code
			
			IF(@issuer_id > 0)
				BEGIN
					UPDATE [issuer]
					SET license_key = @license_key,
						license_file = ''
					WHERE issuer_name = @issuer_name AND
							issuer_code = @issuer_code

					--log the audit record
					--DECLARE @audit_description varchar(500)
					--SELECT @audit_description = 'Load issuer license information (issuer_id = ' + CONVERT(NVARCHAR, @issuer_id)	+ ')'			
					--EXEC sp_insert_audit @audit_user_id, 
					--					 2,
					--					 NULL, 
					--					 @audit_workstation, 
					--					 @audit_description, 
					--					 NULL, NULL, NULL, NULL

					SET @ResultCode = 0
				END
			ELSE
				BEGIN
					SET @ResultCode = 71
				END

			COMMIT TRANSACTION [LOAD_ISSUER_LICENSE_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [LOAD_ISSUER_LICENSE_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_lookup_response_message]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_lookup_response_message] 
	-- Add the parameters for the stored procedure here
	@system_response_code int,
	@system_area int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT english_response, french_response, portuguese_response, spanish_response
	FROM [response_messages]
	WHERE system_response_code = @system_response_code
		  AND system_area = @system_area

END



GO
/****** Object:  StoredProcedure [dbo].[sp_reset_user_password]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 26 March 2014
-- Description:	Resets a users password
-- =============================================
CREATE PROCEDURE [dbo].[sp_reset_user_password] 
	-- Add the parameters for the stored procedure here
	@password varchar(100),
	@user_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [RESET_USER_PASSWORD_TRAN]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate
				--Clear out the password history table with passwords older than 3 months
				DELETE FROM [user_password_history]
				WHERE [date_changed] < DATEADD(MONTH, -3, GETDATE())
				
				--Move the current password into the history table
				INSERT INTO [user_password_history] ([user_id], [password_history], [date_changed])
				SELECT [user].[user_id], [user].[password], GETDATE()
				FROM [user]
				WHERE [user].[user_id] = @user_id				
				
				--Update current password with new password
				UPDATE [user]
				SET	[password] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@password)),
					[last_password_changed_date] = GETDATE(),
					[number_of_incorrect_logins] = 0
				WHERE [user].[user_id] = @user_id
				

				--log the audit record
				DECLARE @audit_description varchar(max),
				        @username varchar(100)

				SELECT  @username = CONVERT(VARCHAR,DECRYPTBYKEY([username]))
				FROM [user]
				WHERE [user_id] = @user_id

				IF (@user_id = @audit_user_id)
					BEGIN
						SELECT @audit_description = 'Change Password: ' + @username
					END
				ELSE
					BEGIN
						SELECT @audit_description = 'Reset Password: ' + @username
					END		

				
				EXEC sp_insert_audit @audit_user_id, 
									 7,---UserAdmin
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

			COMMIT TRANSACTION [RESET_USER_PASSWORD_TRAN]		
			
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [RESET_USER_PASSWORD_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_search_branch]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 6 March 2014
-- Description:	Search for branches based on inputs
-- =============================================
CREATE PROCEDURE [dbo].[sp_search_branch] 
		@branch_name varchar(30) = null,
		@branch_code varchar(10) = null,
		@branch_status_id int = null,
		@issuer_id int
		
			
	AS 
	BEGIN 
	IF (@branch_name='') OR (@branch_name='null')
	BEGIN
		SET @branch_name = NULL
	END
	
	IF (@branch_code='') OR (@branch_code='null')
	BEGIN
		SET @branch_code = NULL
	END
	
	
	
   SELECT * FROM branch 
   WHERE branch_name = COALESCE(@branch_name,branch_name)
    AND branch_code = COALESCE(@branch_code,branch_code)
	AND branch_status_id = COALESCE(@branch_status_id, branch_status_id)
    AND issuer_id = @issuer_id

END




GO
/****** Object:  StoredProcedure [dbo].[sp_search_branch_cards]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch cards at a branch based on search criterial
-- =============================================
CREATE PROCEDURE [dbo].[sp_search_branch_cards] 
	@branch_id int,
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




GO
/****** Object:  StoredProcedure [dbo].[sp_search_cards]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 28 March 2014
-- Description:	Search for card/s based on input parameters
-- =============================================
CREATE PROCEDURE [dbo].[sp_search_cards] 
	-- Add the parameters for the stored procedure here
	@user_id bigint,
	@user_role_id int = NULL,
	@issuer_id int = NULL,
	@branch_id int = NULL,
	@card_number varchar(30) = NULL, 
	@card_last_four_digits varchar(4) = NULL,	

	@batch_reference varchar(100) = NULL,

	@load_card_status_id int = NULL,
	@dist_card_status_id int = NULL,	
	@branch_card_statuses_id int = NULL,

	@account_number varchar(30) = NULL,	

	@date_from DATETIME = NULL,
	@date_to DATETIME = NULL,

	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [CARD_SEARCH_TRAN]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			IF @date_to IS NOT NULL
				SET @date_to = DATEADD(day, 1, @date_to)

			--THIS IS FOR QUICKER CARD LOOKUP
			DECLARE @objid int
			SET @objid = object_id('cards')
			
			DECLARE @StartRow INT, @EndRow INT;			
			
			SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
			SET @EndRow = @StartRow + @RowsPerPage - 1;

			--append#1
			WITH PAGE_ROWS
			AS
			(
			SELECT ROW_NUMBER() OVER(ORDER BY status_date DESC) AS ROW_NO
					, COUNT(*) OVER() AS TOTAL_ROWS			
					, *
			FROM( 				
				SELECT DISTINCT [cards].card_id, 
					   CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) AS card_number,
					   --[load_batch].load_date, 
					   --[load_batch].load_batch_id,
					   --[load_card_statuses].load_card_status, 
					  -- [dist_batch].date_created,					   
					  --[dist_batch].dist_batch_id,
					 --  [dist_card_statuses].dist_card_status_name,
					 --  [branch_card_statuses].branch_card_statuses_name,
					   
					   COALESCE([branch_card_statuses].branch_card_statuses_name, 
								[dist_card_statuses].dist_card_status_name,
								[load_card_statuses].load_card_status) as current_card_status,
					   COALESCE([branch_card_status_current].status_date, 
								[dist_batch_status_current].status_date,
								[load_batch_status_current].status_date) as status_date,
					   [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
					   [branch].branch_id, [branch].branch_name, [branch].branch_code
				FROM [cards]						
					INNER JOIN [load_batch_cards]
						ON [cards].card_id = [load_batch_cards].card_id
					INNER JOIN [load_batch]
						ON [load_batch].load_batch_id = [load_batch_cards].load_batch_id
					INNER JOIN [load_batch_status_current]
						ON [load_batch].load_batch_id = [load_batch_status_current].load_batch_id
					INNER JOIN [load_card_statuses]
						ON [load_card_statuses].load_card_status_id = [load_batch_cards].load_card_status_id
					--Dist batch joins
					LEFT OUTER JOIN [dist_batch_cards]
						ON [cards].card_id = [dist_batch_cards].card_id						
					LEFT OUTER JOIN [dist_batch]
						ON [dist_batch].dist_batch_id = [dist_batch_cards].dist_batch_id
					LEFT OUTER JOIN [dist_batch_status_current]
						ON [dist_batch].dist_batch_id = [dist_batch_status_current].dist_batch_id				
					LEFT OUTER JOIN [dist_card_statuses]
						ON [dist_card_statuses].dist_card_status_id = [dist_batch_cards].dist_card_status_id
					--branch card joins
					LEFT OUTER JOIN [branch_card_status_current]
						ON [branch_card_status_current].card_id = [cards].card_id						   
					LEFT OUTER JOIN [branch_card_statuses]
						ON [branch_card_status_current].branch_card_statuses_id = [branch_card_statuses].branch_card_statuses_id							
					--Filter out cards linked to branches the user doesnt have access to.
					INNER JOIN (SELECT DISTINCT branch_id								
								FROM [user_roles_branch] INNER JOIN [user_roles]
										ON [user_roles_branch].user_role_id = [user_roles].user_role_id		
								WHERE [user_roles_branch].[user_id] = @user_id
										AND [user_roles_branch].branch_id = COALESCE(@branch_id, [user_roles_branch].branch_id)
										AND [user_roles].user_role_id IN (1,2,3,4,5,7)--Only want roles that allowed to search cards
										AND [user_roles_branch].user_role_id = COALESCE(@user_role_id, [user_roles_branch].user_role_id)
								) as X
						ON [cards].branch_id = X.branch_id
					INNER JOIN [branch]
						ON [branch].branch_id = [cards].branch_id
					INNER JOIN [issuer]
						ON [issuer].issuer_id = [branch].issuer_id
				WHERE 				 
					   ([load_batch].load_batch_reference LIKE COALESCE(@batch_reference, [load_batch].load_batch_reference)
							OR
					       [dist_batch].dist_batch_reference LIKE COALESCE(@batch_reference, [dist_batch].dist_batch_reference))					  
					  AND [load_batch_cards].load_card_status_id = COALESCE(@load_card_status_id, [load_batch_cards].load_card_status_id)
					  AND [branch].branch_status_id = 0	 
					  AND [issuer].issuer_status_id = 0		
					  AND [issuer].issuer_id = COALESCE(@issuer_id, [issuer].issuer_id)		  
					  AND (([dist_batch].date_created BETWEEN COALESCE(@date_from, [dist_batch].date_created) AND COALESCE(@date_to, [dist_batch].date_created))
							OR
						   ([load_batch].load_date BETWEEN COALESCE(@date_from, [load_batch].load_date) AND COALESCE(@date_to, [load_batch].load_date))
							OR
						   ([branch_card_status_current].status_date BETWEEN COALESCE(@date_from, [branch_card_status_current].status_date) AND COALESCE(@date_to, [branch_card_status_current].status_date)))
					  AND ((@card_number IS NULL) OR (DECRYPTBYKEY([cards].card_number) LIKE @card_number))					  
					  AND ((@dist_card_status_id IS NULL) OR ([dist_batch_cards].dist_card_status_id = @dist_card_status_id))
					  AND ((@branch_card_statuses_id IS NULL) OR ([branch_card_status_current].branch_card_statuses_id = @branch_card_statuses_id))
					  AND ((@card_last_four_digits IS NULL) OR ([cards].[card_index] = [dbo].[MAC] (@card_last_four_digits, @objid)))

			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY status_date DESC

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

			--log the audit record		
			--EXEC sp_insert_audit @user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 'Getting cards for card search.', 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [CARD_SEARCH_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CARD_SEARCH_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_search_issuer]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_search_issuer]	
	@issuer_name varchar(25),
	@issuer_status varchar(20),
	@issuer_id int
AS
BEGIN
DECLARE @issuer_status_id varchar(20)

	IF((@issuer_name=null) OR (@issuer_name=''))
	BEGIN
		SET	@issuer_name =NULL
	END	

	SET @issuer_status_id = (SELECT issuer_status_id
							 FROM dbo.issuer_statuses
							 WHERE issuer_status_name = @issuer_status)
	
	IF((@issuer_status=null) OR (@issuer_status='')
		OR (@issuer_status='UNKNOWN'))
	BEGIN
	SET	@issuer_status =NULL
	END
	
	
	SELECT * FROM issuer
	WHERE issuer_id = COALESCE(@issuer_id,issuer_id) AND
		  issuer_name = COALESCE(@issuer_name,issuer_name) AND
		  issuer_status_id = COALESCE(@issuer_status_id, 4) 


END



GO
/****** Object:  StoredProcedure [dbo].[sp_search_issuer_by_id]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_search_issuer_by_id]
	@issuer_id int
AS
BEGIN
DECLARE @issuer_status_id varchar(20)

	SELECT * FROM issuer
	WHERE issuer_id = @issuer_id

END



GO
/****** Object:  StoredProcedure [dbo].[sp_search_user]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec sp_search_user null,null,null,-1,0,null,-1,'',1,20
CREATE PROCEDURE [dbo].[sp_search_user]
	@user_name varchar(50) =null,
	@first_name varchar(50) =null,
	@last_name varchar(50)=null,
	@issuer_id int =null,
	@branch_id varchar(10)=null,
	@user_role varchar(30)=null,
	@audit_user_id int =null,
	@audit_workstation varchar(100)=null,
	@PageIndex INT = 1,
@RowsPerPage INT = 20
AS
BEGIN

IF((@user_name=null) OR (@user_name=''))
	BEGIN
		SET	@user_name =NULL
	END

	IF((@first_name=null) OR (@first_name=''))
	BEGIN
		SET	@first_name =NULL
	END
	
	IF((@last_name=null) OR (@last_name=''))
	BEGIN
	SET	@last_name =NULL
	
	END
		
	IF((@branch_id=null) OR (@branch_id=0))
	BEGIN
	SET	@branch_id =NULL
	END
	
	IF((@issuer_id=null) OR (@issuer_id=0))
	BEGIN
	SET	@issuer_id =NULL
	END
	
	IF((@user_role=null) OR (@user_role=''))
	BEGIN
	SET	@user_role =NULL
	END
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY username ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(	

	
select distinct u.[user_id]						
							,CONVERT(VARCHAR,DECRYPTBYKEY(u.[username])) as 'username'
							,CONVERT(VARCHAR,DECRYPTBYKEY(u.[first_name])) as 'first_name'
							,CONVERT(VARCHAR,DECRYPTBYKEY(u.[last_name])) as 'last_name' 					
							,CONVERT(VARCHAR,DECRYPTBYKEY(u.[employee_id])) as 'empoyee_id'
							,us.[user_status_text] 
							,u.[online]    
							,u.[workstation],b.issuer_id
	
	from [user] u
inner join [dbo].[user_roles_branch] urb on urb.[user_id] =u.[user_id]
inner join branch b on b.branch_id= urb.branch_id
inner join user_status us on us.user_status_id=u.user_status_id
inner join user_roles ur on ur.user_role_id=urb.user_role_id
where 
ISNULL(b.issuer_id, '')=ISNULL(@issuer_id, ISNULL(b.issuer_id, '')) and
 ISNULL(b.branch_id, '')=ISNULL(@branch_id, ISNULL(b.branch_id, ''))  and
 ISNULL(ur.user_role, '')=ISNULL(@user_role, ISNULL(ur.user_role, ''))  and
 (ISNULL(CONVERT(VARCHAR,DECRYPTBYKEY(u.username)), '')=ISNULL(@user_name, ISNULL(CONVERT(VARCHAR,DECRYPTBYKEY(u.username)), '')) or  
 CONVERT(VARCHAR,DECRYPTBYKEY(u.username)) like '%'+@user_name+'%') and
 (ISNULL(CONVERT(VARCHAR,DECRYPTBYKEY(u.first_name)), '')=ISNULL(@first_name, ISNULL(CONVERT(VARCHAR,DECRYPTBYKEY(u.first_name)), '')) or
  CONVERT(VARCHAR,DECRYPTBYKEY(u.first_name)) like '%'+@first_name+'%') and
  (ISNULL(CONVERT(VARCHAR,DECRYPTBYKEY(u.last_name)), '')=ISNULL(@last_name, ISNULL(CONVERT(VARCHAR,DECRYPTBYKEY(u.last_name)), ''))
 or   CONVERT(VARCHAR,DECRYPTBYKEY(u.last_name)) like '%'+@last_name+'%'))

		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY username ASC

	--EXEC sp_insert_audit @audit_user_id, 
	--							1,
	--							NULL, 
	--							@audit_workstation, 
	--							'Getting unassigned users.', 
	--							NULL, NULL, NULL, NULL

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END






GO
/****** Object:  StoredProcedure [dbo].[sp_sys_decrypt]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_sys_decrypt] @cipheredText varbinary(max)
AS
BEGIN
OPEN SYMMETRIC KEY Indigo_Symmetric_Key
DECRYPTION BY CERTIFICATE Indigo_Certificate; 
--SELECT CONVERT(VARCHAR,DECRYPTBYKEY(''))
select CONVERT(varchar,DECRYPTBYKEY(@cipheredText))
CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END



GO
/****** Object:  StoredProcedure [dbo].[sp_sys_encrypt]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_sys_encrypt] @clearText varchar(max)
As
BEGIN
OPEN Symmetric Key Indigo_Symmetric_Key
DECRYPTION BY Certificate Indigo_Certificate;
SELECT CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@clearText)))
CLOSE Symmetric Key Indigo_Symmetric_Key;
END



GO
/****** Object:  StoredProcedure [dbo].[sp_update_branch]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Persist changes to the branch to the DB
-- =============================================
CREATE PROCEDURE [dbo].[sp_update_branch] 
	@branch_id int,
	@branch_status_id int,
	@issuer_id int,
	@branch_code varchar(10),
	@branch_name varchar(30),
	@location varchar(20),
	@contact_person varchar(30),
	@contact_email varchar(30),
	@card_centre varchar(10),	 
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	

	BEGIN TRANSACTION [UPDATE_BRANCH_TRAN]
		BEGIN TRY 

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [branch] WHERE ([branch_code] = @branch_code AND [issuer_id] = @issuer_id) AND branch_id != @branch_id) > 0
				BEGIN
					SET @ResultCode = 211						
				END
			ELSE IF (SELECT COUNT(*) FROM [branch] WHERE ([branch_name] = @branch_name AND [issuer_id] = @issuer_id) AND branch_id != @branch_id) > 0
				BEGIN
					SET @ResultCode = 210
				END
			ELSE
			BEGIN

				UPDATE [branch]
				SET [branch_status_id] = @branch_status_id,
					[issuer_id] = @issuer_id,
					[branch_code] = @branch_code,
					[branch_name] = @branch_name,
					[location] = @location,
					[contact_person] = @contact_person,
					[contact_email] = @contact_email,
					[card_centre] = @card_centre
				WHERE branch_id = @branch_id

				--log the audit record
				DECLARE @audit_description varchar(500),
						@branchstatus  varchar(50),
				        @issuer_code varchar(10)

				SELECT @issuer_code = issuer_code
				FROM issuer
				WHERE issuer_id = @issuer_id

				SELECT @branchstatus = branch_statuses.[branch_status]
				FROM branch_statuses 
				WHERE branch_statuses.branch_status_id = @branch_status_id

				SELECT @audit_description = 'Update: ID ' + CAST(@branch_id AS varchar(max)) + '; [' + CAST(@issuer_id as varchar(100)) + ',' + @issuer_code + ']; [' +
											@branch_code + ',' + @branch_name + ',' + @branchstatus + ']'

				EXEC sp_insert_audit @audit_user_id, 
									 0,--BranchAdmin
									 NULL,
									 @audit_workstation, 
									 @audit_description, 
									 @issuer_id, NULL, NULL, NULL

				SET @ResultCode  = 0				
			END

			COMMIT TRANSACTION [UPDATE_BRANCH_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_BRANCH_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_update_font]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_update_font]
@font_id int,
@font_name nvarchar(50),
@resource_path nvarchar(max),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
@ResultCode int =null OUTPUT 

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		BEGIN TRANSACTION [UPDATE_FONT_TRAN]
		BEGIN TRY 
    -- Insert statements for procedure here
			DECLARE @dup_check int
			SELECT @dup_check = COUNT(*) 
			FROM [dbo].[Issuer_product_font]  
			WHERE  font_name= @font_name AND font_id!=@font_id
			IF @dup_check > 0
				BEGIN
					SELECT @ResultCode = 69							
				END
				
			ELSE
			BEGIN
			
			UPDATE Issuer_product_font set font_name=@font_name,resource_path=@resource_path
			where font_id=@font_id


				--DECLARE @audit_description nvarchar(500)
				--SELECT @audit_description = 'Font updated: ' + @font_name  
																	
				--EXEC sp_insert_audit @audit_user_id, 
				--					 0,
				--					 NULL, 
				--					 @audit_workstation, 
				--					 @audit_description, 
				--					 NULL, NULL, NULL, NULL

				SELECT @ResultCode = 0	
				COMMIT TRANSACTION [UPDATE_FONT_TRAN]
				END
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_FONT_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_update_issuer]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo, Setenane
-- Create date: 2014/04/10
-- Description:	Updates a issuer' and returns its zero(0) if the update
--				was successful, else return the error message.				
-- =============================================

CREATE PROCEDURE [dbo].[sp_update_issuer]
	@issuer_id int,
	@issuer_status_id int,
	@country_id int,
	@issuer_name varchar(50),
	@issuer_code varchar(10),
	@auto_create_dist_batch bit,
	@instant_card_issue_YN bit,
	@pin_mailer_printing_YN bit,
	@delete_pin_file_YN bit,
	@delete_card_file_YN bit,
	@account_validation_YN bit,
	@maker_checker_YN bit,
	@cards_file_location varchar(100) = NULL,
	@card_file_type varchar(20) = NULL,
	@pin_file_location varchar(100) = NULL,
	@pin_encrypted_ZPK varchar(40) = NULL,
	@pin_mailer_file_type varchar(20) = NULL,
	@pin_printer_name varchar(50) = NULL,
	@pin_encrypted_PWK varchar(40) = NULL,
	@language_id int = NULL,
	@account_connection_id int = NULL,
	@corebanking_connection_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_ISSUER_TRAN]
		BEGIN TRY 

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [issuer] WHERE ([issuer].[issuer_name] = @issuer_name AND [issuer].issuer_id != @issuer_id)) > 0
				BEGIN					
					SET @ResultCode = 200						
				END
			ELSE IF (SELECT COUNT(*) FROM [issuer] WHERE ([issuer].[issuer_code] = @issuer_code AND [issuer].issuer_id != @issuer_id)) > 0
				BEGIN
					SET @ResultCode = 201
				END
			ELSE			
			BEGIN

				UPDATE [dbo].[issuer]
				SET [issuer_status_id] = @issuer_status_id,
					[country_id] = @country_id,
					[issuer_name] = @issuer_name,
					[issuer_code] = @issuer_code,
					[auto_create_dist_batch] = @auto_create_dist_batch,
					[instant_card_issue_YN] = @instant_card_issue_YN,
					[pin_mailer_printing_YN] = @pin_mailer_printing_YN,
					[delete_pin_file_YN] = @delete_pin_file_YN,
					[delete_card_file_YN] = @delete_card_file_YN,
					[account_validation_YN] = @account_validation_YN,
					[maker_checker_YN] = @maker_checker_YN,
					[cards_file_location] = @cards_file_location,
					[card_file_type] = @card_file_type,
					[pin_file_location] = @pin_file_location,
					[pin_encrypted_ZPK] = @pin_encrypted_ZPK,
					[pin_mailer_file_type] = @pin_mailer_file_type,
					[pin_printer_name] = @pin_printer_name,
					[pin_encrypted_PWK] = @pin_encrypted_PWK,
					[language_id] = @language_id
				WHERE [issuer_id] = @issuer_id

				DELETE FROM [issuer_interface]
				WHERE [issuer_id] = @issuer_id

				IF (@instant_card_issue_YN = 1)
				BEGIN
					IF (@account_validation_YN = 1)
					BEGIN
						--INSERT ACCOUNT INTERFACE
						INSERT INTO [issuer_interface]
							([issuer_id], [interface_type_id], [connection_parameter_id])
						VALUES (@issuer_id, 0, @account_connection_id)
					END

					--INSERT CORE BANKING INTERFACE
					INSERT INTO [issuer_interface]
						([issuer_id], [interface_type_id], [connection_parameter_id])
					VALUES (@issuer_id, 1, @corebanking_connection_id)
				END


				--log the audit record
				DECLARE @audit_description varchar(max) = '',
						@country_name varchar(100),
						@country_code varchar(50),
						@issuer_status varchar(50)

				SELECT @country_name = country_name, @country_code = country_code
				FROM [country]
				WHERE country_id = @country_id

				SELECT @issuer_status = issuer_status_name
				FROM [issuer_statuses]
				WHERE issuer_status_id = @issuer_status_id


				SET @audit_description = 'Update: id: ' + CAST(@issuer_id AS VARCHAR(max))	+ 
										 ', name: ' + COALESCE(@issuer_name, 'UNKNOWN') +
										 ', code: ' + COALESCE(@issuer_code, 'UNKNOWN') +
										 ', country: ' + COALESCE(@country_code, 'UNKNOWN') + ';' + COALESCE(@country_name, 'UNKNOWN') +
										 ', status: ' + COALESCE(@issuer_status, 'UNKNOWN')
										 	
				EXEC sp_insert_audit @audit_user_id, 
									 4,---IssuerAdmin
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @issuer_id, NULL, NULL, NULL

				SELECT @ResultCode = 0				
			END

			COMMIT TRANSACTION [UPDATE_ISSUER_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_ISSUER_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_update_ldap]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo Setenane
-- Create date: 2014/04/03
-- Description:	Updates an LDAP record
-- =============================================
CREATE PROCEDURE [dbo].[sp_update_ldap]
	@ldap_setting_id int,
	@ldap_setting_name varchar(100),
	@hostname_or_ip varchar(200),	
	@path varchar(200),
	@domain_name varchar(100) = NULL,
	@auth_username varchar(100) = NULL,
	@auth_password varchar(100) = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

		--Check for duplicate's
	IF (SELECT COUNT(*) FROM [ldap_setting] WHERE [ldap_setting_name] = @ldap_setting_name AND [ldap_setting_id] != @ldap_setting_id) > 0
		BEGIN
			SET @ResultCode = 225						
		END
	ELSE
	BEGIN 
		BEGIN TRANSACTION [UPDATE_LDAP_TRAN]
		BEGIN TRY 

			IF @auth_username IS NULL
				SET @auth_username = ''

			IF @auth_password IS NULL
				SET @auth_password = ''

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

				UPDATE [dbo].[ldap_setting]
				SET	[ldap_setting_name] = @ldap_setting_name,
					[hostname_or_ip] = @hostname_or_ip,
					[domain_name] = @domain_name,
					[path] = @path,
					[username] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@auth_username)),
					[password] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@auth_password))
				WHERE ldap_setting_id = @ldap_setting_id


			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

			--log the audit record
			--DECLARE @audit_description varchar(500)
			--SELECT @audit_description = 'Updated ldap ' + CONVERT(NVARCHAR, @ldap_setting_id)	+ ')'			
			--EXEC sp_insert_audit @audit_user_id, 
			--						2,
			--						NULL, 
			--						@audit_workstation, 
			--						@audit_description, 
			--						NULL, NULL, NULL, NULL	

			SET @ResultCode = 0

			COMMIT TRANSACTION [UPDATE_LDAP_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_LDAP_TRAN]
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




GO
/****** Object:  StoredProcedure [dbo].[sp_update_pin_batch_status]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_update_pin_batch_status] 

	@batch_reference varchar(50),
	@userName varchar(25),
	@new_batch_status varchar(25),
	@current_batch_status varchar(25),
	@new_pin_mailer_status varchar(25),
	@manager_comment varchar(100),
	@operator_comment varchar(100)
AS
BEGIN

	DECLARE	@actual_batch_status varchar(25)

	IF((@manager_comment=null) OR (@manager_comment=''))
	BEGIN
		SET @manager_comment=NULL
	END
	IF((@operator_comment=null) OR (@operator_comment=''))
	BEGIN
		SET @operator_comment=NULL
	END

	--validate that the batch is in the correct status
	SET @actual_batch_status = (SELECT batch_status FROM pin_batch 
	WHERE batch_reference = @batch_reference)

	IF @actual_batch_status <> @current_batch_status
	BEGIN
		RETURN 1 -- incorrect status
	END	

	--update batch status
	UPDATE pin_batch
	SET batch_status= @new_batch_status, manager_comment = @manager_comment, operator_comment = @operator_comment
	WHERE batch_reference = @batch_reference
		
	--add to batch status history
	INSERT INTO pin_batch_status 
	VALUES (@batch_reference,@new_batch_status,(GETDATE()),@userName)
			
	--update card status
	IF (@new_pin_mailer_status <> '')
	BEGIN
		UPDATE pin_mailer SET pin_mailer_status= @new_pin_mailer_status
		WHERE batch_reference = @batch_reference
	END		
	
	RETURN '0'
	
END



GO
/****** Object:  StoredProcedure [dbo].[sp_update_product]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_update_product]
	-- Add the parameters for the stored procedure here
	@product_id int,
	@product_name varchar(100),
	@product_code varchar(50),
	@product_bin_code varchar(9),
	@Issuer_id  int,
	@name_on_card_top decimal(8,2),
	@name_on_card_left decimal(8,2),
	@Name_on_card_font_size int,
	@font_id int =null,
	@currencylist as dbo.currency_id_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),	
	@ResultCode int =null OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	BEGIN TRANSACTION [UPDATE_Product_TRAN]
		BEGIN TRY 			

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [issuer_product] WHERE ([product_code] = @product_code AND [product_id] != @product_id AND issuer_id = @issuer_id)) > 0
				BEGIN
					SET @ResultCode = 221						
				END
			IF (SELECT COUNT(*) FROM [issuer_product] WHERE ([product_name] = @product_name AND [product_id] != @product_id)) > 0
				BEGIN
					SET @ResultCode = 220
				END
			ELSE IF (SELECT COUNT(*) FROM [issuer_product] WHERE ([product_bin_code] = @product_bin_code AND [product_id] != @product_id)) > 0
				BEGIN
					SET @ResultCode = 222
				END
			ELSE			
			BEGIN

				UPDATE [dbo].[issuer_product]
					SET [product_code] = @product_code,
						[product_name] = @product_name,
						[product_bin_code] = @product_bin_code,
						[issuer_id] = @issuer_id,
						[font_id] = @font_id,
						[name_on_card_top] = @name_on_card_top,
						[name_on_card_left] = @name_on_card_left,
						[Name_on_card_font_size] = @Name_on_card_font_size
					WHERE [product_id] = @product_id


					DELETE FROM product_currency
		WHERE [product_id] = @product_id

		Declare  @RC as int
		EXECUTE @RC = [sp_insert_product_currency] @product_id, @currencylist, @audit_user_id, @audit_workstation
				--DECLARE @audit_description varchar(500)
				--SELECT @audit_description = 'Product updated: ' + @product_code  + ', Product Name=' + @product_name + 
				--												     ', bin code=' + @product_bin_code 
																	
				--EXEC sp_insert_audit @audit_user_id, 
				--					 0,
				--					 NULL, 
				--					 @audit_workstation, 
				--					 @audit_description, 
				--					 NULL, NULL, NULL, NULL

				SELECT @ResultCode = 0				
			END

			COMMIT TRANSACTION [UPDATE_Product_TRAN]

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_Product_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_update_user]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 24 March 2014
-- Description:	Updates a user
-- =============================================
CREATE PROCEDURE [dbo].[sp_update_user] 
	-- Add the parameters for the stored procedure here
	@user_id bigint,
	@user_status_id int = null,
    @username varchar(50),
    @first_name varchar(50),
    @last_name varchar(50),
	@user_email varchar(100),
    @online bit = null,
    @employee_id varchar(50),
    @last_login_date datetime = null,
    @last_login_attempt datetime = null,
    @number_of_incorrect_logins int = null,
    @last_password_changed_date datetime = null,
    @workstation nchar(50)  = null,
	@user_group_list AS user_group_id_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ldap_setting_id int = null,
	@language_id int = null,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [UPDATE_USER_TRAN]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			--Check for duplicate username
			DECLARE @dup_check int, @new_user_id bigint
			SELECT @dup_check = COUNT(*) 
			FROM [user]
			WHERE CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) = @username
					AND [user_id] != @user_id

			IF @dup_check > 0
				BEGIN
					SELECT @ResultCode = 69							
				END
			ELSE
			BEGIN
				UPDATE [user]
				SET	[user_status_id] = COALESCE(@user_status_id, [user].[user_status_id])
					,[username] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@username))
					,[first_name] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@first_name))
					,[last_name] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@last_name))
					,[user_email] = @user_email
					,[online] = COALESCE(@online, [user].[online])
					,[employee_id] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@employee_id))
					,[last_login_date] = COALESCE(@last_login_date, [user].[last_login_date])
					,[last_login_attempt] = COALESCE(@last_login_attempt, [user].[last_login_attempt])
					,[number_of_incorrect_logins] = COALESCE(@number_of_incorrect_logins, [user].[number_of_incorrect_logins])
					,[last_password_changed_date] = COALESCE(@last_password_changed_date, [user].[last_password_changed_date])
					,[workstation] = COALESCE(@workstation, [user].[workstation])
					,[ldap_setting_id] = @ldap_setting_id
					,[language_id] = @language_id
				WHERE [user].[user_id] = @user_id

				

				--Remove links to user groups
				DELETE FROM [users_to_users_groups]
				WHERE [user_id] = @user_id

				--Link user to user groups
				INSERT INTO [users_to_users_groups]
							([user_id], [user_group_id])
				SELECT @user_id, ugl.user_group_id
				FROM @user_group_list ugl

				--log the audit record
				DECLARE @audit_description varchar(max), 
				        @groupname varchar(max),
						@user_status_name varchar(50)

				SELECT @user_status_name = user_status_text
				FROM [user_status] INNER JOIN [user]
						ON [user_status].user_status_id = [user].user_status_id
				WHERE [user].[user_id] = @user_id				

				SELECT @groupname = STUFF(
									(SELECT ', ' + user_group_name + 
											';' + CAST([user_group].[user_group_id] as varchar(max)) +
											';' + CAST([user_group].issuer_id as varchar(max))
									 FROM [user_group]
											INNER JOIN [users_to_users_groups]
												ON [user_group].user_group_id = [users_to_users_groups].user_group_id
										WHERE [users_to_users_groups].[user_id] = @user_id
										FOR XML PATH(''))
								   , 1
								   , 1
								   , '')
				
				--set @audit_description = 'Created: ' +@username + ', issu:'	+CAST(@login_issuer_id as varchar(100))+', groups:'+@groupname
				set @audit_description = 'Update: id: ' + CAST(@user_id as varchar(max)) + 
										 ', user: ' + @username + 
										 ', status: ' + COALESCE(@user_status_name, 'UNKNOWN') +
										 ', login ldap: ' + CAST(COALESCE(@ldap_setting_id, 0) as varchar(max)) + 
										 ', groups: ' + COALESCE(@groupname, 'none-selected')

				EXEC sp_insert_audit @audit_user_id, 
									 7,---UserAdmin
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL

				SELECT @ResultCode = 0		
						
			END

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
			COMMIT TRANSACTION [UPDATE_USER_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_USER_TRAN]
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
/****** Object:  StoredProcedure [dbo].[sp_update_user_group]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Updates user group
-- =============================================
CREATE PROCEDURE [dbo].[sp_update_user_group] 
	@user_group_id int,
	@user_group_name varchar(50),
	@user_role_id int,
	@can_read bit,
	@can_create bit,
	@can_update bit,
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

	

			--Check for duplicate's
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
			[can_delete] = 0,
            [all_branch_access] = @all_branch_access, 
			[user_group_name] = @user_group_name
		WHERE user_group_id = @user_group_id

		--Delete any linked branches so they may be inserted in the next step.
		DELETE FROM [user_groups_branches]
		WHERE user_group_id = @user_group_id

		--Link branches to user group
		EXECUTE @RC = [sp_insert_user_group_branches] @user_group_id, @branch_list, @audit_user_id, @audit_workstation
		

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

		EXEC sp_insert_audit @audit_user_id, 
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




GO
/****** Object:  StoredProcedure [dbo].[sp_update_user_language]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo Setenane
-- Create date: 22 April 2014
-- Description:	Updates a user's default language
-- =============================================
CREATE PROCEDURE [dbo].[sp_update_user_language] 
	-- Add the parameters for the stored procedure here
	@user_id bigint,
	@language_id int,
	@result_code int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [UPDATE_USER_LANG_TRAN]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			UPDATE [dbo].[user]
			SET [language_id] = @language_id
			WHERE [user_id] = @user_id

			SET @result_code = '0'

			COMMIT TRANSACTION [UPDATE_USER_LANG_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_USER_LANG_TRAN]
		SET @result_code = 'An Exception Occurred.'
	  RETURN ERROR_MESSAGE()
	END CATCH 	

	RETURN
END




GO
/****** Object:  StoredProcedure [dbo].[Update_incorrect_logIn]    Script Date: 2014/08/06 04:01:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_incorrect_logIn]
@username varchar(20),
@lockStatus varchar(15),
@unlockStatus varchar(15)
AS
BEGIN 

DECLARE @login_counts int
DECLARE @time_since int
DECLARE @currentDATE DATETIME
DECLARE @currentUserStatus varchar(15)

SET  @currentDATE = (SELECT GETDATE())


SELECT @login_counts= number_of_incorrect_logins ,
		@currentUserStatus =user_status,
		@time_since = DATEDIFF(MINUTE,last_login_attempt,@currentDATE)
FROM application_user WHERE username = @username

if(@time_since>=240)
	BEGIN
		UPDATE application_user 
		SET number_of_incorrect_logins = 1
		WHERE username = @username
		IF( @currentUserStatus = @lockStatus)
		BEGIN
			UPDATE application_user 
			SET user_status = @unlockStatus WHERE username = @username
		END
	END
ELSE
	BEGIN
		SET @login_counts = @login_counts +1
		IF((@login_counts>2) AND
		( @currentUserStatus = @unlockStatus))			
		BEGIN
			UPDATE application_user 
			SET user_status = @lockStatus,
			number_of_incorrect_logins = @login_counts
			WHERE username = @username
				
		END
	END
	
	UPDATE application_user 
	SET last_login_attempt = ( SELECT GETDATE())
	WHERE username = @username

END
GO

/* Lebo Changes */

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_issue_card_check_account_balance]
	@fee_charged decimal(10,4) = NULL,
	@accountbalance decimal(10,4) = NULL,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [ISSUE_CARD_CHECK_ACCOUNT_BALANCE]
		BEGIN TRY 
				IF (@accountbalance - @fee_charged > 0)
					 BEGIN
						SET @ResultCode = 1
					END
			ELSE
				BEGIN
					SET @ResultCode = 507
				END


				COMMIT TRANSACTION [ISSUE_CARD_CHECK_ACCOUNT_BALANCE]
		END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_CHECK_ACCOUNT_BALANCE]
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi
-- Create date: 20150108
-- Description:	User authorisation pin number
-- =============================================
CREATE PROCEDURE sp_update_instant_authorisation_pin
	@user_id bigint,
	@authorisation_pin_number varchar(100),
	@audit_user_id bigint,
	@audit_workstation varchar(100)

AS
BEGIN

	SET NOCOUNT ON;
	 BEGIN TRANSACTION [UPDATE_USER_AUTH_PIN_TRAN]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate
			
			UPDATE dbo.[user] 
			SET 
				[instant_authorisation_pin] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar, @authorisation_pin_number)),
				[last_authorisation_pin_changed_date] = GETDATE()
			WHERE [user].[user_id] = @user_id

				--log the audit record
				DECLARE @audit_description varchar(max),
				        @username varchar(100)

				SELECT  @username = CONVERT(VARCHAR,DECRYPTBYKEY([username]))
				FROM [user]
				WHERE [user_id] = @user_id

				IF (@user_id = @audit_user_id)
					BEGIN
						SELECT @audit_description = 'Change Authoriation Pin: ' + @username
					END
				ELSE
					BEGIN
						SELECT @audit_description = 'Change Authoriation Pin: ' + @username
					END		

				
				EXEC sp_insert_audit @audit_user_id, 
									 7,---UserAdmin
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

			COMMIT TRANSACTION [UPDATE_USER_AUTH_PIN_TRAN]		
				END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_USER_AUTH_PIN_TRAN]
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


/****** Object:  StoredProcedure [dbo].[sp_get_authpin_by_user_id]    Script Date: 2015/01/12 11:51:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Lebo Tladi
-- Create date:
-- Description:	Returns decrypted instant authorisation pin
-- =============================================
 CREATE PROCEDURE [dbo].[sp_get_authpin_by_user_id] 
	@user_id bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		--For the current password add 99 days to show it's the latest. old passwords should be in the past.
		SELECT CONVERT(VARCHAR,DECRYPTBYKEY([user].instant_authorisation_pin)) as 'Instant Auth Pin', DATEADD(DAY, 99, GETDATE()) as 'date'						
		FROM [user]
		WHERE [user].[user_id] = @user_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END


