CREATE TABLE [dbo].[pin_reissue] (
    [issuer_id]            INT             NOT NULL,
    [branch_id]            INT             NOT NULL,
    [product_id]           INT             NOT NULL,
    [pan]                  VARBINARY (MAX) NOT NULL,
    [reissue_date]         DATETIMEOFFSET        NOT NULL,
    [operator_user_id]     BIGINT          NOT NULL,
    [authorise_user_id]    BIGINT          NULL,
    [failed]               BIT             NOT NULL,
    [notes]                VARCHAR (500)   NOT NULL,
    [pin_reissue_id]       BIGINT          IDENTITY (1, 1) NOT NULL,
    [mobile_number]        VARBINARY (MAX)    NULL,
    [primary_index_number] VARBINARY (MAX)    NULL,
    [request_expiry]       DATETIMEOFFSET (7) NOT NULL,
    [pin_reissue_type_id]  INT                NULL,
    CONSTRAINT [PK_pin_reissue_id] PRIMARY KEY CLUSTERED ([pin_reissue_id] ASC),
    CONSTRAINT [FK_pin_reissue_branch] FOREIGN KEY ([branch_id]) REFERENCES [dbo].[branch] ([branch_id]),
    CONSTRAINT [FK_pin_reissue_issuer] FOREIGN KEY ([issuer_id]) REFERENCES [dbo].[issuer] ([issuer_id]),
    CONSTRAINT [FK_pin_reissue_issuer_product] FOREIGN KEY ([product_id]) REFERENCES [dbo].[issuer_product] ([product_id]),
    CONSTRAINT [FK_pin_reissue_user] FOREIGN KEY ([operator_user_id]) REFERENCES [dbo].[user] ([user_id]),
    CONSTRAINT [FK_pin_reissue_user1] FOREIGN KEY ([authorise_user_id]) REFERENCES [dbo].[user] ([user_id])
);

