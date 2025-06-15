from fastapi import APIRouter, status, Depends
from authentication.schemas import LoginRequest, TokenResponse, RefreshTokenRequest, RefreshTokenResponse
from authentication.services import login, create_refreshed_access_token
from core.sql_connection import get_db
from sqlalchemy.orm import Session




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