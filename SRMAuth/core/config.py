from pydantic_settings import BaseSettings 

"""Configuration classes for the application using Pydantic's BaseSettings."""

class JWTSettings(BaseSettings):
    JWT_SECRET_KEY: str
    JWT_ALGORITHM: str 
    JWT_EXPIRATION_TIME: int
    JWT_REFRESH_EXPIRATION_TIME: int

class redisSettings(BaseSettings):
    REDIS_HOST: str 
    REDIS_PORT: int 
    REDIS_DB: int 
    REDIS_PASSWORD: str
    Decode_Response: bool 
    
    class Config:
        env_file = ".env"

class DatabaseSettings(BaseSettings):
    POSTGRES_HOST: str 
    POSTGRES_PORT: int 
    POSTGRES_DB: str 
    POSTGRES_USER: str 
    POSTGRES_PASSWORD: str
    POSTGRES_Schema: str 

    class Config:
        env_file = ".env"