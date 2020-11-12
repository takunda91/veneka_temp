-- https://www.mssqltips.com/sqlservertip/2313/convert-sql-server-datetime-data-type-to-datetimeoffset-data-type/

--set default timezone for all user

--update
UPDATE dbo.pin_reissue
SET request_expiry =  ToDateTimeOffset( request_expiry, '+02:00')

select ToDateTimeOffset( request_expiry, '+02:00')
FROM pin_reissue