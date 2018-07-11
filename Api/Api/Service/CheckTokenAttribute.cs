using Microsoft.AspNetCore.Mvc;

namespace Api.Service
{
    /// <summary>
    /// Custom attribute to implemented the CheckTokenFilter to handle the Authorization header
    /// </summary>
    public class CheckTokenAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CheckTokenAttribute() : base(typeof(CheckTokenFilter))
        {

        }
    }
}
