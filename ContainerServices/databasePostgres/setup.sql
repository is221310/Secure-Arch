CREATE SCHEMA IF NOT EXISTS securearch;
SET search_path TO securearch;

CREATE TABLE IF NOT EXISTS Kunden (
    kunden_id SERIAL PRIMARY KEY,
    kunden_name VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS Users (
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
    CONSTRAINT fk_kunden FOREIGN KEY (kunden_id) REFERENCES Kunden(kunden_id)
);

CREATE TABLE IF NOT EXISTS Sensoren (
    sensor_id SERIAL PRIMARY KEY,
    sensor_name VARCHAR(255) NOT NULL,
    beschreibung TEXT,      
    kunden_id INT,
	ip_addresses JSONB  DEFAULT '[]',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_sensor_kunde FOREIGN KEY (kunden_id) REFERENCES Kunden(kunden_id)
);


INSERT INTO Kunden (kunden_name) VALUES
('Musterkunde GmbH'),
('Beispiel AG'),
('Demo Firma');


INSERT INTO Users (firstname, lastname, email, password, telephone, role, address, kunden_id) VALUES
('Max', 'Mustermann', 'max@example.com', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', '0123456789', 'Admin', 'Musterstraße 1, 12345 Musterstadt', 1),
('Erika', 'Musterfrau', 'erika@example.com','$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', '0987654321', 'Kunde', 'Beispielweg 5, 54321 Beispielstadt', 2),
('Hans', 'Huber', 'hans.huber@example.com','$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', '01234', 'Mitarbeiter', 'Hauptstraße 12, 10115 Berlin', NULL),
('Anna', 'Schmidt', 'anna.schmidt@example.com','$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', '0301234567', 'abc', 'abc', 3),
('Peter', 'Müller', 'peter.mueller@example.com','$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', '015112345678', 'Mitarbeiter', 'Lindenallee 8, 20095 Hamburg', NULL);


INSERT INTO Sensoren (sensor_name, beschreibung, kunden_id, ip_addresses)
VALUES 
('Temperatursensor A1', 'Raum 101', 1, '["192.168.0.101", "192.168.0.102"]'),
('Feuchtigkeitssensor B2', 'Keller', 1, '["10.0.0.10", "10.0.0.11"]'),
('Bewegungsmelder C3', 'Eingangshalle', 2, '["172.16.5.1"]'),
('Luftqualitätssensor D4', 'Büro 3.OG', 3, '["192.168.100.10", "192.168.100.11", "192.168.100.12"]');
