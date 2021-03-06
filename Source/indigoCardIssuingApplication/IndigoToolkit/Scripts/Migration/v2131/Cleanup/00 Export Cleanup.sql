USE [{SOURCE_DATABASE_NAME}]
GO
--Items needed on source DB
IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_products]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[migration_products]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_connection_parameters]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[migration_connection_parameters]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_ldap]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[migration_ldap]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_users]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[migration_users]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_user_password_history]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[migration_user_password_history]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_cards]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[migration_cards]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_customer_accounts]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[migration_customer_accounts]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_customer_fields]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[migration_customer_fields]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_integration_cardnumbers]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[migration_integration_cardnumbers]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_masterkeys]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[migration_masterkeys]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_terminals]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[migration_terminals]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_pin_reissue]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[migration_pin_reissue]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_zone_keys]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[migration_zone_keys]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_product_fee_accounting]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[migration_product_fee_accounting]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_integration_fields]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[migration_integration_fields]
GO


--Items needed on target DB
USE [{DATABASE_NAME}]
GO
IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_products]') AND type IN ( N'P'))
	DROP PROCEDURE [dbo].[migration_products]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_connection_parameters]') AND type IN ( N'P'))
	DROP PROCEDURE [dbo].[migration_connection_parameters]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_ldap]') AND type IN ( N'P'))
	DROP PROCEDURE [dbo].[migration_ldap]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_users]') AND type IN ( N'P'))
	DROP PROCEDURE [dbo].[migration_users]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_user_password_history]') AND type IN ( N'P'))
	DROP PROCEDURE [dbo].[migration_user_password_history]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_cards]') AND type IN ( N'P'))
	DROP PROCEDURE [dbo].[migration_cards]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_customer_accounts]') AND type IN ( N'P'))
	DROP PROCEDURE [dbo].[migration_customer_accounts]
GO	

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_customer_fields]') AND type IN ( N'P'))
	DROP PROCEDURE [dbo].[migration_customer_fields]
GO	

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_integration_cardnumbers]') AND type IN ( N'P'))
	DROP PROCEDURE [dbo].[migration_integration_cardnumbers]
GO	

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_masterkeys]') AND type IN ( N'P'))
	DROP PROCEDURE [dbo].[migration_masterkeys]
GO	

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_terminals]') AND type IN ( N'P'))
	DROP PROCEDURE [dbo].[migration_terminals]
GO	

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_pin_reissue]') AND type IN ( N'P'))
	DROP PROCEDURE [dbo].[migration_pin_reissue]
GO	

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_zone_keys]') AND type IN ( N'P'))
	DROP PROCEDURE [dbo].[migration_zone_keys]
GO	

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_product_fee_accounting]') AND type IN ( N'P'))
	DROP PROCEDURE [dbo].[migration_product_fee_accounting]
GO	

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_integration_fields]') AND type IN ( N'P'))
	DROP PROCEDURE [dbo].[migration_integration_fields]
GO	
