CREATE TABLE [dbo].[pin_mailer_reprint_audit] (
    [pin_mailer_reprint_id]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [card_id]                      BIGINT         NOT NULL,
    [user_id]                      BIGINT         NOT NULL,
    [pin_mailer_reprint_status_id] INT            NOT NULL,
    [status_date]                  DATETIMEOFFSET       NOT NULL,
    [comments]                     VARCHAR (1000) NULL,
    CONSTRAINT [PK_pin_mailer_reprint_audit] PRIMARY KEY CLUSTERED ([pin_mailer_reprint_id] ASC)    
);

