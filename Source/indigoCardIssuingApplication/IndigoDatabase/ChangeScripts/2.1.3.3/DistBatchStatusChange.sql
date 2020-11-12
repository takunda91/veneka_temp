EXEC sp_rename 'dist_batch_status', 'dist_batch_status_audit'
GO
EXEC sp_rename 'PK_dist_batch_status', 'PK_dist_batch_status_audit'
GO

--DROP contraints
ALTER TABLE dist_batch_status_audit DROP CONSTRAINT FK_dist_batch_status_dist_batch_statuses;
ALTER TABLE dist_batch_status_audit DROP CONSTRAINT FK_dist_batch_status_distribution_batch;
ALTER TABLE dist_batch_status_audit DROP CONSTRAINT FK_dist_batch_status_user;

GO

--Creat current table
CREATE TABLE [dbo].[dist_batch_status] (    
    [dist_batch_id]          BIGINT        NOT NULL,
    [dist_batch_statuses_id] INT           NOT NULL,
    [user_id]                BIGINT        NOT NULL,
    [status_date]            DATETIMEOFFSET      NOT NULL,
    [status_notes]           VARCHAR (150) NULL,
    CONSTRAINT [PK_dist_batch_status] PRIMARY KEY CLUSTERED ([dist_batch_id] ASC),
    CONSTRAINT [FK_dist_batch_status_dist_batch_statuses] FOREIGN KEY ([dist_batch_statuses_id]) REFERENCES [dbo].[dist_batch_statuses] ([dist_batch_statuses_id]),
    CONSTRAINT [FK_dist_batch_status_distribution_batch] FOREIGN KEY ([dist_batch_id]) REFERENCES [dbo].[dist_batch] ([dist_batch_id]),
    CONSTRAINT [FK_dist_batch_status_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user] ([user_id])
);
GO

--Load current records
INSERT INTO [dist_batch_status] (dist_batch_id, dist_batch_statuses_id, [user_id], status_date, status_notes)
SELECT dist_batch_id, dist_batch_statuses_id, [user_id], status_date, status_notes
FROM            dbo.dist_batch_status_audit
WHERE        (status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.dist_batch_status_audit AS bcs2
                               WHERE        (dist_batch_id = dbo.dist_batch_status_audit.dist_batch_id)))

GO

-- Delete current status from audit - THIS MUST ONLY BE RUN ONCE
DELETE FROM dbo.dist_batch_status_audit
WHERE dist_batch_status_id IN (
SELECT dist_batch_status_id
FROM            dbo.dist_batch_status_audit
WHERE        (status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.dist_batch_status_audit AS bcs2
                               WHERE        (dist_batch_id = dbo.dist_batch_status_audit.dist_batch_id))))