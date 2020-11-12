CREATE TABLE [dbo].[pin_calc_methods] (
    [pin_calc_method_id]   INT           NOT NULL,
    [pin_calc_method_name] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_pin_calc_method] PRIMARY KEY CLUSTERED ([pin_calc_method_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'pin_calc_methods'