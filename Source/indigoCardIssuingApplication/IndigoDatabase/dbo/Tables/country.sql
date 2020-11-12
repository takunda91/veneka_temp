CREATE TABLE [dbo].[country] (
    [country_id]           INT           IDENTITY (1, 1) NOT NULL,
    [country_name]         VARCHAR (100) NOT NULL,
    [country_code]         VARCHAR (3)   NOT NULL,
    [country_capital_city] VARCHAR (100) NULL,
    CONSTRAINT [PK_country] PRIMARY KEY CLUSTERED ([country_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'country'