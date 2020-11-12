CREATE PROCEDURE [dbo].[usp_get_billing_report] 
	@year int,
	@month int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    

OPEN SYMMETRIC KEY Indigo_Symmetric_Key
DECRYPTION BY CERTIFICATE Indigo_Certificate;


--DECLARE @pins_issued TABLE
--(
--	pan varchar(30) PRIMARY KEY,
--	pin_date datetimeoffset(7) not null,
--	[pin_set_count] int not null
--)

--INSERT INTO @pins_issued (pan, pin_date, [pin_set_count])
--SELECT CONVERT(VARCHAR(MAX),DECRYPTBYKEY([pin_reissue].pan)) as pan, MIN([pin_reissue_status].status_date) as status_date, COUNT([pin_reissue].pin_reissue_id) as cnt
--FROM [pin_reissue] INNER JOIN [pin_reissue_status] ON [pin_reissue_status].pin_reissue_id = [pin_reissue].pin_reissue_id
--WHERE [pin_reissue_status].pin_reissue_statuses_id = 3
--	AND  DATEDIFF(MONTH, [pin_reissue_status].status_date, DATEFROMPARTS(@year, @month, 1)) = 0
--GROUP BY CONVERT(VARCHAR(MAX),DECRYPTBYKEY([pin_reissue].pan)) 


DECLARE @cards_issued TABLE
(
	card_id bigint PRIMARY KEY,
	status_date datetimeoffset(7) not null,
	branch_id int not null,
	[user_id] bigint not null,
	branch_card_statuses_id int
	

)

INSERT INTO @cards_issued (card_id, status_date, branch_id, [user_id],branch_card_statuses_id)
SELECT [branch_card_status].card_id, status_date, branch_id, [user_id],branch_card_statuses_id
FROM [branch_card_status]
WHERE [branch_card_status].branch_card_statuses_id = 6
	AND  DATEDIFF(MONTH, [branch_card_status].status_date, DATEFROMPARTS(@year, @month, 1)) = 0
UNION
SELECT [branch_card_status_audit].card_id, branch_card_status_current.status_date, [branch_card_status_audit].branch_id, [branch_card_status_audit].[user_id],branch_card_status_current.branch_card_statuses_id
FROM [branch_card_status_audit]
inner join branch_card_status_current on [branch_card_status_audit].card_id=branch_card_status_current.card_id
WHERE [branch_card_status_audit].branch_card_statuses_id = 6
	AND  DATEDIFF(MONTH, [branch_card_status_audit].status_date, DATEFROMPARTS(@year, @month, 1)) = 0

DECLARE @cards_issued_detail TABLE
(
	card_id bigint PRIMARY KEY,
	branch_name varchar(250) not null,
	status_date datetimeoffset(7) not null,
	card_number varchar(30) not null,
	last_4_digits varchar(4) not null,
	account_number varchar(250) not null,
	customer_first_name varchar(250) not null,
	customer_middle_name varchar(250) not null,
	customer_last_name varchar(250) not null,
	branch_card_statuses varchar(250)  
)

INSERT INTO @cards_issued_detail (card_id, branch_name, status_date, card_number, last_4_digits, account_number, customer_first_name, customer_middle_name, customer_last_name,branch_card_statuses)
SELECT [cards_issued].card_id
	,branch_name 
	,[cards_issued].status_date as status_date
	,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)) as card_number
	,SUBSTRING(CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)), 16, 4) as last_4_digits
	,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_account_number)) as account_number
	,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_first_name)) as customer_first_name
	,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_middle_name)) as customer_middle_name
	,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_last_name)) as customer_last_name
	,branch_card_statuses.branch_card_statuses_name as 'branch_card_statuses'
FROM [cards] INNER JOIN @cards_issued as [cards_issued] ON [cards_issued].card_id = [cards].card_id
				INNER JOIN [branch] ON [branch].branch_id = [cards_issued].branch_id
						INNER JOIN [customer_account_cards] ON [customer_account_cards].card_id = [cards].card_id
			INNER JOIN [customer_account]
			ON [customer_account].customer_account_id = [customer_account_cards].customer_account_id
				inner join branch_card_statuses on [cards_issued].branch_card_statuses_id=branch_card_statuses.branch_card_statuses_id

CLOSE SYMMETRIC KEY Indigo_Symmetric_Key


SELECT @year as [issued_year]
		, @month as [issued_month]
		,branch_name as 'branch'
		--,card_number
		,last_4_digits as 'last 4 digits'
		,account_number as 'account number'
		,customer_first_name as 'customer first name'
		,customer_middle_name as 'customer middle name'
		,customer_last_name as 'customer last name',
		branch_card_statuses as 'current status'	
		
		,CAST(SWITCHOFFSET(status_date,'+00:00') as datetime) as 'status_date'
		--,pin_date			
		--,[pin_set_count]
		
FROM @cards_issued_detail [cards_issued_detail] 


END
