CREATE PROC [dbo].[usp_get_print_job_status]
			@printJobId varchar(30),
			@language_id int,
			@audit_user_id bigint,
			@audit_workstation varchar(100)
			
	AS 
	BEGIN 
	
	SELECT        print_statuses_language.language_text as 'print_status', print_jobs.comments, print_jobs.status_date
FROM            print_jobs INNER JOIN
                         print_statuses ON print_jobs.print_statuses_id = print_statuses.print_statuses_id INNER JOIN
                         print_statuses_language ON print_jobs.print_statuses_id = print_statuses_language.print_statuses_id
						 and print_statuses_language.language_id=@language_id

						 where print_jobs.print_job_id=CAST(@printJobId as bigint)

                           
                           
    END