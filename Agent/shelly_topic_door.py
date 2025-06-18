from dotenv import load_dotenv
import os, time, threading, json, sys
import requests, urllib3
from pythonping import ping
import paho.mqtt.client as mqtt
from datetime import datetime

# Konfiguration laden
load_dotenv()

# MQTT
MQTT_BROKER   = os.getenv("MQTT_BROKER")
MQTT_PORT     = int(os.getenv("MQTT_PORT", 1883))
MQTT_USERNAME = os.getenv("MQTT_USERNAME")
MQTT_PASSWORD = os.getenv("MQTT_PASSWORD")

# Web-API
BASE_URL          = os.getenv("BASE_URL").rstrip("/")
LOGIN_EP          = os.getenv("LOGIN_ENDPOINT", "/CoreService/login")
CONFIG_EP         = os.getenv("CONFIG_ENDPOINT", "/DataService/getConfig")
IPRESULT_EP       = os.getenv("IPRESULT_ENDPOINT", "/DataService/ipresults")
TEMP_EP           = os.getenv("TEMP_ENDPOINT", "/DataService/temperatur")
DOOR_EP           = os.getenv("DOOR_ENDPOINT", "/DataService/door")
API_USER          = os.getenv("API_USER")
API_PASS          = os.getenv("API_PASS")
VERIFY_SSL        = os.getenv("VERIFY_SSL", "true").lower() == "true"

PING_INTERVAL     = int(os.getenv("PING_INTERVAL", 5))

if not VERIFY_SSL:
    urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

# HTTP-Session mit Cookie-Handling
session = requests.Session()
session.verify = VERIFY_SSL

def api_login():
    url = f"{BASE_URL}{LOGIN_EP}"
    payload = {"username": API_USER, "password": API_PASS}
    r = session.post(url, json=payload, timeout=10)
    if r.status_code != 200:
        print(f"Login fehlgeschlagen ({r.status_code}) – Script wird beendet")
        sys.exit(1)
    print("Login erfolgreich – Cookie erhalten")

def get_ip_list():
    url = f"{BASE_URL}{CONFIG_EP}/{API_USER}"
    r = session.get(url, timeout=10)
    if r.status_code != 200:
        print(f"Konnte IP-Liste nicht abrufen ({r.status_code})")
        return []
    try:
        ip_list = r.json()
        print(f"IP-Liste geladen: {ip_list}")
        return ip_list
    except Exception as e:
        print(f"Fehler beim Parsen der IP-Liste: {e}")
        return []

def ping_loop(ip_list):
    while True:
        results = {}
        for ip in ip_list:
            try:
                reply = ping(ip, count=1, timeout=1, verbose=False)
                reachable = reply.success()
            except Exception:
                reachable = False
            results[ip] = "true" if reachable else "false"
        try:
            r = session.post(f"{BASE_URL}{IPRESULT_EP}", json=results, timeout=10)
            print(f"Ping-Ergebnisse gesendet ({r.status_code}): {results}")
        except Exception as e:
            print(f"Fehler beim Senden der Ping-Ergebnisse: {e}")
        time.sleep(PING_INTERVAL)

# MQTT
TOPIC_DOOR = "shellies/shellydw2-+/sensor/state"
TOPIC_TEMP = "shellies/shellydw2-+/sensor/temperature"

def on_connect(client, userdata, flags, rc):
    if rc == 0:
        client.subscribe([(TOPIC_DOOR, 0), (TOPIC_TEMP, 0)])
        print("MQTT verbunden und Topics abonniert")
    else:
        print(f"MQTT-Verbindungsfehler: {rc}")

def on_message(client, userdata, msg):
    payload = msg.payload.decode().strip()
    topic   = msg.topic

    if topic.endswith("/sensor/state") and payload.lower() == "open":
        url = f"{BASE_URL}{DOOR_EP}"
        data = {"door_opened": "true", "timestamp": datetime.utcnow().isoformat()}
        try:
            r = session.post(url, json=data, timeout=10)
            print(f"Tür geöffnet gemeldet ({r.status_code})")
        except Exception as e:
            print(f"Fehler beim Tür-Event: {e}")

    elif topic.endswith("/sensor/temperature"):
        try:
            temp_val = float(payload)
        except ValueError:
            print(f"Ungültiger Temperaturwert: {payload}")
            return
        url = f"{BASE_URL}{TEMP_EP}"
        data = {"temp": f"{temp_val}"}
        try:
            r = session.post(url, json=data, timeout=10)
            print(f"Temperatur {temp_val} °C gesendet ({r.status_code})")
        except Exception as e:
            print(f"Fehler beim Senden der Temperatur: {e}")

def main():
    api_login()
    ip_list = get_ip_list()
    if not ip_list:
        print("Keine IPs erhalten – Script wird beendet")
        return

    threading.Thread(target=ping_loop, args=(ip_list,), daemon=True).start()

    client = mqtt.Client()
    client.username_pw_set(MQTT_USERNAME, MQTT_PASSWORD)
    client.on_connect = on_connect
    client.on_message = on_message
    client.connect(MQTT_BROKER, MQTT_PORT, 60)
    client.loop_forever()

if __name__ == "__main__":
    main()