# EasyFileIO

EasyFileIO is a comprehensive C# library for file and directory operations. It provides a simple and efficient way to handle common file tasks, including reading, writing, encryption, compression, and more.

## Features

- Basic file operations: read, write, append, copy, move
- Directory operations: create, delete, check existence
- CSV handling: read and write with different delimiters
- Asynchronous file operations
- JSON serialization and deserialization
- File searching and batch operations
- File encryption and decryption
- File compression and decompression
- File watching for changes
- File hashing
- Temporary file creation
- File locking checks
- File splitting and merging

## Installation

Install EasyFileIO via NuGet:

Install-Package EasyFileIO

## Usage

```csharp
using EasyFileIO;

// Read a file
string content = FileHandler.ReadFile("path/to/file.txt");

// Write to a file
FileHandler.WriteFile("path/to/file.txt", "Hello, World!");

// Encrypt a file
FileHandler.EncryptFile("input.txt", "encrypted.bin", "password123");

// Read CSV
var csvData = FileHandler.ReadCsv("data.csv", CsvDelimiter.Comma);

// ... and many more operations
```

## Documentation
For detailed documentation, please refer to the XML comments in the source code.
## Contributing
Contributions are welcome! Please feel free to submit a Pull Request.
## License
This project is licensed under the MIT License.
