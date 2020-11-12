USE [indigo_database_main_dev]
GO

/****** Object:  Table [dbo].[rswitch_crf_bank_codes]    Script Date: 2015/04/01 01:51:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[rswitch_crf_bank_codes](
	[bank_id] [int] IDENTITY(1,1) NOT NULL,
	[issuer_id] [int] NOT NULL,
	[bank_code] [varchar](2) NULL,
 CONSTRAINT [PK_bank] PRIMARY KEY CLUSTERED 
(
	[bank_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[rswitch_crf_bank_codes]  WITH CHECK ADD  CONSTRAINT [FK_rswitch_crf_bank_codes_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO

ALTER TABLE [dbo].[rswitch_crf_bank_codes] CHECK CONSTRAINT [FK_rswitch_crf_bank_codes_issuer]
GO


