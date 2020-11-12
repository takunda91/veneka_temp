CREATE TABLE [dbo].[printer] (
    [printer_id]       BIGINT             IDENTITY (1, 1) NOT NULL,
    [serial_no]        NVARCHAR (100)     NOT NULL,
    [manufacturer]     NVARCHAR (100)     NOT NULL,
    [model]            NVARCHAR (100)     NOT NULL,
    [firmware_version] NVARCHAR (100)     NULL,
    [branch_id]        INT                NOT NULL,
    [total_prints]     INT                NOT NULL,
    [next_clean]       INT                NOT NULL,
    [audit_user_id]    INT                NOT NULL,
    [last_update_date] DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK__printer__057FF7161B6BE8CE] PRIMARY KEY CLUSTERED ([printer_id] ASC)
);


