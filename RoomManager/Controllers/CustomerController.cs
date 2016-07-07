using RoomManager.Model;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SqlConnection = MySql.Data.MySqlClient.MySqlConnection;

namespace RoomManager.Controllers
{
    [RouteAttribute("api/[controller]")]
    public class CustomerController : Controller
    {
        static SqlConnection conn = DBConnector.GetConnection();
        static DataHelper<Customer> dhCustomer = new DataHelper<Customer>(ref conn);
        [HttpGetAttribute]
        public IActionResult list(string keyword = "", int page = 0, int limit = 0) {
            int offset = 0;
            if (page != 0 && limit != 0) {
                offset = (page - 1) * limit;
            }

            IEnumerable<Customer> res = 
                dhCustomer.Select(String.Format("name LIKE '%{0}%' OR identity LIKE '%{0}%' AND status != 2 ORDER BY reserve_date DESC", keyword), offset, limit);
            
            return new ObjectResult(res);
        }

        [HttpGetAttribute("{id}")]
        public IActionResult Get(string id) {
            return new ObjectResult(dhCustomer.SelectOne(String.Format("id = {0}", id)));
        }
    }
}