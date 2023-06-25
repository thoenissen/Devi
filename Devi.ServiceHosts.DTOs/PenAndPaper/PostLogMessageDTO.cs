using System.Text.Json.Nodes;

using Devi.ServiceHosts.DTOs.PenAndPaper.Enumerations;

namespace Devi.ServiceHosts.DTOs.PenAndPaper
{
    /// <summary>
    /// Post log message
    /// </summary>
    public class PostLogMessageDTO
    {
        /// <summary>
        /// Type
        /// </summary>
        public LogMessageType Type { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        public JsonObject Content { get; set; }
    }

    /// <summary>
    /// Post log message
    /// </summary>
    /// <typeparam name="T">Sub type</typeparam>
    public class PostLogMessageDTO<T>
    {
        /// <summary>
        /// Type
        /// </summary>
        public LogMessageType Type { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        public T Content { get; set; }
    }
}