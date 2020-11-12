USE [indigo_database_main_dev]
GO

/****** Object:  Table [dbo].[issuer_interface]    Script Date: 2015-07-27 01:20:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[product_interface](
	[interface_type_id] [int] NOT NULL,
	[product_id] [int] NOT NULL,
	[connection_parameter_id] [int] NOT NULL,
	[interface_area] [int] NOT NULL,
	[interface_guid] [char](36) NULL,
 CONSTRAINT [PK_product_interface] PRIMARY KEY CLUSTERED 
(
	[interface_type_id] ASC,
	[product_id] ASC,
	[interface_area] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[product_interface]  WITH CHECK ADD  CONSTRAINT [FK_product_interface_connection_parameters] FOREIGN KEY([connection_parameter_id])
REFERENCES [dbo].[connection_parameters] ([connection_parameter_id])
GO

ALTER TABLE [dbo].[product_interface] CHECK CONSTRAINT [FK_product_interface_connection_parameters]
GO

ALTER TABLE [dbo].[product_interface]  WITH CHECK ADD  CONSTRAINT [FK_product_interface_interface_type] FOREIGN KEY([interface_type_id])
REFERENCES [dbo].[interface_type] ([interface_type_id])
GO

ALTER TABLE [dbo].[product_interface] CHECK CONSTRAINT [FK_product_interface_interface_type]
GO

ALTER TABLE [dbo].[product_interface]  WITH CHECK ADD  CONSTRAINT [FK_product_interface_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO

ALTER TABLE [dbo].[product_interface] CHECK CONSTRAINT [FK_product_interface_product]
GO


