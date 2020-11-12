CREATE TABLE [dbo].[threed_secure_batch]
(
	[threed_batch_id] BIGINT NOT NULL  IDENTITY, 
    [date_created] DATETIMEOFFSET NOT NULL,       
    [batch_reference] NVARCHAR(200) NOT NULL, 
    [issuer_id] INT NOT NULL,     
    [no_cards] INT NOT NULL, 
    CONSTRAINT [PK_threed_secure_batch] PRIMARY KEY ([threed_batch_id]), 
    CONSTRAINT [FK_threed_secure_batch_to_issuer] FOREIGN KEY ([issuer_id]) REFERENCES [issuer]([issuer_id])
)
