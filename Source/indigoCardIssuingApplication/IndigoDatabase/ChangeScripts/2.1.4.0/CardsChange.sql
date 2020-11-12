CREATE TABLE [dbo].[fee_charged](
	[fee_id] [BIGINT] IDENTITY(1,1) NOT NULL ,
	[fee_charged] [DECIMAL](10, 4) NULL,
	[vat] [DECIMAL](7, 4) NULL,
	[vat_charged] [NUMERIC](21, 10) NULL,
	[total_charged] [NUMERIC](22, 10) NULL,
	[fee_waiver_YN] [BIT] NULL,
	[fee_editable_YN] [BIT] NULL,
	[fee_overridden_YN] [BIT] NULL,
	[fee_reference_number] [VARCHAR](100) NULL,
	[fee_reversal_ref_number] [VARCHAR](100) NULL,
	[operator_user_id] [BIGINT] NOT NULL,
	[fee_charge_status_id] [INT] NULL,
	[card_id] [bigint]
	 CONSTRAINT [PK_fee_charged] PRIMARY KEY CLUSTERED 
(
	[fee_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
	GO
	alter table dbo.[cards] add fee_id BIGINT
	GO
	ALTER TABLE [dbo].[cards]  WITH CHECK ADD  CONSTRAINT [FK_cards_fee_charged] FOREIGN KEY([fee_id])
REFERENCES [dbo].[fee_charged] ([fee_id])
GO
INSERT INTO [dbo].[fee_charged](fee_waiver_YN,fee_editable_YN,fee_charged,fee_overridden_YN,operator_user_id,card_id)
select  fee_waiver_YN,fee_editable_YN,fee_charged,fee_overridden_YN,-1,card_id from cards

GO
UPDATE [dbo].[cards] SET [dbo].[cards].fee_id=[dbo].[fee_charged].fee_id from [dbo].[cards] inner JOIN [dbo].[fee_charged]  on [dbo].[cards].card_id = [dbo].[fee_charged] .card_id
GO
ALTER TABLE [dbo].[fee_charged] DROP COLUMN card_id
GO
ALTER TABLE [dbo].[cards] DROP COLUMN    [vat_charged]
      ,[total_charged]
GO
alter table [dbo].[cards] Drop CONSTRAINT [FK_cards_card_fee_charge_status]
GO
--alter table [dbo].[cards] Drop CONSTRAINT [DF__cards__card_fee___4589517F]
Declare @constarintname varchar(100)
				SELECT @constarintname=o.name
FROM sys.objects o
WHERE o.name like  'DF__cards__card_fee%' 
  AND o.parent_object_id <> 0
  IF OBJECT_ID(@constarintname) IS NOT NULL  
 Exec ( 'alter table [dbo].[cards] Drop CONSTRAINT ' +@constarintname)
GO
ALTER TABLE [dbo].[cards] DROP COLUMN [fee_charged]
      ,[vat]
   
      ,[fee_waiver_YN]
      ,[fee_editable_YN]
      ,[fee_overridden_YN]
      ,[fee_reference_number]
      ,[fee_reversal_ref_number],[card_fee_charge_status_id];

GO
