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