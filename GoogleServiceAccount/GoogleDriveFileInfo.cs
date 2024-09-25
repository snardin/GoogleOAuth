using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleServiceAccount
{
    public class GoogleDriveFileInfo    
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long? Size { get; set; }
        public long? Version { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string Link { get; set; }
        public bool IsFolder { get; set; }
        public List<string> Permissions { get; set; }
    }
}
