using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RoomManager.Model;
using RoomManager;
using SqlConnection = MySql.Data.MySqlClient.MySqlConnection;

namespace RoomManager.Controllers
{
    [RouteAttribute("api/[controller]")]
    public class RoomController : Controller
    {
        static SqlConnection conn = DBConnector.GetConnection();
        static DataHelper<Room> dhRoom = new DataHelper<Room>(ref conn);
        static DataHelper<Customer> dhCustomer = new DataHelper<Customer>(ref conn);
        [HttpGetAttribute]
        public IActionResult ListRooms(int page = 0, int limit = 0) {
            int offset = 0;
            if (page > 0 && limit > 0) {
                offset = limit * (page - 1);
            }

            return new ObjectResult(dhRoom.Select("", offset, limit));
        }

        [HttpGetAttribute("{id}")]
        public IActionResult Get(string id) {
            Room room = dhRoom.SelectOne(String.Format("id = {0}", id));

            if (room == null) {
                return NotFound();
            }
            
            return new ObjectResult(room);
        }

        [HttpGetAttribute("{id}/customers")]
        public IActionResult GetCustomer(string id) {
            IEnumerable<Customer> customers;
            customers = dhCustomer.Select(String.Format("room_id = {0} AND status != 2", id));
            return new ObjectResult(customers);
        }

        [HttpPostAttribute("{id}/customers")]
        public IActionResult CustomerReserve(string id, [FromBodyAttribute] Customer[] customers) {
            Room currentRoom = dhRoom.SelectOne(String.Format("id = {0}", id));
            if (currentRoom == null) {
                return NotFound();
            }
            if (currentRoom.Capacity < customers.Length) {
                return new ObjectResult(new {error = "CustomerSetting", 
                    message = String.Format("This room can hold {0} customers, but the request posted {1}.", 
                        currentRoom.Capacity, customers.Length)});
            }
            if (currentRoom.Status != RoomStatus.Vacant) {
                return new ObjectResult(new {error = "CustomerSetting",
                    message = "This room is not yet available."});
            }

            currentRoom.Status = RoomStatus.Reserved;
            
            List<Customer> res = new List<Customer>();
            foreach (Customer c in customers) {
                c.ReserveDate = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                res.Add(dhCustomer.Insert(c));
            }

            return new ObjectResult(res);
        }

        [HttpPostAttribute("{id}/customer")]
        public IActionResult CustomerCheckin(string id, [FromBodyAttribute] Customer customer) {
            Room currentRoom = dhRoom.SelectOne(String.Format("id = {0}", id));
            if (currentRoom == null) {
                return NotFound();
            }

            Customer registeredCustomer = dhCustomer.SelectOne(
                String.Format("id = {0} AND identity = {1} AND name = {2} AND room_id = {3} AND status = 0", 
                    customer.Id, customer.Identity, customer.Name, customer.RoomId));
            
            if (registeredCustomer == null) {
                if (currentRoom.Status != RoomStatus.Vacant) {
                    return new ObjectResult(new {error = "CustomerCheckin",
                        message = "Room is not vacant, and customer not reserved."});
                }
                customer.ReserveDate = Common.CurrentTimestamp();
                registeredCustomer = dhCustomer.Insert(customer);
            }

            registeredCustomer.Status = CustomerStatus.CheckedIn;
            registeredCustomer.EntryDate = Common.CurrentTimestamp();
            currentRoom.Status = RoomStatus.CheckedIn;
            dhRoom.Update(currentRoom);
            dhCustomer.Update(registeredCustomer);

            return new ObjectResult(registeredCustomer);
        }

        [HttpDeleteAttribute("{id}/customer")]
        public IActionResult CustomerCheckout(string id, [FromBody] Customer customer) {
            Room currentRoom = dhRoom.SelectOne(String.Format("id = {0}", id));
            if (currentRoom == null) {
                return NotFound();
            }
            
            Customer registeredCustomer = dhCustomer.SelectOne(
                String.Format("id = {0} AND identity = {1} AND name = {2} AND room_id = {3} AND status = 1", 
                    customer.Id, customer.Identity, customer.Name, customer.RoomId));

            if(registeredCustomer == null) {
                return NotFound(new {error = "CustomerCheckout", 
                    message = String.Format("The customer {0} does not exist or not in CheckedIn mode", customer.Name)});
            }

            registeredCustomer.CheckoutDate = Common.CurrentTimestamp();
            float days = (float)Math.Ceiling((registeredCustomer.CheckoutDate - registeredCustomer.EntryDate) / 86400);
            float price;
            if (currentRoom.CustomPrice - 1e-3 < 0) {
                DataHelper<RoomType> dhRt = new DataHelper<RoomType>(ref conn);
                RoomType rt = dhRt.SelectOne(String.Format("id = {0}", currentRoom.Type));
                if (rt == null) {
                    return NotFound(new {error = "CustomerCheckout", 
                        message = "Invalid room type."});
                }
                price = rt.TypicalPrice;
            } else {
                price = currentRoom.CustomPrice;
            }

            Consumption cons = new Consumption();
            cons.Customer = customer.Id;
            cons.Item = 0;
            cons.Comment = String.Format("Room {0} for {1} day(s)", currentRoom.Name, (int)days);
            cons.Count = 1;
            cons.Price = price * days;
            cons.Paid = false;
            DataHelper<Consumption> dhCons = new DataHelper<Consumption>(ref conn);
            cons = dhCons.Insert(cons);

            customer.Status = CustomerStatus.CheckedOut;
            dhCustomer.Update(customer);
            int remainingCustomers = dhCustomer.Count(String.Format("room_id = {0} AND status != 2", id));
            if (remainingCustomers == 0) {
                currentRoom.Status = RoomStatus.Vacant;
                dhRoom.Update(currentRoom);
            }

            return new ObjectResult(customer);
        }

        [HttpPutAttribute]
        public IActionResult AddRoom([FromBody] Room room) {
            if (!UserHelper.IsAdmin(HttpContext)) {
                return Forbid();
            }
            if(UserHelper.IsAdmin(HttpContext)) {
                Room res = dhRoom.Insert(room);
                return new ObjectResult(res);
            }

            return Forbid();
        }

        [HttpPostAttribute("{id}")]
        public IActionResult UpdateRoom(int id, [FromBodyAttribute] Room room) {
            if (!UserHelper.IsAdmin(HttpContext)) {
                return Forbid();
            }
            room.Id = id;
            int res = dhRoom.Update(room);
            return new ObjectResult(new {result = res});
        }

        [HttpDeleteAttribute("{id}")]
        public IActionResult DeleteRoom(int id) {
            if (!UserHelper.IsAdmin(HttpContext)) {
                return Forbid();
            }
            Room r = new Room();
            r.Id = id;
            int res = dhRoom.Delete(r);
            return new ObjectResult(new {result = res});
        }


    }
}