USE [indigo_database_main_dev]
GO

--Create Parent
CREATE TABLE dist_batch_status_flow (
	dist_batch_status_flow_id int PRIMARY KEY,
	dist_batch_status_flow_name varchar(150) not null,
	dist_batch_type_id int not null,
	card_issue_method_id int not null,
	CONSTRAINT FK_dist_batch_status_flow_dist_batcht_ype FOREIGN KEY (dist_batch_type_id) REFERENCES dist_batch_type (dist_batch_type_id),
	CONSTRAINT FK_dist_batch_status_flow_card_issue_method FOREIGN KEY (card_issue_method_id) REFERENCES card_issue_method (card_issue_method_id)
)
GO

--Alter statuses flow
ALTER TABLE dist_batch_statuses_flow
ADD dist_batch_status_flow_id int
GO

--Alter issuer product
ALTER TABLE issuer_product
ADD production_dist_batch_status_flow int
GO
ALTER TABLE issuer_product
ADD distribution_dist_batch_status_flow int
GO

--Insert Values
INSERT INTO dist_batch_status_flow (dist_batch_status_flow_id, dist_batch_status_flow_name, dist_batch_type_id, card_issue_method_id)
VALUES (1, 'DEFAULT_CENTRALISED_PRODUCTION', 0, 0),
	   (2, 'DEFAULT_CENTRALISED_PRODUCTION_WITH_PINMAILER', 0, 0),
	   (3, 'DEFAULT_INSTANT_PRODUCTION', 0, 1),
	   (4, 'DEFAULT_INSTANT_EMP_PRODUCTION', 0, 1),
	   (5, 'DEFAULT_CENTRALISED_DISTRIBUTION', 1, 0),
	   (6, 'DEFAULT_INSTANT_DISTRIBUTION', 1, 1)
GO

--INSERT FLOW values

--Update product. NOTE please check those that need to use EMP!!!!
UPDATE [issuer_product]
SET production_dist_batch_status_flow = dist_batch_status_flow_id
FROM [issuer_product] INNER JOIN [dist_batch_status_flow]
	ON [dist_batch_status_flow].card_issue_method_id = [issuer_product].card_issue_method_id
		AND [dist_batch_status_flow].dist_batch_type_id = 0
		AND dist_batch_status_flow_id IN (2, 3)
GO
UPDATE [issuer_product]
SET distribution_dist_batch_status_flow = dist_batch_status_flow_id
FROM [issuer_product] INNER JOIN [dist_batch_status_flow]
	ON [dist_batch_status_flow].card_issue_method_id = [issuer_product].card_issue_method_id
		AND [dist_batch_status_flow].dist_batch_type_id = 1
		AND dist_batch_status_flow_id IN (5, 6)
GO

--Not nulls
ALTER TABLE dist_batch_statuses_flow
	ALTER COLUMN dist_batch_status_flow_id int not null
GO

ALTER TABLE dist_batch_statuses_flow
	ADD CONSTRAINT FK_dist_batch_status_flow_dist_batch_statuses_flow FOREIGN KEY (dist_batch_status_flow_id) REFERENCES dist_batch_status_flow (dist_batch_status_flow_id)
GO

ALTER TABLE issuer_product
	ALTER COLUMN production_dist_batch_status_flow int not null
GO
ALTER TABLE issuer_product
	ALTER COLUMN distribution_dist_batch_status_flow int not null
GO

ALTER TABLE issuer_product
	ADD CONSTRAINT FK_dist_batch_status_flow_issuer_product FOREIGN KEY (production_dist_batch_status_flow) REFERENCES dist_batch_status_flow (dist_batch_status_flow_id)
GO

ALTER TABLE issuer_product
	ADD CONSTRAINT FK_production_dist_batch_status_flow_issuer_product FOREIGN KEY (distribution_dist_batch_status_flow) REFERENCES dist_batch_status_flow (dist_batch_status_flow_id)
GO

--DROP Primary Key and create unique key
ALTER TABLE [dist_batch_statuses_flow]  
	DROP CONSTRAINT PK_dist_batch_statuses_flow;
GO
ALTER TABLE [dist_batch_statuses_flow]
	ADD CONSTRAINT PK_DistStatusesFlow PRIMARY KEY (dist_batch_status_flow_id, dist_batch_statuses_id, flow_dist_batch_statuses_id)	
GO

--CLEANUP
ALTER TABLE [dist_batch_statuses_flow]
	DROP COLUMN issuer_id, dist_batch_type_id, card_issue_method_id
GO

ALTER TABLE [dist_batch]
	ALTER COLUMN dist_batch_reference varchar(50) not null
GO








--OTHER
INSERT INTO dist_batch_status_flow (dist_batch_status_flow_id, dist_batch_status_flow_name, dist_batch_type_id, card_issue_method_id)
VALUES (10, 'UNIBANK_INSTANT_PRODUCTION', 0, 1),
	   (11, 'UNIBANK_INSTANT_DISTRIBUTION', 1, 1)


UPDATE dist_batch_statuses_flow
SET dist_batch_status_flow_id = 11
WHERE issuer_id = 9 AND dist_batch_type_id = 1 AND card_issue_method_id = 1

UPDATE dist_batch_statuses_flow
SET dist_batch_status_flow_id = 5
WHERE issuer_id = -1 AND dist_batch_type_id = 1 AND card_issue_method_id = 0


UPDATE dist_batch_statuses_flow
SET dist_batch_status_flow_id = 4
WHERE issuer_id = 18 AND dist_batch_type_id = 0 AND card_issue_method_id = 1

UPDATE dist_batch_statuses_flow
SET dist_batch_status_flow_id = 2
WHERE issuer_id = -1 AND dist_batch_type_id = 0 AND card_issue_method_id = 0

UPDATE dist_batch_statuses_flow
SET dist_batch_status_flow_id = 3
WHERE issuer_id = -1 AND dist_batch_type_id = 0 AND card_issue_method_id = 1
