from fastapi import APIRouter, status, Depends, Request
from fastapi.responses import JSONResponse
from sqlalchemy.orm import Session # type: ignore
from core.sql_connection import get_db
from users.schemas import CreateUserRequest, UpdateUserRequest, CreateSensorRequest, UpdateSensorRequest
from users.services import delete_sensor_account, create_user_account, update_user_account, delete_user_account,update_sensor_account, create_sensor_account




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
    try:
        await create_user_account(data=data, db=db)
        payload = {"message": "User account has been succesfully created."}
        return JSONResponse(content=payload)
    except Exception as e:
        payload = {"message": f"An error occurred while creating the user account. {e}"}
        return JSONResponse(content=payload, status_code=status.HTTP_500_INTERNAL_SERVER_ERROR)

@router.patch('/', status_code=status.HTTP_200_OK)
async def update_user(username: str, data: UpdateUserRequest, db: Session = Depends(get_db)):
    try:
        await update_user_account(username=username, db=db, data=data)
        payload = {"message": f"User account '{username}' has been successfully updated."}
        return JSONResponse(content=payload)
    except Exception as e:
        payload = {"message": "An error occurred while updating the user account."}
        return JSONResponse(content=payload, status_code=status.HTTP_500_INTERNAL_SERVER_ERROR)


@router.delete('/', status_code=status.HTTP_204_NO_CONTENT)
async def delete_user(username: str, db: Session = Depends(get_db)):
    try:
        await delete_user_account(username=username, db=db)
        return JSONResponse(content={"message": f"User '{username}' has been successfully deleted."})
    except Exception as e:
        return JSONResponse(content={"message": "An error occurred while deleting the user account."}, status_code=status.HTTP_500_INTERNAL_SERVER_ERROR)
    


@router.post('/sensor/', status_code=status.HTTP_201_CREATED)
async def create_sensor_user(data: CreateSensorRequest, db: Session = Depends(get_db)):
    try:
        await create_sensor_account(data=data, db=db)
        payload = {"message": "Sensor user account has been successfully created."}
        return JSONResponse(content=payload)
    except Exception as e:
        payload = {"message": f"An error occurred while creating the sensor user account. {e}"}
        return JSONResponse(content=payload, status_code=status.HTTP_500_INTERNAL_SERVER_ERROR)
    
@router.patch('/sensor/', status_code=status.HTTP_200_OK)
async def update_sensor_user(sensorname: str, data: UpdateSensorRequest, db: Session = Depends(get_db)):
    try:
        await update_sensor_account(sensor_name=sensorname, db=db, data=data)
        payload = {"message": f"Sensor user account '{sensorname}' has been successfully updated."}
        return JSONResponse(content=payload)
    except Exception as e:
        payload = {"message": "An error occurred while updating the sensor user account."}
        return JSONResponse(content=payload, status_code=status.HTTP_500_INTERNAL_SERVER_ERROR)
    
@router.delete('/sensor/', status_code=status.HTTP_204_NO_CONTENT)
async def delete_sensor_user(sensorname: str, db: Session = Depends(get_db)):
    try:
        await delete_sensor_account(sensor_name=sensorname, db=db)
        return JSONResponse(content={"message": f"Sensor user '{sensorname}' has been successfully deleted."})
    except Exception as e:
        return JSONResponse(content={"message": "An error occurred while deleting the sensor user account."}, status_code=status.HTTP_500_INTERNAL_SERVER_ERROR)