CREATE PROCEDURE [dbo].[usp_fee_charge_update_status]
	@card_id bigint,
	@card_fee_charge_status_id int,
	@fee_reference_number varchar(100) = null,
	@fee_reversal_ref_number varchar(100) = null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE f    
	SET f.fee_charge_status_id = @card_fee_charge_status_id,
		f.fee_reference_number = COALESCE(f.fee_reference_number, @fee_reference_number),
		f.fee_reversal_ref_number = COALESCE(f.fee_reversal_ref_number, @fee_reversal_ref_number)
	from [dbo].fee_charged as f
	inner join [dbo].[cards] on [dbo].[cards].fee_id =f.fee_id	
	WHERE [card_id] = @card_id
