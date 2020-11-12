CREATE TYPE [dbo].[product_external_fields_array] AS TABLE (
    [external_system_field_id] INT           NULL,
    [field_name]               VARCHAR (250) NULL,
    [field_value]              VARCHAR (250) NULL);

