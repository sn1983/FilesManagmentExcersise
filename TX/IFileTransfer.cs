using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TX
{
    public interface IFileTransfer
    {
        void StartWatching();
        void Transfer(string sourcePath, string fileName);
    }
}
