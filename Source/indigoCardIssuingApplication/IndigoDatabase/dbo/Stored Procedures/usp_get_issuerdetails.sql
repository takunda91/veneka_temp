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