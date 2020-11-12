-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Check that a file parameter does not belong to more than one file interface.
-- =============================================
CREATE FUNCTION [dbo].[FileParameterValidation]
(
	@connection_parameter_id int,
	@interface_guid char(36),
	@interface_type_id int
)
RETURNS bit
AS
BEGIN
	--Return true for all other interface types except for file type.
	IF(@interface_type_id != 4)
		RETURN 1
	
	--Make sure that the parameter associated with the interface has not been used by another interface
	RETURN CAST(CASE WHEN EXISTS(SELECT * 
								FROM [product_interface] 
								WHERE connection_parameter_id = @connection_parameter_id 
								AND interface_guid != @interface_guid) 
		THEN 0 ELSE 1 END AS BIT)
END