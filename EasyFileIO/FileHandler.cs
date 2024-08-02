using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace EasyFileIO
{

    public enum CsvDelimiter
    {
        Comma,
        Semicolon,
        Tab
    }

    public class FileHandler
    {
        private static readonly ILogger _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<FileHandler>();


        private static void HandleIOException(string operation, Exception ex)
        {
            throw new IOException($"An error occurred while {operation}: {ex.Message}", ex);
        }

        /// <summary>
        /// Reads all text from a file synchronously.
        /// </summary>
        /// <param name="filePath">The path to the file to read.</param>
        /// <returns>The contents of the file as a string.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file is not found.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        public static string ReadFile(string filePath)
        {
            try
            {
                return ReadFileAsync(filePath).GetAwaiter().GetResult();
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"The file at {filePath} was not found.");
            }
            catch (IOException ex)
            {
                HandleIOException("ReadFile", ex);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while reading the file");
                throw;
            }
        }

        /// <summary>
        /// Reads all text from a file asynchronously.
        /// </summary>
        /// <param name="filePath">The path to the file to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file is not found.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        public static async Task<string> ReadFileAsync(string filePath)
        {
            try
            {
                using var reader = File.OpenText(filePath);
                return await reader.ReadToEndAsync();
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"The file at {filePath} was not found.");
            }
            catch (IOException ex)
            {
                HandleIOException("ReadFile", ex);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while reading the file");
                throw;
            }
        }

        /// <summary>
        /// Writes text to a file synchronously, overwriting any existing content.
        /// </summary>
        /// <param name="filePath">The path to the file to write.</param>
        /// <param name="content">The content to write to the file.</param>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        public static void WriteFile(string filePath, string content)
        {
            try
            {
                File.WriteAllText(filePath, content);
            }
            catch (IOException ex)
            {
                HandleIOException("WriteFile", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while writing the file");
                throw;
            }
        }

        /// <summary>
        /// Writes text to a file asynchronously, overwriting any existing content.
        /// </summary>
        /// <param name="filePath">The path to the file to write.</param>
        /// <param name="content">The content to write to the file.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        public static async Task WriteFileAsync(string filePath, string content)
        {
            try
            {
                using var writer = File.CreateText(filePath);
                await writer.WriteAsync(content);
            }
            catch (IOException ex)
            {
                HandleIOException("WriteFile", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while writing the file");
                throw;
            }

        }

        /// <summary>
        /// Appends text to an existing file synchronously.
        /// </summary>
        /// <param name="filePath">The path to the file to append to.</param>
        /// <param name="content">The content to append to the file.</param>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        public static void AppendToFile(string filePath, string content)
        {
            try
            {
                File.AppendAllText(filePath, content);
            }
            catch (IOException ex)
            {
                HandleIOException("AppendFile", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while appending to the file");
                throw;
            }
        }

        /// <summary>
        /// Appends text to an existing file asynchronously.
        /// </summary>
        /// <param name="filePath">The path to the file to append to.</param>
        /// <param name="content">The content to append to the file.</param>
        /// <returns>A task that represents the asynchronous append operation.</returns>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        public static async Task AppendToFileAsync(string filePath, string content)
        {
            try
            {
                using var writer = File.AppendText(filePath);
                await writer.WriteAsync(content);
            }
            catch (IOException ex)
            {
                HandleIOException("AppendToFile", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while appending to the file");
                throw;
            }
        }

        /// <summary>
        /// Reads specific lines from a file.
        /// </summary>
        /// <param name="filePath">The path to the file to read.</param>
        /// <param name="lineNumbers">An array of line numbers to read (1-based).</param>
        /// <returns>A dictionary with line numbers as keys and line contents as values.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file is not found.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        public static Dictionary<int, string> ReadSpecificLines(string filePath, int[] lineNumbers)
        {
            try
            {
                var result = new Dictionary<int, string>();
                var lines = File.ReadAllLines(filePath);
                foreach (var lineNumber in lineNumbers)
                {
                    if (lineNumber > 0 && lineNumber <= lines.Length)
                    {
                        result[lineNumber] = lines[lineNumber - 1];
                    }
                }
                return result;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"The file at {filePath} was not found.");
            }
            catch (IOException ex)
            {
                HandleIOException("ReadSpecificLines", ex);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while reading specific lines of the file");
                throw;
            }
        }

        /// <summary>
        /// Counts the number of words in a file.
        /// </summary>
        /// <param name="filePath">The path to the file to analyze.</param>
        /// <returns>The number of words in the file.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file is not found.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        public static int CountWords(string filePath)
        {
            try
            {
                int wordCount = 0;
                using (var reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        wordCount += line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
                    }
                }
                return wordCount;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"The file at {filePath} was not found.");
            }
            catch (IOException ex)
            {
                HandleIOException("CountWords", ex);
                return -1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while counting the words");
                throw;
            }
        }

        /// <summary>
        /// Creates a directory if it doesn't exist.
        /// </summary>
        /// <param name="path">The path of the directory to create.</param>
        public static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Deletes a directory and all its contents.
        /// </summary>
        /// <param name="path">The path of the directory to delete.</param>
        /// <param name="recursive">If true, deletes all subdirectories and files.</param>
        public static void DeleteDirectory(string path, bool recursive = true)
        {
            Directory.Delete(path, recursive);
        }

        /// <summary>
        /// Checks if a directory exists.
        /// </summary>
        /// <param name="path">The path of the directory to check.</param>
        /// <returns>True if the directory exists, false otherwise.</returns>
        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Copies a file to a new location.
        /// </summary>
        /// <param name="sourceFilePath">The path of the file to copy.</param>
        /// <param name="destFilePath">The path to copy the file to.</param>
        /// <param name="overwrite">If true, overwrites the destination file if it exists.</param>
        public static void CopyFile(string sourceFilePath, string destFilePath, bool overwrite = false)
        {
            File.Copy(sourceFilePath, destFilePath, overwrite);
        }

        /// <summary>
        /// Moves a file to a new location.
        /// </summary>
        /// <param name="sourceFilePath">The path of the file to move.</param>
        /// <param name="destFilePath">The path to move the file to.</param>
        public static void MoveFile(string sourceFilePath, string destFilePath)
        {
            File.Move(sourceFilePath, destFilePath);
        }

        /// <summary>
        /// Gets file information.
        /// </summary>
        /// <param name="filePath">The path of the file.</param>
        /// <returns>A FileInfo object containing file information.</returns>
        public static FileInfo GetFileInfo(string filePath)
        {
            return new FileInfo(filePath);
        }

        private static char GetDelimiterChar(CsvDelimiter delimiter)
        {
            return delimiter switch
            {
                CsvDelimiter.Comma => ',',
                CsvDelimiter.Semicolon => ';',
                CsvDelimiter.Tab => '\t',
                _ => throw new ArgumentException("Invalid delimiter", nameof(delimiter))
            };
        }

        /// <summary>
        /// Reads a CSV file and returns its contents as a list of string arrays.
        /// </summary>
        /// <param name="filePath">The path of the CSV file to read.</param>
        /// <param name="delimiter">The delimiter used in the CSV file.</param>
        /// <returns>A list of string arrays, where each array represents a row in the CSV.</returns>
        public static List<string[]> ReadCsv(string filePath, CsvDelimiter delimiter = CsvDelimiter.Comma)
        {
            var lines = File.ReadAllLines(filePath);
            char delimiterChar = GetDelimiterChar(delimiter);
            return lines.Select(line => line.Split(delimiterChar)).ToList();
        }

        /// <summary>
        /// Writes data to a CSV file.
        /// </summary>
        /// <param name="filePath">The path of the CSV file to write.</param>
        /// <param name="data">The data to write, where each string array represents a row.</param>
        /// <param name="delimiter">The delimiter to use in the CSV file.</param>
        public static void WriteCsv(string filePath, IEnumerable<string[]> data, CsvDelimiter delimiter = CsvDelimiter.Comma)
        {
            char delimiterChar = GetDelimiterChar(delimiter);
            var lines = data.Select(row => string.Join(delimiterChar, row));
            File.WriteAllLines(filePath, lines);
        }

        /// <summary>
        /// Searches for files matching a specified pattern in a directory.
        /// </summary>
        /// <param name="directory">The directory to search in.</param>
        /// <param name="searchPattern">The search pattern to match against file names.</param>
        /// <param name="searchOption">Specifies whether to search subdirectories.</param>
        /// <returns>An array of file paths matching the search pattern.</returns>
        public static string[] SearchFiles(string directory, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            // Use Path.GetFileName to ensure we're only matching against the file name, not the full path
            return Directory.GetFiles(directory, "*", searchOption)
                .Where(file => Path.GetFileName(file).ToLower().StartsWith(searchPattern.ToLower()))
                .ToArray();
        }

        /// <summary>
        /// Processes a large file line by line asynchronously.
        /// </summary>
        /// <param name="filePath">The path to the file to process.</param>
        /// <param name="lineProcessor">The function to process each line.</param>
        /// <returns>A task that represents the asynchronous processing operation.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file is not found.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        public static async Task ProcessFileLinesAsync(string filePath, Func<string, Task> lineProcessor)
        {
            try
            {
                using var reader = new StreamReader(filePath);
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    await lineProcessor(line);
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"The file at {filePath} was not found.");
            }
            catch (IOException ex)
            {
                HandleIOException("ProcessLargeFile", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing lines of the file");
                throw;
            }
        }

        /// <summary>
        /// Writes an object to a JSON file asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="filePath">The path to the file to write.</param>
        /// <param name="obj">The object to serialize and write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        public static async Task WriteJsonAsync<T>(string filePath, T obj)
        {
            try
            {
                string json = System.Text.Json.JsonSerializer.Serialize(obj);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (IOException ex)
            {
                HandleIOException("WriteJson", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while writing the json file");
                throw;
            }
        }

        /// <summary>
        /// Reads a JSON file and deserializes it to an object asynchronously.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the JSON to.</typeparam>
        /// <param name="filePath">The path to the JSON file to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file is not found.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        public static async Task<T> ReadJsonAsync<T>(string filePath)
        {
            try
            {
                string json = await File.ReadAllTextAsync(filePath);
                return System.Text.Json.JsonSerializer.Deserialize<T>(json);
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"The file at {filePath} was not found.");
            }
            catch (IOException ex)
            {
                HandleIOException("reading JSON", ex);
                throw; // Rethrow the exception after logging
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while reading the json");
                throw;
            }
        }

        /// <summary>
        /// Copies multiple files to a destination directory.
        /// </summary>
        /// <param name="sourceFiles">The paths of the files to copy.</param>
        /// <param name="destinationDirectory">The directory to copy the files to.</param>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        public static void BatchCopyFiles(IEnumerable<string> sourceFiles, string destinationDirectory)
        {
            try
            {
                foreach (var file in sourceFiles)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(destinationDirectory, fileName);
                    File.Copy(file, destFile, true);
                }
            }
            catch (IOException ex)
            {
                HandleIOException("BatchCopyFiles", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while batch copying the files");
                throw;
            }
        }

        /// <summary>
        /// Encrypts a file using SHA256 encryption.
        /// </summary>
        /// <param name="inputFile">The path to the file to encrypt.</param>
        /// <param name="outputFile">The path to save the encrypted file.</param>
        /// <param name="password">The password to use for encryption.</param>
        /// <exception cref="FileEncryptionException">Thrown when encryption fails.</exception>
        public static void EncryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                byte[] salt = new byte[16];
                RandomNumberGenerator.Fill(salt);

                using var keyDerivation = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
                byte[] key = keyDerivation.GetBytes(32);
                byte[] iv = keyDerivation.GetBytes(16);

                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;

                using var inputStream = new FileStream(inputFile, FileMode.Open);
                using var outputStream = new FileStream(outputFile, FileMode.Create);

                // Write salt to the output file
                outputStream.Write(salt, 0, salt.Length);

                using var cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
                inputStream.CopyTo(cryptoStream);
                cryptoStream.FlushFinalBlock();
            }
            catch (IOException ex)
            {
                HandleIOException("EncryptFile", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while encrypting the file");
                throw;
            }
        }

        /// <summary>
        /// Decrypts a file that was encrypted using SHA256 encryption.
        /// </summary>
        /// <param name="inputFile">The path to the encrypted file.</param>
        /// <param name="outputFile">The path to save the decrypted file.</param>
        /// <param name="password">The password used for encryption.</param>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        public static void DecryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                using var inputStream = new FileStream(inputFile, FileMode.Open);

                byte[] salt = new byte[16];
                inputStream.Read(salt, 0, salt.Length);

                using var keyDerivation = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
                byte[] key = keyDerivation.GetBytes(32);
                byte[] iv = keyDerivation.GetBytes(16);

                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;

                using var outputStream = new FileStream(outputFile, FileMode.Create);
                using var cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read);

                cryptoStream.CopyTo(outputStream);

            }
            catch (IOException ex)
            {
                throw new IOException($"An error occurred while decrypting the file: {ex.Message}");
            }
            catch (CryptographicException ex)
            {
                HandleIOException("DecryptFile", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while decrypting the file");
                throw;
            }
        }


        /// <summary>
        /// Normalizes a file path for cross-platform compatibility.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>The normalized path.</returns>
        public static string NormalizePath(string path)
        {
            return Path.GetFullPath(path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar));
        }

        /// <summary>
        /// Compresses a file using GZip compression.
        /// </summary>
        /// <param name="inputFile">The path to the file to compress.</param>
        /// <param name="outputFile">The path to save the compressed file.</param>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        public static void CompressFile(string inputFile, string outputFile)
        {
            try
            {
                using var input = File.OpenRead(inputFile);
                using var output = File.Create(outputFile);
                using var gzip = new System.IO.Compression.GZipStream(output, System.IO.Compression.CompressionMode.Compress);
                input.CopyTo(gzip);
            }
            catch (IOException ex)
            {
                HandleIOException("CompressFile", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while compressing the file");
                throw;
            }
        }

        /// <summary>
        /// Decompresses a GZip compressed file.
        /// </summary>
        /// <param name="inputFile">The path to the compressed file.</param>
        /// <param name="outputFile">The path to save the decompressed file.</param>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        public static void DecompressFile(string inputFile, string outputFile)
        {
            try
            {
                using var input = File.OpenRead(inputFile);
                using var output = File.Create(outputFile);
                using var gzip = new System.IO.Compression.GZipStream(input, System.IO.Compression.CompressionMode.Decompress);
                gzip.CopyTo(output);
            }
            catch (IOException ex)
            {
                HandleIOException("DecompressFile", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while decompressing the file");
                throw;
            }
        }

        /// <summary>
        /// Creates a file watcher for detecting changes to a file.
        /// </summary>
        /// <param name="filePath">The path to the file to watch.</param>
        /// <param name="onChange">The action to perform when the file changes.</param>
        /// <returns>A FileSystemWatcher object.</returns>
        public static FileSystemWatcher WatchFile(string filePath, Action<string> onChange)
        {
            var watcher = new FileSystemWatcher(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
            watcher.Changed += (sender, e) => onChange(e.FullPath);
            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        /// <summary>
        /// Computes the MD5 hash of a file.
        /// </summary>
        /// <param name="filePath">The path to the file to hash.</param>
        /// <returns>The MD5 hash of the file as a lowercase hexadecimal string.</returns>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        public static string GetFileHash(string filePath)
        {
            try
            {
                using var sha256 = SHA256.Create();
                using var stream = File.OpenRead(filePath);
                byte[] hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
            catch (IOException ex)
            {
                HandleIOException("GetFileHash", ex);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the file hash");
                throw;
            }
        }

        /// <summary>
        /// Creates a temporary file with specified parameters.
        /// </summary>
        /// <param name="prefix">Optional prefix for the file name.</param>
        /// <param name="extension">Optional file extension.</param>
        /// <param name="size">Optional file size in bytes.</param>
        /// <param name="content">Optional content to write to the file.</param>
        /// <returns>The path of the created temporary file.</returns>
        public static string CreateTempFile(string prefix = null, string extension = null, long size = 0, string content = null)
        {
            string tempPath = Path.GetTempPath();
            string fileName = $"{prefix ?? Path.GetRandomFileName()}{extension ?? ".tmp"}";
            string filePath = Path.Combine(tempPath, fileName);

            using (var fs = File.Create(filePath))
            {
                if (content != null)
                {
                    byte[] contentBytes = System.Text.Encoding.UTF8.GetBytes(content);
                    fs.Write(contentBytes, 0, contentBytes.Length);
                }
                else if (size > 0)
                {
                    fs.SetLength(size);
                }
            }

            return filePath;
        }

        /// <summary>
        /// Gets the file extension, with additional options.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="includeDot">Whether to include the dot in the extension.</param>
        /// <param name="validExtensions">Optional array of valid extensions to check against.</param>
        /// <returns>The file extension, or an empty string if not found or invalid.</returns>
        public static string GetFileExtension(string filePath, bool includeDot = true, string[] validExtensions = null)
        {
            string fileName = Path.GetFileName(filePath);
            string extension = string.Empty;

            int firstDotIndex = fileName.IndexOf('.');
            if (firstDotIndex >= 0)
            {
                extension = fileName.Substring(firstDotIndex);
            }

            if (validExtensions != null && !validExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return includeDot ? extension : extension.TrimStart('.');
        }

        /// <summary>
        /// Checks if a file is locked (in use by another process).
        /// </summary>
        /// <param name="filePath">The path of the file to check.</param>
        /// <returns>True if the file is locked, false otherwise.</returns>
        public static bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if the file is locked");
                throw;
            }

            return false;
        }

        /// <summary>
        /// Splits a file into smaller chunks.
        /// </summary>
        /// <param name="filePath">The path of the file to split.</param>
        /// <param name="chunkSize">The size of each chunk in bytes.</param>
        /// <param name="outputDirectory">The directory to save the chunks.</param>
        /// <returns>An array of file paths for the created chunks.</returns>
        public static string[] SplitFile(string filePath, int chunkSize, string outputDirectory)
        {
            List<string> chunkPaths = new List<string>();
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                int chunkNumber = 0;
                byte[] buffer = new byte[chunkSize];
                int bytesRead;

                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string chunkPath = Path.Combine(outputDirectory, $"chunk_{chunkNumber}.dat");
                    using (FileStream chunkFs = File.Create(chunkPath))
                    {
                        chunkFs.Write(buffer, 0, bytesRead);
                    }
                    chunkPaths.Add(chunkPath);
                    chunkNumber++;
                }
            }

            return chunkPaths.ToArray();
        }

        /// <summary>
        /// Merges multiple files into a single file.
        /// </summary>
        /// <param name="filePaths">An array of file paths to merge.</param>
        /// <param name="outputPath">The path of the output merged file.</param>
        /// <param name="deleteSource">Whether to delete source files after merging.</param>
        public static void MergeFiles(string[] filePaths, string outputPath, bool deleteSource = false)
        {
            using (FileStream outputStream = File.Create(outputPath))
            {
                foreach (string filePath in filePaths)
                {
                    using (FileStream inputStream = File.OpenRead(filePath))
                    {
                        inputStream.CopyTo(outputStream);
                    }
                    if (deleteSource)
                    {
                        File.Delete(filePath);
                    }
                }
            }
        }
    }
}
