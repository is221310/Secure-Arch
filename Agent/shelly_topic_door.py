import paho.mqtt.client as mqtt
import requests

MQTT_BROKER = "192.168.0.54"
MQTT_PORT = 1883
MQTT_USERNAME = "daniel_bsa"
MQTT_PASSWORD = "secure_bsa"
MQTT_TOPIC = "shellies/shelly-bsa/sensor/state"  # + = Wildcard für Device ID

TICKET_API_URL = "https://deine-webapp.de/api/ticket"  # <--- ANPASSEN

def on_connect(client, userdata, flags, rc):
    if rc == 0:
        print("Verbunden mit MQTT Broker")
        client.subscribe(MQTT_TOPIC)
    else:
        print(f" Fehler beim Verbinden: {rc}")

def on_message(client, userdata, msg):
    payload = msg.payload.decode()
    topic = msg.topic
    print(f" Nachricht empfangen - Topic: {topic} | Payload: {payload}")

    if payload.lower() == "open":
        data = {
            "title": "Tür offen",
            "description": f"Die Tür wurde geöffnet (Topic: {topic})"
        }

        try:
            r = requests.post(TICKET_API_URL, json=data)
            print(f" Ticket gesendet – Status: {r.status_code}")
        except Exception as e:
            print(f" Fehler beim Senden des Tickets: {e}")

client = mqtt.Client()
client.username_pw_set(MQTT_USERNAME, MQTT_PASSWORD)
client.on_connect = on_connect
client.on_message = on_message

client.connect(MQTT_BROKER, MQTT_PORT, 60)
client.loop_forever()
