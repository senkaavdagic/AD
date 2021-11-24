using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.Core;
using HR.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRADWebReporting.Pages.Home
{
    public class ComputerListModel : PageModel
    {
        public IEnumerable<Computer> computers;
        public Computer computer;
        private readonly ComputerRepository computerData;

        public ComputerListModel(ComputerRepository computerData)
        {
            this.computerData = computerData;
        }
        public IActionResult OnGet()
        {
            computers = computerData.GetAllComputers();
            return Page();
        }

    }
}
