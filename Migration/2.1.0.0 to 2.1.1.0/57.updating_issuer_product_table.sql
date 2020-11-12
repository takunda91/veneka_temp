
update i set i.pin_block_format=p.pin_block_formatid from issuer_product i
inner join pin_block_format p on i.pin_block_format=p.pin_block_format
GO
EXEC sp_RENAME 'dbo.[issuer_product].pin_block_format' , 'pin_block_formatid', 'COLUMN'
GO
update issuer_product set pin_block_formatid=0 where  pin_block_formatid='IBM PIN Offset'
GO
ALTER TABLE dbo.issuer_product
   ALTER COLUMN pin_block_formatid int
   GO
   ALTER TABLE dbo.issuer_product
   add foreign key (pin_block_formatid) references pin_block_format  (pin_block_formatid)