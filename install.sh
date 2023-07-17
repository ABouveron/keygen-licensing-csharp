#!/bin/bash

printf "This script only requires you to run it on a Ubuntu environment with Docker installed. If not the case, check their documentation at https://docs.docker.com/get-docker/.\n" 

# Initial setup
export KEYGEN_EDITION="CE"
export KEYGEN_MODE="singleplayer"

docker pull keygen/api
export KEYGEN_HOST="api.keygen.localhost"

while IFS= read -r line
do
  value=$(echo "$line" | cut -d'=' -f2)
  data+=("$value")
done < install.env

export KEYGEN_ACCOUNT_ID=${data[0]}

# Removing already existing containers
printf "Attempting to delete already existing containers and their volumes (needed if keygen has already been setuped or installation failed).\n"
docker stop /redis
docker stop /postgres
docker stop /web.1
docker stop /worker.1
docker rm --volumes /redis
docker rm --volumes /postgres
docker rm --volumes /web.1
docker rm --volumes /worker.1
docker volume rm /postgres
docker volume rm /redis

# Postgres
export DATABASE_URL="postgres://postgres:postgres@localhost:5432"
docker run --name postgres -d -p 5432:5432 \
  -v postgres:/var/lib/postgresql/data \
  -e POSTGRES_PASSWORD=postgres \
  postgres

# Redis
export REDIS_URL="redis://localhost:6379"
docker run --name redis -d -p 6379:6379 \
  -v redis:/var/lib/redis/data \
  redis

# Redirect data towards keygen setup
script -q -c "bash install_aux.sh >/dev/null 2>&1" /dev/null <<EOF
${data[0]}
${data[1]}
${data[2]}
EOF

printf "The next steps use docker and requires sudo level permissions.\n"

# Web Container
docker run -d --name web.1 -p 127.0.0.1:3000:3000 -e SECRET_KEY_BASE="${SECRET_KEY_BASE}" \
  -e ENCRYPTION_DETERMINISTIC_KEY="${ENCRYPTION_DETERMINISTIC_KEY}" \
  -e ENCRYPTION_PRIMARY_KEY="${ENCRYPTION_PRIMARY_KEY}" \
  -e ENCRYPTION_KEY_DERIVATION_SALT="${ENCRYPTION_KEY_DERIVATION_SALT}" \
  -e DATABASE_URL="postgres://postgres:postgres@host.docker.internal:5432/postgres" \
  -e REDIS_URL="redis://host.docker.internal:6379" \
  -e KEYGEN_LICENSE_FILE_PATH="${KEYGEN_LICENSE_FILE_PATH}" \
  -e KEYGEN_LICENSE_KEY="${KEYGEN_LICENSE_KEY}" \
  -e KEYGEN_ACCOUNT_ID="${KEYGEN_ACCOUNT_ID}" \
  -e KEYGEN_EDITION="${KEYGEN_EDITION}" \
  -e KEYGEN_MODE="${KEYGEN_MODE}" \
  -e KEYGEN_HOST="${KEYGEN_HOST}" \
  -v /etc/keygen:/etc/keygen \
  --add-host host.docker.internal:host-gateway \
  keygen/api web

# Caddy setup
sudo apt install -y debian-keyring debian-archive-keyring apt-transport-https
curl -1sLf 'https://dl.cloudsmith.io/public/caddy/stable/gpg.key' | sudo gpg --yes --dearmor --keyring caddy-keyring.gpg -o /usr/share/keyrings/caddy-stable-archive-keyring.gpg
curl -1sLf 'https://dl.cloudsmith.io/public/caddy/stable/debian.deb.txt' | sudo tee /etc/apt/sources.list.d/caddy-stable.list
sudo apt update
sudo apt install caddy

# Worker setup
docker run -d --name worker.1 -e SECRET_KEY_BASE="${SECRET_KEY_BASE}" \
  -e ENCRYPTION_DETERMINISTIC_KEY="${ENCRYPTION_DETERMINISTIC_KEY}" \
  -e ENCRYPTION_PRIMARY_KEY="${ENCRYPTION_PRIMARY_KEY}" \
  -e ENCRYPTION_KEY_DERIVATION_SALT="${ENCRYPTION_KEY_DERIVATION_SALT}" \
  -e DATABASE_URL="postgres://postgres:postgres@host.docker.internal:5432/postgres" \
  -e REDIS_URL="redis://host.docker.internal:6379" \
  -e KEYGEN_LICENSE_FILE_PATH="${KEYGEN_LICENSE_FILE_PATH}" \
  -e KEYGEN_LICENSE_KEY="${KEYGEN_LICENSE_KEY}" \
  -e KEYGEN_ACCOUNT_ID="${KEYGEN_ACCOUNT_ID}" \
  -e KEYGEN_EDITION="${KEYGEN_EDITION}" \
  -e KEYGEN_MODE="${KEYGEN_MODE}" \
  -e KEYGEN_HOST="${KEYGEN_HOST}" \
  -v /etc/keygen:/etc/keygen \
  --add-host host.docker.internal:host-gateway \
  keygen/api worker

# Reverse proxy
echo '127.0.0.1 api.keygen.localhost' | sudo tee -a /etc/hosts
caddy reverse-proxy --from api.keygen.localhost --to :3000