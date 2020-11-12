CREATE TABLE [dbo].[languages] (
    [id]               INT            NOT NULL,
    [language_name]    NVARCHAR (MAX) NOT NULL,
    [language_name_fr] VARCHAR (100)  NOT NULL,
    [language_name_pt] VARCHAR (100)  NOT NULL,
    [language_name_sp] VARCHAR (100)  NOT NULL,
    CONSTRAINT [PK_languages] PRIMARY KEY CLUSTERED ([id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'languages'