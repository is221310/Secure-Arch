from passlib.context import CryptContext


# Create a password context with the default hashing algorithm
pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")
def hash_password(password: str) -> str:
    """
    Hash a password using bcrypt.
    
    :param password: The plain text password to hash.
    :return: The hashed password.
    """
    return pwd_context.hash(password)

def verify_password(plain_password: str, hashed_password: str) -> bool:
    """
    Verify a plain text password against a hashed password.
    
    :param plain_password: The plain text password to verify.
    :param hashed_password: The hashed password to compare against.
    :return: True if the passwords match, False otherwise.
    """
    return pwd_context.verify(plain_password, hashed_password)