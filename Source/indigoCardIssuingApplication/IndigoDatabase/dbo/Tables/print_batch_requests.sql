CREATE TABLE [dbo].[print_batch_requests]
(
	[print_batch_id] [BIGINT] NOT NULL,
	[request_id] [bigint] NOT NULL, 
    CONSTRAINT [PK_print_batch_requests_1] PRIMARY KEY ([print_batch_id], [request_id]), 
    CONSTRAINT [FK_print_batch_requests_hybrid_requests] FOREIGN KEY ([request_id]) REFERENCES [hybrid_requests]([request_id]), 
    CONSTRAINT [FK_print_batch_requests_print_batch] FOREIGN KEY ([print_batch_id]) REFERENCES [print_batch]([print_batch_id]),
	)