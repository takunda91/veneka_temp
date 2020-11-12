CREATE TABLE [dbo].[user] (
    [user_id]                             BIGINT             IDENTITY (1, 1) NOT NULL,
    [user_status_id]                      INT                NOT NULL,
    [user_gender_id]                      INT                NOT NULL,
    [username]                            VARBINARY (256)    NOT NULL,
    [first_name]                          VARBINARY (256)    NOT NULL,
    [last_name]                           VARBINARY (256)    NOT NULL,
    [password]                            VARBINARY (256)    NOT NULL,
    [user_email]                          VARCHAR (100)      NOT NULL,
    [online]                              BIT                CONSTRAINT [DF_user_online] DEFAULT ((0)) NOT NULL,
    [employee_id]                         VARBINARY (256)    NULL,
    [last_login_date]                     DATETIMEOFFSET (7) NULL,
    [last_login_attempt]                  DATETIMEOFFSET (7) NULL,
    [number_of_incorrect_logins]          INT                NULL,
    [last_password_changed_date]          DATETIMEOFFSET (7) NULL,
    [workstation]                         NVARCHAR (100)     NULL,
    [language_id]                         INT                NULL,
    [username_index]                      VARBINARY (20)     NULL,
    [instant_authorisation_pin]           VARBINARY (256)    NULL,
    [last_authorisation_pin_changed_date] DATETIMEOFFSET (7) NULL,
    [time_zone_utcoffset]                 NVARCHAR (50)      NULL,
    [time_zone_id]                        VARCHAR (MAX)      NULL,
    [useradmin_user_id]                   BIGINT             NULL,
    [record_datetime]                     DATETIMEOFFSET (7) NULL,
    [approved_user_id]                    BIGINT             NULL,
    [approved_datetime]                   DATETIMEOFFSET (7) NULL,
    [authentication_configuration_id]     INT                NULL,
    CONSTRAINT [PK_application_user] PRIMARY KEY CLUSTERED ([user_id] ASC)
);







