EXEC sp_rename 'pin_mailer_reprint', 'pin_mailer_reprint_audit'
GO
EXEC sp_rename 'PK_pin_mailer_reprint', 'PK_pin_mailer_reprint_audit'
GO

--DROP contraints
ALTER TABLE pin_mailer_reprint_audit DROP CONSTRAINT [FK_pin_mailer_reprint_cards];
ALTER TABLE pin_mailer_reprint_audit DROP CONSTRAINT [FK_pin_mailer_reprint_pin_mailer_reprint];
ALTER TABLE pin_mailer_reprint_audit DROP CONSTRAINT [FK_pin_mailer_reprint_user];

GO

--Creat current table
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
GO

--Load current records
INSERT INTO [pin_mailer_reprint] ([card_id], pin_mailer_reprint_status_id, status_date, [user_id], comments)
SELECT        dbo.pin_mailer_reprint_audit.card_id,
						 dbo.pin_mailer_reprint_audit.pin_mailer_reprint_status_id, 
                         dbo.pin_mailer_reprint_audit.status_date, dbo.pin_mailer_reprint_audit.[user_id],
                         dbo.pin_mailer_reprint_audit.comments
FROM     dbo.pin_mailer_reprint_audit
WHERE        (dbo.pin_mailer_reprint_audit.status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.pin_mailer_reprint_audit AS bcs2
                               WHERE        (card_id = dbo.pin_mailer_reprint_audit.card_id)))

GO
-- Delete current status from audit - THIS MUST ONLY BE RUN ONCE
DELETE FROM dbo.pin_mailer_reprint_audit
WHERE pin_mailer_reprint_id IN (
SELECT        dbo.pin_mailer_reprint_audit.pin_mailer_reprint_id
FROM     dbo.pin_mailer_reprint_audit
WHERE        (dbo.pin_mailer_reprint_audit.status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.pin_mailer_reprint_audit AS bcs2
                               WHERE        (card_id = dbo.pin_mailer_reprint_audit.card_id))))
