# BLOG WEB APPLICATION
## Tuan Bui Editor

<!-- [![N|Solid](https://cldup.com/dTxpPi9lDf.thumb.png)](https://nodesource.com/products/nsolid) -->
- ✨Magic ✨

## Features
### Back end:
> Part 0: Create Clean Architecture for Asp.net
- Setting up the project structure
- First, create a new ASP.NET Core project and organize it into the following folders:
    + Domain: Contains the domain entities and interfaces.
    + Application: Houses the application services, DTOs, and interfaces.
    + Infrastructure: Contains the implementations of the interfaces defined in the Domain layer.
    + Presentation: Consists of the controllers, views, and view models. 

> Part 1: Create Database schema for blog management Code first
- Create entities for blog project use Confluent API
- ```sh
    ## Remove all applied migrations
    dotnet ef database update 0 --context ApplicationDbContext 

    ## Remove migration files from file system
    dotnet ef migrations remove --context ApplicationDbContext 

    ## Add new migration
    dotnet ef migrations add InitialCreate --context ApplicationDbContext

    ## Apply new migration to database
    dotnet ef database update --context ApplicationDbContext
    ```
- Seed Data
- Auto mapping
>  Part 2: Create API Authentication and Authorization and Roles

> Part 3: Apply design pattern **Unit Of Work**, **Mediator** for web

> Part 4: Apply Unit Test NUnit for web

> Part 5: Create action for admin page
- Category: Add, Update, Delete, Find, Get all
- Post: Add, Update, Delete, Find, Get all
- User account: Add, Update, Delete, Find, Get all
- Decentralization
> Part 6: Create action for User page
- Show category, post, advertise, paging, cache, sale

## Technical

Dillinger uses a number of open source projects to work properly:

- [ReactJS] - HTML enhanced for web apps!
- [Ace Editor] - awesome web-based text editor
- [markdown-it] - Markdown parser done right. Fast and easy to extend.
- [Twitter Bootstrap] - great UI boilerplate for modern web apps
- [.NET CORE 6] - evented I/O for the backend
- [.NET CORE, EF CORE] - 

## Installation
Install the dependencies and devDependencies and start the server.

```sh
cd dillinger
npm i
node app
```

For production environments...

```sh
npm install --production
NODE_ENV=production node app
```

## Plugins

Dillinger is currently extended with the following plugins.
Instructions on how to use them in your own application are linked below.

| Plugin | README |
| ------ | ------ |
| Dropbox | [plugins/dropbox/README.md][PlDb] |
| GitHub | [plugins/github/README.md][PlGh] |
| Google Drive | [plugins/googledrive/README.md][PlGd] |
| OneDrive | [plugins/onedrive/README.md][PlOd] |
| Medium | [plugins/medium/README.md][PlMe] |
| Google Analytics | [plugins/googleanalytics/README.md][PlGa] |

## Development

Want to contribute? Great!

Dillinger uses Gulp + Webpack for fast developing.
Make a change in your file and instantaneously see your updates!

Open your favorite Terminal and run these commands.

First Tab:

```sh
node app
```

Second Tab:

```sh
gulp watch
```

(optional) Third:

```sh
karma test
```

#### Building for source

For production release:

```sh
gulp build --prod
```

Generating pre-built zip archives for distribution:

```sh
gulp build dist --prod
```

## Docker

Dillinger is very easy to install and deploy in a Docker container.

By default, the Docker will expose port 8080, so change this within the
Dockerfile if necessary. When ready, simply use the Dockerfile to
build the image.

```sh
cd dillinger
docker build -t <youruser>/dillinger:${package.json.version} .
```

This will create the dillinger image and pull in the necessary dependencies.
Be sure to swap out `${package.json.version}` with the actual
version of Dillinger.

Once done, run the Docker image and map the port to whatever you wish on
your host. In this example, we simply map port 8000 of the host to
port 8080 of the Docker (or whatever port was exposed in the Dockerfile):

```sh
docker run -d -p 8000:8080 --restart=always --cap-add=SYS_ADMIN --name=dillinger <youruser>/dillinger:${package.json.version}
```

> Note: `--capt-add=SYS-ADMIN` is required for PDF rendering.

Verify the deployment by navigating to your server address in
your preferred browser.

```sh
127.0.0.1:8000
```

## License
