 SELECT 0 as [pin_reissue_status_id]
	  ,[pin_reissue_id]
      ,[pin_reissue_statuses_id]
      ,[status_date]
      ,[user_id]
      ,[audit_workstation]
      ,[comments]
  FROM [dbo].[pin_reissue_status]
  where [pin_reissue_id] = 26
UNION ALL
SELECT [pin_reissue_status_id]
	  ,[pin_reissue_id]
      ,[pin_reissue_statuses_id]
      ,[status_date]
      ,[user_id]
      ,[audit_workstation]
      ,[comments]
  FROM [dbo].[pin_reissue_status_audit]
	where [pin_reissue_id] = 26
ORDER BY [pin_reissue_id], [status_date]

--445823000000000000