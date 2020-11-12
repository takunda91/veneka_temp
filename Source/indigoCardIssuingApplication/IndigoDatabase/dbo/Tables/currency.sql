CREATE TABLE [dbo].[currency] (
    [currency_id]           INT           NOT NULL,
    [currency_code]         CHAR (3)      NOT NULL,
    [iso_4217_numeric_code] CHAR (3)      NOT NULL,
    [iso_4217_minor_unit]   INT           NULL,
    [currency_desc]         VARCHAR (100) NOT NULL,
    [active_YN]             BIT           NOT NULL, 
    CONSTRAINT [PK_currency] PRIMARY KEY ([currency_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'currency'