CREATE SCHEMA IF NOT EXISTS securearch;
SET search_path TO securearch;

CREATE TABLE IF NOT EXISTS Users (
    id SERIAL PRIMARY KEY,
    firstname VARCHAR(50) NOT NULL,
    lastname VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    telephone VARCHAR(20),
    role VARCHAR(20) DEFAULT 'Kunde',
    address TEXT,
    created TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

INSERT INTO Users (firstname, lastname, email, password, telephone, role, address) VALUES
('Max', 'Mustermann', 'max@example.com', '$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', '0123456789', 'Admin', 'Musterstraße 1, 12345 Musterstadt'),
('Erika', 'Musterfrau', 'erika@example.com','$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', '0987654321', 'Kunde', 'Beispielweg 5, 54321 Beispielstadt'),
('Hans', 'Huber', 'hans.huber@example.com','$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', NULL, 'Mitarbeiter', 'Hauptstraße 12, 10115 Berlin'),
('Anna', 'Schmidt', 'anna.schmidt@example.com','$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', '0301234567', 'Kunde', NULL),
('Peter', 'Müller', 'peter.mueller@example.com','$2y$10$wZgGtX8Yi6DpCMEJTBmE5e6i3CxmuToI/E5tLYIaOhj5xGBO1hNne', '015112345678', 'Mitarbeiter', 'Lindenallee 8, 20095 Hamburg');