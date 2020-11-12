CREATE TABLE [dbo].[hybrid_requests](
	[request_id] [bigint] IDENTITY(1,1) NOT NULL,
	[product_id] [int] NULL,
	[branch_id] [int] NULL,
	[request_reference] [varchar](100) NULL,
	[origin_branch_id] [int] NULL,
	[ordering_branch_id] [int] NULL,
	[delivery_branch_id] [int] NULL,
	[card_issue_method_id] [int] NOT NULL,
	[card_priority_id] [int] NULL,
	[fee_id] [bigint] NULL,
 CONSTRAINT [PK_hybrid_requests] PRIMARY KEY CLUSTERED 
(
	[request_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
