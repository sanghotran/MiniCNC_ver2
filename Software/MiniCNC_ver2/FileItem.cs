using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCNC_ver2
{
    public class FileItem
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string FullName { get; set; }

        public FileItem(string name, long size, string fullname)
        {
            Name = name;
            Size = size;
            FullName = fullname;
        }
    }
}
