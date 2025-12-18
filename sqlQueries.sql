CREATE TABLE material_type (
	id INT IDENTITY(1,1) PRIMARY KEY,
	name NVARCHAR(255) NOT NULL,
	defect_percent FLOAT NOT NULL CHECK (defect_percent >= 0)
);

CREATE TABLE product_type (
	id INT IDENTITY(1,1) PRIMARY KEY,
	name NVARCHAR(255) NOT NULL,
	coefficient FLOAT NOT NULL
);

CREATE TABLE material (
	id INT IDENTITY(1,1) PRIMARY KEY,
	name NVARCHAR(255) NOT NULL,
	material_type_id INT NOT NULL,
	cost DECIMAL(18,2) NOT NULL CHECK(cost >= 0),
	stock_quantity DECIMAL(18,2) NOT NULL CHECK(stock_quantity >= 0),
	min_stock DECIMAL(18,2) NOT NULL CHECK (min_stock >= 0),
	package_quantity DECIMAL(18,2) NOT NULL CHECK (package_quantity >= 0),
	unit NVARCHAR(20) NOT NULL,
	CONSTRAINT FK_material_material_type FOREIGN KEY (material_type_id)
	REFERENCES material_type(id)
);

CREATE TABLE product (
	id INT IDENTITY(1,1) PRIMARY KEY,
	product_type_id INT NOT NULL,
	name NVARCHAR(255) NOT NULL,
	article NVARCHAR(50) NOT NULL,
	min_partner_price DECIMAL(18,2) NOT NULL CHECK (min_partner_price >= 0),
	roll_width DECIMAL(18,2) NOT NULL CHECK (roll_width >= 0),
	CONSTRAINT FK_product_product_type FOREIGN KEY (product_type_id) REFERENCES
	product_type(id)
);

CREATE TABLE product_material (
	product_id INT NOT NULL,
	material_id INT NOT NULL,
	quantity_per_product FLOAT NOT NULL CHECK (quantity_per_product >= 0),
	CONSTRAINT PK_product_material PRIMARY KEY (product_id, material_id),
	CONSTRAINT FK_product_material_product FOREIGN KEY (product_id) REFERENCES
	product(id),
	CONSTRAINT FK_product_material_material FOREIGN KEY (material_id) REFERENCES
	material(id)
);

CREATE TABLE material_type_staging (
    name NVARCHAR(255),
    defect_percent NVARCHAR(50)
);

CREATE TABLE product_type_staging (
    name NVARCHAR(255),
    coefficient FLOAT
);

CREATE TABLE material_staging (
    name NVARCHAR(255),
    material_type_name NVARCHAR(100),
    cost DECIMAL(18,2),
    stock_quantity DECIMAL(18,2),
    min_stock DECIMAL(18,2),
    package_quantity DECIMAL(18,2),
    unit NVARCHAR(20)
);

CREATE TABLE product_staging (
    product_type_name NVARCHAR(100),
    name NVARCHAR(255),
    article NVARCHAR(50),
    min_partner_price DECIMAL(18,2),
    roll_width DECIMAL(18,2)
);

CREATE TABLE product_material_staging (
    product_name NVARCHAR(255),
    material_name NVARCHAR(255),
    quantity_per_product FLOAT
);

INSERT INTO material_type (name, defect_percent)
SELECT 
    name,
    TRY_CAST(REPLACE(REPLACE(defect_percent, '%', ''), ',', '.') AS FLOAT) / 100.0
FROM material_type_staging;

INSERT INTO product_type (name, coefficient)
SELECT name, coefficient FROM product_type_staging;

INSERT INTO material(name, material_type_id, cost, stock_quantity, min_stock, package_quantity, unit)
SELECT m.name, mt.id, m.cost, m.stock_quantity, m.min_stock, m.package_quantity, m.unit
FROM material_staging m JOIN material_type mt ON material_type_name = mt.name;

INSERT INTO product (product_type_id, name, article, min_partner_price, roll_width)
SELECT pt.id, p.name, p.article, p.min_partner_price, p.roll_width
FROM product_staging p JOIN product_type pt ON p.product_type_name = pt.name;

INSERT INTO product_material(product_id, material_id, quantity_per_product)
SELECT p.id, m.id, pm.quantity_per_product 
FROM product_material_staging pm 
JOIN product p ON p.name = pm.product_name
JOIN material m ON m.name = pm.material_name;

DROP TABLE material_type_staging, product_type_staging, material_staging, 
product_staging, product_material_staging;