from fastapi import APIRouter, status
from authentication.schemas import LoginRequest, TokenResponse, RefreshTokenRequest, RefreshTokenResponse
from authentication.services import create_token, create_refreshed_access_token




router = APIRouter(
    prefix="/auth",
    tags=["Authentication"],
    responses={404: {"description": "Not found"}},
)

@router.post("/token", status_code=status.HTTP_200_OK, response_model=TokenResponse)
async def authenticate_user(data: LoginRequest):
    return await create_token(data)

@router.post("/refresh", status_code=status.HTTP_200_OK)
async def refresh_access_token(data: RefreshTokenRequest):
    return await create_refreshed_access_token(data)