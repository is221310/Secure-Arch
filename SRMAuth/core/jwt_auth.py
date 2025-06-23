from jose import jwt, JWTError
from datetime import datetime, timedelta
from core.config import JWTSettings

jwt_settings = JWTSettings() # type: ignore

def create_access_token(data: dict, expire):
    to_encode = data.copy()
    expire = datetime.utcnow() + timedelta(minutes=jwt_settings.JWT_EXPIRATION_TIME)
    to_encode.update({"exp": expire, "type": "access"})
    encoded_jwt = jwt.encode(to_encode, jwt_settings.JWT_SECRET_KEY, algorithm=jwt_settings.JWT_ALGORITHM)
    print(to_encode) # Debugging line to check the contents of the token
    print("hellow")
    return encoded_jwt

def create_refresh_token(data: dict, expire):
    to_encode = data.copy()
    expire = datetime.utcnow() + timedelta(minutes=jwt_settings.JWT_REFRESH_EXPIRATION_TIME)
    to_encode.update({"exp": expire, "type": "refresh"})
    encoded_jwt = jwt.encode(to_encode, jwt_settings.JWT_SECRET_KEY, algorithm=jwt_settings.JWT_ALGORITHM)
    return encoded_jwt

def get_user_data(token: str):
    try:
        payload = jwt.decode(token, jwt_settings.JWT_SECRET_KEY, algorithms=jwt_settings.JWT_ALGORITHM)
        return payload
    except JWTError:
        return None