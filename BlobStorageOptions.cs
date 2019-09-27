using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBotTranscript
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Options for blob storage connectivity (transcripts, etc.).
    /// </summary>
    /// <remarks>
    /// See "options pattern" to learn about injecting this options.
    /// </remarks>
    public class BlobStorageOptions
    {
        /// <summary>
        /// Gets or sets the Azure blob storage container name for transcripts.
        /// </summary>
        [Required]
        public string TranscriptsContainer { get; set; }

        /// <summary>
        /// Gets or sets the connection string to be used to connect to the storage.
        /// </summary>
        [Required]
        public string ConnectionString { get; set; }
    }
}
