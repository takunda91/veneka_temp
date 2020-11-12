CREATE TABLE [dbo].[fileloader_status]
(
	[fileloader_status] [int] NOT NULL,
	[executed_datetime] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_fileloader_status] PRIMARY KEY CLUSTERED 
(
	[fileloader_status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
