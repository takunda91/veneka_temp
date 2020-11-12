USE [indigo_database_main_dev]
GO

/****** Object:  Table [dbo].[pin_reissue]    Script Date: 2015-04-24 04:56:04 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[pin_reissue](
	[issuer_id] [int] NOT NULL,
	[branch_id] [int] NOT NULL,
	[product_id] [int] NOT NULL,
	[pan] [varbinary](max) NOT NULL,
	[reissue_date] [datetime] NOT NULL,
	[operator_user_id] [bigint] NOT NULL,
	[authorise_user_id] [bigint] NULL,
	[failed] [bit] NOT NULL,
	[notes] [varchar](500) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[pin_reissue]  WITH CHECK ADD  CONSTRAINT [FK_pin_reissue_branch] FOREIGN KEY([branch_id])
REFERENCES [dbo].[branch] ([branch_id])
GO

ALTER TABLE [dbo].[pin_reissue] CHECK CONSTRAINT [FK_pin_reissue_branch]
GO

ALTER TABLE [dbo].[pin_reissue]  WITH CHECK ADD  CONSTRAINT [FK_pin_reissue_issuer] FOREIGN KEY([issuer_id])
REFERENCES [dbo].[issuer] ([issuer_id])
GO

ALTER TABLE [dbo].[pin_reissue] CHECK CONSTRAINT [FK_pin_reissue_issuer]
GO

ALTER TABLE [dbo].[pin_reissue]  WITH CHECK ADD  CONSTRAINT [FK_pin_reissue_issuer_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO

ALTER TABLE [dbo].[pin_reissue] CHECK CONSTRAINT [FK_pin_reissue_issuer_product]
GO

ALTER TABLE [dbo].[pin_reissue]  WITH CHECK ADD  CONSTRAINT [FK_pin_reissue_user] FOREIGN KEY([operator_user_id])
REFERENCES [dbo].[user] ([user_id])
GO

ALTER TABLE [dbo].[pin_reissue] CHECK CONSTRAINT [FK_pin_reissue_user]
GO

ALTER TABLE [dbo].[pin_reissue]  WITH CHECK ADD  CONSTRAINT [FK_pin_reissue_user1] FOREIGN KEY([authorise_user_id])
REFERENCES [dbo].[user] ([user_id])
GO

ALTER TABLE [dbo].[pin_reissue] CHECK CONSTRAINT [FK_pin_reissue_user1]
GO


