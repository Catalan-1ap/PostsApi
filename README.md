
# PostsApi

Simple api for managing posts

# How to launch?

In "Production" Mode:
1. Open terminal in root directory
2. Run `docker-compose --env-file ./docker.env up`
3. Wait until api is available
4. Go to `localhost:8000`
5. Work...
6. `CTRL+C` in terminal to shutdown api
7. `docker-compose down` stops docker containers

In "Development" Mode:
1. Open terminal in root directory
2. Run `docker-compose --env-file ./docker.env up database`,
   if you change env, make sure you update them in `launchSettings.json` also update ApplicationDbContextDesignTimeFactory
3. Wait until postgres is available
4. Start api from your IDE
5. Work...
6. `CTRL+C` in terminal to disconnect from docker container
7. `docker-compose down` stops docker container

## Prerequisites:

- Docker & Docker-Compose
