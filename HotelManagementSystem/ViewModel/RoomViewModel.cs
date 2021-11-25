using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.ViewModel
{
    public class RoomViewModel
    {
        public int RoomId { get; set; }
        [Display(Name ="Room No.")]
        [Required]
        public string RoomNumber { get; set; }
        [Display(Name = "Room Image")]
        [Required]
        public string RoomImage { get; set; }
        [Display(Name = "Room Price")]
        [Required]
        [Range(1000,10000)]
        public decimal RoomPrice { get; set; }
        [Display(Name = "Booking Status")]
        [Required]
        public int BookingStatusId { get; set; }
        [Display(Name = "Room Type")]
        [Required]
        public int RoomTypeId { get; set; }
        [Display(Name = "Room Capacity")]
        [Required]
        [Range(1,6)]
        public int RoomCapacity { get; set; }
        public HttpPostedFileBase Image { get; set; }
        [Display(Name = "Room Description")]
        [Required]
        public string RoomDescription { get; set; }
        public List<SelectListItem> ListOfBookingStatus { get; set; }
        public List<SelectListItem> ListOfRoomType { get; set; }

    }
}