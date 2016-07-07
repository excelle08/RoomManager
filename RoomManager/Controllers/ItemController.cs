using System;
using Microsoft.AspNetCore.Mvc;
using RoomManager.Model;
using SqlConnection = MySql.Data.MySqlClient.MySqlConnection;

namespace RoomManager.Controllers
{
    [RouteAttribute("api/[controller]")]
    public class ItemController : Controller
    {
        static SqlConnection conn = DBConnector.GetConnection();
        static DataHelper<Item> dhItem = new DataHelper<Item>(conn);

        [HttpGetAttribute]
        public IActionResult Search(string keyword = "", int page = 0, int limit = 0) {
            int offset = 0;
            if (page != 0 && limit != 0) {
                offset = (page - 1) * limit;
            }
            return new ObjectResult(dhItem.Select(String.Format("name LIKE '%{0}%'", keyword), offset, limit));
        }

        [HttpGetAttribute("{id}")]
        public IActionResult Get(string id) {
            return new ObjectResult(dhItem.SelectOne(String.Format("id = {0}", id)));
        } 

        [HttpPutAttribute]
        public IActionResult AddItem([FromBodyAttribute] Item item) {
            if (!UserHelper.IsAdmin(HttpContext)) {
                return Forbid();
            }

            return new ObjectResult(dhItem.Insert(item));
        }

        [HttpPostAttribute]
        public IActionResult UpdateItem([FromBodyAttribute] Item item) {
            if (!UserHelper.IsAdmin(HttpContext)) {
                return Forbid();
            }

            return new ObjectResult(new {result = dhItem.Update(item)});
        }

        [HttpDeleteAttribute("{id}")]
        public IActionResult DeleteItem(int id) {
            if (!UserHelper.IsAdmin(HttpContext)) {
                return Forbid();
            }
            Item item = new Item();
            item.Id = id;

            return new ObjectResult(new {result = dhItem.Delete(item)});
        }
    }
}