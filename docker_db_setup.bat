docker compose -f "./DataAccess/db/docker-compose.yml" down
echo Starting PostgreSQL-Container...
docker compose -f "./DataAccess/db/docker-compose.yml" up -d mrp_db 
pause