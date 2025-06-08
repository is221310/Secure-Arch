#!/bin/bash

set -e

# Start PostgreSQL
/docker-entrypoint.sh postgres &

# Wait for PostgreSQL to start
until pg_isready -h localhost -p 5432 -q; do
  echo "Waiting for PostgreSQL to start..."
  sleep 2
done

# Run migrations
pg_migrate /migrations

# Stop PostgreSQL
pg_ctl -D "$PGDATA" -m fast -w stop

# Start PostgreSQL again
exec /docker-entrypoint.sh postgres

