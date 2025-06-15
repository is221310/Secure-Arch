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

