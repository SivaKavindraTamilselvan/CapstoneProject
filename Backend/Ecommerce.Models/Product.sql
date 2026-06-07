INSERT INTO "ProductSubCategory"
("ProductSubCategoryId", "ProductSubCategoryName", "ProductCategoryId")
VALUES
(1,'Mobile Phones',1),
(2,'Laptops',1),
(3,'Tablets',1),
(4,'Smart Watches',1),
(5,'Headphones',1),
(6,'Speakers',1),
(7,'T-Shirts',2),
(8,'Shirts',2),
(9,'Jeans',2),
(10,'Trousers',2),
(11,'Shoes',2),
(12,'Sandals',2),
(13,'Face Wash',3),
(14,'Moisturizer',3),
(15,'Shampoo',3),
(16,'Conditioner',3),
(17,'Sunscreen',3),
(18,'Perfumes',3),
(19,'Cookware',4),
(20,'Furniture',4),
(21,'Storage Containers',4),
(22,'Home Decor',4),
(23,'Bedsheets',4),
(24,'Kitchen Appliances',4),
(25,'Academic Books',5),
(26,'Novels',5),
(27,'Comics',5),
(28,'Biographies',5),
(29,'Cricket Equipment',6),
(30,'Football Equipment',6),
(31,'Gym Equipment',6),
(32,'Yoga Accessories',6);

INSERT INTO "AttributeMaster"
("AttributeMasterId", "AttributeName")
VALUES
(1,'Brand'),
(2,'Color'),
(3,'Size'),
(4,'Storage'),
(5,'RAM'),
(6,'Screen Size'),
(7,'Volume'),
(8,'Material'),
(9,'Weight'),
(10,'Skin Type'),
(11,'Capacity'),
(12,'Processor'),
(13,'Operating System'),
(14,'Display Type'),
(15,'Battery Capacity'),
(16,'Gender'),
(17,'Author');

INSERT INTO "ProductSubCategoryAttribute"
("ProductSubCategoryAttributeId", "ProductSubCategoryId", "AttributeMasterId")
VALUES
(1,1,1),(2,1,2),(3,1,4),(4,1,5),(5,1,6),(6,1,15),
(7,2,1),(8,2,4),(9,2,5),(10,2,12),(11,2,13),
(12,3,1),(13,3,2),(14,3,4),(15,3,5),(16,3,6),(17,3,13),(18,3,15),
(19,4,1),(20,4,2),(21,4,15),
(22,5,1),(23,5,2),(24,5,9),(25,5,15),
(26,6,1),(27,6,2),(28,6,9),(29,6,15),
(30,7,1),(31,7,2),(32,7,3),(33,7,8),(34,7,16),
(35,8,1),(36,8,2),(37,8,3),(38,8,8),(39,8,16),
(40,9,1),(41,9,2),(42,9,3),(43,9,8),(44,9,16),
(45,10,1),(46,10,2),(47,10,3),(48,10,8),(49,10,16),
(50,11,1),(51,11,2),(52,11,3),(53,11,8),(54,11,16),
(55,12,1),(56,12,2),(57,12,3),(58,12,8),(59,12,16),
(60,13,1),(61,13,7),(62,13,10),
(63,14,1),(64,14,7),(65,14,10),
(66,15,1),(67,15,7),
(68,16,1),(69,16,7),
(70,17,1),(71,17,7),(72,17,10),
(73,18,1),(74,18,7),
(75,19,1),(76,19,8),(77,19,11),
(78,20,1),(79,20,8),(80,20,9),
(81,21,1),(82,21,8),(83,21,11),
(84,22,1),(85,22,8),(86,22,2),
(87,23,1),(88,23,2),(89,23,3),(90,23,8),
(91,24,1),(92,24,9),(93,24,11),
(94,25,1),(95,25,17),
(96,26,1),(97,26,17),
(98,27,1),(99,27,17),
(100,28,1),(101,28,17),
(102,29,1),(103,29,8),(104,29,9),
(105,30,1),(106,30,8),(107,30,9),
(108,31,1),(109,31,8),(110,31,9),
(111,32,1),(112,32,8),(113,32,9);


INSERT INTO "ProductCategory"
("ProductCategoryId", "ProductCategoryName")
VALUES
(1, 'Electronics'),
(2, 'Fashion'),
(3, 'Beauty & Personal Care'),
(4, 'Home & Kitchen'),
(5, 'Books'),
(6, 'Sports & Fitness');

INSERT INTO "ProductSubCategory"
(
    "ProductSubCategoryId",
    "ProductSubCategoryName",
    "ProductCategoryId",
    "CommissionPercentage"
)
VALUES
(1,'Mobile Phones',1,5.00),
(2,'Laptops',1,6.00),
(3,'Tablets',1,5.50),
(4,'Smart Watches',1,7.00),
(5,'Headphones',1,8.00),
(6,'Speakers',1,8.00),

(7,'T-Shirts',2,12.00),
(8,'Shirts',2,12.00),
(9,'Jeans',2,13.00),
(10,'Trousers',2,13.00),
(11,'Shoes',2,15.00),
(12,'Sandals',2,15.00),

(13,'Face Wash',3,18.00),
(14,'Moisturizer',3,18.00),
(15,'Shampoo',3,20.00),
(16,'Conditioner',3,20.00),
(17,'Sunscreen',3,18.00),
(18,'Perfumes',3,22.00),

(19,'Cookware',4,10.00),
(20,'Furniture',4,12.00),
(21,'Storage Containers',4,10.00),
(22,'Home Decor',4,15.00),
(23,'Bedsheets',4,14.00),
(24,'Kitchen Appliances',4,8.00),

(25,'Academic Books',5,5.00),
(26,'Novels',5,8.00),
(27,'Comics',5,8.00),
(28,'Biographies',5,8.00),

(29,'Cricket Equipment',6,10.00),
(30,'Football Equipment',6,10.00),
(31,'Gym Equipment',6,12.00),
(32,'Yoga Accessories',6,12.00);