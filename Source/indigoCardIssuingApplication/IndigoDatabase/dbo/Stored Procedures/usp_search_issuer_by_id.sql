CREATE PROCEDURE [dbo].[usp_search_issuer_by_id]
	@issuer_id int
AS
BEGIN
DECLARE @issuer_status_id varchar(20)

	SELECT * FROM issuer
	WHERE issuer_id = @issuer_id

END