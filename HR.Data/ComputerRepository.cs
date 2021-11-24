using HR.Core;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;

namespace HR.Data
{
    public class ComputerRepository
    {
        public List<Computer> computers { get; set; }
        public Computer computer { get; set; }
        public IEnumerable<Computer> GetAllComputers()
        {
            computers = new List<Computer>();
            SearchResultCollection results;
            DirectorySearcher ds = null;
            DirectoryEntry de = new DirectoryEntry(AdHelpers.GetCurrentDomainPath());
            ds = new DirectorySearcher(de);
            ds.Filter = "(objectCategory=computer)";
            ds.Sort = new SortOption("name", SortDirection.Ascending);
            results = ds.FindAll();
            if (results == null) return null;
            //SearchResult sr = results[0];
            //foreach (string di in sr.Properties.PropertyNames)  Debug.WriteLine("keys={0} ", di);
            foreach (SearchResult sr in results)
            {
                var computer = new Computer();
                computer = AdHelpers.GetAdEntity(sr, computer);
                computers.Add(computer);
            }
            return computers;
        }
    }
}
