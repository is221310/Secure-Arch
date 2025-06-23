from fastapi import APIRouter, status, Depends, Query
from authentication.schemas import LoginRequest, TokenResponse, RefreshTokenRequest, RefreshTokenResponse
from authentication.services import login, create_refreshed_access_token, authenticate_sensor_account
from core.sql_connection import get_db
from sqlalchemy.orm import Session
from core.jwt_auth import get_user_data




router = APIRouter(
    prefix="/auth",
    tags=["Authentication"],
    responses={404: {"description": "Not found"}},
)

@router.post("/token", status_code=status.HTTP_200_OK, response_model=TokenResponse)
async def authenticate_user(data: LoginRequest, db: Session = Depends(get_db)):
    return await login(login_data=data, db=db)

@router.post("/refresh", status_code=status.HTTP_200_OK)
async def refresh_access_token(data: RefreshTokenRequest):
    return await create_refreshed_access_token(data)

@router.get("/verifytoken", status_code=status.HTTP_200_OK, name="Verify Token")
async def verify_token(token: str = Query(..., description="JWT token to verify")):
    """Endpoint to verify the token."""
    return get_user_data(token)

@router.post("/token/sensor", status_code=status.HTTP_200_OK, response_model=TokenResponse)
async def authenticate_sensor_user(data: LoginRequest, db: Session = Depends(get_db)):
    return await authenticate_sensor_account(login_data=data, db=db)


@router.post("/token/sensor/refresh", status_code=status.HTTP_200_OK)
async def refresh_sensor_access_token(data: RefreshTokenRequest):
    return await create_refreshed_access_token(data)

