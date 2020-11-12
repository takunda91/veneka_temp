CREATE TABLE [dbo].[pin_mailer] (
    [pin_mailer_reference] VARCHAR (50) NULL,
    [batch_reference]      VARCHAR (50) NOT NULL,
    [pin_mailer_status]    VARCHAR (25) NOT NULL,
    [card_number]          VARCHAR (19) NOT NULL,
    [pvv_offset]           VARCHAR (4)  NULL,
    [encrypted_pin]        VARCHAR (25) NULL,
    [customer_name]        VARCHAR (25) NOT NULL,
    [customer_address]     VARCHAR (50) NULL,
    [printed_date]         DATETIMEOFFSET     NULL,
    [reprinted_date]       DATETIMEOFFSET     NULL,
    [reprint_request_YN]   INT          NULL
);

