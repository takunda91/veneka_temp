CREATE PROCEDURE [dbo].[usp_get_product_service_request_code2]
AS
	BEGIN 
    SELECT src2_id, name
	FROM product_service_requet_code2
	END