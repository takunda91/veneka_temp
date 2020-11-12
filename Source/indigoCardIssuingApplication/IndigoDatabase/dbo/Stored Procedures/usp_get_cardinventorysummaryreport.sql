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
	@product_id int=null,
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
	
DECLARE @UserTimezone as nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

	SELECT INTERX.issuer_id,
		   branch_code,
		   INTERX.branch_id,
		   INTERX.statuses_name,
		   INTERX.statuses_id,
		   COUNT(INTER.card_id)	AS CardCount,
		   issuer.issuer_name as 'issuer_name',
		   issuer_product.product_code+'-'+issuer_product.product_name as product,
		   issuer_product.product_id,
CAST(SWITCHOFFSET(SYSDATETIMEOFFSET(),@UserTimezone) as datetime) as 'excuted_datetime'

	FROM 	 
		  (SELECT [branch].branch_id, dist_batch_status_current.dist_batch_statuses_id as 'statuses_id', [dist_batch_cards].card_id,
		  issuer_product.product_code,issuer_product.product_name ,issuer_product.product_id

			FROM dist_batch_status_current
			INNER JOIN [dist_batch_cards]
				ON dist_batch_status_current.dist_batch_id = [dist_batch_cards].dist_batch_id
			INNER JOIN [cards]
				ON [dist_batch_cards].card_id = [cards].card_id
			INNER JOIN [branch]
				ON [cards].branch_id = [branch].branch_id
					AND [branch].branch_type_id = 0 
					INNER JOIN [issuer_product] 
		ON [cards].product_id=[issuer_product].product_id				
	WHERE [branch].branch_status_id = 0 
			) AS INTER
				RIGHT OUTER JOIN 			
			(SELECT issuer_id, branch_id,branch.branch_name+'-'+ branch_code as branch_code, [dist_batch_statuses].dist_batch_statuses_id as 'statuses_id', bcl.language_text as 'statuses_name',bcl.language_id
			 FROM	[branch], [dist_batch_statuses]
			 inner join [dist_batch_statuses_language] bcl on [dist_batch_statuses].dist_batch_statuses_id=bcl.dist_batch_statuses_id
			 where branch.branch_status_id=0 and [branch].branch_type_id = 0 
		 		) INTERX

			ON INTER.branch_id = INTERX.branch_id
				AND INTER.statuses_id = INTERX.statuses_id 				
	RIGHT join issuer on issuer.issuer_id=INTERX.issuer_id
	RIGHT join issuer_product on issuer_product.issuer_id=issuer.issuer_id
	WHERE INTERX.issuer_id = COALESCE(@issuer_id, INTERX.issuer_id)
		  AND INTERX.branch_id = COALESCE(@branch_id, INTERX.branch_id) and INTERX.language_id=@language_id
		  AND INTERX.statuses_id not in (17,18,19)
		  AND INTERX.branch_code <> ''  
		  AND issuer_product.product_id=COALESCE(@product_id, INTER.product_id)
	GROUP BY INTERX.issuer_id, INTERX.branch_id, branch_code, statuses_name, INTERX.statuses_id,issuer_name,issuer_product.product_code,issuer_product.product_name, issuer_product.product_id
	ORDER BY issuer_name, INTERX.branch_code, INTERX.statuses_id

END
