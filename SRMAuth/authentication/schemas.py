from pydantic import BaseModel
from datetime import datetime

"""Schema definitions for authentication-related requests and responses."""

class LoginRequest(BaseModel):
    username: str
    password: str

class TokenResponse(BaseModel):
    username: str
    access_token: str
    access_token_expiration: datetime
    refresh_token: str
    refresh_token_expiration: datetime
    token_type: str = "bearer"


class RefreshTokenRequest(BaseModel):
    username: str
    refresh_token: str

class RefreshTokenResponse(BaseModel):
    username: str
    access_token: str
    access_token_expiration: datetime
    token_type: str = "bearer"