from fastapi.exceptions import HTTPException
from core.security import hash_password
from datetime import datetime
from users.models import UserModel, SensorModel
import json


async def create_user_account(data, db):
    user = db.query(UserModel).filter(UserModel.email == data.email).first()
    if user:
        raise HTTPException(status_code=422, detail="Email is already registered with us.")

    new_user = UserModel( 
        firstname=data.firstname,
        lastname=data.lastname,
        email=data.email, 
        password=hash_password(data.password), 
        role=data.role, 
        telephone=data.telephone, 
        address=data.address,
        kunden_id=data.kunden_id
    )
    
    db.add(new_user)
    db.commit()
    db.refresh(new_user)
    return new_user


async def update_user_account(username: str, db, data):
    user = db.query(UserModel).filter(UserModel.email == username).first()
    if not user:
        raise HTTPException(status_code=404, detail="User not found.")

    user.firstname = data.firstname
    user.lastname = data.lastname
    user.email = data.email
    user.password = hash_password(data.password)
    user.role = data.role
    user.telephone = data.telephone
    user.address = data.address
    user.updated = datetime.utcnow()
    user.kunden_id = data.kunden_id

    db.add(user)
    db.commit()
    db.refresh(user)
    return user


async def delete_user_account(username: str, db):
    user = db.query(UserModel).filter(UserModel.email == username).first()
    if not user:
        raise HTTPException(status_code=404, detail="User not found.")

    db.delete(user)
    db.commit()
    return {"message": f"User '{username}' has been successfully deleted."}


async def create_sensor_account(data, db):
    # Check if sensor already exists by sensor_name, not email
    sensor = db.query(SensorModel).filter(SensorModel.sensor_name == data.sensor_name).first()
    if sensor:
        raise HTTPException(status_code=422, detail="Sensor with this name already exists.")

    # Convert IP addresses list to JSON string format
    ip_addresses_json = json.dumps(data.ip_addresses) if hasattr(data, 'ip_addresses') and data.ip_addresses else '[]'

    new_sensor = SensorModel(
        sensor_name=data.sensor_name,
        secret_key=hash_password(data.secret_key) if hasattr(data, 'secret_key') and data.secret_key else None,
        beschreibung=data.beschreibung if hasattr(data, 'beschreibung') else None,
        kunden_id=data.kunden_id if hasattr(data, 'kunden_id') else None,
        ip_addresses=ip_addresses_json
    )
    
    db.add(new_sensor)
    db.commit()
    db.refresh(new_sensor)
    return new_sensor


async def update_sensor_account(sensor_name: str, db, data):
    sensor = db.query(SensorModel).filter(SensorModel.sensor_name == sensor_name).first()
    if not sensor:
        raise HTTPException(status_code=404, detail="Sensor not found.")

    # Convert IP addresses list to JSON string format
    ip_addresses_json = json.dumps(data.ip_addresses) if hasattr(data, 'ip_addresses') and data.ip_addresses else '[]'

    sensor.sensor_name = data.sensor_name
    sensor.secret_key = hash_password(data.secret_key) if hasattr(data, 'secret_key') and data.secret_key else sensor.secret_key
    sensor.beschreibung = data.beschreibung if hasattr(data, 'beschreibung') else sensor.beschreibung
    sensor.kunden_id = data.kunden_id if hasattr(data, 'kunden_id') else sensor.kunden_id
    sensor.ip_addresses = ip_addresses_json
    # Don't update created_at - it should remain the original creation time

    db.add(sensor)
    db.commit()
    db.refresh(sensor)
    return sensor


async def delete_sensor_account(sensor_name: str, db):
    sensor = db.query(SensorModel).filter(SensorModel.sensor_name == sensor_name).first()
    if not sensor:
        raise HTTPException(status_code=404, detail="Sensor not found.")

    db.delete(sensor)
    db.commit()
    return {"message": f"Sensor '{sensor_name}' has been successfully deleted."}