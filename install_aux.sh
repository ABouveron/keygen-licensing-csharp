# Keygen setup
docker run --rm -it -e SECRET_KEY_BASE="$(openssl rand -hex 64)" \
  -e ENCRYPTION_DETERMINISTIC_KEY="$(openssl rand -base64 32)" \
  -e ENCRYPTION_PRIMARY_KEY="$(openssl rand -base64 32)" \
  -e ENCRYPTION_KEY_DERIVATION_SALT="$(openssl rand -base64 32)" \
  -e DATABASE_URL="postgres://postgres:postgres@host.docker.internal:5432/postgres" \
  -e REDIS_URL="redis://host.docker.internal:6379" \
  -e KEYGEN_HOST="api.keygen.localhost" \
  -e KEYGEN_MODE="singleplayer" \
  -e KEYGEN_EDITION="CE" \
  --add-host host.docker.internal:host-gateway \
  keygen/api setup