CREATE TABLE [dbo].[card_fee_charge_status]
(
	[card_fee_charge_status_id] INT NOT NULL , 
    [card_fee_charge_status_name] VARCHAR(100) NOT NULL, 
    CONSTRAINT [PK_card_fee_charge_status] PRIMARY KEY ([card_fee_charge_status_id])
)

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'card_fee_charge_status'