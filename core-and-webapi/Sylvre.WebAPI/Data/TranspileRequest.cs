using System.ComponentModel.DataAnnotations;

namespace Sylvre.WebAPI.Data
{
    /// <summary>
    /// Represents the transpile request from the client.
    /// </summary>
    public class TranspileRequest
    {
        /// <summary>
        /// The full Sylvre code to transpile.
        /// </summary>
        [Required]
        public string Code { get; set; }
    }
}
