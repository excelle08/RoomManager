using System;
using Microsoft.AspNetCore.Mvc;
using RoomManager.Model;
using SqlConnection = MySql.Data.MySqlClient.MySqlConnection;

namespace RoomManager.Controllers
{
    [RouteAttribute("api/[controller]")]
    public class GroupController : Controller
    {
        static SqlConnection conn = DBConnector.GetConnection();
        static DataHelper<Group> dhGroup = new DataHelper<Group>(ref conn);
        
        [HttpGetAttribute("{id}")]
        public IActionResult Get(string id) {
            return new ObjectResult(dhGroup.SelectOne(String.Format("id = {0}", id)));
        }

        [HttpGetAttribute]
        public IActionResult List(int page = 0, int limit = 0) {
            int offset = 0;
            if (page > 0 && limit > 0) {
                offset = (page - 1) * limit;
            }

            return new ObjectResult(dhGroup.Select("status != 2 ORDER BY reserve_date DESC", offset, limit));
        }

        [HttpPutAttribute]
        public IActionResult AddGroup([FromBodyAttribute] Group group) {
            return new ObjectResult(new {result = dhGroup.Insert(group)});
        }
        
    }
}