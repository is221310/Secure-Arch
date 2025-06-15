from sqlalchemy import Boolean, Column, Integer, String, DateTime, func
from datetime import datetime
from core.sql_connection import Base



class UserModel(Base):
    __tablename__ = 'users'
    __table_args__ = {'schema': 'securearch'}
    id = Column(Integer, primary_key=True, autoincrement=True)
    firstname = Column(String(50), nullable=False)
    lastname = Column(String(50), nullable=False)
    email = Column(String(100), unique=True, nullable=False)
    password = Column(String(255), nullable=False)  # Store hashed password
    telephone = Column(String(20), nullable=True)
    role = Column(String(20))
    address = Column(String, nullable=True)
    created = Column(DateTime, default=func.now())




#Users (firstname, lastname, email, password, telefon, role, address) 


"""
CREATE TABLE IF NOT EXISTS Users (
    id SERIAL PRIMARY KEY,
    firstname VARCHAR(50) NOT NULL,
    lastname VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    telefon VARCHAR(20),
    role VARCHAR(20) DEFAULT 'Kunde',
    address TEXT,
    created TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

"""