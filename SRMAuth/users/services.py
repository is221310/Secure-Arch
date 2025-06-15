
from fastapi.exceptions import HTTPException
from core.security import hash_password
from datetime import datetime
from users.models import UserModel


async def create_user_account(data, db):
    user = db.query(UserModel).filter(UserModel.email == data.email).first()
    if user:
        raise HTTPException(status_code=422, detail="Email is already registered with us.")

    new_user = UserModel( 
        firstname=data.first_name, 
        lastname=data.last_name, 
        email=data.email, 
        password=hash_password(data.password), 
        role=data.role, 
        telephone=data.telephone, 
        address=data.address 
    )

    db.add(new_user)
    db.commit()
    db.refresh(new_user)
    return new_user

