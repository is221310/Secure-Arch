from datetime import timedelta
from core.jwt_auth import create_access_token, create_refresh_token
from core.redis_connection import get_redis_client
from authentication.schemas import LoginRequest, TokenResponse, RefreshTokenRequest, RefreshTokenResponse
from fastapi import HTTPException, status
from core.config import JWTSettings
from datetime import timedelta, datetime
from jose import jwt
jwt_settings = JWTSettings()



async def create_token(login_data: LoginRequest) -> TokenResponse:
    """Create access and refresh tokens for the user."""
    access_token_expiration = datetime.utcnow() + timedelta(minutes=jwt_settings.JWT_EXPIRATION_TIME)
    refresh_token_expiration = datetime.utcnow() + timedelta(minutes=jwt_settings.JWT_REFRESH_EXPIRATION_TIME)
    # Create tokens
    token_data = {"sub": login_data.username}
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

