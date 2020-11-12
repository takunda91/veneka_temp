CREATE TABLE [dbo].[issuer_statuses_language] (
    [issuer_status_id] INT           NOT NULL,
    [language_id]      INT           NOT NULL,
    [language_text]    VARCHAR (100) NOT NULL,
  CONSTRAINT [FK_issuer_statuses_issuer_status_id]  FOREIGN KEY ([issuer_status_id]) REFERENCES [dbo].[issuer_statuses] ([issuer_status_id]),
 CONSTRAINT [FK_issuer_statuses_language_language_id]   FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]), 
    CONSTRAINT [PK_issuer_statuses_language] PRIMARY KEY ([issuer_status_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'issuer_statuses_language'