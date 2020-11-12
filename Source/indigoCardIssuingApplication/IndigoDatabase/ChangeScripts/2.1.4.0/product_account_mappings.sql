IF NOT EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'product_account_types_mapping '
    AND COLUMN_NAME = 'account_type_id')
BEGIN
 EXEC sp_rename 'product_account_types_mapping.account_type_id', 'indigo_account_type';
END
