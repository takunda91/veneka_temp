USE indigo_database_main_dev
GO

INSERT INTO [interface_type] (interface_type_id, interface_type_name)
	VALUES (7, 'NOTIFICATIONS')
GO

INSERT INTO [interface_type_language] (interface_type_id, language_id, language_text)
	VALUES(7, 0, 'NOTIFICATIONS'),
		  (7, 1, 'NOTIFICATIONS_fr'),
	      (7, 2, 'NOTIFICATIONS_pt'),
	      (7, 3, 'NOTIFICATIONS_es')
GO

CREATE TABLE notification_batch_messages (
	issuer_id int not null,
	dist_batch_type_id int not null,
	dist_batch_statuses_id int not null,
	language_id int not null,
	channel_id int not null,
	notification_text varchar(max) not null,
	subject_text varchar(max) not null

	CONSTRAINT PK_notification_messages PRIMARY KEY NONCLUSTERED (issuer_id, dist_batch_statuses_id, language_id, channel_id)
	CONSTRAINT FK_issuer_id FOREIGN KEY (issuer_id) REFERENCES Issuer(issuer_id),
	CONSTRAINT FK_dist_batch_statuses FOREIGN KEY (dist_batch_statuses_id) REFERENCES dist_batch_statuses(dist_batch_statuses_id),
	CONSTRAINT FK_languages FOREIGN KEY (language_id) REFERENCES languages(id),
	CONSTRAINT FK_notification_batch_messages_dist_batch_type_id FOREIGN KEY (dist_batch_type_id) REFERENCES dist_batch_type(dist_batch_type_id)
)
GO

CREATE TABLE notification_branch_messages (	
	issuer_id int not null,
	branch_card_statuses_id int not null,
	card_issue_method_id int not null,
	language_id int not null,
	channel_id int not null,
	notification_text varchar(max) not null,
	subject_text varchar(max) not null

	CONSTRAINT PK_notification_branch_messages PRIMARY KEY NONCLUSTERED (issuer_id, branch_card_statuses_id, card_issue_method_id, language_id, channel_id)
	CONSTRAINT FK_notification_branch_messages_issuer_id FOREIGN KEY (issuer_id) REFERENCES Issuer(issuer_id),
	CONSTRAINT FK_notification_branch_messages_branch_card_statuses FOREIGN KEY (branch_card_statuses_id) REFERENCES branch_card_statuses(branch_card_statuses_id),
	CONSTRAINT FK_notification_branch_messages_card_issue_method FOREIGN KEY (card_issue_method_id) REFERENCES card_issue_method(card_issue_method_id),
	CONSTRAINT FK_notification_branch_messages_languages FOREIGN KEY (language_id) REFERENCES languages(id),
)
GO

CREATE TABLE notification_batch_outbox (
	batch_message_id uniqueidentifier not null,
	added_time datetime not null,
	dist_batch_id bigint not null,
	issuer_id int not null,
	dist_batch_statuses_id int not null,
	dist_batch_type_id int not null,
	language_id int not null,
	channel_id int not null
)
GO

CREATE TABLE notification_branch_outbox (
	branch_message_id uniqueidentifier not null,
	added_time datetime not null,
	card_id bigint not null,
	issuer_id int not null,
	branch_card_statuses_id int not null,
	card_issue_method_id int not null,
	language_id int not null,
	channel_id int not null
)
GO

CREATE TABLE notification_batch_log (
	added_time datetime not null,
	issuer_id int not null,
	dist_batch_id int not null,	
	dist_batch_statuses_id int not null,
	[user_id] bigint not null,
	channel_id int not null,
	notification_text varbinary(max) not null
)
GO

CREATE TABLE notification_branch_log (
	added_time datetime not null,
	card_id bigint not null,
	issuer_id int not null,
	branch_card_statuses_id int not null,
	channel_id int not null,
	notification_text varbinary(max) not null
)
GO

CREATE TYPE [dbo].[notification_array] AS TABLE(
	[message_id] uniqueidentifier NULL,
	[message_text] [varchar](max) NULL
)
GO