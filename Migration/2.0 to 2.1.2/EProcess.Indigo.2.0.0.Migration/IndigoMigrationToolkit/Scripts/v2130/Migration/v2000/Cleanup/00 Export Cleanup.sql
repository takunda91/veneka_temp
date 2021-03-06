USE [{SOURCE_DATABASE_NAME}]
GO
--Items needed on source DB

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
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_file_locations]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[migration_file_locations]
GO


--Items needed on target DB
USE [{DATABASE_NAME}]
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
           WHERE  object_id = OBJECT_ID(N'[dbo].[migration_file_locations]') AND type IN ( N'P'))
	DROP PROCEDURE [dbo].[migration_file_locations]
GO	
