using System.Collections.Generic;

namespace HR.Core
{
    public class Group
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string GroupType { get; set; }
        public List<string> Members { get; set; }
        public List<HRUser> Users { get; set; }
        public HRUser User { get; set; }
    }
}
