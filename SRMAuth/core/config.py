from pydantic_settings import BaseSettings 

"""Configuration classes for the application using Pydantic's BaseSettings."""

class JWTSettings(BaseSettings):
    JWT_SECRET_KEY: str
    JWT_ALGORITHM: str 
    JWT_EXPIRATION_TIME: int
    JWT_REFRESH_EXPIRATION_TIME: int

class redisSettings(BaseSettings):
    REDIS_HOST: str = "localhost"
    REDIS_PORT: int = 6379
    REDIS_DB: int = 0
    REDIS_PASSWORD: str
    Decode_Response: bool = True
    
    class Config:
        env_file = ".env"

