# 🔒 SecureArch

Ein IoT-basiertes, Microservice-orientiertes Überwachungssystem zur Absicherung und Beobachtung von Serverräumen – bestehend aus einer Hardware-Appliance beim Kunden (Raspberry Pi,Shelly), einem zentral gehosteten SaaS-Backend sowie einem webbasierten Frontend.

---

## 📚 Inhaltsverzeichnis

- [🔒 SecureArch](#-securearch)
  - [📚 Inhaltsverzeichnis](#-inhaltsverzeichnis)
  - [✨ Features](#-features)
  - [🏗️ Systemarchitektur](#️-systemarchitektur)
  - [📊 ER-Diagramm](#-er-diagramm)
  - [🧱 Klassendiagramm](#-klassendiagramm)
  - [📡 API-Dokumentation](#-api-dokumentation)
    - [Agent → Core](#agent--core)
    - [Frontend → Core](#frontend--core)
    - [Core → Ticket-System](#core--ticket-system)
  - [⚙️ Installation \& Start](#️-installation--start)
    - [Voraussetzungen](#voraussetzungen)
    - [Backend \& Frontend (lokal)](#backend--frontend-lokal)
    - [Agent (z. B. auf Raspberry Pi)](#agent-zb-auf-raspberry-pi)
  - [👥 Teammitglieder](#-teammitglieder)
  - [📄 Lizenz](#-lizenz)
  - [📎 Anhang](#-anhang)

---

## ✨ Features

- **Microservice-Architektur** mit RESTful APIs
    - Agent (Appliance)
    - Core-Service (Business-Logik & zentrale Datenhaltung)
    - Token-Service (Security, Auth, NoSQL)
- **Token-basierte Sicherheit**
- **Frontend (mit Blazor)** mit Benutzer-Rollen:
    - **Kunde**: Einsicht in eigene Sensordaten
    - **Admin**: Volle Konfiguration & Einsicht
- **Shelly-Integration**: Konfiguration & Überwachung von:
    - Türstatus (Event-basiert)
    - Temperatur
    - Helligkeit 
    - Batteriestatus
- **Netzwerküberwachung** durch Agent:
    - Pings an IP-Adressen, zentral definierbar über das Frontend
- **Agent → Core Kommunikation** über **HTTPS REST-API**
- **Automatische Ticket-Erstellung** bei sicherheitsrelevanten Vorfällen
- **Mehrmandantenfähig**
- **CI/CD Pipeline (GitHub Actions)**

---

## 🏗️ Systemarchitektur

![Systemarchitektur](./docs/architektur.png)

---

## 📊 ER-Diagramm

![ER-Diagramm](./docs/er-diagramm.png)

---

## 🧱 Klassendiagramm

![Klassendiagramm](./docs/klassendiagramm.png)

---

## 📡 API-Dokumentation

Alle Services bieten RESTful JSON-APIs. Beispiele:

### Agent → Core

`POST /api/sensordata`
```json
{
  "roomId": "xyz-123",
  "temperature": 22.5,
  "doorOpen": false,
  "timestamp": "2025-06-15T12:00:00Z"
}
```

### Frontend → Core

`GET /api/rooms/{id}/status`  
`POST /api/config/pingtargets`

### Core → Ticket-System

`POST /api/tickets`


---

## ⚙️ Installation & Start

### Voraussetzungen

- Docker & Docker Compose
- .NET 7 SDK
- (Optional) Raspberry Pi + Shelly Device

### Backend & Frontend (lokal)

```bash
git clone https://github.com/yourorg/monitoring-platform.git
cd monitoring-platform
docker-compose up --build
```

Zugriff:
- Frontend: http://localhost:5000
- API Core: http://localhost:5001/api
- Token-Service: http://localhost:5002
- MongoDB, PostgreSQL, etc. auf eigenen Docker-Netzwerken

### Agent (z. B. auf Raspberry Pi)

```bash
cd agent
dotnet run
# Oder als Service einrichten
```

---

## 👥 Teammitglieder

| Name                  | Kontakt               |
|-----------------------|-----------------------|
| Tobias Frank          | is221310@fhstp.ac.at  |
| Daniel Tyraj          | is221345@fhstp.ac.at  |
| Maximilian Eichinger  | is221301@fhstp.ac.at  |
| Abdulmalek Badra      | is231329@fhstp.ac.at  |
| Judith Neuhuber       | is221312@fhstp.ac.at  |
| Haris Cemo            | is221336@fhstp.ac.at  |

---

## 📄 Lizenz

MIT License – siehe `LICENSE`

---

## 📎 Anhang

- `docs/er-diagramm.png`
- `docs/klassendiagramm.png`
- `docs/docker-setup.md`
