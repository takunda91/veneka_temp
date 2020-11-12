

/****** Object:  Table [dbo].[product_fields]    Script Date: 2015-11-20 09:38:26 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[product_fields](
	[product_field_id] [int] IDENTITY(1,1) NOT NULL,
	[product_id] [int] NOT NULL,
	[field_name] [varchar](100) NULL,
	[print_field_type_id] [int] NULL,
	[X] [decimal](18, 2) NULL,
	[Y] [decimal](18, 2) NULL,
	[width] [decimal](18, 2) NULL,
	[height] [decimal](18, 2) NULL,
	[font] [varchar](50) NULL,
	[font_size] [int] NULL,
	[mapped_name] [varchar](max) NULL,
	[editable] [bit] NULL,
	[deleted] [bit] NULL,
	[label] [varchar](100) NULL,
	[max_length] [int] NOT NULL,
 CONSTRAINT [PK_product_fields] PRIMARY KEY CLUSTERED 
(
	[product_field_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[product_fields]  WITH CHECK ADD  CONSTRAINT [FK_product_fields_issuer_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO

ALTER TABLE [dbo].[product_fields] CHECK CONSTRAINT [FK_product_fields_issuer_product]
GO

ALTER TABLE [dbo].[product_fields]  WITH CHECK ADD  CONSTRAINT [FK_product_fields_print_field_types] FOREIGN KEY([print_field_type_id])
REFERENCES [dbo].[print_field_types] ([print_field_type_id])
GO

ALTER TABLE [dbo].[product_fields] CHECK CONSTRAINT [FK_product_fields_print_field_types]
GO


