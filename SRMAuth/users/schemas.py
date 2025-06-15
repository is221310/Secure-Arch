from pydantic import BaseModel, EmailStr

class CreateUserRequest(BaseModel):
    first_name: str
    last_name: str
    email: EmailStr
    role: str
    password: str
    telephone: str
    address: str




""""
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