using BankAppWeb.Infrastructure.Paging;
using BankStartWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankAppWeb.Pages.Customers
{
    [Authorize]
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
            public string Givenname { get; set; }
            public string Fullname => $"{Givenname} {Surname}";
            public string City { get; set; }
            public string NationalId { get; set; }
        }
        public string SearchWord { get; set; }
        public int PageNo { get; set; }
        public string SortCol { get; set; }
        public string Sort { get; set; }
        public int TotalPageCount { get; set; }
        public void OnGet(string SearchWord, int pageno = 1, string col = "Givenname", string order = "asc")
        {
            PageNo = pageno;
            SortCol = col;
            Sort = order;

            var cus = context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(SearchWord))
                cus = cus.Where(customer => customer.Givenname.Contains(SearchWord)
                || customer.City.Contains(SearchWord));

            cus = cus.OrderBy(col, order == "asc" ? ExtensionMethods.QuerySortOrder.Asc : ExtensionMethods.QuerySortOrder.Desc);

            var pageResult = cus.GetPaged(PageNo, 50);

            TotalPageCount = pageResult.PageCount;

            Customers = pageResult.Results.Select(c => new CustomerViewModel
            {
                Id = c.Id,
                Givenname = c.Givenname,
                Surname = c.Surname,
                City = c.City,
                NationalId = c.NationalId
        }).ToList();
        }
    }
}
