CREATE PROCEDURE [dbo].[usp_get_product_service_request_code1]
AS
	BEGIN 
    SELECT src1_id, name
	FROM product_service_requet_code1
	END