using RoomManager.Model;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SqlConnector = MySql.Data.MySqlClient.MySqlConnection;

namespace RoomManager.Controllers
{
    [RouteAttribute("api/[controller]")]
    public class ConsumptionController : Controller
    {
        static SqlConnector conn = DBConnector.GetConnection();
        static DataHelper<Consumption> dhCons = new DataHelper<Consumption>(conn);

        [HttpGetAttribute("{customerId}")]
        public IActionResult GetUnpaidOrders(string customerId) {
            List<Object> lst = new List<Object>();
            DataHelper<Item> dhItem = new DataHelper<Item>(conn);
            IEnumerable<Consumption> cs = dhCons.Select(String.Format("customer = {0} AND paid = false", customerId));
            foreach(Consumption c in cs) {
                Item item = dhItem.SelectOne(String.Format("id = {0}", c.Item));
                lst.Add(new {
                    id = c.Id,
                    item = c.Item,
                    itemname = item == null ? c.Comment : item.Name,
                    price = c.Price,
                    count = c.Count,
                    comment = c.Comment
                });
            }
            return new ObjectResult(lst);
        }

        [HttpPutAttribute]
        public IActionResult AddOrder([FromBodyAttribute] Consumption consumption) {
            consumption.Paid = false;
            DataHelper<Item> dhItem = new DataHelper<Item>(conn);
            Item item = dhItem.SelectOne(String.Format("item = {0}", consumption.Item));
            if (item == null) {
                return NotFound(new {error = "Order", message = "Requested item does not exist."});
            }
            consumption.Price = item.Typical_Price * consumption.Count;
            
            return new ObjectResult(dhCons.Insert(consumption));
        }

        [HttpDeleteAttribute("{customerId}")]
        public IActionResult CheckoutOrder(string customerId) {
            IEnumerable<Consumption> orders = dhCons.Select(String.Format("customer = {0} AND paid = false", customerId));
            foreach (Consumption c in orders) {
                c.Paid = true;
                dhCons.Update(c);
            }

            return new ObjectResult(orders);
        }
    }
}