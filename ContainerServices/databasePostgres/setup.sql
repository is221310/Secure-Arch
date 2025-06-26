CREATE SCHEMA IF NOT EXISTS securearch;
SET search_path TO securearch;

CREATE TABLE IF NOT EXISTS kunden (
    kunden_id SERIAL PRIMARY KEY,
    kunden_name VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    firstname VARCHAR(50) NOT NULL,
    lastname VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    telephone VARCHAR(20),
    role VARCHAR(20) DEFAULT 'Kunde',
    address TEXT,
    kunden_id INT,
    created TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_kunden FOREIGN KEY (kunden_id) REFERENCES Kunden(kunden_id)
);


CREATE TABLE IF NOT EXISTS sensoren (
    sensor_id SERIAL PRIMARY KEY,
    sensor_name VARCHAR(255) NOT NULL,
    secret_key VARCHAR(255),
    beschreibung TEXT,      
    kunden_id INT,
    ip_addresses JSONB  DEFAULT '[]',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_sensor_kunde FOREIGN KEY (kunden_id) REFERENCES Kunden(kunden_id)
);

CREATE TABLE ip_results (
    id SERIAL PRIMARY KEY,
    sensor_id INTEGER NOT NULL,
    ip_address TEXT NOT NULL,
    status BOOLEAN NOT NULL,
    timestamp TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_ipresults_sensor FOREIGN KEY (sensor_id)
        REFERENCES securearch.sensoren(sensor_id)
        ON DELETE CASCADE
);

CREATE TABLE temperatur (
    id SERIAL PRIMARY KEY,
    sensor_id INTEGER NOT NULL,
    temperatur DOUBLE PRECISION NOT NULL,
    timestamp TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_temperatur_sensor FOREIGN KEY (sensor_id)
        REFERENCES securearch.sensoren(sensor_id)
        ON DELETE CASCADE
);


-- Rest of your INSERT statements...
INSERT INTO Kunden (kunden_name) VALUES
('Musterkunde GmbH'),
('Beispiel AG'),
('Demo Firma');

INSERT INTO Users (firstname, lastname, email, password, telephone, role, address, kunden_id) VALUES
('Max', 'Mustermann', 'max@example.com', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', '0123456789', 'Admin', 'Musterstraße 1, 12345 Musterstadt', 1),
('Admin', 'DemoAdmin', 'Admin@example.com', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', '0123456789', 'Admin', 'Musterstraße 5, 12345 Musterstadt', 1),
('Erika', 'Musterfrau', 'erika@example.com','$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', '0987654321', 'Admin', 'Beispielweg 5, 54321 Beispielstadt', 2),
('Hans', 'Huber', 'hans.huber@example.com','$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', '01234', 'Mitarbeiter', 'Hauptstraße 12, 10115 Berlin', NULL),
('Anna', 'Schmidt', 'anna.schmidt@example.com','$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', '0301234567', 'abc', 'abc', 3),
('Peter', 'Müller', 'peter.mueller@example.com','$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', '015112345678', 'Mitarbeiter', 'Lindenallee 8, 20095 Hamburg', NULL);

INSERT INTO Sensoren (sensor_name, secret_key, beschreibung, kunden_id, ip_addresses)
VALUES 
('Sensor_Room123', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', 'Raum 101', 1, '["192.168.0.101", "192.168.0.102"]'),
('Sensor_Room234', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', 'Keller', 1, '["10.0.0.10", "10.0.0.11"]'),
('Sensor_Room235', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', 'Eingangshalle', 2, '["172.16.5.1"]'),
('Sensor_Room269', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', 'Büro 3.OG', 3, '["192.168.100.10", "192.168.100.11", "192.168.100.12"]'),
('Sensor_Lab101', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', 'Labor EG', 1, '["192.168.1.10", "192.168.1.11"]'),
('Sensor_Serverraum', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', 'Serverraum', 2, '["10.1.1.1"]'),
('Sensor_HalleA', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', 'Produktionshalle A', 3, '["172.20.0.5", "172.20.0.6"]'),
('Sensor_Archiv', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', 'Archivraum', 2, '["192.168.10.10"]'),
('Sensor_Rooftop', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', 'Dachbereich', 2, '["10.0.5.20", "10.0.5.21", "10.0.5.22"]'),
('Sensor_LagerWest', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', 'Lagerhalle West', 3, '["192.168.50.1"]'),
('Sensor_Kantine', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', 'Kantine EG', 3, '["10.2.2.2", "10.2.2.3"]'),
('Sensor_Etage4', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', 'Etage 4 Flur', 2, '["172.16.20.10", "172.16.20.11"]'),
('Sensor_Entwicklung', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', 'Entwicklungsbereich', 2, '["192.168.77.10", "192.168.77.11"]'),
('Sensor_Besprechung1', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', 'Besprechungsraum 1', 3, '["10.10.10.1"]');

INSERT INTO Temperatur ("sensor_id", "temperatur")
VALUES
(1, 21.5), (1, 21.7), (1, 21.9), (1, 22.0), (1, 22.1), (1, 22.3), (1, 21.8), (1, 22.2), (1, 21.6), (1, 21.9),
(2, 19.1), (2, 19.3), (2, 19.4), (2, 19.2), (2, 19.5), (2, 19.6), (2, 19.8), (2, 19.7), (2, 19.9), (2, 20.0),
(3, 17.8), (3, 18.0), (3, 18.2), (3, 18.3), (3, 18.5), (3, 18.1), (3, 18.4), (3, 18.6), (3, 18.7), (3, 18.9),
(4, 20.1), (4, 20.2), (4, 20.3), (4, 20.4), (4, 20.6), (4, 20.7), (4, 20.5), (4, 20.8), (4, 20.9), (4, 21.0),
(5, 23.1), (5, 23.3), (5, 23.4), (5, 23.5), (5, 23.6), (5, 23.7), (5, 23.8), (5, 23.9), (5, 24.0), (5, 24.1),
(6, 24.1), (6, 24.3), (6, 24.2), (6, 24.4), (6, 24.5), (6, 24.6), (6, 24.7), (6, 24.8), (6, 24.9), (6, 25.0),
(7, 22.0), (7, 22.2), (7, 22.4), (7, 22.5), (7, 22.6), (7, 22.7), (7, 22.8), (7, 23.0), (7, 23.1), (7, 23.3),
(8, 21.0), (8, 21.1), (8, 21.2), (8, 21.3), (8, 21.4), (8, 21.5), (8, 21.6), (8, 21.7), (8, 21.8), (8, 22.0),
(9, 20.1), (9, 20.2), (9, 20.3), (9, 20.4), (9, 20.5), (9, 20.6), (9, 20.7), (9, 20.8), (9, 20.9), (9, 21.0),
(10, 19.0), (10, 19.2), (10, 19.3), (10, 19.4), (10, 19.5), (10, 19.6), (10, 19.7), (10, 19.8), (10, 20.0), (10, 20.2),
(11, 25.0), (11, 25.1), (11, 25.2), (11, 25.3), (11, 25.4), (11, 25.5), (11, 25.6), (11, 25.7), (11, 25.8), (11, 25.9),
(12, 26.0), (12, 26.1), (12, 26.2), (12, 26.3), (12, 26.4), (12, 26.5), (12, 26.6), (12, 26.7), (12, 26.8), (12, 27.0),
(13, 23.0), (13, 23.1), (13, 23.2), (13, 23.3), (13, 23.4), (13, 23.5), (13, 23.6), (13, 23.7), (13, 23.8), (13, 24.0),
(14, 22.5), (14, 22.6), (14, 22.7), (14, 22.8), (14, 22.9), (14, 23.0), (14, 23.1), (14, 23.2), (14, 23.3), (14, 23.5);


INSERT INTO ip_results ("sensor_id", "ip_address", "status")
VALUES
-- Sensor 1
(1, '192.168.1.10', TRUE),
(1, '192.168.1.11', FALSE),
(1, '192.168.1.12', TRUE),

-- Sensor 2
(2, '192.168.2.10', TRUE),
(2, '192.168.2.11', TRUE),

-- Sensor 3
(3, '10.0.0.1', TRUE),
(3, '10.0.0.2', FALSE),

-- Sensor 4
(4, '10.0.0.3', TRUE),
(4, '10.0.0.4', TRUE),

-- Sensor 5
(5, '172.16.1.1', TRUE),
(5, '172.16.1.2', TRUE),

-- Sensor 6
(6, '172.16.1.3', FALSE),
(6, '172.16.1.4', TRUE),

-- Sensor 7
(7, '192.168.10.1', TRUE),
(7, '192.168.10.2', TRUE),

-- Sensor 8
(8, '192.168.10.3', TRUE),
(8, '192.168.10.4', FALSE),

-- Sensor 9
(9, '192.168.50.1', TRUE),
(9, '192.168.50.2', TRUE),

-- Sensor 10
(10, '10.10.10.1', TRUE),
(10, '10.10.10.2', FALSE),

-- Sensor 11
(11, '172.20.0.5', TRUE),
(11, '172.20.0.6', TRUE),

-- Sensor 12
(12, '10.0.5.20', TRUE),
(12, '10.0.5.21', TRUE),
(12, '10.0.5.22', FALSE),

-- Sensor 13
(13, '192.168.77.10', TRUE),
(13, '192.168.77.11', FALSE),

-- Sensor 14
(14, '172.16.20.10', TRUE),
(14, '172.16.20.11', TRUE); 
-- Final verification
SELECT 'Setup completed successfully' AS status;