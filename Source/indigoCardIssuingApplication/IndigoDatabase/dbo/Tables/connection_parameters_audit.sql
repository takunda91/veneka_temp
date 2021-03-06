﻿CREATE TABLE [dbo].[connection_parameters_audit] (
    [connection_parameter_id]      INT             IDENTITY (1, 1) NOT NULL,
    [connection_name]              VARCHAR (100)   NOT NULL,
    [address]                      VARCHAR (200)   NOT NULL,
    [port]                         INT             NOT NULL,
    [path]                         VARCHAR (200)   NOT NULL,
    [protocol]                     INT             NULL,
    [auth_type]                    INT             NOT NULL,
    [username]                     VARBINARY (MAX) NOT NULL,
    [password]                     VARBINARY (MAX) NOT NULL,
    [connection_parameter_type_id] INT             NOT NULL,
    [header_length]                INT             NULL,
    [identification]               VARBINARY (MAX) NULL,
    [timeout_milli]                INT             NULL,
    [buffer_size]                  INT             NULL,
    [doc_type]                     CHAR (1)        NULL,
    [name_of_file]                 VARCHAR (100)   NULL,
    [file_delete_YN]               BIT             NULL,
    [file_encryption_type_id]      INT             NULL,
    [duplicate_file_check_YN]      BIT             NULL,
    [private_key]                  VARBINARY (MAX) NULL,
    [public_key]                   VARBINARY (MAX) NULL,
    [domain_name]                  VARCHAR (100)   NULL,
    [is_external_auth]             BIT             NULL,
    [remote_port]                  INT             NULL,
    [remote_username]              VARBINARY (MAX) NULL,
    [remote_password]              VARBINARY (MAX) NULL,
    [nonce]                        VARBINARY (MAX) NULL
);

