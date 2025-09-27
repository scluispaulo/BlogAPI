# BlogAPI
A small RESTful blogging API using .NET 8 and SQLite. Supports basic blog posts and comments.

## Requirements
- .NET 8 SDK;
- (Optional) sqlite browser to inspect `BlogDB.db`;

## How to run
- Clone the repo;
- Run `dotnet run --project src/BlogAPI/BlogAPI.csproj`;
- Navigate to http://localhost:5297/swagger/index.html;

## Next steps
- Data access is straight, it can be moved to a dedicated class;
- Improve pagination with QueryObject;
- Implement HATEOAS;
