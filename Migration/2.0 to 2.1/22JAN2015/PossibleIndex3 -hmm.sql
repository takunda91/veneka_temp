/*
Missing Index Details from SQLQuery16.sql - veneka-test.indigo_database (VENEKA\rbrenchley (57))
The Query Processor estimates that implementing the following index could improve the query cost by 59.9372%.
*/

/*
USE [indigo_database]
GO
CREATE NONCLUSTERED INDEX [<Name of Missing Index, sysname,>]
ON [dbo].[dist_batch_cards] ([dist_card_status_id])
INCLUDE ([dist_batch_id],[card_id])
GO
*/
