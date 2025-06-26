# 🔒 SecureArch

Ein IoT-basiertes, Microservice-orientiertes Überwachungssystem zur Absicherung und Beobachtung von Serverräumen – bestehend aus einer Hardware-Appliance beim Kunden (Raspberry Pi,Shelly), einem zentral gehosteten SaaS-Backend sowie einem webbasierten Frontend.

---

## 📚 Inhaltsverzeichnis

- [🔒 SecureArch](#-securearch)
  - [📚 Inhaltsverzeichnis](#-inhaltsverzeichnis)
  - [✨ Features](#-features)
    - [🧩 Architekturkomponenten](#-architekturkomponenten)
    - [🔐 Sicherheit \& Zugriff](#-sicherheit--zugriff)
    - [📶 IoT-Funktionalität](#-iot-funktionalität)
    - [🎫 Automatisierung \& Multitenancy](#-automatisierung--multitenancy)
    - [⚙️ DevOps](#️-devops)
  - [🏗️ Systemarchitektur](#️-systemarchitektur)
  - [📊 ER-Diagramm](#-er-diagramm)
  - [🧱 Klassendiagramm](#-klassendiagramm)
  - [📡 API-Dokumentation](#-api-dokumentation)
    - [Agent → Core](#agent--core)
    - [Frontend → Core](#frontend--core)
    - [Core → Ticket-System](#core--ticket-system)
  - [⚙️ Installation \& Start](#️-installation--start)
    - [Voraussetzungen](#voraussetzungen)
    - [🛠️ Lokale Ausführung: Core Webapp \& Auth-Service](#️-lokale-ausführung-core-webapp--auth-service)
      - [1. Repository klonen](#1-repository-klonen)
      - [2. Umgebungsvariablen setzen](#2-umgebungsvariablen-setzen)
      - [3. Docker-Container starten](#3-docker-container-starten)
    - [🌐 Lokale Ports \& Services](#-lokale-ports--services)
    - [⚠️ Hinweis: Nur für Entwicklungs- und Demo-Zwecke](#️-hinweis-nur-für-entwicklungs--und-demo-zwecke)
    - [Agent (z. B. auf Raspberry Pi)](#agent-zb-auf-raspberry-pi)
  - [👥 Teammitglieder](#-teammitglieder)
  - [📄 Lizenz](#-lizenz)
  - [📎 Anhang](#-anhang)


---

## ✨ Features

### 🧩 Architekturkomponenten

- **Microservice-Architektur** mit RESTful APIs:
  - **Agent** (Appliance)
  - **Core-Service** (Business-Logik & zentrale Datenhaltung)
  - **Token-Service** (Security, Auth, NoSQL)

📷 *Architekturübersicht:*  
![Architekturdiagramm](./docs/architektur.png)

---

### 🔐 Sicherheit & Zugriff

- **Token-basierte Sicherheit**
- **Frontend (mit Blazor)** mit Benutzer-Rollen:
  - **Kunde**: Einsicht in eigene Sensordaten
  - **Admin**: Volle Konfiguration & Einsicht

📷 *Beispielhafte UI-Ansicht für Rollen:*  
*➤ Screenshots der WebUI mit Kunden- und Adminsicht könnten hier eingefügt werden.*

---

### 📶 IoT-Funktionalität

- **Shelly-Integration**: Konfiguration & Überwachung von:
  - Türstatus (Event-basiert)
  - Temperatur
  - Helligkeit
  - Batteriestatus
- **Netzwerküberwachung** durch Agent:
  - Pings an IP-Adressen, zentral definierbar über das Frontend
- **Agent → Core Kommunikation** über **HTTPS REST-API**

📷 *Geräteintegration (z. B. MQTT, Shelly-Datenfluss):*  
*➤ Diagramm zum Datenfluss zwischen Shelly, MQTT und Agent könnte hier ergänzt werden.*

📷 *Kommunikation Agent ↔ Core inkl. Tokenfluss:*  
*➤ Sequenzdiagramm zur API-Kommunikation mit Tokenhandling wäre hier hilfreich.*

---

### 🎫 Automatisierung & Multitenancy

- **Automatische Ticket-Erstellung** bei sicherheitsrelevanten Vorfällen
- **Mehrmandantenfähig**

---

### ⚙️ DevOps

- **CI Pipeline** via GitHub Actions
- Nutzung von GitHub Secrets und GitHub Container Registry

---

## 🏗️ Systemarchitektur

![Systemarchitektur](./assets/asystemarchitektur.png)

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
- .NET 8 SDK
- (Optional) Raspberry Pi + Shelly Device

### 🛠️ Lokale Ausführung: Core Webapp & Auth-Service

#### 1. Repository klonen

```bash
git clone https://github.com/is221310/Secure-Arch.git
cd Secure-Arch
```

#### 2. Umgebungsvariablen setzen

Kopiere die Beispieldatei `.env.dist` zu `.env`, um die nötigen Umgebungsvariablen bereitzustellen:

```bash
cp .env.dist .env
```

#### 3. Docker-Container starten

```bash
docker-compose up --build
```

---

### 🌐 Lokale Ports & Services

| Service          | Beschreibung               | Port (lokal)            |
| ---------------- | -------------------------- | ----------------------- |
| **Frontend**     | Blazor WebApp UI           | `http://localhost:8080` |
| **Core-Service** | Zentrale Logik & REST-API  | `http://localhost:5236` |
| **Auth-Service** | Authentifizierung & Tokens | `http://localhost:8000` |
| **PostgreSQL**   | Datenbank                  | `localhost:5432`        |
| **Redis**        | Cache / PubSub für Tokens  | `localhost:6379`        |

> 💡 Diese Ports sind in der `docker-compose.yml` definiert und können bei Bedarf angepasst werden.

---

### ⚠️ Hinweis: Nur für Entwicklungs- und Demo-Zwecke

Dieses Setup ist **nicht für die Produktion geeignet**.\
Es fehlen unter anderem:

- Sichere Authentifizierung (TLS, Secrets)
- Rate-Limiting, Monitoring
- Sicherheitsrichtlinien (z. B. Auth für Redis)
- Backups & persistente Volumes außerhalb Docker

🔒 Nutze dieses Setup **nur lokal oder in geschlossenen Testumgebungen**!

  
---

### Agent (z. B. auf Raspberry Pi)
Mosquitto MQTT-Broker auf Raspberry PI OS installiert. Ein Python Script übernimmt die Auswertung und das pushen an die Webapp. Shelly Device verbindet sich inkl. MQTT Auth zum Broker. Broker und Agent (Script) wird in Docker Compose containerisiert.

```bash
cd agent
docker compose up -d
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
