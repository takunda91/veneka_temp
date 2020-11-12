CREATE TABLE [dbo].[card_fee_charge_status_language]
(
	[card_fee_charge_status_id] INT NOT NULL , 
    [langauge_text] NVARCHAR(250) NOT NULL, 
    [language_id] INT NOT NULL, 
    CONSTRAINT [FK_card_fee_charge_status_language_card_fee_charge_status] FOREIGN KEY ([card_fee_charge_status_id]) REFERENCES [card_fee_charge_status]([card_fee_charge_status_id]), 
    CONSTRAINT [PK_card_fee_charge_status_language] PRIMARY KEY ([card_fee_charge_status_id], [language_id])
)

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'card_fee_charge_status_language'