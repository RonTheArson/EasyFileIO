using EasyFileIO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace EasyFileIO.Tests
{
    public class FileHandlerTests : IDisposable
    {
        // Test object class for JSON serialization/deserialization
        public class TestObject
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
        private readonly string _testDirectory = Path.Combine(Path.GetTempPath(), "FileHandlerTests");
        private readonly string _testFilePath;
        private readonly string _testCsvPath;

        public FileHandlerTests()
        {
            _testFilePath = Path.Combine(_testDirectory, "testfile.txt");
            _testCsvPath = Path.Combine(_testDirectory, "testfile.csv");
            Directory.CreateDirectory(_testDirectory);
            File.WriteAllText(_testFilePath, "Test content");
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Fact]
        public void CreateDirectory_ShouldCreateNewDirectory()
        {
            string newDir = Path.Combine(_testDirectory, "newdir");
            FileHandler.CreateDirectory(newDir);
            Assert.True(Directory.Exists(newDir));
        }

        [Fact]
        public void DeleteDirectory_ShouldDeleteExistingDirectory()
        {
            string dirToDelete = Path.Combine(_testDirectory, "todelete");
            Directory.CreateDirectory(dirToDelete);
            FileHandler.DeleteDirectory(dirToDelete);
            Assert.False(Directory.Exists(dirToDelete));
        }

        [Fact]
        public void DirectoryExists_ShouldReturnTrueForExistingDirectory()
        {
            Assert.True(FileHandler.DirectoryExists(_testDirectory));
        }

        [Fact]
        public void DirectoryExists_ShouldReturnFalseForNonExistingDirectory()
        {
            Assert.False(FileHandler.DirectoryExists(Path.Combine(_testDirectory, "nonexistent")));
        }

        [Fact]
        public void CopyFile_ShouldCreateCopyOfFile()
        {
            string destPath = Path.Combine(_testDirectory, "copy.txt");
            FileHandler.CopyFile(_testFilePath, destPath);
            Assert.True(File.Exists(destPath));
            Assert.Equal(File.ReadAllText(_testFilePath), File.ReadAllText(destPath));
        }

        [Fact]
        public void MoveFile_ShouldMoveFileToNewLocation()
        {
            string sourcePath = Path.Combine(_testDirectory, "tomove.txt");
            string destPath = Path.Combine(_testDirectory, "moved.txt");
            File.WriteAllText(sourcePath, "Move me");
            FileHandler.MoveFile(sourcePath, destPath);
            Assert.False(File.Exists(sourcePath));
            Assert.True(File.Exists(destPath));
            Assert.Equal("Move me", File.ReadAllText(destPath));
        }

        [Fact]
        public void GetFileInfo_ShouldReturnCorrectFileInfo()
        {
            FileInfo info = FileHandler.GetFileInfo(_testFilePath);
            Assert.Equal("testfile.txt", info.Name);
            Assert.Equal(_testFilePath, info.FullName);
            Assert.Equal(12, info.Length); // "Test content" is 12 bytes
        }

        [Fact]
        public void ReadCsv_ShouldCorrectlyParseCSVFile()
        {
            string csvContent = "Name,Age,City\nJohn,30,New York\nJane,25,London";
            File.WriteAllText(_testCsvPath, csvContent);

            var result = FileHandler.ReadCsv(_testCsvPath);
            Assert.Equal(3, result.Count);
            Assert.Equal(new[] { "Name", "Age", "City" }, result[0]);
            Assert.Equal(new[] { "John", "30", "New York" }, result[1]);
            Assert.Equal(new[] { "Jane", "25", "London" }, result[2]);
        }
        [Fact]
        public void SearchFiles_ShouldReturnMatchingFiles()
        {
            // Clear the test directory
            foreach (var file in Directory.GetFiles(_testDirectory))
            {
                File.Delete(file);
            }

            File.WriteAllText(Path.Combine(_testDirectory, "test1.txt"), "");
            File.WriteAllText(Path.Combine(_testDirectory, "test2.txt"), "");
            File.WriteAllText(Path.Combine(_testDirectory, "other.txt"), "");

            var result = FileHandler.SearchFiles(_testDirectory, "test");

            Assert.Equal(2, result.Length);
            Assert.Contains(result, f => Path.GetFileName(f) == "test1.txt");
            Assert.Contains(result, f => Path.GetFileName(f) == "test2.txt");
            Assert.DoesNotContain(result, f => Path.GetFileName(f) == "other.txt");


            // Test with a more specific pattern
            result = FileHandler.SearchFiles(_testDirectory, "test1");
            Assert.Single(result);
            Assert.Contains(result, f => Path.GetFileName(f) == "test1.txt");

            // Test with a pattern that should match no files
            result = FileHandler.SearchFiles(_testDirectory, "nonexistent");
            Assert.Empty(result);
        }

        [Fact]
        public void ReadCsv_ShouldHandleDifferentDelimiters()
        {
            string csvContent = "Name;Age;City\nJohn;30;New York\nJane;25;London";
            File.WriteAllText(_testCsvPath, csvContent);

            var result = FileHandler.ReadCsv(_testCsvPath, CsvDelimiter.Semicolon);
            Assert.Equal(3, result.Count);
            Assert.Equal(new[] { "Name", "Age", "City" }, result[0]);
            Assert.Equal(new[] { "John", "30", "New York" }, result[1]);
            Assert.Equal(new[] { "Jane", "25", "London" }, result[2]);
        }

        [Fact]
        public void WriteCsv_ShouldCorrectlyWriteCSVFile()
        {
            var data = new List<string[]>
            {
                new[] { "Name", "Age", "City" },
                new[] { "John", "30", "New York" },
                new[] { "Jane", "25", "London" }
            };

            FileHandler.WriteCsv(_testCsvPath, data);

            var result = File.ReadAllText(_testCsvPath);
            Assert.Equal("Name,Age,City\r\nJohn,30,New York\r\nJane,25,London\r\n", result);
        }

        [Fact]
        public void WriteCsv_ShouldHandleDifferentDelimiters()
        {
            var data = new List<string[]>
            {
                new[] { "Name", "Age", "City" },
                new[] { "John", "30", "New York" },
                new[] { "Jane", "25", "London" }
            };

            FileHandler.WriteCsv(_testCsvPath, data, CsvDelimiter.Tab);

            var result = File.ReadAllText(_testCsvPath);
            Assert.Equal("Name\tAge\tCity\r\nJohn\t30\tNew York\r\nJane\t25\tLondon\r\n", result);
        }

        [Fact]
        public async Task ReadFileAsync_ShouldReadFileContents()
        {
            string content = "Async test content";
            await File.WriteAllTextAsync(_testFilePath, content);
            string result = await FileHandler.ReadFileAsync(_testFilePath);
            Assert.Equal(content, result);
        }

        [Fact]
        public async Task ProcessLargeFileAsync_ShouldProcessEachLine()
        {
            string[] lines = { "Line 1", "Line 2", "Line 3" };
            await File.WriteAllLinesAsync(_testFilePath, lines);
            List<string> processedLines = new List<string>();

            await FileHandler.ProcessFileLinesAsync(_testFilePath, async (line) =>
            {
                processedLines.Add(line);
                await Task.CompletedTask;
            });

            Assert.Equal(lines, processedLines);
        }

        [Fact]
        public async Task JsonReadWrite_ShouldSerializeAndDeserialize()
        {
            var obj = new TestObject { Name = "Test", Value = 42 };

            await FileHandler.WriteJsonAsync(_testFilePath, obj);
            var result = await FileHandler.ReadJsonAsync<TestObject>(_testFilePath);

            Assert.Equal(obj.Name, result.Name);
            Assert.Equal(obj.Value, result.Value);
        }


        [Fact]
        public void BatchCopyFiles_ShouldCopyAllFiles()
        {
            string sourceDir = Path.Combine(_testDirectory, "source");
            string destDir = Path.Combine(_testDirectory, "dest");
            Directory.CreateDirectory(sourceDir);
            Directory.CreateDirectory(destDir);

            string[] files = { "file1.txt", "file2.txt" };
            foreach (var file in files)
            {
                File.WriteAllText(Path.Combine(sourceDir, file), "content");
            }

            FileHandler.BatchCopyFiles(files.Select(f => Path.Combine(sourceDir, f)), destDir);

            foreach (var file in files)
            {
                Assert.True(File.Exists(Path.Combine(destDir, file)));
            }
        }

        [Fact]
        public void EncryptDecryptFile_ShouldPreserveContent()
        {
            string content = "Secret content";
            string password = "password123";
            string originalFile = Path.Combine(_testDirectory, "original.txt");
            string encryptedFile = Path.Combine(_testDirectory, "encrypted.bin");
            string decryptedFile = Path.Combine(_testDirectory, "decrypted.txt");

            File.WriteAllText(originalFile, content);

            FileHandler.EncryptFile(originalFile, encryptedFile, password);
            FileHandler.DecryptFile(encryptedFile, decryptedFile, password);

            string decryptedContent = File.ReadAllText(decryptedFile);
            Assert.Equal(content, decryptedContent);
        }

        [Fact]
        public void NormalizePath_ShouldHandleDifferentSeparators()
        {
            string path = "folder1\\folder2/file.txt";
            string normalized = FileHandler.NormalizePath(path);
            Assert.Equal(Path.Combine("folder1", "folder2", "file.txt"), Path.GetRelativePath(Directory.GetCurrentDirectory(), normalized));
        }

        [Fact]
        public void CompressDecompressFile_ShouldPreserveContent()
        {
            string content = "Content to compress";
            string compressedFile = Path.Combine(_testDirectory, "compressed.gz");

            File.WriteAllText(_testFilePath, content);
            FileHandler.CompressFile(_testFilePath, compressedFile);
            FileHandler.DecompressFile(compressedFile, _testFilePath);

            Assert.Equal(content, File.ReadAllText(_testFilePath));
        }

        [Fact]
        public void WatchFile_ShouldDetectChanges()
        {
            var changeDetected = new ManualResetEvent(false);
            using var watcher = FileHandler.WatchFile(_testFilePath, (path) => changeDetected.Set());

            File.WriteAllText(_testFilePath, "New content");

            Assert.True(changeDetected.WaitOne(TimeSpan.FromSeconds(5)));
        }

        [Fact]
        public void GetFileHash_ShouldReturnConsistentHash()
        {
            string content = "Content to hash";
            File.WriteAllText(_testFilePath, content);

            string hash1 = FileHandler.GetFileHash(_testFilePath);
            string hash2 = FileHandler.GetFileHash(_testFilePath);

            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void CreateTempFile_CreatesFileWithSpecifiedParameters()
        {
            string tempFile = FileHandler.CreateTempFile("test", ".txt", content: "Hello, World!");
            Assert.True(File.Exists(tempFile));
            Assert.Equal("Hello, World!", File.ReadAllText(tempFile));
            Assert.StartsWith("test", Path.GetFileName(tempFile));
            Assert.EndsWith(".txt", tempFile);
            File.Delete(tempFile);
        }

        [Fact]
        public void GetFileExtension_ReturnsCorrectExtension()
        {
            Assert.Equal(".txt", FileHandler.GetFileExtension("file.txt"));
            Assert.Equal("txt", FileHandler.GetFileExtension("file.txt", includeDot: false));
            Assert.Equal(".tar.gz", FileHandler.GetFileExtension("file.tar.gz"));
            Assert.Equal("tar.gz", FileHandler.GetFileExtension("file.tar.gz", includeDot: false));
            Assert.Equal("", FileHandler.GetFileExtension("file.txt", validExtensions: new[] { ".doc", ".docx" }));
            Assert.Equal("", FileHandler.GetFileExtension("file"));
        }

        [Fact]
        public void IsFileLocked_ReturnsTrueForLockedFile()
        {
            string filePath = Path.GetTempFileName();
            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                Assert.True(FileHandler.IsFileLocked(filePath));
            }
            File.Delete(filePath);
        }

        [Fact]
        public void SplitFile_CreatesSmallerChunks()
        {
            string testFile = Path.GetTempFileName();
            File.WriteAllText(testFile, new string('A', 1000));

            string outputDir = Path.Combine(Path.GetTempPath(), "SplitFileTest");
            Directory.CreateDirectory(outputDir);

            string[] chunks = FileHandler.SplitFile(testFile, 250, outputDir);

            Assert.Equal(4, chunks.Length);
            foreach (string chunk in chunks)
            {
                Assert.True(new FileInfo(chunk).Length <= 250);
                File.Delete(chunk);
            }

            File.Delete(testFile);
            Directory.Delete(outputDir);
        }

        [Fact]
        public void MergeFiles_CombinesFilesCorrectly()
        {
            string[] sourceFiles = new string[3];
            for (int i = 0; i < 3; i++)
            {
                sourceFiles[i] = Path.GetTempFileName();
                File.WriteAllText(sourceFiles[i], $"Content {i}");
            }

            string mergedFile = Path.GetTempFileName();
            FileHandler.MergeFiles(sourceFiles, mergedFile);

            string mergedContent = File.ReadAllText(mergedFile);
            Assert.Equal("Content 0Content 1Content 2", mergedContent);

            foreach (string file in sourceFiles)
            {
                File.Delete(file);
            }
            File.Delete(mergedFile);
        }
    }
}