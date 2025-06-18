from dotenv import load_dotenv
import os, time, threading
import paho.mqtt.client as mqtt
import requests
from datetime import datetime

# Konfiguration laden
load_dotenv()

MQTT_BROKER   = os.getenv("MQTT_BROKER")
MQTT_PORT     = int(os.getenv("MQTT_PORT", 1883))
MQTT_USERNAME = os.getenv("MQTT_USERNAME")
MQTT_PASSWORD = os.getenv("MQTT_PASSWORD")

TICKET_API_URL = os.getenv("TICKET_API_URL")
API_TOKEN      = os.getenv("API_TOKEN", "")
TEMP_INTERVAL  = int(os.getenv("TEMP_INTERVAL", 60))

# MQTT Topics
TOPIC_DOOR  = "shellies/shelly-bsa/sensor/state"
TOPIC_TEMP  = "shellies/shelly-bsa/sensor/temperature"

# Globale Variablen
latest_temperature = None
lock = threading.Lock()

# Hilfsfunktionen
def send_to_api(payload: dict):
    headers = {"Content-Type": "application/json"}
    if API_TOKEN:
        headers["Authorization"] = f"Bearer {API_TOKEN}"
    try:
        r = requests.post(TICKET_API_URL, json=payload, headers=headers, timeout=10)
        print(f" API Status {r.status_code}")
    except Exception as e:
        print(f"  API Fehler: {e}")

def door_open_event(topic):
    data = {
        "title": "Tür offen",
        "description": f"Die Tür wurde geöffnet ({topic})",
        "timestamp": datetime.utcnow().isoformat()
    }
    send_to_api(data)

def temp_interval_sender():
    """Hintergrund Thread: schickt alle TEMP_INTERVAL Sekunden den letzten Temp Wert."""
    while True:
        time.sleep(TEMP_INTERVAL)
        with lock:
            temp = latest_temperature
        if temp is None:
            print("  Noch kein Temperaturwert empfangen – überspringe Senden.")
            continue
        data = {
            "title": "Temperatur Update",
            "description": f"Aktueller Wert: {temp} °C",
            "value": temp,
            "unit": "°C",
            "timestamp": datetime.utcnow().isoformat()
        }
        print(f"  Sende Temperatur {temp}°C an Web API")
        send_to_api(data)


# MQTT Callbacks
def on_connect(client, userdata, flags, rc):
    if rc == 0:
        print(" MQTT Broker verbunden")
        client.subscribe([(TOPIC_DOOR, 0), (TOPIC_TEMP, 0)])
    else:
        print(f" MQTT Verbindungsfehler: {rc}")

def on_message(client, userdata, msg):
    global latest_temperature
    payload = msg.payload.decode().strip()
    topic   = msg.topic

    if topic.endswith("/sensor/state") and payload.lower() == "open":
        print(f" Tür geöffnet – Topic: {topic}")
        door_open_event(topic)

    elif topic.endswith("/sensor/temperature"):
        try:
            temp = float(payload)
            with lock:
                latest_temperature = temp
            print(f"  Neue Temperatur: {temp}°C")
        except ValueError:
            print(f"  Ungültiger Temp Payload: {payload}")

# Hauptprogramm
client = mqtt.Client()
client.username_pw_set(MQTT_USERNAME, MQTT_PASSWORD)
client.on_connect = on_connect
client.on_message = on_message
client.connect(MQTT_BROKER, MQTT_PORT, 60)

# Starte Temperatur Sender Thread
threading.Thread(target=temp_interval_sender, daemon=True).start()

# MQTT Loop (blockierend)
client.loop_forever()