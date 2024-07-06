using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Text;

namespace Parcel.Standard.System
{
    /// <summary>
    /// Contains standardized definitions for File, Directory, Path and FileSystemEntry operations.
    /// This is a very thin layer (almost duplicate) built on top of standard .Net as part of PSL for interoperability purposes.
    /// TODO: Need to expose functions on FileStream, StreamWriter and FileAttributes in order to make the API complete.
    /// TODO: Pending trimming and pick which ones to keep and which functions to improve/merge.
    /// </summary>
    public static class FileSystem
    {
        #region File
        /// <summary>
        /// Appends lines to a file, and then closes the file. If the specified file does not exist, this method creates a file, writes the specified lines to the file, and then closes the file.
        /// </summary>
        public static void AppendAllLines(string path, IEnumerable<String> contents) => File.AppendAllLines(path, contents);
        /// <summary>
        /// Appends lines to a file by using a specified Encoding encoding, and then closes the file. If the specified file does not exist, this method creates a file, writes the specified lines to the file, and then closes the file.
        /// </summary>
        public static void AppendAllLines(string path, IEnumerable<String> contents, Encoding encoding) => File.AppendAllLines(path, contents, encoding);
        /// <summary>
        /// Opens a file, appends the specified string to the file, and then closes the file. If the file does not exist, this method creates a file, writes the specified string to the file, then closes the file.
        /// </summary>
        public static void AppendAllText(string path, string? contents) => File.AppendAllText(path, contents);
        /// <summary>
        /// Appends the specified string to the file using the specified Encoding encoding, creating the file if it does not already exist.
        /// </summary>
        public static void AppendAllText(string path, string? contents, Encoding encoding) => File.AppendAllText(path, contents, encoding);
        /// <summary>
        /// Creates a StreamWriter that appends UTF-8 encoded text to an existing file, or to a new file if the specified file does not exist.
        /// </summary>
        public static void AppendText(string path) => File.AppendText(path);
        /// <summary>
        /// Copies an existing file to a new file. Overwriting a file of the same name is not allowed.
        /// </summary>
        public static void Copy(string sourceFileName, string destFileName) => File.Copy(sourceFileName, destFileName);
        /// <summary>
        /// Copies an existing file to a new file. Overwriting a file of the same name is allowed.
        /// </summary>
        public static void Copy(string sourceFileName, string destFileName, bool overwrite) => File.Copy(sourceFileName, destFileName, overwrite);
        /// <summary>
        /// Creates, or truncates and overwrites, a file in the specified path, with an optional size. File will be empty.
        /// </summary>
        public static void Touch(string path, int fileSize = 0) => File.Create(path, fileSize).Close(); // TODO: Make proper implementation.
        /// <summary>
        /// Creates, or truncates and overwrites, a file in the specified path.
        /// </summary>
        public static FileStream Create(string path) => File.Create(path);
        /// <summary>
        /// Creates, or truncates and overwrites, a file in the specified path, specifying a buffer size.
        /// </summary>
        public static FileStream Create(string path, int bufferSize) => File.Create(path, bufferSize);
        /// <summary>
        /// Creates or overwrites a file in the specified path, specifying a buffer size and options that describe how to create or overwrite the file.
        /// </summary>
        public static void Create(string path, int bufferSize, FileOptions options) => File.Create(path, bufferSize, options);
        /// <summary>
        /// Creates a file symbolic link identified by path that points to pathToTarget.
        /// </summary>
        public static FileSystemInfo CreateSymbolicLink(string path, string pathToTarget) => File.CreateSymbolicLink(path, pathToTarget);
        /// <summary>
        /// Creates or opens a file for writing UTF-8 encoded text. If the file already exists, its contents are replaced.
        /// </summary>
        public static StreamWriter CreateText(string path) => File.CreateText(path);
        /// <summary>
        /// Decrypts a file that was encrypted with a given key using the Encrypt(string path, string key) method.
        /// </summary>
        public static void Decrypt(string path, string key) => throw new NotImplementedException();
        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        public static void Delete(string path) => File.Delete(path);
        /// <summary>
        /// Encrypts a file with specific key; To decrypt, use the same key.
        /// </summary>
        public static void Encrypt(string path, string key) => throw new NotImplementedException();
        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        public static void Exists(string path) => File.Exists(path);
        /// <summary>
        /// Gets the FileAttributes of the file on the path.
        /// </summary>
        public static FileAttributes GetAttributes(string path) => File.GetAttributes(path);
        /// <summary>
        /// Returns the creation date and time of the specified file or directory.
        /// </summary>
        public static DateTime GetCreationTime(string path) => File.GetCreationTime(path);
        /// <summary>
        /// Returns the creation date and time, in Coordinated Universal Time (UTC), of the specified file or directory.
        /// </summary>
        public static DateTime GetCreationTimeUtc(string path) => File.GetCreationTimeUtc(path); // TODO: Clarify its meaning - does the function sets time in Utc or the DateTime already handles it?
        /// <summary>
        /// Returns the date and time the specified file or directory was last accessed.
        /// </summary>
        public static DateTime GetLastAccessTime(string path) => File.GetLastAccessTime(path);
        /// <summary>
        /// Returns the date and time, in Coordinated Universal Time (UTC), that the specified file or directory was last accessed.
        /// </summary>
        public static DateTime GetLastAccessTimeUtc(string path) => File.GetLastAccessTimeUtc(path); // TODO: Clarify its meaning - does the function sets time in Utc or the DateTime already handles it?
        /// <summary>
        /// Returns the date and time the specified file or directory was last written to.
        /// </summary>
        public static DateTime GetLastWriteTime(string path) => File.GetLastWriteTime(path);
        /// <summary>
        /// Returns the date and time, in Coordinated Universal Time (UTC), that the specified file or directory was last written to.
        /// </summary>
        public static DateTime GetLastWriteTimeUtc(string path) => File.GetLastWriteTimeUtc(path); // TODO: Clarify its meaning - does the function sets time in Utc or the DateTime already handles it?
        /// <summary>
        /// Moves a specified file to a new location, providing the option to specify a new file name.
        /// </summary>
        public static void Move(string sourceFileName, string destFileName) => File.Move(sourceFileName, destFileName);
        /// <summary>
        /// Moves a specified file to a new location, providing the options to specify a new file name and to replace the destination file if it already exists.
        /// </summary>
        public static void Move(string sourceFileName, string destFileName, bool overwrite) => File.Move(sourceFileName, destFileName, overwrite);
        /// <summary>
        /// Opens a FileStream on the specified path with read/write access with no sharing. (Sharing is a feature only available on Windows with SafeHandle)
        /// </summary>
        public static FileStream Open(string path, FileMode fileMode) => File.Open(path, fileMode);
        /// <summary>
        /// Opens a FileStream on the specified path, with the specified mode and access with no sharing.
        /// </summary>
        public static FileStream Open(string path, FileMode mode, FileAccess access) => File.Open(path, mode, access);
        /// <summary>
        /// Opens a FileStream on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.
        /// </summary>
        public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share) => File.Open(path, mode, access, share);
        /// <summary>
        /// Initializes a new instance of the FileStream class with the specified path, creation mode, read/write and sharing permission, the access other FileStreams can have to the same file, the buffer size, additional file options and the allocation size.
        /// </summary>
        public static FileStream Open(string path, FileStreamOptions options) => File.Open(path, options);
        /// <summary>
        /// Opens an existing file for reading.
        /// </summary>
        public static FileStream OpenRead(string path) => File.OpenRead(path);
        /// <summary>
        /// Opens an existing UTF-8 encoded text file for reading.
        /// </summary>
        public static StreamReader OpenText(string path) => File.OpenText(path);
        /// <summary>
        /// Opens an existing file or creates a new file for writing.
        /// </summary>
        public static void OpenWrite(string path) => File.OpenWrite(path);
        /// <summary>
        /// Opens a binary file, reads the contents of the file into a byte array, and then closes the file.
        /// </summary>
        public static void ReadAllBytes(string path) => File.ReadAllBytes(path);
        /// <summary>
        /// Opens a text file, reads all lines of the file, and then closes the file.
        /// </summary>
        public static void ReadAllLines(string path) => File.ReadAllLines(path);
        /// <summary>
        /// Opens a file, reads all lines of the file with the specified Encoding encoding, and then closes the file.
        /// </summary>
        public static void ReadAllLines(string path, Encoding encoding) => File.ReadAllLines(path, encoding);
        /// <summary>
        /// Opens a text file, reads all the text in the file, and then closes the file.
        /// </summary>
        public static void ReadAllText(string path) => File.ReadAllText(path);
        /// <summary>
        /// Opens a file, reads all text in the file with the specified Encoding encoding, and then closes the file.
        /// </summary>
        public static void ReadAllText(string path, Encoding encoding) => File.ReadAllText(path, encoding);
        /// <summary>
        /// Reads the lines of a file.
        /// </summary>
        public static void ReadLines(string path) => File.ReadLines(path);
        /// <summary>
        /// Read the lines of a file that has a specified encoding.
        /// </summary>
        public static void ReadLines(string path, Encoding encoding) => File.ReadLines(path, encoding);
        /// <summary>
        /// Replaces the contents of a specified file with the contents of another file, deleting the original file, and creating a backup of the replaced file.
        /// </summary>
        public static void Replace(string sourceFileName, string destinationFileName, string? destinationBackupFileName) => File.Replace(sourceFileName, destinationFileName, destinationBackupFileName);
        /// <summary>
        /// Replaces the contents of a specified file with the contents of another file, deleting the original file, and creating a backup of the replaced file and optionally ignores merge errors.
        /// </summary>
        public static void Replace(string sourceFileName, string destinationFileName, string? destinationBackupFileName, bool ignoreMetadataErrors) => File.Replace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors);
        /// <summary>
        /// Gets the target of the specified file link.
        /// </summary>
        public static void ResolveLinkTarget(string path, bool returnFinalTarget) => File.ResolveLinkTarget(path, returnFinalTarget);
        /// <summary>
        /// Sets the specified FileAttributes of the file on the specified path.
        /// </summary>
        public static void SetAttributes(string path, FileAttributes attributes) => File.SetAttributes(path, attributes);
        /// <summary>
        /// Sets the date and time the file was created.
        /// </summary>
        public static void SetCreationTime(string path, DateTime creationTime) => File.SetCreationTime(path, creationTime);
        /// <summary>
        /// Sets the date and time, in Coordinated Universal Time (UTC), that the file was created.
        /// </summary>
        public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc) => File.SetCreationTimeUtc(path, creationTimeUtc); // TODO: Clarify its meaning - does the function sets time in Utc or the DateTime already handles it?
        /// <summary>
        /// Sets the date and time the specified file was last accessed.
        /// </summary>
        public static void SetLastAccessTime(string path, DateTime accessTime) => File.SetLastAccessTime(path, accessTime);
        /// <summary>
        /// Sets the date and time, in Coordinated Universal Time (UTC), that the specified file was last accessed.
        /// </summary>
        public static void SetLastAccessTimeUtc(string path, DateTime utcTime) => File.SetLastAccessTimeUtc(path, utcTime); // TODO: Clarify its meaning - does the function sets time in Utc or the DateTime already handles it?
        /// <summary>
        /// Sets the date and time that the specified file was last written to.
        /// </summary>
        public static void SetLastWriteTime(string path, DateTime writeTime) => File.SetLastWriteTime(path, writeTime);
        /// <summary>
        /// Sets the date and time, in Coordinated Universal Time (UTC), that the specified file was last written to.
        /// </summary>
        public static void SetLastWriteTimeUtc(string path, DateTime utcTime) => File.SetLastWriteTimeUtc(path, utcTime); // TODO: Clarify its meaning - does the function sets time in Utc or the DateTime already handles it?
        /// <summary>
        /// Creates a new file, writes the specified byte array to the file, and then closes the file. If the target file already exists, it is truncated and overwritten.
        /// </summary>
        public static void WriteAllBytes(string path, byte[] bytes) => File.WriteAllBytes(path, bytes);
        /// <summary>
        /// Creates a new file, writes a collection of strings to the file, and then closes the file.
        /// </summary>
        public static void WriteAllLines(string path, IEnumerable<String> contents) => File.WriteAllLines(path, contents);
        /// <summary>
        /// Creates a new file by using the specified Encoding encoding, writes a collection of strings to the file, and then closes the file.
        /// </summary>
        public static void WriteAllLines(string path, IEnumerable<String> contents, Encoding encoding) => File.WriteAllLines(path, contents, encoding);
        /// <summary>
        /// Creates a new file, write the specified string array to the file, and then closes the file.
        /// </summary>
        public static void WriteAllLines(string path, string[] lines) => File.WriteAllLines(path, lines);
        /// <summary>
        /// Creates a new file, writes the specified string array to the file by using the specified Encoding encoding, and then closes the file.
        /// </summary>
        public static void WriteAllLines(string path, string[] lines, Encoding encoding) => File.WriteAllLines(path, lines, encoding);
        /// <summary>
        /// Creates a new file, writes the specified string to the file, and then closes the file. If the target file already exists, it is truncated and overwritten.
        /// </summary>
        public static void WriteAllText(string path, string text) => File.WriteAllText(path, text);
        /// <summary>
        /// Creates a new file, writes the specified string to the file using the specified Encoding encoding, and then closes the file. If the target file already exists, it is truncated and overwritten.
        /// </summary>
        public static void WriteAllText(string path, string text, Encoding encoding) => File.WriteAllText(path, text, encoding);
        #endregion

        #region Directory

        #endregion

        #region Path

        #endregion
    }
}
