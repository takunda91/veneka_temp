EXEC sp_rename 'export_batch_status', 'export_batch_status_audit'
GO
EXEC sp_rename 'PK_export_batch_status', 'PK_export_batch_status_audit'
GO

--DROP contraints
ALTER TABLE export_batch_status_audit DROP CONSTRAINT [FK_export_batch_status_export_batch];
ALTER TABLE export_batch_status_audit DROP CONSTRAINT [FK_export_batch_status_export_batch_statuses];
ALTER TABLE export_batch_status_audit DROP CONSTRAINT [FK_export_batch_status_user];

GO

--Creat current table
CREATE TABLE [dbo].[export_batch_status] (    
    [export_batch_id]          BIGINT        NOT NULL,
    [export_batch_statuses_id] INT           NOT NULL,
    [user_id]                  BIGINT        NOT NULL,
    [status_date]              DATETIMEOFFSET NOT NULL,
    [comments]                 VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_export_batch_status] PRIMARY KEY CLUSTERED ([export_batch_id] ASC),
    CONSTRAINT [FK_export_batch_status_export_batch] FOREIGN KEY ([export_batch_id]) REFERENCES [dbo].[export_batch] ([export_batch_id]),
    CONSTRAINT [FK_export_batch_status_export_batch_statuses] FOREIGN KEY ([export_batch_statuses_id]) REFERENCES [dbo].[export_batch_statuses] ([export_batch_statuses_id]),
    CONSTRAINT [FK_export_batch_status_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user] ([user_id])
);
GO

--Load current records
INSERT INTO [export_batch_status] ([export_batch_id], [export_batch_statuses_id], [user_id], [status_date], [comments])
SELECT        export_batch_id, export_batch_statuses_id, [user_id], status_date, comments
FROM            dbo.export_batch_status_audit
WHERE        (status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.export_batch_status_audit AS bcs2
                               WHERE        (export_batch_id = dbo.export_batch_status_audit.export_batch_id)))

GO
-- Delete current status from audit - THIS MUST ONLY BE RUN ONCE
DELETE FROM dbo.export_batch_status_audit
WHERE export_batch_status_id IN (
SELECT        [export_batch_status_id]
FROM            dbo.export_batch_status_audit
WHERE        (status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.export_batch_status_audit AS bcs2
                               WHERE        (export_batch_id = dbo.export_batch_status_audit.export_batch_id))))