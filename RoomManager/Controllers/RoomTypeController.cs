using RoomManager.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using System;

namespace RoomManager.Controllers
{
    [RouteAttribute("api/[controller]")]
    public class RoomTypeController : Controller
    {
        static SqlConnection conn = DBConnector.GetConnection();
        static DataHelper<RoomType> dhRt = new DataHelper<RoomType>(ref conn);

        [HttpGetAttribute]
        public IActionResult GetAll() {
            return new ObjectResult(dhRt.SelectAll(""));
        }

        [HttpGetAttribute("{id}")]
        public IActionResult Get(int id) {
            return new ObjectResult(dhRt.SelectOne(String.Format("id = {0}", id)));
        }

        [HttpPostAttribute("{id}")]
        public IActionResult UpdateRoomType(int id, [FromBodyAttribute] RoomType roomtype) {
            if (!UserHelper.IsAdmin(HttpContext)) {
                return Forbid();
            }
            roomtype.Id = id;
            return new ObjectResult(new {result = dhRt.Update(roomtype)});
        }

        [HttpDeleteAttribute("{id}")]
        public IActionResult DeleteRoomType(int id) {
            RoomType rt = new RoomType();
            rt.Id = id;
            if (!UserHelper.IsAdmin(HttpContext)) {
                return Forbid();
            }

            return new ObjectResult(new {result = dhRt.Delete(rt)});
        }

        [HttpPutAttribute()]
        public IActionResult AddRoomType ([FromBodyAttribute] RoomType roomtype) {
            if (!UserHelper.IsAdmin(HttpContext)) {
                return Forbid();
            }
            return new ObjectResult(new {result = dhRt.Insert(roomtype)});
        }
    }
}