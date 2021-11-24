using System;
using System.Collections.Generic;
using System.Text;

namespace HR.Core
{
    public class Shares
    {
        public string ShareName { get; set; }
        public string UserName { get; set; }
        public string FilePermissions { get; set; }
        //public List<HRUser> Users { get; set; }
        public HRUser User { get; set; }
    }
}
