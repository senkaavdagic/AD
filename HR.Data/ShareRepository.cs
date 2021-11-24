using HR.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace HR.Data
{
    public class ShareRepository
    {
        public List<Shares> shares = new List<Shares>();
        public Shares share = new Shares();
        //DataTable sheetDataShares = new DataTable();
        HRUser hruser = new HRUser();
        List<HRUser> hrusers = new List<HRUser>();
        //string sharessheet = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\repos\\reports\\Shares_Data_2021.xlsx;Extended Properties='Excel 8.0;HDR=Yes'";
        //string orgsheet = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\repos\\reports\\HRMIS_Data_2021.xlsx;Extended Properties='Excel 8.0;HDR=Yes'";   
        string sharessheet = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=.\\reports\\Shares_Data_2021.xlsx;Extended Properties='Excel 8.0;HDR=Yes'";
        string orgsheet = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=.\\reports\\HRMIS_Data_2021.xlsx;Extended Properties='Excel 8.0;HDR=Yes'";
        public IEnumerable<HRUser> GetData()
        {
            // DataTable sheetData = GetDataExcelOrg(sharessheet);
            DataTable sheetDataShares = GetDataExcelOrg(orgsheet);
            foreach (DataRow dr in sheetDataShares.Rows)
            {
                // Debug.WriteLine(dr[0].ToString() + "   " + dr[4].ToString());

                hruser = new HRUser
                {
                    username = dr["username"].ToString().Split('\\')[1],
                    ime = dr["ime"].ToString(),
                    prezime = dr["prezime"].ToString(),
                    OrganizacionaJedinica = dr["OrganizacionaJedinica"].ToString(),
                    OrganizacioniOblik = dr["OrganizacioniOblik"].ToString(),
                    RadnoMjesto = dr["RadnoMjesto"].ToString(),
                };
                hrusers.Add(hruser);
            }
            return hrusers;
        }

        public IEnumerable<Shares> GetShares()
        {
            // DataTable sheetData = GetDataExcelOrg(sharessheet);
            DataTable sheetDataShares = GetDataExcelOrg(sharessheet);
            HRUser hruser = new HRUser();
            hrusers = Enumerable.ToList(GetData());
            foreach (DataRow dr in sheetDataShares.Rows)
            {
                // Debug.WriteLine(dr[0].ToString() + "   " + dr[4].ToString());
                string samaccountname = dr["User Identity"].ToString();
                HRUser hruserEmpty = new HRUser
                {
                    username = " ",
                    ime = " ",
                    prezime = "  ",
                    OrganizacionaJedinica = "   ",
                    OrganizacioniOblik = "   ",
                    RadnoMjesto = "   "
                };
                string ime = string.Empty;
                string prezime = string.Empty;
                
                if (String.IsNullOrEmpty(samaccountname)) { hruser = hruserEmpty; }
                else { 
                    hruser = hrusers.Where(u => u.username.Equals(samaccountname)).FirstOrDefault();  
                    if (hruser == null) 
                    {
                        var names = AdHelpers.FindUser(samaccountname);
                        hruserEmpty.username = samaccountname;
                        hruserEmpty.ime = names.ime;
                        hruserEmpty.prezime = names.prezime;
                        hruser = hruserEmpty;
                    }
                }
                   
                share = new Shares
                {
                    UserName = dr["User Identity"].ToString(),
                    ShareName = dr["Share Name"].ToString(),
                    FilePermissions = dr["File System Rights"].ToString(),
                    User = hruser
                };
                shares.Add(share);
            }
            return shares;
        }


        //public IEnumerable<Shares> GetDataExcel()
        //{
        //    DataTable sheetData = new DataTable();
        //    string strExcelConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\repos\\Shares_Data_2021.xlsx;Extended Properties='Excel 8.0;HDR=Yes'";
        //    OleDbConnection connExcel = new OleDbConnection(strExcelConn);
        //    OleDbCommand cmdExcel = new OleDbCommand();
        //    cmdExcel.Connection = connExcel;
        //    connExcel.Open();
        //    DataTable dtExcelSchema = new DataTable();
        //    dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //    connExcel.Close();
        //    string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
        //    OleDbDataAdapter da = new OleDbDataAdapter();
        //    DataSet ds = new DataSet();
        //    cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
        //    da.SelectCommand = cmdExcel;
        //    da.Fill(ds);
        //    sheetData = ds.Tables[0];
            
        //    foreach (DataRow dr in sheetData.Rows)
        //    {
                

        //        share = new Shares
        //        {
        //            //UserName = dr["User Identity"].ToString().Split('\\')[1],
        //            UserName = dr["User Identity"].ToString(),
        //            ShareName = dr["Share Name"].ToString(),
        //            FilePermissions = dr["File System Rights"].ToString(),
                   
        //        };

        //        shares.Add(share);
        //    }
        //    return shares;
        //}

        public DataTable GetDataExcelOrg(string connectstring)
        {
            DataTable sheetData = new DataTable();
            //string strExcelConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\repos\\HRMIS_Data_2021.xlsx;Extended Properties='Excel 8.0;HDR=Yes'";
            string strExcelConn = connectstring;
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
            return sheetData;
            //Debug.WriteLine(ds.GetXml());
           
        }
    }
}
