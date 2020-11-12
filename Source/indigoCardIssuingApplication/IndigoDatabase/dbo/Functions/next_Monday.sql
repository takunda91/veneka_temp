create function [dbo].[next_Monday] ( @myDate DATETIMEOFFSET )  
returns DATETIMEOFFSET
as
BEGIN
 set @myDate = dateadd(mm, 6, @myDate)  
 while datepart(dw,@myDate) <> 2
  begin  
   set @myDate = dateadd(dd, 1, @myDate)  
  end  
   return @myDate
 end