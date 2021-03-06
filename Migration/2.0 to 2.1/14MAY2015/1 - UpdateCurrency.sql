USE [indigo_database_main_dev]
GO
ALTER TABLE customer_account NOCHECK CONSTRAINT ALL
GO
ALTER TABLE [currency] NOCHECK CONSTRAINT ALL
GO
ALTER TABLE product_fee_charge NOCHECK CONSTRAINT ALL
GO
ALTER TABLE product_currency NOCHECK CONSTRAINT ALL

GO
ALTER TABLE [currency]
	ADD iso_4217_numeric_code char(3)
GO

ALTER TABLE [currency]
	ADD iso_4217_minor_unit int
GO

ALTER TABLE [currency]
	ADD currency_desc varchar(100)
GO

ALTER TABLE [currency]
	ADD active_YN bit
GO
DELETE FROM [currency]
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (0, N'GHS', N'936', 2, N'Ghana Cedi', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (1, N'USD', N'840', 2, N'US Dollar', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (2, N'GBP', N'826', 2, N'Pound Sterling', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (3, N'EUR', N'978', 2, N'Euro', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (4, N'XOF', N'952', 0, N'CFA Franc BCEAO', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (5, N'BIF', N'108', 0, N'Burundi Franc', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (6, N'CDF', N'976', 2, N'Congolese Franc', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (7, N'CVE', N'132', 2, N'Cabo Verde Escudo', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (8, N'GMD', N'270', 2, N'Dalasi', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (9, N'GNF', N'324', 0, N'Guinea Franc', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (10, N'KES', N'404', 2, N'Kenyan Shilling', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (11, N'LRD', N'430', 2, N'Liberian Dollar', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (12, N'MWK', N'454', 2, N'Kwacha', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (13, N'NGN', N'566', 2, N'Naira', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (14, N'RWF', N'646', 0, N'Rwanda Franc', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (15, N'SLL', N'694', 2, N'Leone', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (16, N'SSP', N'728', 2, N'South Sudanese Pound', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (17, N'STD', N'678', 2, N'Dobra', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (18, N'TZS', N'834', 2, N'Tanzanian Shilling', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (19, N'UGX', N'800', 0, N'Uganda Shilling', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (20, N'XAF', N'950', 0, N'CFA Franc BEAC', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (21, N'ZMW', N'967', 2, N'Zambian Kwacha', 1)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (22, N'AED', N'784', 2, N'UAE Dirham', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (23, N'AFN', N'971', 2, N'Afghani', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (24, N'ALL', N'008', 2, N'Lek', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (25, N'AMD', N'051', 2, N'Armenian Dram', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (26, N'ANG', N'532', 2, N'Netherlands Antillean Guilder', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (27, N'AOA', N'973', 2, N'Kwanza', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (28, N'ARS', N'032', 2, N'Argentine Peso', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (29, N'AUD', N'036', 2, N'Australian Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (30, N'AWG', N'533', 2, N'Aruban Florin', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (31, N'AZN', N'944', 2, N'Azerbaijanian Manat', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (32, N'BAM', N'977', 2, N'Convertible Mark', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (33, N'BBD', N'052', 2, N'Barbados Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (34, N'BDT', N'050', 2, N'Taka', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (35, N'BGN', N'975', 2, N'Bulgarian Lev', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (36, N'BHD', N'048', 3, N'Bahraini Dinar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (37, N'BMD', N'060', 2, N'Bermudian Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (38, N'BND', N'096', 2, N'Brunei Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (39, N'BOB', N'068', 2, N'Boliviano', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (40, N'BOV', N'984', 2, N'Mvdol', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (41, N'BRL', N'986', 2, N'Brazilian Real', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (42, N'BSD', N'044', 2, N'Bahamian Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (43, N'BTN', N'064', 2, N'Ngultrum', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (44, N'BWP', N'072', 2, N'Pula', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (45, N'BYR', N'974', 0, N'Belarussian Ruble', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (46, N'BZD', N'084', 2, N'Belize Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (47, N'CAD', N'124', 2, N'Canadian Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (48, N'CHE', N'947', 2, N'WIR Euro', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (49, N'CHF', N'756', 2, N'Swiss Franc', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (50, N'CHW', N'948', 2, N'WIR Franc', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (51, N'CLF', N'990', 4, N'Unidad de Fomento', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (52, N'CLP', N'152', 0, N'Chilean Peso', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (53, N'CNY', N'156', 2, N'Yuan Renminbi', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (54, N'COP', N'170', 2, N'Colombian Peso', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (55, N'COU', N'970', 2, N'Unidad de Valor Real', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (56, N'CRC', N'188', 2, N'Costa Rican Colon', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (57, N'CUC', N'931', 2, N'Peso Convertible', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (58, N'CUP', N'192', 2, N'Cuban Peso', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (59, N'CZK', N'203', 2, N'Czech Koruna', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (60, N'DJF', N'262', 0, N'Djibouti Franc', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (61, N'DKK', N'208', 2, N'Danish Krone', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (62, N'DOP', N'214', 2, N'Dominican Peso', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (63, N'DZD', N'012', 2, N'Algerian Dinar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (64, N'EGP', N'818', 2, N'Egyptian Pound', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (65, N'ERN', N'232', 2, N'Nakfa', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (66, N'ETB', N'230', 2, N'Ethiopian Birr', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (67, N'FJD', N'242', 2, N'Fiji Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (68, N'FKP', N'238', 2, N'Falkland Islands Pound', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (69, N'GEL', N'981', 2, N'Lari', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (70, N'GIP', N'292', 2, N'Gibraltar Pound', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (71, N'GTQ', N'320', 2, N'Quetzal', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (72, N'GYD', N'328', 2, N'Guyana Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (73, N'HKD', N'344', 2, N'Hong Kong Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (74, N'HNL', N'340', 2, N'Lempira', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (75, N'HRK', N'191', 2, N'Croatian Kuna', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (76, N'HTG', N'332', 2, N'Gourde', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (77, N'HUF', N'348', 2, N'Forint', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (78, N'IDR', N'360', 2, N'Rupiah', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (79, N'ILS', N'376', 2, N'New Israeli Sheqel', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (80, N'INR', N'356', 2, N'Indian Rupee', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (81, N'INR', N'356', 2, N'Indian Rupee', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (82, N'IQD', N'368', 3, N'Iraqi Dinar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (83, N'IRR', N'364', 2, N'Iranian Rial', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (84, N'ISK', N'352', 0, N'Iceland Krona', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (85, N'JMD', N'388', 2, N'Jamaican Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (86, N'JOD', N'400', 3, N'Jordanian Dinar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (87, N'JPY', N'392', 0, N'Yen', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (88, N'KGS', N'417', 2, N'Som', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (89, N'KHR', N'116', 2, N'Riel', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (90, N'KMF', N'174', 0, N'Comoro Franc', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (91, N'KPW', N'408', 2, N'North Korean Won', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (92, N'KRW', N'410', 0, N'Won', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (93, N'KWD', N'414', 3, N'Kuwaiti Dinar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (94, N'KYD', N'136', 2, N'Cayman Islands Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (95, N'KZT', N'398', 2, N'Tenge', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (96, N'LAK', N'418', 2, N'Kip', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (97, N'LBP', N'422', 2, N'Lebanese Pound', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (98, N'LKR', N'144', 2, N'Sri Lanka Rupee', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (99, N'LSL', N'426', 2, N'Loti', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (100, N'LYD', N'434', 3, N'Libyan Dinar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (101, N'MAD', N'504', 2, N'Moroccan Dirham', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (102, N'MDL', N'498', 2, N'Moldovan Leu', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (103, N'MGA', N'969', 2, N'Malagasy Ariary', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (104, N'MKD', N'807', 2, N'Denar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (105, N'MMK', N'104', 2, N'Kyat', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (106, N'MNT', N'496', 2, N'Tugrik', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (107, N'MOP', N'446', 2, N'Pataca', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (108, N'MRO', N'478', 2, N'Ouguiya', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (109, N'MUR', N'480', 2, N'Mauritius Rupee', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (110, N'MVR', N'462', 2, N'Rufiyaa', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (111, N'MXN', N'484', 2, N'Mexican Peso', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (112, N'MXV', N'979', 2, N'Mexican Unidad de Inversion (UDI)', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (113, N'MYR', N'458', 2, N'Malaysian Ringgit', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (114, N'MZN', N'943', 2, N'Mozambique Metical', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (115, N'NAD', N'516', 2, N'Namibia Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (116, N'NIO', N'558', 2, N'Cordoba Oro', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (117, N'NOK', N'578', 2, N'Norwegian Krone', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (118, N'NOK', N'578', 2, N'Norwegian Krone', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (119, N'NOK', N'578', 2, N'Norwegian Krone', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (120, N'NPR', N'524', 2, N'Nepalese Rupee', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (121, N'NZD', N'554', 2, N'New Zealand Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (122, N'OMR', N'512', 3, N'Rial Omani', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (123, N'PAB', N'590', 2, N'Balboa', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (124, N'PEN', N'604', 2, N'Nuevo Sol', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (125, N'PGK', N'598', 2, N'Kina', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (126, N'PHP', N'608', 2, N'Philippine Peso', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (127, N'PKR', N'586', 2, N'Pakistan Rupee', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (128, N'PLN', N'985', 2, N'Zloty', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (129, N'PYG', N'600', 0, N'Guarani', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (130, N'QAR', N'634', 2, N'Qatari Rial', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (131, N'RON', N'946', 2, N'New Romanian Leu', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (132, N'RSD', N'941', 2, N'Serbian Dinar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (133, N'RUB', N'643', 2, N'Russian Ruble', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (134, N'SAR', N'682', 2, N'Saudi Riyal', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (135, N'SBD', N'090', 2, N'Solomon Islands Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (136, N'SCR', N'690', 2, N'Seychelles Rupee', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (137, N'SDG', N'938', 2, N'Sudanese Pound', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (138, N'SEK', N'752', 2, N'Swedish Krona', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (139, N'SGD', N'702', 2, N'Singapore Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (140, N'SHP', N'654', 2, N'Saint Helena Pound', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (141, N'SOS', N'706', 2, N'Somali Shilling', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (142, N'SRD', N'968', 2, N'Surinam Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (143, N'SVC', N'222', 2, N'El Salvador Colon', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (144, N'SYP', N'760', 2, N'Syrian Pound', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (145, N'SZL', N'748', 2, N'Lilangeni', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (146, N'THB', N'764', 2, N'Baht', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (147, N'TJS', N'972', 2, N'Somoni', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (148, N'TMT', N'934', 2, N'Turkmenistan New Manat', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (149, N'TND', N'788', 3, N'Tunisian Dinar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (150, N'TOP', N'776', 2, N'Pa’anga', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (151, N'TRY', N'949', 2, N'Turkish Lira', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (152, N'TTD', N'780', 2, N'Trinidad and Tobago Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (153, N'TWD', N'901', 2, N'New Taiwan Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (154, N'UAH', N'980', 2, N'Hryvnia', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (155, N'USN', N'997', 2, N'US Dollar(Next Day)', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (156, N'UYI', N'940', 0, N'Uruguay Peso en Unidades Indexadas (URUIURUI)', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (157, N'UYU', N'858', 2, N'Peso Uruguayo', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (158, N'UZS', N'860', 2, N'Uzbekistan Sum', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (159, N'VEF', N'937', 2, N'Bolivar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (160, N'VND', N'704', 0, N'Dong', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (161, N'VUV', N'548', 0, N'Vatu', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (162, N'WST', N'882', 2, N'Tala', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (163, N'XAG', N'961', NULL, N'Silver', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (164, N'XAU', N'959', NULL, N'Gold', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (165, N'XBA', N'955', NULL, N'Bond Markets Unit European Composite Unit (EURCO)', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (166, N'XBB', N'956', NULL, N'Bond Markets Unit European Monetary Unit (E.M.U.-6)', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (167, N'XBC', N'957', NULL, N'Bond Markets Unit European Unit of Account 9 (E.U.A.-9)', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (168, N'XBD', N'958', NULL, N'Bond Markets Unit European Unit of Account 17 (E.U.A.-17)', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (169, N'XCD', N'951', 2, N'East Caribbean Dollar', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (170, N'XDR', N'960', NULL, N'SDR (Special Drawing Right)', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (171, N'XPD', N'964', NULL, N'Palladium', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (172, N'XPF', N'953', 0, N'CFP Franc', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (173, N'XPT', N'962', NULL, N'Platinum', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (174, N'XSU', N'994', NULL, N'Sucre', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (175, N'XTS', N'963', NULL, N'Codes specifically reserved for testing purposes', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (176, N'XUA', N'965', NULL, N'ADB Unit of Account', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (177, N'XXX', N'999', NULL, N'The codes assigned for transactions where no currency is involved', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (178, N'YER', N'886', 2, N'Yemeni Rial', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (179, N'ZAR', N'710', 2, N'Rand', 0)
GO
INSERT [dbo].[currency] ([currency_id], [currency_code], [iso_4217_numeric_code], [iso_4217_minor_unit], [currency_desc], [active_YN]) VALUES (180, N'ZWL', N'932', 2, N'Zimbabwe Dollar', 0)
GO
ALTER TABLE [currency]
	ALTER COLUMN iso_4217_numeric_code char(3) NOT NULL
GO

ALTER TABLE [currency]
	ALTER COLUMN currency_desc varchar(100) NOT NULL
GO

ALTER TABLE [currency]
	ALTER COLUMN active_YN bit NOT NULL
GO

ALTER TABLE customer_account CHECK CONSTRAINT ALL
ALTER TABLE [currency] CHECK CONSTRAINT ALL
ALTER TABLE product_fee_charge CHECK CONSTRAINT ALL
ALTER TABLE product_currency CHECK CONSTRAINT ALL
GO