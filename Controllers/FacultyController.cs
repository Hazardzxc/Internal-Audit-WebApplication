using Microsoft.AspNetCore.Mvc;
using STD.Models.DB;

namespace STD.Controllers
{
    public class FacultyController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetFaculty()
        {
            List<Faculty> _ListFaculty = new List<Faculty>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _ListFaculty = DB.Faculties.ToList();
            }

            return Json(new { status = true, data = _ListFaculty });
        }

        [HttpGet]
        public IActionResult GetFacultyForm(int FacultyID)
        {
            Faculty? Model = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                Model = DB.Faculties.Where(w => w.FacultyId == FacultyID).FirstOrDefault();
            }

            return View("~/Views/Faculty/FacultyForm.cshtml", Model);
        }

        [HttpPost]
        public IActionResult SaveFaculty(string FacultyName, string FacultyCode, int FacultyID = 0)
        {
            if (string.IsNullOrEmpty(FacultyName)) return Json(new { status = false, message = "Please Enter Faculty Name" });
            if (string.IsNullOrEmpty(FacultyCode)) return Json(new { status = false, message = "Please Enter Faculty Code" });

            Faculty? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.Faculties.Where(w => w.FacultyId.Equals(FacultyID)).FirstOrDefault();
                if (_Obj == null)
                {
                    if (DB.Faculties.Where(w => w.FacultyCode.Equals(FacultyCode.Trim())).Any()) return Json(new { status = false, message = "Faculty Code already exists" });
                    _Obj = new Faculty();
                    _Obj.FacultyName = FacultyName;
                    _Obj.FacultyCode = FacultyCode.Trim();
                    _Obj.CreateDate = DateTime.Now;
                    DB.Faculties.Add(_Obj);
                }
                else
                {
                    if (DB.Faculties.Where(w => w.FacultyCode.Equals(FacultyCode.Trim()) && !w.FacultyId.Equals(FacultyID)).Any()) return Json(new { status = false, message = "Faculty Code already exists" });
                    _Obj.FacultyName = FacultyName;
                    _Obj.FacultyCode = FacultyCode.Trim();
                    DB.Faculties.Update(_Obj);
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
        public IActionResult DeleteFaculty(int FacultyID)
        {
            if (FacultyID <= 0) return Json(new { status = false, message = "Invalid Faculty ID" });

            Faculty? _Faculty = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Faculty = DB.Faculties.Where(w => w.FacultyId.Equals(FacultyID)).FirstOrDefault();
            }

            if (_Faculty == null) return Json(new { status = false, message = "Faculty empty" });

            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                DB.Faculties.Remove(_Faculty);

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