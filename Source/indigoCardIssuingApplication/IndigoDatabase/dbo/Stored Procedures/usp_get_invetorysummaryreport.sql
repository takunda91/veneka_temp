-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [usp_get_invetorysummaryreport] null,1
CREATE PROCEDURE [dbo].[usp_get_invetorysummaryreport]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int =null,
	@product_id int=null
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
	
DECLARE @UserTimezone as nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

	SELECT INTERX.issuer_id,
		   branch_code,
		   INTERX.branch_id,
		   INTERX.statuses_name,
		   INTERX.statuses_id,
		   COUNT(INTER.card_id)	AS CardCount,
		   issuer_code+'-'+ issuer_name as 'issuer_name',
		   product_code+'-'+product_name as product,
		   product_id
		   ,
CAST(SWITCHOFFSET(SYSDATETIMEOFFSET(),@UserTimezone) as datetime) as 'excuted_datetime'

	FROM 	 
		  (SELECT [branch].branch_id, [branch_card_status_current].branch_card_statuses_id as 'statuses_id', [branch_card_status_current].card_id,
		    issuer_product.product_code,issuer_product.product_name ,issuer_product.product_id
			FROM [branch_card_status_current]
					INNER JOIN [cards]
						ON [branch_card_status_current].card_id = [cards].card_id
					INNER JOIN [branch]
						ON [cards].branch_id = [branch].branch_id
							AND [branch].branch_type_id <> 0
					INNER JOIN [issuer_product] 
						ON [cards].product_id=[issuer_product].product_id
					WHERE    [branch].branch_status_id = 0
			) AS INTER
				RIGHT OUTER JOIN 			
			(SELECT issuer_id, branch_id, branch.branch_name+'-'+ branch_code as branch_code , [branch_card_statuses].branch_card_statuses_id as 'statuses_id', bcl.language_text as 'statuses_name',bcl.language_id
			 FROM	[branch], [branch_card_statuses]
			 inner join branch_card_statuses_language bcl on [branch_card_statuses].branch_card_statuses_id=bcl.branch_card_statuses_id
			 where branch.branch_status_id=0 AND [branch].branch_type_id <> 0 
		 		) INTERX

			ON INTER.branch_id = INTERX.branch_id
				AND INTER.statuses_id = INTERX.statuses_id 				
	inner join issuer on issuer.issuer_id=INTERX.issuer_id

	WHERE INTERX.issuer_id = COALESCE(@issuer_id, INTERX.issuer_id)
		  AND INTERX.branch_id = COALESCE(@branch_id, INTERX.branch_id) and INTERX.language_id=@language_id
		  AND INTERX.statuses_id!=6 and INTERX.statuses_id!=7
		  AND INTERX.branch_code <> ''  
		  AND product_id=COALESCE(@product_id, product_id)
	GROUP BY INTERX.issuer_id, INTERX.branch_id, branch_code, statuses_name, INTERX.statuses_id,issuer_name,issuer_code,product_code,product_name ,product_id
	ORDER BY issuer_name,issuer_code, INTERX.branch_code, INTERX.statuses_id

END