using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelManagementSystem.Models;
using HotelManagementSystem.ViewModel;

namespace HotelManagementSystem.Controllers
{
    public class RoomController : Controller
    {
        private HotelBDEntities hotelBbEntities;
        public RoomController()
        {
            hotelBbEntities = new HotelBDEntities();
        }
        public ActionResult Index()
        {
            RoomViewModel roomViewModel = new RoomViewModel();
            roomViewModel.ListOfBookingStatus = (from obj in hotelBbEntities.BookingStatus
                                                    select new SelectListItem()
                                                    {
                                                        Text = obj.BookingStatus,
                                                        Value = obj.BookingStatusId.ToString()
                                                    }).ToList();
            roomViewModel.ListOfRoomType = (from obj in hotelBbEntities.RoomTypes
                                               select new SelectListItem()
                                               {
                                                   Text = obj.RoomTypeName,
                                                   Value = obj.RoomTypeId.ToString()
                                               }).ToList();
            return View(roomViewModel);
        }

        [HttpPost]
        public ActionResult AddRoom(RoomViewModel roomViewModel)
        {
            string message = string.Empty;
            string ImageUniqueName = string.Empty;
            string ActualImageName = string.Empty;
            if (roomViewModel.RoomId == 0)
            {
                ImageUniqueName = Guid.NewGuid().ToString();
                ActualImageName = ImageUniqueName + Path.GetExtension(roomViewModel.Image.FileName);
                roomViewModel.Image.SaveAs(Server.MapPath("~/RoomImages/" + ActualImageName));
                //objHotelBbEntities
                Room room = new Room()
                {
                    RoomNumber = roomViewModel.RoomNumber,
                    RoomDescription = roomViewModel.RoomDescription,
                    RoomPrice = roomViewModel.RoomPrice,
                    BookingStatusId = roomViewModel.BookingStatusId,
                    IsActive = true,
                    RoomImage = ActualImageName,
                    RoomCapacity = roomViewModel.RoomCapacity,
                    RoomTypeId = roomViewModel.RoomTypeId

                };
                hotelBbEntities.Rooms.Add(room);
                message = "Added";
            }
            else
            {
                Room room = hotelBbEntities.Rooms.Single(model => model.RoomId == roomViewModel.RoomId);
                if (roomViewModel.Image != null)
                {
                    ImageUniqueName = Guid.NewGuid().ToString();
                    ActualImageName = ImageUniqueName + Path.GetExtension(roomViewModel.Image.FileName);
                    roomViewModel.Image.SaveAs(Server.MapPath("~/RoomImages/" + ActualImageName));
                    room.RoomImage = ActualImageName;
                }
                room.RoomNumber = roomViewModel.RoomNumber;
                room.RoomDescription = roomViewModel.RoomDescription;
                room.RoomPrice = roomViewModel.RoomPrice;
                room.BookingStatusId = roomViewModel.BookingStatusId;
                room.IsActive = true;
                room.RoomCapacity = roomViewModel.RoomCapacity;
                room.RoomTypeId = roomViewModel.RoomTypeId;
                message = "Updated";
            }
            hotelBbEntities.SaveChanges();
            return Json(new {message = "Room Successfully " + message,success=  true },JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetAllRooms()
        {
            IEnumerable<RoomDetailsViewModel> ListOFRoomDetailsViewModels =
                (from room in hotelBbEntities.Rooms
                 join booking in hotelBbEntities.BookingStatus on room.BookingStatusId equals booking.BookingStatusId
                 join roomType in hotelBbEntities.RoomTypes on room.RoomTypeId equals roomType.RoomTypeId
                 where room.IsActive == true
                 select new RoomDetailsViewModel()
                 {
                     RoomNumber = room.RoomNumber,
                     RoomDescription = room.RoomDescription,
                     RoomCapacity = room.RoomCapacity,
                     RoomPrice = room.RoomPrice,
                     BookingStatus = booking.BookingStatus,
                     RoomType = roomType.RoomTypeName,
                     RoomImage = room.RoomImage,
                     RoomId = room.RoomId
                 }).ToList();
            return PartialView("_RoomDetailsPartial", ListOFRoomDetailsViewModels);
        }
        [HttpGet]
        public JsonResult EditRoomDetails (int roomId)
        {
            var result = hotelBbEntities.Rooms.Single(model => model.RoomId == roomId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult DeleteRoomDetails(int roomId)
        {
            Room room = hotelBbEntities.Rooms.Single(model => model.RoomId == roomId);
            room.IsActive = false;
            hotelBbEntities.SaveChanges();
            return Json(new {message = "Successfully Deleted", success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}