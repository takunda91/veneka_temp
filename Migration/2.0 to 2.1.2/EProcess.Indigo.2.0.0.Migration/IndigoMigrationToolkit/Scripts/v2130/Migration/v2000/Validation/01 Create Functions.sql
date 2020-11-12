USE [indigo_database_group]
GO

--Compare cards
CREATE FUNCTION source_cards( )
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	SELECT [card_id]
			  ,[product_id]
			  ,[branch_id]
			  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, card_number)) as [card_number]
			  ,[card_sequence]		  
		FROM [dbo].[cards]
)
GO

CREATE FUNCTION source_customers( )
RETURNS TABLE 
AS
RETURN 
(
	SELECT [customer_account_id]
		  ,[user_id]
		  ,[card_id]
		  ,[card_issue_reason_id]
		  ,[account_type_id]
		  ,[currency_id]
		  ,[resident_id]
		  ,[customer_type_id]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [customer_account_number])) as [customer_account_number]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [customer_first_name])) as [customer_first_name]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [customer_middle_name])) as [customer_middle_name]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [customer_last_name])) as [customer_last_name]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [name_on_card])) as [name_on_card]
		  ,[date_issued]
		  ,[cms_id]
		  ,[contract_number]
		  ,[customer_title_id]
		  ,[internal_account_nr] as [CustomerId]
	  FROM [dbo].[customer_account] 
)
GO

CREATE FUNCTION source_users( )
RETURNS TABLE 
AS
RETURN 
(
SELECT  [user_id]
	       ,[user_status_id]
	       ,[user_gender_id]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [dbo].[user].[username])) as [username]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [first_name])) as [first_name]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [last_name])) as [last_name]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [dbo].[user].[password])) as [password]
	       ,[user_email]
	       ,[online]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [employee_id])) as [employee_id]
	       ,[last_login_date]
	       ,[last_login_attempt]
	       ,[number_of_incorrect_logins]
	       ,[last_password_changed_date]
	       ,[language_id]		   
	FROM [dbo].[user] 
)
GO



USE [indigo_database_v213]
GO

CREATE FUNCTION target_cards( )
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	SELECT [card_id]
			  ,[product_id]
			  ,[branch_id]
			  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, card_number)) as [card_number]
			  ,[card_sequence]		  
		FROM [dbo].[cards]
)
GO

CREATE FUNCTION target_customers( )
RETURNS TABLE 
AS
RETURN 
(
	SELECT [customer_account_id]
		  ,[user_id]
		  ,[card_id]
		  ,[card_issue_reason_id]
		  ,[account_type_id]
		  ,[currency_id]
		  ,[resident_id]
		  ,[customer_type_id]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [customer_account_number])) as [customer_account_number]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [customer_first_name])) as [customer_first_name]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [customer_middle_name])) as [customer_middle_name]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [customer_last_name])) as [customer_last_name]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [name_on_card])) as [name_on_card]
		  ,[date_issued]
		  ,[cms_id]
		  ,[contract_number]
		  ,[customer_title_id]
		  ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [CustomerId])) as [CustomerId]
	  FROM [dbo].[customer_account] 
)
GO

CREATE FUNCTION target_users( )
RETURNS TABLE 
AS
RETURN 
(
SELECT  [user_id]
	       ,[user_status_id]
	       ,[user_gender_id]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [dbo].[user].[username])) as [username]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [first_name])) as [first_name]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [last_name])) as [last_name]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [dbo].[user].[password])) as [password]
	       ,[user_email]
	       ,[online]
	       ,CONVERT(VARCHAR(MAX), DECRYPTBYKEYAUTOCERT(CERT_ID('Indigo_Certificate'), NULL, [employee_id])) as [employee_id]
	       ,[last_login_date]
	       ,[last_login_attempt]
	       ,[number_of_incorrect_logins]
	       ,[last_password_changed_date]	       
	       ,[language_id]		   
	FROM [dbo].[user] 
)
GO