from sqlalchemy import Boolean, Column, Integer, String, DateTime, func, ForeignKey
from sqlalchemy.orm import relationship
from core.sql_connection import Base

class UserModel(Base):
    __tablename__ = 'users'
    __table_args__ = {'schema': 'securearch'}
    
    id = Column(Integer, primary_key=True, autoincrement=True)
    firstname = Column(String(50), nullable=False)
    lastname = Column(String(50), nullable=False)
    email = Column(String(100), unique=True, nullable=False)
    password = Column(String(255), nullable=False)
    telephone = Column(String(20), nullable=True)
    role = Column(String(20), nullable=False, default='Kunde')
    address = Column(String, nullable=True)
    kunden_id = Column(Integer, ForeignKey('securearch.kunden.kunden_id'), nullable=True)
    created = Column(DateTime, default=func.now())
    updated = Column(DateTime, default=func.now(), onupdate=func.now())
    
    # Relationship to KundeModel
    kunde = relationship("KundeModel", back_populates="users")

class KundeModel(Base):
    __tablename__ = 'kunden'
    __table_args__ = {'schema': 'securearch'}
    
    kunden_id = Column(Integer, primary_key=True, autoincrement=True)
    kunden_name = Column(String(255), nullable=False)
    created_at = Column(DateTime, default=func.now())
    
    # Relationship to UserModel (one-to-many)
    users = relationship("UserModel", back_populates="kunde")
    # Relationship to SensorModel (one-to-many)
    sensoren = relationship("SensorModel", back_populates="kunde")

class SensorModel(Base):
    __tablename__ = 'sensoren'
    __table_args__ = {'schema': 'securearch'}
    
    sensor_id = Column(Integer, primary_key=True, autoincrement=True)
    sensor_name = Column(String(255), nullable=False)
    secret_key = Column(String(255), nullable=True)
    beschreibung = Column(String, nullable=True)
    kunden_id = Column(Integer, ForeignKey('securearch.kunden.kunden_id'), nullable=True)
    ip_addresses = Column(String, default='[]')  # Store as JSON string
    created_at = Column(DateTime, default=func.now())
    
    # Relationship to KundeModel
    kunde = relationship("KundeModel", back_populates="sensoren")