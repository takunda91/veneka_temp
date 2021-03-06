USE [{DATABASE_NAME}]
GO
--Items needed on source DB
IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[validation_products]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[validation_products]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[validation_connection_parameters]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[validation_connection_parameters]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[validation_ldap]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[validation_ldap]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[validation_users]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[validation_users]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[validation_user_password_history]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[validation_user_password_history]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[validation_cards]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[validation_cards]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[validation_customer_accounts]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[validation_customer_accounts]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[validation_customer_fields]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[validation_customer_fields]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[validation_integration_cardnumbers]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[validation_integration_cardnumbers]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[validation_masterkeys]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[validation_masterkeys]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[validation_terminals]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[validation_terminals]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[validation_pin_reissue]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[validation_pin_reissue]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[validation_zone_keys]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[validation_zone_keys]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[validation_product_fee_accounting]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[validation_product_fee_accounting]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[validation_integration_fields]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[validation_integration_fields]
GO