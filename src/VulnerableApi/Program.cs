using System.IO;
using System.Net.Mime;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

const string PUBLIC_FILES_PATH = "PublicFiles";

var builder = WebApplication.CreateSlimBuilder(args);

var app = builder.Build();

app.MapGet("/download", async (string fileName, CancellationToken cancellationToken) =>
{
    var filePath = Path.Combine(PUBLIC_FILES_PATH, fileName);
    if(!File.Exists(filePath))
    {
        return Results.NotFound("File not found");
    }


    var file = await File.ReadAllBytesAsync(filePath, cancellationToken);
    return Results.File(file, MediaTypeNames.Application.Json);
});

await app.RunAsync();
