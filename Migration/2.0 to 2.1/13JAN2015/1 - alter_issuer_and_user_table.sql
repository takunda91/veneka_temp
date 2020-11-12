USE [indigo_database_main_dev]
GO

ALTER TABLE dbo.[user]
  	ADD [instant_authorisation_pin] [varbinary](256) NULL
	, [last_authorisation_pin_changed_date] [datetime] NULL
GO


ALTER TABLE dbo.[issuer]
	ADD [enable_instant_pin_YN] [bit] NOT NULL DEFAULT 0,
	 [authorise_pin_issue_YN] [bit] NOT NULL DEFAULT 0, 
	 [authorise_pin_reissue_YN] [bit] NOT NULL DEFAULT 0
GO
