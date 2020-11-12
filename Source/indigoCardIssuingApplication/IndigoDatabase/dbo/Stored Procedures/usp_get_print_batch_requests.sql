CREATE PROCEDURE [dbo].[usp_get_print_batch_requests] 
	@print_batch_id bigint,
	@language_id int,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_print_BATCH_CARDS]
		BEGIN TRY 
		Declare @check_masking bit
		set @check_masking=1
		Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
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
			SELECT ROW_NUMBER() OVER(ORDER BY  request_reference ASC) AS ROW_NO
					, COUNT(*) OVER() AS TOTAL_ROWS
					, *
			FROM( 
			SELECT DISTINCT [hybrid_requests].request_id as 'request_id', 
				--CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) AS card_number,
				[hybrid_requests].[request_reference] AS 'request_reference',	
				[hybrid_requests].product_id, --[cards].sub_product_id, 
				[hybrid_requests].card_priority_id, 
				[hybrid_requests].card_issue_method_id,			   
				[hybrid_request_statuses_language].language_text AS current_card_status,
				[hybrid_request_status_current].comments,						
				CONVERT(Datetime,SWITCHOFFSET([hybrid_request_status_current].status_date,@UserTimezone)) as status_date,	
				[hybrid_request_status_current].hybrid_request_statuses_id as hybrid_request_statuses_id,					
				
				[hybrid_request_status_current].operator_user_id, 
				'' AS operator_username, 
				'' AS product_bin_code,		
				'' AS sub_product_code,	
				[issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
				[branch].branch_id, [branch].branch_name, [branch].branch_code,cc.card_id, CASE @check_masking
							WHEN 1 THEN 
								CASE 
									WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
									ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
								END
							ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
						 END AS 'card_number'
			FROM hybrid_requests
				INNER JOIN print_batch_requests ON hybrid_requests.request_id = print_batch_requests.request_id

			INNER JOIN [hybrid_request_status_current]
				ON [hybrid_request_status_current].request_id = [hybrid_requests].request_id						   
			INNER JOIN [hybrid_request_statuses_language]
				ON [hybrid_request_status_current].hybrid_request_statuses_id = [hybrid_request_statuses_language].hybrid_request_statuses_id
					AND [hybrid_request_statuses_language].language_id = @language_id
					INNER JOIN [branch]
				ON [branch].branch_id = [hybrid_requests].branch_id
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [branch].issuer_id
				LEFT join [dbo].[customer_account_requests] as cr on hybrid_requests.request_id=cr.request_id
				LEFT JOIN [dbo].[customer_account_cards] as cc on cc.customer_account_id=cr.customer_account_id
				LEFT OUTER JOIN [cards]
						ON [cards].card_id =cc.card_id
				--INNER JOIN print_batch_status ON print_batch_status.print_batch_id = print_batch_requests.print_batch_id
				--INNER JOIN print_batch_statuses ON print_batch_status.print_batch_statuses_id = print_batch_statuses.print_batch_statuses_id
				
			WHERE print_batch_requests.print_batch_id = @print_batch_id
			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY  request_reference ASC
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;		
			
			COMMIT TRANSACTION [GET_print_BATCH_CARDS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_print_BATCH_CARDS]
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