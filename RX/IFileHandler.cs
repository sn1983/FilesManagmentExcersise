using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RX
{
    internal interface IFileHandler
    {

        void StartWatching();
        void Handle(string sourcePath, string fileName);
    }
}
