using System;
namespace JsonValidatorTool
{
    /// <summary>
    /// Json not valid exception.
    /// </summary>
    public class JsonNotValidException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:JsonValidatorTool.JsonNotValidException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public JsonNotValidException(string message) : base(message)
        {

        }
    }
}
