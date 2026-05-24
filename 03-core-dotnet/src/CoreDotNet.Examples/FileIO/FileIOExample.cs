namespace CoreDotNet.Examples.FileIO
{

    /// <summary>
    /// Comprehensive examples for file I/O operations.
    ///
    /// This lesson focuses on the file-system patterns developers actually use:
    /// - Text, binary, and stream-based access.
    /// - Safe temp-file handling and atomic replacement.
    /// - Directory enumeration and path composition.
    /// - Cleanup patterns that prevent file-handle leaks.
    ///
    /// Best practices:
    /// - Always dispose streams with a using statement.
    /// - Use Path.Combine and Path.Join for cross-platform paths.
    /// - Use StreamReader/Writer for text and FileStream for binary data.
    /// - Prefer EnumerateFiles for streaming directory reads when possible.
    /// - Use async I/O when operations may block user-facing code.
    /// </summary>
    public static class FileIOExample
    {
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} File I/O Examples {new string('=', 5)}");

            PrintSection("BASIC FILE OPERATIONS");
            DemoBasicFileOperations();

            PrintSection("READING FILES");
            DemoReadingFiles();

            PrintSection("WRITING FILES");
            DemoWritingFiles();

            PrintSection("BINARY FILES");
            DemoBinaryFiles();

            PrintSection("DIRECTORY OPERATIONS");
            DemoDirectoryOperations();

            PrintSection("ATOMIC WRITE PATTERN");
            DemoAtomicWritePattern();

            PrintSection("PATH MANIPULATION");
            DemoPathManipulation();

            Console.WriteLine();
        }

        private static void DemoBasicFileOperations()
        {
            string tempFile = Path.GetTempFileName();
            try
            {
                Console.WriteLine($"Temp file path: {tempFile}");
                Console.WriteLine($"File exists: {File.Exists(tempFile)}");

                // Write simple content
                File.WriteAllText(tempFile, "Hello, World!");
                Console.WriteLine($"File size: {new FileInfo(tempFile).Length} bytes");

                // Delete
                File.Delete(tempFile);
                Console.WriteLine($"After delete, exists: {File.Exists(tempFile)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void DemoReadingFiles()
        {
            string tempFile = Path.GetTempFileName();
            try
            {
                // Setup test file
                string[] lines = new[] { "Line 1", "Line 2", "Line 3" };
                File.WriteAllLines(tempFile, lines);

                // Read all text
                string content = File.ReadAllText(tempFile);
                Console.WriteLine($"Full content length: {content.Length} chars");

                // Read all lines
                string[] readLines = File.ReadAllLines(tempFile);
                Console.WriteLine($"Lines read: {string.Join(", ", readLines)}");

                // Read with StreamReader
                using (var reader = new StreamReader(tempFile))
                {
                    string? line = reader.ReadLine();
                    Console.WriteLine($"First line via StreamReader: {line}");
                }
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        private static void DemoWritingFiles()
        {
            string tempFile = Path.GetTempFileName();
            try
            {
                // Write text
                File.WriteAllText(tempFile, "Initial content");

                // Append text
                File.AppendAllText(tempFile, "\nAppended line");

                // Write multiple lines
                File.WriteAllLines(tempFile, new[] { "Line A", "Line B", "Line C" });

                // Write with StreamWriter
                using (var writer = new StreamWriter(tempFile, append: true))
                {
                    writer.WriteLine("Written with StreamWriter");
                }

                // Verify content
                string content = File.ReadAllText(tempFile);
                Console.WriteLine($"Final content:\n{content}");
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        private static void DemoBinaryFiles()
        {
            string tempFile = Path.GetTempFileName();
            try
            {
                using (var stream = File.Open(tempFile, FileMode.Create, FileAccess.Write))
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(42);
                    writer.Write(19.95m);
                    writer.Write("Core .NET binary record");
                }

                using (var stream = File.OpenRead(tempFile))
                using (var reader = new BinaryReader(stream))
                {
                    int id = reader.ReadInt32();
                    decimal price = reader.ReadDecimal();
                    string label = reader.ReadString();

                    Console.WriteLine($"Binary record: id={id}, price={price:0.00}, label={label}");
                }
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        private static void DemoDirectoryOperations()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "CoreDotNetExample");
            try
            {
                // Create directory
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                    Console.WriteLine($"Created directory: {tempDir}");
                }

                // Create test files
                File.WriteAllText(Path.Combine(tempDir, "file1.txt"), "Content 1");
                File.WriteAllText(Path.Combine(tempDir, "file2.txt"), "Content 2");

                // List files
                string[] files = Directory.GetFiles(tempDir);
                Console.WriteLine($"Files in directory: {string.Join(", ", files.Select(Path.GetFileName))}");

                // Streaming enumeration avoids loading the entire list at once
                var streamedFiles = Directory.EnumerateFiles(tempDir).Select(Path.GetFileName);
                Console.WriteLine($"Enumerated files: {string.Join(", ", streamedFiles)}");

                // List directories
                string[] subdirs = Directory.GetDirectories(Path.GetTempPath());
                Console.WriteLine($"Subdirectories count: {subdirs.Length}");
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, recursive: true);
                    Console.WriteLine($"Deleted directory: {tempDir}");
                }
            }
        }

        private static void DemoPathManipulation()
        {
            // Cross-platform path handling
            string fileName = "config.json";
            string directory = "settings";
            string combined = Path.Combine(directory, fileName);
            Console.WriteLine($"Combined path: {combined}");

            // Path components
            string fullPath = @"C:\Users\AppData\settings\config.json";
            Console.WriteLine($"Directory: {Path.GetDirectoryName(fullPath)}");
            Console.WriteLine($"File name: {Path.GetFileName(fullPath)}");
            Console.WriteLine($"Extension: {Path.GetExtension(fullPath)}");
            Console.WriteLine($"Name without ext: {Path.GetFileNameWithoutExtension(fullPath)}");
            Console.WriteLine($"Changed extension: {Path.ChangeExtension(fullPath, ".bak")}");

            // Special paths
            Console.WriteLine($"Temp path: {Path.GetTempPath()}");
            Console.WriteLine($"Current dir: {Directory.GetCurrentDirectory()}");
            Console.WriteLine($"Relative path from temp to file: {Path.GetRelativePath(Path.GetTempPath(), fullPath)}");

            // Path validation
            char[] invalidChars = Path.GetInvalidFileNameChars();
            Console.WriteLine($"Invalid file name chars: {string.Join("", invalidChars)}");
        }

        private static void DemoAtomicWritePattern()
        {
            string targetFile = Path.Combine(Path.GetTempPath(), "core-dotnet-settings.json");
            string tempFile = targetFile + ".tmp";

            try
            {
                File.WriteAllText(tempFile, "{\"course\":\"Core .NET\",\"level\":\"intermediate\"}");
                File.Move(tempFile, targetFile, overwrite: true);

                Console.WriteLine($"Atomic write completed: {targetFile}");
                Console.WriteLine($"File exists after move: {File.Exists(targetFile)}");
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }

                if (File.Exists(targetFile))
                {
                    File.Delete(targetFile);
                }
            }
        }

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }
    }
}
