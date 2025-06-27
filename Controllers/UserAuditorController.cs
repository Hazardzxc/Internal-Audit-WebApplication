using Microsoft.AspNetCore.Mvc;
using STD.Models.DB;
using STD.Models.Form;

namespace STD.Controllers
{
    public class UserAuditorController : Controller
    {
        [HttpGet]
        public IActionResult GetUserAuditor()
        {
            List<UserAuditor> _ListUserAuditor = new List<UserAuditor>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _ListUserAuditor = DB.UserAuditors.ToList();
            }

            if (_ListUserAuditor.Count > 0)
            {
                _ListUserAuditor.ForEach(f => f.Password = "");
            }

            return new JsonResult(new { status = true, data = _ListUserAuditor });
        }

        [HttpGet]
        public IActionResult GetAuditorForm(Guid AuditorID)
        {
            UserAuditor? Model = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                Model = DB.UserAuditors.Where(w => w.AuditorId.Equals(AuditorID)).FirstOrDefault();
            }

            return View("~/Views/Settings/UserAuditor/UserAuditorForm.cshtml", Model);
        }

        [HttpPost]
        public IActionResult SaveUserAuditor(UserAuditorForm Model)
        {
            if (string.IsNullOrEmpty(Model.AuditorCode)) return Json(new { status = false, message = "AuditorCode is required" });
            if (string.IsNullOrEmpty(Model.FirstName)) return Json(new { status = false, message = "FirstName is required" });
            if (string.IsNullOrEmpty(Model.LastName)) return Json(new { status = false, message = "LastName is required" });
            if (string.IsNullOrEmpty(Model.Email)) return Json(new { status = false, message = "Email is required" });
            if (Model.Role <= 0) return Json(new { status = false, message = "Role is required" });

            UserAuditor? _UserAuditor = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                if (!Model.AuditorID.HasValue)
                {
                    if (DB.UserAuditors.Any(a => a.AuditorCode.Equals(Model.AuditorCode.Trim()))) return Json(new { status = false, message = "AuditorCode already exists" });
                    _UserAuditor = new UserAuditor();
                    _UserAuditor.AuditorCode = Model.AuditorCode.Trim();
                    _UserAuditor.FirstName = Model.FirstName.Trim();
                    _UserAuditor.LastName = Model.LastName.Trim();
                    _UserAuditor.Email = Model.Email.Trim();
                    _UserAuditor.Role = Model.Role;
                    _UserAuditor.CreateDate = DateTime.Now;
                    DB.UserAuditors.Add(_UserAuditor);
                }
                else
                {
                    _UserAuditor = DB.UserAuditors.Where(w => w.AuditorId.Equals(Model.AuditorID)).FirstOrDefault();
                    if (_UserAuditor == null) return Json(new { status = false, message = "Auditor not found" });
                    if (DB.UserAuditors.Any(a => a.AuditorCode.Equals(Model.AuditorCode.Trim()) && !a.AuditorId.Equals(Model.AuditorID))) return Json(new { status = false, message = "AuditorCode already exists" });
                    _UserAuditor.AuditorCode = Model.AuditorCode.Trim();
                    _UserAuditor.FirstName = Model.FirstName.Trim();
                    _UserAuditor.LastName = Model.LastName.Trim();
                    _UserAuditor.Email = Model.Email.Trim();
                    _UserAuditor.Role = Model.Role;

                    DB.UserAuditors.Update(_UserAuditor);
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

            return Json(new { status = true, message = "Saved successfully" });
        }

        [HttpPost]
        public IActionResult DeleteUserAuditor(Guid AuditorID)
        {
            if (AuditorID == Guid.Empty) return Json(new { status = false, message = "AuditorID is required" });

            UserAuditor? _UserAuditor = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _UserAuditor = DB.UserAuditors.Where(w => w.AuditorId.Equals(AuditorID)).FirstOrDefault();
                if (_UserAuditor == null) return Json(new { status = false, message = "Auditor not found" });
                DB.UserAuditors.Remove(_UserAuditor);

                try
                {
                    DB.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                }
            }

            return Json(new { status = true, message = "Deleted successfully" });
        }
    }
}