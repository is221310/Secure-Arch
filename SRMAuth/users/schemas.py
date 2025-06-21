from pydantic import BaseModel, EmailStr
from typing import Optional

class CreateUserRequest(BaseModel):
    firstname: str
    lastname: str
    email: EmailStr
    role: str
    password: str
    telephone: str
    address: str
    kunden_id: Optional[int] = None

class UpdateUserRequest(BaseModel):
    firstname: Optional[str]
    lastname: Optional[str]
    email: Optional[EmailStr]
    role: Optional[str]
    password: Optional[str]
    telephone: Optional[str]
    address: Optional[str]
    kunden_id: Optional[int] = None


class CreateSensorRequest(BaseModel):
    sensor_name: str
    secret_key: str
    beschreibung: Optional[str] = None
    kunden_id: Optional[int] = None
    ip_addresses: Optional[list[str]] = []

class UpdateSensorRequest(BaseModel):
    sensor_name: Optional[str]
    secret_key: Optional[str]
    beschreibung: Optional[str] = None
    kunden_id: Optional[int] = None
    ip_addresses: Optional[list[str]] = []


