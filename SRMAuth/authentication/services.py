from datetime import timedelta
from core.jwt_auth import create_access_token, create_refresh_token
from core.redis_connection import get_redis_client
from authentication.schemas import LoginRequest, TokenResponse, RefreshTokenRequest, RefreshTokenResponse
from fastapi import HTTPException, status
from core.config import JWTSettings
from datetime import timedelta, datetime
from jose import jwt
from core.security import verify_password
from users.models import UserModel, SensorModel



jwt_settings = JWTSettings() # type: ignore



async def login(login_data: LoginRequest, db) -> TokenResponse:


    """Create access and refresh tokens for the user."""
    # Verify user credentials
    if not verify_user(login_data, db):
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid credentials",
            headers={"WWW-Authenticate": "Bearer"},
        )
    
    # If user exists, generate access and refresh tokens
    
    role = db.query(UserModel.role).filter(UserModel.email == login_data.username).first()
    if not role:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail="User role not found",
            headers={"WWW-Authenticate": "Bearer"},
        )
    
    access_token_expiration = datetime.utcnow() + timedelta(minutes=jwt_settings.JWT_EXPIRATION_TIME)
    refresh_token_expiration = datetime.utcnow() + timedelta(minutes=jwt_settings.JWT_REFRESH_EXPIRATION_TIME)
    token_data = {"sub": login_data.username, "role": role}
    access_token = create_access_token(token_data, access_token_expiration)
    refresh_token = create_refresh_token(token_data, refresh_token_expiration)
    
    # Store tokens in Redis
    redis_client = get_redis_client()
    
    # Store access token (expire in 15 minutes)
    redis_client.setex(
        f"access_token:{login_data.username}", 
        timedelta(minutes=15), 
        access_token
    )
    
    # Store refresh token (expire in 7 days) with
    redis_client.setex(
        f"refresh_token:{login_data.username}", 
        timedelta(days=7), 
        refresh_token
    )
    
    return TokenResponse(
        username=login_data.username,
        access_token=access_token,
        access_token_expiration= access_token_expiration,
        refresh_token=refresh_token,
        refresh_token_expiration=refresh_token_expiration,
        token_type="bearer"
    )

def verify_user(data, db) -> bool:
    """Check if the user exists in the database."""
    # Query the database for the user by email -> unique ID
    user = db.query(UserModel).filter(UserModel.email == data.username).first()

    # If user does not exist, raise an error
    if not user:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail="User not found",
            headers={"WWW-Authenticate": "Bearer"},
        )
    
    # If user exists, verify the password
    if not verify_password(data.password, user.password):
        return False
    
    return True  




async def create_refreshed_access_token(refresh_data: RefreshTokenRequest) -> RefreshTokenResponse:
    

    redis_client = get_redis_client()
    
    # Check if the refresh token exists in Redis with the username
    refresh_token = redis_client.get(f"refresh_token:{refresh_data.username}")
    
    """
    This function will check if the username exists in the Redis store and if so then will check if his refresh token is valid.
    So -> Dopple Check
    """
    # If the refresh token does not exist, raise an error
    if not refresh_token:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid refresh token",
            headers={"WWW-Authenticate": "Bearer"},
        )
    # Verify the refresh token
    token_data = refresh_data.refresh_token
    if token_data != refresh_token:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid refresh token",
            headers={"WWW-Authenticate": "Bearer"},
        )
    
    access_token_expiration = datetime.utcnow() + timedelta(minutes=jwt_settings.JWT_EXPIRATION_TIME)
    new_access_token = create_access_token(
        {"sub": refresh_data.username}, 
        access_token_expiration
    )

    return RefreshTokenResponse(
        username=refresh_data.username,
        access_token=new_access_token,
        access_token_expiration=access_token_expiration,
        token_type="bearer"
    )



async def authenticate_sensor_account(login_data: LoginRequest, db) -> TokenResponse:
    """Authenticate sensor user and return access and refresh tokens."""
    
    # Verify sensor credentials
    if not verify_sensor(login_data, db):
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid credentials",
            headers={"WWW-Authenticate": "Bearer"},
        )
    
    access_token_expiration = datetime.utcnow() + timedelta(minutes=jwt_settings.JWT_EXPIRATION_TIME)
    refresh_token_expiration = datetime.utcnow() + timedelta(minutes=jwt_settings.JWT_REFRESH_EXPIRATION_TIME)
    token_data = {"sub": login_data.username, "role": "sensor"}
    access_token = create_access_token(token_data, access_token_expiration)
    refresh_token = create_refresh_token(token_data, refresh_token_expiration)
    
    # Store tokens in Redis
    redis_client = get_redis_client()
    
    # Store access token (expire in 15 minutes)
    redis_client.setex(
        f"access_token:{login_data.username}", 
        timedelta(days=2), 
        access_token
    )
    
    # Store refresh token (expire in 7 days) with
    redis_client.setex(
        f"refresh_token:{login_data.username}", 
        timedelta(days=7), 
        refresh_token
    )
    
    return TokenResponse(
        username=login_data.username,
        access_token=access_token,
        access_token_expiration= access_token_expiration,
        refresh_token=refresh_token,
        refresh_token_expiration=refresh_token_expiration,
        token_type="bearer"
    )


def verify_sensor(data, db) -> bool:
    """Check if the user exists in the database."""
    # Query the database for the user by email -> unique ID
    account = db.query(SensorModel).filter(SensorModel.sensor_name == data.username).first()

    # If user does not exist, raise an error
    if not account:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail="Account not found",
            headers={"WWW-Authenticate": "Bearer"},
        )
    
    # If user exists, verify the password
    if not verify_password(data.password, account.secret_key):
        return False
    
    return True  
"""async def login(login_data: LoginRequest, db) -> TokenResponse:


    
    # Verify user credentials
    if not verify_user(login_data, db):
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid credentials",
            headers={"WWW-Authenticate": "Bearer"},
        )
    
    # If user exists, generate access and refresh tokens
    
    role = db.query(UserModel.role).filter(UserModel.email == login_data.username).first()
    if not role:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail="User role not found",
            headers={"WWW-Authenticate": "Bearer"},
        )
    
    access_token_expiration = datetime.utcnow() + timedelta(minutes=jwt_settings.JWT_EXPIRATION_TIME)
    refresh_token_expiration = datetime.utcnow() + timedelta(minutes=jwt_settings.JWT_REFRESH_EXPIRATION_TIME)
    token_data = {"sub": login_data.username, "role": role}
    access_token = create_access_token(token_data, access_token_expiration)
    refresh_token = create_refresh_token(token_data, refresh_token_expiration)
    
    # Store tokens in Redis
    redis_client = get_redis_client()
    
    # Store access token (expire in 15 minutes)
    redis_client.setex(
        f"access_token:{login_data.username}", 
        timedelta(minutes=15), 
        access_token
    )
    
    # Store refresh token (expire in 7 days) with
    redis_client.setex(
        f"refresh_token:{login_data.username}", 
        timedelta(days=7), 
        refresh_token
    )
    
    return TokenResponse(
        username=login_data.username,
        access_token=access_token,
        access_token_expiration= access_token_expiration,
        refresh_token=refresh_token,
        refresh_token_expiration=refresh_token_expiration,
        token_type="bearer"
    )"""