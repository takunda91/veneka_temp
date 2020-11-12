CREATE TABLE [dbo].[connection_parameters_additionaldata] (
    [connection_parameter_id] INT           NOT NULL,
    [key]                     VARCHAR (50)  NULL,
    [value]                   VARCHAR (500) NULL,
    CONSTRAINT [FK_connection_parameters _additionaldata_connection_parameters] FOREIGN KEY ([connection_parameter_id]) REFERENCES [dbo].[connection_parameters] ([connection_parameter_id])
);

