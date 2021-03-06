USE [{DATABASE_NAME}]

--Change to external system
INSERT INTO [dbo].[mod_interface_account_params]
           ([BANK_C]
           ,[GROUPC]
           ,[issuer_id]
           ,[STAT_CHANGE]
           ,[LIM_INTR]
           ,[NON_REDUCE_BAL]
           ,[CRD]
           ,[CYCLE]
           ,[DEST_ACCNT_TYPE]
           ,[REP_LANG])

SELECT [BANK_C]
    ,[GROUPC]
    ,[issuer_id]
    ,[STAT_CHANGE]
    ,[LIM_INTR]
    ,[NON_REDUCE_BAL]
    ,[CRD]
    ,[CYCLE]
    ,[DEST_ACCNT_TYPE]
    ,[REP_LANG]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params]
WHERE [issuer_id] = @selected_issuer_id