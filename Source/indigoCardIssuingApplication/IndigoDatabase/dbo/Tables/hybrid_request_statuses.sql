CREATE TABLE [dbo].[hybrid_request_statuses](
	[hybrid_request_statuses_id] [int] NOT NULL,
	[hybrid_request_statuses] [varchar](100) NULL,
 CONSTRAINT [PK_hybrid_request_statuses] PRIMARY KEY CLUSTERED 
(
	[hybrid_request_statuses_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'hybrid_request_statuses'