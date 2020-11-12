USE [indigo_database_main_dev]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Check that a file parameter does not belong to more than one file interface.
-- =============================================
CREATE FUNCTION FileParameterValidation
(
	@connection_parameter_id int,
	@interface_guid char(36),
	@interface_type_id int
)
RETURNS bit
AS
BEGIN

	IF(@interface_type_id != 4)
		RETURN 1
	
	RETURN CAST(CASE WHEN EXISTS(SELECT * 
								FROM [product_interface] 
								WHERE connection_parameter_id = @connection_parameter_id 
								AND interface_guid != @interface_guid) 
		THEN 0 ELSE 1 END AS BIT)
END
GO

ALTER TABLE [product_interface]
    WITH CHECK ADD CONSTRAINT CK_FILE_PARAMETER
    CHECK (dbo.FileParameterValidation(connection_parameter_id, interface_guid, interface_type_id) = 1)
GO