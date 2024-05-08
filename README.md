## Web Apis for a simple blogging app using ASP .NET 8

Web Apis for a simple blogging app using ASP .NET 8, The frontend I made for this can be found here:
**https://github.com/prashantrahul141/dotnet-t3-blogs**


This project uses the following packages.

- BCrypt.Net-Next
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Design
- Npgsql.EntityFrameworkCore.PostgreSQL
- Swashbuckle.AspNetCore
- Swashbuckle.AspNetCore.Filters

### Building

Make sure you .NET 8 installed, you can read installation instructions from here : https://learn.microsoft.com/en-us/dotnet/core/install/

You can also make sure it's installed by running dotnet
```sh
$ dotnet
```

Add the following environment variables to `.env` file, this will get checked at runtime before starting the application using [LoadEnv.cs](https://github.com/prashantrahul141/BlogWebApiDotNet/blob/main/Utils/LoadEnv.cs)
```env
# DB using Postgres.
PG_PASSWORD=password of postgres instance
PG_HOST=hostname of db instance
PG_DATABASE=database name
PG_USERNAME=username of db instance
PG_PORT=port of db instance

# JWT Token for Auth.
JWT_ISSUER=http://localhost:3000
JWT_AUD=http://localhost:3000

# Can be created at https://generate-secret.vercel.app/64
JWT_KEY=
```

Run build
```sh
dotnet build
```

Migrate database, for this you will need to install EF, [Install instructions](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
```sh
$ dotnet ef migrations add InitialMigration
```
Then update
```sh
$ dotnet ef database update
```

Then finnaly run the application
```sh
$ dotnet run
```

Swagger UI should automatically open in your default browser at http://localhost:3000/swagger/index.html
