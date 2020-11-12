CREATE TABLE [dbo].[hybrid_request_statuses_language](
	[hybrid_request_statuses_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NULL,
    CONSTRAINT [FK_hybrid_request_statuses_language_hybrid_request_status] FOREIGN KEY (hybrid_request_statuses_id) REFERENCES [hybrid_request_statuses](hybrid_request_statuses_id), 
    CONSTRAINT [PK_hybrid_request_statuses_language] PRIMARY KEY ([hybrid_request_statuses_id], [language_id]) 

 )

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'hybrid_request_statuses_language'