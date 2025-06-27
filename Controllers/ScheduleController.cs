using Microsoft.AspNetCore.Mvc;
using STD.Models.DB;
using STD.Models.Form;

namespace STD.Controllers
{
    public class ScheduleController : Controller
    {
        [HttpGet]
        public IActionResult Index(int AuditManagementID)
        {
            ViewBag.AuditManagementID = AuditManagementID;
            return View("~/Views/Schedule/Index.cshtml");
        }

        [HttpGet]
        public IActionResult GetScheduleForm(int ScheduleID)
        {
            Schedule? Model = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                Model = DB.Schedules.Where(w => w.ScheduleId.Equals(ScheduleID)).FirstOrDefault();
            }

            return View("~/Views/Schedule/ScheduleForm.cshtml", Model);
        }

        [HttpPost]
        public IActionResult GetSchedule(int AuditManagementID)
        {
            List<object> _List = new List<object>();
            List<Schedule> _ListSchedule = new List<Schedule>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _ListSchedule = DB.Schedules.Where(w => w.AuditManagementId.Equals(AuditManagementID)).ToList();
                if (_ListSchedule != null && _ListSchedule.Count > 0)
                {
                    foreach (Schedule Item in _ListSchedule)
                    {
                        _List.Add(new
                        {
                            ScheduleID = Item.ScheduleId,
                            ScheduleDetail = Item.ScheduleDetail,
                            ScheduleDoc = Item.ScheduleDoc,
                            ScheduleDate = Item.ScheduleDate.ToString("dd/MM/yyyy"),
                            ScheduleTime = Item.ScheduleTime.ToString("HH:mm"),
                        });
                    }
                }
            }

            return Json(new { status = true, data = _List });
        }

        [HttpPost]
        public IActionResult SaveSchedule(ScheduleForm Model)
        {
            if (Model == null) return Json(new { status = false, message = "Invalid Data" });
            if (string.IsNullOrEmpty(Model.ScheduleDetail)) return Json(new { status = false, message = "Schedule Detail is required" });
            if (string.IsNullOrEmpty(Model.ScheduleDoc)) return Json(new { status = false, message = "Schedule Document is required" });
            if (!Model.ScheduleDate.HasValue) return Json(new { status = false, message = "Schedule Date is required" });
            if (!Model.ScheduleTime.HasValue) return Json(new { status = false, message = "Schedule Time is required" });

            DateTime now = DateTime.Now;
            DateTime dateTime = Convert.ToDateTime($"{Model.ScheduleDate.Value.ToString("yyyy-MM-dd")} {Model.ScheduleTime.Value.ToString("HH:mm:ss")}");

            Schedule? _Schedule = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Schedule = DB.Schedules.Where(w => w.ScheduleId.Equals(Model.ScheduleID)).FirstOrDefault();
                if (_Schedule == null)
                {
                    if (dateTime < now) return Json(new { status = false, message = "Schedule Date and Time must be greater than current date and time" });
                    _Schedule = new Schedule();
                    _Schedule.AuditManagementId = Model.AuditManagementID;
                    _Schedule.ScheduleDoc = Model.ScheduleDoc;
                    _Schedule.ScheduleDetail = Model.ScheduleDetail;
                    _Schedule.ScheduleDate = Model.ScheduleDate.Value;
                    _Schedule.ScheduleTime = Model.ScheduleTime.Value;
                    _Schedule.CreateData = now;
                    DB.Schedules.Add(_Schedule);
                }
                else
                {
                    if (_Schedule.ScheduleDate != Model.ScheduleDate || _Schedule.ScheduleTime != Model.ScheduleTime)
                    {
                        if (dateTime < now) return Json(new { status = false, message = "Schedule Date and Time must be greater than current date and time" });
                    }
                    _Schedule.ScheduleDoc = Model.ScheduleDoc;
                    _Schedule.ScheduleDetail = Model.ScheduleDetail;
                    _Schedule.ScheduleDate = Model.ScheduleDate.Value;
                    _Schedule.ScheduleTime = Model.ScheduleTime.Value;
                    DB.Schedules.Update(_Schedule);
                }

                try
                {
                    DB.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.Message });
                }
            }

            return Json(new { status = true, message = "Schedule saved successfully" });
        }

        [HttpPost]
        public IActionResult DeleteSchedule(int ScheduleID)
        {
            if (ScheduleID <= 0) return Json(new { status = false, message = "ScheduleID is required" });
            Schedule? _Schedule = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Schedule = DB.Schedules.Where(w => w.ScheduleId.Equals(ScheduleID)).FirstOrDefault();
                if (_Schedule == null) return Json(new { status = false, message = "Schedule empty" });
                DB.Schedules.Remove(_Schedule);

                try
                {
                    DB.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.Message });
                }
            }

            return Json(new { status = true, message = "Schedule deleted successfully" });
        }
    }
}