 
 IF COL_LENGTH('[dbo].[branch]', 'main_branch_id') IS NULL
BEGIN
 ALTER TABLE  [dbo].[branch]
 ADD main_branch_id int null
 END
 GO
  IF COL_LENGTH('[dbo].[branch]', 'branch_type_id') IS NULL
BEGIN
 ALTER TABLE  [dbo].[branch]	
 ADD  [branch_type_id] int NULL
 END
 GO
 Update [dbo].[branch] SET  [branch_type_id] =  CASE WHEN card_centre_branch_YN = 1 then  0 ELSE 1 end
 GO
 ALTER TABLE [dbo].[branch] DROP COLUMN card_centre_branch_YN;