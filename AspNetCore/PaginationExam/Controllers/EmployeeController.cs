using Microsoft.AspNetCore.Mvc;
using PaginationExam.Dao;
using PaginationExam.Models;
using System.Collections.Generic;

namespace PaginationExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : Controller
    {
        // 1ページに5件
        const int CountPerPage = 5;

        [HttpGet]
        public IEnumerable<Employee> GetList([FromQuery] int page)
        {
            if (page < 1) // 1ページスタートなので、1未満は1に簡単に補正
                page = 1;

            // データ取得
            // 全件数、該当ページのEmployeeリスト
            var dao = new EmployeeDao();
            (int totalItemCount, int lastPage, List<Employee> employees) = 
                dao.GetEmployees(page, EmployeeController.CountPerPage);

            // Response Header追加
            this.Response.Headers.Add("Links", this.CreateLinksHeader("Employee", page, lastPage));
            this.Response.Headers.Add("X-TotalItemCount", totalItemCount.ToString());
            this.Response.Headers.Add("X-CurrentPage", page.ToString());

            // Body Jsonは本来のデータのみ
            return employees;
        }

        /// <summary>
        /// Pagination用のLinkヘッダ値を作成
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="currentPage"></param>
        /// <param name="lastPage"></param>
        /// <returns></returns>
        protected string CreateLinksHeader(string controller, int currentPage, int lastPage)
        {
            List<string> links = new List<string>();

            links.Add(string.Format("<{0}>; rel=\"first\"", this.Url.Link("", new { Controller = controller, page = 1 })));
            if (currentPage > 1)
            {
                links.Add(string.Format("<{0}>; rel=\"prev\"", this.Url.Link("", new { Controller = controller, page = currentPage - 1 })));
            }
            if (currentPage < lastPage)
            {
                links.Add(string.Format("<{0}>; rel=\"next\"", this.Url.Link("", new { Controller = controller, page = currentPage + 1 })));
            }
            links.Add(string.Format("<{0}>; rel=\"last\"", this.Url.Link("", new { Controller = controller, page = lastPage })));

            return string.Join(", ", links);
        }
    }
}
