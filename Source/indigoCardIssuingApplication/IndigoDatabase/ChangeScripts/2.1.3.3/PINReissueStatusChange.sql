EXEC sp_rename 'pin_reissue_status', 'pin_reissue_status_audit'
GO
EXEC sp_rename 'PK_pin_reissue_status', 'PK_pin_reissue_status_audit'
GO

--DROP contraints
ALTER TABLE pin_reissue_status_audit DROP CONSTRAINT [FK_pin_reissue_status_pin_reissue];
ALTER TABLE pin_reissue_status_audit DROP CONSTRAINT [FK_pin_reissue_status_pin_reissue_statuses];

GO

--Creat current table
CREATE TABLE [dbo].[pin_reissue_status] (    
    [pin_reissue_id]          BIGINT         NOT NULL,
    [pin_reissue_statuses_id] INT            NOT NULL,
    [status_date]             DATETIMEOFFSET  NOT NULL,
    [user_id]                 BIGINT         NOT NULL,
    [audit_workstation]       VARCHAR (100)  NOT NULL,
    [comments]                VARCHAR (1000) NULL,
    CONSTRAINT [PK_pin_reissue_status] PRIMARY KEY CLUSTERED ([pin_reissue_id] ASC),
    CONSTRAINT [FK_pin_reissue_status_pin_reissue] FOREIGN KEY ([pin_reissue_id]) REFERENCES [dbo].[pin_reissue] ([pin_reissue_id]),
    CONSTRAINT [FK_pin_reissue_status_pin_reissue_statuses] FOREIGN KEY ([pin_reissue_statuses_id]) REFERENCES [dbo].[pin_reissue_statuses] ([pin_reissue_statuses_id])
);

--Load current records
INSERT INTO [pin_reissue_status] (pin_reissue_id, pin_reissue_statuses_id, status_date, [user_id], audit_workstation, comments)
SELECT        pin_reissue_id, pin_reissue_statuses_id, status_date, [user_id], audit_workstation, comments
FROM            dbo.pin_reissue_status_audit
WHERE        (status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.pin_reissue_status_audit AS bcs2
                               WHERE        (pin_reissue_id = dbo.pin_reissue_status_audit.pin_reissue_id)))

GO
-- Delete current status from audit - THIS MUST ONLY BE RUN ONCE
DELETE FROM dbo.pin_reissue_status_audit
WHERE pin_reissue_status_id IN (
SELECT        pin_reissue_status_id
FROM            dbo.pin_reissue_status_audit
WHERE        (status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.pin_reissue_status_audit AS bcs2
                               WHERE        (pin_reissue_id = dbo.pin_reissue_status_audit.pin_reissue_id))))
