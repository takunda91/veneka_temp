USE [indigo_database_main_dev]
GO

ALTER TABLE [dbo].[integration_cardnumbers] DROP CONSTRAINT [FK_integration_cardnumbers_issuer_product]
GO

/****** Object:  Table [dbo].[integration_cardnumbers]    Script Date: 2014/10/11 04:44:45 PM ******/
DROP TABLE [dbo].[integration_cardnumbers]
GO

/****** Object:  Table [dbo].[integration_cardnumbers]    Script Date: 2014/10/11 04:44:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[integration_cardnumbers](
	[product_id] [int] NOT NULL,
	[card_sequence_number] varbinary(max) NOT NULL,
 CONSTRAINT [PK_integration_cardnumbers] PRIMARY KEY CLUSTERED 
(
	[product_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[integration_cardnumbers]  WITH CHECK ADD  CONSTRAINT [FK_integration_cardnumbers_issuer_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO

ALTER TABLE [dbo].[integration_cardnumbers] CHECK CONSTRAINT [FK_integration_cardnumbers_issuer_product]
GO


