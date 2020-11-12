CREATE TABLE [dbo].[fee_charged](
	[fee_id] [bigint] IDENTITY(1,1) NOT NULL,
	[fee_charged] [decimal](10, 4) NULL,
	[vat] [decimal](7, 4) NULL,
	[vat_charged] [numeric](21, 10) NULL,
	[total_charged] [numeric](22, 10) NULL,
	[fee_waiver_YN] [bit] NULL,
	[fee_editable_YN] [bit] NULL,
	[fee_overridden_YN] [bit] NULL,
	[fee_reference_number] [varchar](100) NULL,
	[fee_reversal_ref_number] [varchar](100) NULL,
	[operator_user_id] [bigint] NOT NULL,
	[fee_charge_status_id] [int] NULL,
 CONSTRAINT [PK_fee_charged] PRIMARY KEY CLUSTERED 
(
	[fee_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

