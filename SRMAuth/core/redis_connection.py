import redis
from core.config import redisSettings


redis_settings = redisSettings()


redis_client = redis.Redis(
    host=redis_settings.REDIS_HOST,
    port=redis_settings.REDIS_PORT,
    db=redis_settings.REDIS_DB,
    password=redis_settings.REDIS_PASSWORD,
    decode_responses=redis_settings.Decode_Response
)



def get_redis_client():
    """
    Returns the Redis client instance.
    """
    return redis_client