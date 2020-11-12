EXEC sp_rename 'load_batch_status', 'load_batch_status_audit'
GO
EXEC sp_rename 'PK_load_batch_status', 'PK_load_batch_status_audit'
GO

--DROP contraints
ALTER TABLE load_batch_status_audit DROP CONSTRAINT FK_load_batch_status_batch_statuses;
ALTER TABLE load_batch_status_audit DROP CONSTRAINT FK_load_batch_status_load_batch;
ALTER TABLE load_batch_status_audit DROP CONSTRAINT FK_load_batch_status_user;

GO

--Creat current table
CREATE TABLE [dbo].[load_batch_status] (    
    [load_batch_id]          BIGINT        NOT NULL,
    [load_batch_statuses_id] INT           NOT NULL,
    [user_id]                BIGINT        NOT NULL,
    [status_date]            DATETIMEOFFSET      NOT NULL,
    [status_notes]           VARCHAR (150) NULL,
    CONSTRAINT [PK_load_batch_status] PRIMARY KEY CLUSTERED ([load_batch_id] ASC),
    CONSTRAINT [FK_load_batch_status_batch_statuses] FOREIGN KEY ([load_batch_statuses_id]) REFERENCES [dbo].[load_batch_statuses] ([load_batch_statuses_id]),
    CONSTRAINT [FK_load_batch_status_load_batch] FOREIGN KEY ([load_batch_id]) REFERENCES [dbo].[load_batch] ([load_batch_id]),
    CONSTRAINT [FK_load_batch_status_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user] ([user_id])
);
GO

--Load current records
INSERT INTO [load_batch_status] ([load_batch_id], [load_batch_statuses_id], [user_id], status_date, status_notes)
SELECT        load_batch_id, load_batch_statuses_id, user_id, status_date, status_notes
FROM            dbo.load_batch_status_audit
WHERE        (status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.load_batch_status_audit AS bcs2
                               WHERE        (load_batch_id = dbo.load_batch_status_audit.load_batch_id)))

GO
-- Delete current status from audit - THIS MUST ONLY BE RUN ONCE
DELETE FROM dbo.load_batch_status_audit
WHERE load_batch_status_id IN (
SELECT        load_batch_status_id
FROM            dbo.load_batch_status_audit
WHERE        (status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.load_batch_status_audit AS bcs2
                               WHERE        (load_batch_id = dbo.load_batch_status_audit.load_batch_id))))