
CREATE VIEW [dbo].[branch_card_status_current]
AS
SELECT        --dbo.branch_card_status.branch_card_status_id, 
					dbo.branch_card_status.card_id, dbo.cards.card_priority_id,
					dbo.cards.product_id, dbo.cards.card_issue_method_id, dbo.cards.branch_id,
						 dbo.branch_card_status.branch_card_statuses_id, 
                         dbo.branch_card_status.status_date, dbo.branch_card_status.user_id, dbo.branch_card_status.operator_user_id, dbo.branch_card_status.branch_card_code_id, 
                         dbo.branch_card_status.comments, dbo.branch_card_codes.branch_card_code_id AS Expr1, dbo.branch_card_codes.branch_card_code_type_id, 
                         dbo.branch_card_codes.branch_card_code_name, dbo.branch_card_codes.branch_card_code_enabled, dbo.branch_card_codes.spoil_only, 
                         dbo.branch_card_codes.is_exception
FROM         dbo.cards 
				INNER JOIN
				   dbo.branch_card_status ON dbo.cards.card_id = dbo.branch_card_status.card_id
			   LEFT OUTER JOIN
                         dbo.branch_card_codes ON dbo.branch_card_status.branch_card_code_id = dbo.branch_card_codes.branch_card_code_id
--WHERE        (dbo.branch_card_status.status_date =
--                             (SELECT        MAX(status_date) AS Expr1
--                               FROM            dbo.branch_card_status AS bcs2
--                               WHERE        (card_id = dbo.branch_card_status.card_id)))
GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'branch_card_status_current';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[30] 4[20] 2[21] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "branch_card_status"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 135
               Right = 254
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "branch_card_codes"
            Begin Extent = 
               Top = 6
               Left = 292
               Bottom = 135
               Right = 524
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 15
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1815
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1380
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'branch_card_status_current';

