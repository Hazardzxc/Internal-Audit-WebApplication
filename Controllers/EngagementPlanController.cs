using Microsoft.AspNetCore.Mvc;
using STD.Models.DB;
using STD.Models.Form;

namespace STD.Controllers
{
    public class EngagementPlanController : Controller
    {
        [HttpGet]
        public IActionResult Index(int AuditManagementID)
        {
            ViewBag.AuditManagementID = AuditManagementID;
            return View("~/Views/EngagementPlan/Index.cshtml");
        }

        [HttpGet]
        public IActionResult GetEngagementPlanForm(int EngagementPlanID)
        {
            EngagementPlan? Model = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                Model = DB.EngagementPlans.Where(w => w.EngagementPlanId.Equals(EngagementPlanID)).FirstOrDefault();
            }

            return View("~/Views/EngagementPlan/EngagementPlanForm.cshtml", Model);
        }

        [HttpPost]
        public IActionResult GetEngagementPlan(int AuditManagementID)
        {
            List<object> _List = new List<object>();
            List<EngagementPlan> _ListEngagementPlan = new List<EngagementPlan>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _ListEngagementPlan = DB.EngagementPlans.Where(w => w.AuditManagementId.Equals(AuditManagementID)).ToList();
                if (_ListEngagementPlan != null && _ListEngagementPlan.Count > 0)
                {
                    foreach (EngagementPlan Item in _ListEngagementPlan)
                    {
                        UserAuditor? _ObjAuditor = DB.UserAuditors.Where(w => w.AuditorId.Equals(Item.AuditorId)).FirstOrDefault();
                        _List.Add(new
                        {
                            EngagementPlanID = Item.EngagementPlanId,
                            AuditManagementID = Item.AuditManagementId,
                            AuditorID = Item.AuditorId,
                            AuditorName = _ObjAuditor != null && !string.IsNullOrEmpty(_ObjAuditor.FirstName) && !string.IsNullOrEmpty(_ObjAuditor.LastName) ? $"{_ObjAuditor.FirstName} {_ObjAuditor.LastName}" : "",
                            AuditProcedure = Item.AuditProcedure,
                            AuditTimeStart = Item.AuditTimeStart.ToString("HH:mm"),
                            AuditTimeEnd = Item.AuditTimeEnd.ToString("HH:mm"),
                            WPCode = Item.Wpcode,
                        });
                    }
                }
            }

            return Json(new { status = true, data = _List });
        }

        [HttpPost]
        public IActionResult SaveEngagementPlan([FromForm] EngagementPlanForm Model)
        {
            if (Model == null) return Json(new { status = false, message = "Invalid Data" });
            if (string.IsNullOrEmpty(Model.AuditProcedure)) return Json(new { status = false, message = "Audit Procedure is required" });
            if (string.IsNullOrEmpty(Model.WPCode)) return Json(new { status = false, message = "WPCode is required" });
            if (Model.AuditManagementID <= 0) return Json(new { status = false, message = "AuditManagement is required" });
            if (Model.AuditorID == Guid.Empty) return Json(new { status = false, message = "Auditor is required" });
            if (!Model.AuditTimeStart.HasValue) return Json(new { status = false, message = "Audit Time Start is required" });
            if (!Model.AuditTimeEnd.HasValue) return Json(new { status = false, message = "Audit Time End is required" });
            if (Model.AuditTimeStart.Value > Model.AuditTimeEnd.Value) return Json(new { status = false, message = "Audit Time Start should be less than Audit Time End" });

            EngagementPlan? _EngagementPlan = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                if (DB.UserAuditors.Where(w => w.AuditorId.Equals(Model.AuditorID)).FirstOrDefault() == null) return Json(new { status = false, message = "Auditor not found" });

                _EngagementPlan = DB.EngagementPlans.Where(w => w.EngagementPlanId.Equals(Model.EngagementPlanID)).FirstOrDefault();
                if (_EngagementPlan == null)
                {
                    _EngagementPlan = new EngagementPlan();
                    _EngagementPlan.AuditProcedure = Model.AuditProcedure;
                    _EngagementPlan.Wpcode = Model.WPCode;
                    _EngagementPlan.AuditorId = Model.AuditorID;
                    _EngagementPlan.AuditManagementId = Model.AuditManagementID;
                    _EngagementPlan.AuditTimeStart = Model.AuditTimeStart.Value;
                    _EngagementPlan.AuditTimeEnd = Model.AuditTimeEnd.Value;
                    _EngagementPlan.CreateDate = DateTime.Now;
                    DB.EngagementPlans.Add(_EngagementPlan);
                }
                else
                {
                    _EngagementPlan.AuditProcedure = Model.AuditProcedure;
                    _EngagementPlan.Wpcode = Model.WPCode;
                    _EngagementPlan.AuditorId = Model.AuditorID;
                    _EngagementPlan.AuditManagementId = Model.AuditManagementID;
                    _EngagementPlan.AuditTimeStart = Model.AuditTimeStart.Value;
                    _EngagementPlan.AuditTimeEnd = Model.AuditTimeEnd.Value;
                    DB.EngagementPlans.Update(_EngagementPlan);
                }

                try
                {
                    DB.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                }
            }

            return Json(new { status = true, message = "Saved Successfully" });
        }

        [HttpPost]
        public IActionResult DeleteEngagementPlan(int EngagementPlanID)
        {
            if (EngagementPlanID <= 0) return Json(new { status = false, message = "EngagementPlanID is required" });

            EngagementPlan? _EngagementPlan = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _EngagementPlan = DB.EngagementPlans.Where(w => w.EngagementPlanId.Equals(EngagementPlanID)).FirstOrDefault();
                if (_EngagementPlan == null) return Json(new { status = false, message = "EngagementPlan empty" });

                DB.EngagementPlans.Remove(_EngagementPlan);

                try
                {
                    DB.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                }
            }

            return Json(new { status = true, message = "Deleted Successfully" });
        }
    }
}