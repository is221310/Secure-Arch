from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker, declarative_base
from typing import Generator
from core.config import DatabaseSettings



sql_settings = DatabaseSettings()

# Create the database URL
DATABASE_URL = f"postgresql://{sql_settings.POSTGRES_USER}:{sql_settings.POSTGRES_PASSWORD}@{sql_settings.POSTGRES_HOST}:{sql_settings.POSTGRES_PORT}/{sql_settings.POSTGRES_DB}"

# Create the SQLAlchemy engine
engine = create_engine(DATABASE_URL,
                        pool_pre_ping=True,
                        pool_recycle=300,
                        pool_size=5,
                        max_overflow=0
                        )

# Create a configured "Session" class
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)
# Create a base class for declarative models
Base = declarative_base()


def get_db() -> Generator:
    """
    Dependency that provides a database session.
    Yields a database session and ensures it is closed after use.
    """
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()