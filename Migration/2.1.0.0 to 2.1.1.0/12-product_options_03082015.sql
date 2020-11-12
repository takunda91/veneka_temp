USE [indigo_database_main_dev]
GO

/****** Object:  Table [dbo].[product_issue_reason]    Script Date: 2015-08-03 09:59:07 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[product_issue_reason](
	[product_id] [int] NOT NULL,
	[card_issue_reason_id] [int] NOT NULL,
 CONSTRAINT [PK_product_issue_reason] PRIMARY KEY CLUSTERED 
(
	[product_id] ASC,
	[card_issue_reason_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[product_issue_reason]  WITH CHECK ADD  CONSTRAINT [FK_product_issue_reason_card_issue_reason] FOREIGN KEY([card_issue_reason_id])
REFERENCES [dbo].[card_issue_reason] ([card_issue_reason_id])
GO

ALTER TABLE [dbo].[product_issue_reason] CHECK CONSTRAINT [FK_product_issue_reason_card_issue_reason]
GO

ALTER TABLE [dbo].[product_issue_reason]  WITH CHECK ADD  CONSTRAINT [FK_product_issue_reason_issuer_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO

ALTER TABLE [dbo].[product_issue_reason] CHECK CONSTRAINT [FK_product_issue_reason_issuer_product]
GO


/****** Object:  Table [dbo].[products_account_types]    Script Date: 2015-08-03 10:06:44 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[products_account_types](
	[product_id] [int] NOT NULL,
	[account_type_id] [int] NOT NULL,
 CONSTRAINT [PK_products_account_types] PRIMARY KEY CLUSTERED 
(
	[product_id] ASC,
	[account_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[products_account_types]  WITH CHECK ADD  CONSTRAINT [FK_products_account_types_customer_account_type] FOREIGN KEY([account_type_id])
REFERENCES [dbo].[customer_account_type] ([account_type_id])
GO

ALTER TABLE [dbo].[products_account_types] CHECK CONSTRAINT [FK_products_account_types_customer_account_type]
GO

ALTER TABLE [dbo].[products_account_types]  WITH CHECK ADD  CONSTRAINT [FK_products_account_types_issuer_product] FOREIGN KEY([product_id])
REFERENCES [dbo].[issuer_product] ([product_id])
GO

ALTER TABLE [dbo].[products_account_types] CHECK CONSTRAINT [FK_products_account_types_issuer_product]
GO

--ADD additional fields
ALTER TABLE [issuer_product]
ADD pan_length smallint
GO

ALTER TABLE [issuer_product]
ADD sub_product_code varchar(4)
GO

ALTER TABLE [issuer_product]
ADD pin_calc_method_id int
GO

ALTER TABLE [issuer_product]
ADD file_delete_YN bit
GO

ALTER TABLE [issuer_product]
ADD auto_approve_batch_YN bit
GO

ALTER TABLE [issuer_product]
ADD account_validation_YN bit
GO

ALTER TABLE [issuer_product]
ADD pin_mailer_printing_YN bit
GO

ALTER TABLE [issuer_product]
ADD pin_mailer_reprint_YN bit
GO

UPDATE [issuer_product]
SET 
	pan_length = 16
	,pin_calc_method_id = 0
	,file_delete_YN = 0
	,auto_approve_batch_YN = 0
	,account_validation_YN = 0
	,pin_mailer_printing_YN = 0
	,pin_mailer_reprint_YN = 0
	

GO

ALTER TABLE [issuer_product]
ALTER COLUMN pan_length smallint not null
GO

ALTER TABLE [issuer_product]
ALTER COLUMN pin_calc_method_id int not null
GO

ALTER TABLE [issuer_product]
ALTER COLUMN file_delete_YN bit not null
GO

ALTER TABLE [issuer_product]
ALTER COLUMN auto_approve_batch_YN bit not null
GO

ALTER TABLE [issuer_product]
ALTER COLUMN account_validation_YN bit not null
GO

ALTER TABLE [issuer_product]
ALTER COLUMN pin_mailer_printing_YN bit not null
GO

ALTER TABLE [issuer_product]
ALTER COLUMN pin_mailer_reprint_YN bit not null
GO

--/****** Object:  Table [dbo].[products_card_issue_methods]    Script Date: 2015-08-03 10:47:27 AM ******/
--SET ANSI_NULLS ON
--GO

--SET QUOTED_IDENTIFIER ON
--GO

--CREATE TABLE [dbo].[products_card_issue_methods](
--	[product_id] [int] NOT NULL,
--	[card_issue_method_id] [int] NOT NULL,
-- CONSTRAINT [PK_products_card_issue_methods] PRIMARY KEY CLUSTERED 
--(
--	[product_id] ASC,
--	[card_issue_method_id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

--GO

--ALTER TABLE [dbo].[products_card_issue_methods]  WITH CHECK ADD  CONSTRAINT [FK_products_card_issue_methods_card_issue_method] FOREIGN KEY([card_issue_method_id])
--REFERENCES [dbo].[card_issue_method] ([card_issue_method_id])
--GO

--ALTER TABLE [dbo].[products_card_issue_methods] CHECK CONSTRAINT [FK_products_card_issue_methods_card_issue_method]
--GO

--ALTER TABLE [dbo].[products_card_issue_methods]  WITH CHECK ADD  CONSTRAINT [FK_products_card_issue_methods_issuer_product] FOREIGN KEY([product_id])
--REFERENCES [dbo].[issuer_product] ([product_id])
--GO

--ALTER TABLE [dbo].[products_card_issue_methods] CHECK CONSTRAINT [FK_products_card_issue_methods_issuer_product]
--GO

-- PIN CALC METHODS-------------------------------------------------------------------------------

/****** Object:  Table [dbo].[pin_calc_methods]    Script Date: 2015-08-03 10:59:13 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[pin_calc_methods](
	[pin_calc_method_id] [int] NOT NULL,
	[pin_calc_method_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_pin_calc_method] PRIMARY KEY CLUSTERED 
(
	[pin_calc_method_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

INSERT INTO [pin_calc_methods] (pin_calc_method_id, pin_calc_method_name)
VALUES (0, 'ISO9564-FORMAT 0')
GO

ALTER TABLE [issuer_product]
ADD CONSTRAINT FK_issuer_product_pin_calc_methods
	FOREIGN KEY (pin_calc_method_id) REFERENCES [pin_calc_methods](pin_calc_method_id)