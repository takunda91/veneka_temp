CREATE TABLE [dbo].[workstation_keys]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[workstation] [varbinary](max) NOT NULL,
	[key] [varbinary](max) NULL,
	[additional_data] [varbinary](max) NULL
 CONSTRAINT [PK_workstation_keys] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
))
