USE [indigo_database_group]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[source_cards]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[source_cards]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[source_customers]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[source_customers]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[source_users]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[source_users]
GO

USE [indigo_database_v213]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[target_cards]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[target_cards]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[target_customers]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[target_customers]
GO

IF EXISTS (SELECT * FROM   [sys].[objects]
           WHERE  object_id = OBJECT_ID(N'[dbo].[target_users]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[target_users]
GO