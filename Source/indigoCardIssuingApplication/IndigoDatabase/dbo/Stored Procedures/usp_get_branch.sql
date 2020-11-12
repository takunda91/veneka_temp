CREATE PROC [dbo].[usp_get_branch]
			@branchName varchar(30),
			@branchCode varchar(10),
			@issuerID int,
			@is_card_centre bit = null
			
	AS 
	BEGIN 
	IF (@branchName='') OR (@branchName=null)
	BEGIN
		SET @branchName=NULL
	END
	
	IF (@branchCode='') OR (@branchCode=null)
	BEGIN
		SET @branchCode=NULL
	END
	
	
	
   SELECT * FROM branch 
   inner join branch_type on branch.branch_type_id=branch_type.branch_type_id
   WHERE branch_name = COALESCE(@branchName,branch_name)
    AND branch_code =COALESCE(@branchCode,branch_code)
    AND issuer_id =@issuerID
	--AND ((@is_card_centre is null) OR (card_centre_branch_YN = @is_card_centre))
	AND (branch_type.branch_type_id =COALESCE( @is_card_centre,branch_type.branch_type_id))

                           
                           
    END