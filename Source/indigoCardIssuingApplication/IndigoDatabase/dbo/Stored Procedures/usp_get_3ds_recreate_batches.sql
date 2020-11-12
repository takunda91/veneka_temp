-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Gets a list of 3D secure batches that are in re-create status for an issuer and interface
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_3ds_recreate_batches]
	@issuer_id INT,
	@interface_guid NCHAR(36), 	
	@language_id INT,
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
	-- Batches should have the same integration for all cards
	SELECT DISTINCT [dbo].[threed_secure_batch].[threed_batch_id], [batch_reference],Cast(SWITCHOFFSET([date_created],@UserTimezone) as datetime) as [date_created], [no_cards]
	FROM [dbo].[threed_secure_batch] INNER JOIN [dbo].[threed_secure_batch_status]
			ON [dbo].[threed_secure_batch_status].[threed_batch_id] = [dbo].[threed_secure_batch].[threed_batch_id]
		INNER JOIN [dbo].[threed_secure_batch_cards]
			ON [dbo].[threed_secure_batch_cards].[threed_batch_id] = [dbo].[threed_secure_batch].[threed_batch_id]
		INNER JOIN [dbo].[cards]
			ON [dbo].[cards].[card_id] = [dbo].[threed_secure_batch_cards].[card_id]
		--INNER JOIN [dbo].[issuer_product]
		--	ON [dbo].[issuer_product].[product_id] = [dbo].[cards].[product_id]
		INNER JOIN [dbo].[product_interface]
			ON [dbo].[product_interface].[product_id] = [dbo].[cards].[product_id]
	WHERE [dbo].[threed_secure_batch].[issuer_id] = @issuer_id
			AND [dbo].[threed_secure_batch_status].[threed_batch_statuses_id] = 3
			AND [dbo].[product_interface].[interface_guid] = @interface_guid

END