FROM python:3.9-slim-bullseye

# Systemtools für Ping
RUN apt-get update && apt-get install -y \
    iputils-ping \
#     mosquitto-clients \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Python-Abhängigkeiten
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Skript kopieren
COPY shelly_door_v2.3.py .

CMD ["python", "shelly_door_v2.3.py"]
