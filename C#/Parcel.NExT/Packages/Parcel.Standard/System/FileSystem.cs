using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Text;

namespace Parcel.Standard.System
{
    /// <summary>
    /// Contains standardized definitions for File, Directory, Path and FileSystemEntry operations.
    /// This is a very thin layer (almost duplicate) built on top of standard .Net as part of PSL for interoperability purposes.
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
        /// Creates, or truncates and overwrites, a file in the specified path.
        /// </summary>
        public static void Create(string path) => File.Create(string path);
        /// <summary>
        /// Creates, or truncates and overwrites, a file in the specified path, specifying a buffer size.
        /// </summary>
        public static void Create(string path, Int32) => File.Create(string path, Int32);
        /// <summary>
        /// Creates or overwrites a file in the specified path, specifying a buffer size and options that describe how to create or overwrite the file.
        /// </summary>
        public static void Create(string path, Int32, FileOptions) => File.Create(string path, Int32, FileOptions);
        /// <summary>
        /// Creates a file symbolic link identified by path that points to pathToTarget.
        /// </summary>
        public static void CreateSymbolicLink(string path, String) => File.CreateSymbolicLink(string path, String);
        /// <summary>
        /// Creates or opens a file for writing UTF-8 encoded text. If the file already exists, its contents are replaced.
        /// </summary>
        public static void CreateText(string path) => File.CreateText(string path);
        /// <summary>
        /// Decrypts a file that was encrypted by the current account using the Encrypt(string path) method.
        /// </summary>
        public static void Decrypt(string path) => File.Decrypt(string path);
        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        public static void Delete(string path) => File.Delete(string path);
        /// <summary>
        /// Encrypts a file so that only the account used to encrypt the file can decrypt it.
        /// </summary>
        public static void Encrypt(string path) => File.Encrypt(string path);
        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        public static void Exists(string path) => File.Exists(string path);
        /// <summary>
        /// Gets the specified FileAttributes of the file or directory associated with fileHandle.
        /// </summary>
        public static void GetAttributes(SafeFileHandle) => File.GetAttributes(SafeFileHandle);
        /// <summary>
        /// Gets the FileAttributes of the file on the path.
        /// </summary>
        public static void GetAttributes(string path) => File.GetAttributes(string path);
        /// <summary>
        /// Returns the creation time of the specified file or directory.
        /// </summary>
        public static void GetCreationTime(SafeFileHandle) => File.GetCreationTime(SafeFileHandle);
        /// <summary>
        /// Returns the creation date and time of the specified file or directory.
        /// </summary>
        public static void GetCreationTime(string path) => File.GetCreationTime(string path);
        /// <summary>
        /// Returns the creation date and time, in Coordinated Universal Time (UTC), of the specified file or directory.
        /// </summary>
        public static void GetCreationTimeUtc(SafeFileHandle) => File.GetCreationTimeUtc(SafeFileHandle);
        /// <summary>
        /// Returns the creation date and time, in Coordinated Universal Time (UTC), of the specified file or directory.
        /// </summary>
        public static void GetCreationTimeUtc(string path) => File.GetCreationTimeUtc(string path);
        /// <summary>
        /// Returns the last access date and time of the specified file or directory.
        /// </summary>
        public static void GetLastAccessTime(SafeFileHandle) => File.GetLastAccessTime(SafeFileHandle);
        /// <summary>
        /// Returns the date and time the specified file or directory was last accessed.
        /// </summary>
        public static void GetLastAccessTime(string path) => File.GetLastAccessTime(string path);
        /// <summary>
        /// Returns the last access date and time, in Coordinated Universal Time (UTC), of the specified file or directory.
        /// </summary>
        public static void GetLastAccessTimeUtc(SafeFileHandle) => File.GetLastAccessTimeUtc(SafeFileHandle);
        /// <summary>
        /// Returns the date and time, in Coordinated Universal Time (UTC), that the specified file or directory was last accessed.
        /// </summary>
        public static void GetLastAccessTimeUtc(string path) => File.GetLastAccessTimeUtc(string path);
        /// <summary>
        /// Returns the last write date and time of the specified file or directory.
        /// </summary>
        public static void GetLastWriteTime(SafeFileHandle) => File.GetLastWriteTime(SafeFileHandle);
        /// <summary>
        /// Returns the date and time the specified file or directory was last written to.
        /// </summary>
        public static void GetLastWriteTime(string path) => File.GetLastWriteTime(string path);
        /// <summary>
        /// Returns the last write date and time, in Coordinated Universal Time (UTC), of the specified file or directory.
        /// </summary>
        public static void GetLastWriteTimeUtc(SafeFileHandle) => File.GetLastWriteTimeUtc(SafeFileHandle);
        /// <summary>
        /// Returns the date and time, in Coordinated Universal Time (UTC), that the specified file or directory was last written to.
        /// </summary>
        public static void GetLastWriteTimeUtc(string path) => File.GetLastWriteTimeUtc(string path);
        /// <summary>
        /// Gets the UnixFileMode of the specified file handle.
        /// </summary>
        public static void GetUnixFileMode(SafeFileHandle) => File.GetUnixFileMode(SafeFileHandle);
        /// <summary>
        /// Gets the UnixFileMode of the file on the path.
        /// </summary>
        public static void GetUnixFileMode(string path) => File.GetUnixFileMode(string path);
        /// <summary>
        /// Moves a specified file to a new location, providing the option to specify a new file name.
        /// </summary>
        public static void Move(string path, String) => File.Move(string path, String);
        /// <summary>
        /// Moves a specified file to a new location, providing the options to specify a new file name and to replace the destination file if it already exists.
        /// </summary>
        public static void Move(string path, String, Boolean) => File.Move(string path, String, Boolean);
        /// <summary>
        /// Opens a FileStream on the specified path with read/write access with no sharing.
        /// </summary>
        public static void Open(string path, FileMode) => File.Open(string path, FileMode);
        /// <summary>
        /// Opens a FileStream on the specified path, with the specified mode and access with no sharing.
        /// </summary>
        public static void Open(string path, FileMode, FileAccess) => File.Open(string path, FileMode, FileAccess);
        /// <summary>
        /// Opens a FileStream on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.
        /// </summary>
        public static void Open(string path, FileMode, FileAccess, FileShare) => File.Open(string path, FileMode, FileAccess, FileShare);
        /// <summary>
        /// Initializes a new instance of the FileStream class with the specified path, creation mode, read/write and sharing permission, the access other FileStreams can have to the same file, the buffer size, additional file options and the allocation size.
        /// </summary>
        public static void Open(string path, FileStreamOptions) => File.Open(string path, FileStreamOptions);
        /// <summary>
        /// Initializes a new instance of the SafeFileHandle class with the specified path, creation mode, read/write and sharing permission, the access other SafeFileHandles can have to the same file, additional file options and the allocation size.
        /// </summary>
        public static void OpenHandle(string path, FileMode, FileAccess, FileShare, FileOptions, Int64) => File.OpenHandle(string path, FileMode, FileAccess, FileShare, FileOptions, Int64);
        /// <summary>
        /// Opens an existing file for reading.
        /// </summary>
        public static void OpenRead(string path) => File.OpenRead(string path);
        /// <summary>
        /// Opens an existing UTF-8 encoded text file for reading.
        /// </summary>
        public static void OpenText(string path) => File.OpenText(string path);
        /// <summary>
        /// Opens an existing file or creates a new file for writing.
        /// </summary>
        public static void OpenWrite(string path) => File.OpenWrite(string path);
        /// <summary>
        /// Opens a binary file, reads the contents of the file into a byte array, and then closes the file.
        /// </summary>
        public static void ReadAllBytes(string path) => File.ReadAllBytes(string path);
        /// <summary>
        /// Opens a text file, reads all lines of the file, and then closes the file.
        /// </summary>
        public static void ReadAllLines(string path) => File.ReadAllLines(string path);
        /// <summary>
        /// Opens a file, reads all lines of the file with the specified Encoding encoding, and then closes the file.
        /// </summary>
        public static void ReadAllLines(string path, Encoding encoding) => File.ReadAllLines(string path, Encoding encoding);
        /// <summary>
        /// Opens a text file, reads all the text in the file, and then closes the file.
        /// </summary>
        public static void ReadAllText(string path) => File.ReadAllText(string path);
        /// <summary>
        /// Opens a file, reads all text in the file with the specified Encoding encoding, and then closes the file.
        /// </summary>
        public static void ReadAllText(string path, Encoding encoding) => File.ReadAllText(string path, Encoding encoding);
        /// <summary>
        /// Reads the lines of a file.
        /// </summary>
        public static void ReadLines(string path) => File.ReadLines(string path);
        /// <summary>
        /// Read the lines of a file that has a specified encoding.
        /// </summary>
        public static void ReadLines(string path, Encoding encoding) => File.ReadLines(string path, Encoding encoding);
        /// <summary>
        /// Replaces the contents of a specified file with the contents of another file, deleting the original file, and creating a backup of the replaced file.
        /// </summary>
        public static void Replace(string path, String, String) => File.Replace(string path, String, String);
        /// <summary>
        /// Replaces the contents of a specified file with the contents of another file, deleting the original file, and creating a backup of the replaced file and optionally ignores merge errors.
        /// </summary>
        public static void Replace(string path, String, String, Boolean) => File.Replace(string path, String, String, Boolean);
        /// <summary>
        /// Gets the target of the specified file link.
        /// </summary>
        public static void ResolveLinkTarget(string path, Boolean) => File.ResolveLinkTarget(string path, Boolean);
        /// <summary>
        /// Sets the specified FileAttributes of the file or directory associated with fileHandle.
        /// </summary>
        public static void SetAttributes(SafeFileHandle fileHandle, FileAttributes) => File.SetAttributes(SafeFileHandle fileHandle, FileAttributes);
        /// <summary>
        /// Sets the specified FileAttributes of the file on the specified path.
        /// </summary>
        public static void SetAttributes(string path, FileAttributes) => File.SetAttributes(string path, FileAttributes);
        /// <summary>
        /// Sets the date and time the file or directory was created.
        /// </summary>
        public static void SetCreationTime(SafeFileHandle fileHandle, DateTime) => File.SetCreationTime(SafeFileHandle fileHandle, DateTime);
        /// <summary>
        /// Sets the date and time the file was created.
        /// </summary>
        public static void SetCreationTime(string path, DateTime) => File.SetCreationTime(string path, DateTime);
        /// <summary>
        /// Sets the date and time, in Coordinated Universal Time (UTC), that the file or directory was created.
        /// </summary>
        public static void SetCreationTimeUtc(SafeFileHandle fileHandle, DateTime) => File.SetCreationTimeUtc(SafeFileHandle fileHandle, DateTime);
        /// <summary>
        /// Sets the date and time, in Coordinated Universal Time (UTC), that the file was created.
        /// </summary>
        public static void SetCreationTimeUtc(string path, DateTime) => File.SetCreationTimeUtc(string path, DateTime);
        /// <summary>
        /// Sets the date and time the specified file or directory was last accessed.
        /// </summary>
        public static void SetLastAccessTime(SafeFileHandle fileHandle, DateTime) => File.SetLastAccessTime(SafeFileHandle fileHandle, DateTime);
        /// <summary>
        /// Sets the date and time the specified file was last accessed.
        /// </summary>
        public static void SetLastAccessTime(string path, DateTime) => File.SetLastAccessTime(string path, DateTime);
        /// <summary>
        /// Sets the date and time, in Coordinated Universal Time (UTC), that the specified file or directory was last accessed.
        /// </summary>
        public static void SetLastAccessTimeUtc(SafeFileHandle fileHandle, DateTime) => File.SetLastAccessTimeUtc(SafeFileHandle fileHandle, DateTime);
        /// <summary>
        /// Sets the date and time, in Coordinated Universal Time (UTC), that the specified file was last accessed.
        /// </summary>
        public static void SetLastAccessTimeUtc(string path, DateTime) => File.SetLastAccessTimeUtc(string path, DateTime);
        /// <summary>
        /// Sets the date and time that the specified file or directory was last written to.
        /// </summary>
        public static void SetLastWriteTime(SafeFileHandle fileHandle, DateTime) => File.SetLastWriteTime(fileHandle, DateTime);
        /// <summary>
        /// Sets the date and time that the specified file was last written to.
        /// </summary>
        public static void SetLastWriteTime(string path, DateTime) => File.SetLastWriteTime(string path, DateTime);
        /// <summary>
        /// Sets the date and time, in Coordinated Universal Time (UTC), that the specified file or directory was last written to.
        /// </summary>
        public static void SetLastWriteTimeUtc(SafeFileHandle fileHandle, DateTime) => File.SetLastWriteTimeUtc(SafeFileHandle fileHandle, DateTime);
        /// <summary>
        /// Sets the date and time, in Coordinated Universal Time (UTC), that the specified file was last written to.
        /// </summary>
        public static void SetLastWriteTimeUtc(string path, DateTime) => File.SetLastWriteTimeUtc(string path, DateTime);
        /// <summary>
        /// Sets the specified UnixFileMode of the specified file handle.
        /// </summary>
        public static void SetUnixFileMode(SafeFileHandle fileHandle, UnixFileMode) => File.SetUnixFileMode(SafeFileHandle fileHandle, UnixFileMode);
        /// <summary>
        /// Sets the specified UnixFileMode of the file on the specified path.
        /// </summary>
        public static void SetUnixFileMode(string path, UnixFileMode) => File.SetUnixFileMode(string path, UnixFileMode);
        /// <summary>
        /// Creates a new file, writes the specified byte array to the file, and then closes the file. If the target file already exists, it is truncated and overwritten.
        /// </summary>
        public static void WriteAllBytes(string path, Byte[] bytes) => File.WriteAllBytes(path, bytes);
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
