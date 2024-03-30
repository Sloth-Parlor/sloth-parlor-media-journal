#!/bin/bash

start_time=$(date +%s)

# Poll for the PostgreSQL service to start
until psql --username=$POSTGRES_USER --dbname=$POSTGRES_DB --command='\q'; do
 
  elapsed_time=$(($(date +%s) - start_time))
  
  if [ $elapsed_time -gt 30 ]; then
    echo "PostgreSQL service is unavailable - timeout"
    exit 1
  fi
  sleep 1
done

# Create the appuser and set its password
psql --username=$POSTGRES_USER --dbname=$POSTGRES_DB <<-EOF 

CREATE USER appuser;
CREATE DATABASE sp_media_journal
  WITH OWNER = appuser;

GRANT ALL PRIVILEGES ON DATABASE sp_media_journal TO appuser;

EOF

psql --username=$user --dbname=$db \
     --command="ALTER USER appuser WITH PASSWORD '$APPUSER_PASSWORD'"

