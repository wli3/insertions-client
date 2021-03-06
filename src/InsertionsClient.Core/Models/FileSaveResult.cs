using System;

namespace Microsoft.DotNet.InsertionsClient.Models
{
    /// <summary> Stores the result of a file save operation.
    public struct FileSaveResult : IEquatable<FileSaveResult>
    {
        /// <summary> Path to the file that was saved. </summary>
        public string Path { get; private set; }

        /// <summary> Exception that was thrown when saving the file, if exists. </summary>
        public Exception? Exception { get; private set; }

        /// <summary> Was file save operation successful? If yes, <see cref="Exception"/>
        /// property holds a reference to the exception that was thrown. </summary>
        public bool Succeeded => Exception == null;

        /// <summary> Creates an instance of FileSaveResult </summary>
        public FileSaveResult(string path, Exception? exception = null)
        {
            Path = path;
            Exception = exception;
        }

        /// <summary>
        /// Compare given instance to this.
        /// </summary>
        /// <param name="other">Instance to compare to this.</param>
        /// <returns>True if both instances are storing the same data. False, otherwise</returns>
        public bool Equals(FileSaveResult other)
        {
            return Path == other.Path && Exception == other.Exception;
        }
    }
}