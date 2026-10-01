Local Postgres Database Container
docker run -d --name postgres16_url_shortener -e POSTGRES_DB=UrlShortener -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -v pgdata:/var/lib/postgresql/data postgres:16
