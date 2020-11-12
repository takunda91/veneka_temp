CREATE TABLE [dbo].[product_account_types_mapping]
([product_id] [int] NOT NULL,
	[cbs_account_type] [varchar](50) NULL,
	[indigo_account_type] VARCHAR(50) NULL,
	[cms_account_type] [varchar](50) NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[product_account_types_mapping]  WITH CHECK ADD  CONSTRAINT [FK_product_account_types_mapping_issuer_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO

ALTER TABLE [dbo].[product_account_types_mapping] CHECK CONSTRAINT [FK_product_account_types_mapping_issuer_product]
GO
