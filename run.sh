# Run containers
docker start /postgres
docker start /redis
docker start /web.1
docker start /worker.1

# Reverse proxy
echo '127.0.0.1 api.keygen.localhost' | sudo tee -a /etc/hosts
caddy reverse-proxy --from api.keygen.localhost --to :3000