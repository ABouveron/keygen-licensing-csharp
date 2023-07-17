# Run containers
docker start /postgres
docker start /redis
docker start /web.1
docker start /worker.1

# Reverse proxy
caddy reverse-proxy --from api.keygen.localhost --to :3000