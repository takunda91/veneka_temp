CREATE TABLE [dbo].[product_fields] (
    [product_field_id]    INT             IDENTITY (1, 1) NOT NULL,
    [product_id]          INT             NOT NULL,
    [field_name]          VARCHAR (100)   NOT NULL,
    [print_field_type_id] INT             NOT NULL,
    [X]                   DECIMAL (18, 2) NULL,
    [Y]                   DECIMAL (18, 2) NULL,
    [width]               DECIMAL (18, 2) NULL,
    [height]              DECIMAL (18, 2) NULL,
    [font]                VARCHAR (50)    NULL,
    [font_size]           INT             NULL,
    [mapped_name]         VARCHAR (MAX)   NULL,
    [editable]            BIT             NOT NULL,
    [deleted]             BIT             NOT NULL,
    [label]               VARCHAR (100)   NULL,
    [max_length]          INT             CONSTRAINT [DF_product_fields_max_length] DEFAULT ((1)) NOT NULL,
    [printable]           BIT             NOT NULL DEFAULT ((0)),
    [printside]           INT             NOT NULL DEFAULT ((2)),
    CONSTRAINT [PK_product_fields] PRIMARY KEY CLUSTERED ([product_field_id] ASC),
    CONSTRAINT [FK_product_fields_issuer_product] FOREIGN KEY ([product_id]) REFERENCES [dbo].[issuer_product] ([product_id]),
    CONSTRAINT [FK_product_fields_print_field_types] FOREIGN KEY ([print_field_type_id]) REFERENCES [dbo].[print_field_types] ([print_field_type_id])
);



