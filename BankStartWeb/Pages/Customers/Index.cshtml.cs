using BankAppWeb.Infrastructure.Paging;
using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankAppWeb.Pages.Customers
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext context;

        public IndexModel(ApplicationDbContext context)
        {
            this.context = context;
        }
        public List<CustomerViewModel> Customers { get; set; }
        public class CustomerViewModel
        {
            public int Id { get; set; }
            public string Surname { get; set; }
            public string City { get; set; }
            public List<Account>? Accounts { get; set; }
        }
        public int PageNo { get; set; }
        public string SortCol { get; set; }
        public string Sort { get; set; }
        public int TotalPageCount { get; set; }
        public void OnGet(int pageno = 1, string col = "Surname", string order = "asc")
        {
            PageNo = pageno;
            SortCol = col;
            Sort = order;

            var cus = context.Customers.AsQueryable();

            cus = cus.OrderBy(col, order == "asc" ? ExtensionMethods.QuerySortOrder.Asc : ExtensionMethods.QuerySortOrder.Desc);

            var pageResult = cus.GetPaged(PageNo, 20);

            TotalPageCount = pageResult.PageCount;

            Customers = pageResult.Results.Select(c => new CustomerViewModel
            {
                Id = c.Id,
                Surname = c.Surname,
                City = c.City,
                Accounts = c.Accounts
            }).ToList();
        }
    }
}
