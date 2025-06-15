from pydantic import BaseModel, EmailStr

class CreateUserRequest(BaseModel):
    first_name: str
    last_name: str
    email: EmailStr
    role: str
    password: str
    telephone: str
    address: str


