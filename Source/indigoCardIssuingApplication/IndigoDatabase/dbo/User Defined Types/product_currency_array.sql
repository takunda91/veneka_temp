CREATE TYPE [dbo].[product_currency_array] AS TABLE (
    [currency_id]      INT           NULL,
    [is_base]          BIT           NULL,
    [usr_field_name_1] VARCHAR (250) NULL,
    [usr_field_val_1]  VARCHAR (250) NULL,
    [usr_field_name_2] VARCHAR (250) NULL,
    [usr_field_val_2]  VARCHAR (250) NULL);

