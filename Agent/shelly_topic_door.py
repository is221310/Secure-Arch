from dotenv import load_dotenv
import os
import paho.mqtt.client as mqtt
import requests

#.env laden
load_dotenv()

MQTT_BROKER   = os.getenv("MQTT_BROKER")
MQTT_PORT     = int(os.getenv("MQTT_PORT", 1883))
MQTT_USERNAME = os.getenv("MQTT_USERNAME")
MQTT_PASSWORD = os.getenv("MQTT_PASSWORD")

TICKET_API_URL = os.getenv("TICKET_API_URL")
API_TOKEN      = os.getenv("API_TOKEN")  # Optional, je nach Webapp

MQTT_TOPIC = "shellies/shelly-bsa/sensor/state"

def on_connect(client, userdata, flags, rc):
    if rc == 0:
        print(" Verbunden mit MQTT Broker")
        client.subscribe(MQTT_TOPIC)
    else:
        print(f" Fehler beim Verbinden: {rc}")

def on_message(client, userdata, msg):
    payload = msg.payload.decode()
    topic = msg.topic
    print(f"Empfangen – Topic: {topic} | Payload: {payload}")

    if payload.lower() == "open":
        data = {
            "title": "Tür offen",
            "description": f"Die Tür wurde geöffnet (Topic: {topic})"
        }

        headers = {
            "Content-Type": "application/json"
        }

        if API_TOKEN:
            headers["Authorization"] = f"Bearer {API_TOKEN}"

        try:
            r = requests.post(TICKET_API_URL, json=data, headers=headers)
            print(f" Ticket gesendet – Status: {r.status_code}")
        except Exception as e:
            print(f" Fehler beim Senden: {e}")

client = mqtt.Client()
client.username_pw_set(MQTT_USERNAME, MQTT_PASSWORD)
client.on_connect = on_connect
client.on_message = on_message

client.connect(MQTT_BROKER, MQTT_PORT, 60)
client.loop_forever()