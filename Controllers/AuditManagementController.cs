using Microsoft.AspNetCore.Mvc;
using STD.Models.DB;
using STD.Models.Form;

namespace STD.Controllers
{
    public class AuditManagementController : Controller
    {
        [HttpGet]
        public IActionResult Index(int FacultyID = 0)
        {
            Faculty? _Faculty = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Faculty = DB.Faculties.Where(w => w.FacultyId.Equals(FacultyID)).FirstOrDefault();
            }

            return View("~/Views/AuditManagement/Index.cshtml", _Faculty);
        }

        [HttpGet]
        public IActionResult GetAuditManagementForm(int AuditManagementID)
        {
            AuditManagement? Model = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                Model = DB.AuditManagements.Where(w => w.AuditManagementId == AuditManagementID).FirstOrDefault();
            }

            return View("~/Views/AuditManagement/AuditManagementForm.cshtml", Model);
        }

        [HttpGet]
        public IActionResult GetAuditManagement(int FacultyID = 0)
        {
            List<object>? _List = new List<object>();
            List<AuditManagement> _ListAuditManagement = new List<AuditManagement>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                if (FacultyID <= 0)
                {
                    _ListAuditManagement = DB.AuditManagements.ToList();
                }
                else
                {
                    _ListAuditManagement = DB.AuditManagements.Where(w => w.FacultyId.Equals(FacultyID)).ToList();
                }

                if (_ListAuditManagement != null && _ListAuditManagement.Count > 0)
                {
                    foreach (AuditManagement item in _ListAuditManagement)
                    {
                        AuditIssueMain? _AuditIssueMain = DB.AuditIssueMains.Where(w => w.AuditIssueMainId == item.AuditIssueMainId).FirstOrDefault();
                        Faculty? _Faculty = DB.Faculties.Where(w => w.FacultyId == item.FacultyId).FirstOrDefault();
                        _List.Add(new
                        {
                            AuditManagementId = item.AuditManagementId,
                            AuditIssueMainName = _AuditIssueMain != null ? _AuditIssueMain.AuditIssueMainName : "",
                            FacultyName = _Faculty != null ? _Faculty.FacultyName : "",
                            AuditStartDate = item.AuditStartDate.ToString("dd/MM/yyyy"),
                            AuditEndDate = item.AuditEndDate.ToString("dd/MM/yyyy"),
                            StatusName = item.Status == 1 ? "รอดำเนินการ" : "เสร็จสิ้น",
                            Status = item.Status,
                            IsExpired = item.AuditEndDate.AddDays(1) < DateOnly.FromDateTime(DateTime.Now),
                            //IsExpired = true,
                        });
                    }
                }
            }

            return Json(new { status = true, data = _List });
        }

        [HttpGet]
        public IActionResult GetAuditManagementByID(int AuditManagementID)
        {
            if (AuditManagementID <= 0) return Json(new { status = false, message = "Please Select Audit Management" });

            AuditManagement? _AuditManagement = null;
            List<AuditManagementSub> _ListAuditManagementSub = new List<AuditManagementSub>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _AuditManagement = DB.AuditManagements.Where(w => w.AuditManagementId.Equals(AuditManagementID)).FirstOrDefault();
                if (_AuditManagement == null) return Json(new { status = false, message = "AuditManagement empty" });
                _ListAuditManagementSub = DB.AuditManagementSubs.Where(w => w.AuditManagementId.Equals(AuditManagementID)).ToList();
            }

            return Json(new
            {
                status = true,
                data = new
                {
                    AuditManagement = _AuditManagement,
                    ListAuditManagementSub = _ListAuditManagementSub,
                    IsExpired = _AuditManagement.AuditEndDate.AddDays(1) < DateOnly.FromDateTime(DateTime.Now),
                    //IsExpired = true,
                }
            });
        }

        [HttpPost]
        public IActionResult SaveAuditManagement([FromForm] AuditManagementForm Model)
        {
            if (Model == null) return Json(new { status = false, message = "Please Enter Audit Management Name" });

            if (Model.FacultyID <= 0) return Json(new { status = false, message = "Please Select Faculty" });

            if (Model.AuditIssueMainID <= 0) return Json(new { status = false, message = "Please Select Audit Issue Main" });

            DateOnly now = DateOnly.FromDateTime(DateTime.Now);
            if (!Model.AuditStartDate.HasValue)
            {
                return Json(new { status = false, message = "Please Enter Audit Start Date" });
            }

            if (!Model.AuditEndDate.HasValue)
            {
                return Json(new { status = false, message = "Please Enter Audit End Date" });
            }

            if (Model.AuditIssueSubID == null || Model.AuditIssueSubID.Count <= 0) return Json(new { status = false, message = "Please Select Audit Issue Sub" });

            if (Model.Status <= 0) return Json(new { status = false, message = "Please Select Status" });

            AuditManagement? _AuditManagement = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                if (DB.Faculties.Where(w => w.FacultyId == Model.FacultyID).Count() == 0)
                {
                    return Json(new { status = false, message = "Faculty empty" });
                }

                if (DB.AuditIssueMains.Where(w => w.AuditIssueMainId == Model.AuditIssueMainID).Count() == 0)
                {
                    return Json(new { status = false, message = "AuditIssueMain empty" });
                }

                if (DB.AuditIssueSubs.Where(w => w.AuditIssueMainId.Equals(Model.AuditIssueMainID)).Count() == 0)
                {
                    return Json(new { status = false, message = "AuditIssueSub empty" });
                }

                _AuditManagement = DB.AuditManagements.Where(w => w.AuditManagementId == Model.AuditManagementID).FirstOrDefault();
            }

            if (_AuditManagement != null)
            {
                if (_AuditManagement.Status == 2) return Json(new { status = false, message = "Audit Management Completed" });
                if (_AuditManagement.AuditEndDate.AddDays(1) < DateOnly.FromDateTime(DateTime.Now)) return Json(new { status = false, message = "Audit Management Expired" });
                if (Model.AuditStartDate != _AuditManagement.AuditStartDate && Model.AuditStartDate < now) return Json(new { status = false, message = "Audit Management Started" });
                if (Model.AuditEndDate != _AuditManagement.AuditEndDate && Model.AuditEndDate < now) return Json(new { status = false, message = "Audit Management Ended" });
            }
            else
            {
                if (Model.AuditStartDate.Value < now)
                {
                    return Json(new { status = false, message = "Please Enter Audit Start Date Less Than Today" });
                }

                if (Model.AuditEndDate.Value < now)
                {
                    return Json(new { status = false, message = "Please Enter Audit End Date Less Than Today" });
                }

                if (Model.AuditEndDate.Value < Model.AuditStartDate.Value)
                {
                    return Json(new { status = false, message = "Please Enter Audit End Date Greater Than Audit Start Date" });
                }
            }

            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                if (_AuditManagement == null)
                {
                    _AuditManagement = new AuditManagement();
                    _AuditManagement.FacultyId = Model.FacultyID;
                    _AuditManagement.AuditIssueMainId = Model.AuditIssueMainID;
                    _AuditManagement.AuditStartDate = Model.AuditStartDate.Value;
                    _AuditManagement.AuditEndDate = Model.AuditEndDate.Value;
                    _AuditManagement.Status = Model.Status;
                    _AuditManagement.CreateDate = DateTime.Now;
                    DB.AuditManagements.Add(_AuditManagement);
                }
                else
                {
                    _AuditManagement.AuditIssueMainId = Model.AuditIssueMainID;
                    _AuditManagement.AuditStartDate = Model.AuditStartDate.Value;
                    _AuditManagement.AuditEndDate = Model.AuditEndDate.Value;
                    _AuditManagement.Status = Model.Status;
                    DB.AuditManagements.Update(_AuditManagement);
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

            if (_AuditManagement != null)
            {
                using (AuditManager_Connect DB = new AuditManager_Connect())
                {
                    DB.AuditManagementSubs.RemoveRange(DB.AuditManagementSubs.Where(w => w.AuditManagementId.Equals(_AuditManagement.AuditManagementId)).ToList());

                    try
                    {
                        DB.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                    }
                }
            }

            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                foreach (int Item in Model.AuditIssueSubID)
                {
                    AuditManagementSub? _Obj = null;
                    _Obj = DB.AuditManagementSubs.Where(w => w.AuditManagementId.Equals(_AuditManagement!.AuditManagementId) && w.AuditIssueSubId.Equals(Item)).FirstOrDefault();
                    if (_Obj == null)
                    {
                        _Obj = new AuditManagementSub();
                        _Obj.AuditManagementId = _AuditManagement!.AuditManagementId;
                        _Obj.AuditManagementId = _AuditManagement.AuditManagementId;
                        _Obj.AuditIssueSubId = Item;
                        _Obj.CreateDate = DateTime.Now;
                        DB.AuditManagementSubs.Add(_Obj);
                    }
                    else
                    {
                        DB.AuditManagementSubs.Update(_Obj);
                    }
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
        public IActionResult DeleteAuditManagement(int AuditManagementID)
        {
            if (AuditManagementID <= 0) return Json(new { status = false, message = "AuditManagementID is null" });

            AuditManagement? _AuditManagement = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _AuditManagement = DB.AuditManagements.Where(w => w.AuditManagementId == AuditManagementID).FirstOrDefault();
                if (_AuditManagement == null) return Json(new { status = false, message = "AuditManagement empty" });

                DB.AuditManagements.Remove(_AuditManagement);

                try
                {
                    DB.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                }
            }

            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                List<AuditManagementSub> _ListAuditManagementSub = DB.AuditManagementSubs.Where(w => w.AuditManagementId == AuditManagementID).ToList();
                if (_ListAuditManagementSub != null && _ListAuditManagementSub.Count > 0)
                {
                    foreach (AuditManagementSub item in _ListAuditManagementSub)
                    {
                        DB.AuditManagementSubs.Remove(item);
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
            }

            return Json(new { status = true, message = "Deleted Successfully" });
        }

        [HttpGet]
        public IActionResult GetAuditManagementDetailView(int AuditManagementID)
        {
            AuditManagement? Model = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                Model = DB.AuditManagements.Where(w => w.AuditManagementId == AuditManagementID).FirstOrDefault();
            }

            return View("~/Views/AuditManagement/AuditManagementDetail.cshtml", Model);
        }
    }
}