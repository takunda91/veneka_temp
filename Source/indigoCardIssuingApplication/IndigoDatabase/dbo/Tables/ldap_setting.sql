CREATE TABLE [dbo].[ldap_setting] (
    [ldap_setting_id]      INT             IDENTITY (1, 1) NOT NULL,
    [ldap_setting_name]    VARCHAR (100)   NOT NULL,
    [hostname_or_ip]       VARCHAR (200)   NOT NULL,
    [path]                 VARCHAR (200)   NOT NULL,
    [domain_name]          VARCHAR (100)   NULL,
    [username]             VARBINARY (MAX) NULL,
    [password]             VARBINARY (MAX) NULL,
    [auth_type_id]         INT             NULL,
    [external_inteface_id] CHAR (36)       NULL,
    CONSTRAINT [PK_ldap_setting] PRIMARY KEY CLUSTERED ([ldap_setting_id] ASC)
);

