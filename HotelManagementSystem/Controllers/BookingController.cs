using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelManagementSystem.Models;
using HotelManagementSystem.ViewModel;

namespace HotelManagementSystem.Controllers
{
    public class BookingController : Controller
    {
        private HotelBDEntities hotelBDEntities;
        public BookingController()
        {
            hotelBDEntities = new HotelBDEntities();
        }
        public ActionResult Index()
        {
            BookingViewModel bookingViewModel = new BookingViewModel();
            bookingViewModel.ListOfRooms = (from room in hotelBDEntities.Rooms
                                            where room.BookingStatusId == 2
                                            select new SelectListItem()
                                            {
                                                Text = room.RoomNumber,
                                                Value = room.RoomId.ToString(),
                                            }).ToList();
            bookingViewModel.BookingFrom = DateTime.Now;
            bookingViewModel.BookingTo = DateTime.Now.AddDays(1);
            return View(bookingViewModel);
        }
        [HttpPost]
        public ActionResult AddBooking(BookingViewModel bookingViewModel)
        {
            int numberOfDays = Convert.ToInt32((bookingViewModel.BookingTo - bookingViewModel.BookingFrom).TotalDays);
            Room room = hotelBDEntities.Rooms.Single(model => model.RoomId == bookingViewModel.AssignRoomId);
            decimal RoomPrice = room.RoomPrice;
            decimal TotalPrice = RoomPrice * numberOfDays;
            RoomBooking roomBooking = new RoomBooking()
            {
                BookingFrom = bookingViewModel.BookingFrom,
                BookingTo = bookingViewModel.BookingTo,
                AssignRoomId = bookingViewModel.AssignRoomId,
                CustomerName = bookingViewModel.CustomerName,
                CustomerAddress = bookingViewModel.CustomerAddress,
                CustomerPhone = bookingViewModel.CustomerPhone,
                NoOfMembers = bookingViewModel.NoOfMembers,
                TotalAmount = TotalPrice
            };
            hotelBDEntities.RoomBookings.Add(roomBooking);
            hotelBDEntities.SaveChanges();
            room.BookingStatusId = 3;
            hotelBDEntities.SaveChanges();
            return Json(new {message = "Booking Done.", success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult GetAllBookingHistory()
        {
            List<RoomBookingViewModel> listOfBookingViewModel = new List<RoomBookingViewModel>();
            listOfBookingViewModel = (from hotelBooking in hotelBDEntities.RoomBookings
                                      join room in hotelBDEntities.Rooms on hotelBooking.AssignRoomId
                                      equals room.RoomId
                                      select new RoomBookingViewModel()
                                      {
                                          CustomerName = hotelBooking.CustomerName,
                                          CustomerAddress = hotelBooking.CustomerAddress,
                                          CustomerPhone = hotelBooking.CustomerPhone,
                                          BookingFrom = hotelBooking.BookingFrom,
                                          BookingTo = hotelBooking.BookingTo,
                                          NoOfMembers = hotelBooking.NoOfMembers,
                                          RoomNumber = room.RoomNumber,
                                          RoomPrice = room.RoomPrice,
                                          TotalAmount = hotelBooking.TotalAmount,
                                          BookingId = hotelBooking.BookingId,
                                          NoOfDays = System.Data.Entity.DbFunctions.DiffDays(hotelBooking.BookingFrom, hotelBooking.BookingTo).Value
                                      }).ToList();
            return PartialView("_BookingHistoryPartial", listOfBookingViewModel);
        }
    }
}