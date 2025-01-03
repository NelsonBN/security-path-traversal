# Security - Demo about Path Traversal

## What is Path Traversal?

Path Traversal, also known as Directory Traversal, is a vulnerability that allows an attacker to access files and directories outside the intended scope of an application. This occurs due to improper validation of user-supplied input. An attacker can exploit this by manipulating file path parameters, using special characters (e.g., `../` or `..\`) to navigate to unauthorized directories.


### Exploitation Example

Consider an application that accepts a `fileName` parameter to fetch files:
```
GET /download?fileName=report.txt
```

An attacker could manipulate the input to access sensitive files:
```
GET /download?fileName=../../etc/passwd
```

Here, the attacker attempts to access `/etc/passwd`, a sensitive file on Linux systems.

For more information and test payloads, see the [Path Traversal Payloads repository](https://github.com/vulnerablesite/Path-transversal-payloads), which provides a collection of examples.


## How to Mitigate Path Traversal?

### Secure Code Example

Below is an example of how to secure an endpoint against Path Traversal in .NET:

```csharp
const string PUBLIC_FILES_PATH = "PublicFiles";
var allowedFilesPath = Path.GetFullPath(PUBLIC_FILES_PATH);

var builder = WebApplication.CreateSlimBuilder(args);
var app = builder.Build();

app.MapGet("/download", async (string fileName, CancellationToken cancellationToken) =>
{
    // Sanitize the file name
    var sanitizedFileName = Path.GetFileName(fileName);
    var filePath = Path.Combine(PUBLIC_FILES_PATH, sanitizedFileName);

    // Ensure the resolved path is within the allowed directory
    var fullPath = Path.GetFullPath(filePath);
    if (!fullPath.StartsWith(allowedFilesPath))
    {
        return Results.BadRequest("Invalid file path");
    }

    // Check if the file exists
    if (!File.Exists(filePath))
    {
        return Results.NotFound("File not found");
    }

    var file = await File.ReadAllBytesAsync(filePath, cancellationToken);
    return Results.File(file, MediaTypeNames.Application.Json);
});

await app.RunAsync();
```

#### Explanation of Solutions

1. **File Name Sanitization**:
   - Using `Path.GetFileName(fileName)` ensures only the file name is processed, stripping out any directory traversal attempts (e.g., `../`, `..\`).

2. **Absolute Path Validation**:
   - `Path.GetFullPath(filePath)` resolves the full path, neutralizing manipulations such as symlinks or encoded payloads.
   - The check `fullPath.StartsWith(allowedFilesPath)` ensures the file is within the designated directory.

3. **Directory Access Restriction**:
   - Defining a public directory like `PUBLIC_FILES_PATH` limits access to only intended files.

4. **File Existence Check**:
   - Before returning a file, the code ensures it exists using `File.Exists(filePath)`.


## Conclusion

Path Traversal is a critical vulnerability that can expose sensitive information or compromise a system. Combining input sanitization with strict absolute path validation is essential to mitigating this risk.

Using standard libraries like .NET's `Path` utilities significantly reduces vulnerabilities and helps safeguard applications against malicious path manipulation.
