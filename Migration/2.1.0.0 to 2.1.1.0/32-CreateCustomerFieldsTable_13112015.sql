USE [indigo_database_main_dev]
GO

/****** Object:  Table [dbo].[customer_fields]    Script Date: 2015-11-20 09:35:53 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[customer_fields](
	[customer_account_id] [bigint] NOT NULL,
	[product_field_id] [int] NOT NULL,
	[value] [varbinary](max) NULL,
 CONSTRAINT [PK_customer_fields] PRIMARY KEY CLUSTERED 
(
	[customer_account_id] ASC,
	[product_field_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[customer_fields]  WITH CHECK ADD  CONSTRAINT [FK_customer_fields_customer_account] FOREIGN KEY([customer_account_id])
REFERENCES [dbo].[customer_account] ([customer_account_id])
GO

ALTER TABLE [dbo].[customer_fields] CHECK CONSTRAINT [FK_customer_fields_customer_account]
GO

ALTER TABLE [dbo].[customer_fields]  WITH CHECK ADD  CONSTRAINT [FK_customer_fields_product_fields] FOREIGN KEY([product_field_id])
REFERENCES [dbo].[product_fields] ([product_field_id])
GO

ALTER TABLE [dbo].[customer_fields] CHECK CONSTRAINT [FK_customer_fields_product_fields]
GO


