CREATE SCHEMA IF NOT EXISTS securearch;
SET search_path TO securearch;
CREATE TABLE IF NOT EXISTS kunden (
    id SERIAL PRIMARY KEY,
    vorname VARCHAR(50) NOT NULL,
    nachname VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    telefon VARCHAR(20),
    adresse TEXT,
    erstellt_am TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

INSERT INTO kunden (vorname, nachname, email, telefon, adresse) VALUES
('Max', 'Mustermann', 'max@example.com', '0123456789', 'Musterstraße 1, 12345 Musterstadt'),
('Erika', 'Musterfrau', 'erika@example.com', '0987654321', 'Beispielweg 5, 54321 Beispielstadt'),
('Hans', 'Huber', 'hans.huber@example.com', NULL, 'Hauptstraße 12, 10115 Berlin'),
('Anna', 'Schmidt', 'anna.schmidt@example.com', '0301234567', NULL),
('Peter', 'Müller', 'peter.mueller@example.com', '015112345678', 'Lindenallee 8, 20095 Hamburg');
