-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE sp_get_card_priority_list
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [card_priority].card_priority_id, [card_priority].card_priority_order, 
			[card_priority_language].language_text AS card_priority_name, [card_priority].default_selection
	FROM [card_priority]
		INNER JOIN [card_priority_language]
			ON [card_priority].card_priority_id = [card_priority_language].card_priority_id
	WHERE [card_priority_language].language_id = @language_id
	ORDER BY [card_priority].card_priority_order

END
GO
