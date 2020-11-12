CREATE TABLE [dbo].[issuer] (
    [issuer_id]                      INT              IDENTITY (1, 1) NOT NULL,
    [issuer_status_id]               INT              NOT NULL,
    [country_id]                     INT              NOT NULL,
    [issuer_name]                    VARCHAR (50)     NOT NULL,
    [issuer_code]                    VARCHAR (10)     NOT NULL,
    [instant_card_issue_YN]          BIT              NOT NULL,
    [maker_checker_YN]               BIT              NOT NULL,
    [license_file]                   VARCHAR (100)    NULL,
    [license_key]                    VARCHAR (1000)   NULL,
    [language_id]                    INT              NULL,
    [card_ref_preference]            BIT              NOT NULL,
    [classic_card_issue_YN]          BIT              NOT NULL,
    [enable_instant_pin_YN]          BIT              DEFAULT ((0)) NOT NULL,
    [authorise_pin_issue_YN]         BIT              DEFAULT ((0)) NOT NULL,
    [authorise_pin_reissue_YN]       BIT              DEFAULT ((0)) NOT NULL,
    [back_office_pin_auth_YN]        BIT              NOT NULL,
    [remote_token]                   UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [remote_token_expiry]            DATETIME         NULL,
    [allow_branches_to_search_cards] BIT              NULL,
    CONSTRAINT [PK_issuer] PRIMARY KEY CLUSTERED ([issuer_id] ASC),
    CONSTRAINT [FK_issuer_has_languages] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]),
    CONSTRAINT [FK_issuer_issuer_statuses] FOREIGN KEY ([issuer_status_id]) REFERENCES [dbo].[issuer_statuses] ([issuer_status_id])
);



