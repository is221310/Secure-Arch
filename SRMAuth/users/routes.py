from fastapi import APIRouter, status, Depends, Request
from fastapi.responses import JSONResponse
from sqlalchemy.orm import Session
from core.sql_connection import get_db
from users.schemas import CreateUserRequest
from users.services import create_user_account




router = APIRouter(
    prefix="/users",
    tags=["Users"],
    responses={404: {"description": "Not found"}},
)

user_router = APIRouter(
    prefix="/users",
    tags=["Users"],
    responses={404: {"description": "Not found"}},
    #dependencies=[Depends(oauth2_scheme)]
)

@router.post('/', status_code=status.HTTP_201_CREATED)
async def create_user(data: CreateUserRequest, db: Session = Depends(get_db)):
    await create_user_account(data=data, db=db)
    payload = {"message": "User account has been succesfully created."}
    return JSONResponse(content=payload)

