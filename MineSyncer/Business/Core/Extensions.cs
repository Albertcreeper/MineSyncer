using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Core
{
    public static class Extensions
    {
        public static T ConvertTo<T>(this object value)
        {
            T returnValue = default(T);

            if (value is T)
            {
                returnValue = (T)value;
            }
            else
            {
                try
                {
                    returnValue = (T)Convert.ChangeType(value, typeof(T));
                }
                catch (InvalidCastException)
                {
                    returnValue = default(T);
                }
            }

            return returnValue;
        }
    }
}
