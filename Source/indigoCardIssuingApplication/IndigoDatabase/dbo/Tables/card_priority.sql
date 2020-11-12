CREATE TABLE [dbo].[card_priority] (
    [card_priority_id]    INT          NOT NULL,
    [card_priority_order] INT          NOT NULL,
    [card_priority_name]  VARCHAR (50) NOT NULL,
    [default_selection]   BIT          NOT NULL,
    CONSTRAINT [PK_card_priority] PRIMARY KEY CLUSTERED ([card_priority_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'card_priority'