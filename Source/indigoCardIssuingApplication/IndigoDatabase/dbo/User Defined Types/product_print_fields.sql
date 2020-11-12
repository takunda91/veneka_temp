CREATE TYPE [dbo].[product_print_fields] AS TABLE (
    [product_field_id]    INT             NULL,
    [product_id]          INT             NULL,
    [field_name]          VARCHAR (100)   NULL,
    [print_field_type_id] INT             NULL,
    [X]                   DECIMAL (18, 2) NULL,
    [Y]                   DECIMAL (18, 2) NULL,
    [width]               DECIMAL (18, 2) NULL,
    [height]              DECIMAL (18, 2) NULL,
    [font]                VARCHAR (50)    NULL,
    [font_size]           INT             NULL,
    [mapped_name]         VARCHAR (MAX)   NULL,
    [editable]            BIT             NULL,
    [deleted]             BIT             NULL,
    [label]               VARCHAR (100)   NULL,
    [max_length]          INT             NULL,
    [printable]           BIT             NULL,
    [printside]           INT             NULL);



