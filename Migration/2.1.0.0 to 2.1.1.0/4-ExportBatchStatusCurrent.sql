USE [indigo_database_main_dev]
GO

/****** Object:  View [dbo].[export_batch_status_current]    Script Date: 2015-06-22 01:50:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[export_batch_status_current]
AS
SELECT        export_batch_status_id, export_batch_id, export_batch_statuses_id, user_id, status_date, comments
FROM            dbo.export_batch_status
WHERE        (status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.export_batch_status AS bcs2
                               WHERE        (export_batch_id = dbo.export_batch_status.export_batch_id)))

GO

