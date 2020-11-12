CREATE TABLE [dbo].[product_fee_detail] (
    [fee_scheme_id]   INT           NOT NULL,
    [fee_detail_id]   INT           IDENTITY (1, 1) NOT NULL,
    [fee_detail_name] VARCHAR (100) NOT NULL,
    [effective_from]  DATETIMEOFFSET      NOT NULL,
    [fee_waiver_YN]   BIT           NOT NULL,
    [fee_editable_YN] BIT           NOT NULL,
    [deleted_yn]      BIT           NOT NULL,
    [effective_to]    DATETIMEOFFSET      NULL,
    CONSTRAINT [PK_fee_detail] PRIMARY KEY CLUSTERED ([fee_detail_id] ASC),
    CONSTRAINT [FK_fee_detail_fee_detail] FOREIGN KEY ([fee_scheme_id]) REFERENCES [dbo].[product_fee_scheme] ([fee_scheme_id])
);

