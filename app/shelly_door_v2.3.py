# this script was created with the help of KI

import os
import time
import json
import threading
import requests
import paho.mqtt.client as mqtt
import urllib3
import subprocess
from dotenv import load_dotenv

# Konfiguration
load_dotenv()
BASE_URL = "http://192.168.0.51:5236"
SESSION = requests.Session()
SESSION.verify = False  # Nur für Selbstsignierte Zertifikate

# Globale Variablen
ip_list = []
last_door_state = None
DEVICE_ID = "shelly-bsa"  # Dein Shelly-Gerätename
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

# Authentifizierung und Konfiguration
def authenticate():
    try:
        response = SESSION.post(
            f"{BASE_URL}/CoreService/loginsensor",
            json={
                "username": os.getenv("WEBAPP_USER"),
                "password": os.getenv("WEBAPP_PASSWORD")
            }
        )
        response.raise_for_status()
        print("Authentifizierung erfolgreich")
        return True
    except Exception as e:
        print(f"Fehler bei Authentifizierung: {str(e)}")
        return False

def fetch_config():
    global ip_list
    try:
        response = SESSION.get(
            f"{BASE_URL}/DataService/getConfig/{os.getenv('WEBAPP_USER')}"
        )
        response.raise_for_status()
        ip_list = response.json()
        print(f"IP-Liste abgerufen: {ip_list}")
        return True
    except Exception as e:
        print(f"Fehler beim Abrufen der Konfiguration: {str(e)}")
        return False

# Ping und Datenübermittlung

def ping_ips():
    results = {}
    for ip in ip_list:
        try:
            # Linux-Ping mit 1 Paket (-c 1), Timeout 1 Sekunde (-W 1)
            result = subprocess.run(['ping', '-c', '1', '-W', '1', ip],
                                  stdout=subprocess.PIPE,
                                  stderr=subprocess.PIPE,
                                  text=True)
            
            # Erfolg wenn returncode 0 UND "1 received" in der Ausgabe
            if result.returncode == 0 and "1 received" in result.stdout:
                results[ip] = "true"
            else:
                results[ip] = "false"
                
            # Debug-Ausgabe
            print(f"Ping {ip}: {results[ip]} (Code: {result.returncode})")
            
        except Exception as e:
            print(f"Fehler beim Ping {ip}: {str(e)}")
            results[ip] = "false"
    return results

#def ping_ips():
#    results = {}
#    for ip in ip_list:
#        try:
#            response = ping(ip, timeout=1)
#            results[ip] = "true" if response is not "Destination Host Unreachable" else "false"
#        except:
#            results[ip] = "false"
#    return results

def send_ping_results():
    while True:
        try:
            if ip_list:  # Nur senden wenn IP-Liste vorhanden
                results = ping_ips()
                response = SESSION.post(
                    f"{BASE_URL}/DataService/ipresults",
                    json=results
                )
                response.raise_for_status()
                print(f"Ping-Ergebnisse gesendet: {results}")
            else:
                print("Keine IPs zum Pingen konfiguriert")
        except Exception as e:
            print(f"Fehler beim Senden der Ping-Ergebnisse: {str(e)}")
        time.sleep(5)

# MQTT-Handler für Shelly
def on_connect(client, userdata, flags, rc):
    if rc == 0:
        print("Verbunden mit MQTT Broker")
        # Topics für shelly-bsa
        client.subscribe([
            (f"shellies/{DEVICE_ID}/sensor/state", 0),         # Türstatus
            (f"shellies/{DEVICE_ID}/sensor/temperature", 0),   # Temperatur
            (f"shellies/{DEVICE_ID}/online", 0)                # Online-Status
        ])
    else:
        print(f"MQTT-Verbindungsfehler: Code {rc}")

def on_message(client, userdata, msg):
    global last_door_state
    
    try:
        topic = msg.topic
        payload = msg.payload.decode()
        print(f"MQTT-Nachricht [{topic}]: {payload}")
        
        # Türstatus
        if topic.endswith("/sensor/state"):
            if payload == "open" and last_door_state != "open":
                try:
                    response = SESSION.post(
                        f"{BASE_URL}/DataService/door_alarm",
                        json={"door_opened": "true"}
                    )
                    response.raise_for_status()
                    print("Türöffnung gemeldet")
                except Exception as e:
                    print(f"Fehler beim Senden des Türstatus: {str(e)}")
            last_door_state = payload
        
        # Temperatur
        elif topic.endswith("/sensor/temperature"):
            try:
                response = SESSION.post(
                    f"{BASE_URL}/DataService/temperatur",
                    json={"temperatur": payload}
                )
                response.raise_for_status()
                print(f"Temperatur gemeldet: {payload}°C")
            except Exception as e:
                print(f"Fehler beim Senden der Temperatur: {str(e)}")
        
        # Online-Status
        elif topic.endswith("/online"):
            print(f"Shelly Online-Status: {'Verbunden' if payload == 'true' else 'Getrennt'}")
            
    except Exception as e:
        print(f"Verarbeitungsfehler: {str(e)}")

# Hauptfunktion
def main():
    # Initiale Authentifizierung
    if not authenticate():
        print("Versuche erneut in 10 Sekunden...")
        time.sleep(10)
        authenticate()
    
    # Konfiguration abrufen
    if not fetch_config():
        print("Versuche erneut in 10 Sekunden...")
        time.sleep(10)
        fetch_config()
    
    # Starte Ping-Service im Hintergrund
    threading.Thread(target=send_ping_results, daemon=True).start()
    print("Ping-Service gestartet")
    
    # MQTT-Client konfigurieren
    client = mqtt.Client()
    client.username_pw_set(
        os.getenv("MQTT_USER"),
        os.getenv("MQTT_PASSWORD")
    )
    client.on_connect = on_connect
    client.on_message = on_message

########################    DEBUG ###############

#    print("\n=== MQTT-Verbindungsparameter ===")
#    print(f"Broker: {os.getenv('MQTT_BROKER')}")
#    print(f"User: {os.getenv('MQTT_USER')}")
#    print(f"Passwort gesetzt: {bool(os.getenv('MQTT_PASSWORD'))}")

########################    DEBUG ###############
    
    try:
    	print("\nVerbindung wird hergestellt...")
    	client.connect(os.getenv("MQTT_BROKER"), 1883, 60)
    	client.loop_forever()

    except KeyboardInterrupt:  # STRG+C abfangen
        print("\nProgramm wird ordnungsgemäß beendet...")
        client.disconnect()  # MQTT-Verbindung sauber trennen

    except Exception as e:
        print(f"Schwerer MQTT-Fehler: {str(e)}")
        print("Neustart in 30 Sekunden...")
        time.sleep(30)
        main()  # Automatischer Neustart

if __name__ == "__main__":
    print("Agent-Service gestartet")
    main()
