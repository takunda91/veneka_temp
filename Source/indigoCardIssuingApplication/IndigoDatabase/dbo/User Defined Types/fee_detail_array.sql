CREATE TYPE [dbo].[fee_detail_array] AS TABLE (
    [fee_scheme_id]   INT           NULL,
    [fee_detail_id]   INT           NULL,
    [fee_detail_name] VARCHAR (100) NULL,
    [fee_waiver_YN]   BIT           NULL,
    [fee_editable_TN] BIT           NULL);

