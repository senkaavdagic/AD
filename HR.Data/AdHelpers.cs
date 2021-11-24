using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Reflection;
using System.Text;

namespace HR.Data
{
    public static class AdHelpers
    {
        public static string GetCurrentDomainPath()
        {
            DirectoryEntry de = new DirectoryEntry("LDAP://RootDSE");

            return "LDAP://" + de.Properties["defaultNamingContext"][0].ToString();
        }

        public static string GetPropertyValue(this SearchResult sr, string propertyName)
        {
            string result = string.Empty;
            //string result = "value not set";
            if (sr.Properties[propertyName].Count < 1) return result;

            if (String.Equals(propertyName, "memberof"))
            {
                List<string> memberof = new List<string>();
                foreach (string item in sr.Properties["memberof"])
                {
                    var temp = item.TrimStart('C', 'N', '=').Split(',')[0];

                    memberof.Add(temp);
                }
                result = string.Join("; ", memberof);
                return result;

            }

            if (String.Equals(propertyName, "useraccountcontrol"))
            {
                int flags = (int)sr.Properties[propertyName][0];
                var isEnabled = !Convert.ToBoolean(flags & 0x0002);
                if (isEnabled) result = "Enabled"; else result = "Disabled";
                return result;
            }

            if (sr.Properties[propertyName][0].GetType() == typeof(long))
            {
                var value = (long)sr.Properties[propertyName][0];
                if (String.Equals(propertyName, "lockouttime") && (value == 0)) { result = "Account is not currently locked out"; return result; }
                if (String.Equals(propertyName, "accountexpires") && ((value == 9223372036854775807) || (value == 0))) { result = "Account never expires"; return result; }
                // result = DateTime.FromFileTimeUtc(value).ToString();
                result = TimeZoneInfo.ConvertTimeFromUtc(DateTime.FromFileTimeUtc(value), TimeZoneInfo.Local).ToString();
                return result;
            }
            if (sr.Properties[propertyName][0].GetType() == typeof(DateTime))
            {
                var value = (DateTime)sr.Properties[propertyName][0];
                //result = value.ToString();
                result = TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.Local).ToString();
                return result;
            }
            if (String.Equals(propertyName, "accounttype") && ((int)sr.Properties[propertyName][0] == 805306368)) { result = "User account"; return result; }
            if (String.Equals(propertyName, "accounttype") && ((int)sr.Properties[propertyName][0] == 805306369)) { result = "Computer account"; return result; }
          
            if (String.Equals(propertyName, "grouptype")) 
            {
                var groupType = sr.Properties["grouptype"][0].ToString();
                switch (groupType)
                {
                    case "2":
                        result = "Global Distribution Group"; 
                        break;
                    case "4":
                        result = "Domain local distribution group";
                        break;
                    case "8":
                        result = "Universal distribution group";
                        break;
                    case "-2147483646":
                        result = "Global security group";
                        break;
                    case "-2147483644":
                        result = "Domain local security group";
                        break;
                    case "-2147483640":
                        result = "Universal security group";
                        break;
                    default:
                        result = "Could not determine the group type.";
                        break;
                }
                return result;
            }

            result = sr.Properties[propertyName][0].ToString();
            return result;
        }

        public static T GetAdEntity<T>(SearchResult sr, T myObject) where T : new()
        {
            ResultPropertyCollection srproperties = sr.Properties;
            foreach (var prop in myObject.GetType().GetProperties())
            {
                prop.SetValue(myObject, "Value_Not_Set");
            }
            foreach (string ldapField in srproperties.PropertyNames)
            {
                PropertyInfo prop = myObject.GetType().GetProperty(ldapField.Replace("-", ""));
                if (prop != null) prop.SetValue(myObject, sr.GetPropertyValue(ldapField));
            }
            return myObject;
        }

        public static (string ime, string prezime) FindUser(string samaccountname)
        {
            DirectorySearcher ds = null;
            DirectoryEntry de = new DirectoryEntry(AdHelpers.GetCurrentDomainPath());
            SearchResult sr;
            ds = new DirectorySearcher(de);
            string ime = String.Empty;
            string prezime = String.Empty;
            ds.Filter = "(&(objectCategory=User)(objectClass=person)(samaccountname=" + samaccountname + "))";
            ds.PropertiesToLoad.Add("samaccountname");
            //ds.PropertiesToLoad.Add("givenname");
            ds.PropertiesToLoad.Add("displayname");
            sr = ds.FindOne();
            if (sr == null) { return (ime: " " , prezime: " "); }
            if (sr.Properties["displayname"].Count > 0) 
            {

                ime = sr.Properties["displayname"][0].ToString().Split()[0];
                if (sr.Properties["displayname"][0].ToString().Split().Length>1)
                prezime = sr.Properties["displayname"][0].ToString().Split()[1];
                
            }


            return (ime: ime, prezime: prezime);
        }
    }
}
