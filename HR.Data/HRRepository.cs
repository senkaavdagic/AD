using HR.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Text;

namespace HR.Data
{
    public class HRRepository
    {
        public List<HRUser> hrusers = new List<HRUser>();
        public HRUser hruser = new HRUser();
        public Group group = new Group();
        public List<Group> groups = new List<Group>();
        public List<Sifrarnik> sifre = new List<Sifrarnik>();
        public Sifrarnik sifrarnik = new Sifrarnik();
        public IEnumerable<HRUser> GetDataExcel()
        {
            DataTable sheetData = new DataTable();
            string strExcelConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=.\\reports\\HRMIS_Data_2021.xlsx;Extended Properties='Excel 8.0;HDR=Yes'";
            //string strExcelConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\repos\\reports\\HRMIS_Data_2021.xlsx;Extended Properties='Excel 8.0;HDR=Yes'";
            OleDbConnection connExcel = new OleDbConnection(strExcelConn);
            OleDbCommand cmdExcel = new OleDbCommand();
            cmdExcel.Connection = connExcel;
            connExcel.Open();
            DataTable dtExcelSchema = new DataTable();
            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            connExcel.Close();
            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
            OleDbDataAdapter da = new OleDbDataAdapter();
            DataSet ds = new DataSet();
            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
            da.SelectCommand = cmdExcel;
            da.Fill(ds);
            sheetData = ds.Tables[0];
            //Debug.WriteLine(ds.GetXml());
            foreach (DataRow dr in sheetData.Rows)
            {
                // Debug.WriteLine(dr[0].ToString() + "   " + dr[4].ToString());
                
                hruser = new HRUser
                {
                    username = dr["Username"].ToString().Split('\\')[1],
                    ime = dr["Ime"].ToString(),
                    prezime = dr["Prezime"].ToString(),
                    OrganizacionaJedinica = dr["OrganizacionaJedinica"].ToString(),
                    OrganizacioniOblik = dr["OrganizacioniOblik"].ToString(),
                    //KompletnaSifra = dr["KompletnaSifra"].ToString(),
                    RadnoMjesto = dr["RadnoMjesto"].ToString()
                    //KompletnaSifra1 = dr["KompletnaSifra1"].ToString()
                };

                hrusers.Add(hruser);
            }
            return hrusers;
        }
        public string FindUsername(string cn)
        {
            DirectorySearcher ds = null;
            DirectoryEntry de = new DirectoryEntry(AdHelpers.GetCurrentDomainPath());
            SearchResult sr;
            string samaccountname = String.Empty;
            ds = new DirectorySearcher(de);
            var temp = cn.Substring(0, cn.IndexOf(','));
            ds.Filter = "(&(objectCategory=User)(objectClass=person)(" + temp + "))";
            ds.PropertiesToLoad.Add("samaccountname");
            sr = ds.FindOne();
            if (sr == null) { return null; }
            if (sr.Properties["samaccountname"].Count > 0) 
            {
                samaccountname = sr.Properties["samaccountname"][0].ToString();
            }
            return samaccountname;
        }

        
        public IEnumerable<Group> GetGroups()
        {
            SearchResultCollection results;
            DirectorySearcher ds = null;
            DirectoryEntry de = new DirectoryEntry(AdHelpers.GetCurrentDomainPath());
            ds = new DirectorySearcher(de);
            // Sort by name
            ds.Sort = new SortOption("name", SortDirection.Ascending);
            ds.Filter = "(&(objectCategory=Group)(objectClass=group))";
            ds.PropertiesToLoad.Add("name");
            ds.PropertiesToLoad.Add("member");
            ds.PropertiesToLoad.Add("grouptype");
            ds.PropertiesToLoad.Add("description");
            results = ds.FindAll();
            hrusers = Enumerable.ToList(GetDataExcel());
                       
            HRUser hruserEmpty = new HRUser
            {
                username = " ",
                ime = " ",
                prezime = "  ",
                OrganizacionaJedinica = "   ",
                OrganizacioniOblik = "   ",
                RadnoMjesto = "   "
            };
            foreach (SearchResult sr in results)
            {
                List<HRUser> users = new List<HRUser>();
               
                if (sr.Properties["member"].Count > 0)
                {
                    foreach (string item in sr.Properties["member"])
                    {
                        
                        var samaccountname = FindUsername(item);
                        
                        hruser = hrusers.Where(u => u.username.Equals(samaccountname)).FirstOrDefault();
                        if (hruser != null) {
                            // users.Add(hruserEmpty);
                            var group = new Group
                            {
                                Name = sr.GetPropertyValue("name"),
                                Description = sr.GetPropertyValue("description"),
                                GroupType = sr.GetPropertyValue("grouptype"),
                                User = hruser
                                // MembersName = membersName
                            };
                            groups.Add(group);
                        }
                        //else { users.Add(hruser); }
                        
                    }
                    
                }

               
            }
            return groups;
        }
    }
}
