using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monads
{
    [Serializable]
    /// <inheritdoc cref="ArgumentException"/>
    public sealed class TypeArgumentException : ArgumentException
    {
        /// <inheritdoc cref="ArgumentException()"/>
        public TypeArgumentException() { }
        /// <inheritdoc cref="ArgumentException(string)"/>
        public TypeArgumentException(string message) : base(message) { }
        /// <inheritdoc cref="ArgumentException(string, Exception)"/>
        public TypeArgumentException(string message, Exception inner) : base(message, inner) { }
        /// <inheritdoc cref="ArgumentException(string, string)"/>
        public TypeArgumentException(string message, string paramName) : base(message, paramName) { }
        /// <inheritdoc cref="ArgumentException(string, string, Exception)"/>
        public TypeArgumentException(string message, string paramName, Exception inner) : base(message, paramName, inner) { }
    }
}
