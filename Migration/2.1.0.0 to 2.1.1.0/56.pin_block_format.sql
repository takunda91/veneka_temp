create table pin_block_format
(
pin_block_formatid int not null PRIMARY KEY,
pin_block_format nvarchar(50) not null

)


GO
insert  into pin_block_format values(0,'ISO9564-Format 0')
insert  into pin_block_format values(1,'ISO9564-Format 1')
