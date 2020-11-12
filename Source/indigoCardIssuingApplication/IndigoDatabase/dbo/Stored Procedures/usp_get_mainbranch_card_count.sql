CREATE PROCEDURE [dbo].[usp_get_mainbranch_card_count] 
	@issuer_id int,
	@branch_id int,
	@product_id int,
	--@sub_product_id int = NULL,
	@card_issue_method_id int = NULL,
	
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@cardcount int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DecLare @batchcount int,@main_branch_id int
	set @cardcount=0
	set @batchcount=0
	set @main_branch_id =0;

	select @main_branch_id=main_branch_id from branch where branch.branch_id=@branch_id
	--IF(@main_branch_id<>-1)
	--SET @branch_id=@main_branch_id
	

	SELECT @cardcount = COUNT(DISTINCT [cards].card_id)
	FROM [cards]
			INNER JOIN [branch_card_status_current]
				ON [cards].card_id = [branch_card_status_current].card_id
			INNER JOIN [branch]
				ON [cards].branch_id = [branch].branch_id
				--Left JOIN dist_batch_cards_temp
				--ON [cards].card_id <>dist_batch_cards_temp.card_id  
				
	WHERE  
	cards.branch_id = case when @main_branch_id=-1 then @branch_id else @main_branch_id end
			AND [cards].product_id = COALESCE([cards].product_id,@product_id)		
			AND [branch_card_status_current].branch_card_statuses_id = 0
			AND [cards].card_issue_method_id = COALESCE(@card_issue_method_id, [cards].card_issue_method_id)
						--AND [branch_card_status_current].issuer_id = @issuer_id



				 

		SELECT    @batchcount=    COUNT(*) 
		FROM            print_batch INNER JOIN
                         print_batch_requests ON print_batch.print_batch_id = print_batch_requests.print_batch_id INNER JOIN
						 print_batch_status ON  print_batch_status.print_batch_id = print_batch_requests.print_batch_id LEFT JOIN
                         hybrid_request_status ON print_batch_requests.request_id = hybrid_request_status.request_id 
                         
						 where  hybrid_request_status.hybrid_request_statuses_id  in (2)
						 AND print_batch.issuer_id=@issuer_id 
						 --and print_batch.branch_id=@branch_id
						 and print_batch.branch_id in (COALESCE(print_batch.branch_id,@branch_id)) 
  
	

	set @cardcount=@cardcount-@batchcount
	print @cardcount
END











