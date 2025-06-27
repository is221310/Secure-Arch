# this class was created with extensive help of KI
import os
import unittest
from datetime import datetime, timedelta, timezone
from jose import jwt

# Set env vars BEFORE importing modules that instantiate JWTSettings
os.environ["JWT_SECRET_KEY"] = "testkey"
os.environ["JWT_ALGORITHM"] = "HS256"
os.environ["JWT_EXPIRATION_TIME"] = "10"
os.environ["JWT_REFRESH_EXPIRATION_TIME"] = "60"

class Test_Core_JWTAuth(unittest.TestCase):
    def test_create_access_token(self):
        # Now import the module that uses JWTSettings
        from SRMAuth.core.jwt_auth import create_access_token
        
        # Arrange
        data = {"sub": "user123"}
        expiration_minutes = int(os.environ["JWT_EXPIRATION_TIME"])
        expected_exp = datetime.utcnow() + timedelta(minutes=expiration_minutes)

        # Act
        token = create_access_token(data, expire=None)

        # Assert
        decoded = jwt.decode(token, "testkey", algorithms=["HS256"])
        self.assertEqual(decoded["sub"], "user123")
        self.assertEqual(decoded["type"], "access")
        self.assertAlmostEqual(decoded["exp"], int(expected_exp.timestamp()), delta=5)

    def test_create_refresh_token(self):
        # Now import the module that uses JWTSettings
        from SRMAuth.core.jwt_auth import create_refresh_token

        # Arrange
        data = {"sub": "user123"}  
        expiration_minutes = int(os.environ["JWT_REFRESH_EXPIRATION_TIME"])
        expected_exp = datetime.utcnow() + timedelta(minutes=expiration_minutes)

        # Act
        refreshtoken = create_refresh_token(data, expire=None)

        # Assert
        decoded = jwt.decode(refreshtoken, "testkey", algorithms=["HS256"])    
        self.assertEqual(decoded["sub"], "user123")
        self.assertEqual(decoded["type"], "refresh")
        self.assertAlmostEqual(decoded["exp"], int(expected_exp.timestamp()), delta=5)

if __name__ == '__main__':
    try:
        unittest.main()
    finally:
        # Clean up env vars after tests to avoid side effects
        del os.environ["JWT_SECRET_KEY"]
        del os.environ["JWT_ALGORITHM"]
        del os.environ["JWT_EXPIRATION_TIME"]
        del os.environ["JWT_REFRESH_EXPIRATION_TIME"]
