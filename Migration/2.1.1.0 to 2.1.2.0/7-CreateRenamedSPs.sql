USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[AddMacForTable]    Script Date: 2016-04-05 09:55:42 AM ******/
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
/****** Object:  StoredProcedure [dbo].[GenerateAuditReport]    Script Date: 2016-04-05 09:55:42 AM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_activate_product]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_activate_product]
	@productid int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@result_code int =null output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	BEGIN TRANSACTION [ACTIVATE_PRODUCT_TRAN]
		BEGIN TRY 
			-- then disable the product
			UPDATE issuer_product set DeletedYN=0 where product_id=@productid

			-- then update the audit information
			DECLARE @audit_description varchar(max), @issuer_id int
			SELECT @audit_description = 'Product Activated: ' + CAST(@productid as varchar(max))
										+' , Product Id: ' + CAST(@productid as varchar(max))

			SELECT 	@issuer_id = issuer_id
			FROM [issuer_product]
			WHERE product_id = @productid
																	
			EXEC usp_insert_audit @audit_user_id, 
									4,
									NULL, 
									@audit_workstation, 
									@audit_description, 
									@issuer_id, NULL, NULL, NULL

			COMMIT TRANSACTION [ACTIVATE_PRODUCT_TRAN]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ACTIVATE_PRODUCT_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_branch_order_report]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_branch_order_report] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = NULL,
	@branch_id int = NULL,
	@date_from datetime,
	@date_to datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @date_to = DATEADD(d, 1, @date_to)

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT 
		DISTINCT CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_first_name)) AS customer_first_name
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_middle_name)) AS customer_middle_name
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_last_name)) AS customer_last_name
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_account_number)) AS customer_account_number
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].name_on_card)) AS name_on_card
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].contact_number)) AS contact_number
		, [dist_batch].dist_batch_reference
		, [dist_batch].date_created
		, [issuer].issuer_name
		, [issuer].issuer_code
		, [branch].branch_name
		, branch.branch_code
	FROM 
		[cards]
		INNER JOIN [customer_account]
			ON [cards].card_id = [customer_account].card_id
		INNER JOIN [dist_batch_cards]
			ON [cards].card_id = [dist_batch_cards].card_id
		INNER JOIN [dist_batch]
			ON [dist_batch_cards].dist_batch_id = [dist_batch].dist_batch_id
				AND [dist_batch].dist_batch_type_id = 0
				AND [dist_batch].date_created = ( SELECT MAX(db.date_created)
												  FROM [dist_batch] db
														INNER JOIN [dist_batch_cards] dbc
															ON db.dist_batch_id = dbc.dist_batch_id
												  WHERE dbc.card_id = [cards].card_id)
		INNER JOIN [branch]
			ON [cards].branch_id = [branch].branch_id
		INNER JOIN [issuer]
			ON [branch].issuer_id = [issuer].issuer_id	
	WHERE 
		[cards].card_issue_method_id = 0
		AND [issuer].issuer_id = COALESCE(@issuer_id, [issuer].issuer_id)
		AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
		AND [dist_batch].date_created >= @date_from
		AND [dist_batch].date_created <= @date_to
	ORDER BY 
		issuer_name
		, issuer_code
		, branch_name
		, branch_code
		, date_created
		, customer_account_number
		, customer_first_name
		, customer_last_name

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END





GO
/****** Object:  StoredProcedure [dbo].[usp_card_branch_spoil]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_card_branch_spoil] 
	@card_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	EXEC usp_issue_card_spoil @card_id, 
							 @audit_user_id,
							 @audit_workstation,
							 @ResultCode
END










GO
/****** Object:  StoredProcedure [dbo].[usp_card_dispatch_report]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_card_dispatch_report] 
	-- Add the parameters for the stored procedure here
	 @issuer_id int = NULL
	,@branch_id int = NULL
	,@date_from datetime
	,@date_to datetime
	,@audit_user_id bigint
	,@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @date_to = DATEADD(d, 1, @date_to)

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT 
		DISTINCT CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_first_name)) AS customer_first_name
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_middle_name)) AS customer_middle_name
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_last_name)) AS customer_last_name
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_account_number)) AS customer_account_number
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].name_on_card)) AS name_on_card
		, CASE 
			WHEN @mask_report = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
			ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
		  END AS 'card_number'
		, cards.card_request_reference AS card_reference_number
		, [dist_batch].dist_batch_reference, [dist_batch].date_created
		, [issuer].issuer_name
		, [issuer].issuer_code
		, [branch].branch_name
		, branch.branch_code
	FROM 
		[cards]
		INNER JOIN [customer_account]
			ON [cards].card_id = [customer_account].card_id
		INNER JOIN [dist_batch_cards]
			ON [cards].card_id = [dist_batch_cards].card_id
		INNER JOIN [dist_batch]
			ON [dist_batch_cards].dist_batch_id = [dist_batch].dist_batch_id
				AND [dist_batch].dist_batch_type_id = 0
		INNER JOIN [dist_batch_status_current]
			ON [dist_batch].dist_batch_id = [dist_batch_status_current].dist_batch_id
				AND [dist_batch_status_current].dist_batch_statuses_id = 2					
		INNER JOIN [branch]
			ON [cards].branch_id = [branch].branch_id
		INNER JOIN [issuer]
			ON [branch].issuer_id = [issuer].issuer_id	
	WHERE [cards].card_issue_method_id = 0
		AND [issuer].issuer_id = COALESCE(@issuer_id, [issuer].issuer_id)
		AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
		AND [dist_batch].date_created >= @date_from
		AND [dist_batch].date_created <= @date_to
	ORDER BY
		issuer_name
		, issuer_code
		, branch_name
		, branch_code
		, date_created
		, customer_account_number
		, customer_first_name
		, customer_last_name

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[usp_card_expiry_report]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_card_expiry_report] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = NULL,
	@branch_id int = NULL,
	@date_from datetime,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @date_from = DATEADD(M, 1, @date_from)

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT 
			DISTINCT  
			CASE 
				WHEN @mask_report = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
				ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
			END AS 'card_number'
			, cards.card_request_reference AS card_reference_number
			, CONVERT(DATETIME, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_expiry_date))) AS card_expiry_date
			, [issuer].issuer_name
			, [issuer].issuer_code
			, [branch].branch_name
			, branch.branch_code
		FROM [cards]								
				INNER JOIN [branch]
					ON [cards].branch_id = [branch].branch_id
				INNER JOIN [issuer]
					ON [branch].issuer_id = [issuer].issuer_id	
		WHERE [cards].card_issue_method_id = 0
				AND [issuer].issuer_id = COALESCE(@issuer_id, [issuer].issuer_id)
				AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
				AND DATEPART(m, CONVERT(DATETIME, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_expiry_date)))) = DATEPART(m, @date_from)
				AND DATEPART(yy, CONVERT(DATETIME, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_expiry_date)))) = DATEPART(yy, @date_from)
		ORDER BY issuer_name, issuer_code, branch_name, branch_code, card_expiry_date

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[usp_card_MakerChecker]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_card_MakerChecker] 
	@card_id bigint,
	@approve bit,
	@notes varchar(1000),
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
							@operator_username varchar(100),
							@card_issue_method_id int

					--Operator username needed for audit
					SELECT @operator_username = CONVERT(VARCHAR,DECRYPTBYKEY(username))
					FROM [branch_card_status_current]
							INNER JOIN [user]
								ON [branch_card_status_current].operator_user_id = [user].[user_id]
					WHERE [branch_card_status_current].card_id = @card_id

					--Get the issue method of the card
					SELECT @card_issue_method_id = card_issue_method_id
					FROM [cards]
					where card_id = @card_id
					

					SET @status_date = GETDATE()

					IF(@approve = 1)
						SET @new_status_id = 3
					ELSE
						BEGIN
							IF (@card_issue_method_id = 1)
								BEGIN
									SET @new_status_id = 1

									--Delete the customer information, it is not needed.
									DELETE FROM customer_account
									WHERE card_id = @card_id
								END
							ELSE
								SET @new_status_id = 11
						END

					--Update Branch Cards status with checked out cards
					INSERT INTO [branch_card_status]
									(card_id, operator_user_id, status_date, [user_id], branch_card_statuses_id, comments)
					SELECT @card_id, [branch_card_status_current].operator_user_id, @status_date, @audit_user_id, @new_status_id, @notes
					FROM [branch_card_status_current]
					WHERE [branch_card_status_current].card_id = @card_id


					--log the audit record
					DECLARE @audit_description varchar(max),
					        @branchcardstatus  varchar(50),
							@cardnumber varchar(50)	,
							@cardreferencenumber varchar(50)			

					SELECT  @branchcardstatus =  branch_card_statuses_name
					FROM    branch_card_statuses 
					WHERE	branch_card_statuses_id = @new_status_id

					
					 
					SELECT	@cardnumber = [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4)
						, @cardreferencenumber = [cards].card_request_reference
					FROM
						[cards] 				
						INNER JOIN [branch] ON [cards].branch_id = [branch].branch_id
						INNER JOIN [issuer] ON [branch].issuer_id = [issuer].issuer_id	
					WHERE	card_id = @card_id					

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

					IF(@approve = 0)
						BEGIN
							SET @audit_description = COALESCE(@branchcardstatus, 'UNKNOWN') + '(reject)' + 
													', ' + @cardnumber + 
													', ' + @cardreferencenumber +
													', to ' + COALESCE(@operator_username, 'UNKOWN')
						END
					ELSE
						BEGIN
							SET @audit_description = COALESCE(@branchcardstatus, 'UNKNOWN') +
													', ' + @cardnumber + ', ' + @cardreferencenumber
						END					
						
					EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_card_object_details]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch all cards linked to export batch with their details
-- =============================================
CREATE PROCEDURE [dbo].[usp_card_object_details] 
	@card_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)) AS 'card_number'
		, cards.card_request_reference AS card_reference_number
		, [cards].branch_id
		, [cards].card_id
		, [cards].card_issue_method_id
		, [cards].card_priority_id
		, [cards].card_request_reference
		, [cards].card_sequence
		, [cards].product_id
		, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_activation_date)), 109) as 'card_activation_date'
		, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_expiry_date)), 109) as 'card_expiry_date'
		, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_production_date)), 109) as 'card_production_date'						
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].pvv)) as 'pvv'
		--, [dist_batch].dist_batch_reference
		--, [dist_batch_cards].dist_batch_id
		--, [dist_batch_cards].dist_card_status_id
		--, [dist_card_statuses].dist_card_status_name
		, [cards].export_batch_id as dist_batch_id
		, 0 as dist_card_status_id
		, [customer_account].account_type_id
		, [customer_account].cms_id
		, [customer_account].contract_number
		, [customer_account].currency_id
		, [customer_account].customer_account_id
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_account_number)) as 'customer_account_number'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].name_on_card)) as 'name_on_card'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].Id_number)) as 'Id_number'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].contact_number)) as 'contact_number'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].CustomerId)) as 'CustomerId'
		, [customer_account].customer_title_id
		, [customer_account].customer_type_id
		, [customer_account].date_issued
		, [customer_account].resident_id
		, [customer_account].[user_id]
		, [issuer].issuer_id
		, [issuer].issuer_code
		, [issuer].issuer_name
		, [branch].branch_code
		, [branch].branch_name
		, [issuer_product].[product_code]
		, [issuer_product].[sub_product_code]
		, [issuer_product].[product_name]
		, [issuer_product].[product_bin_code]
		, [issuer_product].[src1_id]
		, [issuer_product].[src2_id]
		, [issuer_product].[src3_id]
		, CONVERT(INT, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVKI]))) as 'PVKI'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVK])) as 'PVK'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKA])) as 'CVKA'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKB])) as 'CVKB'
		, [issuer_product].[expiry_months]

	FROM 
		[cards]							
		INNER JOIN [branch]
			ON [cards].branch_id = [branch].branch_id
		INNER JOIN [issuer]
			ON [branch].issuer_id = [issuer].issuer_id
		INNER JOIN [issuer_product]
			ON [cards].product_id = [issuer_product].product_id
		LEFT OUTER JOIN [customer_account]
			ON [cards].card_id = [customer_account].card_id
	WHERE 
		[cards].card_id = @card_id
					
														   
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

END



GO
/****** Object:  StoredProcedure [dbo].[usp_card_production_report]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_card_production_report] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = NULL,
	@branch_id int = NULL,
	@date_from datetime,
	@date_to datetime,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @date_to = DATEADD(d, 1, @date_to)

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT DISTINCT
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_first_name)) AS customer_first_name
			, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_middle_name)) AS customer_middle_name
			, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_last_name)) AS customer_last_name
			, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_account_number)) AS customer_account_number
			, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].name_on_card)) AS name_on_card
			, CASE 
				WHEN @mask_report = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
				ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
			  END AS 'card_number'
			, cards.card_request_reference AS card_reference_number
			, [dist_batch].dist_batch_reference
			, [dist_batch].date_created
			, [issuer].issuer_name
			, [issuer].issuer_code
			, [branch].branch_name
			, branch.branch_code
		FROM [cards]
			INNER JOIN [customer_account]
				ON [cards].card_id = [customer_account].card_id
			INNER JOIN [dist_batch_cards]
				ON [cards].card_id = [dist_batch_cards].card_id
			INNER JOIN [dist_batch]
				ON [dist_batch_cards].dist_batch_id = [dist_batch].dist_batch_id
					AND [dist_batch].dist_batch_type_id = 0
			INNER JOIN [dist_batch_status]
				ON [dist_batch].dist_batch_id = [dist_batch_status].dist_batch_id
					AND [dist_batch_status].dist_batch_statuses_id = 13						
			INNER JOIN [branch]
				ON [cards].branch_id = [branch].branch_id
			INNER JOIN [issuer]
				ON [branch].issuer_id = [issuer].issuer_id	
		WHERE [cards].card_issue_method_id = 0
			AND [issuer].issuer_id = COALESCE(@issuer_id, [issuer].issuer_id)
			AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
			AND [dist_batch].date_created >= @date_from
			AND [dist_batch].date_created <= @date_to
		ORDER BY 
			issuer_name
			, issuer_code
			, branch_name
			, branch_code
			, date_created
			, customer_account_number
			, customer_first_name
			, customer_last_name

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[usp_card_pvv]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_card_pvv] 
	-- Add the parameters for the stored procedure here
	@card_id bigint,
	@pvv_lmk varchar(50),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [CARD_PVV]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			UPDATE [cards]
			SET pvv = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR(max), @pvv_lmk))
			WHERE card_id = @card_id

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

			COMMIT TRANSACTION [CARD_PVV]			
		END TRY

		BEGIN CATCH
			ROLLBACK TRANSACTION [CARD_PVV]
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
/****** Object:  StoredProcedure [dbo].[usp_cards_checkInOut]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Mark list of cards as checked in or out
-- =============================================
CREATE PROCEDURE [dbo].[usp_cards_checkInOut] 
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
				', ' + dbo.MaskString(CONVERT(VARCHAR(max),DECRYPTBYKEY(card_number)),6,4) +
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
					', ' + [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4)
						 +
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











GO
/****** Object:  StoredProcedure [dbo].[usp_cms_search_for_error]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_cms_search_for_error] 
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
/****** Object:  StoredProcedure [dbo].[usp_connection_parameter_create]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_connection_parameter_create] 
	@connection_name varchar(100),
	@connection_parameter_type_id int,
	@address varchar(200),
	@port int,
	@path varchar(200),
	@name_of_file varchar(100),
	@file_delete_YN bit,
	@file_encryption_type_id int,
	@duplicate_file_check_YN bit,
	@protocol int,
	@auth_type int,
	@header_length int = null,
	@identification varchar(100) = NULL,
	@timeout_milli int = NULL,
	@buffer_size int = NULL,
	@doc_type char = NULL,
	@auth_username VARCHAR(100),
	@auth_password VARCHAR(100),
	@private_key VARCHAR(MAX) = NULL,
	@public_key VARCHAR(MAX) = NULL,
	@domain_name varchar(100),
	@is_external_auth bit,
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

			IF @identification IS NULL
				SET @identification = ''

			IF @auth_username IS NULL
				SET @auth_username = ''

			IF @auth_password IS NULL
				SET @auth_password = ''

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

				INSERT INTO [dbo].[connection_parameters]
					   ([connection_name],[address],[port],[path],[protocol],[auth_type],[header_length],[identification],[username],[password],
						connection_parameter_type_id, timeout_milli, buffer_size, doc_type, name_of_file, file_delete_YN, file_encryption_type_id, duplicate_file_check_YN,
						private_key, public_key,domain_name,is_external_auth )
				VALUES
					   (@connection_name, @address, @port, @path, @protocol, @auth_type, @header_length,
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@identification)),
					    ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_username)),
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_password)),
						@connection_parameter_type_id,
						@timeout_milli,
						@buffer_size,
						@doc_type,
						@name_of_file,
						@file_delete_YN,
						@file_encryption_type_id,
						@duplicate_file_check_YN,
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@private_key)),
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@public_key)),@domain_name,@is_external_auth)		


				SET @connection_parameter_id = SCOPE_IDENTITY();

				CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

				--log the audit record
				DECLARE @audit_description varchar(max)
				SELECT @audit_description = 'Parameter Created: ' + @connection_name
											+ ' , Paramter Id: ' + CAST(@connection_parameter_id AS varchar(max))

				EXEC usp_insert_audit @audit_user_id, 
									 9,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL		


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
/****** Object:  StoredProcedure [dbo].[usp_connection_parameter_update]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_connection_parameter_update]
	@connection_parameter_id int,
	@connection_name varchar(100),
	@connection_parameter_type_id int,
	@address varchar(200),
	@port int,
	@name_of_file varchar(100),
	@file_delete_YN bit,
	@file_encryption_type_id int,
	@duplicate_file_check_YN bit,
	@path varchar(200),
	@protocol int,
	@auth_type int,
	@header_length int = null,
	@identification varchar(100) = NULL,
	@timeout_milli int = NULL,
	@buffer_size int = NULL,
	@doc_type char = NULL,
	@auth_username VARCHAR(100) = NULL,
	@auth_password VARCHAR(100) = NULL,
	@private_key VARCHAR(MAX) = NULL,
	@public_key VARCHAR(MAX) = NULL,
	@domain_name varchar(100),
	@is_external_auth bit,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_CONN_PARAM_TRAN]
		BEGIN TRY 

			IF @identification IS NULL
				SET @identification = ''

			IF @auth_username IS NULL
				SET @auth_username = ''

			IF @auth_password IS NULL
				SET @auth_password = ''

			DECLARE @priv varbinary(max), @pub varbinary(max)

									

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

				IF(ISNULL(@private_key, '') = '' )
					SELECT @priv = [private_key] FROM [connection_parameters] WHERE [connection_parameter_id] = @connection_parameter_id
				ELSE 
					SELECT @priv = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@private_key))


				IF(ISNULL(@public_key, '') = '' )
					SELECT @pub = [public_key] FROM [connection_parameters] WHERE [connection_parameter_id] = @connection_parameter_id
				ELSE 
					SELECT @pub = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@public_key))	

				UPDATE [dbo].[connection_parameters]
				SET [connection_name] = @connection_name,
					[address] = @address,
					[port] = @port,
					[path] = @path,
					[protocol] = @protocol,
					[auth_type] = @auth_type,
					[header_length] = @header_length,
					[identification] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@identification)),
					[username] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_username)),
					[password] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_password)),
					[connection_parameter_type_id]=@connection_parameter_type_id,
					[timeout_milli] = @timeout_milli,
					[buffer_size] = @buffer_size,
					[doc_type] = @doc_type,
					[name_of_file] = @name_of_file,
					file_delete_YN = @file_delete_YN,
					file_encryption_type_id = @file_encryption_type_id,
					duplicate_file_check_YN = @duplicate_file_check_YN,
					[private_key] = @priv,
					[public_key] = @pub,
					[domain_name]=@domain_name,
					[is_external_auth]=@is_external_auth
				WHERE [connection_parameter_id] = @connection_parameter_id


				CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

				--log the audit record
				DECLARE @audit_description varchar(max)
				SELECT @audit_description = 'Parameter Updated: ' + @connection_name
											+ ' , Paramter Id: ' + CAST(@connection_parameter_id AS varchar(max))			
				EXEC usp_insert_audit @audit_user_id, 
									 9,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL		


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
/****** Object:  StoredProcedure [dbo].[usp_create_cms_upload_batch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_cms_upload_batch] 
	@card_issue_method_id int,
	@issuer_id int,
	@branch_id int = null,
	@product_id int = null,
	@card_priority_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@cards_in_batch int OUTPUT,
	@dist_batch_id bigint OUTPUT,
	--@dist_batch_ref varchar(50) OUTPUT,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [CREATE_CMS_BATCH]
		BEGIN TRY 

		DECLARE @dist_batch_ref varchar(50)

		SET @cards_in_batch = 0
		SET	@dist_batch_id = 0
		SET @dist_batch_ref = ''

		DECLARE @branch_card_statuses_id int
		SET @branch_card_statuses_id = 6

		--Only create a batch if there are cards for the batch
		IF( (SELECT COUNT(*) 
			 FROM branch_card_status_current
					INNER JOIN branch
						ON branch_card_status_current.branch_id = branch.branch_id						
			 WHERE branch_card_statuses_id = @branch_card_statuses_id
					AND product_id = COALESCE(@product_id, product_id)
					AND card_issue_method_id = @card_issue_method_id
					AND branch_card_status_current.branch_id = COALESCE(@branch_id, branch_card_status_current.branch_id)
					AND issuer_id = @issuer_id
					AND card_id NOT IN(SELECT card_id 
										FROM [dist_batch_cards] 
											INNER JOIN [dist_batch]
												ON [dist_batch_cards].dist_batch_id = [dist_batch].dist_batch_id
												AND [dist_batch].dist_batch_type_id = 0
												AND [dist_batch].issuer_id = @issuer_id)) = 0)
		BEGIN
			SET @ResultCode = 400
			COMMIT TRANSACTION [CREATE_CMS_BATCH]
		END			
		ELSE
			BEGIN

				DECLARE @cards_total int = 0,
						@batch_branch_id int,
						@audit_msg nvarchar(500)


				--create the production batch
				INSERT INTO [dist_batch]
					([card_issue_method_id],[issuer_id],[branch_id], [no_cards],[date_created],[dist_batch_reference],[dist_batch_type_id])
				VALUES (@card_issue_method_id, @issuer_id, @branch_id, 0, GETDATE(), GETDATE(),0)

				SET @dist_batch_id = SCOPE_IDENTITY();

				--Add cards to production batch
				INSERT INTO [dist_batch_cards]
					([dist_batch_id],[card_id],[dist_card_status_id])
				SELECT @dist_batch_id, card_id, 12
				FROM branch_card_status_current
						INNER JOIN branch
							ON branch_card_status_current.branch_id = branch.branch_id	
				WHERE branch_card_statuses_id = @branch_card_statuses_id
					AND product_id = COALESCE(@product_id, product_id)
					AND card_issue_method_id = @card_issue_method_id
					AND branch_card_status_current.branch_id = COALESCE(@branch_id, branch_card_status_current.branch_id)
					AND issuer_id = @issuer_id
					AND card_id NOT IN(SELECT card_id 
										FROM [dist_batch_cards] 
											INNER JOIN [dist_batch]
												ON [dist_batch_cards].dist_batch_id = [dist_batch].dist_batch_id
												AND [dist_batch].dist_batch_type_id = 0
												AND [dist_batch].issuer_id = @issuer_id)


				--add prod batch status of created
				INSERT INTO [dbo].[dist_batch_status]
					([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
				VALUES(@dist_batch_id, 0, @audit_user_id, GETDATE(), 'Dist Batch Create')

				--Generate dist batch reference
				SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
										  CONVERT(VARCHAR(MAX),[issuer_product].product_id) + '' +										  
										  CONVERT(VARCHAR(8), GETDATE(), 112) + '' +
										  CAST(@dist_batch_id AS varchar(max))
				FROM [issuer]					
					INNER JOIN [issuer_product]
						ON [issuer_product].issuer_id = [issuer].issuer_id
				WHERE [issuer].issuer_id = @issuer_id

				SELECT @cards_in_batch = COUNT(*)
				FROM dist_batch_cards
				WHERE dist_batch_id = @dist_batch_id 

				--UPDATE prod batch with reference and number of cards
				UPDATE [dist_batch]
				SET [dist_batch_reference] = @dist_batch_ref,
					[no_cards] = @cards_in_batch
				WHERE [dist_batch].dist_batch_id = @dist_batch_id

				--update prod batch status to sent to cms
				INSERT INTO [dbo].[dist_batch_status]
					([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
				VALUES(@dist_batch_id, 10, @audit_user_id, DATEADD(ss, 1, GETDATE()), '')


				--UPDATE branch card status for those cards that have been added to the new dist batch.
				--INSERT INTO [branch_card_status]
				--	(branch_card_statuses_id, card_id, comments, status_date, [user_id])
				--SELECT 10, card_id, 'Assigned to batch', GETDATE(), @audit_user_id
				--FROM dist_batch_cards
				--WHERE dist_batch_id = @dist_batch_id	

				--Add audit for dist batch creation	
				DECLARE @dist_batch_status_name varchar(50)
				SELECT @dist_batch_status_name =  dist_batch_status_name
				FROM dist_batch_statuses
				WHERE dist_batch_statuses_id = 0
											
				SET @audit_msg = 'Create: ' + CAST(@dist_batch_id AS varchar(max)) +
									', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
									', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										2,
										NULL, 
										@audit_workstation, 
										@audit_msg, 
										NULL, NULL, NULL, NULL

				SELECT @ResultCode = 0
				COMMIT TRANSACTION [CREATE_CMS_BATCH]	

			END					
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_CMS_BATCH]
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
/****** Object:  StoredProcedure [dbo].[usp_create_customer_fields]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi	
-- Create date: 20151116
-- Description:	Insert customer_fields into db
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_customer_fields]
	@product_field_id bigint
	, @value VARCHAR(MAX)
	, @audit_user_id bigint
	, @audit_workstation varchar(100)
	, @customer_fields_id bigint OUTPUT
	, @ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	OPEN Symmetric Key Indigo_Symmetric_Key
	DECRYPTION BY Certificate Indigo_Certificate;

	BEGIN TRANSACTION [INSERT_CUSTOMER_FIELDS_TRAN]
	BEGIN TRY 
		INSERT INTO [dbo].[customer_fields]
			   ([product_field_id]
			   ,[value])
		 VALUES
			   (@product_field_id
			   , CONVERT(VARBINARY(MAX),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR,@value))))
		   
		SET @customer_fields_id = SCOPE_IDENTITY();
		SET @ResultCode = 0	
		

		--log the audit record
		DECLARE @audit_description varchar(max), @audit_branch_name varchar(max), 
				@audit_branch_code varchar(max)

		SET @audit_description = 'Customer Print Fields Create: Product Print Id '+ COALESCE(@product_field_id, 'UNKNOWN') 
									+ ' , Customer Field Id: ' + CAST(@customer_fields_id AS VARCHAR(MAX))
										 	
		EXEC usp_insert_audit @audit_user_id, 
								9,
								NULL, 
								@audit_workstation, 
								@audit_description, 
								NULL, NULL, NULL, NULL

								SET @ResultCode = 0		

		COMMIT TRANSACTION [INSERT_CUSTOMER_FIELDS_TRAN]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_CUSTOMER_FIELDS_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_create_dist_batch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Manually create a new distribution batch. 
-- This distributin batch is created from cards received at a card centre
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_dist_batch] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@branch_id int,	
	@to_branch_id int,
	@card_issue_method_id int,
	@product_id int,
	--@sub_product_id int = NULL,
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
						--AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
						AND [cards].card_issue_method_id = @card_issue_method_id
						AND [cards].branch_id = @branch_id
			
				--Get the end card if
				SELECT @end_card_id = card_id 
				FROM [cards]				
				WHERE [cards].card_request_reference  = @end_ref
						AND [cards].product_id = @product_id
						--AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
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
							--AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
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
							--AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
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
						--AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
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
						--AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
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
					VALUES(@dist_batch_id, 0, @audit_user_id, @status_date, 'Batch Created')
				END
				ELSE
				BEGIN
					--add dist batch status of created
					INSERT INTO [dbo].[dist_batch_status]
						([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
					VALUES(@dist_batch_id, 19, @audit_user_id, @status_date, 'Batch Created')
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
				EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_create_export_batches]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_export_batches]
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @product_id int, @issuer_id_cursor int
	DECLARE @confirmed_export_batches TABLE (export_batch_id bigint)

	SET @ResultCode = 0

	--Find products who have exportable set to true and are not deleted.
	DECLARE export_product_cursor CURSOR FOR 
		SELECT product_id, issuer_id 
		FROM [issuer_product] 
		WHERE cms_exportable_YN = 1 
				AND DeletedYN = 0
				AND issuer_id = COALESCE(@issuer_id, issuer_id)
	OPEN export_product_cursor
	FETCH NEXT FROM export_product_cursor
	INTO @product_id, @issuer_id_cursor

		WHILE @@FETCH_STATUS = 0
		BEGIN
			DECLARE @export_cards TABLE (card_id bigint)
			DECLARE @create_export_batch_statuses_id int = 0
				

			INSERT INTO @export_cards (card_id)
			SELECT [cards].card_id 
			FROM [cards] INNER JOIN [branch_card_status]						
					ON [cards].card_id = [branch_card_status].card_id 					
			WHERE [cards].product_id = @product_id 
				AND [branch_card_status].branch_card_statuses_id = 6
				AND [cards].export_batch_id IS NULL
				
			--Are there any cards that need a batch created
			IF EXISTS(SELECT * FROM @export_cards)
				BEGIN
					BEGIN TRANSACTION [CREATE_EXPORT_BATCHES]
					BEGIN TRY 
						DECLARE @export_batch TABLE (ID bigint)
						DECLARE @no_cards int

						SELECT @no_cards = COUNT(card_id) FROM @export_cards

						--CREATE NEW EXPORT BATCH
						INSERT INTO [export_batch] (batch_reference, date_created, issuer_id, no_cards)
							OUTPUT inserted.export_batch_id INTO @export_batch(ID)
						VALUES (SYSDATETIME(), SYSDATETIME(), @issuer_id_cursor, @no_cards)

						DECLARE @export_batch_id bigint
						SELECT TOP 1 @export_batch_id = ID FROM @export_batch

						--ADD CREATED STATUS
						INSERT INTO [export_batch_status] (export_batch_id, export_batch_statuses_id, status_date, [user_id], comments)
						VALUES (@export_batch_id, @create_export_batch_statuses_id, SYSDATETIME(), @audit_user_id, 'CREATED')
						
						--LINK CARDS TO EXPORT BATCH
						UPDATE [cards]
						SET export_batch_id = @export_batch_id
						WHERE card_id IN (SELECT card_id FROM @export_cards)
						
						--AUDIT CREATE
						DECLARE @batch_status_name varchar(100),
								@batch_ref varchar(100),
								@audit_msg varchar(max)

						SELECT @batch_status_name =  export_batch_statuses_name
						FROM export_batch_statuses
						WHERE export_batch_statuses_id = @create_export_batch_statuses_id

						SELECT @batch_ref = batch_reference
						FROM export_batch
						WHERE export_batch_id = @export_batch_id

						--Add audit for pin batch update								
						SET @audit_msg = 'Create: ' + CAST(@export_batch_id AS varchar(max)) +
											', ' + COALESCE(@batch_ref, 'UNKNOWN') +
											', ' + COALESCE(@batch_status_name, 'UNKNOWN')
								   
						--log the audit record		
						EXEC usp_insert_audit @audit_user_id, 
												11,
												NULL, 
												@audit_workstation, 
												@audit_msg, 
												NULL, NULL, NULL, NULL


						--AUTO-APPROVE BATCH?
						DECLARE @ApproveResultCode int
						EXEC usp_export_batch_status_approve @export_batch_id, 1, 0, @audit_user_id, @audit_workstation, @ApproveResultCode OUTPUT
						--INSERT INTO [export_batch_status] (export_batch_id, export_batch_statuses_id, status_date, [user_id], comments)
						--VALUES (@export_batch_id, 1, DATEADD(SECOND, 1, SYSDATETIME()), @audit_user_id, 'AUTO APPROVED')

						--CHECK THAT ALL CARDS HAVE BEEN LINKED, if it doesnt match rollback for this product
						IF ( @ApproveResultCode = 0 AND
								(SELECT COUNT(*) FROM @export_cards) =
								(SELECT COUNT(*) FROM [cards] WHERE export_batch_id = @export_batch_id
									AND card_id IN (SELECT card_id FROM @export_cards)))
							BEGIN
								INSERT INTO @confirmed_export_batches(export_batch_id)
								VALUES (@export_batch_id)							


								COMMIT TRANSACTION [CREATE_EXPORT_BATCHES]
							END
						ELSE
							BEGIN
								SET @ResultCode = 1000						
								ROLLBACK TRANSACTION [CREATE_EXPORT_BATCHES]
							END

					END TRY
					BEGIN CATCH
						SET @ResultCode = 1000	
						ROLLBACK TRANSACTION [CREATE_EXPORT_BATCHES]
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
		
			FETCH NEXT FROM export_product_cursor
			INTO @product_id, @issuer_id
		END 
	CLOSE export_product_cursor;
	DEALLOCATE export_product_cursor;

	SELECT * FROM [export_batch]
	WHERE export_batch_id IN (SELECT export_batch_id FROM @confirmed_export_batches)
END




GO
/****** Object:  StoredProcedure [dbo].[usp_create_external_system_fields]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_external_system_fields]
@external_system_id int,
@field_name nvarchar(100),
@audit_user_id bigint,
@audit_workstation varchar(100),
@external_system_field_id int output,
@ResultCode int output
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		BEGIN TRANSACTION [CREATE_EXTERNAL_SYSTEM_FIELD]
		BEGIN TRY 
			BEGIN
						INSERT INTO external_system_fields ( external_system_id, field_name)
						VALUES        (@external_system_id,@field_name)
		
				SET @external_system_field_id=SCOPE_IDENTITY()

				set @ResultCode=0
				
				Declare @audit_msg nvarchar(500)
						 --Add audit for dist batch creation								
				SET @audit_msg = 'CREATE: ' + CAST(@external_system_field_id AS varchar(max))+
									'FIELD NAME:'+@field_name
									
								   
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										9,
										NULL, 
										@audit_workstation, 
										@audit_msg, 
										NULL, NULL, NULL, NULL
										
										
			COMMIT TRANSACTION [CREATE_EXTERNAL_SYSTEM_FIELD]	
			END
									
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_EXTERNAL_SYSTEM_FIELD]
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;
		SET @external_system_field_id=0
		
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
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;



END

GO
/****** Object:  StoredProcedure [dbo].[usp_create_external_systems]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_external_systems]
@external_system_type_id int,
@system_name nvarchar(100),
@external_system_fields as dbo.[product_external_fields_array] READONLY,	
@audit_user_id bigint,
@audit_workstation varchar(100),
@external_system_id int output,
@ResultCode int output
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		BEGIN TRANSACTION [CREATE_EXTERNAL_SYSTEMS]
		BEGIN TRY 
			BEGIN
						 INSERT INTO external_systems
                         ( external_system_type_id, system_name)
VALUES        (@external_system_type_id,@system_name)

		SET @external_system_id=SCOPE_IDENTITY()

				DECLARE @RCExternal int
				EXECUTE @RCExternal = usp_modify_external_system_fields @external_system_id, @external_system_fields, @audit_user_id, @audit_workstation
				
				Declare @audit_msg nvarchar(500)
						 --Add audit for dist batch creation								
				SET @audit_msg = 'CREATE: ' + CAST(@external_system_id AS varchar(max))+
									'SYSTEM NAME:'+@system_name
									
					set @ResultCode=0			   
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										9,
										NULL, 
										@audit_workstation, 
										@audit_msg, 
										NULL, NULL, NULL, NULL
										
										
			COMMIT TRANSACTION [CREATE_EXTERNAL_SYSTEMS]	
			END
									
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_EXTERNAL_SYSTEMS]
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;
		SET @external_system_id=0
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
/****** Object:  StoredProcedure [dbo].[usp_create_fee_scheme]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_fee_scheme] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@fee_scheme_name varchar(100),
	@fee_detail_list as dbo.fee_detail_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@new_fee_scheme_id int OUTPUT,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [CREATE_PRODUCT_FEE_SCHEME_TRAN]
		BEGIN TRY 			
			IF (SELECT COUNT(*) FROM [product_fee_scheme] 
					WHERE fee_scheme_name = @fee_scheme_name AND issuer_id = @issuer_id) > 0
				BEGIN
					SET @new_fee_scheme_id = 0
					SET @ResultCode = 226						
				END		
			ELSE
				BEGIN
					DECLARE @effective_from DATETIME = GETDATE()

					INSERT INTO product_fee_scheme (fee_scheme_name, issuer_id, deleted_yn)
						VALUES (@fee_scheme_name, @issuer_id, 0)

					SET @new_fee_scheme_id = SCOPE_IDENTITY();

					INSERT INTO [product_fee_detail] (fee_scheme_id, fee_detail_name, fee_editable_YN, fee_waiver_YN, 
														effective_from, effective_to, deleted_yn)
					SELECT @new_fee_scheme_id, dl.fee_detail_name, dl.fee_editable_TN, dl.fee_waiver_YN, 
							@effective_from, null, 0
					FROM @fee_detail_list dl

					--log the audit record
			DECLARE @audit_description varchar(max), @fee_details varchar(max)

			SELECT  @fee_details = STUFF(
							(SELECT ' Fee Band Name: ' + dl.fee_detail_name + ', editable: ' +  CAST(dl.fee_editable_TN AS VARCHAR(MAX)) 
								+ ', waiver: ' +  CAST(dl.fee_waiver_YN AS VARCHAR(MAX)) + ';' 
								FROM @fee_detail_list dl
								FOR XML PATH(''))
							, 1
							, 1
							, '')
					

			SET @audit_description = 'Fee Scheme Create: ' + @fee_scheme_name	
										+ ' , Fee Scheme Id: ' + CAST(@new_fee_scheme_id  AS VARCHAR(max))
										+' , Fee Bands : ' + COALESCE(@fee_details, 'NONE')
										 	
			EXEC usp_insert_audit @audit_user_id, 
									4,
									NULL, 
									@audit_workstation, 
									@audit_description, 
									@issuer_id, NULL, NULL, NULL
					
					SET @ResultCode = 0
				END

				COMMIT TRANSACTION [CREATE_PRODUCT_FEE_SCHEME_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_PRODUCT_FEE_SCHEME_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_create_issuer]    Script Date: 2016-04-05 09:55:42 AM ******/
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

CREATE PROCEDURE [dbo].[usp_create_issuer]
	@issuer_status_id int,
	@country_id int,
	@issuer_name varchar(50),
	@issuer_code varchar(10),	
	@instant_card_issue_YN bit,	
	@maker_checker_YN bit,
	@enable_instant_pin_YN bit,
	@authorise_pin_issue_YN bit,
	@authorise_pin_reissue_YN bit,	
	@license_file varchar(100) = NULL,
	@license_key varchar(1000) = NULL,	
	@language_id int = NULL,
	@card_ref_preference bit,
	@classic_card_issue_YN bit,
	@back_office_pin_auth_YN bit,
	@prod_interface_parameters_list AS dbo.bikey_value_array READONLY,
	@issue_interface_parameters_list AS dbo.bikey_value_array READONLY,
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
					   ([issuer_status_id],[country_id],[issuer_name],[issuer_code]
					   ,[instant_card_issue_YN],[maker_checker_YN],[license_file],[license_key]
					   ,[language_id],[card_ref_preference],[classic_card_issue_YN],[enable_instant_pin_YN],
					   [authorise_pin_issue_YN],[authorise_pin_reissue_YN], back_office_pin_auth_YN)
				 VALUES
					   (@issuer_status_id, @country_id, @issuer_name, @issuer_code,
						@instant_card_issue_YN,	@maker_checker_YN,
						@license_file, @license_key, @language_id,@card_ref_preference,
						@classic_card_issue_YN,@enable_instant_pin_YN, @authorise_pin_issue_YN,
						@authorise_pin_reissue_YN, @back_office_pin_auth_YN)

				SET @new_issuer_id = SCOPE_IDENTITY();

				INSERT INTO [issuer_interface] (issuer_id, interface_type_id, connection_parameter_id, interface_guid, interface_area)
				SELECT @new_issuer_id, key1, key2, value, 0
				FROM @prod_interface_parameters_list

				INSERT INTO [issuer_interface] (issuer_id, interface_type_id, connection_parameter_id, interface_guid, interface_area)
				SELECT @new_issuer_id, key1, key2, value, 1
				FROM @issue_interface_parameters_list

				--IF (@instant_card_issue_YN = 1)
				--BEGIN

				--	IF (@account_validation_YN = 1)
				--	BEGIN
				--		--INSERT ACCOUNT INTERFACE
				--		INSERT INTO [issuer_interface]
				--			([issuer_id], [interface_type_id], [connection_parameter_id])
				--		VALUES (@new_issuer_id, 0, @account_connection_id)
				--	END

				--	--INSERT CORE BANKING INTERFACE
				--	INSERT INTO [issuer_interface]
				--		([issuer_id], [interface_type_id], [connection_parameter_id])
				--	VALUES (@new_issuer_id, 1, @corebanking_connection_id)
				--END

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
										 	
				EXEC usp_insert_audit @audit_user_id, 
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

				EXEC usp_create_user_group @group_name, 4, 
										  @new_issuer_id, 1, 1, 1, 1, 1, 1,
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

				EXEC usp_create_user_group @group_name, 5, 
									      @new_issuer_id, 1, 1, 1, 1, 1, 1,
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
/****** Object:  StoredProcedure [dbo].[usp_create_ldap]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo Setenane
-- Create date: 2014/04/03
-- Description:	Creates a new LDAP record associated and returns the new record's ID
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_ldap]
	@ldap_setting_name varchar(100),
	@hostname_or_ip varchar(200),
	@path varchar(200),
	@domain_name varchar(100) = NULL,
	@auth_username varchar(200) = NULL,
	@auth_password varchar(200) = NULL,
	@auth_type_id int,
	@external_inteface_id char(36),
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
						([ldap_setting_name],[hostname_or_ip],[domain_name],[path],[username],[password],auth_type_id,external_inteface_id)
					VALUES
							(@ldap_setting_name, @hostname_or_ip, @domain_name, @path, 
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_username)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_password)),@auth_type_id,@external_inteface_id)		


					SET @new_ldap_id = SCOPE_IDENTITY();

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key ;--Closes sym key

					--log the audit record
					DECLARE @audit_description varchar(max)
					SELECT @audit_description = 'LDAP Created: ' + @ldap_setting_name
												+ ', Id: ' + CAST(@new_ldap_id as varchar(max))

					EXEC usp_insert_audit @audit_user_id, 
											9,
											NULL, 
											@audit_workstation, 
											@audit_description, 
											NULL, NULL, NULL, NULL		

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
/****** Object:  StoredProcedure [dbo].[usp_create_load_batch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--DROP PROCEDURE [dbo].[usp_create_load_batch] 

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 6 March 2014
-- Description:	Create's the load batch, load batch cards, load batch status and file history within a transaction.
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_load_batch] 
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
	@file_type_id int,
	@load_batch_type_id int,
	@file_id bigint = NULL OUTPUT,
	@load_batch_id bigint = NULL OUTPUT,
	@load_batch_status_id bigint = NULL OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;
	SET QUERY_GOVERNOR_COST_LIMIT 0;

	BEGIN TRANSACTION [CREATE_LOAD_BATCH_TRAN]
	BEGIN TRY 

		DECLARE @RC int
		DECLARE @number_of_cards int

		--Inserts should happen in the following order
		--1. file_history
		--2. load_batch
		--3. load_batch_status
		--4. cards
		--5. load_batch_cards

		SET @number_of_cards = (SELECT COUNT(*) FROM @card_list)

		--Insert into file history
		EXECUTE @RC = [dbo].[usp_insert_file_history] @file_load_id, @issuer_id,@name_of_file,@file_created_date
													,@file_size,@load_date,@file_status_id,@file_directory
													,@number_successful_records,@number_failed_records
													,@file_load_comments,@file_type_id,@file_id OUTPUT

		--Insert into load_batch
		EXECUTE @RC = [dbo].[usp_insert_load_batch] @load_batch_reference,@file_id,@issuer_id,@batch_status_id
												  ,@load_date,@number_of_cards,@load_batch_type_id, @load_batch_id OUTPUT

		--Insert into load_batch_status
		EXECUTE @RC = [dbo].[usp_insert_load_batch_status] @load_batch_id,@batch_status_id,@load_date
														 ,@user_id,@load_batch_status_id OUTPUT

		
		OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
		DECRYPTION BY CERTIFICATE Indigo_Certificate
			DECLARE @objid int,
					@total_records int,
					@processed_records int = 0,
					@status_date DATETIME = GETDATE(),
					@processed_records_end int,
					@records_to_process int = 500 --Use this to tune the amount of records to load	

			--This section helps with creating the card_index, instead of calling the fuction each time
			--Which slows down the insers, we get the key and then just encrypt
			SET @objid = object_id('cards')			
			DECLARE @key varbinary(100)
			SET @key = null
			SELECT @key = DecryptByKeyAutoCert(cert_id('cert_ProtectIndexingKeys'), null, mac_key) 
			FROM mac_index_keys 
			WHERE table_id = @objid

			IF(@key IS NULL)
				RAISERROR (N'MAC Index Key is null.', 15, 1);

			--Determin the number of records
			SELECT @total_records = COUNT(*) FROM @card_list

			--table car for storing the new card id's. used later when creating the ref numbers.
			DECLARE @new_card_id_list TABLE ( card_id bigint)

			--Insert new card records in batches
			WHILE (@processed_records < @total_records)
			BEGIN

				SET @processed_records_end = @processed_records + @records_to_process;

				MERGE [cards] AS cardsTable
				USING (SELECT *
						FROM (
								SELECT *, 
								ROW_NUMBER() OVER (ORDER BY card_number) AS RowNum
								 FROM @card_list								 
								 ) AS cards_to_process
						 WHERE cards_to_process.RowNum BETWEEN @processed_records AND @processed_records_end
								AND cards_to_process.load_batch_type_id IN (1, 2, 3))
								
						AS cardsList
				ON (DECRYPTBYKEY(cardsTable.card_number) = cardsList.card_number AND cardsTable.product_id = cardsList.product_id) 
				WHEN NOT MATCHED BY TARGET
					THEN INSERT ([product_id],[card_issue_method_id],[branch_id],[origin_branch_id],[card_number],
									[card_sequence],[card_priority_id],[card_request_reference]
									,[card_expiry_date]
									,[card_index]) 
						VALUES(cardsList.product_id,
							   --cardsList.sub_product_id,
							   cardsList.card_issue_method_id,
							   cardsList.branch_id,
							   cardsList.branch_id
							   ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR,cardsList.card_number))
							   ,cardsList.card_sequence
							   ,1
							   ,cardsList.card_reference
							   ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR,cardsList.[expiry_date]))
							   ,CONVERT(varbinary(24),HashBytes( N'SHA1', CONVERT(varbinary(8000), CONVERT(nvarchar(4000),RIGHT(cardsList.card_number, 4))) + @key )))
				OUTPUT inserted.card_id INTO @new_card_id_list;
				
				--Update with card reference number
				--UPDATE [cards]
				--	SET card_request_reference = dbo.GenCardReferenceNo(@status_date, [cards].card_id)
				--WHERE [card_id] IN (SELECT card_id FROM @new_card_id_list)

				DELETE FROM @new_card_id_list

				SET @processed_records = @processed_records_end
			END

			DECLARE @load_batch_records int = 0

			--Update ordered cards
			UPDATE cardsupdate
			SET cardsupdate.card_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR,card_list.card_number)),
				cardsupdate.card_expiry_date = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR,card_list.[expiry_date])),
				cardsupdate.card_request_reference = card_list.card_reference,
				cardsupdate.card_sequence = card_list.card_sequence,
				cardsupdate.branch_id = card_list.branch_id,
				cardsupdate.origin_branch_id = card_list.branch_id,
				cardsupdate.card_index = CONVERT(varbinary(24),HashBytes( N'SHA1', CONVERT(varbinary(8000), CONVERT(nvarchar(4000),RIGHT(card_list.card_number, 4))) + @key ))
			FROM @card_list card_list
					INNER JOIN [cards] cardsupdate
						ON cardsupdate.card_id = card_list.card_id
			WHERE card_list.load_batch_type_id = 4


			DECLARE @inserted_load_cards TABLE(load_batch_id bigint, card_id bigint, load_card_status_id int);

			--Insert into load_batch_cards, links cards to load batch
			INSERT INTO load_batch_cards ([load_batch_id], [card_id], [load_card_status_id])
			OUTPUT inserted.* INTO @inserted_load_cards
			SELECT @load_batch_id, cards.card_id, @load_card_status_id
			FROM cards 
				INNER JOIN (SELECT * FROM @card_list) as cardList
				ON cards.product_id = cardList.product_id
				AND DECRYPTBYKEY(cards.card_number) = cardList.card_number 
			--WHERE DECRYPTBYKEY(cards.card_number) IN (SELECT cards_list.card_number
			--										  FROM @card_list cards_list
			--										  WHERE cards_list.product_id = cards.product_id)

			--SELECT @load_batch_records = @@ROWCOUNT

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
					
			--declare @audit_msg nvarchar(50)
			--SET @audit_msg = 'INSERT :' + CAST(@load_batch_reference AS varchar(max))  +', LOADED'
			----log the audit record		
			--EXEC usp_insert_audit @user_id, 
			--						5, --LoadBatch
			--						NULL, 
			--						'LOADED', 
			--						@audit_msg, 
			--						NULL, NULL, NULL, NULL		

			SELECT @load_batch_records = COUNT(*) FROM @inserted_load_cards

		IF(@number_of_cards != @load_batch_records) --@load_batch_records)
			RAISERROR (N'Records inserted for loadbatch (%i) dont match those for the load (%i).', 15, 1, @load_batch_records, @number_of_cards);

		COMMIT TRANSACTION [CREATE_LOAD_BATCH_TRAN]

	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_LOAD_BATCH_TRAN]
		RETURN ERROR_MESSAGE()
	END CATCH 

	RETURN '0'
END




GO
/****** Object:  StoredProcedure [dbo].[usp_create_load_batch_bulk_card_request]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		LTladi
-- Create date: 6 March 2014
-- Description:	Create's the load batch, load batch cards, load batch status and file history within a transaction.
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_load_batch_bulk_card_request] 
	@load_batch_reference varchar(50),
	@batch_status_id int,
	@user_id bigint,	
	@load_card_status_id int,
	@load_batch_type_id int,
	@card_list AS [dbo].[load_bulk_card_request] READONLY,
	@product_fields as [dbo].[bi_key_binary_value_array] READONLY,
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
	--SET NOCOUNT ON;
	SET QUERY_GOVERNOR_COST_LIMIT 0;

	BEGIN TRANSACTION [CREATE_LOAD_REQUEST_BATCH_TRAN]
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
		EXECUTE @RC = [dbo].[usp_insert_file_history] @file_load_id, @issuer_id,@name_of_file,@file_created_date
													,@file_size,@load_date,@file_status_id,@file_directory
													,@number_successful_records,@number_failed_records
													,@file_load_comments,@file_type_id,@file_id OUTPUT

		--Insert into load_batch
		EXECUTE @RC = [dbo].[usp_insert_load_batch] @load_batch_reference,@file_id,@issuer_id,@batch_status_id
												  ,@load_date,@number_of_cards,@load_batch_type_id, @load_batch_id OUTPUT

		--Insert into load_batch_status
		EXECUTE @RC = [dbo].[usp_insert_load_batch_status] @load_batch_id,@batch_status_id,@load_date
														 ,@user_id,@load_batch_status_id OUTPUT


		
		OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
		DECRYPTION BY CERTIFICATE Indigo_Certificate
			DECLARE @objid int,
					@total_records int,
					@processed_records int = 0,
					@status_date DATETIME = GETDATE(),
					@processed_records_end int,
					@records_to_process int = 500 --Use this to tune the amount of records to load	

			--This section helps with creating the card_index, instead of calling the fuction each time
			--Which slows down the insers, we get the key and then just encrypt
			SET @objid = object_id('cards')			
			DECLARE @key varbinary(100)
			SET @key = null
			SELECT @key = DecryptByKeyAutoCert(cert_id('cert_ProtectIndexingKeys'), null, mac_key) 
			FROM mac_index_keys 
			WHERE table_id = @objid

			IF(@key IS NULL)
				RAISERROR (N'MAC Index Key is null.', 15, 1);

			--Determine the number of records
			SELECT @total_records = COUNT(*) FROM @card_list

			DECLARE @temp_customer_account TABLE (					
						[temp_customer_account_id] [bigint] NOT NULL,
						[card_id] [bigint] NOT NULL)

			--Used to link the actual customer account ID with the temp customer account id
			DECLARE @temp_link_customer TABLE (	
					[customer_account_id] [bigint] NOT NULL,					
					[temp_customer_account_id] [bigint] NOT NULL)

			--Insert new card records in batches
			WHILE (@processed_records < @total_records)
			BEGIN

				SET @processed_records_end = @processed_records + @records_to_process;

				DELETE FROM @temp_customer_account
				DELETE FROM @temp_link_customer

				MERGE [cards] AS cardsTable
				USING (SELECT *
						FROM (
								SELECT *, 
								ROW_NUMBER() OVER (ORDER BY reference_number) AS RowNum
								 FROM @card_list								 
								 ) AS cards_to_process
						 WHERE cards_to_process.RowNum BETWEEN @processed_records AND @processed_records_end
								AND cards_to_process.load_product_batch_type_id IN (5, 6)) AS cardList
				ON (cardsTable.card_request_reference = cardList.reference_number AND cardsTable.product_id = cardList.product_id) 
				WHEN NOT MATCHED BY TARGET THEN
					-- INSERT into Table1:
					INSERT ( [product_id], [branch_id], [origin_branch_id], [card_sequence], [card_index], [card_issue_method_id]
						, [card_priority_id], [fee_waiver_YN], [fee_editable_YN], [fee_charged], [fee_overridden_YN]
						, card_request_reference, card_number)
					VALUES ( cardList.product_id, cardList.branch_id, cardList.branch_id, 0, [dbo].[MAC]('0', @objid), 0
						, cardList.card_priority_id, cardList.fee_waiver_YN, cardList.fee_editable_YN, cardList.fee_charged, cardList.fee_overridden_YN
						, cardList.reference_number
						, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR(MAX), cardList.card_number)))
		   		OUTPUT					
					cardList.temp_customer_account_id					
					, inserted.card_id					
				INTO @temp_customer_account;			
				
				--Insert temp customers to actual table
				MERGE INTO [customer_account]
				USING (SELECT t2.*, t1.card_id
						FROM @temp_customer_account t1 INNER JOIN @card_list t2
								ON t1.temp_customer_account_id = t2.temp_customer_account_id
						) AS tca
				ON 1 = 0
				WHEN NOT MATCHED THEN
				  INSERT ([user_id],[card_id],[card_issue_reason_id],[account_type_id],[currency_id]
							,[resident_id],[customer_type_id],[customer_account_number]
							,[customer_first_name],[customer_middle_name],[customer_last_name]
							,[name_on_card],[date_issued],[cms_id],[contract_number],[customer_title_id]
							,[Id_number],[contact_number],[CustomerId],[domicile_branch_id])
					VALUES ([tca].audit_user_id,[tca].[card_id],[tca].[card_issue_reason_id],[tca].[account_type_id],[tca].[currency_id]
							,[tca].[resident_id],[tca].[customer_type_id]
							,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CONVERT(VARCHAR(MAX),[tca].[customer_account_number]))
							,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CONVERT(VARCHAR(MAX),[tca].[customer_first_name]))
							,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CONVERT(VARCHAR(MAX),[tca].[customer_middle_name]))
							,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CONVERT(VARCHAR(MAX),[tca].[customer_last_name]))
							,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CONVERT(VARCHAR(MAX),[tca].[name_on_card]))
							,GETDATE(),[tca].[cms_id],[tca].[contract_number],[tca].[customer_title_id]
							,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CONVERT(VARCHAR(MAX),[tca].[Idnumber]))
							,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CONVERT(VARCHAR(MAX),[tca].[contact_number]))
							,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CONVERT(VARCHAR(MAX),[tca].[customer_id]))
							,[tca].[domicile_branch_id])			
				OUTPUT inserted.customer_account_id, tca.temp_customer_account_id
				INTO @temp_link_customer;

				--Insert Product Fields
				--INSERT INTO customer_fields (customer_account_id, product_field_id, value)
				--	SELECT [customer_account_id], [key2], [value]
				--	FROM @product_fields INNER JOIN @temp_link_customer
				--			ON temp_customer_account_id = key1

				--Update Product Image Fields
				INSERT INTO customer_fields (customer_account_id, product_field_id, value)
					SELECT tlc.[customer_account_id], pf.[key2], ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CAST(pf.[value] as varbinary(max)))
					FROM @product_fields pf INNER JOIN @temp_link_customer tlc
								ON tlc.temp_customer_account_id = pf.key1
							INNER JOIN product_fields
								ON pf.[key2] = product_fields.product_field_id
					WHERE product_fields.print_field_type_id = 0

				--Update Product Image Fields
				INSERT INTO customer_image_fields (customer_account_id, product_field_id, value)
					SELECT [customer_account_id], pf.[key2], pf.[value]
					FROM @product_fields pf INNER JOIN @temp_link_customer
								ON temp_customer_account_id = [key1]
							INNER JOIN product_fields
								ON pf.[key2] = product_fields.product_field_id
					WHERE product_fields.print_field_type_id = 1

				SET @processed_records = @processed_records_end
			END
			

			DECLARE @load_batch_records int = 0
			DECLARE @inserted_load_cards TABLE(load_batch_id bigint, card_id bigint, load_card_status_id int);

			--Insert into load_batch_cards, links cards to load batch
			INSERT INTO load_batch_cards ([load_batch_id], [card_id], [load_card_status_id])
			OUTPUT inserted.* INTO @inserted_load_cards
			SELECT @load_batch_id, cards.card_id, @load_card_status_id
			FROM cards 
				INNER JOIN (SELECT * FROM @card_list) as cardList
				ON cards.product_id = cardList.product_id
				AND cards.card_request_reference = cardList.reference_number
			--WHERE cards.card_request_reference IN (SELECT cardList.reference_number
			--										  FROM @card_list cardList
			--										  WHERE cardList.product_id = cards.product_id)
													  
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
					
		SELECT @load_batch_records = COUNT(*) FROM @inserted_load_cards

		IF(@number_of_cards != @load_batch_records) 
			RAISERROR (N'Records inserted for loadbatch (%i) dont match those for the load (%i).', 15, 1, @load_batch_records, @number_of_cards);

		COMMIT TRANSACTION [CREATE_LOAD_REQUEST_BATCH_TRAN]

	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_LOAD_REQUEST_BATCH_TRAN]
		RETURN ERROR_MESSAGE()
	END CATCH 

	RETURN '0'
END


GO
/****** Object:  StoredProcedure [dbo].[usp_create_masterkey]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi	
-- Create date: 20150212
-- Description:	Create a new Masterkey
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_masterkey]
	@masterkey varchar(max)
	, @masterkey_name varchar(100)
	, @issuer_id int
	, @audit_user_id bigint
	, @audit_workstation varchar(100)
	, @new_masterkey_id int OUTPUT
	, @ResultCode int OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	    -- Insert statements for procedure here

	BEGIN TRANSACTION [INSERT_MASTERKEY_TRAN]
		BEGIN TRY 

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [masterkeys] WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([masterkeys].masterkey)) =@masterkey) > 0
				BEGIN
					SET @new_masterkey_id = 0
					SET @ResultCode = 606						
				END
			ELSE	
				IF (SELECT COUNT(*) FROM [masterkeys] WHERE [masterkeys].masterkey_name =@masterkey_name) > 0
				BEGIN
					SET @new_masterkey_id = 0
					SET @ResultCode = 607						
				END
			ELSE			
			BEGIN

			OPEN Symmetric Key Indigo_Symmetric_Key
			DECRYPTION BY Certificate Indigo_Certificate;

			INSERT INTO [dbo].[masterkeys]
				   ([masterkey]
				   ,[issuer_id]
				   ,[masterkey_name]
				   ,[date_created]
				   ,[date_changed])
			 VALUES
				   (ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@masterkey))
				   ,@issuer_id
				   ,@masterkey_name
				   ,GETDATE()
				   ,GETDATE())

		   	SET @new_masterkey_id = SCOPE_IDENTITY();
			SET @ResultCode =0
			CLOSE Symmetric Key Indigo_Symmetric_Key;

				--log the audit record
				DECLARE @audit_description varchar(max) = '',
						@issuer_name varchar(100)

				SELECT @issuer_name = issuer_name
				FROM [issuer]
				WHERE issuer_id = @issuer_id

				SET @audit_description = 'MasterKey Create: ' + @masterkey_name	
										+ ', Maskerkey Id: ' + CAST(@new_masterkey_id as varchar(max)) 
										 	
				EXEC usp_insert_audit @audit_user_id, 
									 9,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @issuer_id, NULL, NULL, NULL

										
				SET @ResultCode = 0		
			END
			 
			COMMIT TRANSACTION [INSERT_MASTERKEY_TRAN]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_MASTERKEY_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_create_product_print_fields]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi
-- Create date: 20151116
-- Description:	Create product print fields
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_product_print_fields]
	@product_id int
	, @field_name varchar(100)
	, @print_field_type_id int
	, @X float
	, @Y float
	, @width float
	, @height float
	, @font varchar(50)
	, @font_size int
	, @mapped_name varbinary(50)
	, @editable bit
	, @deleted bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [dbo].[product_fields]
           (product_id
		   ,[field_name]
           ,[print_field_type_id]
           ,[X]
           ,[Y]
           ,[width]
           ,[height]
           ,[font]
           ,[font_size]
           ,[mapped_name]
           ,[editable]
           ,[deleted])
     VALUES
           (@product_id 
			, @field_name 
			, @print_field_type_id 
			, @X 
			, @Y 
			, @width 
			, @height 
			, @font 
			, @font_size 
			, @mapped_name 
			, @editable 
			, @deleted )

END


GO
/****** Object:  StoredProcedure [dbo].[usp_create_terminal]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi
-- Create date: 20150130
-- Description:	Insert new terminals (pin pad/POS)
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_terminal]
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
				DECLARE @audit_description varchar(max), @issuer_id int, @audit_branch_name varchar(max), 
						@audit_branch_code varchar(max)

				SELECT @issuer_id = issuer_id, @audit_branch_name = branch_name,
						@audit_branch_code = branch_code
				FROM [branch]
				WHERE branch_id = @branch_id

				SET @audit_description = 'Terminal Create: '+ COALESCE(@terminal_name, 'UNKNOWN') 
										 + ' , Terminal Id: ' + CAST(@new_terminal_id as varchar(max))
										  + ' , branch code: ' + @audit_branch_code
										  + ' , branch name: ' + @audit_branch_name
										 	
				EXEC usp_insert_audit @audit_user_id, 
									 9,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @issuer_id, NULL, NULL, NULL

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
/****** Object:  StoredProcedure [dbo].[usp_create_user_group]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_create_user_group]
	@user_group_name varchar(50),
	@user_role_id int,
	@issuer_id int,
	@can_read bit,
	@can_create bit,
	@can_update bit,
	@mask_screen_pan bit = 1,
	@mask_report_pan bit = 1,
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
				[mask_screen_pan], [mask_report_pan],
				[all_branch_access], [user_group_name])
			VALUES
			   (@user_role_id, @issuer_id, 
			    @can_create, @can_read, @can_update, 0, 
				@mask_screen_pan, @mask_report_pan,
				@all_branch_access, @user_group_name)

			SET @new_user_group_id = SCOPE_IDENTITY();

			--Link branches to user group
			EXECUTE @RC = [usp_insert_user_group_branches] @new_user_group_id, @branch_list, @audit_user_id, @audit_workstation

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

			EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_create_useradminsettings]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_useradminsettings]
	@PasswordValidPeriod int,
	@PasswordMinLength int,
	@PasswordMaxLength int,
	@PreviousPasswordsCount int,
	@maxInvalidPasswordAttempts int,
	@PasswordAttemptLockoutDuration int,
	@CreatedBy int,
	@audit_workstation nvarchar(50),
	@user_admin_id int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  INSERT INTO user_admin
                         (PasswordValidPeriod, PasswordMinLength, PasswordMaxLength, PreviousPasswordsCount,maxInvalidPasswordAttempts,PasswordAttemptLockoutDuration, CreatedBy, CreatedDateTime)
VALUES        (@PasswordValidPeriod,@PasswordMinLength,@PasswordMaxLength,@PreviousPasswordsCount,@maxInvalidPasswordAttempts,@PasswordAttemptLockoutDuration,@CreatedBy,GETDATE())


set @user_admin_id=SCOPE_IDENTITY()

															
			EXEC usp_insert_audit @CreatedBy, 
									4,
									NULL, 
									@audit_workstation, 
									'created user admin settings.', 
									null, NULL, NULL, NULL

END


GO
/****** Object:  StoredProcedure [dbo].[usp_delete_connenction_params]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_connenction_params]
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
			--Log Audit
			DECLARE @audit_description varchar(max), @connection_name varchar(max)

			SELECT @connection_name = connection_name
			FROM connection_parameters
			WHERE connection_parameter_id = @connection_parameter_id 
			
			SELECT @audit_description = 'Parameter Deleted: ' + @connection_name
											+ ' , Paramter Id: ' + CAST(@connection_parameter_id AS varchar(max))
			EXEC usp_insert_audit @audit_user_id, 
						9,
						NULL, 
						@audit_workstation, 
						@audit_description, 
						NULL, NULL, NULL, NULL	


			delete from [dbo].connection_parameters
				WHERE connection_parameter_id = @connection_parameter_id
		--DECLARE @audit_description varchar(500)
		--		SELECT @audit_description = 'Deleted Connection Parameters : ' + CONVERT(NVARCHAR, @connection_parameter_id)	+ ')'			
		--		EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_delete_external_system_fields]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_external_system_fields]
	@external_system_field_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
BEGIN TRANSACTION [DELETE_EXTERNAL_SYSTEM_FIELDS]
		BEGIN TRY 

			
				BEGIN
					DELETE FROM external_system_fields
					WHERE external_system_field_id=@external_system_field_id
					
					SET @ResultCode = 0
				END


			COMMIT TRANSACTION [DELETE_EXTERNAL_SYSTEM_FIELDS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_EXTERNAL_SYSTEM_FIELDS]
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
/****** Object:  StoredProcedure [dbo].[usp_delete_external_systems]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_external_systems]
	@external_system_id INT,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
BEGIN TRANSACTION [DELETE_EXTERNAL_SYSTEMS]
		BEGIN TRY 

			--IF((SELECT COUNT(*) FROM external_system_fields WHERE external_system_id = @external_system_id) > 0)
			--	BEGIN
			--		SET @ResultCode = 803
			--	END
			--ELSE
				BEGIN
					DELETE FROM external_systems
					WHERE external_system_id=@external_system_id
					
					DELETE FROM external_system_fields
					WHERE external_system_id=@external_system_id

					SET @ResultCode = 0
				END


			COMMIT TRANSACTION [DELETE_EXTERNAL_SYSTEMS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_EXTERNAL_SYSTEMS]
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
/****** Object:  StoredProcedure [dbo].[usp_delete_fee_scheme]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_fee_scheme] 
	-- Add the parameters for the stored procedure here
	@fee_scheme_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRY
		BEGIN TRANSACTION [DELETE_MASTERKEY_TRAN]


		IF((SELECT count(*) FROM [issuer_product] WHERE fee_scheme_id = @fee_scheme_id) > 0)
			BEGIN
				SET @ResultCode = 702
			END
		ELSE IF((SELECT count(*) FROM [sub_product] WHERE fee_scheme_id = @fee_scheme_id) > 0)
			BEGIN
				SET @ResultCode = 703
			END
		ELSE 
			BEGIN
				DECLARE @audit_product_name varchar(400), @issuer_id int
				SELECT @audit_product_name = fee_scheme_name
				FROM [product_fee_scheme]
				WHERE fee_scheme_id = @fee_scheme_id

				SELECT @issuer_id = issuer_id
				FROM [product_fee_scheme]
				WHERE fee_scheme_id = @fee_scheme_id

				--Delete charges
				DELETE FROM [product_fee_charge]
				WHERE fee_detail_id IN (SELECT fee_detail_id
										FROM [product_fee_detail]
										WHERE fee_scheme_id = @fee_scheme_id)

				--Delete details
				DELETE FROM product_fee_detail
				WHERE fee_scheme_id = @fee_scheme_id

				--Delete the Scheme
				DELETE FROM product_fee_scheme
				WHERE fee_scheme_id = @fee_scheme_id

				--Log in audit trail
				DECLARE @audit_description varchar(500)
				SELECT @audit_description = 'Fee Scheme Deleted: ' + @audit_product_name
											+ ' , Fee Scheme Id: ' + CAST(@fee_scheme_id AS VARCHAR(MAX))		
																	
				EXEC usp_insert_audit @audit_user_id, 
										4,
										NULL, 
										@audit_workstation, 
										@audit_description, 
										@issuer_id, NULL, NULL, NULL	

				SET @ResultCode = 0
			END

			COMMIT TRANSACTION [DELETE_MASTERKEY_TRAN]
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_MASTERKEY_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_delete_font]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_font]
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
																	
				--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_delete_ldap]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_ldap]
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
		--		EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_delete_masterkey]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_masterkey] 
	@masterkeyid int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@resultcode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
		
		BEGIN TRY 
		
		IF((select count(*) from terminals where terminal_masterkey_id=@masterkeyid)> 0)
		BEGIN
			set @resultcode=608
		END
		ELSE 
		BEGIN
		BEGIN TRANSACTION [DELETE_MASTERKEY_TRAN]

		DECLARE @masterkey_name varchar(max) = '', @issuer_id int
		SELECT @masterkey_name = [masterkey_name], @issuer_id = issuer_id
				FROM masterkeys
				WHERE  masterkey_id=@masterkeyid

		delete dbo.masterkeys where masterkey_id=@masterkeyid
			set @resultcode=0
		
		DECLARE @audit_description varchar(max)
		
		SET @audit_description = 'MasterKey Delete: '+ COALESCE(@masterkey_name, 'UNKNOWN') 
									+ 'MasterKey Id: ' + CAST(@masterkeyid AS varchar(max))
										 	
		EXEC usp_insert_audit @audit_user_id, 
								9,
								NULL, 
								@audit_workstation, 
								@audit_description, 
								@issuer_id, NULL, NULL, NULL

		COMMIT TRANSACTION [DELETE_MASTERKEY_TRAN]
		END
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_MASTERKEY_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_delete_product]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_product]
	@productid int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@result_code int =null output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	BEGIN TRANSACTION [DELETE_PRODUCT_TRAN]
		BEGIN TRY 

			-- delete interface connections first
			DELETE FROM product_interface WHERE product_id = @productid

			
				
			-- then disable the product
			UPDATE issuer_product set DeletedYN=1 where product_id=@productid

			--Hard delete if no cards are linked to product
			IF(NOT EXISTS (SELECT product_id from cards where product_id = @productid))
			BEGIN
				DELETE FROM product_fields
				WHERE product_id = @productid

				DELETE FROM integration_cardnumbers
				WHERE product_id = @productid

					DELETE FROM product_external_system
				WHERE product_id = @productid

				DELETE FROM issuer_product
				WHERE product_id = @productid

			
				--AND NOT EXISTS (SELECT product_id from cards where product_id = @productid)
			END		


			-- then update the audit information
			DECLARE @audit_description varchar(max), @issuer_id int
			SELECT @audit_description = 'Product Deleted: ' + CAST(@productid as varchar(max))
										+' , Product Id: ' + CAST(@productid as varchar(max))

			IF EXISTS(SELECT product_id from cards where product_id = @productid)
				SET @audit_description += ' , Product soft deleted.'
			ELSE
				SET @audit_description += ' , Product hard deleted.'

			SELECT 	@issuer_id = issuer_id
			FROM [issuer_product]
			WHERE product_id = @productid
																	
			EXEC usp_insert_audit @audit_user_id, 
									4,
									NULL, 
									@audit_workstation, 
									@audit_description, 
									@issuer_id, NULL, NULL, NULL

			COMMIT TRANSACTION [DELETE_PRODUCT_TRAN]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_PRODUCT_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_delete_subproduct]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_subproduct]
@product_id int,
@sub_product_id int,
@audit_user_id bigint,
@audit_workstation varchar(100),
@ResultCode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   BEGIN TRANSACTION [DELETE_SUBPRODUCT_TRAN]
		BEGIN TRY 

			UPDATE sub_product 
			SET deleteYN=1 
			WHERE product_id=@product_id and sub_product_id=@sub_product_id

   				
		
			--log the audit record
			DECLARE @audit_description varchar(max) = '',
					@sub_product_code varchar(100),
					@sub_product_name varchar(100),
					@issuer_id int

			SELECT @issuer_id = issuer_id
			FROM [issuer_product]
			WHERE product_id = @product_id

			SELECT @sub_product_code = sub_product_code,
					@sub_product_name = sub_product_name
			FROM [sub_product]
			WHERE product_id = @product_id AND 
					sub_product_id = @sub_product_id

			SET @audit_description = 'Sub-Product Deleted: ' + @sub_product_code + ' ' + @sub_product_name	
									+ ' , Sub-Product Id: ' + CAST(@sub_product_id as varchar(max))
									+ ' , Product Id: ' + CAST(@product_id as varchar(max))
									--+ ', issuer: ' + COALESCE(@issuer_name, 'UNKNOWN') 
										 	
			EXEC usp_insert_audit @audit_user_id, 
									4,
									NULL, 
									@audit_workstation, 
									@audit_description, 
									@issuer_id, NULL, NULL, NULL	

			SET @ResultCode = 0	

			COMMIT TRANSACTION [DELETE_SUBPRODUCT_TRAN]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_SUBPRODUCT_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_delete_terminaldetails]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_terminaldetails]
	@terminalid int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@resultcode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   
	BEGIN TRANSACTION [DELETE_TERMINAL_TRAN]
		BEGIN TRY 

		DECLARE @terminal_name varchar(max) = '', @issuer_id int, @audit_branch_name varchar(max), 
						@audit_branch_code varchar(max)


		SELECT @terminal_name = [terminal_name], @issuer_id = issuer_id, @audit_branch_name = branch_name,
						@audit_branch_code = branch_code
		FROM terminals
				INNER JOIN [branch]
				ON [branch].branch_id = terminals.branch_id
		WHERE  terminal_id=@terminalid
		
		delete dbo.terminals where terminal_id=@terminalid
		set @resultcode=0

		DECLARE @audit_description varchar(max) 
		
		SET @audit_description = 'Terminal Deleted: '+ COALESCE(@terminal_name, 'UNKNOWN') 
										  + ' , Terminal Id: ' + CAST(@terminalid as varchar(max))
										  + ' , branch code: ' + @audit_branch_code
										  + ' , branch name: ' + @audit_branch_name		
										 	
			EXEC usp_insert_audit @audit_user_id, 
									9,
									NULL, 
									@audit_workstation, 
									@audit_description, 
									@issuer_id, NULL, NULL, NULL

		COMMIT TRANSACTION [DELETE_TERMINAL_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_TERMINAL_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_delete_user_group]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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









GO
/****** Object:  StoredProcedure [dbo].[usp_dist_batch_reject_production]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Reject a production batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_dist_batch_reject_production] 
	@dist_batch_id bigint,
	@status_notes varchar(150),
	@reject_card_list AS dbo.key_value_array READONLY,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [REJECT_PROD_BATCH]
		BEGIN TRY 
			
			DECLARE @current_dist_batch_status_id int,
					@audit_msg varchar(max),
					@status_date datetime = GETDATE()

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
					VALUES (@dist_batch_id, @reject_dist_batch_statuses_id, @audit_user_id, @status_date, @status_notes)

					--Update the cards linked to the prod batch with the new status.
					UPDATE dist_batch_cards
					SET dist_card_status_id = 7
					WHERE dist_batch_id = @dist_batch_id

					--Return valid cards to pool
					INSERT INTO branch_card_status
						(card_id, comments, status_date, branch_card_statuses_id, [user_id])
					SELECT [dist_batch_cards].card_id, '', @status_date, 3, @audit_user_id
					FROM [dist_batch_cards]
					WHERE dist_batch_id = @dist_batch_id AND
						[dist_batch_cards].card_id NOT IN (SELECT [key] FROM @reject_card_list)

					--Put rejected cards back to operators to do list
					INSERT INTO branch_card_status
						(card_id, comments, status_date, branch_card_statuses_id, [user_id])
					SELECT [dist_batch_cards].card_id, reject_list.value, @status_date, 11, @audit_user_id
					FROM [dist_batch_cards] 
							INNER JOIN @reject_card_list reject_list
								ON [dist_batch_cards].card_id = reject_list.[key]
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
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					SET @ResultCode = 0					
				END

				--Fetch the distribution batch with latest details
				EXEC usp_get_dist_batch @dist_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [REJECT_PROD_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [REJECT_PROD_BATCH]
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
/****** Object:  StoredProcedure [dbo].[usp_dist_batch_status_change]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Change batch status - Change
-- =============================================
CREATE PROCEDURE [dbo].[usp_dist_batch_status_change] 
	@dist_batch_id bigint,
	@dist_batch_statuses_id int,
	@new_dist_batch_statuses_id int,
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

	BEGIN TRANSACTION [BATCH_STATUS_CHANGE]
		BEGIN TRY 
			
			DECLARE @audit_msg varchar(max),
					@original_batch_type_id int,
					@new_batch_type_id int,
					@new_dist_card_statuses_id int,
					@new_branch_card_statuses_id int
						
			--If the statuses are the same then we arent changing the batches status, just return dist batch
			--IF(@dist_batch_statuses_id = @new_dist_batch_statuses_id)	
			--	BEGIN
			--		SET @ResultCode = 0	
			--	END							  
			--Check that someone hasn't already updated the dist batch
			IF(dbo.DistBatchInCorrectStatus(@dist_batch_statuses_id, @new_dist_batch_statuses_id, @dist_batch_id) = 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN	
					--Check if we need to create dist batch
					SELECT @original_batch_type_id = [dist_batch_statuses_flow].dist_batch_type_id,
							  @new_batch_type_id = flow_dist_batch_type_id,
							  @new_dist_card_statuses_id = flow_dist_card_statuses_id,
							  @new_branch_card_statuses_id = branch_card_statuses_id
						FROM [dist_batch_statuses_flow]
							INNER JOIN [dist_batch]
								ON [dist_batch_statuses_flow].card_issue_method_id = [dist_batch].card_issue_method_id
									AND [dist_batch_statuses_flow].dist_batch_type_id = [dist_batch].dist_batch_type_id
							INNER JOIN [dist_batch_status_current]
								ON [dist_batch_status_current].dist_batch_statuses_id = [dist_batch_statuses_flow].dist_batch_statuses_id
									AND [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
					WHERE [dist_batch].dist_batch_id = @dist_batch_id
					

					--Update the batch status.
					INSERT [dist_batch_status]
							([dist_batch_id], [dist_batch_statuses_id], [user_id], [status_date], [status_notes])
					VALUES (@dist_batch_id, @new_dist_batch_statuses_id, @audit_user_id, GETDATE(), @status_notes)


					--Receiving at card centre, update originating batch to batchid
					IF(@new_dist_batch_statuses_id = 14)
					BEGIN
						UPDATE [cards]
						SET origin_branch_id = [cards].branch_id
						FROM [cards]
							INNER JOIN [dist_batch_cards]
								ON [cards].card_id = [dist_batch_cards].card_id
						WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id
					END


					--Check if we need to update the card status
					IF (@new_dist_card_statuses_id IS NOT NULL)
					BEGIN 
						--Update the cards linked to the dist batch with the new status.
						UPDATE dist_batch_cards
						SET dist_card_status_id = @new_dist_card_statuses_id
						WHERE dist_batch_id = @dist_batch_id

						--Reject status, make sure cards are back at originating branch
						IF(@new_dist_card_statuses_id = 18 OR @new_dist_card_statuses_id = 11 OR
							@new_dist_card_statuses_id = 7)
							BEGIN
								UPDATE [cards]
								SET branch_id = [cards].origin_branch_id
								FROM [cards]
									INNER JOIN [dist_batch_cards]
										ON [cards].card_id = [dist_batch_cards].card_id
								WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id 

								IF(@new_branch_card_statuses_id = 0)
								BEGIN
									INSERT INTO [branch_card_status] (branch_card_statuses_id, card_id, status_date, [user_id])
									SELECT @new_branch_card_statuses_id, [dist_batch_cards].card_id, GETDATE(), @audit_user_id
									FROM [dist_batch_cards]
									WHERE dist_batch_id = @dist_batch_id
								END
							END						
					END
					
					--Do we need to create a distribution batch from a production batch
					IF(@original_batch_type_id = 0 AND @new_batch_type_id = 1)
					BEGIN
						EXEC usp_prod_to_dist_batch @dist_batch_id, @audit_user_id, @audit_workstation
					END

					--PINS_PRINTED check if we need to create pin mailer batch
					IF (@new_dist_batch_statuses_id = 18)
					BEGIN
						EXEC usp_prod_to_pin @dist_batch_id, @audit_user_id, @audit_workstation
					END

					--RECEIVED_AT_BRANCH needs to add cards to branch_card_status
					IF (@new_dist_batch_statuses_id = 3)
					BEGIN
						EXEC usp_dist_batch_to_vault @dist_batch_id, @audit_user_id, @audit_workstation

						--Check if the cards should be changed to another status, overriding previous sp.
						IF(@new_branch_card_statuses_id IS NOT NULL)
						BEGIN
							INSERT INTO [branch_card_status] (branch_card_statuses_id, card_id, status_date, [user_id])
							SELECT @new_branch_card_statuses_id, [dist_batch_cards].card_id, DATEADD(s, 1,GETDATE()), @audit_user_id
							FROM [dist_batch_cards]
							WHERE dist_batch_id = @dist_batch_id
						END
					END

					--AUDIT 
					DECLARE @batch_status_name varchar(100),
							@batch_ref varchar(100)

					SELECT @batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = @new_dist_batch_statuses_id

					SELECT @batch_ref = dist_batch_reference
					FROM dist_batch
					WHERE dist_batch_id = @dist_batch_id

					--Add audit for pin batch update								
					SET @audit_msg = 'Update: ' + CAST(@dist_batch_id AS varchar(max)) +
										', ' + COALESCE(@batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					 

					SET @ResultCode = 0					
				END

				--Fetch the batch with latest details
				EXEC usp_get_dist_batch @dist_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [BATCH_STATUS_CHANGE]
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
/****** Object:  StoredProcedure [dbo].[usp_dist_batch_status_reject]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Change batch status - Reject
-- =============================================
CREATE PROCEDURE [dbo].[usp_dist_batch_status_reject] 
	@dist_batch_id bigint,
	@new_dist_batch_status_id int,
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

	BEGIN TRANSACTION [BATCH_STATUS_CHANGE_REJECT]
		BEGIN TRY 
			
			DECLARE @audit_msg varchar(max),
					@new_dist_card_statuses_id int,
					@new_branch_card_statuses_id int

			--Check that someone hasn't already updated the dist batch
			IF(dbo.DistBatchInCorrectStatusReject(@new_dist_batch_status_id, @dist_batch_id) = 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN
					SELECT @new_dist_card_statuses_id = reject_dist_card_statuses_id,
							@new_branch_card_statuses_id = reject_branch_card_statuses_id
						FROM [dist_batch_statuses_flow]
							INNER JOIN [dist_batch]
								ON [dist_batch_statuses_flow].card_issue_method_id = [dist_batch].card_issue_method_id
									AND [dist_batch_statuses_flow].dist_batch_type_id = [dist_batch].dist_batch_type_id
							INNER JOIN [dist_batch_status_current]
								ON [dist_batch_status_current].dist_batch_statuses_id = [dist_batch_statuses_flow].dist_batch_statuses_id
									AND [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
					WHERE [dist_batch].dist_batch_id = @dist_batch_id
							AND [dist_batch_statuses_flow].reject_dist_batch_statuses_id = @new_dist_batch_status_id


					--Update the batch status.
					INSERT [dist_batch_status]
							([dist_batch_id], [dist_batch_statuses_id], [user_id], [status_date], [status_notes])
					VALUES (@dist_batch_id, @new_dist_batch_status_id, @audit_user_id, GETDATE(), @status_notes)

					--Check if we need to update the card status
					IF (@new_dist_card_statuses_id IS NOT NULL)
					BEGIN 
						--Update the cards linked to the dist batch with the new status.
						UPDATE dist_batch_cards
						SET dist_card_status_id = @new_dist_card_statuses_id
						WHERE dist_batch_id = @dist_batch_id

						--Going back to checked in or destroying, make sure cards are back at originating branch
						IF(@new_dist_card_statuses_id = 18 OR @new_dist_card_statuses_id = 11)
						BEGIN
							UPDATE [cards]
							SET branch_id = [cards].origin_branch_id
							FROM [cards]
								INNER JOIN [dist_batch_cards]
									ON [cards].card_id = [dist_batch_cards].card_id
							WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id 
						END
					END

					IF(@new_branch_card_statuses_id IS NOT NULL)
					BEGIN
						INSERT INTO [branch_card_status] (branch_card_statuses_id, card_id, status_date, [user_id])
						SELECT @new_branch_card_statuses_id, [dist_batch_cards].card_id, GETDATE(), @audit_user_id
						FROM [dist_batch_cards]
						WHERE dist_batch_id = @dist_batch_id
						
					END
				
					--AUDIT
					DECLARE @batch_status_name varchar(100),
							@batch_ref varchar(100)

					SELECT @batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = @new_dist_batch_status_id

					SELECT @batch_ref = dist_batch_reference
					FROM dist_batch
					WHERE dist_batch_id = @dist_batch_id

					--Add audit for pin batch update								
					SET @audit_msg = 'Update: ' + CAST(@dist_batch_id AS varchar(max)) +
										', ' + COALESCE(@batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					SET @ResultCode = 0					
				END

				--Fetch the batch with latest details
				EXEC usp_get_dist_batch @dist_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [BATCH_STATUS_CHANGE_REJECT]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [BATCH_STATUS_CHANGE_REJECT]
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
/****** Object:  StoredProcedure [dbo].[usp_dist_batch_to_vault]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_dist_batch_to_vault] 
	-- Add the parameters for the stored procedure here
	@dist_batch_id bigint, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

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

	--Update the originating branch to the current branch.
	UPDATE [cards]
	SET origin_branch_id = [cards].branch_id
	FROM [cards]
		INNER JOIN [dist_batch_cards]
			ON [cards].card_id = [dist_batch_cards].card_id
	WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id 

END






GO
/****** Object:  StoredProcedure [dbo].[usp_export_batch_status_approve]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_export_batch_status_approve]
	-- Add the parameters for the stored procedure here
	@export_batch_id bigint,
	@is_auto_approval bit = 0,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [EXPORT_BATCH_APPROVE]
		BEGIN TRY 
			
			DECLARE @audit_msg varchar(max),
					@current_export_batch_id int,
					@new_export_batch_statuses_id int = 1

			SELECT @current_export_batch_id = export_batch_statuses_id FROM [export_batch_status_current] WHERE export_batch_id = @export_batch_id
						
						  
			--Check that someone hasn't already updated the dist batch
			IF(@current_export_batch_id = 0 OR @current_export_batch_id = 3)
				BEGIN	

					DECLARE @auditDatetime DATETIME2 = SYSDATETIME()

					IF(@is_auto_approval = 1)
						SET @auditDatetime = DATEADD(NANOSECOND, 100, @auditDatetime)

					INSERT INTO export_batch_status (export_batch_id, export_batch_statuses_id, status_date, [user_id], comments)
					VALUES (@export_batch_id, @new_export_batch_statuses_id, @auditDatetime, @audit_user_id, 'BATCH APPROVED')
					

					--AUDIT 
					DECLARE @batch_status_name varchar(100),
							@batch_ref varchar(100)

					SELECT @batch_status_name =  export_batch_statuses_name
					FROM export_batch_statuses
					WHERE export_batch_statuses_id = @new_export_batch_statuses_id

					SELECT @batch_ref = batch_reference
					FROM export_batch
					WHERE export_batch_id = @export_batch_id

					--Add audit for pin batch update								
					SET @audit_msg = 'Update: ' + CAST(@export_batch_id AS varchar(max)) +
										', ' + COALESCE(@batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											11,
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

				--Fetch the batch with latest details
				EXEC usp_get_export_batch @export_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [EXPORT_BATCH_APPROVE]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [EXPORT_BATCH_APPROVE]
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
/****** Object:  StoredProcedure [dbo].[usp_export_batch_status_exported]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_export_batch_status_exported] 
	-- Add the parameters for the stored procedure here
	@export_batch_id bigint,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [EXPORT_BATCH_EXPORTED]
		BEGIN TRY 
			
			DECLARE @audit_msg varchar(max),
					@new_export_batch_statuses_id int = 2
						
						  
			--Check that someone hasn't already updated the dist batch
			IF((SELECT export_batch_statuses_id FROM [export_batch_status_current] WHERE export_batch_id = @export_batch_id) != 1)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN	

					INSERT INTO export_batch_status (export_batch_id, export_batch_statuses_id, status_date, [user_id], comments)
					VALUES (@export_batch_id, @new_export_batch_statuses_id, SYSDATETIME(), @audit_user_id, 'BATCH EXPORTED')
					

					--AUDIT 
					DECLARE @batch_status_name varchar(100),
							@batch_ref varchar(100)

					SELECT @batch_status_name =  export_batch_statuses_name
					FROM export_batch_statuses
					WHERE export_batch_statuses_id = @new_export_batch_statuses_id

					SELECT @batch_ref = batch_reference
					FROM export_batch
					WHERE export_batch_id = @export_batch_id

					--Add audit for pin batch update								
					SET @audit_msg = 'Update: ' + CAST(@export_batch_id AS varchar(max)) +
										', ' + COALESCE(@batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											11,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					 

					SET @ResultCode = 0					
				END

				--Fetch the batch with latest details
				EXEC usp_get_export_batch @export_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [EXPORT_BATCH_EXPORTED]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [EXPORT_BATCH_EXPORTED]
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
/****** Object:  StoredProcedure [dbo].[usp_export_batch_status_reject]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_export_batch_status_reject]
	-- Add the parameters for the stored procedure here
	@export_batch_id bigint,
	@comments varchar(100),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [EXPORT_BATCH_REJECT]
		BEGIN TRY 
			
			DECLARE @audit_msg varchar(max),
					@current_export_batch_id int,
					@new_export_batch_statuses_id int = 4

			SELECT @current_export_batch_id = export_batch_statuses_id FROM [export_batch_status_current] WHERE export_batch_id = @export_batch_id
						
						  
			--Check that someone hasn't already updated the dist batch
			IF(@current_export_batch_id = 0 OR @current_export_batch_id = 3)
				BEGIN	

					DECLARE @auditDatetime DATETIME2 = SYSDATETIME()

					INSERT INTO export_batch_status (export_batch_id, export_batch_statuses_id, status_date, [user_id], comments)
					VALUES (@export_batch_id, @new_export_batch_statuses_id, @auditDatetime, @audit_user_id, @comments)
					

					--AUDIT 
					DECLARE @batch_status_name varchar(100),
							@batch_ref varchar(100)

					SELECT @batch_status_name =  export_batch_statuses_name
					FROM export_batch_statuses
					WHERE export_batch_statuses_id = @new_export_batch_statuses_id

					SELECT @batch_ref = batch_reference
					FROM export_batch
					WHERE export_batch_id = @export_batch_id

					--Add audit for pin batch update								
					SET @audit_msg = 'Update: ' + CAST(@export_batch_id AS varchar(max)) +
										', ' + COALESCE(@batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											11,
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

				--Fetch the batch with latest details
				EXEC usp_get_export_batch @export_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [EXPORT_BATCH_REJECT]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [EXPORT_BATCH_REJECT]
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
/****** Object:  StoredProcedure [dbo].[usp_export_batch_status_request]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_export_batch_status_request]
	-- Add the parameters for the stored procedure here
	@export_batch_id bigint,
	@comments varchar(100),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [EXPORT_BATCH_REQUEST]
		BEGIN TRY 
			
			DECLARE @audit_msg varchar(max),
					@current_export_batch_id int,
					@new_export_batch_statuses_id int = 3

			SELECT @current_export_batch_id = export_batch_statuses_id FROM [export_batch_status_current] WHERE export_batch_id = @export_batch_id
						
						  
			--Check that someone hasn't already updated the dist batch
			IF(@current_export_batch_id = 2 OR @current_export_batch_id = 4)
				BEGIN	

					DECLARE @auditDatetime DATETIME2 = SYSDATETIME()

					INSERT INTO export_batch_status (export_batch_id, export_batch_statuses_id, status_date, [user_id], comments)
					VALUES (@export_batch_id, @new_export_batch_statuses_id, @auditDatetime, @audit_user_id, @comments)
					

					--AUDIT 
					DECLARE @batch_status_name varchar(100),
							@batch_ref varchar(100)

					SELECT @batch_status_name =  export_batch_statuses_name
					FROM export_batch_statuses
					WHERE export_batch_statuses_id = @new_export_batch_statuses_id

					SELECT @batch_ref = batch_reference
					FROM export_batch
					WHERE export_batch_id = @export_batch_id

					--Add audit for pin batch update								
					SET @audit_msg = 'Update: ' + CAST(@export_batch_id AS varchar(max)) +
										', ' + COALESCE(@batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											11,
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

				--Fetch the batch with latest details
				EXEC usp_get_export_batch @export_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [EXPORT_BATCH_REQUEST]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [EXPORT_BATCH_REQUEST]
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
/****** Object:  StoredProcedure [dbo].[usp_file_load_create]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Insert new file load
-- =============================================
CREATE PROCEDURE [dbo].[usp_file_load_create] 
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
/****** Object:  StoredProcedure [dbo].[usp_file_load_update]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Insert new file load
-- =============================================
CREATE PROCEDURE [dbo].[usp_file_load_update]
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
/****** Object:  StoredProcedure [dbo].[usp_finalise_login]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 3 March 2014
-- Description:	This stored procedure will fanalise the login by updating last login, workstation etc.
-- =============================================
CREATE PROCEDURE [dbo].[usp_finalise_login] 
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
			EXEC usp_insert_audit @user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_finalise_login_failed]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Selebalo Setenane
-- Create date: 11 March 2014
-- Description:	This stored procedure will fanalise the failed login by updating last login, workstation etc.
-- =============================================

CREATE PROCEDURE [dbo].[usp_finalise_login_failed] 
    @user_id bigint,    
	@audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [FINALISE_LOGIN_FAILED_TRAN]
		BEGIN TRY
			
			DECLARE @incorrect_login_attempts int,
					 @maxInvalidPasswordAttempts int,
					 @PasswordAttemptLockoutDuration int,
					 @uer_status_id int
			DECLARE @last_login_attempt datetime

			--Fetch number of incorrect login attempts
			SELECT @incorrect_login_attempts = number_of_incorrect_logins,@uer_status_id=user_status_id,@last_login_attempt=last_login_attempt
			FROM [user]
			WHERE [user].[user_id] = @user_id


			SELECT @maxInvalidPasswordAttempts = maxInvalidPasswordAttempts,@PasswordAttemptLockoutDuration=PasswordAttemptLockoutDuration
			FROM [user_admin]

			--and @uer_status_id = 0 and
			if( DATEADD(hour,@PasswordAttemptLockoutDuration,@last_login_attempt) >= GETDATE())
			BEGIN
				if(@incorrect_login_attempts >= @maxInvalidPasswordAttempts)
					BEGIN
						UPDATE [user] 
							SET [user_status_id]=3 
							WHERE [user].[user_id] = @user_id
					END
				
				--Update last login and increment the incorrect login attempts
					UPDATE [user]
					SET last_login_attempt = GETDATE(),
						number_of_incorrect_logins = (@incorrect_login_attempts + 1)
					WHERE [user].[user_id] = @user_id

			END

			ELSE
			BEGIN
			UPDATE [user] 
							SET [user_status_id]=0 ,number_of_incorrect_logins=1,last_login_attempt = GETDATE()
							WHERE [user].[user_id] = @user_id
			END
			--log the audit record
			DECLARE @audit_description varchar(500)
			SELECT @audit_description = 'Login failed'			
			EXEC usp_insert_audit @user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_finalise_logout]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Update user logout status
-- =============================================
CREATE PROCEDURE [dbo].[usp_finalise_logout] 
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
						EXEC usp_insert_audit @audit_user_id, 
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

						EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_find_distinct_load_cards]    Script Date: 2016-04-05 09:55:42 AM ******/
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
CREATE PROCEDURE [dbo].[usp_find_distinct_load_cards] 
	-- Add the parameters for the stored procedure here
	@card_list dbo.DistBatchCards READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT DISTINCT [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4)
		FROM [cards] 
			INNER JOIN [load_batch_cards]
				ON [cards].card_id = [load_batch_cards].card_id
			INNER JOIN [load_batch]
				ON [load_batch_cards].load_batch_id = [load_batch].load_batch_id
			INNER JOIN [load_batch_status_current]
				ON [load_batch_status_current].load_batch_id = [load_batch].load_batch_id
		WHERE [load_batch_status_current].load_batch_statuses_id NOT IN (2, 3)
			  AND CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_number)) IN (SELECT cardlist.card_number
																		 FROM @card_list cardlist )
																		
      
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END











GO
/****** Object:  StoredProcedure [dbo].[usp_find_distinct_load_requests]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 21 October 2015
-- Description:	Find all distinct card references from load cards based on supplied card reference list
--				and returns the matched cards.
--				Cards that are attached to a load batch which has been rejected are not considered
--				duplicates in the system as they are not active. When creating a new load batch
--				a merege on the card table will happen to stop duplicate card number from entereing
--				the system.
-- =============================================
CREATE PROCEDURE [dbo].[usp_find_distinct_load_requests] 
	-- Add the parameters for the stored procedure here
	@card_ref_list dbo.key_value_array READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT DISTINCT card_request_reference
	FROM [cards] 
		INNER JOIN [load_batch_cards]
			ON [cards].card_id = [load_batch_cards].card_id
		INNER JOIN [load_batch]
			ON [load_batch_cards].load_batch_id = [load_batch].load_batch_id
		INNER JOIN [load_batch_status_current]
			ON [load_batch_status_current].load_batch_id = [load_batch].load_batch_id
	WHERE [load_batch_status_current].load_batch_statuses_id NOT IN (2, 3)
			AND [cards].card_request_reference IN (SELECT cardReflist.value
													FROM @card_ref_list cardReflist )

END











GO
/****** Object:  StoredProcedure [dbo].[usp_find_file_info_by_filename]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchey
-- Create date: 5 March 2014
-- Description:	Find file info based on filename.
-- =============================================
CREATE PROCEDURE [dbo].[usp_find_file_info_by_filename]
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
/****** Object:  StoredProcedure [dbo].[usp_find_issuer_by_status]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 5 March 2014
-- Description:	Find issuers based on status
-- =============================================
CREATE PROCEDURE [dbo].[usp_find_issuer_by_status] 
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
/****** Object:  StoredProcedure [dbo].[usp_find_reissue_cards]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 28 March 2014
-- Description:	Search for card/s based on input parameters
-- =============================================
CREATE PROCEDURE [dbo].[usp_find_reissue_cards] 
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
					   dbo.MaskString(CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_number)), 6,4) AS card_number,
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
					INNER JOIN [issuer_product]
						ON [issuer_product].product_id =[cards].product_id 
				WHERE [branch].branch_status_id = 0	 
					  AND [issuer].issuer_status_id = 0
					  AND (([issuer_product].pin_mailer_printing_YN = 0 AND [branch_card_status_current].branch_card_statuses_id = 4)
							OR
						    ([issuer_product].pin_mailer_printing_YN = 1 AND [branch_card_status_current].branch_card_statuses_id = 5))

			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY card_number ASC

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

			--log the audit record		
			--EXEC usp_insert_audit @user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_generate_audit_report]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_generate_audit_report]
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
/****** Object:  StoredProcedure [dbo].[usp_get_all_countries]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_all_countries] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_all_issuers]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo Setenane
-- Create date: 2014/03/19
-- Description:	Returns all the issuers and their interfaces
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_all_issuers]
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
/****** Object:  StoredProcedure [dbo].[usp_get_all_issuers_for_role]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_all_issuers_for_role] 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT 
		[issuer].issuer_id
		, [issuer].issuer_name
		, [issuer].issuer_code
		, [issuer].maker_checker_YN		
		, [issuer].classic_card_issue_YN
		, [issuer].instant_card_issue_YN
		, [issuer].card_ref_preference
		, [issuer].enable_instant_pin_YN
		, [issuer].authorise_pin_issue_YN
		, [issuer].authorise_pin_reissue_YN		
		, [issuer].back_office_pin_auth_YN
		,CAST(CASE WHEN 1 = ANY (SELECT [issuer_product].account_validation_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id)
				THEN 1 ELSE 0 END as BIT) AS account_validation_YN
		,CAST(CASE WHEN 1 = ANY (SELECT [issuer_product].auto_approve_batch_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id)
				THEN 1 ELSE 0 END as BIT) AS auto_create_dist_batch
		,CAST(CASE WHEN 1 = ANY (SELECT [issuer_product].pin_mailer_printing_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id)
				THEN 1 ELSE 0 END as BIT) AS pin_mailer_printing_YN
		,CAST(CASE WHEN 1 = ANY (SELECT [issuer_product].pin_mailer_reprint_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id)
				THEN 1 ELSE 0 END as BIT) AS pin_mailer_reprint_YN
		,CAST(CASE WHEN 0 <> ANY (SELECT [issuer_product].product_load_type_id FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id)
				THEN 1 ELSE 0 END as BIT) AS EnableCardFileLoader_YN

	FROM [issuer]
	WHERE issuer_status_id = 0
END










GO
/****** Object:  StoredProcedure [dbo].[usp_get_auditdata]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE PROCEDURE [dbo].[usp_get_auditdata]
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
				SELECT [audit_control].audit_id,[audit_control].action_description,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].username)) as 'UserName', CONVERT(VARCHAR(10),audit_date,103) as audit_date,CONVERT(VARCHAR(30),audit_date,108) as 'audit_Time', [workstation_address],
					  [audit_action].audit_action_name, [audit_control].[data_before],[audit_control].[data_after],
					   [audit_control].issuer_id 
				FROM [audit_control]
						INNER JOIN [audit_action] 
							ON [audit_control].audit_action_id = [audit_action].audit_action_id
						INNER JOIN [user]
							ON [user].[user_id] = [audit_control].[user_id]												
				WHERE [audit_control].audit_action_id = COALESCE(@audit_action_id, [audit_control].audit_action_id)
					AND audit_date BETWEEN @date_from AND DATEADD(day, 1, @date_to)					
					AND ((@username IS NULL) OR (CONVERT(VARCHAR(max),DECRYPTBYKEY([user].username)) LIKE @username))
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
/****** Object:  StoredProcedure [dbo].[usp_get_authenticationtypes]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_authenticationtypes]
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	 SELECT        auth_type_id, auth_type_name
FROM            auth_type
    
END


GO
/****** Object:  StoredProcedure [dbo].[usp_get_authpin_by_user_id]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Lebo Tladi
-- Create date:
-- Description:	Returns decrypted instant authorisation pin
-- =============================================
 CREATE PROCEDURE [dbo].[usp_get_authpin_by_user_id] 
	@username varchar(200),
	@branch_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate;

			SELECT [user].[user_id],
					CONVERT(VARCHAR(max),DECRYPTBYKEY([user].instant_authorisation_pin)) as 'instant_authorisation_pin'
			FROM [user_roles_branch]
				INNER JOIN [user]
				ON [user_roles_branch].[user_id] = [user].[user_id]
					AND [user_roles_branch].user_role_id = 2
					AND [user_roles_branch].branch_id = @branch_id
					AND [user].user_status_id = 0
				INNER JOIN [branch]
					ON [branch].branch_id = [user_roles_branch].branch_id
						AND [branch].branch_status_id = 0
				INNER JOIN [issuer]
					ON [issuer].issuer_id = [branch].issuer_id
						AND [issuer].issuer_status_id = 0
			WHERE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].username)) = @username

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_bank_codes]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi
-- Create date: 20150401	
-- Description:	Get bank codes
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_bank_codes]
	@issuer_id INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
		bank_code 
	FROM 
		rswitch_crf_bank_codes 
	WHERE 
		issuer_id = @issuer_id
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_brachesperusergroup_auditreport]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_brachesperusergroup_auditreport] 
@issuer_id int=null,
@user_group_id int =null,
@branch_id int =null,
@user_role_id int=null,
@audit_user_id bigint,
@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if(@issuer_id=-1 or @issuer_id=0)
	set @issuer_id=null

(SELECT  distinct      ug.user_group_name,issuer.issuer_code+'-'+ issuer.issuer_name as 'issuer_name', ug.mask_screen_pan, ug.mask_report_pan, ug.can_read, ug.can_update, ug.can_delete, ug.all_branch_access, 
                         case when all_branch_access=1 then 'ALL' else branch.branch_code+'-'+ branch.branch_name end as 'branch_name', '' as user_role, '' as username, ug.can_create
FROM            user_group AS ug left join  user_groups_branches on
						ug.user_group_id = user_groups_branches.user_group_id
						left join branch on branch.branch_id=user_groups_branches.branch_id
						inner join
                         issuer ON ug.issuer_id = issuer.issuer_id 
						 INNER JOIN
                         user_roles ON ug.user_role_id = user_roles.user_role_id
	 WHERE ug.issuer_id = COALESCE(@issuer_id, ug.issuer_id)
			and ug.user_group_id =COALESCE(@user_group_id, ug.user_group_id)
			--and branch.branch_id=COALESCE(@branch_id, branch.branch_id)
				and ug.user_role_id=COALESCE(@user_role_id, ug.user_role_id) and all_branch_access=1
				)
	union
	
	(SELECT  distinct      ug.user_group_name, issuer.issuer_code+'-'+ issuer.issuer_name as 'issuer_name', ug.mask_screen_pan, ug.mask_report_pan, ug.can_read, ug.can_update, ug.can_delete, ug.all_branch_access, 
                         case when all_branch_access=1 then 'ALL' else branch.branch_code+'-'+ branch.branch_name end as 'branch_name', '' as user_role, '' as username, ug.can_create
FROM            user_group AS ug 
						inner join  user_groups_branches on
						ug.user_group_id = user_groups_branches.user_group_id
						inner join branch on branch.branch_id=user_groups_branches.branch_id
						inner join
                         issuer ON ug.issuer_id = issuer.issuer_id 
						 INNER JOIN
                         user_roles ON ug.user_role_id = user_roles.user_role_id
						 
	 WHERE ug.issuer_id = COALESCE(@issuer_id, ug.issuer_id)
			and ug.user_group_id =COALESCE(@user_group_id, ug.user_group_id)
			and branch.branch_id=COALESCE(@branch_id, branch.branch_id)
				and ug.user_role_id=COALESCE(@user_role_id, ug.user_role_id)
	)

	--ORDER BY 	issuer.issuer_name,ug.user_group_name, branch_name
  
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_branch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_get_branch]
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
/****** Object:  StoredProcedure [dbo].[usp_get_branch_by_id]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 17 March 2014
-- Description:	Get a branch by its Id.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_branch_by_id] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_branch_card_count]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get the card count for a branch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_branch_card_count] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_branchcardstock_report]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [usp_get_branchcardstock_report] null,3,0,2,'veneka'

CREATE PROCEDURE [dbo].[usp_get_branchcardstock_report]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(@issuer_id = -1 or @issuer_id = 0)
	 set @issuer_id=null

	if(@branch_id  =0)
		set @branch_id = null

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)

    -- Insert statements for procedure here
	 OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT distinct 
		[branch].branch_id,branch.branch_code
		,CASE 
			WHEN @mask_report = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
			ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
		  END AS 'card_number'
		, [cards].card_request_reference as 'card_request_reference'
		, Convert(datetime, CONVERT(varchar(max),DECRYPTBYKEY([cards].card_expiry_date))) as 'card_expiry_date'
		, Convert(datetime, [dist_batch].date_created,103) as'card_production_date'
	FROM 
		branch_card_status_current
		INNER JOIN [cards] 
			ON branch_card_status_current.card_id = [cards].card_id
		INNER JOIN [dist_batch_cards]
			ON [cards].card_id = [dist_batch_cards].card_id
		INNER JOIN [dist_batch]
			ON [dist_batch_cards].dist_batch_id = [dist_batch].dist_batch_id
					AND [dist_batch].dist_batch_type_id = 0
		INNER JOIN [branch] 
			ON [cards].branch_id = [branch].branch_id
				AND [branch].card_centre_branch_YN = 0
		INNER JOIN [issuer] 
			ON issuer.issuer_id = branch.issuer_id
	WHERE
		(branch_card_status_current.branch_card_statuses_id = 1 
			OR branch_card_status_current.branch_card_statuses_id = 0)
		AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
		AND [branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)	
	ORDER BY
		[branch].branch_id
		
			
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_branches_for_issuer]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get a branchs by issuer Id.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_branches_for_issuer] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@card_centre_branch bit = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT  *
	FROM branch
	WHERE issuer_id = @issuer_id
			AND branch_status_id = 0
			AND card_centre_branch_YN = COALESCE(@card_centre_branch, card_centre_branch_YN)
END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_branches_for_user]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 12 March 2014
-- Description:	Fetch all branches by role, issuer and user
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_branches_for_user] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = NULL, 
	@user_id bigint,
	@user_role_id int = NULL,
	@card_centre_branch_YN bit = null,
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
					AND [branch].card_centre_branch_YN = COALESCE(@card_centre_branch_YN, [branch].card_centre_branch_YN)
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
				  AND [branch].card_centre_branch_YN = COALESCE(@card_centre_branch_YN, [branch].card_centre_branch_YN)
				  AND bsl.language_id=@languageId
		  END
END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_branches_for_user_admin]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Description:	Fetch all branches by issuer and user for use on branch admin screen.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_branches_for_user_admin] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_branches_for_user_group]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get a list of branches for a user group.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_branches_for_user_group] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_branches_for_userroles]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 12 March 2014
-- Description:	Fetch all branches by role, issuer and user
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_branches_for_userroles] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = NULL, 
	@user_id bigint,	
	@roles_list AS dbo.key_value_array READONLY,
	@card_centre bit = NULL,
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
		   [user_group].user_role_id IN (SELECT [key] FROM @roles_list)
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
					AND [branch].card_centre_branch_YN = COALESCE(@card_centre, [branch].card_centre_branch_YN)
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
				[user_roles_branch].[user_id] = @user_id
				   AND [user_roles_branch].user_role_id IN (SELECT [key] FROM @roles_list)
				   AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				   AND [branch].branch_status_id = 0
				   AND [branch].card_centre_branch_YN = COALESCE(@card_centre, [branch].card_centre_branch_YN)
				   AND bsl.language_id=@languageId
		  END
END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_branches_with_load_card_count]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetches all branches for a user as well as total available cards for distribution batch creation
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_branches_with_load_card_count]
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
/****** Object:  StoredProcedure [dbo].[usp_get_burnrate_report]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [usp_get_burnrate_report] null,3,0,null
CREATE PROCEDURE [dbo].[usp_get_burnrate_report]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int,
	@REPORT_DATE datetime,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
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

		DECLARE  @WEEK_BEGINING VARCHAR(10)
--SELECT @REPORT_DATE = '2004-09-21T00:00:00'
SELECT @WEEK_BEGINING = 'MONDAY'

set @REPORT_DATE = convert(datetime, getdate(), 126)

IF @WEEK_BEGINING = 'MONDAY' 
    SET DATEFIRST 1 
ELSE IF @WEEK_BEGINING = 'TUESDAY' 
    SET  DATEFIRST 2 
ELSE IF @WEEK_BEGINING = 'WEDNESDAY'
    SET  DATEFIRST 3 
ELSE IF @WEEK_BEGINING =  'THURSDAY'
    SET  DATEFIRST 4 
ELSE IF @WEEK_BEGINING =  'FRIDAY'
    SET  DATEFIRST 5 
ELSE IF @WEEK_BEGINING =  'SATURDAY'
    SET  DATEFIRST 6 
ELSE IF @WEEK_BEGINING =  'SUNDAY'
    SET  DATEFIRST 7 


DECLARE @WEEK_START_DATE DATETIME, @WEEK_END_DATE DATETIME
--GET THE WEEK START DATE
set  @WEEK_START_DATE = @REPORT_DATE - (DATEPART(DW,  @REPORT_DATE) - 1) 

--GET THE WEEK END DATE
set  @WEEK_END_DATE = @REPORT_DATE + (7 - DATEPART(DW,  @REPORT_DATE))

select interx.branch_id, interx.branch_code , sum(interx.oneeighty_d) as '180 d' ,sum(interx.oneeighty_d) as '90 d',sum(interx.Week_one) as 'Week 1', sum(interx.Week_two) as 'Week 2',sum(interx.Week_three) as 'Week 3',sum(interx.Week_four) as 'Week 4',sum(interx.Current_Card_Stock) as 'Current Card Stock',sum(interx.Weeks_remaining) as 'Weeks remaining'

from (
SELECT distinct [branch].branch_id, branch.branch_code ,count(branch_card_status.card_id) as 'oneeighty_d',0 as 'ninty_d',0 as 'Week_one', 0 as 'Week_two',0  as 'Week_three',0  as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
		FROM [cards]
		INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
		INNER JOIN [customer_account] 	ON [customer_account].card_id = branch_card_status.card_id
		INNER JOIN [branch] ON [cards].branch_id = [branch].branch_id
			WHERE branch_card_status.branch_card_statuses_id = 6 and 
			[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)	
			AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  AND branch_card_status.[status_date] <=  @REPORT_DATE
				  AND branch_card_status.[status_date] >= DATEADD(d, -180,@REPORT_DATE)
				  group by [branch].branch_id, branch.branch_code 

union

SELECT distinct [branch].branch_id, branch.branch_code , 0 as 'oneeighty_d',count(branch_card_status.card_id) as 'ninty_d',0 as 'Week_one', 0 as 'Week_two',0  as 'Week_three',0  as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [cards]
			INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
			INNER JOIN [customer_account] 	ON [customer_account].card_id = branch_card_status.card_id
			INNER JOIN [branch] 	ON [cards].branch_id = [branch].branch_id
			WHERE branch_card_status.branch_card_statuses_id = 6 and
			[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)		
			 AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  AND branch_card_status.[status_date] <=  @REPORT_DATE
				  AND branch_card_status.[status_date] >= DATEADD(d, -90,@REPORT_DATE)
				  group by [branch].branch_id, branch.branch_code 

union 
SELECT distinct [branch].branch_id, branch.branch_code , 0 as 'oneeighty_d',0 as 'ninty_d', count(branch_card_status.card_id) as 'Week_one', 0 as 'Week_two',0  as 'Week_three',0  as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [cards]
			INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
			INNER JOIN [customer_account] 	ON [customer_account].card_id = branch_card_status.card_id
			INNER JOIN [branch]		ON [cards].branch_id = [branch].branch_id
			WHERE branch_card_status.branch_card_statuses_id = 6 and 
			[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)		
			 AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  AND branch_card_status.[status_date] >=  @WEEK_START_DATE
				  AND branch_card_status.[status_date] <=  @WEEK_END_DATE
				  group by [branch].branch_id, branch.branch_code 

union 
SELECT distinct [branch].branch_id, branch.branch_code , 0 as 'oneeighty_d',0 as 'ninty_d', 0  as 'Week_one', count(branch_card_status.card_id)   as 'Week_two',0  as 'Week_three',0  as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [cards]
		INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
		INNER JOIN [customer_account] 	ON [customer_account].card_id = branch_card_status.card_id
        INNER JOIN [branch] ON [cards].branch_id = [branch].branch_id
			WHERE branch_card_status.branch_card_statuses_id = 6  
			and [branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)		
			AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  AND branch_card_status.[status_date] >= DATEADD(d, -7,@WEEK_START_DATE)
				  AND branch_card_status.[status_date] <= DATEADD(d, -1,@WEEK_START_DATE)
				  group by [branch].branch_id, branch.branch_code 

union 
SELECT distinct [branch].branch_id, branch.branch_code , 0 as 'oneeighty_d',0 as 'ninty_d', 0  as 'Week_one',0 as  'Week_two',count(branch_card_status.card_id)  as 'Week_three',0  as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [cards]
		INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
		INNER JOIN [customer_account]		ON [customer_account].card_id = branch_card_status.card_id
		INNER JOIN [branch]	ON [cards].branch_id = [branch].branch_id
			WHERE branch_card_status.branch_card_statuses_id = 6   
			and[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)	
			AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  AND branch_card_status.[status_date] >= DATEADD(d, -14,@WEEK_START_DATE)
				  AND branch_card_status.[status_date] <= DATEADD(d, -8,@WEEK_START_DATE)
				  group by [branch].branch_id, branch.branch_code 
union 
SELECT  distinct [branch].branch_id, branch.branch_code , 0 as 'oneeighty_d',0 as 'ninty_d', 0  as 'Week_one',0 as  'Week_two', 0 as 'Week_three',count(branch_card_status.card_id) as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [cards]
		INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
		INNER JOIN [customer_account] 	ON [customer_account].card_id = branch_card_status.card_id
		INNER JOIN [branch] ON [cards].branch_id = [branch].branch_id
			WHERE branch_card_status.branch_card_statuses_id = 6 	
			and[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)	
			AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  AND branch_card_status.[status_date] >= DATEADD(d, -21,@WEEK_START_DATE)
				  AND branch_card_status.[status_date] <= DATEADD(d, -15,@WEEK_START_DATE)
				  group by [branch].branch_id, branch.branch_code 
union 
SELECT distinct [branch].branch_id, branch.branch_code , 0 as 'oneeighty_d',0 as 'ninty_d', 0  as 'Week_one',0 as  'Week_two', 0 as 'Week_three',0 as 'Week_four',count([branch_card_status_current].card_id)  as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [branch_card_status_current] 
					INNER JOIN [cards]
						ON [branch_card_status_current].card_id = [cards].card_id
					INNER JOIN [branch]
						ON [cards].branch_id = [branch].branch_id
			WHERE ([branch_card_status_current].branch_card_statuses_id = 1	or [branch_card_status_current].branch_card_statuses_id = 0)
			and[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)		
			 AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  group by [branch].branch_id, branch.branch_code 
				 
) AS interx
group by interx.branch_id, interx.branch_code
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_card]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get card info
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_card] 
	@card_id BIGINT,
	@check_masking BIT,
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

			DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			--@check_masking = This proc is used by some backend process that require a clear mask and therefore 
			-- overrides the @mask_screen checking.

				SELECT distinct 
						[cards].card_id
					   , CASE @check_masking
							WHEN 1 THEN 
								CASE 
									WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
									ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
								END
							ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
						 END AS 'card_number'
					   , CASE @check_masking
							WHEN 1 THEN CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].pvv)) ELSE ''
							END as pvv							
					   , [cards].card_request_reference
					   , [cards].card_priority_id
					   , [card_priority_language].language_text AS 'card_priority_name'
					   , [cards].card_issue_method_id
					   , [card_issue_method_language].language_text AS 'card_issue_method_name'
					   , [cards].fee_charged
					   , [cards].fee_waiver_YN
					   , [cards].fee_editable_YN
					   , [cards].fee_overridden_YN
					   , [cards].fee_reference_number
					   , [cards].fee_reversal_ref_number
					   , CAST(0 as bit) AS pin_selected

					   , [cards].product_id
					   , [issuer].issuer_name, [issuer].issuer_code, [issuer].issuer_id
					   , [issuer].instant_card_issue_YN 
					   , [issuer].maker_checker_YN, [issuer].enable_instant_pin_YN as enable_instant_pin_YN_issuer
					   , [issuer].back_office_pin_auth_YN, [issuer].authorise_pin_issue_YN
					   , [issuer].card_ref_preference
					   , [country].country_id
					   , [country].country_code
					   , [country].country_name
					   , [country].country_capital_city
					   , [branch].branch_name, [branch].branch_code, [branch].branch_id
					   , [load_card_statuses].load_card_status			   					   
					   , [load_batch].load_batch_reference

					   , [issuer_product].product_name
					   , [issuer_product].product_code
					   , [issuer_product].product_bin_code
					   , [issuer_product].sub_product_code
					   , [issuer_product].Name_on_card_font_size
					   , [issuer_product].name_on_card_left
					   , [issuer_product].name_on_card_top
					   , [Issuer_product_font].font_name
					   , [issuer_product].enable_instant_pin_YN
					   , [issuer_product].pin_mailer_printing_YN
					   , [issuer_product].pin_mailer_reprint_YN

					   , [dist_card_statuses].dist_card_status_name
					   , [dist_batch].dist_batch_reference 

					   , [branch_card_status_current].branch_card_statuses_id
					   , [branch_card_status_current].status_date
					   , [branch_card_statuses_language].language_text AS 'branch_card_statuses_name'
					   , [branch_card_status_current].branch_card_code_type_id
					   , [branch_card_status_current].branch_card_code_name			   
					   , [branch_card_status_current].spoil_only
					   , [branch_card_status_current].comments
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY(operator.username)) as operator
					   , [customer_account].domicile_branch_id
					   , [domicile_branch].branch_code as domicile_branch_code
					   , [domicile_branch].branch_name as domicile_branch_name
					   , [customer_account].customer_account_id
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_account_number)) as 'customer_account_number'
					   , [customer_account].card_issue_reason_id
					   , [card_issue_reason_language].language_text AS 'card_issuer_reason_name'
					   , [customer_account].account_type_id
					   , [customer_account_type_language].language_text AS 'customer_account_type_name'
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name'
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name'
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name'
					   , [customer_account].customer_title_id
					   , [customer_title_language].language_text AS 'customer_title_name'
					   , --[customer_account].date_issued
							CASE
								WHEN [branch_card_status_current].branch_card_statuses_id = 6 THEN [branch_card_status_current].status_date
								ELSE null
							END as date_issued
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].name_on_card)) as 'name_on_card'
					   , [customer_account].[user_id]
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].first_name)) as 'user_first_name'
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].last_name)) as 'user_last_name'
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].username)) as 'username'
					   , [customer_account].cms_id
					   , [customer_account].currency_id

					   , [currency].currency_code
					   , [currency].iso_4217_numeric_code
					   , [product_currency].is_base as is_base_currency
					   , [product_currency].usr_field_name_1
					   , [product_currency].usr_field_val_1
					   , [product_currency].usr_field_name_2
					   , [product_currency].usr_field_val_2

					   , [customer_account].resident_id
					   , [customer_residency_language].language_text AS 'customer_residency_name'
					   , [customer_account].customer_type_id
					   , [customer_type_language].language_text AS 'customer_type_name'
					   , [customer_account].contract_number
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].Id_number)) as 'id_number'
					   , CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].contact_number)) as 'contact_number'
					   , [pin_mailer_reprint_statuses_language].pin_mailer_reprint_status_id
					   , [pin_mailer_reprint_statuses_language].language_text as 'pin_mailer_reprint_status_name'

				FROM [cards]
					INNER JOIN [branch]
						ON [branch].branch_id = [cards].branch_id
					INNER JOIN [issuer]
						ON [issuer].issuer_id = [branch].issuer_id
					INNER JOIN [country]
						ON [country].country_id = [issuer].country_id						
					INNER JOIN [card_issue_method_language]
						ON [cards].card_issue_method_id = [card_issue_method_language].card_issue_method_id
							AND [card_issue_method_language].language_id = @language_id
					INNER JOIN [card_priority_language]
						ON [cards].card_priority_id = [card_priority_language].card_priority_id
							AND [card_priority_language].language_id = @language_id					
					LEFT OUTER JOIN [load_batch_cards]
						ON [load_batch_cards].card_id = [cards].[card_id]
					LEFT OUTER JOIN [load_batch]
						ON [load_batch].load_batch_id = [load_batch_cards].load_batch_id
					LEFT OUTER JOIN [load_card_statuses]
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
					LEFT OUTER JOIN [user] operator
						ON [branch_card_status_current].operator_user_id = operator.[user_id]
					LEFT OUTER JOIN [branch_card_statuses_language]
						ON [branch_card_statuses_language].branch_card_statuses_id = [branch_card_status_current].branch_card_statuses_id
							AND [branch_card_statuses_language].language_id = @language_id

					LEFT OUTER JOIN [customer_account]
						ON [cards].card_id = [customer_account].card_id
					LEFT OUTER JOIN [branch] as [domicile_branch]
						ON [domicile_branch].branch_id = [customer_account].domicile_branch_id
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
					LEFT OUTER JOIN [currency]
						ON [currency].currency_id = [customer_account].currency_id
					LEFT OUTER JOIN [product_currency]
						ON [product_currency].currency_id = [customer_account].currency_id
							AND [product_currency].product_id = [issuer_product].product_id
					LEFT OUTER JOIN [user]
						ON [customer_account].[user_id] = [user].[user_id]

					LEFT OUTER JOIN [pin_mailer_reprint_status_current]
						ON [pin_mailer_reprint_status_current].card_id = [cards].card_id
					LEFT OUTER JOIN [pin_mailer_reprint_statuses_language]
						ON [pin_mailer_reprint_statuses_language].pin_mailer_reprint_status_id = [pin_mailer_reprint_status_current].pin_mailer_reprint_status_id
							AND [pin_mailer_reprint_statuses_language].language_id = @language_id
				WHERE [cards].card_id = @card_id
				

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key


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
/****** Object:  StoredProcedure [dbo].[usp_get_card_centre_card_count]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get the card count for a card centre
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_card_centre_card_count] 
	@branch_id int,
	@product_id int,
	--@sub_product_id int = NULL,
	@card_issue_method_id int = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @card_centre_count int = 0,
			@branch_count int = 0,
			@load_count int = 0

	--Form load batch
 --   SELECT @load_count = COUNT([load_batch_cards].card_id)
	--FROM [cards]
	--		INNER JOIN [load_batch_cards]
	--			ON [load_batch_cards].[card_id] = [cards].card_id
	--				AND [load_batch_cards].load_card_status_id = 1
	--WHERE [cards].branch_id = @branch_id
	--		AND [cards].product_id = @product_id
	--		AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))	
	--		AND [cards].card_issue_method_id = COALESCE(@card_issue_method_id, [cards].card_issue_method_id)

	--Form Production Batch
	--SELECT COUNT([cards].card_id)
	--FROM [cards]
	--		INNER JOIN [dist_batch_cards]
	--			ON [cards].card_id = [dist_batch_cards].card_id
	--		INNER JOIN [dist_batch]
	--			ON [dist_batch_cards].dist_batch_id = [dist_batch].dist_batch_id
	--		INNER JOIN [dist_batch_status_current]
	--			ON [dist_batch].dist_batch_id = [dist_batch_status_current].dist_batch_id
	--WHERE [cards].branch_id = @branch_id
	--		AND [cards].product_id = @product_id
	--		AND [cards].sub_product_id = @sub_product_id
	--		AND [dist_batch].dist_batch_type_id = 0
	--		AND [dist_batch_status_current].dist_batch_statuses_id = 14
	--		AND [dist_batch_cards].dist_card_status_id = 18
	--		AND [cards].card_issue_method_id = COALESCE(@card_issue_method_id, [cards].card_issue_method_id)

	SELECT @card_centre_count = COUNT(DISTINCT [cards].card_id)
	FROM [cards]
			INNER JOIN [avail_cc_and_load_cards]
				ON [cards].card_id = [avail_cc_and_load_cards].card_id
			INNER JOIN [branch]
				ON [cards].branch_id = [branch].branch_id
	WHERE [cards].branch_id = @branch_id
			AND [cards].product_id = @product_id
			--AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
			AND [cards].card_issue_method_id = COALESCE(@card_issue_method_id, [cards].card_issue_method_id)
			AND [branch].card_centre_branch_YN = 1
	

	SELECT @branch_count = COUNT(DISTINCT [cards].card_id)
	FROM [cards]
			INNER JOIN [branch_card_status_current]
				ON [cards].card_id = [branch_card_status_current].card_id
			INNER JOIN [branch]
				ON [cards].branch_id = [branch].branch_id
	WHERE [cards].branch_id = @branch_id
			AND [cards].product_id = @product_id
			--AND [cards].sub_product_id = @sub_product_id			
			AND [branch_card_status_current].branch_card_statuses_id = 0
			AND [cards].card_issue_method_id = COALESCE(@card_issue_method_id, [cards].card_issue_method_id)
			AND [branch].card_centre_branch_YN = 0


	SELECT (@card_centre_count + @branch_count + @load_count) as CardCount
END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_card_history_reference]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi
-- Create date: 20150520
-- Description:	Card View History Page: Get batch reference numbers
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_card_history_reference]
	@card_id INT
	, @languageId INT = NULL
	, @audit_user_id BIGINT
	, @audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	BEGIN TRANSACTION [GET_CARD_HIST]
		BEGIN TRY 

		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate;

			SELECT
				[branch_card_statuses_name]
				, status_date
				, CONVERT(VARCHAR(MAX),DECRYPTBYKEY(u.username)) AS username
			FROM 
				cards c
				LEFT JOIN branch_card_status cs ON c.card_id = cs.card_id
				LEFT JOIN branch_card_statuses bcs ON cs.branch_card_statuses_id = bcs.branch_card_statuses_id
				LEFT JOIN [user] u ON u.[user_id] = cs.[user_id]
			WHERE
				c.card_id = @card_id

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

			COMMIT TRANSACTION [GET_CARD_HIST]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_CARD_HIST]
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
/****** Object:  StoredProcedure [dbo].[usp_get_card_history_status]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi
-- Create date: 20150520
-- Description:	Card View History Page: Get batch status history
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_card_history_status]
	@card_id INT
	, @languageId INT = NULL
	, @audit_user_id BIGINT
	, @audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
		BEGIN TRANSACTION [GET_CARD_HIST_STATUS]
			BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			SELECT
				db.dist_batch_id
				, dist_batch_reference
				, dbt.dist_batch_type_name
				, db.date_created
			FROM
				dist_batch db
				LEFT JOIN dist_batch_cards dbc ON db.dist_batch_id = dbc.dist_batch_id
				LEFT JOIN cards c ON c.card_id = dbc.card_id
				LEFT JOIN dist_batch_type dbt ON dbt.dist_batch_type_id = db.dist_batch_type_id
			WHERE
				dbc.card_id = @card_id

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

			COMMIT TRANSACTION [GET_CARD_HIST_STATUS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_CARD_HIST_STATUS]
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
/****** Object:  StoredProcedure [dbo].[usp_get_card_object]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch card object
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_card_object] 
	@card_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)) AS 'card_number'
		, cards.card_request_reference AS card_reference_number
		, [cards].branch_id
		, [cards].card_id
		, [cards].card_issue_method_id
		, [cards].card_priority_id
		, [cards].card_request_reference
		, [cards].card_sequence
		, [cards].product_id		
		--, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_activation_date)), 109) as 'card_activation_date'
		--, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_expiry_date)), 109) as 'card_expiry_date'
		--, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_production_date)), 109) as 'card_production_date'		
		, CONVERT(DATETIME, CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_activation_date)), 109) as 'card_activation_date'
		, CONVERT(DATETIME2, CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_expiry_date))) as 'card_expiry_date'
		, CONVERT(DATETIME, CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_production_date)), 109) as 'card_production_date'					
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].pvv)) as 'pvv'
		--, [dist_batch].dist_batch_reference
		--, [dist_batch_cards].dist_batch_id
		--, [dist_batch_cards].dist_card_status_id
		--, [dist_card_statuses].dist_card_status_name
		, 0 as dist_batch_id
		, 0 as dist_card_status_id
		, [customer_account].account_type_id
		, [customer_account].cms_id
		, [customer_account].contract_number
		, [customer_account].currency_id
		, [customer_account].customer_account_id
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_account_number)) as 'customer_account_number'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].name_on_card)) as 'name_on_card'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].Id_number)) as 'Id_number'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].contact_number)) as 'contact_number'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].CustomerId)) as 'CustomerId'
		, [customer_account].customer_title_id
		, [customer_account].customer_type_id
		, [customer_account].date_issued
		, [customer_account].resident_id
		, [customer_account].[user_id]
		, [issuer].issuer_id
		, [issuer].issuer_code
		, [issuer].issuer_name
		, [branch].branch_code
		, [branch].branch_name
		, [issuer_product].[product_code]
		, [issuer_product].[sub_product_code]
		, [issuer_product].[product_name]
		, [issuer_product].[product_bin_code]
		, [issuer_product].[pan_length]
		, [issuer_product].[src1_id]
		, [issuer_product].[src2_id]
		, [issuer_product].[src3_id]
		, CONVERT(INT, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVKI]))) as 'PVKI'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVK])) as 'PVK'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKA])) as 'CVKA'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKB])) as 'CVKB'
		, [issuer_product].[expiry_months]

	FROM 
		[cards]							
		INNER JOIN [branch]
			ON [cards].branch_id = [branch].branch_id
		INNER JOIN [issuer]
			ON [branch].issuer_id = [issuer].issuer_id
		INNER JOIN [issuer_product]
			ON [cards].product_id = [issuer_product].product_id
		LEFT OUTER JOIN [customer_account]
			ON [cards].card_id = [customer_account].card_id
	WHERE 
		[cards].card_id = @card_id
					
														   
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

END



GO
/****** Object:  StoredProcedure [dbo].[usp_get_card_priority_list]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_card_priority_list]
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [card_priority].card_priority_id, [card_priority].card_priority_order, 
			[card_priority_language].language_text AS card_priority_name, [card_priority].default_selection
	FROM [card_priority]
		INNER JOIN [card_priority_language]
			ON [card_priority].card_priority_id = [card_priority_language].card_priority_id
	WHERE [card_priority_language].language_id = @language_id
	ORDER BY [card_priority].card_priority_order

END







GO
/****** Object:  StoredProcedure [dbo].[usp_get_cardinventorysummaryreport]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [usp_get_cardinventorysummaryreport] null,3,0,1,''
CREATE PROCEDURE [dbo].[usp_get_cardinventorysummaryreport]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int =null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
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
		   INTERX.statuses_name,
		   INTERX.statuses_id,
		   COUNT(INTER.card_id)	AS CardCount,
		   issuer.issuer_name as 'issuer_name'

	FROM 	 
		  (SELECT [branch].branch_id, dist_batch_status_current.dist_batch_statuses_id as 'statuses_id', [dist_batch_cards].card_id
			FROM dist_batch_status_current
			INNER JOIN [dist_batch_cards]
				ON dist_batch_status_current.dist_batch_id = [dist_batch_cards].dist_batch_id
			INNER JOIN [cards]
				ON [dist_batch_cards].card_id = [cards].card_id
			INNER JOIN [branch]
				ON [cards].branch_id = [branch].branch_id
					AND [branch].card_centre_branch_YN = 1					
	WHERE [branch].branch_status_id = 0 
			) AS INTER
				RIGHT OUTER JOIN 			
			(SELECT issuer_id, branch_id, branch_code, [dist_batch_statuses].dist_batch_statuses_id as 'statuses_id', bcl.language_text as 'statuses_name',bcl.language_id
			 FROM	[branch], [dist_batch_statuses]
			 inner join [dist_batch_statuses_language] bcl on [dist_batch_statuses].dist_batch_statuses_id=bcl.dist_batch_statuses_id
			 where branch.branch_status_id=0 and branch.card_centre_branch_YN = 1
		 		) INTERX

			ON INTER.branch_id = INTERX.branch_id
				AND INTER.statuses_id = INTERX.statuses_id 				
	inner join issuer on issuer.issuer_id=INTERX.issuer_id

	WHERE INTERX.issuer_id = COALESCE(@issuer_id, INTERX.issuer_id)
		  AND INTERX.branch_id = COALESCE(@branch_id, INTERX.branch_id) and INTERX.language_id=@language_id
		  AND INTERX.statuses_id not in (17,18,19)
		  AND INTERX.branch_code <> ''  
	GROUP BY INTERX.issuer_id, INTERX.branch_id, branch_code, statuses_name, INTERX.statuses_id,issuer_name
	ORDER BY issuer_name, INTERX.branch_code, INTERX.statuses_id

END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_cardrequests]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_cardrequests]	
	@issuer_id int,
	@branch_id int = null,
	@languageId int,	
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @branch_card_statuses_id int
	SET @branch_card_statuses_id = 3

	BEGIN
		DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY product_name ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(		
		SELECT issuer_product.product_id,
			   issuer_product.product_name,
			   card_priority.card_priority_id, 
			   card_priority_language.language_text as 'card_priority_name',
			   COUNT(cards.card_id) as 'cardscount'
		FROM cards 
				INNER JOIN issuer_product 
					ON cards.product_id = issuer_product.product_id 
				INNER JOIN	branch_card_status_current 
					ON branch_card_status_current.card_id = cards.card_id 
				INNER JOIN card_priority 
					ON cards.card_priority_id = card_priority.card_priority_id
				INNER JOIN card_priority_language 
					ON card_priority.card_priority_id=card_priority_language.card_priority_id
		WHERE cards.[card_issue_method_id] = 0 
				AND	branch_card_status_current.branch_card_statuses_id = @branch_card_statuses_id  
				AND card_priority_language.language_id = @languageId
				AND issuer_id = @issuer_id 
				AND cards.branch_id = COALESCE(@branch_id, cards.branch_id)
		GROUP BY issuer_product.product_id, card_priority.card_priority_id, language_text, product_name   
	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY product_name ASC

	END
END








GO
/****** Object:  StoredProcedure [dbo].[usp_get_cards_in_error]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_cards_in_error] 
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

	DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)
			
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
		SELECT 
			DISTINCT [cards].card_id
			, CASE 
				WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
				ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
			  END AS 'card_number'
			, cards.card_request_reference AS card_reference_number
			, [cards].product_id, --[cards].sub_product_id, 
			[cards].card_priority_id, [cards].card_issue_method_id				   
			, [branch_card_statuses_language].language_text AS current_card_status
			, [branch_card_status_current].comments					
			, [branch_card_status_current].status_date
			, [branch_card_status_current].branch_card_statuses_id					
			, [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code
			, [branch].branch_id, [branch].branch_name, [branch].branch_code
			, card_request_reference
			, null as operator_user_id
			, null as operator_username
			, '' as product_bin_code
			, '' AS sub_product_code	
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
/****** Object:  StoredProcedure [dbo].[usp_get_centercardstock_report]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [usp_get_centercardstock_report] null,3,0,2,'veneka'

CREATE PROCEDURE [dbo].[usp_get_centercardstock_report]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(@issuer_id = -1 or @issuer_id = 0)
	 set @issuer_id=null

	if(@branch_id  =0)
		set @branch_id = null

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)

    -- Insert statements for procedure here
	 OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT distinct 
		[branch].branch_id,branch.branch_code
		,CASE 
			WHEN @mask_report = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
			ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
		  END AS 'card_number'
		, [cards].card_request_reference as 'card_request_reference'
		, CONVERT(datetime, CONVERT(varchar(max),DECRYPTBYKEY([cards].card_expiry_date))) as 'card_expiry_date'
		, CONVERT(datetime, [dist_batch].date_created,103) as'card_production_date'
	FROM 
		dist_batch_status_current		
		INNER JOIN [dist_batch_cards]
			ON dist_batch_status_current.dist_batch_id = [dist_batch_cards].dist_batch_id
		INNER JOIN [dist_batch]
			ON [dist_batch_cards].dist_batch_id = [dist_batch].dist_batch_id
		INNER JOIN [cards] 
			ON [dist_batch_cards].card_id = [cards].card_id
		INNER JOIN [branch] 
			ON [cards].branch_id = [branch].branch_id
				AND [branch].card_centre_branch_YN = 1
		INNER JOIN [issuer] 
			ON issuer.issuer_id = branch.issuer_id
	WHERE
		dist_batch_cards.dist_card_status_id = 18		
		AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
		AND [branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)	
	ORDER BY
		[branch].branch_id
		
			
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_cms_bank_parameters]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_cms_bank_parameters] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_cms_parameters]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_cms_parameters] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_connection_parameter]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_connection_parameter] 
	-- Add the parameters for the stored procedure here
	@connection_parameter_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT [connection_parameter_id],[connection_name],[address],[port],[path],[protocol],[auth_type],[header_length],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([identification])) as identification,
			CONVERT(VARCHAR(max),DECRYPTBYKEY([username])) as [username],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([password])) as [password], 
			connection_parameter_type_id, timeout_milli, buffer_size, doc_type, name_of_file, 
			file_delete_YN, file_encryption_type_id, duplicate_file_check_YN,
			CONVERT(VARCHAR(max),DECRYPTBYKEY([private_key])) as [private_key], 
			CONVERT(VARCHAR(max),DECRYPTBYKEY([public_key])) as [public_key],domain_name ,is_external_auth
		FROM connection_parameters
		WHERE connection_parameter_id = @connection_parameter_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

END










GO
/****** Object:  StoredProcedure [dbo].[usp_get_connection_params]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_connection_params] 
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

		SELECT [connection_parameter_id],[connection_name],[address],[port],[path],[protocol],[auth_type],[header_length],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([identification])) as identification,
			CONVERT(VARCHAR(max),DECRYPTBYKEY([username])) as [username],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([password])) as [password], 
			connection_parameter_type_id, timeout_milli, buffer_size, doc_type, name_of_file, 
			file_delete_YN, file_encryption_type_id, duplicate_file_check_YN,
			CONVERT(VARCHAR(max),DECRYPTBYKEY([private_key])) as [private_key], 
			CONVERT(VARCHAR(max),DECRYPTBYKEY([public_key])) as [public_key],domain_name ,is_external_auth
		FROM connection_parameters

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

END










GO
/****** Object:  StoredProcedure [dbo].[usp_get_currency]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_currency] 
	-- Add the parameters for the stored procedure here
	@CCY varchar(3)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT TOP 1 currency_id
	FROM [currency]
	WHERE currency_code = @CCY

END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_currency_id]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_currency_id] 
	-- Add the parameters for the stored procedure here
	@currency_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT TOP 1 currency_code
	FROM [currency]
	WHERE currency_id = @currency_id

END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_currency_iso_numeric_code]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_currency_iso_numeric_code] 
	-- Add the parameters for the stored procedure here
	@currency_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT TOP 1 iso_4217_numeric_code
	FROM [currency]
	WHERE currency_id = @currency_id

END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_currency_list]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_currency_list] 
	-- Add the parameters for the stored procedure here
	@language_id int = 0,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM [currency]
	WHERE active_YN = 1

END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_currenies_product]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_currenies_product] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_current_product_fee]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_current_product_fee] 
	-- Add the parameters for the stored procedure here
	@fee_detail_id int,
	@currency_id int,
	@card_issue_reason_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [product_fee_charge].* 
	FROM [product_fee_detail]
			INNER JOIN [product_fee_charge]
				ON [product_fee_detail].fee_detail_id = [product_fee_charge].fee_detail_id
					AND [product_fee_charge].card_issue_reason_id = @card_issue_reason_id
					AND [product_fee_charge].currency_id = @currency_id
	WHERE [product_fee_detail].fee_detail_id = @fee_detail_id

END





GO
/****** Object:  StoredProcedure [dbo].[usp_get_customer_details]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_customer_details] 
	-- Add the parameters for the stored procedure here
	@card_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT CONVERT(VARCHAR(max),DECRYPTBYKEY(customer_account_number)) as customer_account_number,
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(customer_first_name)) as customer_first_name,
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(customer_middle_name)) as customer_middle_name,
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(customer_last_name)) as customer_last_name,
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(name_on_card)) as name_on_card,
			   domicile_branch_id,
			   customer_account_id, 
			   [user_id], 
			   card_id, 
			   card_issue_reason_id, 
			   account_type_id,
			   currency_id, 
			   resident_id, 
			   customer_type_id, 
			   date_issued,
			   cms_id, 
			   contract_number, 
			   customer_title_id , 
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(Id_number)) as 'Id_number',
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(contact_number)) as contact_number
		FROM customer_account
		WHERE card_id = @card_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END







GO
/****** Object:  StoredProcedure [dbo].[usp_get_customercardsearch_list]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_customercardsearch_list]
	@issuer_id int =null,
	@branch_id int =null,
	@cardrefno nvarchar(50),
	@customeraccountno nvarchar(50),
	@product_id int =null,
	@card_Priority int=null,
	@card_issuemethod int =null,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		IF @customeraccountno = '' OR @customeraccountno IS NULL
		SET @customeraccountno = NULL
	ELSE
		SET @customeraccountno = '%' + @customeraccountno + '%'
			
			IF @cardrefno = '' OR @cardrefno IS NULL
		SET @cardrefno = NULL
	ELSE
		SET @cardrefno = '%' + @cardrefno + '%'

		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)

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
	
    -- Insert statements for procedure here
	SELECT
		ip.product_name 
		, CASE 
			WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY(c.card_number)),6,4) 
			ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY(c.card_number))
		  END AS 'card_number'
		, c.card_request_reference AS card_reference_number
		, ca.card_id
		, CONVERT(VARCHAR(max),DECRYPTBYKEY(ca.customer_first_name)) as 'first_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY(ca.customer_last_name)) as 'last_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY(ca.customer_account_number)) AS account_number
	FROM
		customer_account ca 
		INNER JOIN cards as c ON ca.card_id = c.card_id 
		INNER JOIN [branch_card_status_current] ON [branch_card_status_current].card_id = c.card_id	
		INNER JOIN issuer_product as ip ON c.product_id = ip.product_id
		INNER JOIN [branch] ON [branch].branch_id = c.branch_id
		INNER JOIN [issuer] ON [issuer].[issuer_id] = [branch].issuer_id
	WHERE
		ip.issuer_id =COALESCE(@issuer_id, ip.issuer_id)
		AND branch.branch_id=COALESCE(@branch_id,branch.branch_id) 
		AND (@customeraccountno IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY(ca.customer_account_number)) LIKE @customeraccountno) 
		AND (@cardrefno IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY(c.card_number)) LIKE @cardrefno)
		AND CONVERT(VARCHAR(max),DECRYPTBYKEY(c.card_number)) <>''
		AND [branch_card_status_current].branch_card_statuses_id = 1 
		AND c.card_issue_method_id=COALESCE(@card_issuemethod, c.card_issue_method_id) 
		AND c.card_priority_id =COALESCE(@card_Priority, c.card_priority_id)
		AND c.product_id =COALESCE(@product_id, c.product_id)
		)
						  AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY product_name ASC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END







GO
/****** Object:  StoredProcedure [dbo].[usp_get_dist_batch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Retreive a distribution batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_dist_batch] 
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

		DECLARE @flow_id int

		if(NOT EXISTS(SELECT * FROM [dist_batch_statuses_flow] INNER JOIN [dist_batch]
					ON [dist_batch_statuses_flow].issuer_id = [dist_batch].issuer_id
						AND [dist_batch_statuses_flow].card_issue_method_id = [dist_batch].card_issue_method_id
						AND [dist_batch_statuses_flow].dist_batch_type_id = [dist_batch].dist_batch_type_id
					WHERE [dist_batch].dist_batch_id = @dist_batch_id))
			BEGIN
				SET @flow_id = -1
			END

		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate;
			SELECT 
				distinct CAST(1 as BIGINT) as ROW_NO, 1 AS TOTAL_ROWS, 1 as TOTAL_PAGES
				, [dist_batch].dist_batch_id
				, [dist_batch].date_created
				, [dist_batch].dist_batch_reference
				, [dist_batch].no_cards
				, [dist_batch_status_current].dist_batch_statuses_id
				, [dist_batch_status_current].status_notes
				, [dist_batch_statuses_language].language_text AS 'dist_batch_status_name'
				, [issuer].issuer_id
				, [issuer].issuer_name
				, [issuer].issuer_code
				--, [issuer].auto_create_dist_batch
				, [card_issue_method_language].language_text AS 'card_issue_method_name'
				, [dist_batch].card_issue_method_id
				, [dist_batch].dist_batch_type_id
				, [issuer_flow].flow_dist_batch_statuses_id
				, [issuer_flow].flow_dist_batch_type_id
				, [issuer_flow].user_role_id
				, [issuer_flow].reject_dist_batch_statuses_id
				, ISNULL([branch].branch_name, '-') as branch_name
				, ISNULL([branch].branch_code,'') as branch_code
				, issuer_product.product_name as 'product_name'
				, CONVERT(VARCHAR(max),DECRYPTBYKEY(CREATOR.username)) as [username]
				
			FROM [dist_batch] 
				INNER JOIN [dist_batch_status_current]
					ON [dist_batch].dist_batch_id = [dist_batch_status_current].dist_batch_id
				INNER JOIN [dist_batch_statuses_language]
					ON [dist_batch_status_current].dist_batch_statuses_id = [dist_batch_statuses_language].dist_batch_statuses_id
				INNER JOIN [issuer]
					ON [issuer].issuer_id = [dist_batch].issuer_id
				INNER JOIN [card_issue_method_language]
					ON [dist_batch].card_issue_method_id = [card_issue_method_language].card_issue_method_id
						AND [card_issue_method_language].language_id = @language_id	
				LEFT OUTER JOIN (SELECT [username], dist_batch_id
							FROM [user]
								INNER JOIN [dist_batch_status]
									ON [dist_batch_status].[user_id] = [user].[user_id]
										AND [dist_batch_status].dist_batch_statuses_id = 0
										AND [dist_batch_status].dist_batch_id = @dist_batch_id) AS CREATOR
					ON CREATOR.dist_batch_id = [dist_batch].dist_batch_id

				LEFT OUTER JOIN [branch]
					ON [branch].branch_id = [dist_batch].branch_id	

				LEFT OUTER JOIN [dist_batch_statuses_flow] as [issuer_flow]
					ON [dist_batch_status_current].dist_batch_statuses_id = [issuer_flow].dist_batch_statuses_id
						AND [dist_batch].card_issue_method_id = [issuer_flow].card_issue_method_id
						AND [issuer_flow].dist_batch_type_id = [dist_batch].dist_batch_type_id
						AND [issuer_flow].issuer_id = COALESCE(@flow_id, [dist_batch].issuer_id)
			INNER JOIN [dist_batch_cards]
			 on [dist_batch].dist_batch_id=[dist_batch_cards].dist_batch_id
			 INNER JOIN cards  
			 ON [dist_batch_cards].card_id=cards.card_id
			 INNER JOIN issuer_product 
			 ON cards.product_id = issuer_product.product_id

			WHERE [dist_batch].dist_batch_id = @dist_batch_id
					AND [dist_batch_statuses_language].language_id = @language_id	
					
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting dist batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_get_dist_batch_card_details]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch all cards linked to distribution batch with their details
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_dist_batch_card_details] 
	@dist_batch_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_DIST_BATCH_CARD_DETAILS]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			SELECT
				  CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)) AS 'card_number'
				, cards.card_request_reference AS card_reference_number
				, [cards].branch_id
				, [cards].card_id
				, [cards].card_issue_method_id
				, [cards].card_priority_id
				, [cards].card_request_reference
				, [cards].card_sequence
				, [cards].product_id
				, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_activation_date)), 109) as 'card_activation_date'
				, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_expiry_date)), 109) as 'card_expiry_date'
				, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_production_date)), 109) as 'card_production_date'						
				, CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].pvv)) as 'pvv'
				, [dist_batch].dist_batch_reference
				, [dist_batch_cards].dist_batch_id
				, [dist_batch_cards].dist_card_status_id
				, [dist_card_statuses].dist_card_status_name
				, [customer_account].account_type_id
				, [customer_account].cms_id
				, [customer_account].contract_number
				, [customer_account].currency_id
				, [customer_account].customer_account_id
				, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_account_number)) as 'customer_account_number'
				, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name'
				, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name'
				, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name'
				, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].name_on_card)) as 'name_on_card'
				, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].Id_number)) as 'Id_number'
				, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].contact_number)) as 'contact_number'
				, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].CustomerId)) as 'CustomerId'
				, [customer_account].customer_title_id
				, [customer_account].customer_type_id
				, [customer_account].date_issued
				, [customer_account].resident_id
				, [customer_account].[user_id]
				, [issuer].issuer_id
				, [issuer].issuer_code
				, [issuer].issuer_name
				, [branch].branch_code
				, [branch].branch_name
				, [issuer_product].[product_code]
				, [issuer_product].[product_name]
				, [issuer_product].[product_bin_code]
				, [issuer_product].[sub_product_code]
				, [issuer_product].[pan_length]
				, [issuer_product].[src1_id]
				, [issuer_product].[src2_id]
				, [issuer_product].[src3_id]
				--, [issuer_product].sub_product_id_length
				, CONVERT(INT, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVKI]))) as 'PVKI'
				, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVK])) as 'PVK'
				, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKA])) as 'CVKA'
				, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKB])) as 'CVKB'
				, [issuer_product].[expiry_months]
				--, [sub_product].sub_product_id
				--, [sub_product].sub_product_name
				--, [sub_product].sub_product_code
			FROM 
				[cards]							
				INNER JOIN [dist_batch_cards] 
					ON [dist_batch_cards].card_id = [cards].card_id	
				INNER JOIN [dist_card_statuses]
					ON 	[dist_batch_cards].dist_card_status_id = [dist_card_statuses].dist_card_status_id
				INNER JOIN [dist_batch]
					ON [dist_batch].dist_batch_id = [dist_batch_cards].dist_batch_id	
				INNER JOIN [branch]
					ON [cards].branch_id = [branch].branch_id
				INNER JOIN [issuer]
					ON [branch].issuer_id = [issuer].issuer_id
				INNER JOIN [issuer_product]
					ON [cards].product_id = [issuer_product].product_id
				LEFT OUTER JOIN [customer_account]
					ON [cards].card_id = [customer_account].card_id
				--LEFT OUTER JOIN [sub_product]
				--	ON [cards].sub_product_id = [sub_product].sub_product_id
				--		AND [cards].product_id = [sub_product].product_id
			WHERE 
				[dist_batch_cards].dist_batch_id = @dist_batch_id
					
														   
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;		

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting cards for distribution batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_DIST_BATCH_CARD_DETAILS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_DIST_BATCH_CARD_DETAILS]
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
/****** Object:  StoredProcedure [dbo].[usp_get_dist_batch_card_details_paged]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch all cards linked to distribution batch with their details
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_dist_batch_card_details_paged] 
	@dist_batch_id bigint,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @StartRow INT, @EndRow INT;	
	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;


	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY card_id ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM( 

		SELECT
			[dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) AS 'card_number'
			, cards.card_request_reference AS card_reference_number
			, [cards].branch_id
			, [cards].card_id
			, [cards].card_issue_method_id
			, [cards].card_priority_id
			, [cards].card_request_reference
			, [cards].card_sequence
			, [cards].product_id
			, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_activation_date)), 109) as 'card_activation_date' 
			, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_expiry_date)), 109) as 'card_expiry_date'
			, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_production_date)), 109) as 'card_production_date'						
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].pvv)) as 'pvv'
			, [dist_batch_cards].dist_batch_id
			, [dist_batch_cards].dist_card_status_id
			, [dist_card_statuses].dist_card_status_name
			, [customer_account].account_type_id
			, [customer_account].cms_id
			, [customer_account].contract_number
			, [customer_account].currency_id, [customer_account].customer_account_id
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_account_number)) as 'customer_account_number'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].name_on_card)) as 'name_on_card'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].Id_number)) as 'Id_number'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].contact_number)) as 'contact_number'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].CustomerId)) as 'CustomerId'
			, [customer_account].customer_title_id
			, [customer_account].customer_type_id
			, [customer_account].date_issued
			, [customer_account].resident_id
			, [customer_account].[user_id]
			, [issuer].issuer_id
			, [issuer].issuer_code
			, [issuer].issuer_name
			, [branch].branch_code
			, [branch].branch_name
			, [issuer_product].[product_code]
			, [issuer_product].[product_name]
			, [issuer_product].[product_bin_code]
			, [issuer_product].[sub_product_code]
			, [issuer_product].[src1_id]
			, [issuer_product].[src2_id]
			, [issuer_product].[src3_id]
			, CONVERT(INT, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVKI]))) as 'PVKI'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVK])) as 'PVK'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKA])) as 'CVKA'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKB])) as 'CVKB'
			, [issuer_product].[expiry_months]
		FROM [cards]
			INNER JOIN [dist_batch_cards] 
				ON [dist_batch_cards].card_id = [cards].card_id	
			INNER JOIN [dist_card_statuses]
				ON 	[dist_batch_cards].dist_card_status_id = [dist_card_statuses].dist_card_status_id
			INNER JOIN [branch]
				ON [cards].branch_id = [branch].branch_id
			INNER JOIN [issuer]
				ON [branch].issuer_id = [issuer].issuer_id
			INNER JOIN [issuer_product]
				ON [cards].product_id = [issuer_product].product_id
			LEFT OUTER JOIN [customer_account]
				ON [cards].card_id = [customer_account].card_id
		WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id

	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY card_id ASC
														   
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;		

END












GO
/****** Object:  StoredProcedure [dbo].[usp_get_dist_batch_cards]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch all cards linked to distribution batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_dist_batch_cards] 
	@dist_batch_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_DIST_BATCH_CARDS]
		BEGIN TRY 
			DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				SELECT 
					CASE 
						WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
						ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
					END AS 'card_number'
					, cards.card_request_reference AS card_reference_number
					, [dist_card_statuses].dist_card_status_name					   
				FROM [cards]
					INNER JOIN [dist_batch_cards] ON [dist_batch_cards].card_id = [cards].card_id	
					INNER JOIN [dist_card_statuses] ON 	[dist_batch_cards].dist_card_status_id = [dist_card_statuses].dist_card_status_id
					INNER JOIN [branch] ON [branch].branch_id = cards.branch_id
					INNER JOIN [issuer] ON branch.issuer_id = issuer.issuer_id
				WHERE 
					[dist_batch_cards].dist_batch_id = @dist_batch_id
					
														   
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;		

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting cards for distribution batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_get_dist_batch_history]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch the distribution batches history
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_dist_batch_history] 
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

				SELECT 
					[dist_batch].dist_batch_reference
					, [dist_batch].no_cards
					, [dist_batch_status].status_date
					, [dist_batch_status].status_notes
					, [dist_batch_statuses_language].language_text as dist_batch_status_name
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'username'
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].first_name)) as 'first_name'
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].last_name)) as 'last_name'   
					, branch.branch_code
					, branch.branch_name
				FROM [dist_batch]
						INNER JOIN [dist_batch_status]
							ON [dist_batch].dist_batch_id = [dist_batch_status].dist_batch_id
						INNER JOIN [dist_batch_statuses]
							ON [dist_batch_status].dist_batch_statuses_id = [dist_batch_statuses].dist_batch_statuses_id
							INNER JOIN [dist_batch_statuses_language]
							ON [dist_batch_status].dist_batch_statuses_id = [dist_batch_statuses_language].dist_batch_statuses_id
						INNER JOIN [user]
							ON [dist_batch_status].[user_id] = [user].[user_id]
						LEFT JOIN [branch]
							ON [branch].branch_id = dist_batch.branch_id
				WHERE [dist_batch].dist_batch_id = @dist_batch_id and [dist_batch_statuses_language].language_id=@languageId
				ORDER BY [dist_batch_status].status_date ASC	

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting history for distribution batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_get_dist_batches]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_get_dist_batches]
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
/****** Object:  StoredProcedure [dbo].[usp_get_dist_batches_for_user]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 26 March 2014
-- Description:	Gets a list of distribution batches for a user
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_dist_batches_for_user] 
	-- Add the parameters for the stored procedure here
	@user_id bigint,
	@issuerId int =null,
	@dist_batch_reference VARCHAR(25) = NULL,
	@dist_batch_statuses_id int = NULL,
	@branch_id INT = NULL,
	@card_issue_method_id int = NULL,
	@distBatchTypeId int = null,
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
			IF(@dist_batch_reference = '' or @dist_batch_reference IS NULL)
				SET @dist_batch_reference = ''

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
					   [issuer].[issuer_id], [issuer].issuer_name, [issuer].issuer_code, 
					   --[issuer].auto_create_dist_batch,
					   [card_issue_method_language].language_text AS 'card_issue_method_name',
					   [dist_batch].card_issue_method_id, [dist_batch].dist_batch_type_id,
					   [issuer_flow].flow_dist_batch_statuses_id,
					   [issuer_flow].flow_dist_batch_type_id,
					   [issuer_flow].user_role_id,
					   [issuer_flow].reject_dist_batch_statuses_id,
					   ISNULL([branch].branch_name, '-') as branch_name, 
					   ISNULL([branch].branch_code,'') as branch_code, issuer_product.product_name as 'product_name',
					   '' as [username]
				FROM [dist_batch] 
					INNER JOIN [dist_batch_status_current]
						ON [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
					INNER JOIN [dist_batch_statuses_language]
						ON [dist_batch_status_current].dist_batch_statuses_id = [dist_batch_statuses_language].dist_batch_statuses_id
							AND [dist_batch_statuses_language].language_id = @language_id
					--INNER JOIN [user_roles_branch]
					--	ON [dist_batch].branch_id = [user_roles_branch].branch_id
					--		AND [user_roles_branch].user_role_id IN ( 2, 4, 5, 11, 12 )								
					INNER JOIN [issuer]
						ON [issuer].issuer_id = [dist_batch].issuer_id
					INNER JOIN [card_issue_method_language]
						ON [dist_batch].card_issue_method_id = [card_issue_method_language].card_issue_method_id
							AND [card_issue_method_language].language_id = @language_id		

					LEFT OUTER JOIN [branch]
						ON [branch].branch_id = [dist_batch].branch_id	
					--LEFT OUTER JOIN [dist_batch_statuses_flow]
					--	ON [dist_batch_status_current].dist_batch_statuses_id = [dist_batch_statuses_flow].dist_batch_statuses_id
					--		AND [dist_batch].card_issue_method_id = [dist_batch_statuses_flow].card_issue_method_id
					--		AND [dist_batch_statuses_flow].dist_batch_type_id = [dist_batch].dist_batch_type_id	
					LEFT OUTER JOIN [dist_batch_statuses_flow] as [issuer_flow]
						ON [dist_batch_status_current].dist_batch_statuses_id = [issuer_flow].dist_batch_statuses_id
							AND [dist_batch].card_issue_method_id = [issuer_flow].card_issue_method_id
							AND [issuer_flow].dist_batch_type_id = [dist_batch].dist_batch_type_id
							AND [issuer_flow].issuer_id = [dist_batch].issuer_id
				INNER JOIN [dist_batch_cards]
					 on [dist_batch].dist_batch_id=[dist_batch_cards].dist_batch_id
				 INNER JOIN [cards] 
					ON [dist_batch_cards].card_id=cards.card_id
				 INNER JOIN issuer_product 
					 ON cards.product_id = issuer_product.product_id
				WHERE 
					(([dist_batch].branch_id IS NULL 
						AND [dist_batch].issuer_id IN (SELECT issuer_id FROM [user_roles_issuer] WHERE user_role_id IN (1, 2, 4, 5, 11, 12, 13) AND [user_id] = @user_id))
					OR 
					  ([dist_batch].branch_id IS NOT NULL 
						AND ([dist_batch].branch_id IN (SELECT branch_id FROM [user_roles_branch] WHERE user_role_id IN (1, 2, 4, 5, 11, 12, 13) AND [user_id] = @user_id)
								OR
								[cards].origin_branch_id IN(SELECT branch_id FROM [user_roles_branch] WHERE user_role_id IN (2) AND [user_id] = @user_id))))
					--[user_roles_branch].[user_id] = @user_id
					AND ([dist_batch].dist_batch_reference like '%'+@dist_batch_reference+'%')
					--AND [dist_batch].dist_batch_reference LIKE COALESCE(@dist_batch_reference, [dist_batch].dist_batch_reference)
					AND [dist_batch_status_current].dist_batch_statuses_id = COALESCE(@dist_batch_statuses_id, [dist_batch_status_current].dist_batch_statuses_id)
					AND [dist_batch].card_issue_method_id = COALESCE(@card_issue_method_id, [dist_batch].card_issue_method_id)
					AND [dist_batch].dist_batch_type_id = COALESCE(@distBatchTypeId, [dist_batch].dist_batch_type_id)
					AND [dist_batch].date_created >= COALESCE(@date_start, [dist_batch].date_created)
					AND [dist_batch].date_created <= COALESCE(@date_end, [dist_batch].date_created)
					--AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
					--AND [branch].branch_status_id = 0	 
					AND [issuer].issuer_status_id = 0
					AND [issuer].issuer_id = COALESCE(@issuerId,  [issuer].issuer_id)
			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY date_created DESC, dist_batch_reference ASC

			--log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_get_export_batch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_export_batch] 
	-- Add the parameters for the stored procedure here
	@export_batch_id bigint, 
	@language_id int = 0,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [export_batch].*,
				[export_batch_statuses_language].language_text as export_batch_statuses_name,
				[export_batch_status_current].export_batch_statuses_id, 
				[export_batch_status_current].comments,
				[export_batch_status_current].status_date,
				[issuer].issuer_name,
				[issuer].issuer_code,
				0 as TOTAL_ROWS, 
				CONVERT(bigint, 0) AS ROW_NO,
				0 as TOTAL_PAGES				
		FROM [export_batch]
			INNER JOIN [export_batch_status_current]
				ON [export_batch].export_batch_id = [export_batch_status_current].export_batch_id
			INNER JOIN [export_batch_statuses_language]
				ON [export_batch_statuses_language].export_batch_statuses_id = [export_batch_status_current].export_batch_statuses_id
					AND [export_batch_statuses_language].language_id = @language_id
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [export_batch].issuer_id
		WHERE [export_batch].export_batch_id = @export_batch_id
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_export_batch_card_details]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch all cards linked to export batch with their details
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_export_batch_card_details] 
	@export_batch_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)) AS 'card_number'
		, cards.card_request_reference AS card_reference_number
		, [cards].branch_id
		, [cards].card_id
		, [cards].card_issue_method_id
		, [cards].card_priority_id
		, [cards].card_request_reference
		, [cards].card_sequence
		, [cards].product_id
		, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_activation_date)), 109) as 'card_activation_date'
		, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_expiry_date)), 109) as 'card_expiry_date'
		, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_production_date)), 109) as 'card_production_date'						
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].pvv)) as 'pvv'
		--, [dist_batch].dist_batch_reference
		--, [dist_batch_cards].dist_batch_id
		--, [dist_batch_cards].dist_card_status_id
		--, [dist_card_statuses].dist_card_status_name
		, [cards].export_batch_id as dist_batch_id
		, 0 as dist_card_status_id
		, [customer_account].account_type_id
		, [customer_account].cms_id
		, [customer_account].contract_number
		, [customer_account].currency_id
		, [customer_account].customer_account_id
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_account_number)) as 'customer_account_number'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].name_on_card)) as 'name_on_card'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].Id_number)) as 'Id_number'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].contact_number)) as 'contact_number'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].CustomerId)) as 'CustomerId'
		, [customer_account].customer_title_id
		, [customer_account].customer_type_id
		, [customer_account].date_issued
		, [customer_account].resident_id
		, [customer_account].[user_id]
		, [issuer].issuer_id
		, [issuer].issuer_code
		, [issuer].issuer_name
		, [branch].branch_code
		, [branch].branch_name
		, [issuer_product].[product_code]
		, [issuer_product].[sub_product_code]
		, [issuer_product].[product_name]
		, [issuer_product].[product_bin_code]
		, [issuer_product].[src1_id]
		, [issuer_product].[src2_id]
		, [issuer_product].[src3_id]
		, CONVERT(INT, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVKI]))) as 'PVKI'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVK])) as 'PVK'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKA])) as 'CVKA'
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKB])) as 'CVKB'
		, [issuer_product].[expiry_months]

	FROM 
		[cards]							
		INNER JOIN [branch]
			ON [cards].branch_id = [branch].branch_id
		INNER JOIN [issuer]
			ON [branch].issuer_id = [issuer].issuer_id
		INNER JOIN [issuer_product]
			ON [cards].product_id = [issuer_product].product_id
		LEFT OUTER JOIN [customer_account]
			ON [cards].card_id = [customer_account].card_id
	WHERE 
		[cards].export_batch_id = @export_batch_id
					
														   
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

END



GO
/****** Object:  StoredProcedure [dbo].[usp_get_export_batch_cards]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch all cards linked to distribution batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_export_batch_cards] 
	@export_batch_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_EXPORT_BATCH_CARDS]
		BEGIN TRY 
			DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				SELECT distinct
					CASE 
						WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
						ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
					END AS 'card_number'
					, cards.card_request_reference AS card_reference_number
					, '' as [export_batch_statuses_name]				   
				FROM [cards]
								INNER JOIN [export_batch] ON cards.export_batch_id = [export_batch].export_batch_id
						--INNER JOIN [export_batch_status]
						--	ON [export_batch].export_batch_id = [export_batch_status].export_batch_id
						--INNER JOIN [export_batch_statuses]
						--	ON [export_batch_status].export_batch_statuses_id = [export_batch_statuses].export_batch_statuses_id
					INNER JOIN [branch] ON [branch].branch_id = cards.branch_id
					INNER JOIN [issuer] ON branch.issuer_id = issuer.issuer_id
				WHERE 
				[cards].export_batch_id = @export_batch_id
					
														   
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting cards for distribution batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_EXPORT_BATCH_CARDS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_EXPORT_BATCH_CARDS]
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
/****** Object:  StoredProcedure [dbo].[usp_get_export_batch_history]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch the distribution batches history
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_export_batch_history] 
	@dist_batch_id bigint,	
	@languageId int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_EXPORT_BATCH_HISTORY]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

		
				SELECT 
					[export_batch].batch_reference
					, [export_batch].no_cards
					, [export_batch_status].status_date
					, [export_batch_status].comments
					, [export_batch_statuses_language].language_text as export_batch_status_name
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'username'
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].first_name)) as 'first_name'
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].last_name)) as 'last_name'   
					--, branch.branch_code
					--, branch.branch_name
				FROM [export_batch]
						INNER JOIN [export_batch_status]
							ON [export_batch].export_batch_id = [export_batch_status].export_batch_id
						INNER JOIN [export_batch_statuses]
							ON [export_batch_status].export_batch_statuses_id = [export_batch_statuses].export_batch_statuses_id
							INNER JOIN [export_batch_statuses_language]
							ON [export_batch_status].export_batch_statuses_id = [export_batch_statuses_language].export_batch_statuses_id
						INNER JOIN [user]
							ON [export_batch_status].[user_id] = [user].[user_id]
						--LEFT JOIN [branch]
						--	ON [branch].branch_id = [export_batch].branch_id
				WHERE [export_batch].export_batch_id = @dist_batch_id and [export_batch_statuses_language].language_id=@languageId
				ORDER BY [export_batch_status].status_date ASC	

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting history for distribution batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_EXPORT_BATCH_HISTORY]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_EXPORT_BATCH_HISTORY]
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
/****** Object:  StoredProcedure [dbo].[usp_get_external_system_fields]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_external_system_fields]
	@external_system_field_id int,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @StartRow INT, @EndRow INT;	
	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;


	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY external_system_field_id ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM( 

		SELECT      external_system_field_id, external_system_fields.external_system_id, field_name,system_name 
		FROM            external_system_fields		
		INNER JOIN 	external_systems ON external_systems.external_system_id=external_system_fields.external_system_id
		WHERE external_system_field_id= COALESCE(@external_system_field_id, external_system_field_id) 

	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY external_system_field_id ASC
														   
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;		

END













GO
/****** Object:  StoredProcedure [dbo].[usp_get_external_systems]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_external_systems]
	@external_system_id int,
	@language_id int,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @StartRow INT, @EndRow INT;	
	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;


	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY external_system_id ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM( 

		SELECT        external_system_id, external_systems.external_system_type_id, system_name,[language_text] as 'external_system_type'
		FROM            external_systems		
		INNER JOIN external_system_types_language ON 	
		external_system_types_language.[external_system_type_id]=external_systems.external_system_type_id
		WHERE [language_id]= @language_id
			AND external_system_id= COALESCE(@external_system_id, external_system_id) 

	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY external_system_id ASC
														   
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;		

END













GO
/****** Object:  StoredProcedure [dbo].[usp_get_feerevenue_report]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_feerevenue_report]
	-- Add the parameters for the stored procedure here
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int,
	@fromdate datetime,
	@todate datetime,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
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

		SELECT [branch].branch_id,[branch].branch_code, case when cards.fee_waiver_YN =1 then count([branch_card_status_current].card_id) else 0 end as 'zero_no_fee',
case when cards.fee_overridden_YN =1 then count([branch_card_status_current].card_id) else 0 end as 'amended_fee',
case when cards.fee_overridden_YN =0 then count([branch_card_status_current].card_id) else 0 end as 'full_fee', sum(cards.fee_charged) as 'fee_charged'
			FROM [branch_card_status_current] 
					INNER JOIN [cards]
						ON [branch_card_status_current].card_id = [cards].card_id
					INNER JOIN [branch]
						ON [cards].branch_id = [branch].branch_id
			WHERE [branch_card_status_current].branch_card_statuses_id = 6
				and[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)	
			AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  AND [branch_card_status_current].[status_date] >= @fromdate 
				  AND [branch_card_status_current].[status_date] <= @todate
				  group by [branch].branch_id,[branch].branch_code,cards.fee_overridden_YN,cards.fee_waiver_YN

END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_file_history]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_file_history] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_file_historys]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_file_historys] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_file_load_list]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_file_load_list]
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
/****** Object:  StoredProcedure [dbo].[usp_get_fileloaderlog]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: 19 may 2014
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_fileloaderlog]
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
/****** Object:  StoredProcedure [dbo].[usp_get_fonts_ist]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_fonts_ist]
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
/****** Object:  StoredProcedure [dbo].[usp_get_fonts_list]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_fonts_list]
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
/****** Object:  StoredProcedure [dbo].[usp_get_groups_roles_for_user]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_groups_roles_for_user] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_interface_Rowaccountbaseinfo]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: 20/04/2014
-- Description:	to pass values to cms webservice
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_interface_Rowaccountbaseinfo]
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
/****** Object:  StoredProcedure [dbo].[usp_get_invetorysummaryreport]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [usp_get_invetorysummaryreport] null,1
CREATE PROCEDURE [dbo].[usp_get_invetorysummaryreport]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int =null
	,@audit_user_id bigint
	,@audit_workstation varchar(100)
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
		   INTERX.statuses_name,
		   INTERX.statuses_id,
		   COUNT(INTER.card_id)	AS CardCount,issuer.issuer_name as 'issuer_name'

	FROM 	 
		  (SELECT [branch].branch_id, [branch_card_status_current].branch_card_statuses_id as 'statuses_id', [branch_card_status_current].card_id
			FROM [branch_card_status_current]
					INNER JOIN [cards]
						ON [branch_card_status_current].card_id = [cards].card_id
					INNER JOIN [branch]
						ON [cards].branch_id = [branch].branch_id
							AND [branch].card_centre_branch_YN = 0
					WHERE    [branch].branch_status_id = 0
			) AS INTER
				RIGHT OUTER JOIN 			
			(SELECT issuer_id, branch_id, branch_code, [branch_card_statuses].branch_card_statuses_id as 'statuses_id', bcl.language_text as 'statuses_name',bcl.language_id
			 FROM	[branch], [branch_card_statuses]
			 inner join branch_card_statuses_language bcl on [branch_card_statuses].branch_card_statuses_id=bcl.branch_card_statuses_id
			 where branch.branch_status_id=0 AND branch.card_centre_branch_YN = 0
		 		) INTERX

			ON INTER.branch_id = INTERX.branch_id
				AND INTER.statuses_id = INTERX.statuses_id 				
	inner join issuer on issuer.issuer_id=INTERX.issuer_id

	WHERE INTERX.issuer_id = COALESCE(@issuer_id, INTERX.issuer_id)
		  AND INTERX.branch_id = COALESCE(@branch_id, INTERX.branch_id) and INTERX.language_id=@language_id
		  AND INTERX.statuses_id!=6 and INTERX.statuses_id!=7
		  AND INTERX.branch_code <> ''  
	GROUP BY INTERX.issuer_id, INTERX.branch_id, branch_code, statuses_name, INTERX.statuses_id,issuer_name
	ORDER BY issuer_name, INTERX.branch_code, INTERX.statuses_id

END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_issuecardsummaryreport]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [usp_get_invetorysummaryreport] null,1
CREATE PROCEDURE [dbo].[usp_get_issuecardsummaryreport]
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
/****** Object:  StoredProcedure [dbo].[usp_get_issuedcardsreport]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: 16/05/2014
-- Description:	for displaying issued card report
-- =============================================
----exec usp_get_issuedcardsreport 2,'6/14/2014 12:00:00 AM','6/27/2014 12:00:00 AM' ,25,null
CREATE PROCEDURE [dbo].[usp_get_issuedcardsreport]
	@isuerid int,
	@fromdate datetime,
	@todate datetime,
	@userid int = null,
	@branchid int=null,
	@language_id int = null
	,@audit_user_id bigint
	,@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if(@userid=0)
	set @userid=null

	if(@isuerid=0)
	set @isuerid=null

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)

    -- Insert statements for procedure here
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
		
	SELECT 
		branch.branch_code+'-'+ branch.branch_name as 'BranchCode'
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([operator].[username])) as 'IssuedBy'
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY(customer_account.customer_first_name))+' '+ CONVERT(VARCHAR(MAX),DECRYPTBYKEY(customer_account.customer_last_name)) as 'CustomerNames'
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY(customer_account.[customer_account_number])) as 'customeraccountNumber'
		, CASE 
			WHEN @mask_report = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
			ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
		  END AS 'card_number'
		, [operator].[user_id] as original_operator_user_id
		, [branch_card_status_current].[user_id]
		, cards.card_request_reference AS card_reference_number
		, [branch_card_statuses_name] as 'CardStatus'
		, [status_date] as'IssuedDate'
		, APPROVER.username as 'APPROVER USER'
		, language_text as 'Scenario'
		, [Cards].fee_charged as 'fee_Charged'
	FROM 
		[branch_card_status_current]
			INNER JOIN [cards] 
				ON [branch_card_status_current].card_id = [cards].card_id
			INNER JOIN [branch] 
				ON [cards].branch_id = [branch].branch_id
			INNER JOIN [customer_account] ON 
				[cards].card_id = [customer_account].card_id
			INNER JOIN [user] as [operator]
				ON [operator].[user_id] = [customer_account].[user_id]
			INNER JOIN [branch_card_statuses] 
				ON [branch_card_statuses].branch_card_statuses_id = [branch_card_status_current].branch_card_statuses_id
			INNER JOIN [card_issue_reason_language] 
				ON [card_issue_reason_language].card_issue_reason_id = [customer_account].card_issue_reason_id
			INNER JOIN [issuer] 
				ON [issuer].issuer_id = branch.issuer_id
			LEFT OUTER JOIN (
					SELECT CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].username)) as 'username' , card_id
					FROM [branch_card_status]
						INNER JOIN [user] 
							ON [user].[user_id] = [branch_card_status].[user_id]
					WHERE
						[branch_card_status].[branch_card_statuses_id] = 3) AS [APPROVER] 
				ON [cards].card_id = [APPROVER].card_id			
	 WHERE
		[branch_card_status_current].branch_card_statuses_id = 6 	
		AND [branch].issuer_id = COALESCE(@isuerid, [branch].issuer_id)
		--Match on both the original operator or the final person to have issued the card
		AND ([operator].[user_id] = COALESCE(@userid, [operator].[user_id]) OR
			  [branch_card_status_current].[user_id] = COALESCE(@userid, [branch_card_status_current].[user_id]))
	    AND [cards].branch_id = COALESCE(@branchid, [cards].branch_id) 
		AND [card_issue_reason_language].language_id = @language_id 
		AND [branch_card_status_current].status_date >= @fromdate 
		AND [branch_card_status_current].status_date <= @todate
	 ORDER BY
		CardStatus

	 CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_issuer]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_issuer] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_issuer_by_branch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_issuer_by_branch] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_issuer_by_code]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_issuer_by_code] 
	-- Add the parameters for the stored procedure here
	@issuer_code varchar(10),
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT TOP 1 *
	FROM [issuer]
	WHERE issuer_code = @issuer_code
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_issuer_by_interface]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_issuer_by_interface] 
	@connection_params_id INT,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT TOP 1 [connection_parameters].[connection_parameter_id],[connection_name],[address],[port],[path],[protocol],[auth_type],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([username])) as [username],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([password])) as [password]
		FROM [connection_parameters]
			INNER JOIN [issuer_interface]
				ON [issuer_interface].connection_parameter_id = [connection_parameters].connection_parameter_id
		WHERE [issuer_interface].connection_parameter_id = @connection_params_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END










GO
/****** Object:  StoredProcedure [dbo].[usp_get_issuer_by_product_and_branchcode]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_issuer_by_product_and_branchcode] 
	-- Add the parameters for the stored procedure here
	@bin_code char(6),
	@branch_code char(3) = NULL
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
			AND [branch].branch_code = COALESCE(@branch_code, [branch].branch_code)
END










GO
/****** Object:  StoredProcedure [dbo].[usp_get_issuer_interface_conn]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_issuer_interface_conn] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@interface_type_id int,
	@interface_area int,
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
			CONVERT(VARCHAR(max),DECRYPTBYKEY([username])) as [username],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([password])) as [password]
		FROM [connection_parameters]
			INNER JOIN [issuer_interface]
				ON [issuer_interface].connection_parameter_id = [connection_parameters].connection_parameter_id
		WHERE [issuer_interface].issuer_id = @issuer_id AND
			  [issuer_interface].interface_type_id = @interface_type_id AND
			  [issuer_interface].interface_area = @interface_area

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END










GO
/****** Object:  StoredProcedure [dbo].[usp_get_issuer_interface_details]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo, Setenane
-- Create date: 2014/03/20
-- Description:	Gets the issuer interface details based on the @interface_id
-- =============================================

CREATE PROCEDURE [dbo].[usp_get_issuer_interface_details]
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
/****** Object:  StoredProcedure [dbo].[usp_get_issuer_interfaces]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo, Setenane
-- Create date: 2014/03/25
-- Description:	Gets a list of issuer interfaces based on the @issuer_id
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_issuer_interfaces]
	@issuer_id int = NULL,
	@interface_type_id int = NULL,
	@interface_area int = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    
	SELECT *
	FROM [issuer_interface]
	WHERE issuer_id = COALESCE(@issuer_id, issuer_id) AND
		  interface_type_id = COALESCE(@interface_type_id, interface_type_id) AND
		  interface_area = COALESCE(@interface_area, interface_area)


END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_issuerdetails]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_issuerdetails]
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
/****** Object:  StoredProcedure [dbo].[usp_get_languages]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_languages] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_ldap_setting]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo, Setenane
-- Create date: 2014/04/02
-- Description:	Gets ldap settings for
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_ldap_setting]	
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
				CONVERT(VARCHAR(max),DECRYPTBYKEY([ldap_setting].username)) as domain_username,
				CONVERT(VARCHAR(max),DECRYPTBYKEY([ldap_setting].[password])) as domain_password,[ldap_setting].auth_type_id,external_inteface_id
		FROM [ldap_setting]
		WHERE [ldap_setting].ldap_setting_id = @ldap_setting_id
		 

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_ldap_settings]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_ldap_settings] 
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
			CONVERT(VARCHAR(max),DECRYPTBYKEY([username])) as [username],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([password])) as [password],[ldap_setting].auth_type_id,external_inteface_id
		FROM ldap_setting

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END










GO
/****** Object:  StoredProcedure [dbo].[usp_get_license_issuers]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch all issuers who are licensed/unlicensed
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_license_issuers] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_load_batch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Returns a single load batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_load_batch] 
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

			SELECT 
				DISTINCT
				CAST(1 as BIGINT) as ROW_NO
				, 1 AS TOTAL_ROWS
				, 1 as TOTAL_PAGES
				, [load_batch].load_batch_id
				, [load_batch].load_date
				, [load_batch].load_batch_reference
				, [load_batch].no_cards
				, [load_batch_status].load_batch_statuses_id
				, [load_batch_status].status_notes
				, [load_batch_statuses_language].language_text AS 'load_batch_status_name'
				, branch.branch_name as 'BranchName'
				--, branch.branch_code as 'BranchCode'
			FROM [load_batch] 
			INNER JOIN [load_batch_status] ON [load_batch_status].load_batch_id = [load_batch].load_batch_id
			INNER JOIN [load_batch_statuses_language] ON 
						[load_batch_status].load_batch_statuses_id = [load_batch_statuses_language].load_batch_statuses_id	
						AND [load_batch_statuses_language].language_id = @language_id
			INNER JOIN [load_batch_cards] ON load_batch_cards.load_batch_id = load_batch.load_batch_id
			INNER JOIN cards ON cards.card_id = load_batch_cards.card_id
			INNER JOIN branch ON branch.branch_id = cards.branch_id	
			WHERE 
				[load_batch].load_batch_id = @load_batch_id	
				AND [load_batch_status].status_date = (SELECT MAX(lbs2.status_date)
													   FROM [load_batch_status] lbs2
													   WHERE lbs2.load_batch_id = [load_batch].load_batch_id)		

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting Load batch with id: ' + CAST(@load_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_get_load_batch_cards]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch all cards for load batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_load_batch_cards] 
	@load_batch_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_LOAD_BATCH_CARDS]
		BEGIN TRY 
			DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				SELECT 
					CASE 
						WHEN @mask_report = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
						ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
					END AS 'card_number'
					--[dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) AS 'card_number'					
					, cards.card_request_reference AS card_reference_number
					, [load_card_statuses].load_card_status	, branch.branch_code
					, branch.branch_name  				   
				FROM [cards]
					INNER JOIN [load_batch_cards] 
						ON [load_batch_cards].card_id = [cards].card_id	
					INNER JOIN [load_card_statuses]
						ON 	[load_batch_cards].load_card_status_id = [load_card_statuses].load_card_status_id
					INNER JOIN branch
						ON branch.branch_id = cards.branch_id
					INNER JOIN [issuer]
						ON [issuer].issuer_id = branch.issuer_id
				WHERE [load_batch_cards].load_batch_id = @load_batch_id
					
														   
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;		

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting cards for load batch with id: ' + CAST(@load_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_get_load_batch_history]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get status hostory for a load batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_load_batch_history] 
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
					   CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) as 'username',
					   CONVERT(VARCHAR(max),DECRYPTBYKEY([user].first_name)) as 'first_name',
					   CONVERT(VARCHAR(max),DECRYPTBYKEY([user].last_name)) as 'last_name'   
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
			--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_get_load_batches]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_get_load_batches]
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
					   --[branch].[branch_code]+' '+ [branch].branch_name as 'BranchName'
					   '-' as 'BranchName'
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
			--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_get_masterkey]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi	
-- Create date: 20150220
-- Description:	Get masterkey
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_masterkey]
	@masterkey_id INT
AS
BEGIN
	SET NOCOUNT ON;
	
		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate; 

		SELECT
			[masterkey_name]
			, CONVERT(varchar(max),DECRYPTBYKEY([masterkey])) AS 'masterkey'
			,[issuer_id]
		FROM 
			[masterkeys]
		WHERE 
			[masterkey_id] = @masterkey_id

		CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_next_sequence]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_next_sequence] 
	-- Add the parameters for the stored procedure here
	@sequence_name varchar(100),
	@reset_period int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_SEQUENCE_TRAN]
	BEGIN TRY 

		--Reset period
		-- 0 = None
		-- 1 = Daily
		-- 2 = Weekly
		-- 3 = Monthly
		-- 4 = Yearly
		UPDATE [sequences]
			SET last_sequence_number = 0,
				last_updated = GETDATE()
		WHERE sequence_name = @sequence_name AND
				( (@reset_period = 1 AND DATEDIFF(day, last_updated, GETDATE()) != 0) OR
				  (@reset_period = 2 AND DATEDIFF(week, last_updated, GETDATE()) != 0) OR
				  (@reset_period = 3 AND DATEDIFF(month, last_updated, GETDATE()) != 0) OR
				  (@reset_period = 4 AND DATEDIFF(year, last_updated, GETDATE()) != 0) )

		DECLARE @last_seq_number bigint,
				@next_seq_number bigint

		-- Insert statements for procedure here
		SELECT @last_seq_number = last_sequence_number
		FROM [sequences]
		WHERE sequence_name = @sequence_name


		IF(@last_seq_number IS NOT NULL)
		BEGIN
			SET @next_seq_number = @last_seq_number + 1

			UPDATE [sequences]
			SET last_sequence_number = @next_seq_number,
				last_updated = GETDATE()
			WHERE sequence_name = @sequence_name
		END
		ELSE
		BEGIN
			SET @next_seq_number = 1

			INSERT INTO [sequences] (sequence_name, last_sequence_number, last_updated)
				VALUES (@sequence_name, @next_seq_number, GETDATE())
		END

		COMMIT TRANSACTION [GET_SEQUENCE_TRAN]

		SELECT @next_seq_number

	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_LOAD_BATCH_TRAN]
		RETURN ERROR_MESSAGE()
	END CATCH 
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_operator_cards_inprogress]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_operator_cards_inprogress] 
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

	DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)

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
				--CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) AS card_number,
				CASE 
					WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
					ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
				END AS 'card_number',	
				[cards].product_id, --[cards].sub_product_id, 
				[cards].card_priority_id, 
				[cards].card_issue_method_id, [cards].card_request_reference,			   
				[branch_card_statuses_language].language_text AS current_card_status,
				[branch_card_status_current].comments,						
				[branch_card_status_current].status_date,	
				[branch_card_status_current].branch_card_statuses_id,					
				
				[branch_card_status_current].operator_user_id, 
				'' AS operator_username, 
				'' AS product_bin_code,		
				'' AS sub_product_code,	
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
			INNER JOIN [issuer_product]
					ON [issuer_product].product_id = [cards].product_id
		WHERE  [branch].branch_status_id = 0					 
				AND [issuer].issuer_status_id = 0	
				AND [cards].card_issue_method_id = 1
				AND [branch_card_status_current].operator_user_id = @user_id				 	  
				AND (([branch_card_status_current].branch_card_statuses_id = 2
						AND [issuer].maker_checker_YN = 0)
					OR ([branch_card_status_current].branch_card_statuses_id = 3
						AND [cards].card_issue_method_id = 1)
					OR [branch_card_status_current].branch_card_statuses_id = 11
					OR (( ([issuer].enable_instant_pin_YN = 1 AND [issuer_product].enable_instant_pin_YN = 0) OR
							([issuer].enable_instant_pin_YN = 0))
							 AND [branch_card_status_current].branch_card_statuses_id = 4))
		UNION
		SELECT DISTINCT [cards].card_id, 
				--CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) AS card_number,
				CASE 
					WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
					ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
				END AS 'card_number',	
				[cards].product_id, --[cards].sub_product_id, 
				[cards].card_priority_id, 
				[cards].card_issue_method_id, [cards].card_request_reference,		   
				[branch_card_statuses_language].language_text AS current_card_status,
				[branch_card_status_current].comments,						
				[branch_card_status_current].status_date,	
				[branch_card_status_current].branch_card_statuses_id,	
				
				[branch_card_status_current].operator_user_id,
				'' AS operator_username, 
				'' AS product_bin_code,
				'' AS sub_product_code,						
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
			INNER JOIN [issuer_product]
					ON [issuer_product].product_id = [cards].product_id
						AND [issuer_product].enable_instant_pin_YN = 1
			WHERE  [branch].branch_status_id = 0	 
					AND [issuer].issuer_status_id = 0	
					--AND [cards].card_issue_method_id = 1	
					AND [issuer].enable_instant_pin_YN = 1	  
					AND ([branch_card_status_current].branch_card_statuses_id = 4
						OR [branch_card_status_current].branch_card_statuses_id = 5)
	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY status_date DESC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END









GO
/****** Object:  StoredProcedure [dbo].[usp_get_parameters_issuer_interface]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_parameters_issuer_interface] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@interface_type_id int,
	@interface_area int,
	@interface_guid char(36) = null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

    SELECT issuer_interface.interface_guid
		, connection_parameters.[connection_parameter_id]
		, connection_parameters.connection_parameter_type_id
		, [connection_name]
		, [address]
		, [port]
		, [path]
		, [protocol]
		, [auth_type]
		, [header_length]
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([identification])) as identification
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([username])) as [username]
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([password])) as [password]
		, connection_parameter_type_id
		, timeout_milli
		, buffer_size
		, doc_type
		, name_of_file
		, file_delete_YN
		, file_encryption_type_id
		, duplicate_file_check_YN
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([private_key])) as [private_key]
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([public_key])) as [public_key]
	FROM connection_parameters
		INNER JOIN issuer_interface
			ON connection_parameters.connection_parameter_id = issuer_interface.connection_parameter_id
	WHERE issuer_interface.issuer_id = COALESCE(@issuer_id, issuer_interface.issuer_id)
			AND issuer_interface.interface_type_id = @interface_type_id
			AND issuer_interface.interface_area = @interface_area
			AND issuer_interface.interface_guid = COALESCE(@interface_guid, issuer_interface.interface_guid)

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END





GO
/****** Object:  StoredProcedure [dbo].[usp_get_parameters_product_interface]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_parameters_product_interface] 
	-- Add the parameters for the stored procedure here
	@product_id int = null,
	@interface_type_id int,
	@interface_area int,
	@interface_guid char(36) = null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

    SELECT  product_interface.interface_guid
			, connection_parameters.[connection_parameter_id]
			, connection_parameters.connection_parameter_type_id
			, [connection_name],[address],[port],[path],[protocol],
			[auth_type],[header_length],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([identification])) as identification,
			CONVERT(VARCHAR(max),DECRYPTBYKEY([username])) as [username],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([password])) as [password], 
			connection_parameter_type_id,
			timeout_milli, buffer_size, doc_type, name_of_file, file_delete_YN, file_encryption_type_id, duplicate_file_check_YN,
			CONVERT(VARCHAR(max),DECRYPTBYKEY([private_key])) as [private_key],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([public_key])) as [public_key]
	FROM connection_parameters
		INNER JOIN product_interface
			ON connection_parameters.connection_parameter_id = product_interface.connection_parameter_id
	WHERE product_interface.product_id = COALESCE(@product_id, product_interface.product_id)
			AND product_interface.interface_type_id = @interface_type_id
			AND product_interface.interface_area = @interface_area
			AND product_interface.interface_guid = COALESCE(@interface_guid, product_interface.interface_guid)

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END





GO
/****** Object:  StoredProcedure [dbo].[usp_get_passwords_by_user_id]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 26 March 2014
-- Description:	Returns decrypted passwords
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_passwords_by_user_id] 
	@user_id bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		--For the current password add 99 days to show it's the latest. old passwords should be in the past.
		SELECT CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[password])) as 'password', DATEADD(DAY, 99, GETDATE()) as 'date'						
		FROM [user]
		WHERE [user].[user_id] = @user_id
		UNION ALL
		SELECT CONVERT(VARCHAR(max),DECRYPTBYKEY([user_password_history].[password_history])) as 'password', 
			   [user_password_history].[date_changed] as 'date'						
		FROM [user_password_history]
		WHERE [user_password_history].[user_id] = @user_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_passwordshistorystatus]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_passwordshistorystatus]
	@user_id int,
	@password varchar(100),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			IF((SELECT COUNT(*) FROM  [user_password_history] WHERE 
			CONVERT(VARCHAR(max),DECRYPTBYKEY([user_password_history].[password_history]))=@password and [user_password_history].[user_id] = @user_id)>0)
			set @ResultCode=0
			else 
			set @ResultCode=1

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
			END TRY
	BEGIN CATCH
		
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
/****** Object:  StoredProcedure [dbo].[usp_get_pin_batch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Retreive a pin batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_pin_batch] 
	@pin_batch_id bigint,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_PIN_BATCH]
		BEGIN TRY 

			SELECT CAST(1 as BIGINT) as ROW_NO, 1 AS TOTAL_ROWS, 1 as TOTAL_PAGES, 
				   [pin_batch].pin_batch_id, [pin_batch].date_created, [pin_batch].pin_batch_reference, 
				   [pin_batch].no_cards, [pin_batch_status_current].pin_batch_statuses_id, 
				   [pin_batch_status_current].status_notes, [pin_batch_statuses_language].language_text AS 'pin_batch_status_name', 
				   [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
				   [card_issue_method_language].language_text AS 'card_issue_method_name',
				   [pin_batch].card_issue_method_id, [pin_batch].pin_batch_type_id,
				   [branch].branch_name, [branch].branch_code,
				   [pin_batch_statuses_flow].flow_pin_batch_statuses_id, 
				   [pin_batch_statuses_flow].flow_pin_batch_type_id,
				   [pin_batch_statuses_flow].user_role_id,
				   [pin_batch_statuses_flow].reject_pin_batch_statuses_id  
			FROM [pin_batch] 
				INNER JOIN [pin_batch_status_current]
					ON [pin_batch].pin_batch_id = [pin_batch_status_current].pin_batch_id
				INNER JOIN [pin_batch_statuses_language]
					ON [pin_batch_status_current].pin_batch_statuses_id = [pin_batch_statuses_language].pin_batch_statuses_id							
				INNER JOIN [issuer]
					ON [issuer].issuer_id = [pin_batch].issuer_id
				INNER JOIN [card_issue_method_language]
					ON [pin_batch].card_issue_method_id = [card_issue_method_language].card_issue_method_id
						AND [card_issue_method_language].language_id = @language_id	
						
				LEFT OUTER JOIN [branch]
					ON [branch].branch_id = [pin_batch].branch_id

				LEFT OUTER JOIN [pin_batch_statuses_flow]
					ON [pin_batch_status_current].pin_batch_statuses_id = [pin_batch_statuses_flow].pin_batch_statuses_id
						AND [pin_batch].card_issue_method_id = [pin_batch_statuses_flow].card_issue_method_id
						AND [pin_batch_statuses_flow].pin_batch_type_id = [pin_batch].pin_batch_type_id
			WHERE [pin_batch].pin_batch_id = @pin_batch_id
					AND [pin_batch_statuses_language].language_id = @language_id		

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting dist batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_PIN_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_PIN_BATCH]
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
/****** Object:  StoredProcedure [dbo].[usp_get_pin_batch_card_details]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_pin_batch_card_details] 
	-- Add the parameters for the stored procedure here
	@pin_batch_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT 
			 
				CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)) as 'card_number', 
				[cards].branch_id, [cards].card_id, [cards].card_issue_method_id,
				[cards].card_priority_id, [cards].card_request_reference, [cards].card_sequence,
				[cards].product_id,
				CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_activation_date)), 109) as 'card_activation_date', 
				CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_expiry_date)), 109) as 'card_expiry_date', 
				CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_production_date)), 109) as 'card_production_date',						
				CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].pvv)) as 'pvv',
				[pin_batch_cards].pin_batch_id,
				[pin_batch_cards].pin_batch_cards_statuses_id,
				[pin_batch_card_statuses].pin_batch_card_statuses_name,
				[customer_account].account_type_id, [customer_account].cms_id, 
				[customer_account].contract_number, [customer_account].currency_id, [customer_account].customer_account_id,
				CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_account_number)) as 'customer_account_number', 
				CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].name_on_card)) as 'name_on_card',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].Id_number)) as 'Id_number',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].contact_number)) as 'contact_number',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].CustomerId)) as 'CustomerId',
				[customer_account].customer_title_id, [customer_account].customer_type_id,
				[customer_account].date_issued,   [customer_account].resident_id,
				[customer_account].[user_id],
				[issuer].issuer_id, [issuer].issuer_code, [issuer].issuer_name,
				[branch].branch_code, [branch].branch_name,
				[issuer_product].[product_code],
				[issuer_product].sub_product_code,
				[issuer_product].[product_name],
				[issuer_product].[product_bin_code],
				[issuer_product].[src1_id],
				[issuer_product].[src2_id],
				[issuer_product].[src3_id],
				--[issuer_product].sub_product_id_length,
				CONVERT(INT, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVKI]))) as 'PVKI',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVK])) as 'PVK',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKA])) as 'CVKA',
				CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKB])) as 'CVKB',
				[issuer_product].[expiry_months]
				--[sub_product].sub_product_id,
				--[sub_product].sub_product_name,
				--[sub_product].sub_product_code
		FROM [cards]
			INNER JOIN [pin_batch_cards] 
				ON [pin_batch_cards].card_id = [cards].card_id	
			INNER JOIN [pin_batch_card_statuses]
				ON 	[pin_batch_cards].pin_batch_cards_statuses_id = [pin_batch_card_statuses].pin_batch_card_statuses_id
			INNER JOIN [branch]
				ON [cards].branch_id = [branch].branch_id
			INNER JOIN [issuer]
				ON [branch].issuer_id = [issuer].issuer_id
			INNER JOIN [issuer_product]
				ON [cards].product_id = [issuer_product].product_id
			LEFT OUTER JOIN [customer_account]
				ON [cards].card_id = [customer_account].card_id
			--LEFT OUTER JOIN [sub_product]
			--	ON [cards].sub_product_id = [sub_product].sub_product_id
			--		AND [cards].product_id = [sub_product].product_id
		WHERE [pin_batch_cards].pin_batch_id = @pin_batch_id					
														   
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_pin_batch_cards]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_get_pin_batch_cards] 
	@pin_batch_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_PIN_BATCH_CARDS]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;
			
			SELECT 
				[dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY(cards.card_number)),6,4) AS 'card_number'
				, cards.card_request_reference AS card_reference_number
				, pin_batch_statuses.pin_batch_statuses_name
			FROM 
				[cards]
				INNER JOIN pin_batch_cards ON cards.card_id = pin_batch_cards.card_id
				INNER JOIN pin_batch_statuses ON pin_batch_cards.pin_batch_cards_statuses_id = pin_batch_statuses.pin_batch_statuses_id
			WHERE pin_batch_id = @pin_batch_id

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;		
			
			COMMIT TRANSACTION [GET_PIN_BATCH_CARDS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_PIN_BATCH_CARDS]
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
/****** Object:  StoredProcedure [dbo].[usp_get_pin_batch_history]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_get_pin_batch_history] 
	@pin_batch_id bigint,	
	@languageId int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_PIN_BATCH_HISTORY]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			SELECT 
				pb.pin_batch_reference
				, pb.no_cards
				, pbs.status_date
				, '' AS status_notes -- [pin_batch_status].status_notes
				, pbsl.language_text as pin_batch_status_name
				, CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'username'
				, CONVERT(VARCHAR,DECRYPTBYKEY([user].first_name)) as 'first_name'
				, CONVERT(VARCHAR,DECRYPTBYKEY([user].last_name)) as 'last_name'   
			FROM 
				[pin_batch] pb
				INNER JOIN [pin_batch_status] pbs ON pbs.pin_batch_id = pb.pin_batch_id
				INNER JOIN pin_batch_statuses pbss ON pbs.pin_batch_statuses_id = pbss.pin_batch_statuses_id
				INNER JOIN pin_batch_statuses_language pbsl ON pbsl.pin_batch_statuses_id = pbs.pin_batch_statuses_id
				INNER JOIN [user] ON pbs.[user_id] = [user].[user_id]
			WHERE 
				pb.pin_batch_id = @pin_batch_id
				AND pbsl.language_id = @languageId
			GROUP BY	
				pb.pin_batch_reference
				, pb.no_cards
				, pbs.status_date
				, pbsl.language_text
				, CONVERT(VARCHAR,DECRYPTBYKEY([user].[username]))
				, CONVERT(VARCHAR,DECRYPTBYKEY([user].first_name)) 
				, CONVERT(VARCHAR,DECRYPTBYKEY([user].last_name))
			ORDER BY 
				pbs.status_date ASC	

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting history for distribution batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_PIN_BATCH_HISTORY]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_PIN_BATCH_HISTORY]
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
/****** Object:  StoredProcedure [dbo].[usp_get_pin_batches_for_user]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Description:	Gets a list of pin batches for a user
-- =============================================

--exec usp_get_pin_batches_for_user 3,null,null,null,null,null,null,null,null,1,20,17,'veneka-13'
CREATE PROCEDURE [dbo].[usp_get_pin_batches_for_user] 
	-- Add the parameters for the stored procedure here
	@issuerId int = NULL,
	@pin_batch_reference VARCHAR(50) = NULL,
	@pin_batch_statuses_id int = NULL,
	@branch_id INT = NULL,
	@card_issue_method_id int = NULL,
	@pin_batch_type_id int = NULL,
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

	BEGIN TRANSACTION [GET_PIN_BATCH_FOR_USER_TRAN]
		BEGIN TRY 
			IF(@pin_batch_reference = '' or @pin_batch_reference IS NULL)
				SET @pin_batch_reference = ''
				
			IF @date_end IS NOT NULL
				SET @date_end = DATEADD(DAY, 1, @date_end)

			DECLARE @StartRow INT, @EndRow INT;			

			SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
			SET @EndRow = @StartRow + @RowsPerPage - 1;

			--append#1
			WITH PAGE_ROWS
			AS
			(
			SELECT ROW_NUMBER() OVER(ORDER BY date_created DESC, pin_batch_reference ASC) AS ROW_NO
					, COUNT(*) OVER() AS TOTAL_ROWS
					, *
			FROM( 
				SELECT [pin_batch].pin_batch_id, [pin_batch].date_created, [pin_batch].pin_batch_reference, 
					   [pin_batch].no_cards, [pin_batch_status_current].status_notes,
					   [pin_batch_status_current].pin_batch_statuses_id, [pin_batch_statuses_language].language_text AS 'pin_batch_status_name', 
					   [issuer].[issuer_id], [issuer].issuer_name, [issuer].issuer_code,
					   [card_issue_method_language].language_text AS 'card_issue_method_name',
					   [pin_batch].card_issue_method_id, [pin_batch].pin_batch_type_id,
					   [branch].branch_name, [branch].branch_code,
					   [pin_batch_statuses_flow].flow_pin_batch_statuses_id, 
					   [pin_batch_statuses_flow].flow_pin_batch_type_id,
					   [pin_batch_statuses_flow].user_role_id,
					   [pin_batch_statuses_flow].reject_pin_batch_statuses_id 				   
				FROM [pin_batch] 
					INNER JOIN [pin_batch_status_current]
						ON [pin_batch_status_current].pin_batch_id = [pin_batch].pin_batch_id
					INNER JOIN [pin_batch_statuses_language]
						ON [pin_batch_status_current].pin_batch_statuses_id = [pin_batch_statuses_language].pin_batch_statuses_id
							AND [pin_batch_statuses_language].language_id = @language_id								
					INNER JOIN [issuer]
						ON [issuer].issuer_id = [pin_batch].issuer_id
					INNER JOIN [card_issue_method_language]
						ON [pin_batch].card_issue_method_id = [card_issue_method_language].card_issue_method_id
							AND [card_issue_method_language].language_id = @language_id
									
					LEFT OUTER JOIN [branch]
						ON [branch].branch_id = [pin_batch].branch_id
					LEFT OUTER JOIN [pin_batch_statuses_flow]
						ON [pin_batch_status_current].pin_batch_statuses_id = [pin_batch_statuses_flow].pin_batch_statuses_id
							AND [pin_batch].card_issue_method_id = [pin_batch_statuses_flow].card_issue_method_id
							AND [pin_batch_statuses_flow].pin_batch_type_id = [pin_batch].pin_batch_type_id	
				WHERE 
					(([pin_batch].branch_id IS NULL 
						AND [pin_batch].issuer_id IN (SELECT issuer_id FROM [user_roles_issuer] WHERE user_role_id IN (4, 12, 13, 14, 15) AND [user_id] = @audit_user_id))
					OR 
					  ([pin_batch].branch_id IS NOT NULL 
						AND [pin_batch].branch_id IN (SELECT branch_id FROM [user_roles_branch] WHERE user_role_id IN (4, 12, 13, 14, 15) AND [user_id] = @audit_user_id)))

					--AND [pin_batch].pin_batch_reference LIKE COALESCE(@pin_batch_reference, [pin_batch].pin_batch_reference)
					AND ([pin_batch].pin_batch_reference like '%'+@pin_batch_reference+'%')
					AND [pin_batch_status_current].pin_batch_statuses_id = COALESCE(@pin_batch_statuses_id, [pin_batch_status_current].pin_batch_statuses_id)
					AND [pin_batch].card_issue_method_id = COALESCE(@card_issue_method_id, [pin_batch].card_issue_method_id)
					AND [pin_batch].pin_batch_type_id = COALESCE(@pin_batch_type_id, [pin_batch].pin_batch_type_id)
					AND [pin_batch].date_created >= COALESCE(@date_start, [pin_batch].date_created)
					AND [pin_batch].date_created <= COALESCE(@date_end, [pin_batch].date_created)
					--AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
					--AND [branch].branch_status_id = 0	 
					AND [issuer].issuer_status_id = 0
					AND [issuer].issuer_id = COALESCE(@issuerId,  [issuer].issuer_id)
			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY date_created DESC, pin_batch_reference ASC

			--log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 'Getting Distribution batches for user.', 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_PIN_BATCH_FOR_USER_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_PIN_BATCH_FOR_USER_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_get_pin_block_format]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_pin_block_format]
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT pin_block_formatid, pin_block_format from pin_block_format
END


GO
/****** Object:  StoredProcedure [dbo].[usp_get_pin_mailer_reprint_requests]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_pin_mailer_reprint_requests] 
	@issuer_id int,
	@branch_id int = null,
	@languageId int,	
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @pin_mailer_reprint_status_id int
	SET @pin_mailer_reprint_status_id = 1

	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY product_name ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(		
		SELECT issuer_product.product_id,
				issuer_product.product_name,
				COUNT(cards.card_id) as 'cardscount'
		FROM [cards]
				INNER JOIN [issuer_product] 
					ON [cards].product_id = [issuer_product].product_id 
				INNER JOIN	[pin_mailer_reprint_status_current] 
					ON [pin_mailer_reprint_status_current].card_id = [cards].card_id 
		WHERE cards.[card_issue_method_id] = 0 
				AND	[pin_mailer_reprint_status_current].pin_mailer_reprint_status_id = @pin_mailer_reprint_status_id
				AND issuer_id = @issuer_id 
				AND cards.branch_id = COALESCE(@branch_id, cards.branch_id)
		GROUP BY issuer_product.product_id, product_name   
	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY product_name ASC

	
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_pin_reissue]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_pin_reissue]
	-- Add the parameters for the stored procedure here
	@pin_reissue_id bigint,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

		DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)

		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate

		SELECT [pin_reissue].*,
					CASE 
						WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([pin_reissue].pan)),6,4) 
						ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([pin_reissue].pan))
					END AS 'card_number',
					[pin_reissue_status_current].pin_reissue_statuses_id,
					[pin_reissue_status_current].status_date,
					[pin_reissue_status_current].[user_id],
					[pin_reissue_status_current].comments,
					[pin_reissue_statuses_language].language_text as pin_reissue_statuses_name,
					CONVERT(VARCHAR(max),DECRYPTBYKEY([user].username)) as operator_usename,
					CONVERT(VARCHAR(max),DECRYPTBYKEY([authoriser].username)) as authorise_username,
					[issuer].issuer_name, [issuer].issuer_code,
					[branch].branch_code, [branch].branch_name,
					[issuer_product].product_code, [issuer_product].product_name,
					[issuer].authorise_pin_reissue_YN,
					CAST(1 as BIGINT) as ROW_NO, 1 AS TOTAL_ROWS, 1 as TOTAL_PAGES
			FROM [pin_reissue]
				INNER JOIN [pin_reissue_status_current]
					ON [pin_reissue].pin_reissue_id = [pin_reissue_status_current].pin_reissue_id
				INNER JOIN [pin_reissue_statuses_language]
					ON [pin_reissue_status_current].pin_reissue_statuses_id = [pin_reissue_statuses_language].pin_reissue_statuses_id
						AND [pin_reissue_statuses_language].language_id = @language_id
				INNER JOIN [user]
					ON [user].[user_id] = [pin_reissue].operator_user_id
				INNER JOIN [issuer]
					ON [issuer].issuer_id = [pin_reissue].issuer_id
				INNER JOIN [branch]
					ON [branch].branch_id = [pin_reissue].branch_id
				INNER JOIN [issuer_product]
					ON [issuer_product].product_id = [pin_reissue].product_id
				LEFT OUTER JOIN [user] as [authoriser]
						ON [authoriser].[user_id] = [pin_reissue].authorise_user_id
				
			WHERE [pin_reissue].pin_reissue_id = @pin_reissue_id
			
		CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key	

END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_pinreissue_report]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_pinreissue_report]
	@isuerid int,
	@fromdate datetime,
	@todate datetime,
	@userid int = null,
	@branchid int=null,
	@language_id int = null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(@userid=0)
	set @userid=null

	if(@isuerid=0)
	set @isuerid=null

	
	if(@branchid=0)
	set @branchid=null

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)

    -- Insert statements for procedure here
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		 SELECT branch.branch_code+'-'+branch.branch_name as 'branchcode', 
				issuer.issuer_name, 
				pin_reissue.pan, 
				CONVERT(VARCHAR,DECRYPTBYKEY([user].username)) as 'IssuedBy',  
				CONVERT(VARCHAR,DECRYPTBYKEY(apporved.username)) as 'APPROVER_USER', 
				CASE 
					WHEN pin_reissue_status_current.pin_reissue_statuses_id = 2 
					THEN pin_reissue_status_current.status_date 
					ELSE pin_reissue.reissue_date 
				END AS 'approveddate',
				CASE 
					WHEN @mask_report = 1 
					THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY(pin_reissue.pan)),6,4) 
					ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY(pin_reissue.pan))
				END AS 'cardnumber', 
				'pin re-issue' as Reason,
				pin_reissue.reissue_date as 'ReIssuedDate'
			FROM pin_reissue 
					LEFT JOIN pin_reissue_status_current 
						ON pin_reissue.pin_reissue_id = pin_reissue_status_current.pin_reissue_id
                    INNER JOIN  branch 
						ON pin_reissue.branch_id = branch.branch_id 
					INNER JOIN issuer 
						ON pin_reissue.issuer_id = issuer.issuer_id 
					INNER JOIN [user] 
						ON pin_reissue.operator_user_id = [user].[user_id] 
					LEFT JOIN [user] as apporved 
						ON pin_reissue.authorise_user_id = apporved.[user_id] 
			WHERE 
				--issuer.issuer_id=COALESCE(@isuerid, issuer.issuer_id)AND
				branch.branch_id = COALESCE(@branchid, branch.branch_id)
				AND pin_reissue.reissue_date >= @fromdate 
				AND pin_reissue.reissue_date <= @todate 
				--AND pin_reissue_status_current.pin_reissue_statues_id=1

   CLOSE SYMMETRIC KEY Indigo_Symmetric_Key


END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_printfields_byproductid]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_printfields_byproductid]
	@Productid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
		
		
		SELECT  distinct      product_fields.field_name, print_field_types.print_field_name, product_fields.product_field_id,product_fields.mapped_name,product_fields.[editable],
		CONVERT(VARCHAR(max),DECRYPTBYKEY('')) as 'Value',product_fields.deleted
					FROM  product_fields 
					--left JOIN
     --                    customer_fields ON product_fields.product_field_id = customer_fields.product_field_id 
						 INNER JOIN
                         print_field_types ON product_fields.print_field_type_id = print_field_types.print_field_type_id
						 where product_fields.product_id=@Productid 
						 --and Upper(product_fields.Mapped_Name)<>'IND_SYS_NOC'
 CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END


GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_account_types]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_account_types]
	-- Add the parameters for the stored procedure here
	@product_id int,
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
	FROM [products_account_types]
	WHERE product_id = @product_id
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_base_currency]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_base_currency] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_product_by_interfaces]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi
-- Create date: 2015/10/08
-- Description:	Gets a list of product based on the interface
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_by_interfaces]
	@connection_parameter_id int = NULL,
	--@interface_id int = NULL,
	--@interface_area int = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    
	SELECT *
	FROM [product_interface]
	WHERE connection_parameter_id = @connection_parameter_id
	--product_id = COALESCE(@product_id, product_id) AND
	--	  interface_type_id = COALESCE(@interface_type_id, interface_type_id) AND
	--	  interface_area = COALESCE(@interface_area, interface_area)


END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_currency]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_currency] 
	-- Add the parameters for the stored procedure here
	@product_id int, 
	@currency_id int = null,
	@active_YN bit = null,
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
	FROM [currency] LEFT OUTER JOIN [product_currency]
		ON [currency].currency_id = [product_currency].currency_id
		AND [product_currency].product_id = @product_id
	WHERE ((@active_YN IS NULL) OR ([currency].active_YN = @active_YN))
	 AND ((@currency_id IS NULL) OR ([currency].currency_id = @currency_id))

END

GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_details_by_product]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_details_by_product] 
	-- Add the parameters for the stored procedure here
	@product_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT DISTINCT
		   [product_fee_detail].fee_scheme_id,
		   [product_fee_detail].fee_detail_id,
		   [product_fee_detail].fee_detail_name,
		   [product_fee_detail].fee_waiver_YN,
		   [product_fee_detail].fee_editable_YN
	FROM [issuer_product]
			INNER JOIN [product_fee_detail]
				ON [issuer_product].fee_scheme_id = [product_fee_detail].fee_scheme_id
	WHERE [issuer_product].product_id = @product_id


END





GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_external_system_fields]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_external_system_fields] 
	-- Add the parameters for the stored procedure here
	@external_system_type_id int, 
	@product_id int,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT PES.external_system_field_id,PES.product_id,PES.field_value, [external_system_fields].external_system_id,[external_system_fields].field_name,[external_systems].external_system_type_id
	FROM [product_external_system] PES INNER JOIN [external_system_fields]
			ON PES.external_system_field_id = [external_system_fields].external_system_field_id
		INNER JOIN [external_systems]
			ON [external_system_fields].external_system_id = [external_systems].external_system_id
	WHERE [external_systems].external_system_type_id =  COALESCE(@external_system_type_id,[external_systems].external_system_type_id)
			AND PES.product_id = @product_id
END

GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_fee_charges]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_fee_charges] 
	-- Add the parameters for the stored procedure here
	@fee_detail_id int,
	@card_issue_reason_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [currency].*, ISNULL([product_fee_charge].fee_charge, 0) as fee_charge
	FROM [currency]	
			LEFT OUTER JOIN [product_fee_charge]
				ON [currency].currency_id = [product_fee_charge].currency_id
					AND [product_fee_charge].fee_detail_id = @fee_detail_id
					AND [product_fee_charge].card_issue_reason_id = @card_issue_reason_id
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_fee_details]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_fee_details]
	@fee_scheme_id int	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT *
	FROM product_fee_detail
	WHERE fee_scheme_id = @fee_scheme_id
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_fee_scheme]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_fee_scheme]
	-- Add the parameters for the stored procedure here
	@fee_scheme_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [product_fee_scheme].*, [issuer].issuer_name, 
				0 as TOTAL_ROWS, 
				CONVERT(bigint, 0) AS ROW_NO,
				0 as TOTAL_PAGES
	FROM [product_fee_scheme]
		INNER JOIN [issuer]	
			ON [product_fee_scheme].issuer_id = [issuer].issuer_id
	WHERE [product_fee_scheme].fee_scheme_id = @fee_scheme_id
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_fee_scheme_list]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_fee_scheme_list] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY fee_scheme_name ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
		SELECT [product_fee_scheme].*, [issuer].issuer_name
		FROM [product_fee_scheme]
			INNER JOIN [issuer]	
				ON [product_fee_scheme].issuer_id = [issuer].issuer_id
		WHERE [product_fee_scheme].issuer_id = COALESCE(@issuer_id, [product_fee_scheme].issuer_id)
	)
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY fee_scheme_name ASC

END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_interfaces]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 2015/07/28
-- Description:	Gets a list of product interfaces based on the @product_id
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_interfaces]
	@product_id int = NULL,
	@interface_type_id int = NULL,
	@interface_area int = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    
	SELECT *
	FROM [product_interface]
	WHERE product_id = COALESCE(@product_id, product_id) AND
		  interface_type_id = COALESCE(@interface_type_id, interface_type_id) AND
		  interface_area = COALESCE(@interface_area, interface_area)


END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_issue_reasons]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_issue_reasons]
	-- Add the parameters for the stored procedure here
	@product_id int,
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
	FROM [product_issue_reason]
	WHERE product_id = @product_id
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_print_fields]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi 
-- Create date: 20151116
-- Description:	Get product print fields
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_print_fields]
	@product_id bigint 
AS
BEGIN
	SET NOCOUNT ON;
	
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT
		product_fields.product_field_id
		, product_fields.product_id
		, product_fields.field_name
		, product_fields.print_field_type_id
		, product_fields.X
		, product_fields.Y
		, product_fields.width
		, product_fields.height
		, product_fields.font
		, product_fields.font_size
		, product_fields.mapped_name
		, product_fields.editable
		, product_fields.deleted
		, product_fields.label
		, product_fields.max_length
		, cast(null as varbinary) as value,
		print_field_types.print_field_name
	FROM
		product_fields
		 INNER JOIN
                         print_field_types ON product_fields.print_field_type_id = print_field_types.print_field_type_id
	WHERE
		product_fields.product_id = @product_id
	
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END


GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_print_fields_by_code]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi 
-- Create date: 20151116
-- Description:	Get product print fields by product code
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_print_fields_by_code]
	@product_code varchar(50),
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		product_fields.product_field_id
		, product_fields.product_id
		, product_fields.field_name
		, product_fields.print_field_type_id
		, product_fields.X
		, product_fields.Y
		, product_fields.width
		, product_fields.height
		, product_fields.font
		, product_fields.font_size
		, product_fields.mapped_name
		, product_fields.editable
		, product_fields.deleted
		, product_fields.label
		, product_fields.max_length
		, cast(null as varbinary) as value,
		print_field_types.print_field_name

	FROM
		product_fields
		 INNER JOIN print_field_types ON product_fields.print_field_type_id = print_field_types.print_field_type_id
	
		 INNER JOIN issuer_product 	ON product_fields.product_id = issuer_product.product_id
	WHERE
		issuer_product.product_code = @product_code
		AND product_fields.deleted = 0

END


GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_print_fields_value]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi 
-- Create date: 20151116
-- Description:	Get product print fields
-- =============================================
--exec usp_get_product_print_fields_value '87309'

CREATE PROCEDURE [dbo].[usp_get_product_print_fields_value]
	@card_id bigint
AS
BEGIN
	SET NOCOUNT ON;
	
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT
		product_fields.product_field_id
		, product_fields.product_id
		, product_fields.field_name
		, product_fields.print_field_type_id
		, product_fields.X
		, product_fields.Y
		, product_fields.width
		, product_fields.height
		, product_fields.font
		, product_fields.font_size
		, product_fields.mapped_name
		, product_fields.editable
		, product_fields.deleted		
		, product_fields.label
		, product_fields.max_length
		, CAST(DECRYPTBYKEY(customer_fields.value) as image) value,
		print_field_types.print_field_name
	FROM
		product_fields
		 INNER JOIN print_field_types ON product_fields.print_field_type_id = print_field_types.print_field_type_id
		inner JOIN customer_fields ON customer_fields.product_field_id = product_fields.product_field_id
		inner JOIN customer_account ON customer_account.customer_account_id = customer_fields.customer_account_id
		--LEFT JOIN cards ON cards.card_id = customer_account.card_id
	WHERE customer_account.card_id = @card_id
	UNION ALL
	SELECT
		product_fields.product_field_id
		, product_fields.product_id
		, product_fields.field_name
		, product_fields.print_field_type_id
		, product_fields.X
		, product_fields.Y
		, product_fields.width
		, product_fields.height
		, product_fields.font
		, product_fields.font_size
		, product_fields.mapped_name
		, product_fields.editable
		, product_fields.deleted		
		, product_fields.label
		, product_fields.max_length
		, customer_image_fields.value,
		print_field_types.print_field_name
	FROM
		product_fields
		 INNER JOIN print_field_types ON product_fields.print_field_type_id = print_field_types.print_field_type_id

		inner JOIN customer_image_fields ON customer_image_fields.product_field_id = product_fields.product_field_id
		inner JOIN customer_account ON customer_account.customer_account_id = customer_image_fields.customer_account_id
		--LEFT JOIN cards ON cards.card_id = customer_account.card_id
	WHERE customer_account.card_id = @card_id
	
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END


GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_service_request_code1]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_get_product_service_request_code1]
AS
	BEGIN 
    SELECT src1_id, name
	FROM product_service_requet_code1
	END





GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_service_request_code2]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_get_product_service_request_code2]
AS
	BEGIN 
    SELECT src2_id, name
	FROM product_service_requet_code2
	END





GO
/****** Object:  StoredProcedure [dbo].[usp_get_product_service_request_code3]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_get_product_service_request_code3]
AS
	BEGIN 
    SELECT src3_id, name
	FROM product_service_requet_code3
	END





GO
/****** Object:  StoredProcedure [dbo].[usp_get_productlist]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_productlist]
	@issuer_id int =null,
	@card_issue_method_id int = null,
	@Deleted_YN bit = NULL,
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
		SELECT DISTINCT issuer_product.product_code,
			   issuer_product.product_name,
			   issuer_product.product_id,
			   issuer_product.product_bin_code,			   
			   issuer_product.issuer_id, 
			   issuer_product.pan_length,
			   issuer_product.sub_product_code,
			   issuer_product.pin_calc_method_id,
			   issuer_product.auto_approve_batch_YN,
			   issuer_product.account_validation_YN,
			   issuer_product.name_on_card_top,                
			   issuer_product.name_on_card_left, 
			   issuer_product.Name_on_card_font_size, 
			   issuer_product.font_id, 
			   Issuer_product_font.font_name, 
               Issuer_product_font.resource_path,
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.PVK)) as PVK, 
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.PVKI)) as PVKI, 
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.CVKB)) as CVKB, 			   
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.CVKA)) as CVKA,
			   src1_id, src2_id, src3_id,
			   issuer_product.expiry_months,
			   issuer_product.fee_scheme_id,
			   issuer_product.enable_instant_pin_YN,
			   issuer_product.min_pin_length,
			   issuer_product.max_pin_length,
			   issuer_product.enable_instant_pin_reissue_YN,
			   issuer_product.cms_exportable_YN,
			   issuer_product.product_load_type_id,
			   issuer_product.pin_mailer_printing_YN,
			   issuer_product.pin_mailer_reprint_YN,
			   issuer_product.card_issue_method_id,
			     CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.decimalisation_table)) as decimalisation_table, 
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.pin_validation_data)) as pin_validation_data,
			   pin_block_formatid,
			   issuer_product.DeletedYN
		FROM issuer_product 
				INNER JOIN Issuer_product_font 
					ON issuer_product.font_id = Issuer_product_font.font_id	
		WHERE issuer_product.issuer_id= @issuer_id 
				AND issuer_product.DeletedYN = COALESCE(@Deleted_YN, issuer_product.DeletedYN)
				AND card_issue_method_id = COALESCE(@card_issue_method_id, card_issue_method_id)
						
		)
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY product_name ASC
END













GO
/****** Object:  StoredProcedure [dbo].[usp_get_products_by_bincode]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch products that match the BIN. 
-- Only products that are active and have an active issuer are returned
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_products_by_bincode] 
	-- Add the parameters for the stored procedure here
	@bin_code char(6),
	@issuer_id int = null,
	@only_active_records bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

    SELECT [issuer_product].product_id,
			[issuer_product].product_code,
			[issuer_product].product_name,
			[issuer_product].product_bin_code,
			ISNULL([issuer_product].sub_product_code, '') as sub_product_code,
			[issuer_product].card_issue_method_id,
			[issuer_product].product_load_type_id,
			[issuer_product].DeletedYN,
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([issuer_product].CVKA)) as CVKA, 
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([issuer_product].CVKB)) as CVKB, 
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([issuer_product].PVK)) as PVK, 
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([issuer_product].PVKI)) as PVKI,
			[issuer_product].src1_id, [issuer_product].src2_id, [issuer_product].src3_id,
			[issuer_product].issuer_id,
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([issuer_product].decimalisation_table)) as decimalisation_table,
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([issuer_product].pin_validation_data)) as pin_validation_data,
			pin_block_formatid
	FROM [issuer_product]
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [issuer_product].issuer_id					
	WHERE [issuer_product].product_bin_code = @bin_code
			AND [issuer_product].issuer_id = COALESCE(@issuer_id, [issuer_product].issuer_id)
			AND ((@only_active_records = 0) OR (@only_active_records = 1
												AND [issuer_product].DeletedYN = 0
												AND [issuer].issuer_status_id = 0))

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END










GO
/****** Object:  StoredProcedure [dbo].[usp_get_products_by_productcode]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Lebo Tladi
-- Create date: 
-- Description:	Fetch products that match the product code. 
-- Only products that are active and have an active issuer are returned
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_products_by_productcode] 
	-- Add the parameters for the stored procedure here
	@product_code varchar(100),
	@issuer_id int = null,
	@only_active_records bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

    SELECT [issuer_product].product_id,
			[issuer_product].product_code,
			[issuer_product].product_name,
			[issuer_product].product_bin_code,
			ISNULL([issuer_product].sub_product_code, '') as sub_product_code,
			[issuer_product].card_issue_method_id,
			[issuer_product].product_load_type_id,
			[issuer_product].DeletedYN,
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([issuer_product].CVKA)) as CVKA, 
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([issuer_product].CVKB)) as CVKB, 
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([issuer_product].PVK)) as PVK, 
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([issuer_product].PVKI)) as PVKI,
			[issuer_product].src1_id, [issuer_product].src2_id, [issuer_product].src3_id,
			[issuer_product].issuer_id
	FROM [issuer_product]
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [issuer_product].issuer_id					
	WHERE [issuer_product].product_code = @product_code
			AND [issuer_product].issuer_id = COALESCE(@issuer_id, [issuer_product].issuer_id)
			AND ((@only_active_records = 0) OR (@only_active_records = 1
												AND [issuer_product].DeletedYN = 0
												AND [issuer].issuer_status_id = 0))

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END










GO
/****** Object:  StoredProcedure [dbo].[usp_get_report_fields]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_report_fields]
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
/****** Object:  StoredProcedure [dbo].[usp_get_reportfields_Labels]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE PROCEDURE [dbo].[usp_get_reportfields_Labels]
	@languageId int=null,
	@ReportId int=null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT        report_fields.reportfieldid, reportfields_language.language_text
FROM            reports INNER JOIN
                         report_reportfields ON reports.Reportid = report_reportfields.reportid INNER JOIN
                         report_fields ON report_reportfields.reportfieldid = report_fields.reportfieldid INNER JOIN
                         reportfields_language ON report_reportfields.reportfieldid = reportfields_language.reportfieldid

						 where reportfields_language.[language_id]=@languageid and reports.[Reportid]=@reportid

--select Reportid,col1,col2,col3,col4,col5,col6,col7,col8,col9,col10,col11,col12,col13,col14,col15,col16,col17,col18,col19,col20,col21
--from
--(
--SELECT       reports.Reportid, reportfields_language.language_text ,
--'col'+CAST( Row_Number() over (partition by  reports.Reportid order by  report_reportfields.[reportfieldorderno]) as varchar(10)) as columnsequence
--FROM            reports INNER JOIN
--                         report_reportfields ON reports.Reportid = report_reportfields.reportid INNER JOIN
--                         report_fields ON report_reportfields.reportfieldid = report_fields.reportfieldid INNER JOIN
--                         reportfields_language ON report_reportfields.reportfieldid = reportfields_language.reportfieldid

--						 where reportfields_language.[language_id]=@languageId and  reports.Reportid=@ReportId
--) 	 Temp
				
				
--PIVOT
--(
--		max(language_text) 
--		for columnsequence in (col1,col2,col3,col4,col5,col6,col7,col8,col9,col10,col11,col12,col13,col14,col15,col16,col17,col18,col19,col20,col21)

--)PIV
END


GO
/****** Object:  StoredProcedure [dbo].[usp_get_sequencenumber]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_sequencenumber]
	@product_id bigint,
	@sub_product_id int = NULL,
	@auditUserId int ,
	@auditWorkStation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF (@sub_product_id IS NULL)
		SET @sub_product_id = -1

	BEGIN TRANSACTION [GET_CARD_SEQ_TRAN]
		BEGIN TRY 
		
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			--check if the product has a sequence number, if it doesnt add the new sequence
			IF ((SELECT COUNT(*) FROM integration_cardnumbers WHERE product_id = @product_id AND
					sub_product_id = @sub_product_id) = 0)
				BEGIN
					INSERT INTO integration_cardnumbers (product_id, sub_product_id, card_sequence_number)
					VALUES (@product_id, @sub_product_id, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,0)))
				END			

			--Grab the latest sequence number for the product
			SELECT TOP 1 CONVERT(VARCHAR(max),DECRYPTBYKEY(card_sequence_number))
			FROM	integration_cardnumbers
			WHERE	product_id = @product_id
				AND	sub_product_id = @sub_product_id

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
				
			COMMIT TRANSACTION [GET_CARD_SEQ_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_CARD_SEQ_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_get_spoilcardsreport]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: 16/05/2014
-- Description:	for displaying issued card report
-- =============================================
----exec usp_get_issuedcardsreport 2,'6/14/2014 12:00:00 AM','6/27/2014 12:00:00 AM' ,25,null
CREATE PROCEDURE [dbo].[usp_get_spoilcardsreport]
	@isuerid int = null,
	@language_id int,
	@userid int = null,
	@branchid int = null,
	@fromdate datetime,
	@todate datetime,
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100)

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

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)

    -- Insert statements for procedure here
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
		
		SELECT 
			DISTINCT [branch_code] as 'BranchCode'
			, CONVERT(VARCHAR,DECRYPTBYKEY(u.[username])) as 'SpoliedBy'
			,'' as 'IssuedBy'
			, CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.customer_first_name))+' '+ CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.customer_last_name)) as 'CustomerNames'
			, CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.[customer_account_number])) as 'customeraccountNumber'
			,CASE 
				WHEN @mask_report = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
				ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
			END AS 'cardnumber'
			, cards.card_request_reference AS cardreferencnumber
			, [branch_card_statuses_name] as 'CardStatus'
			, [status_date] as'IssuedDate'
			, bcl.[language_text] as 'Reason'
		FROM branch_card_status_current
				INNER JOIN cards  
					ON branch_card_status_current.[card_id] = cards.[card_id]
				INNER JOIN branch 
					ON cards.[branch_id] = branch.[branch_id]
				INNER JOIN issuer 
					ON issuer.issuer_id = branch.issuer_id
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
/****** Object:  StoredProcedure [dbo].[usp_get_Spoilcardsummaryreport]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE PROCEDURE [dbo].[usp_get_Spoilcardsummaryreport]
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
/****** Object:  StoredProcedure [dbo].[usp_get_status_flow_roles]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_status_flow_roles] 
	@role_list AS dbo.key_value_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [dist_batch_statuses_flow].*
	FROM [dist_batch_statuses_flow]
		INNER JOIN @role_list roles
			ON  [dist_batch_statuses_flow].user_role_id = roles.[key]
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_terminal]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi
-- Create date: 20150210
-- Description:	Search for terminal by branch or masterkey
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_terminal]
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
/****** Object:  StoredProcedure [dbo].[usp_get_terminal_parameters]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_get_terminal_parameters] 
	-- Add the parameters for the stored procedure here
	@product_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT product_bin_code, product_name, product_code, product_id, min_pin_length, max_pin_length, 
			[issuer_product].enable_instant_pin_YN as product_enable_instant_pin_YN,
			[issuer_product].enable_instant_pin_reissue_YN, DeletedYN, 
			[issuer].issuer_status_id , 
			[issuer].enable_instant_pin_YN as issuer_enable_instant_pin_YN
	FROM [issuer_product]
		INNER JOIN [issuer]
			ON [issuer_product].issuer_id = [issuer].issuer_id
	WHERE product_id = @product_id
END





GO
/****** Object:  StoredProcedure [dbo].[usp_get_terminals_list]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi	
-- Create date: 20150210
-- Description: Gets a list of all the terminals
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_terminals_list]
	@languageId INT = null
	,@issuer_id INT,
	@branch_id int,
	@PageIndex INT = 1
	,@RowsPerPage INT = 20
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	WITH PAGE_ROWS
	AS
	( 
		SELECT ROW_NUMBER() OVER(ORDER BY [terminal_id] ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
		FROM(
			
				SELECT  
					DISTINCT [terminals].[terminal_id]
					, [terminals].[terminal_name]
					, [terminals].[terminal_model]
					, [issuer].[issuer_name],branch.branch_name,CONVERT(varchar,DECRYPTBYKEY(device_id)) as deviceid
				FROM
					[terminals]
					INNER JOIN [branch] ON [branch].[branch_id] = [terminals].[branch_id]
					INNER JOIN [issuer] ON [issuer].[issuer_id] =  [branch].[issuer_id]
				WHERE 
					issuer.issuer_id=COALESCE(@issuer_id,issuer.issuer_id) AND [terminals].branch_id = COALESCE(@branch_id, [terminals].branch_id))

		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY [terminal_Id] ASC
		CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;


END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_tmk_by_issuer]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi	
-- Create date: 20150213
-- Description:	Get Masterkeys for issuer
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_tmk_by_issuer]
	@issuer_id INT
	, @branch_id INT=null,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20
AS
BEGIN

	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY [masterkey_name] DESC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
		SELECT 
			CONVERT(VARCHAR(max),DECRYPTBYKEY([masterkeys].masterkey)) as 'masterkey'
			, [masterkeys].[masterkey_name]
			, [masterkeys].issuer_id
			, [issuer].issuer_name
			, [masterkeys].masterkey_id
		FROM [masterkeys]
				INNER JOIN [issuer]
					ON [issuer].issuer_id = [masterkeys].issuer_id
						AND [issuer].issuer_status_id = 0
		WHERE [masterkeys].issuer_id = @issuer_id
			)
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY [masterkey_name] DESC
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_tmk_by_terminal]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_tmk_by_terminal] 
	-- Add the parameters for the stored procedure here
	@device_id varchar(250), 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT CONVERT(VARCHAR(max),DECRYPTBYKEY([masterkeys].masterkey)) as 'masterkey'
				,[user_branch_access].issuer_id
				,[masterkeys].[masterkey_name]
				,[user_branch_access].issuer_name
				,[masterkeys].masterkey_id
		FROM [masterkeys]
				INNER JOIN [terminals]
					ON [masterkeys].masterkey_id = [terminals].terminal_masterkey_id
				INNER JOIN 
					(SELECT DISTINCT [user_roles_branch].branch_id, [issuer].issuer_id, [issuer].issuer_name						
						FROM [user_roles_branch] 
							INNER JOIN [user_roles]
								ON [user_roles_branch].user_role_id = [user_roles].user_role_id											
							INNER JOIN [branch]
								ON [user_roles_branch].branch_id = [branch].branch_id	
									AND [branch].branch_status_id = 0
							INNER JOIN [issuer]
								ON [branch].issuer_id = [issuer].issuer_id
									AND [issuer].issuer_status_id = 0
						WHERE [user_roles_branch].user_role_id = 7
							AND [user_roles_branch].[user_id] = @audit_user_id	
						) AS [user_branch_access]
					ON [terminals].branch_id = [user_branch_access].branch_id
		WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([terminals].device_id)) = @device_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_unassigned_users]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get all users not assigned to any groups.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_unassigned_users] 
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
							,[user].[workstation],case when ([user].[connection_parameter_id] is null) then 'Indigo' else 'LDAP' end as 'AUTHENTICATION_TYPE'
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
		--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_get_user]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_get_user]
	(
		@username varchar(256),
		@user_status varchar(30)
	)
AS
	/* SET NOCOUNT ON */

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

    SELECT user_id, u.user_status_id, user_gender_id, username, first_name,
		   last_name, CONVERT(VARCHAR(max),DECRYPTBYKEY(password)), online, employee_id, last_login_date,
		   last_login_attempt, number_of_incorrect_logins, last_password_changed_date,
		   workstation 
	FROM [user] u,user_status us
	WHERE (DECRYPTBYKEY(username)=@username And us.user_status_text = @user_status)
	
CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;










GO
/****** Object:  StoredProcedure [dbo].[usp_get_user_by_user_id]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 24 March 2014
-- Description:	Get a user based on the users system ID.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_user_by_user_id] 
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
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) as 'username'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[first_name])) as 'first_name'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[last_name])) as 'last_name' 					
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[employee_id])) as 'empoyee_id'
						,[user].[user_email]
						,[user].[user_status_id] 
						,[user].[connection_parameter_id]
						,[user].[language_id]
						,[user].[online]    
						,[user].[workstation],(case when last_password_changed_date is null then getdate() else last_password_changed_date end ) as 'last_password_changed_date'
						,[user].number_of_incorrect_logins,[user].last_login_attempt,[user].external_interface_id
		FROM [user]
		WHERE [user].[user_id] = @user_id		

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_user_by_username]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 14 March 2014
-- Description:	Return a user based on the username
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_user_by_username] 
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
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) as 'username'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[first_name])) as 'first_name'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[last_name])) as 'last_name' 					
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[employee_id])) as 'empoyee_id'
						,[user].[user_email]
						,[user].[connection_parameter_id]
						,[user].[language_id]
						,[user].[user_status_id] 
						,[user].[online]    
						,[user].[workstation],(case when last_password_changed_date is null then getdate() else last_password_changed_date end ) as 'last_password_changed_date'
						,[user].number_of_incorrect_logins,[user].last_login_attempt,[user].external_interface_id

		FROM [user]
		WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) = @username		

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_user_for_login]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 20 March 2014
-- Description:	Gets a user for login purposes.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_user_for_login] 
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
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) as 'clr_username'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[password])) as 'clr_password'
						,[user].[online]    
						,[user].[workstation]
						,[user].last_login_attempt
						,[user].last_password_changed_date
						,[user].number_of_incorrect_logins
						,[user].language_id
						,[user].[connection_parameter_id]	
						,[user].user_status_id
						,[connection_parameters].address as domain_hostname_or_ip
						,[connection_parameters].domain_name
						,[connection_parameters].[path] as domain_path
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([connection_parameters].username)) as domain_username	
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([connection_parameters].[password])) as domain_password,
						[connection_parameters].connection_parameter_type_id, [connection_parameters].port,
						[connection_parameters].is_external_auth,[user].external_interface_id,auth_type,protocol,[path]								
		FROM [user] 
			LEFT OUTER JOIN [connection_parameters]
				ON [user].[connection_parameter_id] = [connection_parameters].[connection_parameter_id]
		WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) = @username

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_user_groups_admin]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Gets a list of user groups and indicates if the user has been assigned to it.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_user_groups_admin] 
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
/****** Object:  StoredProcedure [dbo].[usp_get_user_groups_by_issuer]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 13 March 2014
-- Description:	Get user groups by issuer.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_user_groups_by_issuer] 
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
		--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_get_user_roles_for_user]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 11 March 2014
-- Description:	Get all the roles linked to the specified user.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_user_roles_for_user]
	@user_id bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [user_group].user_role_id, [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
			[issuer].maker_checker_YN, 
			[issuer].classic_card_issue_YN, [issuer].instant_card_issue_YN, [issuer].card_ref_preference,
			[issuer].enable_instant_pin_YN, [issuer].authorise_pin_issue_YN, [issuer].authorise_pin_reissue_YN,			
			[issuer].back_office_pin_auth_YN,
			[user_roles].allow_multiple_login, 
			[user_group].can_read, [user_group].can_create, [user_group].can_update,
			CAST(CASE WHEN 1 = ANY (SELECT [issuer_product].account_validation_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id)
				THEN 1 ELSE 0 END as BIT) AS account_validation_YN,
			CAST(CASE WHEN 1 = ANY (SELECT [issuer_product].auto_approve_batch_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id)
				THEN 1 ELSE 0 END as BIT) AS auto_create_dist_batch		
			,CAST(CASE WHEN 1 = ANY (SELECT [issuer_product].pin_mailer_printing_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id)
				THEN 1 ELSE 0 END as BIT) AS pin_mailer_printing_YN
			,CAST(CASE WHEN 1 = ANY (SELECT [issuer_product].pin_mailer_reprint_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id)
				THEN 1 ELSE 0 END as BIT) AS pin_mailer_reprint_YN
			,CAST(CASE WHEN 0 <> ANY (SELECT [issuer_product].product_load_type_id FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id)
				THEN 1 ELSE 0 END as BIT) AS EnableCardFileLoader_YN

	FROM [user_group] 
			INNER JOIN [users_to_users_groups] 
				ON [user_group].user_group_id = [users_to_users_groups].user_group_id
			INNER JOIN [user_roles]
				ON [user_group].user_role_id = [user_roles].user_role_id
			INNER JOIN [user] 
				ON [user].[user_id] = [users_to_users_groups].[user_id]			 
			INNER JOIN [issuer] 
				ON [user_group].issuer_id = [issuer].issuer_id
			--LEFT OUTER JOIN [issuer_product]
			--	ON 	[issuer_product].issuer_id = [issuer].issuer_id		
	WHERE [user].[user_id] = @user_id
		  AND [issuer].issuer_status_id = 0
		  AND [user].user_status_id = 0
	GROUP BY [user_group].user_role_id, [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
			[issuer].maker_checker_YN, --account_validation_YN, auto_create_dist_batch,
			[issuer].classic_card_issue_YN, [issuer].instant_card_issue_YN, [issuer].card_ref_preference,
			[issuer].enable_instant_pin_YN, [issuer].authorise_pin_issue_YN, [issuer].authorise_pin_reissue_YN,
			--[issuer].pin_mailer_printing_YN, [issuer].pin_mailer_reprint_YN, 
			--[issuer].EnableCardFileLoader_YN,
			[issuer].back_office_pin_auth_YN, [user_roles].allow_multiple_login, 
			[user_group].can_read, [user_group].can_create, [user_group].can_update
END











GO
/****** Object:  StoredProcedure [dbo].[usp_get_useradminsettingslist]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_useradminsettingslist]

	@audit_user_id bigint,
	@audit_workstation nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select user_admin_id, PasswordValidPeriod,PasswordMinLength,PasswordMaxLength,PreviousPasswordsCount,maxInvalidPasswordAttempts,PasswordAttemptLockoutDuration
	from user_admin 
	--where user_admin_id=@user_admin_id
						
END


GO
/****** Object:  StoredProcedure [dbo].[usp_get_userbyroles_auditreport]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec usp_get_userbyroles_auditreport -1 ,null,null,null
CREATE PROCEDURE [dbo].[usp_get_userbyroles_auditreport]
@issuer_id int=null,
@user_group_id int =null,
@user_role_id int =null,
@user_id int=null,
@audit_user_id bigint,
@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
	if(@issuer_id=-1 or @issuer_id=0)
	set @issuer_id=null

	SELECT        ug.user_group_name, user_roles.user_role, issuer.issuer_code+'-'+ issuer.issuer_name as 'issuer_name', ug.mask_screen_pan, ug.mask_report_pan, ug.can_read, ug.can_update, ug.can_delete, 
                         ug.all_branch_access, CONVERT(VARCHAR(max),DECRYPTBYKEY([user].username)) as 'username','' as branch_name, ug.can_create
FROM            user_group AS ug INNER JOIN
                         issuer ON ug.issuer_id = issuer.issuer_id INNER JOIN
                         user_roles ON ug.user_role_id = user_roles.user_role_id INNER JOIN
                         users_to_users_groups ON ug.user_group_id = users_to_users_groups.user_group_id INNER JOIN
                         [user] ON users_to_users_groups.user_id = [user].user_id

	 WHERE ug.issuer_id = COALESCE(@issuer_id, ug.issuer_id)
			and ug.user_group_id =COALESCE(@user_group_id, ug.user_group_id)
			and ug.user_role_id=COALESCE(@user_role_id, ug.user_role_id)
			--and [user].user_id=COALESCE(@user_id, [user].user_id)
					
	ORDER BY 	issuer.issuer_name,ug.user_group_name,user_roles.user_role,username

  CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_usergroup]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_get_usergroup]
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
/****** Object:  StoredProcedure [dbo].[usp_get_usergroup_auditreport]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec usp_get_usergroup_auditreport -1,null,null
CREATE PROCEDURE [dbo].[usp_get_usergroup_auditreport]
@issuer_id int=null,
@user_group_id int =null,
@user_role_id int =null,
@audit_user_id bigint,
@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if(@issuer_id=-1 or @issuer_id=0)
	set @issuer_id=null

		SELECT        ug.user_group_name, user_roles.user_role, issuer.issuer_code+'-'+ issuer.issuer_name as issuer_name, ug.mask_screen_pan, ug.mask_report_pan, ug.can_read, 
                         ug.can_update, ug.can_delete, ug.all_branch_access, '' as branch_name, '' as username, ug.can_create
		FROM            user_group ug INNER JOIN
                         issuer ON ug.issuer_id = issuer.issuer_id INNER JOIN
                         user_roles ON ug.user_role_id = user_roles.user_role_id

	 WHERE ug.issuer_id = COALESCE(@issuer_id, ug.issuer_id)
			and ug.user_group_id =COALESCE(@user_group_id, ug.user_group_id)
			and ug.user_role_id=COALESCE(@user_role_id, ug.user_role_id)
					
	ORDER BY 	issuer.issuer_name,ug.user_group_name,user_roles.user_role

  
END




GO
/****** Object:  StoredProcedure [dbo].[usp_get_userroles_enterprise]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_userroles_enterprise]
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
/****** Object:  StoredProcedure [dbo].[usp_get_users_by_branch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 13 March 2014
-- Description:	Get all ussers linked to the specified branch.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_users_by_branch] 
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
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) as 'username'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[first_name])) as 'first_name'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[last_name])) as 'last_name' 					
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[employee_id])) as 'empoyee_id'
						,usl.language_text as [user_status_text] 
						,[user].[online]    
						,[user].[workstation],case when ([user].connection_parameter_id is null) then 'Indigo' else 'LDAP' end as 'AUTHENTICATION_TYPE'
			FROM [branch] INNER JOIN [user_roles_branch]
					ON [branch].branch_id = [user_roles_branch].branch_id
				INNER JOIN [user]
					ON [user].[user_id] = [user_roles_branch].[user_id]
				INNER JOIN user_status
					ON user_status.user_status_id = [user].user_status_id
				INNER JOIN  [dbo].[user_status_language] usl 
					ON usl.user_status_id=user_status.user_status_id 
			WHERE [user_roles_branch].issuer_id = COALESCE(@issuer_id, [user_roles_branch].issuer_id)
				AND (@username IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) LIKE @username) 
				AND (@first_name IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[first_name])) LIKE @first_name) 
				AND (@last_name IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[last_name])) LIKE @last_name) 
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
/****** Object:  StoredProcedure [dbo].[usp_get_users_by_branch_admin]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 13 March 2014
-- Description:	Get all ussers linked to the specified branch for user admin
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_users_by_branch_admin] 
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
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) as 'username'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[first_name])) as 'first_name'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[last_name])) as 'last_name' 					
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[employee_id])) as 'empoyee_id'
						,usl.language_text as  [user_status_text] 
						,[user].[online]    
						,[user].[workstation],case when ([user].[connection_parameter_id] is null) then 'Indigo' else 'LDAP' end as 'AUTHENTICATION_TYPE'
			FROM [branch] INNER JOIN [user_group_branch_ex_ent]
					ON [branch].branch_id = [user_group_branch_ex_ent].branch_id
				INNER JOIN [user]
					ON [user].[user_id] = [user_group_branch_ex_ent].[user_id]
				INNER JOIN user_status
					ON user_status.user_status_id = [user].user_status_id
				INNER JOIN  [dbo].[user_status_language] usl 
					ON usl.user_status_id=user_status.user_status_id 
			WHERE [user_group_branch_ex_ent].issuer_id = COALESCE(@issuer_id, [user_group_branch_ex_ent].issuer_id)
				AND (@username IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) LIKE @username) 
				AND (@first_name IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[first_name])) LIKE @first_name) 
				AND (@last_name IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[last_name])) LIKE @last_name) 
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
/****** Object:  StoredProcedure [dbo].[usp_get_zone_key]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_zone_key] 
	-- Add the parameters for the stored procedure here
	@issuer_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY key_injection_keys
	DECRYPTION BY CERTIFICATE cert_ZoneMasterKeys;

    SELECT 	CONVERT(VARCHAR(MAX),DECRYPTBYKEY([zone_keys].zone)) as zone, 
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([zone_keys].final)) as final
	FROM [zone_keys]
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [zone_keys].issuer_id
					AND [issuer].issuer_status_id = 0
	WHERE [zone_keys].issuer_id = @issuer_id

	CLOSE SYMMETRIC KEY key_injection_keys
END




GO
/****** Object:  StoredProcedure [dbo].[usp_getFont_by_fontid]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getFont_by_fontid]
@fontid int 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

		Select font_id,font_name,resource_path,DeletedYN,0 as TOTAL_ROWS, ROW_NUMBER() OVER(ORDER BY font_name ASC) AS ROW_NO,0 as TOTAL_PAGES from Issuer_product_font where font_id=@fontid

END












GO
/****** Object:  StoredProcedure [dbo].[usp_getFontsList]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getFontsList]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select  font_id,font_name,'' as resource_path,[DeletedYN] from Issuer_product_font where [DeletedYN]=0
END













GO
/****** Object:  StoredProcedure [dbo].[usp_getProductbyProductid]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getProductbyProductid]
	@productid int 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

    -- Insert statements for procedure here
	SELECT issuer_product.product_code,
			   issuer_product.product_name,
			   issuer_product.product_id,
			   issuer_product.product_bin_code,
			   issuer_product.issuer_id, 
			   issuer_product.sub_product_code,
			   issuer_product.pan_length,
			   issuer_product.pin_calc_method_id,
			   issuer_product.auto_approve_batch_YN,
			   issuer_product.account_validation_YN,
			   issuer_product.name_on_card_top,                
			   issuer_product.name_on_card_left, 
			   issuer_product.Name_on_card_font_size, 
			   issuer_product.font_id, 
			   Issuer_product_font.font_name, 
               Issuer_product_font.resource_path, 
			   issuer_product.enable_instant_pin_YN,
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.PVK)) as PVK, 
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.PVKI)) as PVKI, 
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.CVKB)) as CVKB, 			   
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.CVKA)) as CVKA,
			   src1_id, src2_id, src3_id,
			   issuer_product.expiry_months,
			   issuer_product.fee_scheme_id,
			   issuer_product.min_pin_length,
			   issuer_product.max_pin_length,
			   issuer_product.enable_instant_pin_reissue_YN,
			   issuer_product.cms_exportable_YN,
			   issuer_product.product_load_type_id,
			   issuer_product.pin_mailer_printing_YN,
			   issuer_product.pin_mailer_reprint_YN,
			   issuer_product.card_issue_method_id,
				0 as TOTAL_ROWS, 
				CONVERT(bigint, 0) AS ROW_NO,
				0 as TOTAL_PAGES,
				     CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.decimalisation_table)) as decimalisation_table, 
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(issuer_product.pin_validation_data)) as pin_validation_data,
			   pin_block_formatid,
			   issuer_product.DeletedYN
	FROM issuer_product 
			INNER JOIN Issuer_product_font 
				ON issuer_product.font_id = Issuer_product_font.font_id			
	WHERE issuer_product.product_id  = @productid

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

END











GO
/****** Object:  StoredProcedure [dbo].[usp_getproductcode]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getproductcode]
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
/****** Object:  StoredProcedure [dbo].[usp_insert_audit]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_insert_audit]
	@audit_user_id bigint,
	@audit_action_id int,
	@audit_date datetime = NULL,
	@workstation_address varchar(100),
	@action_description varchar(max),
	@issuer_id int = NULL,
	@data_changed varchar(max) = NULL,
	@data_before varchar(max) = NULL,
	@data_after varchar(max) = NULL

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
/****** Object:  StoredProcedure [dbo].[usp_insert_branch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Persist new branch to db
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_branch] 
	@branch_status_id int,
	@issuer_id int,
	@branch_code varchar(10),
	@branch_name varchar(30),
	@card_centre_branch_YN bit,
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
						,[contact_person],[contact_email],[card_centre], [card_centre_branch_YN])
					VALUES
						(@branch_status_id, @issuer_id, @branch_code, @branch_name, @location,
						@contact_person, @contact_email, @card_centre, @card_centre_branch_YN)

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
						user_group_name, user_role_id, mask_report_pan, mask_screen_pan)
				VALUES 
					(0, 1, 1, 1, 1, @issuer_id, @group_name, 2, 1, 1)

				SET @new_group_id = SCOPE_IDENTITY();

				INSERT INTO [user_groups_branches]
					(branch_id, user_group_id)
				VALUES 
					(@new_branch_id, @new_group_id)

				SET @group_name =  @issuer_code + '_' + @branch_code + '_OPERATOR'
				INSERT INTO [user_group]
					(all_branch_access, can_create, can_delete, can_read, can_update, issuer_id,
						user_group_name, user_role_id, mask_report_pan, mask_screen_pan)
				VALUES 
					(0, 1, 1, 1, 1, @issuer_id, @group_name, 3, 1, 1)

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

				EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_insert_file_history]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 6 March 2014
-- Description:	Insert to file_history table.
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_file_history] 	
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

	IF(@file_id IS NULL)
		BEGIN
			INSERT INTO [dbo].[file_history]
				   ([issuer_id],[name_of_file],[file_created_date],[file_size],[load_date],[file_status_id],[file_directory],
					[number_successful_records],[number_failed_records],[file_load_comments],[file_type_id], [file_load_id])		 
			 VALUES
					(@issuer_id, @name_of_file, @file_created_date, @file_size, @load_date, @file_status_id, @file_directory,
					 @number_successful_records, @number_failed_records, @file_load_comments, @file_type_id, @file_load_id)		

			SET @file_id = SCOPE_IDENTITY();
		END
	ELSE
		BEGIN
			UPDATE [dbo].[file_history]
			SET [file_status_id] = @file_status_id,
				[number_successful_records] = @number_successful_records,
				[number_failed_records] = @number_failed_records,
				[file_load_comments] = @file_load_comments
			WHERE [file_id] = @file_id
		END
END











GO
/****** Object:  StoredProcedure [dbo].[usp_insert_FlexcubeAudit]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_FlexcubeAudit] 
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100),
	@audit_description varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	EXEC usp_insert_audit @audit_user_id, 
						 3,
						 NULL, 
						 @audit_workstation, 
						 @audit_description, 
						 NULL, NULL, NULL, NULL
END











GO
/****** Object:  StoredProcedure [dbo].[usp_insert_font]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_insert_font]
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
																	
				--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_insert_load_batch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 6 March 2014
-- Description:	Insert into load_batch table and returns the id of the inserted row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_load_batch] 
	@load_batch_reference varchar(50),
	@file_id bigint,
	@issuer_id int,
	@load_batch_status_id int,
	@load_date datetime,
	@number_of_cards int,
	@load_batch_type_id int,
	@load_batch_id bigint = NULL OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [dbo].[load_batch]
			([load_batch_reference],[file_id],[load_batch_status_id],[load_date],[no_cards], [load_batch_type_id])
	VALUES
			(@load_batch_reference, @file_id, @load_batch_status_id, @load_date, @number_of_cards, @load_batch_type_id)

	SET @load_batch_id = SCOPE_IDENTITY();
END











GO
/****** Object:  StoredProcedure [dbo].[usp_insert_load_batch_status]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 6 March 2014
-- Description:	Insert into load_batch_status
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_load_batch_status] 	
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
/****** Object:  StoredProcedure [dbo].[usp_insert_pin_reissue]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		usp_insert_pin_reissue
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_pin_reissue] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@branch_id int,
	@product_id int,
	@pan varchar(19), 
	@authorise_user_id bigint = null,
	@failed bit,
	@note varchar(500),
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate

	 BEGIN TRANSACTION [PIN_REISSUE]
	 BEGIN TRY 

		-- Insert statements for procedure here
		INSERT INTO [dbo].[pin_reissue] ([issuer_id], [branch_id], [product_id], [pan], [reissue_date],
											[operator_user_id], [authorise_user_id], [failed], [notes])
		 VALUES (@issuer_id, @branch_id, @product_id,
			   ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@pan)),
			   GETDATE(), @audit_user_id, @authorise_user_id, @failed, @note)


		DECLARE @audit_msg varchar(max)

		SET @audit_msg = 'PIN REISSUED, ' + dbo.MaskString(@pan, 6, 4)
						--log the audit record		
						EXEC usp_insert_audit @audit_user_id, 
											 3,
											 NULL, 
											 @audit_workstation, 
											 @audit_msg, 
											 NULL, NULL, NULL, NULL

		COMMIT TRANSACTION [PIN_REISSUE]

		CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [PIN_REISSUE]
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
/****** Object:  StoredProcedure [dbo].[usp_insert_product]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [usp_insert_product] 'Ghana_Visa_Black_Card','GVBC02','484680',-1,50.00,50.00,15,2,null,-1,'veneka-04'
CREATE PROCEDURE [dbo].[usp_insert_product]
	-- Add the parameters for the stored procedure here
	@product_name varchar(100),
	@product_code varchar(50),
	@product_bin_code varchar(9),
	@issuer_id  int,
	@pan_length smallint,
	@sub_product_code varchar(4),
	@expiry_months int,
	@fee_scheme_id int = null,
	@card_issue_method_id int,

	@name_on_card_top decimal(8,2)=0,
	@name_on_card_left decimal(8,2)=0,
	@Name_on_card_font_size int=0,
	@font_id int=1,	

	@src1_id int,
	@src2_id int,
	@src3_id int,
	@PVKI varchar(100),
	@PVK varchar(100),
	@CVKA varchar(100),
	@CVKB varchar(100),	
		
	@enable_instant_pin_YN bit,
	@enable_instant_pin_reissue_YN bit,
	@pin_calc_method_id int,
	@min_pin_length int,
	@max_pin_length int,	

	@cms_exportable_YN bit,
	@product_load_type_id int,
	@auto_approve_batch_YN bit,
	@account_validation_YN bit,
	@pin_mailer_printing_YN bit,
	@pin_mailer_reprint_YN bit,

	@card_issue_reasons_list as dbo.key_value_array READONLY,
	@account_types_list as dbo.key_value_array READONLY,
	@currencylist AS dbo.product_currency_array READONLY,	
	@external_system_fields AS  dbo.[product_external_fields_array] READONLY,	
	@prod_interface_parameters_list AS dbo.bikey_value_array READONLY,
	@issue_interface_parameters_list AS dbo.bikey_value_array READONLY,

	@decimalisation_table varchar(100),
	@pin_validation_data varchar(100),
	@pin_block_formatid int,

	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int =null OUTPUT,
	@new_product_id int =null OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		BEGIN TRANSACTION [INSERT_Product_TRAN]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate
			--Check for duplicate's
			IF EXISTS(SELECT * FROM [issuer_product] WHERE [product_code] = @product_code AND issuer_id = @issuer_id)
				BEGIN
					SET @new_product_id = 0
					SET @ResultCode = 221						
				END
			 ELSE
			IF EXISTS(SELECT * FROM [issuer_product] WHERE [product_name] = @product_name)
				BEGIN
					SET @new_product_id = 0
					SET @ResultCode = 220
				END
			ELSE IF dbo.ProductValidation(NULL, @product_bin_code, @sub_product_code) = 0
				BEGIN
					SET @new_product_id = 0
					SET @ResultCode = 222
				END
			ELSE IF 0 = ANY(SELECT dbo.FileParameterValidation(key2, value, key1) FROM @prod_interface_parameters_list)
				BEGIN
					SET @new_product_id = 0
					SET @ResultCode = 228
				END
			ELSE			
			BEGIN
				INSERT INTO [dbo].[issuer_product]
					   ([product_code], [product_name], [product_bin_code], [issuer_id],
						[name_on_card_top], [name_on_card_left], [Name_on_card_font_size], [font_id],DeletedYN,
						[src1_id],[src2_id],[src3_id],[PVKI],[PVK],[CVKA],CVKB,[expiry_months], 
						[fee_scheme_id], [enable_instant_pin_YN],[min_pin_length],[max_pin_length],
						[enable_instant_pin_reissue_YN],[cms_exportable_YN],[product_load_type_id],
						[pan_length],[sub_product_code],[pin_calc_method_id],
						[auto_approve_batch_YN],[account_validation_YN], [pin_mailer_printing_YN],
						[pin_mailer_reprint_YN], [card_issue_method_id],[decimalisation_table],[pin_validation_data],pin_block_formatid)
				VALUES (@product_code, @product_name, @product_bin_code, @issuer_id,
						@name_on_card_top, @name_on_card_left, @Name_on_card_font_size, @font_id, 0,
						@src1_id, @src2_id, @src3_id, 
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@PVKI)), 
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@PVK)), 
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@CVKA)),
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@CVKB)), @expiry_months,
						@fee_scheme_id, @enable_instant_pin_YN,
						@min_pin_length, @max_pin_length, @enable_instant_pin_reissue_YN, @cms_exportable_YN,
						@product_load_type_id,@pan_length,@sub_product_code,@pin_calc_method_id,
						@auto_approve_batch_YN,@account_validation_YN, @pin_mailer_printing_YN,@pin_mailer_reprint_YN,
						@card_issue_method_id,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@decimalisation_table)), 
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@pin_validation_data)),@pin_block_formatid )
			
				SET @new_product_id= SCOPE_IDENTITY()

				--insert issuing reason
				INSERT INTO [product_issue_reason] (product_id, card_issue_reason_id)
				SELECT @new_product_id, value
				FROM @card_issue_reasons_list	

				--insert account types
				INSERT INTO [products_account_types] (product_id, account_type_id)
				SELECT @new_product_id, value
				FROM @account_types_list

				--insert production interfaces
				INSERT INTO [product_interface] (product_id, interface_type_id, connection_parameter_id, interface_guid, interface_area)
				SELECT @new_product_id, key1, key2, value, 0
				FROM @prod_interface_parameters_list

				--insert issuing interfaces
				INSERT INTO [product_interface] (product_id, interface_type_id, connection_parameter_id, interface_guid, interface_area)
				SELECT @new_product_id, key1, key2, value, 1
				FROM @issue_interface_parameters_list

				--insert currencies
				DECLARE @RC int
				EXECUTE @RC = [usp_insert_product_currency] @new_product_id, @currencylist, @audit_user_id, @audit_workstation

				--insert external system fields

				DECLARE @RCExternal int
				EXECUTE @RCExternal = [usp_insert_product_external_systems] @new_product_id, @external_system_fields, @audit_user_id, @audit_workstation

				DECLARE @audit_description varchar(500)
				SELECT @audit_description = 'Product Created: ' + @product_code  + ', Product Name:' + @product_name + 
																     ', bin code:' + @product_bin_code 
															+ ' , Product Id: ' + CAST(@new_product_id as varchar(max))
																	
				EXEC usp_insert_audit @audit_user_id, 
									 4,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @issuer_id, NULL, NULL, NULL

				SET @ResultCode = 0				
			END
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
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
/****** Object:  StoredProcedure [dbo].[usp_insert_product_currency]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya
-- Create date: 
-- Description:	Insert link between users group and branches
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_product_currency] 
	-- Add the parameters for the stored procedure here
	@product_id int,
	@currencylist as dbo.product_currency_array READONLY,
	
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
						(product_id, currency_id, is_base, usr_field_name_1, usr_field_val_1, usr_field_name_2, usr_field_val_2)
			SELECT @product_id, cl.currency_id, is_base, usr_field_name_1, usr_field_val_1, usr_field_name_2, usr_field_val_2
			FROM @currencylist cl

			--Insert audit train
			--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_insert_product_external_systems]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya
-- Create date: <Create Date,,>
-- Description:Inserting external fields to product
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_product_external_systems] 
	-- Add the parameters for the stored procedure here
	@product_id int,
	@external_system_fields as dbo.[product_external_fields_array] READONLY,	
	@audit_user_id BIGINT,
	@audit_workstation varchar(100)
AS
BEGIN
	
	SET NOCOUNT ON;

	

			INSERT INTO product_external_system
						(product_id, external_system_field_id, field_value)
			SELECT @product_id, esx.external_system_field_id, field_value
			FROM @external_system_fields esx

			

END








GO
/****** Object:  StoredProcedure [dbo].[usp_insert_user]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 20 March 2014
-- Description:	Insert a user
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_user] 
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
	@connection_parameter_id int = null,
	@external_interface_id char(36)=null,
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
			WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) = @username

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
						  ,[last_password_changed_date],[workstation], [connection_parameter_id], [language_id], [username_index],external_interface_id)
					VALUES
						   (@user_status_id,
							1,
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@username)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@first_name)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@last_name)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@password)),
							@user_email,
							@online,
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@employee_id)),
							@last_login_date,
							@last_login_attempt,
							@number_of_incorrect_logins,
							@last_password_changed_date,
							@workstation,
							@connection_parameter_id,
							@language_id,
							[dbo].[MAC] (@username, @objid),@external_interface_id)

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
										 ', login ldap: ' + CAST(COALESCE(@connection_parameter_id, 0) as varchar(max)) + 
										 ', groups: ' + COALESCE(@groupname, 'none-selected')
				EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_insert_user_group_branches]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Insert link between users group and branches
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_user_group_branches] 
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
			--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_integration_bellid_get_sequence]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_integration_bellid_get_sequence] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @seq_no smallint,
			@current_date date

	SET @current_date = GETDATE()

    -- Insert statements for procedure here
	SELECT @seq_no = batch_sequence_number
	FROM [integration_bellid_batch_sequence]
	WHERE file_generation_date = @current_date

	IF @seq_no IS NULL
		BEGIN
			--Start of a new seqence for today.
			SET @seq_no = 1
			INSERT INTO [integration_bellid_batch_sequence] (file_generation_date, batch_sequence_number)
				VALUES (@current_date, @seq_no)
		END
	ELSE
		BEGIN
			--BECAUSE there is already a sequence for today, increment it and update the table.
			SET @seq_no = @seq_no + 1
			UPDATE [integration_bellid_batch_sequence]
			SET batch_sequence_number = @seq_no
			WHERE file_generation_date = @current_date
		END

	SELECT @seq_no

END





GO
/****** Object:  StoredProcedure [dbo].[usp_integration_get_default_values]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_integration_get_default_values] 
	-- Add the parameters for the stored procedure here
	@integration_name varchar(max),
	@integration_object_name varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT DISTINCT [integration_fields].integration_id,
						[integration_fields].integration_object_id,
						[integration_fields].integration_field_id,
						[integration_fields].integration_field_name, 
						CONVERT(VARCHAR(max),DECRYPTBYKEY([integration_fields].integration_field_default_value)) as integration_field_default_value
		FROM [integration]
				INNER JOIN [integration_object]
					ON [integration].integration_id = [integration_object].integration_id
				INNER JOIN [integration_fields]
					ON [integration_object].integration_id = [integration_fields].integration_id	
						AND [integration_object].integration_object_id = [integration_fields].integration_object_id				
		WHERE UPPER([integration].integration_name) = UPPER(@integration_name)
				AND UPPER([integration_object].integration_object_name) = UPPER(@integration_object_name)
				AND [integration_fields].integration_field_default_value IS NOT NULL

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[usp_integration_get_response_fields]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_integration_get_response_fields] 
	-- Add the parameters for the stored procedure here
	@integration_name varchar(max),
	@integration_object_name varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT DISTINCT [integration_fields].*
	FROM [integration]
			INNER JOIN [integration_object]
				ON [integration].integration_id = [integration_object].integration_id
			INNER JOIN [integration_fields]
				ON [integration_object].integration_id = [integration_fields].integration_id	
					AND [integration_object].integration_object_id = [integration_fields].integration_object_id
			INNER JOIN [integration_responses]
				ON [integration_fields].integration_id = [integration_responses].integration_id	
					AND [integration_fields].integration_object_id = [integration_responses].integration_object_id
					AND [integration_fields].integration_field_id = [integration_responses].integration_field_id
	WHERE UPPER([integration].integration_name) = UPPER(@integration_name)
			AND UPPER([integration_object].integration_object_name) = UPPER(@integration_object_name)
END




GO
/****** Object:  StoredProcedure [dbo].[usp_integration_get_response_values]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_integration_get_response_values] 
	-- Add the parameters for the stored procedure here
	@field_list AS dbo.trikey_value_array READONLY,
	@language_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [integration_responses].*, [integration_responses_language].response_text
	FROM [integration_responses]
			LEFT OUTER JOIN [integration_responses_language]
				ON [integration_responses].integration_id = [integration_responses_language].integration_id
					AND [integration_responses].integration_object_id = [integration_responses_language].integration_object_id
					AND [integration_responses].integration_field_id = [integration_responses_language].integration_field_id
					AND [integration_responses].integration_response_id = [integration_responses_language].integration_response_id
					AND [integration_responses_language].language_id = @language_id
			INNER JOIN @field_list fields
				ON [integration_responses].integration_id = fields.key1
					AND [integration_responses].integration_object_id = fields.key2
					AND [integration_responses].integration_field_id = fields.key3
					AND fields.value LIKE [integration_responses].integration_response_value
	 
END




GO
/****** Object:  StoredProcedure [dbo].[usp_issue_card_check_account_balance]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_issue_card_check_account_balance]
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
/****** Object:  StoredProcedure [dbo].[usp_issue_card_cms_edit_fail]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_issue_card_cms_edit_fail] 
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
			EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_issue_card_cms_fail]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_issue_card_cms_fail] 
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
			EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_issue_card_cms_relink_fail]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_issue_card_cms_relink_fail] 
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
			EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_issue_card_cms_retry]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_issue_card_cms_retry] 
	@card_id bigint,
	@error varchar(1000), 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION [ISSUE_CARD_CMS_RETRY_TRAN]
		BEGIN TRY 

			DECLARE @branch_card_code_id int,
					@current_card_status_id int,
					@status_date datetime

			--Check if a valid code was found for error, if not set as UNKNOWN Error
			IF @branch_card_code_id IS NULL
				SET @branch_card_code_id = 7

			SET @status_date = GETDATE()

			--Update the cards status.
			INSERT branch_card_status
					(card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id, 
					 branch_card_code_id, comments)
			VALUES (@card_id, 15, @status_date, @audit_user_id, @audit_user_id, @branch_card_code_id, @error) 


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
			WHERE branch_card_statuses_id = 15

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

			SET @audit_msg = '' + COALESCE(@branch_card_status_name, 'UNKNOWN') + 
							 ', ' + dbo.MaskString(@cardnumber, 6, 4) +
							 ', ' + @error
			--log the audit record		
			EXEC usp_insert_audit @audit_user_id, 
									3,
									NULL, 
									@audit_workstation, 
									@audit_msg, 
									NULL, NULL, NULL, NULL

		

			COMMIT TRANSACTION [ISSUE_CARD_CMS_RETRY_TRAN]

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ISSUE_CARD_CMS_RETRY_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_issue_card_cms_success]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_issue_card_cms_success] 
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
			EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_issue_card_complete]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_issue_card_complete] 
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
					EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_issue_card_PIN_captured]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_issue_card_PIN_captured] 
	@card_id bigint,
	@pin_auth_user_id bigint = null,
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
			IF(@current_card_status_id = 4 OR @current_card_status_id = 5)				
				BEGIN

					--TODO: Need to update card table the pin

					SET @status_date = GETDATE()

					--Update the cards status.
					INSERT branch_card_status
							(card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id, pin_auth_user_id)
					VALUES (@card_id, 5, @status_date, @audit_user_id, null, @pin_auth_user_id) 

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
					EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_issue_card_print_error]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_issue_card_print_error] 
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
			
			DECLARE @maker_checker bit,
					@current_card_status_id int,
					@status_date datetime
					

			--get the current status for the card
			SELECT @current_card_status_id = branch_card_statuses_id
			FROM branch_card_status_current
			WHERE card_id = @card_id

			SELECT @maker_checker = [issuer].maker_checker_YN
			FROM [issuer] INNER JOIN [branch]
					ON [issuer].issuer_id = [branch].issuer_id
					INNER JOIN [cards]
					ON [branch].branch_id = [cards].branch_id
			WHERE [cards].card_id = @card_id
										  
			--Check that someone hasn't already updated the card			
			IF ( (@current_card_status_id = 2 AND @maker_checker = 0) OR @current_card_status_id = 3)
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
					EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_issue_card_printed]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_issue_card_printed] 
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
			
			DECLARE @maker_checker bit,
			        @current_card_status_id int,
					@status_date datetime
					

			--get the current status for the card
			SELECT @current_card_status_id = branch_card_statuses_id
			FROM branch_card_status_current
			WHERE card_id = @card_id

			SELECT @maker_checker = [issuer].maker_checker_YN
			FROM [issuer] INNER JOIN [branch]
					ON [issuer].issuer_id = [branch].issuer_id
					INNER JOIN [cards]
					ON [branch].branch_id = [cards].branch_id
			WHERE [cards].card_id = @card_id
										  
			--Check that someone hasn't already updated the card
							
			IF ( (@current_card_status_id = 2 AND @maker_checker = 0) OR @current_card_status_id = 3)
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
					EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_issue_card_spoil]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_issue_card_spoil] 
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
					EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_issue_card_to_customer]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_issue_card_to_customer]
	@card_id bigint,	
    @customer_account_number varchar(27),
	@domicile_branch_id int,
	@account_type_id int,
	@card_issue_reason_id int,
	@customer_first_name varchar(50),
	@customer_middle_name varchar(50),
	@customer_last_name varchar(50),
	@name_on_card varchar(30),
	@customer_title_id int,
	@cms_id varchar(50),
	@contract_number varchar(50),
	@contact_number varchar(50),
	@id_number varchar(50),
	@currency_id int,
	@resident_id int,
	@customer_type_id int,
	@fee_waiver_YN bit = NULL,
	@fee_editable_YN bit = NULL,
	@fee_charged decimal(10,4) = NULL,
	@fee_overridden_YN bit = NULL,
	@product_fields as dbo.key_binary_value_array READONLY,
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
			
			DECLARE @new_customer_account_id bigint,
					@current_card_status_id int,
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
								customer_first_name, customer_middle_name, customer_last_name, name_on_card, contact_number,Id_number, customer_title_id, 
								date_issued, customer_type_id, currency_id, resident_id, cms_id, contract_number, domicile_branch_id)
					VALUES (@audit_user_id, @card_id, @card_issue_reason_id, @account_type_id, 
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_account_number)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_first_name)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_middle_name)), 
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@customer_last_name)), 
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@name_on_card)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@contact_number)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@id_number)) ,
							@customer_title_id, @status_date, @customer_type_id, @currency_id, @resident_id, 
							@cms_id, @contract_number, @domicile_branch_id)	
							
					SET @new_customer_account_id = SCOPE_IDENTITY()				
					
				

					--Update fee's for card
					UPDATE [cards]
					SET fee_waiver_YN = @fee_waiver_YN,
						fee_editable_YN = @fee_editable_YN, 
						fee_charged = @fee_charged,
						fee_overridden_YN = @fee_overridden_YN
					WHERE card_id = @card_id

					--Update Product Fields
					INSERT INTO customer_fields (customer_account_id, product_field_id, value)
						SELECT @new_customer_account_id, pf.[key], ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CAST(pf.[value] as varbinary(max)))
						FROM @product_fields pf INNER JOIN product_fields
							ON pf.[key] = product_fields.product_field_id
						WHERE product_fields.print_field_type_id = 0 
						--and product_fields.deleted=0

					--Update Product Image Fields
					INSERT INTO customer_image_fields (customer_account_id, product_field_id, value)
						SELECT @new_customer_account_id, pf.[key], pf.[value]
						FROM @product_fields pf INNER JOIN product_fields
							ON pf.[key] = product_fields.product_field_id
						WHERE product_fields.print_field_type_id = 1  
						--and product_fields.deleted=0
					
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
					EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_audit_actions]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_audit_actions] 
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
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_branch_card_statuses]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_branch_card_statuses] 
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
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_branch_statuses]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_branch_statuses] 
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
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_card_issue_method]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_card_issue_method]
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT        card_issue_method_language.language_text, card_issue_method_language.card_issue_method_id as lookup_id
  FROM            card_issue_method INNER JOIN
                         card_issue_method_language ON card_issue_method.card_issue_method_id = card_issue_method_language.card_issue_method_id
						 where card_issue_method_language.language_id=@language_id
END






GO
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_card_issue_reasons]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_card_issue_reasons] 
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
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_connection_parameter_type]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_connection_parameter_type]
	@language_id int,
		@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT  connection_parameter_type_id as lookup_id, language_text
	FROM connection_parameter_type_language 				
	WHERE language_id = @language_id
END






GO
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_customer_account_types]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_customer_account_types] 
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
	SELECT [customer_account_type].account_type_id AS lookup_id, language_text
	FROM [customer_account_type_language]
		INNER JOIN [customer_account_type]
			ON [customer_account_type_language].account_type_id = [customer_account_type].account_type_id
	WHERE language_id = @language_id
		AND active_YN = 1

END










GO
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_customer_residency]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_customer_residency] 
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
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_customer_title]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_customer_title] 
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
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_customer_type]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_customer_type] 
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
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_dist_batch_statuses]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_dist_batch_statuses] 
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
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_dist_card_statuses]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_dist_card_statuses] 
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
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_export_batch_statuses]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_export_batch_statuses] 
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
	SELECT export_batch_statuses_id AS lookup_id, language_text
	FROM [export_batch_statuses_language]
	WHERE language_id = @language_id
END










GO
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_external_system_types]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_external_system_types]
		@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [external_system_type_id] AS lookup_id, language_text
	FROM external_system_types_language
	WHERE language_id = @language_id
END

GO
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_file_encryption_type]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_file_encryption_type] 
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
	SELECT file_encryption_type_id AS lookup_id, file_encryption_type as language_text
	FROM [file_encryption_type]
	
END










GO
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_file_statuses]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_file_statuses] 
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
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_interface_types]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_interface_types] 
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
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_issuer_statuses]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_issuer_statuses] 
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
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_load_batch_statuses]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_load_batch_statuses] 
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
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_load_card_statuses]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_load_card_statuses] 
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
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_pin_batch_statuses]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_pin_batch_statuses] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  SELECT        language_text, pin_batch_statuses_id AS lookup_id
FROM            pin_batch_statuses_language
WHERE language_id = @language_id
END




GO
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_pin_reissue_statuses]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_pin_reissue_statuses] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  SELECT        language_text, pin_reissue_statuses_id AS lookup_id
FROM            pin_reissue_statuses_language
WHERE language_id = @language_id
END




GO
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_print_field_types]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_print_field_types]
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT print_field_type_id AS lookup_id, language_text
	FROM print_field_types_language
	WHERE language_id = @language_id

END


GO
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_product_load_type]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_product_load_type] 
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
	SELECT product_load_type_id AS lookup_id, language_text
	FROM [product_load_type_language]
	WHERE language_id = @language_id
END










GO
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_user_roles]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_user_roles] 
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
/****** Object:  StoredProcedure [dbo].[usp_lang_lookup_user_status]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_user_status] 
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
/****** Object:  StoredProcedure [dbo].[usp_list_branch_card_codes]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_list_branch_card_codes] 
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
/****** Object:  StoredProcedure [dbo].[usp_load_batch_approve]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Approve's the load batch. if issuer has auto create dist batch then create distbatch
-- =============================================
CREATE PROCEDURE [dbo].[usp_load_batch_approve] 
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
			
			DECLARE @current_load_batch_status_id int,
					@issuer_id int,
					@dist_batch_id bigint,
					@number_of_dist_cards int,
					@dist_batch_ref varchar(50),
					@dist_batch_status_name varchar(50),
					@audit_msg varchar(max)

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

					DECLARE @product_load_type_id int,
							@auto_approve bit,
							@card_issue_method_id int
										
					--STEP ONE: What do we do with the product in the load batch?
					--ASSUMPTION IS THAT LOAD IS DONE PER PRODUCT
					IF( (SELECT COUNT(DISTINCT product_id)
						 FROM [cards] INNER JOIN [load_batch_cards]
							ON [load_batch_cards].card_id = [cards].card_id
						 WHERE [load_batch_cards].load_batch_id = @load_batch_id) > 1)
						RAISERROR ('Multiple products found in batch.',	12,	12);

					--Get the load type for the product
					SELECT @product_load_type_id = product_load_type_id,
							@auto_approve = auto_approve_batch_YN,
							@card_issue_method_id = card_issue_method_id
					FROM [issuer_product] 
					WHERE product_id = (SELECT TOP 1 product_id 
										FROM [cards] INNER JOIN [load_batch_cards]
											ON [load_batch_cards].card_id = [cards].card_id
										WHERE [load_batch_cards].load_batch_id = @load_batch_id)

					--STEP TWO-a: Create Production Batch
					IF(@product_load_type_id = 1 OR @product_load_type_id = 5)
						BEGIN
							--Fetch CC, if no card centre use null
							DECLARE @card_centre_id int	= null							

							SELECT TOP 1 @card_centre_id = branch_id, @issuer_id = issuer_id
							FROM [branch]
							WHERE card_centre_branch_YN = 1 AND 
								issuer_id = (SELECT TOP 1 issuer_id
											 FROM [branch] 
													INNER JOIN [cards]
														ON [branch].branch_id = [cards].branch_id
													INNER JOIN [load_batch_cards]
														ON [cards].card_id = [load_batch_cards].card_id
											 WHERE [load_batch_cards].load_batch_id = @load_batch_id)
							
							--Create new production batch
							INSERT INTO [dist_batch] (branch_id, no_cards, date_created, dist_batch_reference, 
														card_issue_method_id, dist_batch_type_id, issuer_id)
							 VALUES (@card_centre_id, 0, GETDATE(), GETDATE(), @card_issue_method_id, 0, @issuer_id)

							 SET @dist_batch_id = SCOPE_IDENTITY();

							 --Link cards
							 INSERT INTO [dist_batch_cards] (dist_batch_id, card_id, dist_card_status_id)
							 SELECT @dist_batch_id, card_id, 0
							 FROM [load_batch_cards]
							 WHERE load_batch_id = @load_batch_id

							 --Find the number of cards
							 SELECT @number_of_dist_cards = COUNT(card_id)
							 FROM [dist_batch_cards]
							 WHERE dist_batch_id = @dist_batch_id

							--Update batch reference
							--Generate dist batch reference
							IF(@product_load_type_id = 1)
								BEGIN
									SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
																[branch].branch_code + '' + 
																CONVERT(VARCHAR(8), GETDATE(), 112) + '' +
																CAST(@dist_batch_id AS varchar(max))
									FROM [branch] INNER JOIN [issuer]
										ON [branch].issuer_id = [issuer].issuer_id
									WHERE [branch].branch_id = @card_centre_id
								END
							ELSE
								BEGIN
									SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
																CONVERT(VARCHAR(MAX), [cards].product_id) + '' +										  
																CONVERT(VARCHAR(8), GETDATE(), 112) + '' +
																CAST(@dist_batch_id AS varchar(max))
									FROM [load_batch_cards] 
										INNER JOIN [cards] ON [load_batch_cards].card_id = cards.card_id
										INNER JOIN [branch] ON [branch].branch_id = [cards].branch_id
										INNER JOIN [issuer] ON [branch].issuer_id = [issuer].issuer_id
								END

							--UPDATE dist batch with reference and number of cards
							UPDATE [dist_batch]
							SET [dist_batch_reference] = @dist_batch_ref,
								[no_cards] = @number_of_dist_cards
							WHERE [dist_batch].dist_batch_id = @dist_batch_id

							--Add status of created
							INSERT INTO dist_batch_status(dist_batch_id, dist_batch_statuses_id, [user_id], status_date, status_notes)
							VALUES (@dist_batch_id, 0, @audit_user_id, GETDATE(), 'Batch Created')

							--UPDATE the load batch cards status to allocated							
							UPDATE [load_batch_cards]
							SET [load_batch_cards].load_card_status_id = 2
							WHERE [load_batch_cards].card_id IN 
									(SELECT [dist_batch_cards].card_id
										FROM [dist_batch_cards]
										WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id)

						
							SELECT @dist_batch_status_name =  dist_batch_status_name
							FROM dist_batch_statuses
							WHERE dist_batch_statuses_id = 0

						
							IF(@product_load_type_id = 5)
							BEGIN
								-- Assign card requests at branch to assigned to batch
								-- UPDATE branch card status for those cards that have been added to the new dist batch.
								INSERT INTO [branch_card_status]
									(branch_card_statuses_id, card_id, comments, status_date, [user_id])
								SELECT 10, card_id, 'Assigned to batch', GETDATE(), @audit_user_id
								FROM dist_batch_cards
								WHERE dist_batch_id = @dist_batch_id	
							END

							--Add audit for dist batch creation								
							SET @audit_msg = 'Create: ' + CAST(@dist_batch_id AS varchar(max)) +
												', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
												', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
							--log the audit record		
							EXEC usp_insert_audit @audit_user_id, 
													2,
													NULL, 
													@audit_workstation, 
													@audit_msg, 
													NULL, NULL, NULL, NULL

							--Check we've done it all correctly


							--AUTO-APPROVE


						END
					--STEP TWO-b: Create Distribution Batch
					ELSE IF(@product_load_type_id = 2 OR @product_load_type_id = 6)
						BEGIN	

							--Get a distinct list of branches from the load batch
							DECLARE @branch_id int,
									@cards_total int = 0

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

								--IF(SELECT [issuer].auto_create_dist_batch
								--	FROM [issuer] INNER JOIN [branch]
								--			ON [issuer].issuer_id = [branch].issuer_id
								--	WHERE [branch].branch_id = @branch_id) = 1
								--	BEGIN

										--create the distribution batch
										INSERT INTO [dist_batch]
											([issuer_id], [branch_id], [no_cards],[date_created],[dist_batch_reference], 
												[card_issue_method_id],	[dist_batch_type_id])
										SELECT issuer_id, @branch_id, 0, GETDATE(), GETDATE(), @card_issue_method_id, 1
										FROM [branch]
										WHERE branch_id = @branch_id
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

										SELECT @dist_batch_status_name =  dist_batch_status_name
										FROM dist_batch_statuses
										WHERE dist_batch_statuses_id = 0

										--Add audit for dist batch creation								
										SET @audit_msg = 'Create: ' + CAST(@dist_batch_id AS varchar(max)) +
														 ', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
														 ', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
										--log the audit record		
										EXEC usp_insert_audit @audit_user_id, 
															 2,
															 NULL, 
															 @audit_workstation, 
															 @audit_msg, 
															 NULL, NULL, NULL, NULL

										IF(@auto_approve = 1)
										BEGIN
											--auto add approve the distbatch
											INSERT INTO [dbo].[dist_batch_status]
												([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
											VALUES(@dist_batch_id, 1, @audit_user_id, DATEADD(ss,1,GETDATE()), 'Auto Dist Batch Create Aproval')								

											SELECT @dist_batch_status_name =  dist_batch_status_name
											FROM dist_batch_statuses
											WHERE dist_batch_statuses_id = 1

											--Add audit for dist batch update								
											SET @audit_msg = 'Update: ' + CAST(@dist_batch_id AS varchar(max)) +
															 ', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
															 ', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
											--log the audit record		
											EXEC usp_insert_audit @audit_user_id, 
																 2,
																 NULL, 
																 @audit_workstation, 
																 @audit_msg, 
																 NULL, NULL, NULL, NULL
										END

								--	END
								--ELSE
								--	BEGIN
								--		--Update the cards linked to the load batch and cursors branch with the available status.
								--		UPDATE [load_batch_cards]
								--		SET load_card_status_id = 1
								--		FROM [load_batch_cards] INNER JOIN [cards]
								--			 ON [load_batch_cards].card_id = [cards].card_id
								--		WHERE [load_batch_cards].load_batch_id = @load_batch_id
								--				AND [cards].branch_id = @branch_id

								--		--Get the number of cards updated
								--		SELECT @number_of_dist_cards = @@ROWCOUNT
								--	END						

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
						END
					--STEP TWO-c: Create Card Centre Card Stock
					ELSE IF(@product_load_type_id = 3)
						BEGIN
							UPDATE [load_batch_cards] SET [load_card_status_id]=1 WHERE load_batch_id=@load_batch_id 
							-- WHERE card_id=(SELECT avail_cc_and_load_cards.card_id FROM avail_cc_and_load_cards
							--				INNER JOIN load_batch_cards on avail_cc_and_load_cards.card_id = load_batch_cards.card_id
							--				WHERE load_batch_id=@load_batch_id )
						END
					--STEP TWO-d: Update Production Batch
					ELSE IF(@product_load_type_id = 4)
						BEGIN
							UPDATE [dist_batch_cards]
							SET dist_card_status_id = 21
							WHERE card_id IN (SELECT card_id
												FROM [load_batch_cards]
												WHERE load_batch_id = @load_batch_id)

						
							INSERT INTO [dist_batch_status] (dist_batch_id, dist_batch_statuses_id, status_date, status_notes, [user_id])
							SELECT DISTINCT dbc.dist_batch_id, 21, GETDATE(), 'All cards loaded', @audit_user_id
							FROM [dist_batch] dbc
								INNER JOIN [dist_batch_cards]
									ON [dist_batch_cards].dist_batch_id = dbc.dist_batch_id
								INNER JOIN [load_batch_cards]
									ON [dist_batch_cards].card_id = [load_batch_cards].card_id
							WHERE 21 = ALL(SELECT dist_card_status_id 
											FROM [dist_batch_cards] dbc2 						
											WHERE dbc2.dist_batch_id = dbc.dist_batch_id)
									AND [load_batch_cards].load_batch_id = @load_batch_id
						END					
					ELSE
						RAISERROR ('Unkown load batch type.', 12, 12);


					--STEP THREE: Approve load batch
					--Update the load batch status.
					INSERT load_batch_status
							([load_batch_id], [load_batch_statuses_id], [user_id], [status_date], [status_notes])
					VALUES (@load_batch_id, 1, @audit_user_id, GETDATE(), @status_notes)
	
					--STEP FOUR: Log Audit
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
					EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_load_batch_reject]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Reject the load batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_load_batch_reject] 
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
					EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_load_issuer_license]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Loads license information for issuer
-- =============================================
CREATE PROCEDURE [dbo].[usp_load_issuer_license] 
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
					--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_lookup_response_message]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_lookup_response_message] 
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
/****** Object:  StoredProcedure [dbo].[usp_modify_external_system_fields]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_modify_external_system_fields]
	@external_system_id int,
	@external_system_fields as dbo.[product_external_fields_array] READONLY,	
	@audit_user_id BIGINT,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--BEGIN TRANSACTION [CRATE_EXTERNAL_FILEDS_TRANS]
		BEGIN TRY 
							DECLARE @external_system_field_id int,
							 @fieldname nvarchar(100)

							CREATE TABLE #Temp ( external_system_field_id  integer );
							 
				     DECLARE External_Systems_Field CURSOR FOR 
								SELECT esx.external_system_field_id,esx.field_name
							FROM @external_system_fields esx
						

							OPEN External_Systems_Field

							FETCH NEXT FROM External_Systems_Field 
							INTO @external_system_field_id,@fieldname

							WHILE @@FETCH_STATUS = 0
							BEGIN

							IF ((SELECT count(*) FROM external_system_fields WHERE external_system_field_id = @external_system_field_id)=0) 
								BEGIN 
									 INSERT INTO external_system_fields(external_system_id, field_name) 
									 values(@external_system_id,@fieldname)
									 
									 insert #Temp(external_system_field_id) values(SCOPE_IDENTITY())
									 

								END 
								ELSE 
								BEGIN 
								
									 UPDATE external_system_fields
									SET external_system_id=@external_system_id,
									  field_name=@fieldname
									  where external_system_field_id=@external_system_field_id
									
								END 				
													
							FETCH NEXT FROM External_Systems_Field 
							INTO @external_system_field_id,@fieldname
								END 
								
							
							CLOSE External_Systems_Field;
							DEALLOCATE External_Systems_Field;



							DELETE FROM external_system_fields 
							WHERE external_system_field_id NOT IN(SELECT external_system_field_id FROM @external_system_fields   )
											AND external_system_id=@external_system_id
											AND external_system_field_id NOT IN(select external_system_field_id from #Temp)

											
							--COMMIT TRANSACTION [CRATE_EXTERNAL_FILEDS_TRANS]
					

		END TRY
	BEGIN CATCH
		--ROLLBACK TRANSACTION [CRATE_EXTERNAL_FILEDS_TRANS]
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
/****** Object:  StoredProcedure [dbo].[usp_pin_batch_status_change]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Change pin batch status
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_batch_status_change] 
	@pin_batch_id bigint,
	@status_notes varchar(150) = null,
	@pin_batch_status_id int,
	@new_pin_batch_status_id int,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [PIN_BATCH_STATUS_CHANGE]
		BEGIN TRY 
			
			DECLARE @current_dist_batch_status_id int,
					@audit_msg varchar(max),
					@original_batch_type_id int,
					@new_batch_type_id int,
					@new_pin_card_statuses_id int		
					
			--If the statuses are the same then we arent changing the batches status, just return pin batch
			--IF(@pin_batch_status_id = @new_pin_batch_status_id)	
			--	BEGIN
			--		SET @ResultCode = 0	
			--	END					  
			--Check that someone hasn't already updated the dist batch
			IF(dbo.PinBatchInCorrectStatus(@pin_batch_status_id, @new_pin_batch_status_id, @pin_batch_id) = 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN		
					--Check if we need to create dist batch
					SELECT @original_batch_type_id = [pin_batch_statuses_flow].pin_batch_type_id,
							  @new_batch_type_id = flow_pin_batch_type_id,
							  @new_pin_card_statuses_id = flow_pin_card_statuses_id
						FROM [pin_batch_statuses_flow]
							INNER JOIN [pin_batch]
								ON [pin_batch_statuses_flow].card_issue_method_id = [pin_batch].card_issue_method_id
									AND [pin_batch_statuses_flow].pin_batch_type_id = [pin_batch].pin_batch_type_id
							INNER JOIN [pin_batch_status_current]
								ON [pin_batch_status_current].pin_batch_statuses_id = [pin_batch_statuses_flow].pin_batch_statuses_id
									AND [pin_batch_status_current].pin_batch_id = [pin_batch].pin_batch_id
					WHERE [pin_batch].pin_batch_id = @pin_batch_id

					--Update the pin batch status.
					INSERT pin_batch_status
							([pin_batch_id], [pin_batch_statuses_id], [user_id], [status_date], [status_notes])
					VALUES (@pin_batch_id, @new_pin_batch_status_id, @audit_user_id, GETDATE(), @status_notes)

					IF(@new_pin_card_statuses_id IS NOT NULL)
					BEGIN
						--Update the cards linked to the dist batch with the new status.
						UPDATE pin_batch_cards
						SET pin_batch_cards_statuses_id = @new_pin_card_statuses_id
						WHERE pin_batch_id = @pin_batch_id

						IF (@new_pin_card_statuses_id = 4)
						BEGIN
							--Update the pin reprint cards to completed.
							INSERT INTO [pin_mailer_reprint] (card_id, comments, pin_mailer_reprint_status_id, status_date, [user_id])
							SELECT [pin_mailer_reprint].card_id, '', 3, GETDATE(), @audit_user_id
							FROM [pin_batch_cards]
									INNER JOIN [pin_mailer_reprint]
										ON [pin_batch_cards].card_id = [pin_mailer_reprint].card_id
										AND [pin_mailer_reprint].pin_mailer_reprint_status_id = 2
							WHERE pin_batch_id = @pin_batch_id
						END
					END

					IF(@original_batch_type_id != 1 AND @new_batch_type_id = 1)
					BEGIN
						EXEC [usp_pin_prod_to_pin_batch] @pin_batch_id,
														@audit_user_id,
														@audit_workstation
					END
				
					--AUDIT
					DECLARE @pin_batch_status_name varchar(50),
							@pin_batch_ref varchar(100)

					SELECT @pin_batch_status_name =  pin_batch_statuses_name
					FROM pin_batch_statuses
					WHERE pin_batch_statuses_id = @pin_batch_status_id

					SELECT @pin_batch_ref = pin_batch_reference
					FROM pin_batch
					WHERE pin_batch_id = @pin_batch_id

					--Add audit for pin batch update								
					SET @audit_msg = 'Update: ' + CAST(@pin_batch_id AS varchar(max)) +
										', ' + COALESCE(@pin_batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@pin_batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					SET @ResultCode = 0					
				END

				--Fetch the pin batch with latest details
				EXEC usp_get_pin_batch @pin_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [PIN_BATCH_STATUS_CHANGE]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [PIN_BATCH_STATUS_CHANGE]
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
/****** Object:  StoredProcedure [dbo].[usp_pin_batch_status_reject]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Change batch status - Reject
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_batch_status_reject] 
	@pin_batch_id bigint,
	@new_pin_batch_status_id int,
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

	BEGIN TRANSACTION [PIN_BATCH_STATUS_CHANGE_REJECT]
		BEGIN TRY 
			
			DECLARE @audit_msg varchar(max),
					@new_pin_card_statuses_id int

			--Check that someone hasn't already updated the pin batch
			IF(dbo.PinBatchInCorrectStatusReject(@new_pin_batch_status_id, @pin_batch_id) = 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN
					SELECT @new_pin_card_statuses_id = reject_pin_card_statuses_id
						FROM [pin_batch_statuses_flow]
							INNER JOIN [pin_batch]
								ON [pin_batch_statuses_flow].card_issue_method_id = [pin_batch].card_issue_method_id
									AND [pin_batch_statuses_flow].pin_batch_type_id = [pin_batch].pin_batch_type_id
							INNER JOIN [pin_batch_status_current]
								ON [pin_batch_status_current].pin_batch_statuses_id = [pin_batch_statuses_flow].pin_batch_statuses_id
									AND [pin_batch_status_current].pin_batch_id = [pin_batch].pin_batch_id
					WHERE [pin_batch].pin_batch_id = @pin_batch_id
							AND [pin_batch_statuses_flow].reject_pin_batch_statuses_id = @new_pin_batch_status_id


					--Update the batch status.
					INSERT [pin_batch_status]
							([pin_batch_id], [pin_batch_statuses_id], [user_id], [status_date], [status_notes])
					VALUES (@pin_batch_id, @new_pin_batch_status_id, @audit_user_id, GETDATE(), @status_notes)

					--Check if we need to update the card status
					IF (@new_pin_card_statuses_id IS NOT NULL)
					BEGIN 
						--Update the cards linked to the pin batch with the new status.
						UPDATE pin_batch_cards
						SET pin_batch_cards_statuses_id = @new_pin_card_statuses_id
						WHERE pin_batch_id = @pin_batch_id
					END
				
					--AUDIT
					DECLARE @batch_status_name varchar(100),
							@batch_ref varchar(100)

					SELECT @batch_status_name =  pin_batch_statuses_name
					FROM pin_batch_statuses
					WHERE pin_batch_statuses_id = @new_pin_batch_status_id

					SELECT @batch_ref = pin_batch_reference
					FROM pin_batch
					WHERE pin_batch_id = @pin_batch_id

					--Add audit for pin batch update								
					SET @audit_msg = 'Update: ' + CAST(@pin_batch_id AS varchar(max)) +
										', ' + COALESCE(@batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					SET @ResultCode = 0					
				END

				--Fetch the batch with latest details
				EXEC usp_get_pin_batch @pin_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [PIN_BATCH_STATUS_CHANGE_REJECT]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [PIN_BATCH_STATUS_CHANGE_REJECT]
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
/****** Object:  StoredProcedure [dbo].[usp_pin_mailer_report]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_mailer_report] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = NULL,
	@branch_id int = NULL,
	@date_from datetime,
	@date_to datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @date_to = DATEADD(d, 1, @date_to)

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

				SELECT DISTINCT
				CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_account_number)) AS customer_account_number,				
				[pin_batch].pin_batch_reference, [pin_batch].date_created,
				[issuer].issuer_name, [issuer].issuer_code, [branch].branch_name, branch.branch_code
		FROM [cards]
				INNER JOIN [customer_account]
					ON [cards].card_id = [customer_account].card_id
				INNER JOIN [pin_batch_cards]
					ON [cards].card_id = [pin_batch_cards].card_id
				INNER JOIN [pin_batch]
					ON [pin_batch_cards].pin_batch_id = [pin_batch].pin_batch_id					
				INNER JOIN [branch]
					ON [cards].branch_id = [branch].branch_id
				INNER JOIN [issuer]
					ON [branch].issuer_id = [issuer].issuer_id	
		WHERE [cards].card_issue_method_id = 0
				AND [issuer].issuer_id = COALESCE(@issuer_id, [issuer].issuer_id)
				AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
				AND [pin_batch].date_created >= @date_from
				AND [pin_batch].date_created <= @date_to
		ORDER BY issuer_name, issuer_code, branch_name, branch_code, date_created,
				pin_batch_reference, customer_account_number

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END





GO
/****** Object:  StoredProcedure [dbo].[usp_pin_mailer_reprint_approve]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_mailer_reprint_approve] 
	-- Add the parameters for the stored procedure here
	@card_id bigint,
	@comment nvarchar(1000),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [TRAN_PIN_REPRINT_APPROVE]
		BEGIN TRY 
			
			DECLARE @audit_msg varchar(max),
					@pin_mailer_reprint_id bigint,
					@new_batch_type_id int,
					@new_dist_card_statuses_id int

			IF ( (SELECT COUNT(*) FROM [pin_mailer_reprint_status_current] 
					WHERE [pin_mailer_reprint_status_current].pin_mailer_reprint_status_id != 0 AND 
						  [pin_mailer_reprint_status_current].card_id = @card_id) > 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN

					DECLARE @pin_mailer_reprint_status_id int = 1
					--Insert create pin request
					INSERT INTO [pin_mailer_reprint] (card_id, comments, pin_mailer_reprint_status_id, status_date, [user_id])
						VALUES (@card_id, @comment, @pin_mailer_reprint_status_id, GETDATE(), @audit_user_id)
					
					--Add audit for pin reprint update								
					SET @audit_msg = 'create: ' + CAST(@card_id AS varchar(max)) +
										', pin mailer reprint approved by ' + CAST(@audit_user_id AS varchar(max)) 
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					 

					SET @ResultCode = 0	
				END	

				COMMIT TRANSACTION [TRAN_PIN_REPRINT_APPROVE]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [TRAN_PIN_REPRINT_APPROVE]
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
/****** Object:  StoredProcedure [dbo].[usp_pin_mailer_reprint_reject]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_mailer_reprint_reject] 
	-- Add the parameters for the stored procedure here
	@card_id bigint,
	@comment nvarchar(1000),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [TRAN_PIN_REPRINT_REJECT]
		BEGIN TRY 
			
			DECLARE @audit_msg varchar(max),
					@pin_mailer_reprint_id bigint,
					@new_batch_type_id int,
					@new_dist_card_statuses_id int

			IF ( (SELECT COUNT(*) FROM [pin_mailer_reprint_status_current] 
					WHERE [pin_mailer_reprint_status_current].pin_mailer_reprint_status_id != 0 AND 
						  [pin_mailer_reprint_status_current].card_id = @card_id) > 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN

					DECLARE @pin_mailer_reprint_status_id int = 4
					--Insert create pin request
					INSERT INTO [pin_mailer_reprint] (card_id, comments, pin_mailer_reprint_status_id, status_date, [user_id])
						VALUES (@card_id, @comment, @pin_mailer_reprint_status_id, GETDATE(), @audit_user_id)
					
					--Add audit for pin reprint update								
					SET @audit_msg = 'create: ' + CAST(@card_id AS varchar(max)) +
										', pin mailer reprint rejected by ' + CAST(@audit_user_id AS varchar(max)) 
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					 

					SET @ResultCode = 0	
				END	

				COMMIT TRANSACTION [TRAN_PIN_REPRINT_REJECT]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [TRAN_PIN_REPRINT_REJECT]
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
/****** Object:  StoredProcedure [dbo].[usp_pin_mailer_reprint_report]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_mailer_reprint_report] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = NULL,
	@branch_id int = NULL,
	@date_from datetime,
	@date_to datetime,
	@audit_userid int=null,
	@audit_workstation nvarchar(50) =null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @date_to = DATEADD(d, 1, @date_to)

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

				SELECT DISTINCT
				CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_account_number)) AS customer_account_number,				
				[pin_batch].pin_batch_reference, [pin_batch].date_created,
				[issuer].issuer_name, [issuer].issuer_code, [branch].branch_name, branch.branch_code,
				CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_first_name))+''+CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_name'
		FROM [cards]
				INNER JOIN [customer_account]
					ON [cards].card_id = [customer_account].card_id
				INNER JOIN [pin_batch_cards]
					ON [cards].card_id = [pin_batch_cards].card_id
				INNER JOIN [pin_batch]
					ON [pin_batch_cards].pin_batch_id = [pin_batch].pin_batch_id
					INNER JOIN [pin_mailer_reprint]	ON 	[pin_mailer_reprint].card_id=[pin_batch_cards].card_id		
				INNER JOIN [branch]
					ON [cards].branch_id = [branch].branch_id
				INNER JOIN [issuer]
					ON [branch].issuer_id = [issuer].issuer_id	
		WHERE [cards].card_issue_method_id = 0
				AND [issuer].issuer_id = COALESCE(null, [issuer].issuer_id)
				AND [branch].branch_id = COALESCE(null, [branch].branch_id)
				AND [pin_batch].date_created >= @date_from
				AND [pin_batch].date_created <= @date_to
				AND [pin_batch].pin_batch_type_id=2 AND pin_mailer_reprint_status_id=1
		ORDER BY issuer_name, issuer_code, branch_name, branch_code, date_created,
				pin_batch_reference, customer_account_number

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END





GO
/****** Object:  StoredProcedure [dbo].[usp_pin_mailer_reprint_request]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_mailer_reprint_request] 
	-- Add the parameters for the stored procedure here
	@card_id bigint, 
	@comment nvarchar(1000),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [TRAN_PIN_REPRINT_REQUEST]
		BEGIN TRY 
			
			DECLARE @audit_msg varchar(max),
					@pin_mailer_reprint_id bigint,
					@new_batch_type_id int,
					@new_dist_card_statuses_id int

			IF ( (SELECT COUNT(*) FROM [pin_mailer_reprint_status_current] 
					WHERE [pin_mailer_reprint_status_current].pin_mailer_reprint_status_id NOT IN (3, 4) AND 
						  [pin_mailer_reprint_status_current].card_id = @card_id) > 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN

					DECLARE @pin_mailer_reprint_status_id int = 0
					--Insert create pin request
					INSERT INTO [pin_mailer_reprint] (card_id, comments, pin_mailer_reprint_status_id, status_date, [user_id])
						VALUES (@card_id, @comment, @pin_mailer_reprint_status_id, GETDATE(), @audit_user_id)

					--Depending if maker/checker is on, do auto approve
					IF((SELECT TOP(1) [issuer].maker_checker_YN
						FROM [issuer]
								INNER JOIN [branch]
									ON [issuer].issuer_id = [branch].issuer_id
								INNER JOIN [cards]
									ON [branch].branch_id = [cards].branch_id
						WHERE [cards].card_id = @card_id) = 0)
					BEGIN
						SET @pin_mailer_reprint_status_id = 1

						INSERT INTO [pin_mailer_reprint] (card_id, comments, pin_mailer_reprint_status_id, status_date, [user_id])
						VALUES (@card_id, 'Auto Approved: '+@comment, @pin_mailer_reprint_status_id, DATEADD(second, 1, GETDATE()), @audit_user_id)
					END

					--Add audit for pin reprint update								
					SET @audit_msg = 'create: ' + CAST(@card_id AS varchar(max)) +
										', pin mailer reprint requested by ' + CAST(@audit_user_id AS varchar(max)) 
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					 

					SET @ResultCode = 0	
				END	

				COMMIT TRANSACTION [TRAN_PIN_REPRINT_REQUEST]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [TRAN_PIN_REPRINT_REQUEST]
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
/****** Object:  StoredProcedure [dbo].[usp_pin_prod_to_pin_batch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_prod_to_pin_batch] 
	-- Add the parameters for the stored procedure here
	@pin_batch_id bigint, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

		--Get a pininct list of branches from the batch
		DECLARE @branch_id int,
				@cards_total int = 0,
				@card_issue_method_id int,
				@check_prod_batch_id int,
				@new_pin_batch_id int,
				@audit_msg varchar(max)

		SELECT @card_issue_method_id = card_issue_method_id, @check_prod_batch_id = pin_batch_type_id
		FROM pin_batch
		WHERE pin_batch_id = @pin_batch_id

		IF (@check_prod_batch_id = 1)
			RAISERROR ('Can only create pin distribution batchs from a production batch.', 12, 12 );


		--Loop through all pininct branches for the production batch and create pinribution batches
		DECLARE branchId_cursor CURSOR FOR 
			SELECT DISTINCT [cards].branch_id
			FROM [cards] 
				INNER JOIN [pin_batch_cards]
					ON [cards].card_id = [pin_batch_cards].card_id
			WHERE [pin_batch_cards].pin_batch_id = @pin_batch_id
						

		OPEN branchId_cursor

		FETCH NEXT FROM branchId_cursor 
		INTO @branch_id

		WHILE @@FETCH_STATUS = 0
		BEGIN
			DECLARE @number_of_pin_cards int = 0,
					@pin_status_date datetime = GETDATE()						
								
					--create the batch
					INSERT INTO [pin_batch]
						([issuer_id], [branch_id], [no_cards],[date_created],[pin_batch_reference], [card_issue_method_id], [pin_batch_type_id])
					SELECT issuer_id, @branch_id, 0, @pin_status_date, @pin_status_date, @card_issue_method_id, 1
					FROM [branch]
					WHERE branch_id = @branch_id

					SET @new_pin_batch_id = SCOPE_IDENTITY();

					--Add cards to pinribution batch
					INSERT INTO [pin_batch_cards]
						([pin_batch_id],[card_id],[pin_batch_cards_statuses_id])
					SELECT
						@new_pin_batch_id, [cards].card_id, 0
					FROM [cards] INNER JOIN [pin_batch_cards]
						ON [cards].card_id = [pin_batch_cards].card_id
					WHERE [pin_batch_cards].pin_batch_id = @pin_batch_id
							AND [cards].branch_id = @branch_id
							
					--Get the number of cards inserted
					SELECT @number_of_pin_cards = @@ROWCOUNT										

					--add pin batch status of created
					INSERT INTO [dbo].[pin_batch_status]
						([pin_batch_id],[pin_batch_statuses_id],[user_id],[status_date],[status_notes])
					VALUES(@new_pin_batch_id, 0, @audit_user_id, @pin_status_date, 'PIN Distribution Batch Create From: ' + CONVERT(VARCHAR(max),@pin_batch_id))

					--Generate pin batch reference
					DECLARE @pin_batch_ref varchar(50)
					SELECT @pin_batch_ref =  [issuer].issuer_code + '' + 
												[branch].branch_code + '' + 
												CONVERT(VARCHAR(8), @pin_status_date, 112) + '' +
												CAST(@new_pin_batch_id AS varchar(max))
					FROM [branch] INNER JOIN [issuer]
						ON [branch].issuer_id = [issuer].issuer_id
					WHERE [branch].branch_id = @branch_id

					--UPDATE pin batch with reference and number of cards
					UPDATE [pin_batch]
					SET [pin_batch_reference] = @pin_batch_ref,
						[no_cards] = @number_of_pin_cards
					WHERE [pin_batch].pin_batch_id = @new_pin_batch_id

					--UPDATE the production batch cards status to allocated							
					UPDATE [pin_batch_cards]
					SET [pin_batch_cards].pin_batch_cards_statuses_id = 1
					WHERE pin_batch_id = @pin_batch_id
						AND	[pin_batch_cards].card_id IN 
							(SELECT [pin_batch_cards].card_id
								FROM [pin_batch_cards]
								WHERE [pin_batch_cards].pin_batch_id = @new_pin_batch_id)
							

					DECLARE @pin_batch_status_name varchar(50)
					SELECT @pin_batch_status_name =  pin_batch_statuses_name
					FROM pin_batch_statuses
					WHERE pin_batch_statuses_id = 0

					--Add audit for pin batch creation								
					SET @audit_msg = 'Create: ' + CAST(@new_pin_batch_id AS varchar(max)) +
										', ' + COALESCE(@pin_batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@pin_batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					
					----auto add approve the pinbatch
					--INSERT INTO [dbo].[pin_batch_status]
					--	([pin_batch_id],[pin_batch_statuses_id],[user_id],[status_date],[status_notes])
					--VALUES(@new_pin_batch_id, 1, @audit_user_id, DATEADD(ss,1,@pin_status_date), 'Auto pin Batch Create Approval')								

					--SELECT @pin_batch_status_name =  pin_batch_statuses_name
					--FROM pin_batch_statuses
					--WHERE pin_batch_statuses_id = 1

					----Add audit for pin batch update								
					--SET @audit_msg = 'Update: ' + CAST(@new_pin_batch_id AS varchar(max)) +
					--					', ' + COALESCE(@pin_batch_ref, 'UNKNOWN') +
					--					', ' + COALESCE(@pin_batch_status_name, 'UNKNOWN')
								   
					----log the audit record		
					--EXEC usp_insert_audit @audit_user_id, 
					--						2,
					--						NULL, 
					--						@audit_workstation, 
					--						@audit_msg, 
					--						NULL, NULL, NULL, NULL

				

			SELECT @cards_total += @number_of_pin_cards

				-- Get the next branch.
			FETCH NEXT FROM branchId_cursor 
			INTO @branch_id
			END 
		CLOSE branchId_cursor;
		DEALLOCATE branchId_cursor;

		--Check that all cards for the load batch have been updated
		IF (SELECT COUNT(card_id) FROM [pin_batch_cards] WHERE pin_batch_id = @pin_batch_id) != @cards_total
		BEGIN
			RAISERROR ('Not all cards have been moved from production batch to pinribution batch.',
						12,
						12 );
		END
END







GO
/****** Object:  StoredProcedure [dbo].[usp_pin_reissue_approve]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_reissue_approve]
	-- Add the parameters for the stored procedure here
	@pin_reissue_id bigint,
	@notes varchar(100),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [APPROVE_PIN_REISSUE_TRAN]
		BEGIN TRY 

		IF((SELECT pin_reissue_statuses_id FROM [pin_reissue_status_current] 
			WHERE pin_reissue_status_current.pin_reissue_id = @pin_reissue_id) != 0)
			BEGIN
				SET @ResultCode = 800
			END
		ELSE
			BEGIN
				INSERT INTO [pin_reissue_status] (pin_reissue_id, pin_reissue_statuses_id, [user_id], audit_workstation, comments, status_date)
				VALUES (@pin_reissue_id, 1, @audit_user_id, @audit_workstation, @notes, GETDATE())

				UPDATE [pin_reissue]
				SET authorise_user_id = @audit_user_id
				WHERE pin_reissue_id = @pin_reissue_id		
			
				OPEN SYMMETRIC KEY Indigo_Symmetric_Key
				DECRYPTION BY CERTIFICATE Indigo_Certificate

				DECLARE @cardnumber varchar(50),
						@issuer_id int,
						@pin_reissue_statuses_name varchar(100),
						@audit_msg varchar(max)

				SELECT @cardnumber = CONVERT(VARCHAR(max),DECRYPTBYKEY(pan)) ,
						@issuer_id = issuer_id
				FROM [pin_reissue] 
				WHERE pin_reissue_id = @pin_reissue_id

				SELECT @pin_reissue_statuses_name = pin_reissue_statuses_id
				FROM [pin_reissue_statuses]
				WHERE pin_reissue_statuses_id = 1

				CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

				SET @audit_msg = '' + COALESCE(@pin_reissue_statuses_name, 'UNKNOWN') + 
										 ', ' + dbo.MaskString(@cardnumber, 6, 4)
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										10,
										NULL, 
										@audit_workstation, 
										@audit_msg,
										@issuer_id, NULL, NULL, NULL							
				
				SET @ResultCode = 0
			END

			COMMIT TRANSACTION [APPROVE_PIN_REISSUE_TRAN]
			EXEC usp_get_pin_reissue @pin_reissue_id, @language_id, @audit_user_id, @audit_workstation
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [APPROVE_PIN_REISSUE_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_pin_reissue_complete]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_reissue_complete]
	-- Add the parameters for the stored procedure here
	@pin_reissue_id bigint,
	@notes varchar(100),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [COMPLETE_PIN_REISSUE_TRAN]
		BEGIN TRY 

		DECLARE @auth_pin_reissue bit = 0, 
				@pin_reissue_statuses_id int

		SELECT @pin_reissue_statuses_id = pin_reissue_statuses_id,
				@auth_pin_reissue = [issuer].authorise_pin_reissue_YN
		FROM [pin_reissue_status_current] 
				INNER JOIN [pin_reissue]
					ON [pin_reissue].pin_reissue_id = [pin_reissue_status_current].pin_reissue_id
				INNER JOIN [issuer]
					ON [issuer].issuer_id = [pin_reissue].issuer_id
		WHERE pin_reissue_status_current.pin_reissue_id = @pin_reissue_id

		IF(@auth_pin_reissue = 1 AND @pin_reissue_statuses_id != 1)
			BEGIN
				SET @ResultCode = 800
			END
		ELSE IF(@auth_pin_reissue = 0 AND @pin_reissue_statuses_id != 0)
			BEGIN
				SET @ResultCode = 800
			END
		ELSE
			BEGIN
				INSERT INTO [pin_reissue_status] (pin_reissue_id, pin_reissue_statuses_id, [user_id], audit_workstation, comments, status_date)
				VALUES (@pin_reissue_id, 3, @audit_user_id, @audit_workstation, @notes, GETDATE())

				UPDATE [pin_reissue]
				SET primary_index_number = null
				WHERE pin_reissue_id = @pin_reissue_id
			
				OPEN SYMMETRIC KEY Indigo_Symmetric_Key
				DECRYPTION BY CERTIFICATE Indigo_Certificate

				DECLARE @cardnumber varchar(50),
						@issuer_id int,
						@pin_reissue_statuses_name varchar(100),
						@audit_msg varchar(max)

				SELECT @cardnumber = CONVERT(VARCHAR(max),DECRYPTBYKEY(pan)) ,
						@issuer_id = issuer_id
				FROM [pin_reissue] 
				WHERE pin_reissue_id = @pin_reissue_id

				SELECT @pin_reissue_statuses_name = pin_reissue_statuses_id
				FROM [pin_reissue_statuses]
				WHERE pin_reissue_statuses_id = 3

				CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

				SET @audit_msg = '' + COALESCE(@pin_reissue_statuses_name, 'UNKNOWN') + 
										 ', ' + dbo.MaskString(@cardnumber, 6, 4)
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										10,
										NULL, 
										@audit_workstation, 
										@audit_msg,
										@issuer_id, NULL, NULL, NULL

				SET @ResultCode = 0
			END

			COMMIT TRANSACTION [COMPLETE_PIN_REISSUE_TRAN]
			EXEC usp_get_pin_reissue @pin_reissue_id, @language_id, @audit_user_id, @audit_workstation			

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [COMPLETE_PIN_REISSUE_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_pin_reissue_expired]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_reissue_expired]
	-- Add the parameters for the stored procedure here
	@pin_reissue_id bigint,
	@notes varchar(100),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [EXPIRE_PIN_REISSUE_TRAN]
		BEGIN TRY 

		IF((SELECT pin_reissue_statuses_id FROM [pin_reissue_status_current] 
			WHERE pin_reissue_status_current.pin_reissue_id = @pin_reissue_id) = 3)
			BEGIN
				SET @ResultCode = 800
			END
		ELSE
			BEGIN
				INSERT INTO [pin_reissue_status] (pin_reissue_id, pin_reissue_statuses_id, [user_id], audit_workstation, comments, status_date)
				VALUES (@pin_reissue_id, 4, @audit_user_id, @audit_workstation, @notes, GETDATE())

				UPDATE [pin_reissue]
				SET primary_index_number = null
				WHERE pin_reissue_id = @pin_reissue_id
				
				OPEN SYMMETRIC KEY Indigo_Symmetric_Key
				DECRYPTION BY CERTIFICATE Indigo_Certificate

				DECLARE @cardnumber varchar(50),
						@issuer_id int,
						@pin_reissue_statuses_name varchar(100),
						@audit_msg varchar(max)

				SELECT @cardnumber = CONVERT(VARCHAR(max),DECRYPTBYKEY(pan)) ,
						@issuer_id = issuer_id
				FROM [pin_reissue] 
				WHERE pin_reissue_id = @pin_reissue_id

				SELECT @pin_reissue_statuses_name = pin_reissue_statuses_id
				FROM [pin_reissue_statuses]
				WHERE pin_reissue_statuses_id = 4

				CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

				SET @audit_msg = '' + COALESCE(@pin_reissue_statuses_name, 'UNKNOWN') + 
										 ', ' + dbo.MaskString(@cardnumber, 6, 4)
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										10,
										NULL, 
										@audit_workstation, 
										@audit_msg,
										@issuer_id, NULL, NULL, NULL

				SET @ResultCode = 801
			END

			COMMIT TRANSACTION [EXPIRE_PIN_REISSUE_TRAN]
			EXEC usp_get_pin_reissue @pin_reissue_id, @language_id, @audit_user_id, @audit_workstation						

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [EXPIRE_PIN_REISSUE_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_pin_reissue_reject]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_reissue_reject]
	-- Add the parameters for the stored procedure here
	@pin_reissue_id bigint,
	@notes varchar(100),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [REJECT_PIN_REISSUE_TRAN]
		BEGIN TRY 

		IF((SELECT pin_reissue_statuses_id FROM [pin_reissue_status_current] 
			WHERE pin_reissue_status_current.pin_reissue_id = @pin_reissue_id) != 0)
			BEGIN
				SET @ResultCode = 800
			END
		ELSE
			BEGIN
				INSERT INTO [pin_reissue_status] (pin_reissue_id, pin_reissue_statuses_id, [user_id], audit_workstation, comments, status_date)
				VALUES (@pin_reissue_id, 2, @audit_user_id, @audit_workstation, @notes, GETDATE())

				UPDATE [pin_reissue]
				SET authorise_user_id = @audit_user_id,
					primary_index_number = null
				WHERE pin_reissue_id = @pin_reissue_id

				OPEN SYMMETRIC KEY Indigo_Symmetric_Key
				DECRYPTION BY CERTIFICATE Indigo_Certificate

				DECLARE @cardnumber varchar(50),
						@issuer_id int,
						@pin_reissue_statuses_name varchar(100),
						@audit_msg varchar(max)

				SELECT @cardnumber = CONVERT(VARCHAR(max),DECRYPTBYKEY(pan)) ,
						@issuer_id = issuer_id
				FROM [pin_reissue] 
				WHERE pin_reissue_id = @pin_reissue_id

				SELECT @pin_reissue_statuses_name = pin_reissue_statuses_id
				FROM [pin_reissue_statuses]
				WHERE pin_reissue_statuses_id = 2

				CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

				SET @audit_msg = '' + COALESCE(@pin_reissue_statuses_name, 'UNKNOWN') + 
										 ', ' + dbo.MaskString(@cardnumber, 6, 4)
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										10,
										NULL, 
										@audit_workstation, 
										@audit_msg,
										@issuer_id, NULL, NULL, NULL

				SET @ResultCode = 0
			END

			COMMIT TRANSACTION [REJECT_PIN_REISSUE_TRAN]
			EXEC usp_get_pin_reissue @pin_reissue_id, @language_id, @audit_user_id, @audit_workstation						

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [REJECT_PIN_REISSUE_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_prod_to_dist_batch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_prod_to_dist_batch] 
	-- Add the parameters for the stored procedure here
	@dist_batch_id bigint, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

	--BEGIN TRANSACTION [PROD_TO_DIST_TRAN]
	--BEGIN TRY 

		--Get a distinct list of branches from the batch
		DECLARE @branch_id int,
				@cards_total int = 0,
				@card_issue_method_id int,
				@check_prod_batch_id int,
				@new_dist_batch_id int,
				@audit_msg varchar(max)

		SELECT @card_issue_method_id = card_issue_method_id, @check_prod_batch_id = dist_batch_type_id
		FROM dist_batch
		WHERE dist_batch_id = @dist_batch_id

		IF (@check_prod_batch_id = 1)
			RAISERROR ('Can only create distribution batchs from a production batch.', 12, 12 );


		--Loop through all distinct branches for the production batch and create distribution batches
		DECLARE branchId_cursor CURSOR FOR 
			SELECT DISTINCT [cards].branch_id
			FROM [cards] 
				INNER JOIN [dist_batch_cards]
					ON [cards].card_id = [dist_batch_cards].card_id
			WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id
						

		OPEN branchId_cursor

		FETCH NEXT FROM branchId_cursor 
		INTO @branch_id

		WHILE @@FETCH_STATUS = 0
		BEGIN
			DECLARE @number_of_dist_cards int = 0,
					@dist_status_date datetime = GETDATE()

			--IF(SELECT [issuer].auto_create_dist_batch
			--	FROM [issuer] INNER JOIN [branch]
			--			ON [issuer].issuer_id = [branch].issuer_id
			--	WHERE [branch].branch_id = @branch_id) = 1
			--	BEGIN							
								
					--create the distribution batch
					INSERT INTO [dist_batch]
						([issuer_id], [branch_id], [no_cards],[date_created],[dist_batch_reference], [card_issue_method_id], [dist_batch_type_id])
					SELECT issuer_id, @branch_id, 0, @dist_status_date, @dist_status_date, @card_issue_method_id, 1
					FROM [branch]
					WHERE branch_id = @branch_id

					SET @new_dist_batch_id = SCOPE_IDENTITY();

					--Add cards to distribution batch
					INSERT INTO [dist_batch_cards]
						([dist_batch_id],[card_id],[dist_card_status_id])
					SELECT
						@new_dist_batch_id, [cards].card_id, 0
					FROM [cards] INNER JOIN [dist_batch_cards]
						ON [cards].card_id = [dist_batch_cards].card_id
					WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id
							AND [cards].branch_id = @branch_id
							
					--Get the number of cards inserted
					SELECT @number_of_dist_cards = @@ROWCOUNT										

					--add dist batch status of created
					INSERT INTO [dbo].[dist_batch_status]
						([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
					VALUES(@new_dist_batch_id, 0, @audit_user_id, @dist_status_date, 'Distribution Batch Create From Production: ' + CONVERT(VARCHAR(max),@dist_batch_id))

					--Generate dist batch reference
					DECLARE @dist_batch_ref varchar(50)
					SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
												[branch].branch_code + '' + 
												CONVERT(VARCHAR(8), @dist_status_date, 112) + '' +
												CAST(@new_dist_batch_id AS varchar(max))
					FROM [branch] INNER JOIN [issuer]
						ON [branch].issuer_id = [issuer].issuer_id
					WHERE [branch].branch_id = @branch_id

					--UPDATE dist batch with reference and number of cards
					UPDATE [dist_batch]
					SET [dist_batch_reference] = @dist_batch_ref,
						[no_cards] = @number_of_dist_cards
					WHERE [dist_batch].dist_batch_id = @new_dist_batch_id

					--UPDATE the production batch cards status to allocated							
					UPDATE [dist_batch_cards]
					SET [dist_batch_cards].dist_card_status_id = 0
					WHERE dist_batch_id = @dist_batch_id
						AND	[dist_batch_cards].card_id IN 
							(SELECT [dist_batch_cards].card_id
								FROM [dist_batch_cards]
								WHERE [dist_batch_cards].dist_batch_id = @new_dist_batch_id)
							

					DECLARE @dist_batch_status_name varchar(50)
					SELECT @dist_batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = 0

					--Add audit for dist batch creation								
					SET @audit_msg = 'Create: ' + CAST(@new_dist_batch_id AS varchar(max)) +
										', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					--TODO: look at the dist_batch_flow table
					--auto add approve the distbatch
					INSERT INTO [dbo].[dist_batch_status]
						([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
					VALUES(@new_dist_batch_id, 1, @audit_user_id, DATEADD(ss,1,@dist_status_date), 'Auto Dist Batch Create Approval')								

					SELECT @dist_batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = 1

					--Add audit for dist batch update								
					SET @audit_msg = 'Update: ' + CAST(@new_dist_batch_id AS varchar(max)) +
										', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

			--	END
			--ELSE
			--	BEGIN
			--		--Update the cards linked to the load batch and cursors branch with the available status.
			--		UPDATE [load_batch_cards]
			--		SET load_card_status_id = 1
			--		FROM [load_batch_cards] INNER JOIN [cards]
			--				ON [load_batch_cards].card_id = [cards].card_id
			--		WHERE [load_batch_cards].load_batch_id = @load_batch_id
			--				AND [cards].branch_id = @branch_id

			--		--Get the number of cards updated
			--		SELECT @number_of_dist_cards = @@ROWCOUNT
			--	END						

			SELECT @cards_total += @number_of_dist_cards

				-- Get the next branch.
			FETCH NEXT FROM branchId_cursor 
			INTO @branch_id
			END 
		CLOSE branchId_cursor;
		DEALLOCATE branchId_cursor;

		--Check that all cards for the load batch have been updated
		IF (SELECT COUNT(card_id) FROM [dist_batch_cards] WHERE dist_batch_id = @dist_batch_id) != @cards_total
		BEGIN
			RAISERROR ('Not all cards have been moved from production batch to distribution batch.',
						12,
						12 );
		END

	--	COMMIT TRANSACTION [PROD_TO_DIST_TRAN]				
	--	END TRY
	--BEGIN CATCH
	--	ROLLBACK TRANSACTION [PROD_TO_DIST_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_prod_to_pin]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
				@pin_status_date datetime = GETDATE(),
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




GO
/****** Object:  StoredProcedure [dbo].[usp_request_card_for_customer]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Create date: 
-- Description:	This proc is used to add details to the DB to request a card for a customer
-- =============================================
CREATE PROCEDURE [dbo].[usp_request_card_for_customer] 
	@branch_id int,
	@product_id int,		
	@card_priority_id int,
    @customer_account_number varchar(27),
	@domicile_branch_id int,
	@account_type_id int,
	@card_issue_reason_id int,
	@customer_first_name varchar(50),
	@customer_middle_name varchar(50),
	@customer_last_name varchar(50),
	@name_on_card varchar(30),
	@customer_title_id int,	
	@currency_id int,
	@resident_id int,
	@customer_type_id int,
	@cms_id varchar(50),
	@contract_number varchar(50),
	@idnumber varchar(50),
	@contact_number varchar(50),
	@customer_id varchar(50),
	@fee_waiver_YN bit = NULL,
	@fee_editable_YN bit = NULL,
	@fee_charged decimal(10,4) = NULL,
	@fee_overridden_YN bit = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@card_id bigint OUTPUT,
	@ResultCode int OUTPUT,
	@new_customer_account_id int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [REQUEST_CARD_FOR_CUST_TRAN]
		BEGIN TRY 

			IF @customer_middle_name IS NULL
				SET @customer_middle_name = ''
			
			DECLARE @status_date datetime,
					@branch_card_statuses_id int

			SET @branch_card_statuses_id = 2

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			DECLARE @objid int
			SET @objid = object_id('cards')
			SET @status_date = GETDATE()

			--Inserting a card record with an empty card number, the card number will be generated later in the process. 
			-- when that happens this record should be populated with a card number.
			INSERT INTO [cards]	([product_id],[branch_id],[origin_branch_id],[card_number],[card_sequence],[card_index], 
									card_issue_method_id, card_priority_id, fee_waiver_YN, fee_editable_YN, fee_charged, fee_overridden_YN) 
				VALUES(@product_id, @branch_id, @branch_id, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR, '')), 0,
					   [dbo].[MAC]('0', @objid), 0, @card_priority_id, @fee_waiver_YN, @fee_editable_YN, @fee_charged, @fee_overridden_YN)

			SET @card_id = SCOPE_IDENTITY();

			--Update card with reference number
			--Generate card reference
			DECLARE @card_ref varchar(100)
			SET @card_ref =  'CCR' + CONVERT(VARCHAR(8), GETDATE(), 112) + CAST(@product_id AS varchar(max)) + CAST(@card_id AS varchar(max))

			UPDATE [cards]
				SET card_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR(max), @card_ref)),
					card_request_reference = @card_ref
			WHERE [card_id] = @card_id


			--The initial card status.
			INSERT branch_card_status
					(card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id)
			VALUES (@card_id, @branch_card_statuses_id, @status_date, @audit_user_id, @audit_user_id)

			--Check if we need to do maker/checker for the request.
			--If no maker checker then we "Auto" approve the card for issue.
			IF ((SELECT [issuer].maker_checker_YN
				FROM [issuer] INNER JOIN [branch]
					ON [issuer].issuer_id = [branch].issuer_id
				WHERE [branch].branch_id = @branch_id) = 0)		
				BEGIN		
					SET @branch_card_statuses_id = 3       --Not MakerChecker	
					--Add additional second to the request so that the order is preserved,
					INSERT branch_card_status
						(card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id, comments)
					VALUES (@card_id, @branch_card_statuses_id, DATEADD(ss, 1, @status_date), @audit_user_id, @audit_user_id, 'Auto Approve Card For Issue')
				END	
						 

			--Save customer details
			INSERT customer_account
					([user_id], card_id, card_issue_reason_id, account_type_id, customer_account_number,
						customer_first_name, customer_middle_name, customer_last_name, name_on_card, customer_title_id, 
						date_issued, customer_type_id, currency_id, resident_id, cms_id, contract_number, Id_number,contact_number, CustomerId,
						domicile_branch_id)
			VALUES (@audit_user_id, @card_id, @card_issue_reason_id, @account_type_id, 
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_account_number)),
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_first_name)),
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_middle_name)), 
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_last_name)), 
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),UPPER(@name_on_card))), 
					@customer_title_id, @status_date, @customer_type_id, @currency_id, @resident_id, @cms_id, @contract_number,
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@idnumber)),
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@contact_number)),
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_id)),
					@domicile_branch_id)					

		

		SET @new_customer_account_id = SCOPE_IDENTITY()		

			
			--Log audit stuff
			DECLARE @branchcardstatus  varchar(max),
					@Scenario  varchar(max),
					@audit_msg varchar(max),
					@cardnumber varchar(16)

			SELECT  @branchcardstatus =  branch_card_statuses.branch_card_statuses_name
			FROM    branch_card_statuses 
			WHERE	branch_card_statuses.branch_card_statuses_id = @branch_card_statuses_id

			SELECT  @Scenario =  card_issue_reason.[card_issuer_reason_name]
			FROM	card_issue_reason 
			WHERE	card_issue_reason.[card_issue_reason_id] = @card_issue_reason_id

			SET @audit_msg =  'card request-' + 
								COALESCE(@branchcardstatus, 'UNKNWON') +  
								', cust id:' + COALESCE(CAST(@cms_id as varchar(max)), 'n/a') +
								', a/c:' + dbo.MaskString(@customer_account_number, 3, 4) + 
								', ' + COALESCE(@Scenario, 'UNKNWON')

			--log the audit record		
			EXEC usp_insert_audit @audit_user_id, 
									3,---IssueCard
									NULL, 
									@audit_workstation, 
									@audit_msg, 
									NULL, NULL, NULL, NULL

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key			
				
			COMMIT TRANSACTION [REQUEST_CARD_FOR_CUST_TRAN]
			SET @ResultCode = 0

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [REQUEST_CARD_FOR_CUST_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_request_card_stock]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Nduvho Mukhavhuli
-- Create date: 2014/09/25
-- Description:	Creates a new card order
-- =============================================
CREATE PROCEDURE [dbo].[usp_request_card_stock] 
	@issuer_id int,
	@branch_id int,
	@product_id int,	
	@card_priority_id int,
	@card_issue_method_id int,	
	@cards_in_batch int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@dist_batch_id bigint OUTPUT,
	@dist_batch_ref varchar(50) OUTPUT,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [CREATE_CARDS_STOCK]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			DECLARE @status_date datetime
			DECLARE @card_id bigint,
					@newGuid uniqueidentifier

			DECLARE @objid int
			SET @objid = object_id('cards')
			SET @status_date = GETDATE()


			--create the production batch
			INSERT INTO [dist_batch]
				([card_issue_method_id],[issuer_id], [branch_id], [no_cards],[date_created],[dist_batch_reference],[dist_batch_type_id])
			VALUES (@card_issue_method_id, @issuer_id, @branch_id, @cards_in_batch, @status_date, @status_date, 0)

			SET @dist_batch_id = SCOPE_IDENTITY();

			--add prod batch status of created
			INSERT INTO [dbo].[dist_batch_status]
				([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
			VALUES(@dist_batch_id, 0, @audit_user_id, @status_date, 'Batch Created')

			--Generate prod batch reference
			SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
										CONVERT(VARCHAR(MAX),[issuer_product].product_id) + '' +										  
										CONVERT(VARCHAR(8), @status_date, 112) + '' +
										CAST(@dist_batch_id AS varchar(max))
			FROM [issuer]					
				INNER JOIN [issuer_product]
					ON [issuer_product].issuer_id = [issuer].issuer_id
			WHERE [issuer_product].product_id = @product_id

			--UPDATE prod batch with reference and number of cards
			UPDATE [dist_batch]
			SET [dist_batch_reference] = @dist_batch_ref,
				[no_cards] = @cards_in_batch
			WHERE [dist_batch].dist_batch_id = @dist_batch_id

			--This section helps with creating the card_index, instead of calling the fuction each time
			--Which slows down the insers, we get the key and then just encrypt
			SET @objid = object_id('cards')			
			DECLARE @key varbinary(100)
			SET @key = null
			SELECT @key = DecryptByKeyAutoCert(cert_id('cert_ProtectIndexingKeys'), null, mac_key) 
			FROM mac_index_keys 
			WHERE table_id = @objid

			IF(@key IS NULL)
				RAISERROR (N'MAC Index Key is null.', 10, 1);

			--Table for storing new card id's
			Declare @inserted_cards TABLE (	card_id bigint )

			--Create Cards for the Batch
			DECLARE @index int = 0			
			WHILE @index < @cards_in_batch
			BEGIN
				SET @newGuid = NEWID();

				--Inserting a card record with an empty card number, the card number will be generated later in the process. 
				-- when that happens this record should be populated with a card number.
				INSERT INTO [cards]	([product_id], [branch_id],[origin_branch_id],[card_number],[card_sequence],
										[card_issue_method_id], [card_priority_id], [card_request_reference], [card_index]) 
					OUTPUT Inserted.card_id INTO @inserted_cards
					VALUES(@product_id, @branch_id, @branch_id, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR(max), @newGuid)), 0,
						   --[dbo].[MAC]('0', @objid), 
						   @card_issue_method_id, @card_priority_id
						   ,CONVERT(varchar(100), @newGuid)
						   ,CONVERT(varbinary(24),HashBytes( N'SHA1', CONVERT(varbinary(8000), CONVERT(nvarchar(4000), @newGuid)) + @key )))

				--SET @card_id = SCOPE_IDENTITY();

				SET @index = @index +1
			END			

			--UPDATE CARDS WITH UNIQUE REF
			UPDATE [cards]
				SET card_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR, dbo.GenCardReferenceNo(@status_date, [cards].card_id))),
					card_request_reference = dbo.GenCardReferenceNo(@status_date, [cards].card_id),
					card_index = CONVERT(varbinary(24),HashBytes( N'SHA1', CONVERT(varbinary(8000), CONVERT(nvarchar(4000), RIGHT(dbo.GenCardReferenceNo(@status_date, [cards].card_id),4))) + @key ))
			WHERE [card_id] IN (SELECT card_id FROM @inserted_cards)

			--LINK CARDS TO THE BATCH
			INSERT INTO [dist_batch_cards] (card_id, dist_batch_id, dist_card_status_id)
				SELECT card_id, @dist_batch_id, 12 FROM @inserted_cards

			--UPDATE CARDS WITH UNIQUE REF
			--UPDATE [cards]
			--		SET [cards].card_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),
			--									CONVERT(VARCHAR, dbo.GenCardReferenceNo(@status_date, [cards].card_id))),
			--			[cards].card_request_reference = dbo.GenCardReferenceNo(@status_date, [cards].card_id)
			--FROM [cards]
			--		INNER JOIN [dist_batch_cards]
			--			ON [cards].card_id = [dist_batch_cards].card_id
			--WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id


			--Add audit for dist batch creation	
			DECLARE @dist_batch_status_name varchar(50),
					@audit_msg nvarchar(500)
			SELECT @dist_batch_status_name =  dist_batch_status_name
			FROM dist_batch_statuses
			WHERE dist_batch_statuses_id = 0
											
			SET @audit_msg = 'Create: ' + CAST(@dist_batch_id AS varchar(max)) +
								', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
								', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
			--log the audit record		
			EXEC usp_insert_audit @audit_user_id, 
									2,
									NULL, 
									@audit_workstation, 
									@audit_msg, 
									NULL, NULL, NULL, NULL

			SET @ResultCode = 0


			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

			COMMIT TRANSACTION [CREATE_CARDS_STOCK]			
		END TRY

		BEGIN CATCH
			ROLLBACK TRANSACTION [CREATE_CARDS_STOCK]
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
/****** Object:  StoredProcedure [dbo].[usp_request_create_dist_batch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_request_create_dist_batch] 
	@card_issue_method_id int,
	@issuer_id int,
	@branch_id int = null,
	@product_id int,
	@card_priority_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@cards_in_batch int OUTPUT,
	@dist_batch_id int OUTPUT,
	@dist_batch_ref varchar(50) OUTPUT,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [CREATE_DIST_BATCH]
		BEGIN TRY 

		SET @cards_in_batch = 0
		SET	@dist_batch_id = 0
		SET @dist_batch_ref = ''

		DECLARE @branch_card_statuses_id int
		SET @branch_card_statuses_id = 3

		--RAB: Card should always be in approved for issue state. Card Request will do an "Auto Approval" for maker/checker
		--See if the issuer of the branch requires MakerChecker, set branch card statis accordingly.
		--IF((SELECT TOP 1 [issuer].maker_checker_YN
		 
		--    FROM [issuer] INNER JOIN [branch] ON [branch].issuer_id = [issuer].issuer_id 
		--    WHERE [branch].branch_id = @branch_id) = 1)
		--	BEGIN
		--		SET @branch_card_statuses_id = 3
		--	END
		--ELSE
		--	BEGIN
		--		SET @branch_card_statuses_id = 2
		--	END


		--Only create a batch if there are cards for the batch
		IF( (SELECT COUNT(*) 
			 FROM branch_card_status_current
					INNER JOIN branch
						ON branch_card_status_current.branch_id = branch.branch_id					
			 WHERE branch_card_statuses_id = @branch_card_statuses_id
					AND product_id = @product_id
					AND card_issue_method_id = @card_issue_method_id
					AND card_priority_id = @card_priority_id
					AND branch_card_status_current.branch_id = COALESCE(@branch_id, branch_card_status_current.branch_id)
					AND issuer_id = @issuer_id) = 0)
		BEGIN
			SET @ResultCode = 400
			COMMIT TRANSACTION [CREATE_DIST_BATCH]
		END			
		ELSE
			BEGIN

				DECLARE @cards_total int = 0,
						@batch_branch_id int,
						@audit_msg nvarchar(500)


				
				--SELECT TOP 1 @batch_branch_id = branch_card_status_current.branch_id
				--FROM branch_card_status_current
				--INNER JOIN branch
				--	ON branch_card_status_current.branch_id = branch.branch_id					
				--	WHERE branch_card_statuses_id = @branch_card_statuses_id
				--		AND product_id = @product_id
				--		AND card_issue_method_id = @card_issue_method_id
				--		AND card_priority_id = @card_priority_id
				--		AND branch_card_status_current.branch_id = COALESCE(@branch_id, branch_card_status_current.branch_id)
				--		AND issuer_id = @issuer_id


				--create the production batch
				INSERT INTO [dist_batch]
					([card_issue_method_id],[issuer_id],[branch_id], [no_cards],[date_created],[dist_batch_reference],[dist_batch_type_id])
				VALUES (@card_issue_method_id, @issuer_id, @branch_id, 0, GETDATE(), GETDATE(),0)

				SET @dist_batch_id = SCOPE_IDENTITY();

				--Add cards to production batch
				INSERT INTO [dist_batch_cards]
					([dist_batch_id],[card_id],[dist_card_status_id])
				SELECT @dist_batch_id, card_id, 12
				FROM branch_card_status_current
						INNER JOIN branch
							ON branch_card_status_current.branch_id = branch.branch_id	
				WHERE branch_card_statuses_id = @branch_card_statuses_id 
					AND product_id = @product_id
					AND card_issue_method_id = @card_issue_method_id
					AND card_priority_id = @card_priority_id
					AND branch_card_status_current.branch_id = COALESCE(@branch_id, branch_card_status_current.branch_id)
					AND issuer_id = @issuer_id


				--add prod batch status of created
				INSERT INTO [dbo].[dist_batch_status]
					([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
				VALUES(@dist_batch_id, 0, @audit_user_id, GETDATE(), 'Batch Created')

				--Generate dist batch reference
				SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
										  CONVERT(VARCHAR(MAX),[issuer_product].product_id) + '' +										  
										  CONVERT(VARCHAR(8), GETDATE(), 112) + '' +
										  CAST(@dist_batch_id AS varchar(max))
				FROM [issuer]					
					INNER JOIN [issuer_product]
						ON [issuer_product].issuer_id = [issuer].issuer_id
				WHERE [issuer].issuer_id = @issuer_id

				SELECT @cards_in_batch = COUNT(*)
				FROM dist_batch_cards
				WHERE dist_batch_id = @dist_batch_id 

				--UPDATE prod batch with reference and number of cards
				UPDATE [dist_batch]
				SET [dist_batch_reference] = @dist_batch_ref,
					[no_cards] = @cards_in_batch
				WHERE [dist_batch].dist_batch_id = @dist_batch_id


				--UPDATE branch card status for those cards that have been added to the new dist batch.
				INSERT INTO [branch_card_status]
					(branch_card_statuses_id, card_id, comments, status_date, [user_id])
				SELECT 10, card_id, 'Assigned to batch', GETDATE(), @audit_user_id
				FROM dist_batch_cards
				WHERE dist_batch_id = @dist_batch_id	

				--Add audit for dist batch creation	
				DECLARE @dist_batch_status_name varchar(50)
				SELECT @dist_batch_status_name =  dist_batch_status_name
				FROM dist_batch_statuses
				WHERE dist_batch_statuses_id = 0
											
				SET @audit_msg = 'Create: ' + CAST(@dist_batch_id AS varchar(max)) +
									', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
									', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										2,
										NULL, 
										@audit_workstation, 
										@audit_msg, 
										NULL, NULL, NULL, NULL

				SELECT @ResultCode = 0
				COMMIT TRANSACTION [CREATE_DIST_BATCH]	

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
/****** Object:  StoredProcedure [dbo].[usp_request_pin_mailer_reprints]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_request_pin_mailer_reprints] 
	@card_issue_method_id int,
	@issuer_id int,
	@branch_id int = null,
	@product_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@cards_in_batch int OUTPUT,
	@pin_batch_id int OUTPUT,
	@pin_batch_ref varchar(100) OUTPUT,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [TRAN_CREATE_PIN_REPRINT_BATCH]
		BEGIN TRY 

		SET @cards_in_batch = 0
		SET	@pin_batch_id = 0
		SET @pin_batch_ref = ''

		DECLARE @pin_mailer_reprint_status_id int
		SET @pin_mailer_reprint_status_id = 1

		--Only create a batch if there are cards for the batch
		IF( (SELECT COUNT(*) 
			 FROM [pin_mailer_reprint_status_current]
					INNER JOIN [branch]
						ON [pin_mailer_reprint_status_current].branch_id = [branch].branch_id					
			 WHERE [pin_mailer_reprint_status_current].pin_mailer_reprint_status_id = @pin_mailer_reprint_status_id
					AND product_id = @product_id
					AND card_issue_method_id = @card_issue_method_id
					AND [pin_mailer_reprint_status_current].branch_id = COALESCE(@branch_id, [pin_mailer_reprint_status_current].branch_id)
					AND issuer_id = @issuer_id) = 0)
		BEGIN
			SET @ResultCode = 400
			COMMIT TRANSACTION [TRAN_CREATE_PIN_REPRINT_BATCH]
		END			
		ELSE
			BEGIN

				DECLARE @cards_total int = 0,
						@batch_branch_id int,
						@audit_msg nvarchar(500)

				--create the reprint batch
				INSERT INTO [pin_batch]
					([card_issue_method_id],[issuer_id],[branch_id], [no_cards],[date_created],[pin_batch_reference],[pin_batch_type_id])
				VALUES (@card_issue_method_id, @issuer_id, @branch_id, 0, GETDATE(), GETDATE(), 2)

				SET @pin_batch_id = SCOPE_IDENTITY();

				--Add cards to production batch
				INSERT INTO [pin_batch_cards]
					([pin_batch_id],[card_id],[pin_batch_cards_statuses_id])
				SELECT @pin_batch_id, card_id, 0
				FROM [pin_mailer_reprint_status_current]
					INNER JOIN [branch]
						ON [pin_mailer_reprint_status_current].branch_id = [branch].branch_id					
				WHERE [pin_mailer_reprint_status_current].pin_mailer_reprint_status_id = @pin_mailer_reprint_status_id
					AND product_id = @product_id
					AND card_issue_method_id = @card_issue_method_id
					AND [pin_mailer_reprint_status_current].branch_id = COALESCE(@branch_id, [pin_mailer_reprint_status_current].branch_id)
					AND issuer_id = @issuer_id


				--add reprint batch status of created
				INSERT INTO [dbo].[pin_batch_status]
					([pin_batch_id],[pin_batch_statuses_id],[user_id],[status_date],[status_notes])
				VALUES(@pin_batch_id, 0, @audit_user_id, GETDATE(), 'Pin Mailer Reprint Batch Created')

				--Generate dist batch reference
				SELECT @pin_batch_ref =  [issuer].issuer_code + '' + 
										  CONVERT(VARCHAR(MAX),[issuer_product].product_id) + '' +										  
										  CONVERT(VARCHAR(8), GETDATE(), 112) + '' +
										  CAST(@pin_batch_id AS varchar(max))
				FROM [issuer]					
					INNER JOIN [issuer_product]
						ON [issuer_product].issuer_id = [issuer].issuer_id
				WHERE [issuer].issuer_id = @issuer_id

				SELECT @cards_in_batch = COUNT(*)
				FROM pin_batch_cards
				WHERE pin_batch_id = @pin_batch_id 

				--UPDATE reprint batch with reference and number of cards
				UPDATE [pin_batch]
				SET [pin_batch_reference] = @pin_batch_ref,
					[no_cards] = @cards_in_batch
				WHERE [pin_batch].pin_batch_id = @pin_batch_id


				--UPDATE pin reprint status for those cards that have been added to the new reprint batch.
				INSERT INTO [pin_mailer_reprint]
					(pin_mailer_reprint_status_id, card_id, comments, status_date, [user_id])
				SELECT 2, card_id, 'Assigned to batch', GETDATE(), @audit_user_id
				FROM pin_batch_cards
				WHERE pin_batch_id = @pin_batch_id	

				--Add audit for pin batch creation	
				DECLARE @pin_batch_status_name varchar(50)
				SELECT @pin_batch_status_name = pin_batch_statuses.pin_batch_statuses_name
				FROM pin_batch_statuses
				WHERE pin_batch_statuses_id = 0
											
				SET @audit_msg = 'Create: ' + CAST(@pin_batch_id AS varchar(max)) +
									', ' + COALESCE(@pin_batch_ref, 'UNKNOWN') +
									', ' + COALESCE(@pin_batch_status_name, 'UNKNOWN')
								   
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										2,
										NULL, 
										@audit_workstation, 
										@audit_msg, 
										NULL, NULL, NULL, NULL

				SELECT @ResultCode = 0
				COMMIT TRANSACTION [TRAN_CREATE_PIN_REPRINT_BATCH]	

			END					
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [TRAN_CREATE_PIN_REPRINT_BATCH]
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
/****** Object:  StoredProcedure [dbo].[usp_request_pin_reissue]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_request_pin_reissue] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@branch_id int,
	@product_id int,
	@pan varchar(19), 
	@primary_index_number varbinary(max),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [REQUEST_PIN_REISSUE_TRAN]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			DECLARE @pin_reissue_id bigint = -1

			--Check there are no outstanding requests for this card
			IF((SELECT COUNT(*) FROM [pin_reissue_status_current] INNER JOIN [pin_reissue] 
					ON [pin_reissue_status_current].pin_reissue_id = [pin_reissue].pin_reissue_id
				WHERE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([pin_reissue].pan)) = @pan
						AND [pin_reissue_status_current].pin_reissue_statuses_id IN (0, 1)) > 0)
				BEGIN
					SET @ResultCode = 802
				END
			ELSE
				BEGIN
					DECLARE @OutputTbl TABLE (pin_reissue_id bigint)
					DECLARE @request_date datetime = GETDATE()

					INSERT INTO [pin_reissue] (issuer_id, branch_id, operator_user_id, pan, primary_index_number, product_id, reissue_date, failed, notes, request_expiry)
					OUTPUT INSERTED.pin_reissue_id INTO @OutputTbl(pin_reissue_id)
					VALUES (@issuer_id, @branch_id, @audit_user_id, 
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@pan)),
							@primary_index_number, @product_id, @request_date, 0, '', DATEADD(mi, 60, @request_date))

					INSERT INTO [pin_reissue_status] (pin_reissue_id, pin_reissue_statuses_id, [user_id], audit_workstation, comments, status_date)
					SELECT pin_reissue_id, 0, @audit_user_id, @audit_workstation, 'PIN reissue requested', @request_date
					FROM @OutputTbl


					DECLARE @pin_reissue_statuses_name varchar(100),
							@audit_msg varchar(max)
							

					SELECT @pin_reissue_id = pin_reissue_id
					FROM @OutputTbl

					SELECT @pin_reissue_statuses_name = pin_reissue_statuses_id
					FROM [pin_reissue_statuses]
					WHERE pin_reissue_statuses_id = 0

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

					SET @audit_msg = '' + COALESCE(@pin_reissue_statuses_name, 'UNKNOWN') + 
											 ', ' + dbo.MaskString(@pan, 6, 4)
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											10,
											NULL, 
											@audit_workstation, 
											@audit_msg,
											@issuer_id, NULL, NULL, NULL

						
					
					SET @ResultCode = 0
				END
				EXEC usp_get_pin_reissue @pin_reissue_id, @language_id, @audit_user_id, @audit_workstation
			COMMIT TRANSACTION [REQUEST_PIN_REISSUE_TRAN]		
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [REQUEST_PIN_REISSUE_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_reset_user_password]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 26 March 2014
-- Description:	Resets a users password
-- =============================================
CREATE PROCEDURE [dbo].[usp_reset_user_password] 
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
				--DELETE FROM [user_password_history]
				--WHERE [date_changed] < DATEADD(MONTH, -3, GETDATE())

			DECLARE @PreviousPasswordsCount int
			DECLARE @query nvarchar(max)
			DECLARE @subquery nvarchar(max)

			
			SELECT @PreviousPasswordsCount = PreviousPasswordsCount
			FROM [user_admin]
			--- select all other records other than @PreviousPasswordsCount
			set @subquery=' select  user_id from  user_password_history where  user_id not in (select top '+ cast( @PreviousPasswordsCount as varchar(10))+' user_id from [user_password_history] where [user_id] = '+cast( @user_id	as varchar(10))+' order by [date_changed] asc)'
			
			set @query=' DELETE FROM [user_password_history] where user_id  in ('+@subquery+')'
			
			exec (@query)
				
				--Move the current password into the history table
				INSERT INTO [user_password_history] ([user_id], [password_history], [date_changed])
				SELECT [user].[user_id], [user].[password], GETDATE()
				FROM [user]
				WHERE [user].[user_id] = @user_id				
				
				--Update current password with new password
				UPDATE [user]
				SET	[password] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@password)),
					[last_password_changed_date] = GETDATE(),
					[number_of_incorrect_logins] = 0
				WHERE [user].[user_id] = @user_id
				

				--log the audit record
				DECLARE @audit_description varchar(max),
				        @username varchar(100)

				SELECT  @username = CONVERT(VARCHAR(max),DECRYPTBYKEY([username]))
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

				
				EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_search_branch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 6 March 2014
-- Description:	Search for branches based on inputs
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_branch] 
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
/****** Object:  StoredProcedure [dbo].[usp_search_branch_cards]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch cards at a branch based on search criterial
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_branch_cards]
	@issuer_id int = NULL, 
	@branch_id int = NULL,
	@user_role_id int = NULL,
	@product_id int = NULL,
	--@sub_product_id int = NULL,
	@priority_id int = NULL,
	@card_issue_method_id int = null,
	@card_number varchar(20) = NULL,
	@branch_card_statuses_id int = NULL,
	@operator_user_id bigint = NULL,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@language_id int = 0,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [BRANCH_CARD_SEARCH_TRAN]
		BEGIN TRY 

			DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			--THIS IS FOR QUICKER CARD LOOKUP
			DECLARE @objid int,
					@card_index varbinary(max)
			SET @objid = object_id('cards')

			IF LEN(@card_number) = 4
				BEGIN
					SET @card_index = [dbo].[MAC] (@card_number, @objid)
					SET @card_number = NULL
				END

			DECLARE @StartRow INT, @EndRow INT;			
			
			SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
			SET @EndRow = @StartRow + @RowsPerPage - 1;


			WITH PAGE_ROWS
			AS
			(
			SELECT ROW_NUMBER() OVER(ORDER BY status_date DESC) AS ROW_NO
					, COUNT(*) OVER() AS TOTAL_ROWS			
					, *
			FROM( 

				SELECT [cards].card_id,  
					   CASE 
							WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
							ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
						END as card_number,
					   [cards].[card_request_reference],
					   [cards].product_id, 
					   --[cards].sub_product_id, 
					   [cards].card_issue_method_id, 
					   [cards].card_priority_id, 
					   [branch_card_status_current].branch_card_statuses_id, 
					   [branch_card_statuses_language].language_text as current_card_status,
					   [branch_card_status_current].operator_user_id, 
					   [branch_card_status_current].status_date, 
					   CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].username)) AS operator_username, 
					   [issuer_product].product_bin_code,
					   [issuer_product].sub_product_code,
					   [user_branch_access].issuer_id, issuer_name, branch_name, issuer_code, branch_code, [cards].branch_id,
					   [branch_card_status_current].comments
				FROM [cards]
					--Filter out cards linked to branches the user doesnt have access to.
					INNER JOIN (SELECT DISTINCT [user_roles_branch].branch_id, [branch].branch_code, [branch].branch_name, [branch].issuer_id
												,[issuer].issuer_name, [issuer].issuer_code, card_ref_preference							
								FROM [user_roles_branch] 
									INNER JOIN [user_roles]
										ON [user_roles_branch].user_role_id = [user_roles].user_role_id											
									INNER JOIN [branch]
										ON [user_roles_branch].branch_id = [branch].branch_id	
											AND [branch].branch_status_id = 0
									INNER JOIN [issuer]
										ON [branch].issuer_id = [issuer].issuer_id
											AND [issuer].issuer_status_id = 0
								WHERE [user_roles_branch].[user_id] = @audit_user_id		
										AND [user_roles_branch].issuer_id = COALESCE(@issuer_id, [user_roles_branch].issuer_id)								
										AND [user_roles_branch].branch_id = COALESCE(@branch_id, [user_roles_branch].branch_id)										
										AND [user_roles_branch].user_role_id = COALESCE(@user_role_id, [user_roles_branch].user_role_id)
										AND [user_roles].user_role_id IN (1,2,3,4,5,7)--Only want roles that allowed to search cards
								) as [user_branch_access]
						ON [cards].branch_id = [user_branch_access].branch_id
					INNER JOIN [issuer_product]
						ON [cards].product_id = [issuer_product].product_id
					INNER JOIN [branch_card_status_current]
						ON [cards].card_id = [branch_card_status_current].card_id
					INNER JOIN [branch_card_statuses_language]
							ON [branch_card_statuses_language].branch_card_statuses_id = [branch_card_status_current].branch_card_statuses_id
								AND [branch_card_statuses_language].language_id = @language_id
					--INNER JOIN [branch]
					--	ON [branch].branch_id = [cards].branch_id
					LEFT OUTER JOIN [user]
						ON [branch_card_status_current].operator_user_id = [user].[user_id]
				WHERE [cards].product_id = COALESCE(@product_id, [cards].product_id) AND
					 -- (([issuer_product].sub_product_id_length > 0 AND [cards].sub_product_id = COALESCE(@sub_product_id, [cards].sub_product_id))
						--OR ([issuer_product].sub_product_id_length = 0)) AND
					  [cards].card_priority_id = COALESCE(@priority_id, [cards].card_priority_id) AND
					  [cards].card_issue_method_id = COALESCE(@card_issue_method_id, [cards].card_issue_method_id) AND
					  ((@card_number IS NULL) OR (DECRYPTBYKEY([cards].card_number) LIKE @card_number))	AND
					  [branch_card_status_current].branch_card_statuses_id = COALESCE(@branch_card_statuses_id, [branch_card_status_current].branch_card_statuses_id) AND
					  ISNULL([branch_card_status_current].operator_user_id, -999) = COALESCE(@operator_user_id, [branch_card_status_current].operator_user_id, -999) 				  
					  AND ((@card_index IS NULL) OR ([cards].[card_index] = @card_index))
			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY status_date DESC
			
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

			--log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_search_cards]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 28 March 2014
-- Description:	Search for card/s based on input parameters
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_cards] 
	-- Add the parameters for the stored procedure here
	@user_id bigint,
	@user_role_id int = NULL,
	@issuer_id int = NULL,
	@branch_id int = NULL,
	@card_number varchar(30) = NULL, 
	@card_last_four_digits varchar(4) = NULL,	
	@card_refnumber varchar(50) =NULL,
	@batch_reference varchar(100) = NULL,
	@load_card_status_id int = NULL,
	@dist_card_status_id int = NULL,	
	@branch_card_statuses_id int = NULL,
	@dist_batch_id bigint = NULL,
	@pin_batch_id bigint=NULL,

	@account_number varchar(100) = NULL,
	@first_name varchar(100) = NULL,
	@last_name varchar(100) = NULL,
	@cms_id varchar(100) = NULL,	

	@date_from DATETIME = NULL,
	@date_to DATETIME = NULL,

	@card_issue_method_id int = NULL,
	@product_id int = NULL,
	--@sub_product_id int = NULL,
	@priority_id int = NULL,

	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [CARD_SEARCH_TRAN]
		BEGIN TRY 

			DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			IF @date_to IS NOT NULL
				SET @date_to = DATEADD(day, 1, @date_to)

			--THIS IS FOR QUICKER CARD LOOKUP
			DECLARE @objid int,
					@card_index varbinary(max)
			SET @objid = object_id('cards')

			IF (@card_last_four_digits IS NOT NULL)
				SET @card_index =  [dbo].[MAC] (@card_last_four_digits, @objid)
			
			DECLARE @StartRow INT, @EndRow INT;			
			
			SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
			SET @EndRow = @StartRow + @RowsPerPage - 1;

			--Table variable for branches the user has access to, filtered out by branch and issuer
			DECLARE @branches_user TABLE (branch_id int, branch_code varchar(100), branch_name varchar(100),
					issuer_id int, issuer_name varchar(100), issuer_code varchar(100), card_ref_preference bit)

			INSERT INTO @branches_user (branch_id, branch_code, branch_name, issuer_id, issuer_name, issuer_code, card_ref_preference)
			SELECT DISTINCT [user_roles_branch].branch_id, [branch].branch_code, [branch].branch_name, [branch].issuer_id
														,[issuer].issuer_name, [issuer].issuer_code, card_ref_preference							
										FROM [user_roles_branch] 
											INNER JOIN [user_roles]
												ON [user_roles_branch].user_role_id = [user_roles].user_role_id											
											INNER JOIN [branch]
												ON [user_roles_branch].branch_id = [branch].branch_id	
													AND [branch].branch_status_id = 0
											INNER JOIN [issuer]
												ON [branch].issuer_id = [issuer].issuer_id
													AND [issuer].issuer_status_id = 0
										WHERE [user_roles_branch].[user_id] = @user_id		
												AND [user_roles_branch].issuer_id = COALESCE(@issuer_id, [user_roles_branch].issuer_id)								
												AND [user_roles_branch].branch_id = COALESCE(@branch_id, [user_roles_branch].branch_id)										
												AND [user_roles_branch].user_role_id = COALESCE(@user_role_id, [user_roles_branch].user_role_id)
												AND [user_roles].user_role_id IN (1,2,3,4,5,7,11,12,13,14,15)--Only want roles that allowed to search cards

			IF OBJECT_ID('tempdb..#Cards') IS NOT NULL
				DROP TABLE #Cards

			--Temp table to store all the cards that match the filters
			CREATE TABLE #Cards(card_id BIGINT PRIMARY KEY
							   ,product_id int, sub_product_id int null
							   ,card_priority_id int, card_issue_method_id int
							   ,branch_id int, branch_code varchar(100), branch_name varchar(100)
							   ,issuer_id int, issuer_code varchar(100), issuer_name varchar(100)					   
							   ,card_number varchar(100), card_request_reference varchar(100))

			INSERT INTO #Cards (card_id, product_id, card_priority_id, card_issue_method_id, card_number, 
								card_request_reference, branch_id, branch_code, branch_name, issuer_id, issuer_code, issuer_name)
			SELECT 
				card_id
				, product_id
				--, sub_product_id
				, card_priority_id
				, card_issue_method_id
				, CASE 
						WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
						ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
					END AS 'card_number'
				, [cards].card_request_reference 
				, [cards].branch_id
				, branch_code
				, branch_name
				, [branches_for_user].issuer_id
				, [branches_for_user].issuer_code
				, [branches_for_user].issuer_name
			FROM [cards]
					INNER JOIN @branches_user as [branches_for_user]
						ON [cards].branch_id = [branches_for_user].branch_id
					INNER JOIN [issuer] ON issuer.issuer_id = [branches_for_user].issuer_id
			WHERE 
							((@card_number IS NULL) OR (DECRYPTBYKEY([cards].card_number) LIKE @card_number))
							AND ((@card_issue_method_id IS NULL) OR ([cards].card_issue_method_id = @card_issue_method_id))
							AND ((@card_last_four_digits IS NULL) OR ([cards].[card_index] = @card_index))
							AND ((@product_id IS NULL) OR ([cards].product_id = @product_id))
							--AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
							AND ((@priority_id IS NULL) OR ([cards].card_priority_id  = @priority_id))
							AND ((@card_refnumber IS NULL) OR ([cards].card_request_reference = @card_refnumber));


			--append#1
			WITH PAGE_ROWS
			AS
			(
			SELECT ROW_NUMBER() OVER(ORDER BY status_date DESC) AS ROW_NO
					, COUNT(*) OVER() AS TOTAL_ROWS			
					, *
			FROM( 
					SELECT DISTINCT TOP 1000
					     [cards_temp].card_id
					   , [cards_temp].card_number
					   , [cards_temp].card_request_reference
					   , [cards_temp].product_id
					   , [cards_temp].sub_product_id
					   , [cards_temp].card_issue_method_id
					   , [cards_temp].card_priority_id
					   , ISNULL([branch_card_status_current].branch_card_statuses_id, 0) as branch_card_statuses_id 
					   , COALESCE([branch_card_statuses_language].language_text, 
						 	     [dist_card_statuses_language].language_text,
						 	     [load_card_statuses_language].language_text) as current_card_status
					   , null as operator_user_id 
					   , COALESCE([branch_card_status_current].status_date, 
						 		 [dist_batch_status_card_current].status_date,
						 		 [load_batch_status_card_current].status_date) as status_date
					   , null as operator_username
					   , '' as product_bin_code
					   , '' AS sub_product_code
					   , [cards_temp].issuer_id
					   , issuer_code
					   , issuer_name
					   , branch_name	  
					   , branch_code	
					   , [cards_temp].branch_id	
					   , COALESCE([branch_card_status_current].comments,
						 		 [dist_batch_status_card_current].status_notes,
						 		 [load_batch_status_card_current].status_notes,
						 		 '') as comments
				FROM #Cards as [cards_temp]
					--Load Batch Joins
					LEFT OUTER JOIN [load_batch_status_card_current]
						ON [cards_temp].card_id = [load_batch_status_card_current].card_id
						 
					LEFT OUTER JOIN [load_batch]
						ON [load_batch].load_batch_id = [load_batch_status_card_current].load_batch_id
					LEFT OUTER JOIN [load_card_statuses_language]
						ON [load_card_statuses_language].load_card_status_id = [load_batch_status_card_current].load_card_status_id
							AND [load_card_statuses_language].language_id = 0

					--Dist Batch Joins
					LEFT OUTER JOIN  [dist_batch_status_card_current]
						ON [cards_temp].card_id = [dist_batch_status_card_current].card_id
							--AND [dist_batch_status_card_current].dist_card_status_id = 0
					LEFT OUTER JOIN [dist_batch]
						ON [dist_batch].dist_batch_id = [dist_batch_status_card_current].dist_batch_id
					LEFT OUTER JOIN [dist_card_statuses_language]
						ON [dist_card_statuses_language].dist_card_status_id = [dist_batch_status_card_current].dist_card_status_id
							AND [dist_card_statuses_language].language_id = 0

					--branch card joins
					LEFT OUTER JOIN [branch_card_status_current]
						ON [cards_temp].card_id = [branch_card_status_current].card_id						   
					LEFT OUTER JOIN [branch_card_statuses_language]
						ON [branch_card_statuses_language].branch_card_statuses_id = [branch_card_status_current].branch_card_statuses_id
							AND [branch_card_statuses_language].language_id = 0

					--Link to customer
					LEFT JOIN [customer_account]
						ON [cards_temp].card_id = [customer_account].card_id

					LEFT Join pin_batch_cards
						ON [cards_temp].card_id =[pin_batch_cards].card_id

				WHERE --Customer Search
					 ((@account_number IS NULL) OR (CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_account_number)) LIKE @account_number))
					AND ((@first_name IS NULL) OR (CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_first_name)) LIKE @first_name))
					AND ((@last_name IS NULL) OR (CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_last_name)) LIKE @last_name))
					AND ((@cms_id IS NULL) OR ([customer_account].cms_id LIKE @cms_id))
					--Other
					AND ((@dist_batch_id IS NULL) OR ([dist_batch].dist_batch_id = @dist_batch_id))
					AND ((@load_card_status_id IS NULL) OR ([load_batch_status_card_current].load_card_status_id = @load_card_status_id))
					AND ((@dist_card_status_id IS NULL) OR ([dist_batch_status_card_current].dist_card_status_id = @dist_card_status_id))
					AND ((@branch_card_statuses_id IS NULL) OR ([branch_card_status_current].branch_card_statuses_id = @branch_card_statuses_id))

					AND ((@batch_reference IS NULL) OR ([load_batch].load_batch_reference LIKE @batch_reference	OR [dist_batch].dist_batch_reference LIKE @batch_reference))	
					  	

					AND (([dist_batch].date_created BETWEEN COALESCE(@date_from, [dist_batch].date_created) AND COALESCE(@date_to, [dist_batch].date_created))
						OR
						([load_batch].load_date BETWEEN COALESCE(@date_from, [load_batch].load_date) AND COALESCE(@date_to, [load_batch].load_date))
						OR
						([branch_card_status_current].status_date BETWEEN COALESCE(@date_from, [branch_card_status_current].status_date) AND COALESCE(@date_to, [branch_card_status_current].status_date)))

						AND  ((@pin_batch_id IS NULL) OR ([pin_batch_cards].pin_batch_id = @pin_batch_id))

			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY status_date DESC

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

			DROP TABLE #Cards
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
/****** Object:  StoredProcedure [dbo].[usp_search_export_batches]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Search for export batches based on filter criteria
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_export_batches] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@product_id int = null,
	@export_batch_statuses_id int = null,
	@date_from datetime2 = null,
	@date_to datetime2 = null,
	@language_id int = 0,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @date_to IS NOT NULL
	SET @date_to = DATEADD(day, 1, @date_to)

	SELECT [export_batch_status_current].*, 
			[export_batch].date_created,
			[export_batch].batch_reference
	FROM [export_batch]
		INNER JOIN [export_batch_status_current]
			ON [export_batch].export_batch_id = [export_batch_status_current].export_batch_id
		INNER JOIN [export_batch_statuses_language]
			ON [export_batch_statuses_language].export_batch_statuses_id = [export_batch_status_current].export_batch_statuses_id
				AND [export_batch_statuses_language].language_id = @language_id
	WHERE [export_batch].issuer_id = COALESCE(@issuer_id, [export_batch].issuer_id)
		AND [export_batch].date_created BETWEEN COALESCE(@date_from, [export_batch].date_created) AND COALESCE(@date_to, [export_batch].date_created)
		AND	[export_batch_status_current].export_batch_statuses_id = COALESCE(@export_batch_statuses_id, [export_batch_status_current].export_batch_statuses_id)
	
END




GO
/****** Object:  StoredProcedure [dbo].[usp_search_export_batches_paged]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Search for export batches based on filter criteria
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_export_batches_paged] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@product_id int = null,
	@export_batch_statuses_id int = null,
	@batch_reference varchar(100) = null,
	@date_from datetime2 = null,
	@date_to datetime2 = null,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@language_id int = 0,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @date_to IS NOT NULL
	SET @date_to = DATEADD(day, 1, @date_to)

	DECLARE @StartRow INT, @EndRow INT;			
			
	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;


	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY date_created DESC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS			
			, *
	FROM( 
		SELECT [export_batch].*,
				[export_batch_statuses_language].language_text as export_batch_statuses_name,
				[export_batch_status_current].export_batch_statuses_id, 
				[export_batch_status_current].comments,
				[export_batch_status_current].status_date,
				[issuer].issuer_name,
				[issuer].issuer_code				
				
		FROM [export_batch]
			INNER JOIN [export_batch_status_current]
				ON [export_batch].export_batch_id = [export_batch_status_current].export_batch_id
			INNER JOIN [export_batch_statuses_language]
				ON [export_batch_statuses_language].export_batch_statuses_id = [export_batch_status_current].export_batch_statuses_id
					AND [export_batch_statuses_language].language_id = @language_id
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [export_batch].issuer_id
		WHERE [export_batch].issuer_id = COALESCE(@issuer_id, [export_batch].issuer_id)
			AND [export_batch].batch_reference = COALESCE(@batch_reference, [export_batch].batch_reference)
			AND [export_batch].date_created BETWEEN COALESCE(@date_from, [export_batch].date_created) AND COALESCE(@date_to, [export_batch].date_created)
			AND	[export_batch_status_current].export_batch_statuses_id = COALESCE(@export_batch_statuses_id, [export_batch_status_current].export_batch_statuses_id)
	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY date_created DESC
END




GO
/****** Object:  StoredProcedure [dbo].[usp_search_issuer]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_search_issuer]	
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
/****** Object:  StoredProcedure [dbo].[usp_search_issuer_by_id]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_search_issuer_by_id]
	@issuer_id int
AS
BEGIN
DECLARE @issuer_status_id varchar(20)

	SELECT * FROM issuer
	WHERE issuer_id = @issuer_id

END










GO
/****** Object:  StoredProcedure [dbo].[usp_search_pin_mailer_reprint]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_pin_mailer_reprint]
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@branch_id int,
	@user_role_id int,
	@pin_mailer_reprint_status_id int,
	@language_id int,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

    DECLARE @StartRow INT, @EndRow INT;			

			SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
			SET @EndRow = @StartRow + @RowsPerPage - 1;

			--append#1
			WITH PAGE_ROWS
			AS
			(
			SELECT ROW_NUMBER() OVER(ORDER BY status_date DESC, current_reprint_status ASC) AS ROW_NO
					, COUNT(*) OVER() AS TOTAL_ROWS
					, *
			FROM( 
				SELECT [cards].card_id,  
					   CASE 
							WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
							ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
						END AS 'card_number',
					   [cards].product_id, 
					   --[cards].sub_product_id, 
					   [cards].card_issue_method_id, 
					   [cards].card_priority_id, 
					   [cards].card_request_reference,
					   [pin_mailer_reprint_status_current].pin_mailer_reprint_status_id, 
					   [pin_mailer_reprint_statuses_language].language_text as 'current_reprint_status',
					   [pin_mailer_reprint_status_current].status_date, 
					   [issuer_product].product_bin_code,
					   [user_branch_access].issuer_id, issuer_name, branch_name, issuer_code, branch_code, [cards].branch_id,
					   [pin_mailer_reprint_status_current].comments
				FROM [cards]
				--Filter out cards linked to branches the user doesnt have access to.
					INNER JOIN (SELECT DISTINCT [user_roles_branch].branch_id, [branch].branch_code, [branch].branch_name, [branch].issuer_id
												,[issuer].issuer_name, [issuer].issuer_code, card_ref_preference							
								FROM [user_roles_branch] 
									INNER JOIN [user_roles]
										ON [user_roles_branch].user_role_id = [user_roles].user_role_id											
									INNER JOIN [branch]
										ON [user_roles_branch].branch_id = [branch].branch_id	
											AND [branch].branch_status_id = 0
									INNER JOIN [issuer]
										ON [branch].issuer_id = [issuer].issuer_id
											AND [issuer].issuer_status_id = 0
								WHERE [user_roles_branch].[user_id] = @audit_user_id		
										AND [user_roles_branch].issuer_id = COALESCE(@issuer_id, [user_roles_branch].issuer_id)								
										AND [user_roles_branch].branch_id = COALESCE(@branch_id, [user_roles_branch].branch_id)										
										AND [user_roles_branch].user_role_id = COALESCE(@user_role_id, [user_roles_branch].user_role_id)
										AND [user_roles].user_role_id IN (1,2,3,4,5,7)--Only want roles that allowed to search cards
								) as [user_branch_access]
						ON [cards].branch_id = [user_branch_access].branch_id
					INNER JOIN [issuer_product]
						ON [cards].product_id = [issuer_product].product_id
					INNER JOIN [pin_mailer_reprint_status_current]
						ON [pin_mailer_reprint_status_current].card_id = [cards].card_id
					INNER JOIN [pin_mailer_reprint_statuses_language]
						ON [pin_mailer_reprint_statuses_language].pin_mailer_reprint_status_id = [pin_mailer_reprint_status_current].pin_mailer_reprint_status_id
							AND [pin_mailer_reprint_statuses_language].language_id = @language_id
				WHERE [pin_mailer_reprint_status_current].pin_mailer_reprint_status_id = @pin_mailer_reprint_status_id
			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY status_date DESC, current_reprint_status ASC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[usp_search_pin_reissue]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_pin_reissue]
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@branch_id int = null,
	@user_role_id int = null,
	@pin_reissue_statuses_id int = null,
	@operator_user_id bigint = null,
	@authorise_user_id bigint = null,
	@date_from DATETIME = NULL,
	@date_to DATETIME = NULL,
	@language_id int,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Clean up any old requests that may have expired
	INSERT INTO [pin_reissue_status] (pin_reissue_statuses_id, pin_reissue_id, status_date, comments, [user_id], audit_workstation)
	SELECT 4, [pin_reissue].pin_reissue_id, GETDATE(), 'Expired', -2, 'SYSTEM'
	FROM [pin_reissue]
			INNER JOIN [pin_reissue_status_current]
		ON [pin_reissue_status_current].pin_reissue_id = [pin_reissue].pin_reissue_id
	WHERE [pin_reissue].request_expiry <= GETDATE()
		AND [pin_reissue_status_current].pin_reissue_statuses_id NOT IN ( 2, 3, 4)	


	DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)

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
				SELECT [pin_reissue].*,
						CASE 
							WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([pin_reissue].pan)),6,4) 
							ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([pin_reissue].pan))
						END AS 'card_number',
						[pin_reissue_status_current].pin_reissue_statuses_id,
						[pin_reissue_status_current].status_date,
						[pin_reissue_status_current].[user_id],
						[pin_reissue_status_current].comments,
						CONVERT(VARCHAR(max),DECRYPTBYKEY([user].username)) as operator_usename,
						CONVERT(VARCHAR(max),DECRYPTBYKEY([authoriser].username)) as authorise_username,
						[pin_reissue_statuses_language].language_text as pin_reissue_statuses_name,
						[user_branch_access].issuer_name,
						[user_branch_access].issuer_code,
						[user_branch_access].branch_name,
						[user_branch_access].branch_code,
						[user_branch_access].authorise_pin_reissue_YN,
						[issuer_product].product_code, [issuer_product].product_name
				FROM [pin_reissue]
					INNER JOIN [pin_reissue_status_current]
						ON [pin_reissue].pin_reissue_id = [pin_reissue_status_current].pin_reissue_id
					INNER JOIN [pin_reissue_statuses_language]
						ON [pin_reissue_status_current].pin_reissue_statuses_id = [pin_reissue_statuses_language].pin_reissue_statuses_id
							AND [pin_reissue_statuses_language].language_id = @language_id
					INNER JOIN [user]
						ON [user].[user_id] = [pin_reissue].operator_user_id
					INNER JOIN [issuer_product]
						ON [issuer_product].product_id = [pin_reissue].product_id
					--Filter out cards linked to branches the user doesnt have access to.
					INNER JOIN (SELECT DISTINCT [user_roles_branch].branch_id, [branch].branch_code, [branch].branch_name, [branch].issuer_id
												,[issuer].issuer_name, [issuer].issuer_code, authorise_pin_reissue_YN							
								FROM [user_roles_branch] 
									INNER JOIN [user_roles]
										ON [user_roles_branch].user_role_id = [user_roles].user_role_id											
									INNER JOIN [branch]
										ON [user_roles_branch].branch_id = [branch].branch_id	
											AND [branch].branch_status_id = 0
									INNER JOIN [issuer]
										ON [branch].issuer_id = [issuer].issuer_id
											AND [issuer].issuer_status_id = 0
											AND [issuer].enable_instant_pin_YN = 1
								WHERE [user_roles_branch].[user_id] = @audit_user_id		
										AND [user_roles_branch].issuer_id = COALESCE(@issuer_id, [user_roles_branch].issuer_id)								
										AND [user_roles_branch].branch_id = COALESCE(@branch_id, [user_roles_branch].branch_id)										
										AND [user_roles_branch].user_role_id = COALESCE(@user_role_id, [user_roles_branch].user_role_id)
										AND [user_roles].user_role_id IN (1,2,7)--Only want roles that allowed to search cards
								) as [user_branch_access]
						ON [pin_reissue].branch_id = [user_branch_access].branch_id
						LEFT OUTER JOIN [user] as [authoriser]
							ON [authoriser].[user_id] = [pin_reissue].authorise_user_id
				WHERE [pin_reissue_status_current].pin_reissue_statuses_id = COALESCE(@pin_reissue_statuses_id, [pin_reissue_status_current].pin_reissue_statuses_id)
						AND [pin_reissue].operator_user_id = COALESCE(@operator_user_id, [pin_reissue].operator_user_id)
						--AND [pin_reissue].authorise_user_id = COALESCE(@authorise_user_id, [pin_reissue].authorise_user_id)
						AND [pin_reissue_status_current].status_date BETWEEN COALESCE(@date_from, [pin_reissue_status_current].status_date) AND COALESCE(@date_to, [pin_reissue_status_current].status_date)

			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY status_date DESC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END




GO
/****** Object:  StoredProcedure [dbo].[usp_search_terminal]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi
-- Create date: 20150210
-- Description:	Search for terminal by branch or masterkey
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_terminal]
	@issuer_id int,
	@branch_id INT,
	@terminal_name nvarchar(50),
	@device_id nvarchar(50),	
	@terminal_model nvarchar(50),
		@PageIndex INT = 1
	,@RowsPerPage INT = 20
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN
		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate; 
		DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	WITH PAGE_ROWS
	AS
	( 
		SELECT ROW_NUMBER() OVER(ORDER BY [terminal_id] ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
		FROM(
			SELECT  
					DISTINCT t.[terminal_id]
					, t.[terminal_name]
					, t.[terminal_model]
					, i.[issuer_name],b.branch_name,CONVERT(varchar,DECRYPTBYKEY(device_id)) as deviceid
	FROM
		[terminals] t
		inner join [branch] b on b.branch_id=t.branch_id
		inner join [issuer] i on i.issuer_id= b.issuer_id
		where 		
		 (ISNULL(t.terminal_name, '')=ISNULL(@terminal_name, ISNULL(t.terminal_name, '')) or  
		 t.terminal_name like '%'+@terminal_name+'%') 
			AND 
		  (ISNULL(t.[terminal_model], '')=ISNULL(@terminal_model, ISNULL(t.terminal_name, '')) or  
		 t.terminal_name like '%'+@terminal_model+'%') 
		 AND 		 
		 (ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(t.device_id)), '')=ISNULL(@device_id, ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(t.device_id)), '')) or  
		 CONVERT(VARCHAR(max),DECRYPTBYKEY(t.device_id)) like '%'+@device_id+'%') 
		 AND t.branch_id = COALESCE(@branch_id, t.branch_id) and i.issuer_id=COALESCE(@issuer_id,i.issuer_id))
		 AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY [terminal_Id] ASC
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

	END

END




GO
/****** Object:  StoredProcedure [dbo].[usp_search_user]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec usp_search_user null,null,null,-1,0,null,-1,'',1,20
CREATE PROCEDURE [dbo].[usp_search_user]
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
							,CONVERT(VARCHAR(max),DECRYPTBYKEY(u.[username])) as 'username'
							,CONVERT(VARCHAR(max),DECRYPTBYKEY(u.[first_name])) as 'first_name'
							,CONVERT(VARCHAR(max),DECRYPTBYKEY(u.[last_name])) as 'last_name' 					
							,CONVERT(VARCHAR(max),DECRYPTBYKEY(u.[employee_id])) as 'empoyee_id'
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
 (ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(u.username)), '')=ISNULL(@user_name, ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(u.username)), '')) or  
 CONVERT(VARCHAR(max),DECRYPTBYKEY(u.username)) like '%'+@user_name+'%') and
 (ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(u.first_name)), '')=ISNULL(@first_name, ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(u.first_name)), '')) or
  CONVERT(VARCHAR(max),DECRYPTBYKEY(u.first_name)) like '%'+@first_name+'%') and
  (ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(u.last_name)), '')=ISNULL(@last_name, ISNULL(CONVERT(VARCHAR(max),DECRYPTBYKEY(u.last_name)), ''))
 or   CONVERT(VARCHAR(max),DECRYPTBYKEY(u.last_name)) like '%'+@last_name+'%'))

		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY username ASC

	--EXEC usp_insert_audit @audit_user_id, 
	--							1,
	--							NULL, 
	--							@audit_workstation, 
	--							'Getting unassigned users.', 
	--							NULL, NULL, NULL, NULL

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END













GO
/****** Object:  StoredProcedure [dbo].[usp_sys_decrypt]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_sys_decrypt] @cipheredText varbinary(max)
AS
BEGIN
OPEN SYMMETRIC KEY Indigo_Symmetric_Key
DECRYPTION BY CERTIFICATE Indigo_Certificate; 
--SELECT CONVERT(VARCHAR,DECRYPTBYKEY(''))
select CONVERT(varchar,DECRYPTBYKEY(@cipheredText))
CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END










GO
/****** Object:  StoredProcedure [dbo].[usp_sys_encrypt]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_sys_encrypt] @clearText varchar(max)
As
BEGIN
OPEN Symmetric Key Indigo_Symmetric_Key
DECRYPTION BY Certificate Indigo_Certificate;
SELECT CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@clearText)))
CLOSE Symmetric Key Indigo_Symmetric_Key;
END










GO
/****** Object:  StoredProcedure [dbo].[usp_update_branch]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Persist changes to the branch to the DB
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_branch] 
	@branch_id int,
	@branch_status_id int,
	@issuer_id int,
	@branch_code varchar(10),
	@branch_name varchar(30),
	@card_centre_branch_YN bit,
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
					[card_centre] = @card_centre,
					[card_centre_branch_YN] = @card_centre_branch_YN
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

				EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_update_card_fee_reference]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_card_fee_reference] 
	-- Add the parameters for the stored procedure here
	@card_id bigint,
	@fee_reference_number varchar(100),
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE [cards]
	SET fee_reference_number = @fee_reference_number
	WHERE card_id = @card_id
END




GO
/****** Object:  StoredProcedure [dbo].[usp_update_card_fee_reversal_ref]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_card_fee_reversal_ref]
	-- Add the parameters for the stored procedure here
	@card_id bigint,
	@fee_reversal_reference_number varchar(100),
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE [cards]
	SET fee_reversal_ref_number = @fee_reversal_reference_number
	WHERE card_id = @card_id
END




GO
/****** Object:  StoredProcedure [dbo].[usp_update_card_numbers]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_card_numbers] 
	@card_list AS dbo.key_value_array READONLY,
	@product_list AS dbo.key_value_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_CARD_NUMBER_TRAN]
		BEGIN TRY 
		
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				--This section helps with creating the card_index, instead of calling the fuction each time
				--Which slows down the insers, we get the key and then just encrypt
				DECLARE @objid int = object_id('cards'),		
						@key varbinary(100)
				SET @key = null
				SELECT @key = DecryptByKeyAutoCert(cert_id('cert_ProtectIndexingKeys'), null, mac_key) 
				FROM mac_index_keys 
				WHERE table_id = @objid

				UPDATE [cards]
				SET	[cards].card_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),cardarray.[value])),
					[cards].card_index = CONVERT(varbinary(24),HashBytes( N'SHA1', CONVERT(varbinary(8000), CONVERT(nvarchar(4000),RIGHT(cardarray.[value], 4))) + @key )),
					[cards].card_production_date = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),GETDATE()))
				FROM [cards] INNER JOIN @card_list cardarray			
					ON [cards].card_id = cardarray.[key]

				UPDATE [integration_cardnumbers]
				SET [integration_cardnumbers].card_sequence_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),productarray.[value]))
				FROM [integration_cardnumbers] INNER JOIN @product_list productarray
					ON [integration_cardnumbers].product_id = productarray.[key]

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
				
			COMMIT TRANSACTION [UPDATE_CARD_NUMBER_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_CARD_NUMBER_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_update_card_numbers_bikey]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_card_numbers_bikey] 
	@card_list AS dbo.key_value_array READONLY,
	@product_list AS dbo.bikey_value_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_CARD_NUMBER_TRAN]
		BEGIN TRY 

			--This section helps with creating the card_index, instead of calling the fuction each time
			--Which slows down the insers, we get the key and then just encrypt
			DECLARE @objid int = object_id('cards'),		
					@key varbinary(100)
			SET @key = null
			SELECT @key = DecryptByKeyAutoCert(cert_id('cert_ProtectIndexingKeys'), null, mac_key) 
			FROM mac_index_keys 
			WHERE table_id = @objid
		
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				UPDATE [cards]
				SET	[cards].card_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,cardarray.[value])),
					[cards].card_index = CONVERT(varbinary(24),HashBytes( N'SHA1', CONVERT(varbinary(8000), CONVERT(nvarchar(4000),RIGHT(cardarray.[value], 4))) + @key )),
					[cards].card_production_date = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),GETDATE()))
				FROM [cards] INNER JOIN @card_list cardarray			
					ON [cards].card_id = cardarray.[key]

				UPDATE [integration_cardnumbers]
				SET [integration_cardnumbers].card_sequence_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),productarray.[value]))
				FROM [integration_cardnumbers] INNER JOIN @product_list productarray
					ON [integration_cardnumbers].product_id = productarray.[key1]
						AND [integration_cardnumbers].sub_product_id = COALESCE(productarray.[key2], -1)

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
				
			COMMIT TRANSACTION [UPDATE_CARD_NUMBER_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_CARD_NUMBER_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_update_cardsequencenumber]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_cardsequencenumber]
	@product_id int,
	@newSequenceNumber int,
	@auditUserId bigint,
	@auditWorkStation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

       OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	   DECRYPTION BY CERTIFICATE Indigo_Certificate;

			UPDATE integration_cardnumbers 
			SET  card_sequence_number= ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@newSequenceNumber))
			WHERE product_id = @product_id

		CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END






GO
/****** Object:  StoredProcedure [dbo].[usp_update_customer_details]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_customer_details] 
	@card_id bigint,
	@customer_account_id bigint,
	@branch_id int,
	@product_id int,
	@card_priority_id int,
    @customer_account_number varchar(27),
	@domicile_branch_id int,
	@account_type_id int,
	@card_issue_reason_id int,
	@customer_first_name varchar(50),
	@customer_middle_name varchar(50),
	@customer_last_name varchar(50),
	@name_on_card varchar(30),
	@customer_title_id int,	
	@currency_id int,
	@resident_id int,
	@customer_type_id int,
	@cms_id varchar(50),
	@contract_number varchar(50),
	@customer_idnumber varchar(50),
	@contact_number varchar(50),
	@audit_user_id bigint,
	@audit_workstation varchar(100),	
	@ResultCode int OUTPUT	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_CUST_TRAN]
		BEGIN TRY 

			IF @customer_middle_name IS NULL
				SET @customer_middle_name = ''
			
			DECLARE @status_date datetime,
					@branch_card_statuses_id int

			SET @branch_card_statuses_id = 2

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			--The initial card status.
			INSERT branch_card_status
					(card_id, branch_card_statuses_id, status_date, [user_id], operator_user_id)
			VALUES (@card_id, @branch_card_statuses_id, GETDATE(), @audit_user_id, @audit_user_id)
			
			UPDATE [cards]
			SET branch_id = @branch_id,
				product_id = @product_id,
				card_priority_id = @card_priority_id
			WHERE card_id = @card_id

			--Save customer details
			UPDATE customer_account 
			SET card_issue_reason_id = @card_issue_reason_id, 
				account_type_id = @account_type_id, 
				customer_account_number= ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_account_number)),
				customer_first_name = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_first_name)), 
				customer_middle_name = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_middle_name)), 
				customer_last_name = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_last_name)), 
				name_on_card = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@name_on_card)), 
				customer_title_id = @customer_title_id, 
				customer_type_id = @customer_type_id, 
				currency_id = @currency_id, 
				resident_id = @resident_id, 
				cms_id = @cms_id, 
				contract_number = @contract_number, 
				Id_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@customer_idnumber)),
				contact_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@contact_number)),
				domicile_branch_id = @domicile_branch_id
			WHERE customer_account_id = @customer_account_id			

		


			--Log audit stuff
			DECLARE @branchcardstatus  varchar(max),
					@Scenario  varchar(max),
					@audit_msg varchar(max),
					@cardnumber varchar(16)

			SELECT  @branchcardstatus =  branch_card_statuses.branch_card_statuses_name
			FROM    branch_card_statuses 
			WHERE	branch_card_statuses.branch_card_statuses_id = @branch_card_statuses_id

			SELECT  @Scenario =  card_issue_reason.[card_issuer_reason_name]
			FROM	card_issue_reason 
			WHERE	card_issue_reason.[card_issue_reason_id] = @card_issue_reason_id

			SET @audit_msg =  'card request-' + 
								COALESCE(@branchcardstatus, 'UNKNWON') +  
								', cust id:' + COALESCE(CAST(@cms_id as varchar(max)), 'n/a') +
								', a/c:' + dbo.MaskString(@customer_account_number, 3, 4) + 
								', ' + COALESCE(@Scenario, 'UNKNWON')

			--log the audit record		
			EXEC usp_insert_audit @audit_user_id, 
									3,---IssueCard
									NULL, 
									@audit_workstation, 
									@audit_msg, 
									NULL, NULL, NULL, NULL

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key			
				
			COMMIT TRANSACTION [UPDATE_CUST_TRAN]
			SET @ResultCode = 0

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_CUST_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_update_external_system_fields]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_external_system_fields]
@external_system_field_id int,
@external_system_id int ,
@field_name nvarchar(100),
@audit_user_id bigint,
@audit_workstation varchar(100),
@ResultCode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		BEGIN TRANSACTION [UPATE_EXTERNAL_SYSTEM_FIELDS]
		BEGIN TRY 
			BEGIN
						UPDATE  external_system_fields
						SET external_system_id = @external_system_id,
						 field_name=@field_name
						 WHERE external_system_field_id=@external_system_field_id
				set @ResultCode=0
				
				Declare @audit_msg nvarchar(500)
						 --Add audit for dist batch creation								
				SET @audit_msg = 'Update: ' + CAST(@external_system_field_id AS varchar(max))
									
								   
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										9,
										NULL, 
										@audit_workstation, 
										@audit_msg, 
										NULL, NULL, NULL, NULL
										
										
			COMMIT TRANSACTION [UPATE_EXTERNAL_SYSTEM_FIELDS]	
			END
									
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPATE_EXTERNAL_SYSTEM_FIELDS]
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
/****** Object:  StoredProcedure [dbo].[usp_update_external_systems]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_external_systems]
@external_system_id int ,
@external_system_type_id int,
@system_name nvarchar(100),
@external_system_fields as dbo.[product_external_fields_array] READONLY,	
@audit_user_id bigint,
@audit_workstation varchar(100),
@ResultCode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		BEGIN TRANSACTION [UPATE_EXTERNAL_SYSTEMS]
		BEGIN TRY 
			BEGIN
						UPDATE  external_systems
						SET external_system_type_id = @external_system_type_id,
						 system_name=@system_name
						 WHERE external_system_id=@external_system_id

						 	DECLARE @RCExternal int
				EXECUTE @RCExternal = usp_modify_external_system_fields @external_system_id, @external_system_fields, @audit_user_id, @audit_workstation

				set @ResultCode=0

				Declare @audit_msg nvarchar(500)
						 --Add audit for dist batch creation								
				SET @audit_msg = 'Update: ' + CAST(@external_system_id AS varchar(max))
									
								   
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										9,
										NULL, 
										@audit_workstation, 
										@audit_msg, 
										NULL, NULL, NULL, NULL
										
										
			COMMIT TRANSACTION [UPATE_EXTERNAL_SYSTEMS]	
			END
									
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPATE_EXTERNAL_SYSTEMS]
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
/****** Object:  StoredProcedure [dbo].[usp_update_fee_charge]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_fee_charge] 
	-- Add the parameters for the stored procedure here
	@fee_detail_id int, 
	@card_issue_reason_id int,
	@fee_list  as dbo.key_value_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),	
	@ResultCode int = null OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

       -- Insert statements for procedure here
	BEGIN TRANSACTION [UPDATE_PRODUCT_FEE_CHARGE_TRAN]
		BEGIN TRY 			

			DELETE FROM [product_fee_charge]
			WHERE fee_detail_id = @fee_detail_id
					AND card_issue_reason_id = @card_issue_reason_id

			INSERT INTO [product_fee_charge] (fee_detail_id, card_issue_reason_id, currency_id, fee_charge, date_created)
			SELECT @fee_detail_id, @card_issue_reason_id, fl.[key], CAST(replace(fl.value,',','.')  AS DECIMAL(10,4)), GETDATE()
			FROM @fee_list fl	


			--log the audit record
			DECLARE @audit_description varchar(max), @issuer_id int, @fees varchar(max) = ''

			SELECT  @fees = STUFF(
									(SELECT ', ' + [currency].currency_code + '=' +  fee_list.value 
									 FROM @fee_list fee_list
										INNER JOIN [currency]
											ON [currency].currency_id = fee_list.[key]
									 FOR XML PATH(''))
								   , 1
								   , 1
								   , '')


			SELECT @issuer_id = issuer_id
			FROM [product_fee_detail] INNER JOIN [product_fee_scheme]
				ON [product_fee_scheme].fee_scheme_id = [product_fee_detail].fee_scheme_id
			WHERE [product_fee_detail].fee_detail_id = @fee_detail_id
					

			SET @audit_description = 'Fee Charge Update: Fee Detail Id ' + CAST(@fee_detail_id AS VARCHAR(max))	
										+', Issue Reason Id :'+  CAST(@card_issue_reason_id AS VARCHAR(max)) 
										+ ', Fees: ' + COALESCE(@fees, 'NONE')
										 	
			EXEC usp_insert_audit @audit_user_id, 
									4,
									NULL, 
									@audit_workstation, 
									@audit_description, 
									@issuer_id, NULL, NULL, NULL

			SET @ResultCode = 0	

			COMMIT TRANSACTION [UPDATE_PRODUCT_FEE_CHARGE_TRAN]
			SET @ResultCode = 0
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_PRODUCT_FEE_CHARGE_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_update_fee_scheme]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_fee_scheme] 
	-- Add the parameters for the stored procedure here
	@fee_scheme_id int,
	@issuer_id int,
	@fee_scheme_name varchar(100),
	@fee_detail_list as dbo.fee_detail_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [UPDATE_PRODUCT_FEE_SCHEME_TRAN]
		BEGIN TRY 			

			IF (SELECT COUNT(*) FROM [product_fee_scheme] 
					WHERE fee_scheme_name = @fee_scheme_name AND issuer_id = @issuer_id AND fee_scheme_id != @fee_scheme_id) > 0
				BEGIN
					SET @ResultCode = 226						
				END		
			ELSE IF (SELECT COUNT(*) FROM
						(SELECT fee_detail_name FROM @fee_detail_list fdl GROUP BY fdl.fee_detail_name
						HAVING COUNT(*) > 1) AS tb1) > 0
				BEGIN
					SET @ResultCode = 227
				END
			ELSE
				BEGIN
					DECLARE @effective_from DATETIME = GETDATE()


					UPDATE [product_fee_scheme]
					SET fee_scheme_name = @fee_scheme_name
					WHERE fee_scheme_id = @fee_scheme_id	
			
					--DELETE DELATAILS AND LINKED CHARGES FOR THOSE NO LONGER IN THE LIST
					DELETE FROM [product_fee_charge]
					WHERE fee_detail_id IN (
						SELECT fee_detail_id
						FROM [product_fee_detail]
						WHERE fee_scheme_id = @fee_scheme_id 
							AND fee_detail_id NOT IN (SELECT dl.fee_detail_id FROM @fee_detail_list dl WHERE dl.fee_detail_id > 0)
					)

					DELETE FROM [product_fee_detail]
					WHERE fee_scheme_id = @fee_scheme_id
							AND fee_detail_id NOT IN (SELECT dl.fee_detail_id FROM @fee_detail_list dl WHERE dl.fee_detail_id > 0)

					--UPDATE THOSE WITH VALID ID"S
					UPDATE [product_fee_detail]
					SET fee_detail_name = dl.fee_detail_name, 
						fee_editable_YN = dl.fee_editable_TN, 
						fee_waiver_YN = dl.fee_waiver_YN
					FROM [product_fee_detail]
							INNER JOIN @fee_detail_list dl
								ON [product_fee_detail].fee_detail_id = dl.fee_detail_id
					WHERE [product_fee_detail].fee_scheme_id = @fee_scheme_id
							AND dl.fee_detail_id > 0

					--INSERT ANY NEW DETAILS
					INSERT INTO [product_fee_detail] (fee_scheme_id, fee_detail_name, fee_editable_YN, fee_waiver_YN, 
														effective_from, effective_to, deleted_yn)
					SELECT @fee_scheme_id, dl.fee_detail_name, dl.fee_editable_TN, dl.fee_waiver_YN, 
							@effective_from, null, 0
					FROM @fee_detail_list dl
					WHERE dl.fee_detail_id < 0

					--log the audit record
					DECLARE @audit_description varchar(max), @fee_details varchar(max)

					SELECT  @fee_details = STUFF(
									(SELECT ' Fee Band Name: ' + dl.fee_detail_name + ', editable: ' +  CAST(dl.fee_editable_TN AS VARCHAR(MAX)) 
										+ ', waiver: ' +  CAST(dl.fee_waiver_YN AS VARCHAR(MAX)) + ';' 
									 FROM @fee_detail_list dl
									 FOR XML PATH(''))
								   , 1
								   , 1
								   , '')
					

					SET @audit_description = 'Fee Scheme Update: ' + @fee_scheme_name
												+' , Fee Scheme Id :'+  CAST(@fee_scheme_id AS VARCHAR(max)) 
												+' , Fee Bands : ' + COALESCE(@fee_details, 'NONE')
										 	
					EXEC usp_insert_audit @audit_user_id, 
											4,
											NULL, 
											@audit_workstation, 
											@audit_description, 
											@issuer_id, NULL, NULL, NULL
										
					SET @ResultCode = 0
			END

			COMMIT TRANSACTION [UPDATE_PRODUCT_FEE_SCHEME_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_PRODUCT_FEE_SCHEME_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_update_font]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_font]
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
																	
				--EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_update_instant_authorisation_pin]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi
-- Create date: 20150108
-- Description:	User authorisation pin number
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_instant_authorisation_pin]
	@user_id bigint,
	@authorisation_pin_number varchar(100),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	 BEGIN TRANSACTION [UPDATE_USER_AUTH_PIN_TRAN]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate
			
			UPDATE dbo.[user] 
			SET 
				[instant_authorisation_pin] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max), @authorisation_pin_number)),
				[last_authorisation_pin_changed_date] = GETDATE()
			WHERE [user].[user_id] = @user_id

				--log the audit record
				DECLARE @audit_description varchar(max),
				        @username varchar(100)

				SELECT  @username = CONVERT(VARCHAR(max),DECRYPTBYKEY([username]))
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

				
				EXEC usp_insert_audit @audit_user_id, 
									 7,---UserAdmin
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

			COMMIT TRANSACTION [UPDATE_USER_AUTH_PIN_TRAN]	
			SET @ResultCode = 0	
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
/****** Object:  StoredProcedure [dbo].[usp_update_issuer]    Script Date: 2016-04-05 09:55:42 AM ******/
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

CREATE PROCEDURE [dbo].[usp_update_issuer]
	@issuer_id int,
	@issuer_status_id int,
	@country_id int,
	@issuer_name varchar(50),
	@issuer_code varchar(10),
	@instant_card_issue_YN bit,
	@maker_checker_YN bit,
	@back_office_pin_auth_YN bit,	
	@language_id int = NULL,
	@card_ref_preference bit,
	@classic_card_issue_YN bit,
	@enable_instant_pin_YN bit,
	@authorise_pin_issue_YN bit,
	@authorise_pin_reissue_YN bit,	
	@prod_interface_parameters_list AS dbo.bikey_value_array READONLY,
	@issue_interface_parameters_list AS dbo.bikey_value_array READONLY,
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
					[instant_card_issue_YN] = @instant_card_issue_YN,					
					[maker_checker_YN] = @maker_checker_YN,					
					[language_id] = @language_id,
					[card_ref_preference] = @card_ref_preference,
					[classic_card_issue_YN] = @classic_card_issue_YN,
					[enable_instant_pin_YN] = @enable_instant_pin_YN,
					[authorise_pin_issue_YN] = @authorise_pin_issue_YN,
					[authorise_pin_reissue_YN] = @authorise_pin_reissue_YN ,					
					[back_office_pin_auth_YN] = @back_office_pin_auth_YN
				WHERE [issuer_id] = @issuer_id

				DELETE FROM [issuer_interface]
				WHERE [issuer_id] = @issuer_id

				IF (@issuer_status_id = 0)
				BEGIN

					INSERT INTO [issuer_interface] (issuer_id, interface_type_id, connection_parameter_id, interface_guid, interface_area)
					SELECT @issuer_id, key1, key2, value, 0
					FROM @prod_interface_parameters_list

					INSERT INTO [issuer_interface] (issuer_id, interface_type_id, connection_parameter_id, interface_guid, interface_area)
					SELECT @issuer_id, key1, key2, value, 1
					FROM @issue_interface_parameters_list				
				END
				ELSE
				BEGIN
					-- Clean up the product tables related to this issuer.
					DELETE FROM product_interface 
					WHERE product_id IN (SELECT product_id FROM issuer_product WHERE issuer_id = @issuer_id)
					
					UPDATE issuer_product SET DeletedYN = 1 
					WHERE issuer_id = @issuer_id

				END

				--IF (@instant_card_issue_YN = 1)
				--BEGIN
				--	IF (@account_validation_YN = 1)
				--	BEGIN
				--		--INSERT ACCOUNT INTERFACE
				--		INSERT INTO [issuer_interface]
				--			([issuer_id], [interface_type_id], [connection_parameter_id])
				--		VALUES (@issuer_id, 0, @account_connection_id)
				--	END

				--	--INSERT CORE BANKING INTERFACE
				--	INSERT INTO [issuer_interface]
				--		([issuer_id], [interface_type_id], [connection_parameter_id])
				--	VALUES (@issuer_id, 1, @corebanking_connection_id)
				--END


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
										 	
				EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_update_issuer_card_ref_pref]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Nduvho Mukhavhuli
-- Create date: 2014/09/29	
-- =============================================

CREATE PROCEDURE [dbo].[usp_update_issuer_card_ref_pref]
	@issuer_id int,
	@selectedOption int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_ISSUER_CARD_REF_PREF]
		BEGIN TRY 			
			BEGIN
				UPDATE [dbo].[issuer]
				SET [card_ref_preference] = @selectedOption					
				WHERE [issuer_id] = @issuer_id

				SELECT @ResultCode = 0
				COMMIT TRANSACTION [UPDATE_ISSUER_CARD_REF_PREF]				
			END
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_ISSUER_CARD_REF_PREF]
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
/****** Object:  StoredProcedure [dbo].[usp_update_ldap]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo Setenane
-- Create date: 2014/04/03
-- Description:	Updates an LDAP record
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_ldap]
	@ldap_setting_id int,
	@ldap_setting_name varchar(100),
	@hostname_or_ip varchar(200),	
	@path varchar(200),
	@domain_name varchar(100) = NULL,
	@auth_username varchar(100) = NULL,
	@auth_password varchar(100) = NULL,
	@auth_type_id int,
	@external_inteface_id char(36),
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
					[username] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_username)),
					[password] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_password)),
					auth_type_id=@auth_type_id,
					external_inteface_id=@external_inteface_id
				WHERE ldap_setting_id = @ldap_setting_id


			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

			--log the audit record
			DECLARE @audit_description varchar(max)
			SELECT @audit_description = 'LDAP Updated: ' + @ldap_setting_name
										+ ', Id: ' + CAST(@ldap_setting_id as varchar(max))		

			EXEC usp_insert_audit @audit_user_id, 
									9,
									NULL, 
									@audit_workstation, 
									@audit_description, 
									NULL, NULL, NULL, NULL	

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
/****** Object:  StoredProcedure [dbo].[usp_update_masterkey]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi
-- Create date: 20150213
-- Description:	Update Masterkey
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_masterkey]
	@masterkey_id int 
	, @masterkey varchar(max)
	, @masterkey_name varchar(250)
	, @issuer_id int
	, @audit_user_id bigint
	, @audit_workstation varchar(100)
	, @ResultCode int =null OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	
	BEGIN TRANSACTION [UPDATE_MASTERKEY_TRAN]
		BEGIN TRY 
		
				OPEN Symmetric Key Indigo_Symmetric_Key
				DECRYPTION BY Certificate Indigo_Certificate;
				IF (SELECT COUNT(*) FROM [masterkeys] WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([masterkeys].masterkey)) =@masterkey and masterkey_id != @masterkey_id) > 0
				BEGIN
					
					SET @ResultCode = 606						
				END
			ELSE	
		
				IF (SELECT COUNT(*) FROM [masterkeys] WHERE [masterkeys].masterkey_name =@masterkey_name and masterkey_id != @masterkey_id) > 0
				BEGIN
					
					SET @ResultCode = 607						
				END
			ELSE			
			BEGIN
				UPDATE [dbo].[masterkeys]
				SET [masterkey] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@masterkey))
				  ,[issuer_id] = @issuer_id
				  ,[masterkey_name] = @masterkey_name 
				  ,[date_changed] = GETDATE()
				WHERE masterkey_id = @masterkey_id

				SET @ResultCode = 0	

				CLOSE Symmetric Key Indigo_Symmetric_Key;

				--log the audit record
				DECLARE @audit_description varchar(max) = '',
						@issuer_name varchar(100)

				SELECT @issuer_name = issuer_name
				FROM [issuer]
				WHERE issuer_id = @issuer_id

				SET @audit_description = 'Masterkey Update: ' + CAST(@masterkey_name AS VARCHAR(max))	+ 
										+', id :'+  CAST(@masterkey_id AS VARCHAR(max)) 
										--+ ', issuer: ' + COALESCE(@issuer_name, 'UNKNOWN') 
										 	
				EXEC usp_insert_audit @audit_user_id, 
									 9,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @issuer_id, NULL, NULL, NULL	
					END

			COMMIT TRANSACTION [UPDATE_MASTERKEY_TRAN]				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_MASTERKEY_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_update_printfieldvalues]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_printfieldvalues]	
	@product_fields as dbo.key_binary_value_array READONLY,
	@customeraccountid int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	--if( (select count(*) from customer_fields where customer_fields.product_field_id=@product_field_id 
	--and customer_fields.customer_account_id=@customeraccountid)>0)
	--	BEGIN
	--		 update customer_fields set value=	ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@value))
	--		  where [product_field_id]= @product_field_id
			 
	
	--	 END
	-- ELSE
		 BEGIN
			--Update Product Fields
					INSERT INTO customer_fields (customer_account_id, product_field_id, value)
						SELECT @customeraccountid, pf.[key], ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CAST(pf.[value] as varbinary(max)))
						FROM @product_fields pf INNER JOIN product_fields
							ON pf.[key] = product_fields.product_field_id
						WHERE product_fields.print_field_type_id = 0 
						--and product_fields.deleted=0

					--		-- if values in printfields updating 
					--if((select count(*) from customer_fields as c
					--	inner join product_fields as  p on c.product_field_id=p.product_field_id
					--where 	 c.customer_account_id= @customeraccountid and Upper(p.Mapped_Name)='IND_SYS_NOC' )>0)
					--BEGIN
		
					--update c 			
					--set c.value=	ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@name_on_card))
					--from  customer_fields as c inner join  product_fields as  p on c.product_field_id=p.product_field_id
					--  where c.customer_account_id=@customeraccountid 
					--  and Upper(p.Mapped_Name)='IND_SYS_NOC'

					--END

					--Update Product Image Fields
					INSERT INTO customer_image_fields (customer_account_id, product_field_id, value)
						SELECT @customeraccountid, pf.[key], pf.[value]
						FROM @product_fields pf INNER JOIN product_fields
							ON pf.[key] = product_fields.product_field_id
						WHERE product_fields.print_field_type_id = 1 
						--and product_fields.deleted=0
		 END

	  CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END


GO
/****** Object:  StoredProcedure [dbo].[usp_update_product]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_product]
	-- Add the parameters for the stored procedure here
	@product_id int,
	@product_name varchar(100),
	@product_code varchar(50),
	@product_bin_code varchar(9),
	@issuer_id  int,
	@pan_length smallint,
	@sub_product_code varchar(4),
	@expiry_months int,
	@fee_scheme_id int = null,
	@card_issue_method_id int,

	@name_on_card_top decimal(8,2) =0 ,
	@name_on_card_left decimal(8,2)=0,
	@Name_on_card_font_size int=0,
	@font_id int=1,

	@src1_id int,
	@src2_id int,
	@src3_id int,
	@PVKI varchar(100),
	@PVK varchar(100),
	@CVKA varchar(100),
	@CVKB varchar(100),
	
	@enable_instant_pin_YN bit,
	@enable_instant_pin_reissue_YN bit,
	@pin_calc_method_id int,
	@min_pin_length int,
	@max_pin_length int,	

	@cms_exportable_YN bit,
	@product_load_type_id int,
	@auto_approve_batch_YN bit,
	@account_validation_YN bit,

	@pin_mailer_printing_YN bit,
	@pin_mailer_reprint_YN bit,

	@card_issue_reasons_list as dbo.key_value_array READONLY,
	@account_types_list as dbo.key_value_array READONLY,
	@currencylist AS dbo.product_currency_array READONLY,
	@external_system_fields as dbo.[product_external_fields_array] READONLY,			
	@prod_interface_parameters_list AS dbo.bikey_value_array READONLY,
	@issue_interface_parameters_list AS dbo.bikey_value_array READONLY,

	@decimalisation_table varchar(100),
	@pin_validation_data varchar(100),
	@pin_block_formatid int,

	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int =null OUTPUT,
	@new_product_id int =null OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	BEGIN TRANSACTION [UPDATE_Product_TRAN]
		BEGIN TRY 			

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate
			--Check for duplicate's
			IF EXISTS(SELECT * FROM [issuer_product] WHERE ([product_code] = @product_code AND [product_id] != @product_id AND issuer_id = @issuer_id))
				BEGIN
					SET @ResultCode = 221						
				END
			IF EXISTS(SELECT * FROM [issuer_product] WHERE ([product_name] = @product_name AND [product_id] != @product_id))
				BEGIN
					SET @ResultCode = 220
				END
			ELSE IF dbo.ProductValidation(@product_id, @product_bin_code, @sub_product_code) = 0
				BEGIN
					SET @ResultCode = 222
				END
			ELSE IF 0 = ANY(SELECT dbo.FileParameterValidation(key2, value, key1) FROM @prod_interface_parameters_list)
				BEGIN
					SET @new_product_id = 0
					SET @ResultCode = 228
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
						[Name_on_card_font_size] = @Name_on_card_font_size,
						[src1_id] = @src1_id,
						[src2_id] = @src2_id,
						[src3_id] = @src3_id,
						[PVKI] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@PVKI)),
						[PVK] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@PVK)),
						[CVKA] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@CVKA)),
						[CVKB] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@CVKB)),
						[expiry_months] = @expiry_months,						
						[fee_scheme_id] = @fee_scheme_id,
						[enable_instant_pin_YN] = @enable_instant_pin_YN,
						[min_pin_length] = @min_pin_length,
						[max_pin_length] = @max_pin_length,
						[enable_instant_pin_reissue_YN] = @enable_instant_pin_reissue_YN,
						[cms_exportable_YN] = @cms_exportable_YN,
						[product_load_type_id] = @product_load_type_id,
						[pan_length] = @pan_length,
						[sub_product_code] = @sub_product_code,
						[pin_calc_method_id] = @pin_calc_method_id,
						[auto_approve_batch_YN] = @auto_approve_batch_YN,
						[account_validation_YN] = @account_validation_YN,
						[pin_mailer_printing_YN] = @pin_mailer_printing_YN,
						[pin_mailer_reprint_YN] = @pin_mailer_reprint_YN,
						[card_issue_method_id] = @card_issue_method_id,
						[decimalisation_table]=ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@decimalisation_table)), 
						[pin_validation_data]=ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@pin_validation_data)),
						[pin_block_formatid]=@pin_block_formatid
					WHERE [product_id] = @product_id



				--insert issuing reason
				DELETE FROM [product_issue_reason] WHERE product_id = @product_id

				INSERT INTO [product_issue_reason] (product_id, card_issue_reason_id)
				SELECT @product_id, value
				FROM @card_issue_reasons_list	

				--insert account types
				DELETE FROM [products_account_types] WHERE product_id = @product_id

				INSERT INTO [products_account_types] (product_id, account_type_id)
				SELECT @product_id, value
				FROM @account_types_list

				--Insert interfaces
				DELETE FROM [product_interface]
				WHERE [product_id] = @product_id

				INSERT INTO [product_interface] (product_id, interface_type_id, connection_parameter_id, interface_guid, interface_area)
				SELECT @product_id, key1, key2, value, 0
				FROM @prod_interface_parameters_list

				INSERT INTO [product_interface] (product_id, interface_type_id, connection_parameter_id, interface_guid, interface_area)
				SELECT @product_id, key1, key2, value, 1
				FROM @issue_interface_parameters_list

				--Update the products currency
				DELETE FROM product_currency
				WHERE [product_id] = @product_id

				Declare  @RC as int
				EXECUTE @RC = [usp_insert_product_currency] @product_id, @currencylist, @audit_user_id, @audit_workstation


					--Update the products currency
				DELETE FROM product_external_system
				WHERE [product_id] = @product_id

				DECLARE @RCExternal int
				EXECUTE @RCExternal = [usp_insert_product_external_systems] @product_id, @external_system_fields, @audit_user_id, @audit_workstation

										
				
				DECLARE @audit_description varchar(500)
				SELECT @audit_description = 'Product updated: ' + @product_code  + ', Product Name: ' + @product_name + 
																     ', bin code: ' + @product_bin_code 
																	 + ', Product Id: ' + CAST(@product_id as varchar(max))
																	
				EXEC usp_insert_audit @audit_user_id, 
									 4,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @Issuer_id, NULL, NULL, NULL

				SELECT @ResultCode = 0				
			END
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
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
/****** Object:  StoredProcedure [dbo].[usp_update_product_print_fields]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_product_print_fields]
	-- Add the parameters for the stored procedure here
	@product_id int,
	@product_fields_list as dbo.[product_print_fields] READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [UPDATE_PRODUCT_FIELDS_TRAN]
		BEGIN TRY 			

			IF(SELECT COUNT(*) FROM
						(SELECT field_name FROM @product_fields_list fdl WHERE product_id = @product_id GROUP BY fdl.field_name
						HAVING COUNT(*) > 1) AS tb1) > 0
				BEGIN
					SET @ResultCode = 227
				END
			ELSE
				BEGIN
					DECLARE @effective_from DATETIME = GETDATE()
					
					MERGE product_fields AS [target]
						USING @product_fields_list AS [source]
						ON ([target].product_id = [source].product_id AND [target].product_field_id = [source].product_field_id) 
						WHEN NOT MATCHED BY TARGET 
							THEN 
								INSERT 
								   ([product_id]
								   ,[field_name]
								   ,[print_field_type_id]
								   ,[X]
								   ,[Y]
								   ,[width]
								   ,[height]
								   ,[font]
								   ,[font_size]
								   ,[mapped_name]
								   ,[editable]
								   ,[deleted]
								   ,[label]
								   ,[max_length])
							 VALUES
								   ([source].product_id
								   ,[source].field_name
								   ,[source].print_field_type_id
								   ,[source].X
								   ,[source].Y
								   ,[source].width
								   ,[source].height
								   ,[source].font
								   ,[source].font_size
								   ,[source].mapped_name
								   ,[source].editable
								   ,[source].deleted
								   ,[source].label
								   ,[source].max_length)
						WHEN MATCHED 
							THEN UPDATE SET 
								 [product_id] = [source].product_id 
								  ,[field_name] = [source].field_name 
								  ,[print_field_type_id] = [source].print_field_type_id 
								  ,[X] = [source].X 
								  ,[Y] = [source].Y 
								  ,[width] = [source].width 
								  ,[height] = [source].height 
								  ,[font] = [source].font 
								  ,[font_size] = [source].font_size 
								  ,[mapped_name] = [source].mapped_name 
								  ,[editable] = [source].editable 
								  ,[deleted] = [source].deleted 
								  ,[label] = [source].label 
								  ,[max_length] = [source].max_length
						OUTPUT $action, inserted.*, deleted.*;
																
					SET @ResultCode = 0


					DECLARE @product_field_id int

					DECLARE PrintFieldTypes_cursor CURSOR FOR 
							SELECT DISTINCT product_fields.product_field_id
							FROM @product_fields_list p
							INNER JOIN product_fields on p.[product_id]=product_fields.[product_id]
							WHERE p.[deleted]=1

					OPEN PrintFieldTypes_cursor

						FETCH NEXT FROM PrintFieldTypes_cursor 
						INTO @product_field_id

						WHILE @@FETCH_STATUS = 0
						BEGIN
							FETCH NEXT FROM PrintFieldTypes_cursor 
							INTO @product_field_id
							--- if the print field is not using in customer_fields consider as hard delte.
							if((SELECT count(*) from customer_fields 
								WHERE customer_fields.product_field_id= @product_field_id)<=0 and (SELECT count(*) from customer_image_fields 
								WHERE customer_image_fields.product_field_id= @product_field_id)<=0 )

									BEGIN

									Delete from product_fields where product_fields.product_field_id=@product_field_id

									END

							
							END 
						CLOSE PrintFieldTypes_cursor;
						DEALLOCATE PrintFieldTypes_cursor;

			END

			COMMIT TRANSACTION [UPDATE_PRODUCT_FIELDS_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_PRODUCT_FIELDS_TRAN]
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
/****** Object:  StoredProcedure [dbo].[usp_update_terminal]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi
-- Create date: 20150130
-- Description:	Update the terminal information
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_terminal]
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
					  ,[device_id] = CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@device_id)))
					  ,[branch_id] = @branch_id
					  ,[terminal_masterkey_id] = @terminal_masterkey_id
					  ,[workstation] = @audit_workstation
					  ,[date_changed] = GETDATE()
				 WHERE [terminal_id] = @terminal_id
				 SET @ResultCode = 0
			

				--log the audit record
				DECLARE @audit_description varchar(max), @issuer_id int, @audit_branch_name varchar(max), 
						@audit_branch_code varchar(max)

				SELECT @issuer_id = issuer_id, @audit_branch_name = branch_name,
						@audit_branch_code = branch_code
				FROM [branch]
				WHERE branch_id = @branch_id

				SET @audit_description = 'Terminal Updated: '+ COALESCE(@terminal_name, 'UNKNOWN') 
										  + ' , Terminal Id: ' + CAST(@terminal_id as varchar(max))
										  + ' , branch code: ' + @audit_branch_code
										  + ' , branch name: ' + @audit_branch_name		

				EXEC usp_insert_audit @audit_user_id, 
									 9,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @issuer_id, NULL, NULL, NULL
					
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
/****** Object:  StoredProcedure [dbo].[usp_update_user]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 24 March 2014
-- Description:	Updates a user
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_user] 
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
	@connection_parameter_id int = null,
	@external_interface_id char(36)=null,
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
			WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) = @username
					AND [user_id] != @user_id

			IF @dup_check > 0
				BEGIN
					SELECT @ResultCode = 69							
				END
			ELSE
			BEGIN
				UPDATE [user]
				SET	[user_status_id] = COALESCE(@user_status_id, [user].[user_status_id])
					,[username] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@username))
					,[first_name] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@first_name))
					,[last_name] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@last_name))
					,[user_email] = @user_email
					,[online] = COALESCE(@online, [user].[online])
					,[employee_id] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@employee_id))
					,[last_login_date] = COALESCE(@last_login_date, [user].[last_login_date])
					,[last_login_attempt] = COALESCE(@last_login_attempt, [user].[last_login_attempt])
					,[number_of_incorrect_logins] = COALESCE(@number_of_incorrect_logins, [user].[number_of_incorrect_logins])
					,[last_password_changed_date] = COALESCE(@last_password_changed_date, [user].[last_password_changed_date])
					,[workstation] = COALESCE(@workstation, [user].[workstation])
					,[connection_parameter_id] = @connection_parameter_id
					,[language_id] = @language_id
					,[external_interface_id]=@external_interface_id
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
										 ', login ldap: ' + CAST(COALESCE(@connection_parameter_id, 0) as varchar(max)) + 
										 ', groups: ' + COALESCE(@groupname, 'none-selected')

				EXEC usp_insert_audit @audit_user_id, 
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
/****** Object:  StoredProcedure [dbo].[usp_update_user_group]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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











GO
/****** Object:  StoredProcedure [dbo].[usp_update_user_language]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Selebalo Setenane
-- Create date: 22 April 2014
-- Description:	Updates a user's default language
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_user_language] 
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
/****** Object:  StoredProcedure [dbo].[usp_update_useradminsettings]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_useradminsettings]
	@user_admin_id int,
	@PasswordValidPeriod int,
	@PasswordMinLength int,
	@PasswordMaxLength int,
	@PreviousPasswordsCount int,	
	@maxInvalidPasswordAttempts int,
	@PasswordAttemptLockoutDuration int,
	@audit_user_id int,
	@audit_workstation nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  update user_admin set PasswordValidPeriod=@PasswordValidPeriod,
										PasswordMinLength=@PasswordMinLength,
										PasswordMaxLength=@PasswordMaxLength,
										PreviousPasswordsCount=@PreviousPasswordsCount,
										maxInvalidPasswordAttempts=@maxInvalidPasswordAttempts,
										PasswordAttemptLockoutDuration=@PasswordAttemptLockoutDuration,
										createdby=@audit_user_id
										where user_admin_id=@user_admin_id


			EXEC usp_insert_audit @audit_user_id, 
									4,
									NULL, 
									@audit_workstation, 
									'updated user admin settings.', 
									null, NULL, NULL, NULL

END



GO
/****** Object:  StoredProcedure [dbo].[usp_validate_cards_ordered]    Script Date: 2016-04-05 09:55:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_validate_cards_ordered] 
	@card_list dbo.key_value_array READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
		SELECT DISTINCT card_id, product_id
		FROM [cards]
		WHERE card_id IN (
			SELECT DISTINCT [dist_batch_cards].card_id
			FROM [dist_batch_cards]
					INNER JOIN [dist_batch]
						ON [dist_batch].dist_batch_id = [dist_batch_cards].dist_batch_id
							AND [dist_batch].dist_batch_type_id = 0
					INNER JOIN [dist_batch_status_current]
						ON [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
			WHERE [dist_batch_cards].dist_card_status_id = 20
					AND [dist_batch_status_current].dist_batch_statuses_id = 20
					AND [dist_batch_cards].card_id IN ( SELECT cardList.[key]
														FROM @card_list cardList)
		)
END


GO
