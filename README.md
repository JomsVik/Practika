DROP TABLE IF EXISTS order_items CASCADE;
DROP TABLE IF EXISTS orders CASCADE;
DROP TABLE IF EXISTS products CASCADE;
DROP TABLE IF EXISTS users CASCADE;
DROP TABLE IF EXISTS roles CASCADE;
DROP TABLE IF EXISTS pickup_points CASCADE;

CREATE TABLE roles (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE pickup_points (
    id SERIAL PRIMARY KEY,
    address TEXT NOT NULL UNIQUE
);

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    full_name VARCHAR(200) NOT NULL,
    login VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role_id INTEGER NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (role_id) REFERENCES roles(id)
);

CREATE TABLE products (
    id SERIAL PRIMARY KEY,
    article VARCHAR(50) NOT NULL UNIQUE,
    name VARCHAR(200) NOT NULL,
    unit VARCHAR(20) DEFAULT 'шт.',
    price DECIMAL(10,2) NOT NULL,
    supplier VARCHAR(200),
    manufacturer VARCHAR(200),
    category VARCHAR(100),
    discount INTEGER DEFAULT 0,
    quantity INTEGER NOT NULL DEFAULT 0,
    description TEXT,
    image_url VARCHAR(500)
);

CREATE TABLE orders (
    id SERIAL PRIMARY KEY,
    order_number VARCHAR(50) NOT NULL UNIQUE,
    user_id INTEGER NOT NULL,
    order_date TIMESTAMP,
    delivery_date DATE,
    pickup_point_id INTEGER,
    pickup_code VARCHAR(20),
    status VARCHAR(50) DEFAULT 'Новый',
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (pickup_point_id) REFERENCES pickup_points(id)
);

CREATE TABLE order_items (
    id SERIAL PRIMARY KEY,
    order_id INTEGER NOT NULL,
    product_id INTEGER NOT NULL,
    quantity INTEGER NOT NULL,
    price_at_moment DECIMAL(10,2),
    FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products(id)
);

CREATE INDEX idx_users_login ON users(login);
CREATE INDEX idx_products_article ON products(article);
CREATE INDEX idx_orders_number ON orders(order_number);
CREATE INDEX idx_orders_user ON orders(user_id);
CREATE INDEX idx_order_items_order ON order_items(order_id);




DO $$
DECLARE
    v_order1 INTEGER;
    v_order2 INTEGER;
    v_order3 INTEGER;
    v_order4 INTEGER;
    v_order5 INTEGER;
    v_order6 INTEGER;
    v_order7 INTEGER;
    v_order8 INTEGER;
    v_order9 INTEGER;
    v_order10 INTEGER;
BEGIN
    -- Получаем ID заказов по order_number
    SELECT id INTO v_order1 FROM orders WHERE order_number = '1';
    SELECT id INTO v_order2 FROM orders WHERE order_number = '2';
    SELECT id INTO v_order3 FROM orders WHERE order_number = '3';
    SELECT id INTO v_order4 FROM orders WHERE order_number = '4';
    SELECT id INTO v_order5 FROM orders WHERE order_number = '5';
    SELECT id INTO v_order6 FROM orders WHERE order_number = '6';
    SELECT id INTO v_order7 FROM orders WHERE order_number = '7';
    SELECT id INTO v_order8 FROM orders WHERE order_number = '8';
    SELECT id INTO v_order9 FROM orders WHERE order_number = '9';
    SELECT id INTO v_order10 FROM orders WHERE order_number = '10';
    
    -- Заказ 1: А112Т4 (2 шт), F635R4 (2 шт)
    INSERT INTO order_items (order_id, product_id, quantity, price_at_moment) VALUES
    (v_order1, (SELECT id FROM products WHERE article = 'А112Т4'), 2, 4990),
    (v_order1, (SELECT id FROM products WHERE article = 'F635R4'), 2, 3244);
    
    -- Заказ 2: H782T5 (1 шт), G783F5 (1 шт)
    INSERT INTO order_items (order_id, product_id, quantity, price_at_moment) VALUES
    (v_order2, (SELECT id FROM products WHERE article = 'H782T5'), 1, 4499),
    (v_order2, (SELECT id FROM products WHERE article = 'G783F5'), 1, 5900);
    
    -- Заказ 3: J384T6 (10 шт), D572U8 (10 шт)
    INSERT INTO order_items (order_id, product_id, quantity, price_at_moment) VALUES
    (v_order3, (SELECT id FROM products WHERE article = 'J384T6'), 10, 3800),
    (v_order3, (SELECT id FROM products WHERE article = 'D572U8'), 10, 4100);
    
    -- Заказ 4: F572H7 (5 шт), D329H3 (4 шт)
    INSERT INTO order_items (order_id, product_id, quantity, price_at_moment) VALUES
    (v_order4, (SELECT id FROM products WHERE article = 'F572H7'), 5, 2700),
    (v_order4, (SELECT id FROM products WHERE article = 'D329H3'), 4, 1890);
    
    -- Заказ 5: А112Т4 (2 шт), F635R4 (2 шт)
    INSERT INTO order_items (order_id, product_id, quantity, price_at_moment) VALUES
    (v_order5, (SELECT id FROM products WHERE article = 'А112Т4'), 2, 4990),
    (v_order5, (SELECT id FROM products WHERE article = 'F635R4'), 2, 3244);
    
    -- Заказ 6: H782T5 (1 шт), G783F5 (1 шт)
    INSERT INTO order_items (order_id, product_id, quantity, price_at_moment) VALUES
    (v_order6, (SELECT id FROM products WHERE article = 'H782T5'), 1, 4499),
    (v_order6, (SELECT id FROM products WHERE article = 'G783F5'), 1, 5900);
    
    -- Заказ 7: J384T6 (10 шт), D572U8 (10 шт)
    INSERT INTO order_items (order_id, product_id, quantity, price_at_moment) VALUES
    (v_order7, (SELECT id FROM products WHERE article = 'J384T6'), 10, 3800),
    (v_order7, (SELECT id FROM products WHERE article = 'D572U8'), 10, 4100);
    
    -- Заказ 8: F572H7 (5 шт), D329H3 (4 шт)
    INSERT INTO order_items (order_id, product_id, quantity, price_at_moment) VALUES
    (v_order8, (SELECT id FROM products WHERE article = 'F572H7'), 5, 2700),
    (v_order8, (SELECT id FROM products WHERE article = 'D329H3'), 4, 1890);
    
    -- Заказ 9: B320R5 (5 шт), G432E4 (1 шт)
    INSERT INTO order_items (order_id, product_id, quantity, price_at_moment) VALUES
    (v_order9, (SELECT id FROM products WHERE article = 'B320R5'), 5, 4300),
    (v_order9, (SELECT id FROM products WHERE article = 'G432E4'), 1, 2800);
    
    -- Заказ 10: S213E3 (5 шт), E482R4 (5 шт)
    INSERT INTO order_items (order_id, product_id, quantity, price_at_moment) VALUES
    (v_order10, (SELECT id FROM products WHERE article = 'S213E3'), 5, 2156),
    (v_order10, (SELECT id FROM products WHERE article = 'E482R4'), 5, 1800);
    
END $$;




DO $$
DECLARE
    v_stevanov INTEGER;
    v_savchenko INTEGER;
    v_kazhov INTEGER; 
    v_kuznetsov INTEGER;
    v_kolenichenko INTEGER;
BEGIN

    SELECT id INTO v_stevanov FROM users WHERE full_name = 'Степанов Михаил Артёмович';
    SELECT id INTO v_savchenko FROM users WHERE full_name = 'Савченко Максим Дмитриевич';
    SELECT id INTO v_kazhov FROM users WHERE full_name = 'Кажов Александ Сергеевич';
    SELECT id INTO v_kuznetsov FROM users WHERE full_name = 'Кузнецов Константин';
    SELECT id INTO v_kolenichenko FROM users WHERE full_name = 'Колениченко Алексей';
    

    INSERT INTO orders (order_number, user_id, order_date, delivery_date, pickup_point_id, pickup_code, status) VALUES
	
    ('1', v_stevanov, '2025-02-27', '2025-04-20', 1, '901', 'Завершен'),
    ('5', v_stevanov, '2025-03-17', '2025-04-24', 2, '905', 'Завершен'),
    ('9', v_stevanov, '2025-04-02', '2025-04-28', 5, '909', 'Новый'),
    ('10', v_stevanov, '2025-04-03', '2025-04-29', 19, '910', 'Новый'),
    

    ('2', v_savchenko, '2022-09-28', '2025-04-21', 11, '902', 'Завершен'),
    ('6', v_savchenko, '2025-03-01', '2025-04-25', 15, '906', 'Завершен'),
    

    ('3', v_kazhov, '2025-03-21', '2025-04-22', 2, '903', 'Завершен'),
    ('7', v_kazhov, '2025-02-28', '2025-04-26', 3, '907', 'Завершен'),
    

    ('4', v_kuznetsov, '2025-02-20', '2025-04-23', 11, '904', 'Завершен'),
    ('8', v_kolenichenko, '2025-03-31', '2025-04-27', 19, '908', 'Новый');
    
END $$;



INSERT INTO users (full_name, login, password_hash, role_id) VALUES
('Савченко Максим Дмитриевич', 'admin', 'admin', 1),
('Кажов Александ Сергеевич', 'admin1', 'admin1', 1),
('Кузнецов Константин', 'admin2', 'admin2', 1),
('Колениченко Алексей', 'admin3', 'admin3', 1);

INSERT INTO users (full_name, login, password_hash, role_id) VALUES
('Степанов Михаил Артёмович', '1diph5e@tutanota.com', '8ntwUp', 2),
('Ворсин Петр Евгеньевич', 'tjde7c@yahoo.com', 'YOyhfR', 2),
('Старикова Елена Павловна', 'wpmrc3do@tutanota.com', 'RSbvHv', 2);

INSERT INTO users (full_name, login, password_hash, role_id) VALUES
('Михайлюк Анна Вячеславовна', '5d4zbu@tutanota.com', 'rwVDh9', 3),
('Ситдикова Елена Анатольевна', 'ptec8ym@yahoo.com', 'LdNyos', 3),
('Ворсин Петр Евгеньевич', '1qz4kw@mail.com', 'gynQMT', 3),
('Старикова Елена Павловна', '4np6se@mail.com', 'AtnDjr', 3);




INSERT INTO pickup_points (address) VALUES
('420151, г. Лесной, ул. Вишневая, 32'),
('125061, г. Лесной, ул. Подгорная, 8'),
('630370, г. Лесной, ул. Шоссейная, 24'),
('400562, г. Лесной, ул. Зеленая, 32'),
('614510, г. Лесной, ул. Маяковского, 47'),
('410542, г. Лесной, ул. Светлая, 46'),
('620839, г. Лесной, ул. Цветочная, 8'),
('443890, г. Лесной, ул. Коммунистическая, 1'),
('603379, г. Лесной, ул. Спортивная, 46'),
('603721, г. Лесной, ул. Гоголя, 41'),
('410172, г. Лесной, ул. Северная, 13'),
('614611, г. Лесной, ул. Молодежная, 50'),
('454311, г.Лесной, ул. Новая, 19'),
('660007, г.Лесной, ул. Октябрьская, 19'),
('603036, г. Лесной, ул. Садовая, 4'),
('394060, г.Лесной, ул. Фрунзе, 43'),
('410661, г. Лесной, ул. Школьная, 50'),
('625590, г. Лесной, ул. Коммунистическая, 20'),
('625683, г. Лесной, ул. 8 Марта'),
('450983, г.Лесной, ул. Комсомольская, 26'),
('394782, г. Лесной, ул. Чехова, 3'),
('603002, г. Лесной, ул. Дзержинского, 28'),
('450558, г. Лесной, ул. Набережная, 30'),
('344288, г. Лесной, ул. Чехова, 1'),
('614164, г.Лесной, ул. Степная, 30'),
('394242, г. Лесной, ул. Коммунистическая, 43'),
('660540, г. Лесной, ул. Солнечная, 25'),
('125837, г. Лесной, ул. Шоссейная, 40'),
('125703, г. Лесной, ул. Партизанская, 49'),
('625283, г. Лесной, ул. Победы, 46'),
('614753, г. Лесной, ул. Полевая, 35'),
('426030, г. Лесной, ул. Маяковского, 44'),
('450375, г. Лесной ул. Клубная, 44'),
('625560, г. Лесной, ул. Некрасова, 12'),
('630201, г. Лесной, ул. Комсомольская, 17'),
('190949, г. Лесной, ул. Мичурина, 26');




INSERT INTO products (article, name, unit, price, supplier, manufacturer, category, discount, quantity, description, image_url) VALUES
('А112Т4', 'Ботинки', 'шт.', 4990, 'Kari', 'Kari', 'Женская обувь', 3, 6, 'Женские Ботинки демисезонные kari', '1.jpg'),
('F635R4', 'Ботинки', 'шт.', 3244, 'Обувь для вас', 'Marco Tozzi', 'Женская обувь', 2, 13, 'Ботинки Marco Tozzi женские демисезонные, размер 39, цвет бежевый', '2.jpg'),
('H782T5', 'Туфли', 'шт.', 4499, 'Kari', 'Kari', 'Мужская обувь', 4, 5, 'Туфли kari мужские классика MYZ21AW-450A, размер 43, цвет: черный', '3.jpg'),
('G783F5', 'Ботинки', 'шт.', 5900, 'Kari', 'Рос', 'Мужская обувь', 2, 8, 'Мужские ботинки Рос-Обувь кожаные с натуральным мехом', '4.jpg'),
('J384T6', 'Ботинки', 'шт.', 3800, 'Обувь для вас', 'Rieker', 'Мужская обувь', 2, 16, 'B3430/14 Полуботинки мужские Rieker', '5.jpg'),
('D572U8', 'Кроссовки', 'шт.', 4100, 'Обувь для вас', 'Рос', 'Мужская обувь', 3, 6, '129615-4 Кроссовки мужские', '6.jpg'),
('F572H7', 'Туфли', 'шт.', 2700, 'Kari', 'Marco Tozzi', 'Женская обувь', 2, 14, 'Туфли Marco Tozzi женские летние, размер 39, цвет черный', '7.jpg'),
('D329H3', 'Полуботинки', 'шт.', 1890, 'Обувь для вас', 'Alessio Nesca', 'Женская обувь', 4, 4, 'Полуботинки Alessio Nesca женские 3-30797-47, размер 37, цвет: бордовый', '8.jpg'),
('B320R5', 'Туфли', 'шт.', 4300, 'Kari', 'Rieker', 'Женская обувь', 2, 6, 'Туфли Rieker женские демисезонные, размер 41, цвет коричневый', '9.jpg'),
('G432E4', 'Туфли', 'шт.', 2800, 'Kari', 'Kari', 'Женская обувь', 3, 15, 'Туфли kari женские TR-YR-413017, размер 37, цвет: черный', '10.jpg'),
('S213E3', 'Полуботинки', 'шт.', 2156, 'Обувь для вас', 'CROSBY', 'Мужская обувь', 3, 6, '407700/01-01 Полуботинки мужские CROSBY', NULL),
('E482R4', 'Полуботинки', 'шт.', 1800, 'Kari', 'Kari', 'Женская обувь', 2, 14, 'Полуботинки kari женские MYZ20S-149, размер 41, цвет: черный', NULL),
('S634B5', 'Кеды', 'шт.', 5500, 'Обувь для вас', 'CROSBY', 'Мужская обувь', 3, 0, 'Кеды Caprice мужские демисезонные, размер 42, цвет черный', NULL),
('K345R4', 'Полуботинки', 'шт.', 2100, 'Обувь для вас', 'CROSBY', 'Мужская обувь', 2, 3, '407700/01-02 Полуботинки мужские CROSBY', NULL),
('O754F4', 'Туфли', 'шт.', 5400, 'Обувь для вас', 'Rieker', 'Женская обувь', 4, 18, 'Туфли женские демисезонные Rieker артикул 55073-68/37', NULL),
('G531F4', 'Ботинки', 'шт.', 6600, 'Kari', 'Kari', 'Женская обувь', 12, 9, 'Ботинки женские зимние ROMER арт. 893167-01 Черный', NULL),
('J542F5', 'Тапочки', 'шт.', 500, 'Kari', 'Kari', 'Мужская обувь', 13, 0, 'Тапочки мужские Арт.70701-55-67син р.41', NULL),
('B431R5', 'Ботинки', 'шт.', 2700, 'Обувь для вас', 'Rieker', 'Мужская обувь', 2, 5, 'Мужские кожаные ботинки/мужские ботинки', NULL),
('P764G4', 'Туфли', 'шт.', 6800, 'Kari', 'CROSBY', 'Женская обувь', 15, 15, 'Туфли женские, ARGO, размер 38', NULL),
('C436G5', 'Ботинки', 'шт.', 10200, 'Kari', 'Alessio Nesca', 'Женская обувь', 15, 9, 'Ботинки женские, ARGO, размер 40', NULL),
('F427R5', 'Ботинки', 'шт.', 11800, 'Обувь для вас', 'Rieker', 'Женская обувь', 15, 11, 'Ботинки на молнии с декоративной пряжкой FRAU', NULL),
('N457T5', 'Полуботинки', 'шт.', 4600, 'Kari', 'CROSBY', 'Женская обувь', 3, 13, 'Полуботинки Ботинки черные зимние, мех', NULL),
('D364R4', 'Туфли', 'шт.', 12400, 'Kari', 'Kari', 'Женская обувь', 16, 5, 'Туфли Luiza Belly женские Kate-lazo черные из натуральной замши', NULL),
('S326R5', 'Тапочки', 'шт.', 9900, 'Обувь для вас', 'CROSBY', 'Мужская обувь', 17, 15, 'Мужские кожаные тапочки "Профиль С.Дали"', NULL),
('L754R4', 'Полуботинки', 'шт.', 1700, 'Kari', 'Kari', 'Женская обувь', 2, 7, 'Полуботинки kari женские WB2020SS-26, размер 38, цвет: черный', NULL),
('M542T5', 'Кроссовки', 'шт.', 2800, 'Обувь для вас', 'Rieker', 'Мужская обувь', 18, 3, 'Кроссовки мужские TOFA', NULL),
('D268G5', 'Туфли', 'шт.', 4399, 'Обувь для вас', 'Rieker', 'Женская обувь', 3, 12, 'Туфли Rieker женские демисезонные, размер 36, цвет коричневый', NULL),
('T324F5', 'Сапоги', 'шт.', 4699, 'Kari', 'CROSBY', 'Женская обувь', 2, 5, 'Сапоги замша Цвет: синий', NULL),
('K358H6', 'Тапочки', 'шт.', 599, 'Kari', 'Rieker', 'Мужская обувь', 20, 2, 'Тапочки мужские син р.41', NULL),
('H535R5', 'Ботинки', 'шт.', 2300, 'Обувь для вас', 'Rieker', 'Женская обувь', 2, 7, 'Женские Ботинки демисезонные', NULL);

INSERT INTO roles (name) VALUES
('Администратор'),
('Менеджер'),
('Авторизированный клиент'),
('Гость');

CREATE EXTENSION IF NOT EXISTS pgcrypto;


TRUNCATE TABLE users CASCADE;
ALTER SEQUENCE users_id_seq RESTART WITH 1;

INSERT INTO roles (name) VALUES
('Администратор'),
('Менеджер'),
('Авторизированный клиент'),
('Гость')
ON CONFLICT (name) DO NOTHING;


INSERT INTO users (full_name, login, password_hash, role_id) VALUES
('Савченко Максим Дмитриевич', 'admin', crypt('admin', gen_salt('bf', 10)), 1),
('Кажов Александ Сергеевич', 'admin1', crypt('admin1', gen_salt('bf', 10)), 1),
('Кузнецов Константин', 'admin2', crypt('admin2', gen_salt('bf', 10)), 1),
('Колениченко Алексей', 'admin3', crypt('admin3', gen_salt('bf', 10)), 1);


INSERT INTO users (full_name, login, password_hash, role_id) VALUES
('Степанов Михаил Артёмович', '1diph5e@tutanota.com', crypt('8ntwUp', gen_salt('bf', 10)), 2),
('Ворсин Петр Евгеньевич', 'tjde7c@yahoo.com', crypt('YOyhfR', gen_salt('bf', 10)), 2),
('Старикова Елена Павловна', 'wpmrc3do@tutanota.com', crypt('RSbvHv', gen_salt('bf', 10)), 2);

INSERT INTO users (full_name, login, password_hash, role_id) VALUES
('Михайлюк Анна Вячеславовна', '5d4zbu@tutanota.com', crypt('rwVDh9', gen_salt('bf', 10)), 3),
('Ситдикова Елена Анатольевна', 'ptec8ym@yahoo.com', crypt('LdNyos', gen_salt('bf', 10)), 3),
('Ворсин Петр Евгеньевич', '1qz4kw@mail.com', crypt('gynQMT', gen_salt('bf', 10)), 3),
('Старикова Елена Павловна', '4np6se@mail.com', crypt('AtnDjr', gen_salt('bf', 10)), 3);



CREATE OR REPLACE FUNCTION check_user_password(
    p_login TEXT,
    p_password TEXT
)
RETURNS TABLE (
    user_id INTEGER,
    full_name VARCHAR,
    role_id INTEGER,
    role_name VARCHAR
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        u.id,
        u.full_name,
        u.role_id,
        r.name
    FROM users u
    JOIN roles r ON u.role_id = r.id
    WHERE u.login = p_login 
    AND u.password_hash = crypt(p_password, u.password_hash);
END;
$$ LANGUAGE plpgsql;


SELECT * FROM check_user_password('admin', 'admin');
SELECT * FROM check_user_password('1diph5e@tutanota.com', '8ntwUp');
SELECT * FROM check_user_password('5d4zbu@tutanota.com', 'rwVDh9');




CREATE OR REPLACE FUNCTION auto_hash_password()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' OR (TG_OP = 'UPDATE' AND NEW.password_hash != OLD.password_hash) THEN
        -- Проверяем, не захэширован ли уже пароль (примерная проверка)
        IF LENGTH(NEW.password_hash) < 60 THEN
            NEW.password_hash = crypt(NEW.password_hash, gen_salt('bf', 10));
        END IF;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


DROP TRIGGER IF EXISTS hash_user_password_trigger ON users;
CREATE TRIGGER hash_user_password_trigger
    BEFORE INSERT OR UPDATE OF password_hash ON users
    FOR EACH ROW
    EXECUTE FUNCTION auto_hash_password();


UPDATE users 
SET password_hash = crypt(password_hash, gen_salt('bf', 10))
WHERE LENGTH(password_hash) < 60;
