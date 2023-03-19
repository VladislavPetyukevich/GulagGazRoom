docker-compose -f system.docker.local.yml down -v --rmi all
docker-compose -f system.docker.local.yml down --remove-orphans
# docker-compose -f system.docker.local.yml rm -f