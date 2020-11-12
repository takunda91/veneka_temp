CREATE TABLE [dbo].[pin_mailer_reprint] (    
    [card_id]                      BIGINT         NOT NULL,
    [user_id]                      BIGINT         NOT NULL,
    [pin_mailer_reprint_status_id] INT            NOT NULL,
    [status_date]                  DATETIMEOFFSET       NOT NULL,
    [comments]                     VARCHAR (1000) NULL,
    CONSTRAINT [PK_pin_mailer_reprint] PRIMARY KEY CLUSTERED ([card_id] ASC),
    CONSTRAINT [FK_pin_mailer_reprint_cards] FOREIGN KEY ([card_id]) REFERENCES [dbo].[cards] ([card_id]),
    CONSTRAINT [FK_pin_mailer_reprint_pin_mailer_reprint] FOREIGN KEY ([pin_mailer_reprint_status_id]) REFERENCES [dbo].[pin_mailer_reprint_statuses] ([pin_mailer_reprint_status_id]),
    CONSTRAINT [FK_pin_mailer_reprint_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user] ([user_id])
);

