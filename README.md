# Description
- This is a simple file storage API service. It allows a user to create, update, delete, and list binary files.

- This repository contains a solution (`Agorus.sln`) with three projects a follows:
    - `Agorus.Data`
        - `Migrations`
        - `Models`
        - `Repositories`
    - `Agorus.Data.Tests`
        - `Repositories`
    - `Agorus.Web.Api`
        - `Controllers`
        - `Dtos`

# Endpoints
- Note: run the service and navigate to https://localhost:7290/swagger/index.html for the full Swagger documentation

- Base URL: https://localhost:7290
    - GET:
        - `/files?fileId={fileId:uuid}`
            - Gets file DTOs, optionally filtered by file ID.
        - `/files/{id:int}`
            - Gets a binary file by database ID.
        - `/files/{fileId:uuid}/{version:int}`
            - Gets a binary file by file ID and version.
    - POST
        - `/files`
            - Adds a file to the database.
            - Payload:
                - type: `multipart/form-data`
                - key: `files`, value: `{file name}`
    - PUT
        - `/files/{fileId:uuid}`
            - Updates a file by adding new version of the file to the database.
            - Payload:
                - type: `multipart/form-data`
                - key: `files`, value: `{file name}`
    - DELETE
        - `/files/{id:int}`
            - Deletes a file by database ID.
        - `/files/{fileId:uuid}/{version:int}`
            - Deletes a file by file ID and version.

# Instructions
1. Open `Agorus.sln`.
2. Open the Package Manager Console and run `Update-Database`.
3. Run the `Agorus.Web.Api` project.
