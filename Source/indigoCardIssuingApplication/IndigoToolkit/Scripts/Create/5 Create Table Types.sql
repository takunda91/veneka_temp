USE [{DATABASE_NAME}]
GO
/****** Object:  UserDefinedTableType [dbo].[auth_configuration_interface]    Script Date: 2018-09-13 10:11:43 AM ******/
CREATE TYPE [dbo].[auth_configuration_interface] AS TABLE(
	[authentication_configuration_id] [int] NULL,
	[auth_type_id] [int] NULL,
	[connection_parameter_id] [int] NULL,
	[interface_guid] [char](36) NULL,
	[external_interface_id] [char](36) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[bi_key_binary_value_array]    Script Date: 2018-09-13 10:11:43 AM ******/
CREATE TYPE [dbo].[bi_key_binary_value_array] AS TABLE(
	[key1] [bigint] NULL,
	[key2] [bigint] NULL,
	[value] [image] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[bi_key_varchar_value_array]    Script Date: 2018-09-13 10:11:43 AM ******/
CREATE TYPE [dbo].[bi_key_varchar_value_array] AS TABLE(
	[key1] [varchar](50) NULL,
	[value] [varchar](500) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[bikey_value_array]    Script Date: 2018-09-13 10:11:43 AM ******/
CREATE TYPE [dbo].[bikey_value_array] AS TABLE(
	[key1] [bigint] NULL,
	[key2] [bigint] NULL,
	[value] [varchar](max) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[branch_id_array]    Script Date: 2018-09-13 10:11:43 AM ******/
CREATE TYPE [dbo].[branch_id_array] AS TABLE(
	[branch_id] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[card_id_array]    Script Date: 2018-09-13 10:11:43 AM ******/
CREATE TYPE [dbo].[card_id_array] AS TABLE(
	[card_id] [bigint] NULL,
	[branch_card_statuses_id] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[card_numbers_array]    Script Date: 2018-09-13 10:11:43 AM ******/
CREATE TYPE [dbo].[card_numbers_array] AS TABLE(
	[card_number] [varchar](100) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[currency_id_array]    Script Date: 2018-09-13 10:11:43 AM ******/
CREATE TYPE [dbo].[currency_id_array] AS TABLE(
	[Currency_id] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[DistBatchCards]    Script Date: 2018-09-13 10:11:43 AM ******/
CREATE TYPE [dbo].[DistBatchCards] AS TABLE(
	[card_number] [varchar](20) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[fee_charge_array]    Script Date: 2018-09-13 10:11:43 AM ******/
CREATE TYPE [dbo].[fee_charge_array] AS TABLE(
	[currency_id] [int] NULL,
	[fee_charge] [decimal](10, 4) NULL,
	[vat] [decimal](7, 4) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[fee_detail_array]    Script Date: 2018-09-13 10:11:43 AM ******/
CREATE TYPE [dbo].[fee_detail_array] AS TABLE(
	[fee_scheme_id] [int] NULL,
	[fee_detail_id] [int] NULL,
	[fee_detail_name] [varchar](100) NULL,
	[fee_waiver_YN] [bit] NULL,
	[fee_editable_TN] [bit] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[key_binary_value_array]    Script Date: 2018-09-13 10:11:44 AM ******/
CREATE TYPE [dbo].[key_binary_value_array] AS TABLE(
	[key] [bigint] NULL,
	[value] [image] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[key_value_array]    Script Date: 2018-09-13 10:11:44 AM ******/
CREATE TYPE [dbo].[key_value_array] AS TABLE(
	[key] [bigint] NULL,
	[value] [varchar](max) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[load_bulk_card_request]    Script Date: 2018-09-13 10:11:44 AM ******/
CREATE TYPE [dbo].[load_bulk_card_request] AS TABLE(
	[temp_customer_account_id] [bigint] NULL,
	[card_number] [varchar](20) NULL,
	[reference_number] [varchar](100) NULL,
	[branch_id] [int] NULL,
	[product_id] [int] NULL,
	[card_priority_id] [int] NULL,
	[customer_account_number] [varchar](30) NULL,
	[domicile_branch_id] [int] NULL,
	[account_type_id] [int] NULL,
	[card_issue_reason_id] [int] NULL,
	[customer_first_name] [varchar](50) NULL,
	[customer_middle_name] [varchar](50) NULL,
	[customer_last_name] [varchar](50) NULL,
	[name_on_card] [varchar](100) NULL,
	[customer_title_id] [int] NULL,
	[currency_id] [int] NULL,
	[resident_id] [int] NULL,
	[customer_type_id] [int] NULL,
	[cms_id] [varchar](50) NULL,
	[contract_number] [varchar](50) NULL,
	[idnumber] [varchar](50) NULL,
	[contact_number] [varchar](50) NULL,
	[customer_id] [varchar](50) NULL,
	[fee_waiver_YN] [bit] NULL,
	[fee_editable_YN] [bit] NULL,
	[fee_charged] [decimal](10, 4) NULL,
	[fee_overridden_YN] [bit] NULL,
	[audit_user_id] [bigint] NULL,
	[audit_workstation] [varchar](100) NULL,
	[sub_product_id] [varchar](100) NULL,
	[load_product_batch_type_id] [int] NULL,
	[already_loaded] [bit] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[load_cards_type]    Script Date: 2018-09-13 10:11:44 AM ******/
CREATE TYPE [dbo].[load_cards_type] AS TABLE(
	[card_number] [varchar](20) NULL,
	[card_reference] [varchar](100) NULL,
	[branch_id] [int] NULL,
	[card_sequence] [int] NULL,
	[expiry_date] [datetime2](7) NULL,
	[product_id] [int] NULL,
	[card_issue_method_id] [int] NULL,
	[sub_product_id] [int] NULL,
	[already_loaded] [bit] NULL,
	[card_id] [bigint] NULL,
	[load_batch_type_id] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[notification_array]    Script Date: 2018-09-13 10:11:44 AM ******/
CREATE TYPE [dbo].[notification_array] AS TABLE(
	[message_id] [uniqueidentifier] NULL,
	[message_text] [varchar](max) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[notifications_lang_messages]    Script Date: 2018-09-13 10:11:44 AM ******/
CREATE TYPE [dbo].[notifications_lang_messages] AS TABLE(
	[language_id] [int] NOT NULL,
	[channel_id] [int] NULL,
	[notification_text] [varchar](max) NOT NULL,
	[subject_text] [varchar](max) NOT NULL,
	[from_address] [nvarchar](50) NOT NULL,
	PRIMARY KEY CLUSTERED 
(
	[language_id] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  UserDefinedTableType [dbo].[parm_card_expected_status]    Script Date: 2018-09-13 10:11:44 AM ******/
CREATE TYPE [dbo].[parm_card_expected_status] AS TABLE(
	[card_status] [varchar](20) NOT NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[product_currency_array]    Script Date: 2018-09-13 10:11:44 AM ******/
CREATE TYPE [dbo].[product_currency_array] AS TABLE(
	[currency_id] [int] NULL,
	[is_base] [bit] NULL,
	[usr_field_name_1] [varchar](250) NULL,
	[usr_field_val_1] [varchar](250) NULL,
	[usr_field_name_2] [varchar](250) NULL,
	[usr_field_val_2] [varchar](250) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[product_external_fields_array]    Script Date: 2018-09-13 10:11:44 AM ******/
CREATE TYPE [dbo].[product_external_fields_array] AS TABLE(
	[external_system_field_id] [int] NULL,
	[field_name] [varchar](250) NULL,
	[field_value] [varchar](250) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[product_print_fields]    Script Date: 2018-09-13 10:11:45 AM ******/
CREATE TYPE [dbo].[product_print_fields] AS TABLE(
	[product_field_id] [int] NULL,
	[product_id] [int] NULL,
	[field_name] [varchar](100) NULL,
	[print_field_type_id] [int] NULL,
	[X] [decimal](18, 2) NULL,
	[Y] [decimal](18, 2) NULL,
	[width] [decimal](18, 2) NULL,
	[height] [decimal](18, 2) NULL,
	[font] [varchar](50) NULL,
	[font_size] [int] NULL,
	[mapped_name] [varchar](max) NULL,
	[editable] [bit] NULL,
	[deleted] [bit] NULL,
	[label] [varchar](100) NULL,
	[max_length] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[remote_card_updates]    Script Date: 2018-09-13 10:11:45 AM ******/
CREATE TYPE [dbo].[remote_card_updates] AS TABLE(
	[card_id] [bigint] NULL,
	[successful] [bit] NULL,
	[comment] [nvarchar](max) NULL,
	[time_update] [datetimeoffset](7) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[request_id_array]    Script Date: 2018-09-13 10:11:45 AM ******/
CREATE TYPE [dbo].[request_id_array] AS TABLE(
	[request_id] [bigint] NULL,
	[request_statues_id] [int] NULL,
	[card_number] [nvarchar](100) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[trikey_value_array]    Script Date: 2018-09-13 10:11:45 AM ******/
CREATE TYPE [dbo].[trikey_value_array] AS TABLE(
	[key1] [bigint] NULL,
	[key2] [bigint] NULL,
	[key3] [bigint] NULL,
	[value] [varchar](max) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[user_group_id_array]    Script Date: 2018-09-13 10:11:45 AM ******/
CREATE TYPE [dbo].[user_group_id_array] AS TABLE(
	[user_group_id] [int] NULL
)
GO
