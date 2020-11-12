CREATE TABLE [dbo].[print_batch]
(
[print_batch_id] [bigint] IDENTITY(1,1) NOT NULL,
	[branch_id] [int] NOT NULL,
	[date_created] [datetimeoffset](7) NULL,
	[print_batch_reference] [varchar](50) NULL,
	[card_issue_method_id] [int] NOT NULL,
	[issuer_id] [int] NULL,
	[origin_branch_id] [int] NULL,
	[no_of_requests] [int] NULL, 
    CONSTRAINT [PK_print_batch_requests] PRIMARY KEY ([print_batch_id]), 
    CONSTRAINT [FK_print_batch_requests_branch] FOREIGN KEY (branch_id) REFERENCES [branch]([branch_id]), 
    CONSTRAINT [FK_print_batch_requests_origin_branch] FOREIGN KEY ([origin_branch_id]) REFERENCES [branch]([branch_id]),    
   CONSTRAINT [FK_print_batch_requests_issuer] FOREIGN KEY ([issuer_id]) REFERENCES [issuer]([issuer_id]),	
    CONSTRAINT [FK_print_batch_requests_card_issue_method] FOREIGN KEY (card_issue_method_id) REFERENCES [card_issue_method]([card_issue_method_id])
)


