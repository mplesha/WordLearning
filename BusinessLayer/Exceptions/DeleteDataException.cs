using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Exceptions
{
    internal class DeleteTranslationException : Exception
    {
        public DeleteTranslationException(string message)
            : base(message)
        {
        }
    }
}
