USE [indigo_database_main_dev]
GO

CREATE TABLE [dbo].[product_service_requet_code1](
	[src1_id] [int] NOT NULL,
	[name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_product_service_requet_code1] PRIMARY KEY CLUSTERED 
(
	[src1_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[product_service_requet_code2](
	[src2_id] [int] NOT NULL,
	[name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_product_service_requet_code2] PRIMARY KEY CLUSTERED 
(
	[src2_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[product_service_requet_code3](
	[src3_id] [int] NOT NULL,
	[name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_product_service_requet_code3] PRIMARY KEY CLUSTERED 
(
	[src3_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO product_service_requet_code1 (src1_id, name)
	VALUES (1, 'International card')
INSERT INTO product_service_requet_code1 (src1_id, name)
	VALUES (2, 'International card - integrated circuit facilities')
INSERT INTO product_service_requet_code1 (src1_id, name)
	VALUES (5, 'National use only')
INSERT INTO product_service_requet_code1 (src1_id, name)
	VALUES (6, 'National use only - integrated circuit facilities')
INSERT INTO product_service_requet_code1 (src1_id, name)
	VALUES (9, 'Test card - online authorization mandatory')
GO

INSERT INTO product_service_requet_code2 (src2_id, name)
	VALUES (0, 'Normal authorization')
INSERT INTO product_service_requet_code2 (src2_id, name)
	VALUES (2, 'Online authorization mandatory')
INSERT INTO product_service_requet_code2 (src2_id, name)
	VALUES (4, 'Online authorization mandatory')
GO

INSERT INTO product_service_requet_code3 (src3_id, name)
	VALUES (0, 'PIN required')
INSERT INTO product_service_requet_code3 (src3_id, name)
	VALUES (1, 'No restrictions - normal cardholder verification')
INSERT INTO product_service_requet_code3 (src3_id, name)
	VALUES (2, 'Goods and services only')
INSERT INTO product_service_requet_code3 (src3_id, name)
	VALUES (3, 'PIN required, ATM only')
INSERT INTO product_service_requet_code3 (src3_id, name)
	VALUES (5, 'PIN required, goods and services only at POS, cash at ATM')
INSERT INTO product_service_requet_code3 (src3_id, name)
	VALUES (6, 'PIN required if PIN pad present')
INSERT INTO product_service_requet_code3 (src3_id, name)
	VALUES (7, 'PIN required if PIN pad present, goods and services only at POS, cash at ATM')
GO

ALTER TABLE issuer_product
	ADD expiry_months int,
		src1_id int REFERENCES product_service_requet_code1 (src1_id),
		src2_id int REFERENCES product_service_requet_code2 (src2_id),
		src3_id int REFERENCES product_service_requet_code3 (src3_id),
		PVK varbinary(max),
		PVKI varbinary(max),		
		CVKA varbinary(max),
		CVKB varbinary(max)
GO

UPDATE issuer_product
	SET src1_id = 6,
		src2_id = 0,
		src3_id = 6

ALTER TABLE issuer_product
	ALTER COLUMN src1_id int NOT NULL

ALTER TABLE issuer_product
	ALTER COLUMN src2_id int NOT NULL

ALTER TABLE issuer_product
	ALTER COLUMN src3_id int NOT NULL