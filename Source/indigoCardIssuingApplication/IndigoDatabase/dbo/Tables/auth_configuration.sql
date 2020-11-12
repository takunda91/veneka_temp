CREATE TABLE [dbo].[auth_configuration] (
    [authentication_configuration_id] INT           IDENTITY (1, 1) NOT NULL,
    [authentication_configuration]    VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_authentication_configuration] PRIMARY KEY CLUSTERED ([authentication_configuration_id] ASC)
);
