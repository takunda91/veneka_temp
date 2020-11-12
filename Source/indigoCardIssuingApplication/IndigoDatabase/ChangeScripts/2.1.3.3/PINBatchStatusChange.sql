EXEC sp_rename 'pin_batch_status', 'pin_batch_status_audit'
GO
EXEC sp_rename 'PK_pin_batch_status', 'PK_pin_batch_status_audit'
GO

--DROP contraints
ALTER TABLE pin_batch_status_audit DROP CONSTRAINT [FK_pin_batch_status_pin_batch_status];
ALTER TABLE pin_batch_status_audit DROP CONSTRAINT [FK_pin_batch_status_pin_batch_statuses];
ALTER TABLE pin_batch_status_audit DROP CONSTRAINT [FK_pin_batch_status_user];

GO

--Creat current table
CREATE TABLE [dbo].[pin_batch_status] (    
    [pin_batch_id]          BIGINT        NOT NULL,
    [pin_batch_statuses_id] INT           NOT NULL,
    [user_id]               BIGINT        NOT NULL,
    [status_date]           DATETIMEOFFSET      NOT NULL,
    [status_notes]          VARCHAR (250) NULL,
    CONSTRAINT [PK_pin_batch_status] PRIMARY KEY CLUSTERED ([pin_batch_id] ASC),
    CONSTRAINT [FK_pin_batch_status_pin_batch_status] FOREIGN KEY ([pin_batch_id]) REFERENCES [dbo].[pin_batch] ([pin_batch_id]),
    CONSTRAINT [FK_pin_batch_status_pin_batch_statuses] FOREIGN KEY ([pin_batch_statuses_id]) REFERENCES [dbo].[pin_batch_statuses] ([pin_batch_statuses_id]),
    CONSTRAINT [FK_pin_batch_status_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user] ([user_id])
);

--Load current records
INSERT INTO pin_batch_status (pin_batch_id, pin_batch_statuses_id, [user_id], status_date, status_notes)
SELECT         pin_batch_id, pin_batch_statuses_id, [user_id], status_date, status_notes
FROM            dbo.pin_batch_status_audit
WHERE        (status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.pin_batch_status_audit AS bcs2
                               WHERE        (pin_batch_id = dbo.pin_batch_status_audit.pin_batch_id)))

GO
-- Delete current status from audit - THIS MUST ONLY BE RUN ONCE
DELETE FROM dbo.pin_batch_status_audit
WHERE pin_batch_status_id IN (
SELECT        pin_batch_status_id
FROM            dbo.pin_batch_status_audit
WHERE        (status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.pin_batch_status_audit AS bcs2
                               WHERE        (pin_batch_id = dbo.pin_batch_status_audit.pin_batch_id))))
