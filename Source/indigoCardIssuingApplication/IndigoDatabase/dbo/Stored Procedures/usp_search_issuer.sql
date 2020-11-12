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