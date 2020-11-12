CREATE TABLE [dbo].[customer_account_requests]
(
	[customer_account_id] [bigint] NOT NULL,
	[request_id] [bigint] NOT NULL, 
    CONSTRAINT [PK_customer_account_requests] PRIMARY KEY ([request_id], [customer_account_id]), 
    CONSTRAINT [FK_customer_account_requests_customer_account] FOREIGN KEY ([customer_account_id]) REFERENCES [customer_account]([customer_account_id]), 
    CONSTRAINT [FK_customer_account_requests_hybrid_requests] FOREIGN KEY ([request_id]) REFERENCES [hybrid_requests]([request_id]),
)
