using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HR.Core;
using HR.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRADWebReporting.Pages.Home
{
    public class UsersListModel : PageModel
    {
        private readonly HRRepository hrrepo;
        public IEnumerable<HRUser> hrusers = new List<HRUser>();
        public IEnumerable<Group> groups = new List<Group>();
        public UsersListModel(HRRepository hrrepo)
        {
            this.hrrepo = hrrepo;
        }
        public IActionResult OnGet()
        {
            //hrusers = hrrepo.GetDataExcel();
            groups = hrrepo.GetGroups();
            foreach (Group g in groups)
            {
               
                    
                        Debug.WriteLine(g.Name + "  " + g.User.username + "    " + g.User.RadnoMjesto);
                

            }
            return Page();
        }
    }
}
