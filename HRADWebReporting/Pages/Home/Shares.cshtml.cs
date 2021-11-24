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
    public class SharesModel : PageModel
    {
        private readonly ShareRepository sharesrepo;
        public IEnumerable<Shares> shares = new List<Shares>();
        public SharesModel(ShareRepository sharesrepo)
        {
            this.sharesrepo = sharesrepo;
        }
        public IActionResult OnGet()
        {
            //shares = sharesrepo.GetDataExcel();
            shares = sharesrepo.GetShares();
            return Page();
        }
    }
}
