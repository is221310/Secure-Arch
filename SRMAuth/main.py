from fastapi import FastAPI, HTTPException
from fastapi.responses import JSONResponse
from authentication.routes import router as auth_router
from users.routes import router as user_router

app = FastAPI()

app.include_router(auth_router)
app.include_router(user_router)


@app.get('/')
def health_check(): 
    return JSONResponse(content={"status": "Running!"})