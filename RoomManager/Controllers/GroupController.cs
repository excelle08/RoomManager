using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RoomManager.Model;
using SqlConnection = MySql.Data.MySqlClient.MySqlConnection;

namespace RoomManager.Controllers
{
    [RouteAttribute("api/[controller]")]
    public class GroupController : Controller
    {
        static SqlConnection conn = DBConnector.GetConnection();
        static DataHelper<Group> dhGroup = new DataHelper<Group>(conn);
        
        [HttpGetAttribute("{id}")]
        public IActionResult Get(string id) {
            return new ObjectResult(dhGroup.SelectOne(String.Format("id = {0}", id)));
        }

        [HttpGetAttribute]
        public IActionResult List(string keyword = "", int page = 0, int limit = 0) {
            int offset = 0;
            if (page > 0 && limit > 0) {
                offset = (page - 1) * limit;
            }

            if (keyword != "") {
                DataHelper<Customer> dhCustomer = new DataHelper<Customer>(conn);
                IEnumerable<Customer> leaders = 
                    dhCustomer.Select(String.Format("name LIKE '%{0}%' OR identity LIKE '%{0}%' AND status != 2", keyword));

                List<Group> groups = new List<Group>();
                
                foreach (Customer c in leaders) {
                    IEnumerable<Group> gs = dhGroup.Select(String.Format("leader_id = {0}", c.Id), offset, limit);
                    foreach (Group g in gs) {
                        groups.Add(g);
                    }
                }

                return new ObjectResult(groups);
            }

            return new ObjectResult(dhGroup.Select("status != 2 ORDER BY reserve_date DESC", offset, limit));
        }

        [HttpPutAttribute]
        public IActionResult AddGroup([FromBodyAttribute] Group grp) {
            grp.Reserve_Date = Common.CurrentTimestamp();
            return new ObjectResult(new {result = dhGroup.Insert(grp)});
        }

        [HttpPostAttribute]        
        public IActionResult CheckinGroup([FromBodyAttribute] Group grp) {
            grp.Entry_Date = Common.CurrentTimestamp();
            grp.Status = CustomerStatus.CheckedIn;
            string[] ids = grp.Members.Split(',');
            DataHelper<Customer> dhCustomer = new DataHelper<Customer>(conn);
            DataHelper<Room> dhRoom = new DataHelper<Room>(conn);

            foreach(string id in ids) {
                Customer c = dhCustomer.SelectOne(String.Format("id={0}", id));
                c.Entry_Date = Common.CurrentTimestamp();
                c.Status = CustomerStatus.CheckedIn;
                dhCustomer.Update(c);

                Room r = dhRoom.SelectOne(String.Format("id={0}", c.Room_Id));
                r.Status = RoomStatus.CheckedIn;
                dhRoom.Update(r);
            }

            dhGroup.Update(grp);
            return new ObjectResult(grp);
        }

        [HttpDeleteAttribute("{id}")]
        public IActionResult CheckoutGroup(string id) {
            Group grp = dhGroup.SelectOne(String.Format("id={0}", id));
            grp.Checkout_Date = Common.CurrentTimestamp();
            grp.Status = CustomerStatus.CheckedOut;

            string[] ids = grp.Members.Split(',');
            int days = (int)Math.Ceiling((grp.Checkout_Date - grp.Entry_Date) / 86400);
            DataHelper<Room> dhRoom = new DataHelper<Room>(conn);
            DataHelper<RoomType> dhrt = new DataHelper<RoomType>(conn);
            DataHelper<Consumption> dhCons = new DataHelper<Consumption>(conn);
            DataHelper<Customer> dhCus = new DataHelper<Customer>(conn);
            
            List<Consumption> cons = new List<Consumption>();
            foreach(string i in ids) {
                float price;
                Customer c = dhCus.SelectOne(String.Format("id = {0}", i));
                Room r = dhRoom.SelectOne(String.Format("id={0}", c.Room_Id));
                RoomType rt = dhrt.SelectOne(String.Format("id={0}", r.Type));
                price = (r.Custom_Price - 1e-3 < 0) ? rt.Typical_Price : r.Custom_Price;

                IEnumerable<Consumption> ieCons = dhCons.SelectAll(String.Format("customer={0}", i));
                foreach(Consumption consumption in ieCons) {
                    cons.Add(consumption);
                }

                Consumption cos = new Consumption();
                cos.Item = 0;
                cos.Comment = String.Format("Room {0} (Type: {1}) for {2} day(s)", r.Name, rt.Name, days);
                cos.Count = 1;
                cos.Customer = c.Id;
                cos.Paid = false;
                cos.Price = price * days;

                cons.Add(cos);
                dhCons.Insert(cos);

                r.Status = RoomStatus.Vacant;
                dhRoom.Update(r);

                c.Status = CustomerStatus.CheckedOut;
                c.Checkout_Date = Common.CurrentTimestamp();
                dhCus.Update(c);
            }

            dhGroup.Update(grp);

            return new ObjectResult(cons);
        }
    }
}