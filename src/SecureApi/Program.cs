using System.IO;
using System.Net.Mime;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

const string PUBLIC_FILES_PATH = "PublicFiles";
var allowedFilesPath = Path.GetFullPath(PUBLIC_FILES_PATH);

var builder = WebApplication.CreateSlimBuilder(args);

var app = builder.Build();

app.MapGet("/download", async (string fileName, CancellationToken cancellationToken) =>
{
    // Ensure the file name is sanitized
    var sanitizedFileName = Path.GetFileName(fileName);
    var filePath = Path.Combine(PUBLIC_FILES_PATH, sanitizedFileName);


    // Ensure the file path is within the allowed path
    var fullPath = Path.GetFullPath(filePath);
    if(!fullPath.StartsWith(allowedFilesPath))
    {
        return Results.BadRequest("Invalid file path");
    }


    if(!File.Exists(filePath))
    {
        return Results.NotFound("File not found");
    }


    var file = await File.ReadAllBytesAsync(filePath, cancellationToken);
    return Results.File(file, MediaTypeNames.Application.Json);
});

await app.RunAsync();
