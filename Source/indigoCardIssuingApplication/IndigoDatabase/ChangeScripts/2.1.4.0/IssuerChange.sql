
IF NOT EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'issuer'
    AND COLUMN_NAME = 'allow_branches_to_search_cards')
BEGIN
 ALTER Table [dbo].[issuer]
  ADD  allow_branches_to_search_cards  BIT  NOT NULL DEFAULT(0)
  update [dbo].[issuer] set allow_branches_to_search_cards=0
END
ELSE
BEGIN
update [dbo].[issuer] set allow_branches_to_search_cards=0
ALTER TABLE [dbo].[issuer]
ALTER COLUMN   allow_branches_to_search_cards  BIT  NOT NULL 
END
GO
