CREATE TABLE [dbo].[response_messages] (
    [system_response_code] INT           NOT NULL,
    [system_area]          INT           NOT NULL,
    [english_response]     VARCHAR (500) NOT NULL,
    [french_response]      VARCHAR (500) NOT NULL,
    [portuguese_response]  VARCHAR (500) NOT NULL,
    [spanish_response]     VARCHAR (500) NOT NULL,
    CONSTRAINT [PK_response_messages] PRIMARY KEY CLUSTERED ([system_response_code] ASC, [system_area] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'response_messages'