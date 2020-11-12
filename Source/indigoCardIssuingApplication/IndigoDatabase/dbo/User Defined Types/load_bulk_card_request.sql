﻿CREATE TYPE [dbo].[load_bulk_card_request] AS TABLE (
    [temp_customer_account_id]   BIGINT          NULL,
    [card_number]                VARCHAR (20)    NULL,
    [reference_number]           VARCHAR (100)   NULL,
    [branch_id]                  INT             NULL,
    [product_id]                 INT             NULL,
    [card_priority_id]           INT             NULL,
    [customer_account_number]    VARCHAR (30)    NULL,
    [domicile_branch_id]         INT             NULL,
    [account_type_id]            INT             NULL,
    [card_issue_reason_id]       INT             NULL,
    [customer_first_name]        VARCHAR (50)    NULL,
    [customer_middle_name]       VARCHAR (50)    NULL,
    [customer_last_name]         VARCHAR (50)    NULL,
    [name_on_card]               VARCHAR (100)   NULL,
    [customer_title_id]          INT             NULL,
    [currency_id]                INT             NULL,
    [resident_id]                INT             NULL,
    [customer_type_id]           INT             NULL,
    [cms_id]                     VARCHAR (50)    NULL,
    [contract_number]            VARCHAR (50)    NULL,
    [idnumber]                   VARCHAR (50)    NULL,
    [contact_number]             VARCHAR (50)    NULL,
    [customer_id]                VARCHAR (50)    NULL,
    [fee_waiver_YN]              BIT             NULL,
    [fee_editable_YN]            BIT             NULL,
    [fee_charged]                DECIMAL (10, 4) NULL,
    [fee_overridden_YN]          BIT             NULL,
    [audit_user_id]              BIGINT          NULL,
    [audit_workstation]          VARCHAR (100)   NULL,
    [sub_product_id]             VARCHAR (100)   NULL,
    [load_product_batch_type_id] INT             NULL,
    [already_loaded]             BIT             NULL);
