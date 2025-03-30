using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Core.Exceptions
{
    public class AppException : ApplicationException
    {
        public AppException() : base()
        {

        }

        public AppException(string message) : base(message)
        {

        }

        public AppException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
