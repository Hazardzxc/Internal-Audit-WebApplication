using Microsoft.AspNetCore.Mvc;
using STD.Models.DB;
using STD.Models.Form;

namespace STD.Controllers
{
    public class SetUpSegmentAuditController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/SetUpSegmentAudit/Index.cshtml");
        }

        [HttpGet]
        public IActionResult GetPageFormWorkingPaper(string? AuditManegementSegmentID = "", string? WorkingPaperID = "")
        {
            AuditManegementSegment? _Obj = null;
            if (!string.IsNullOrEmpty(AuditManegementSegmentID))
            {
                using (AuditManager_Connect DB = new AuditManager_Connect())
                {
                    _Obj = DB.AuditManegementSegments.Where(w => w.AuditManegementSegmentId.Equals(Guid.Parse(AuditManegementSegmentID!))).FirstOrDefault();
                }
            }

            ViewBag.WorkingPaperID = WorkingPaperID;
            return View("~/Views/SetUpSegmentAudit/AuditManegementSegment/WorkingPaperForm.cshtml", _Obj);
        }

        [HttpGet]
        public IActionResult GetPageViewWorkingPaper(string AuditManegementSegmentID, string WorkingPaperID)
        {
            if (string.IsNullOrEmpty(AuditManegementSegmentID) || string.IsNullOrEmpty(WorkingPaperID)) return View("~/Views/Home/Index.cshtml");
            AuditManegementSegment? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.AuditManegementSegments.Where(w => w.AuditManegementSegmentId.Equals(Guid.Parse(AuditManegementSegmentID!))).FirstOrDefault();
            }

            ViewBag.WorkingPaperID = WorkingPaperID;
            return View("~/Views/SetUpSegmentAudit/AuditManegementSegment/ViewWorkingPaper.cshtml", _Obj);
        }

        [HttpGet]
        public IActionResult GetPageForm(int FacultyID, int Type = 1, string? AuditManegementSegmentID = null)
        {
            ViewBag.FacultyID = FacultyID;
            AuditManegementSegment? _Obj = null;
            if (!string.IsNullOrEmpty(AuditManegementSegmentID))
            {
                using (AuditManager_Connect DB = new AuditManager_Connect())
                {
                    _Obj = DB.AuditManegementSegments.Where(w => w.AuditManegementSegmentId.ToString().Equals(AuditManegementSegmentID)).FirstOrDefault();
                }
            }

            return View($"~/Views/SetUpSegmentAudit/AuditManegementSegment/AuditManegementSegment{(Type == 1 ? "Add" : "Edit")}Form.cshtml", _Obj);
        }

        [HttpGet]
        public IActionResult GetPageSetupSegmentDetail(string AuditManegementSegmentID, int AuditIssueMainID)
        {
            ViewBag.AuditIssueMainID = AuditIssueMainID;
            AuditManegementSegment? _Obj = null;
            if (!string.IsNullOrEmpty(AuditManegementSegmentID))
            {
                using (AuditManager_Connect DB = new AuditManager_Connect())
                {
                    _Obj = DB.AuditManegementSegments.Where(w => w.AuditManegementSegmentId.ToString().Equals(AuditManegementSegmentID)).FirstOrDefault();
                }
            }

            return View($"~/Views/SetUpSegmentAudit/AuditManegementSegment/SegmentDetail.cshtml", _Obj);
        }

        #region AuditManegementSegment
        [HttpGet]
        public IActionResult AuditManegementSegment(int FacultyID = 0)
        {
            Faculty? _Faculty = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Faculty = DB.Faculties.Where(w => w.FacultyId.Equals(FacultyID)).FirstOrDefault();
            }

            return View("~/Views/SetUpSegmentAudit/AuditManegementSegment/Index.cshtml", _Faculty);
        }

        [HttpGet]
        public IActionResult GetAuditManegementSegment(int FacultyID = 0)
        {
            List<object>? _List = new List<object>();
            List<AuditManegementSegment> _ListAuditManegementSegment = new List<AuditManegementSegment>();
            DateOnly _CurrentDate = DateOnly.FromDateTime(DateTime.Now);
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _ListAuditManegementSegment = DB.AuditManegementSegments.Where(w => w.FacultyId.Equals(FacultyID)).OrderByDescending(o => o.CreateDate).ToList();
                if (_ListAuditManegementSegment != null && _ListAuditManegementSegment.Count > 0)
                {
                    foreach (AuditManegementSegment item in _ListAuditManegementSegment)
                    {
                        string _StatusName = "ยังไม่ได้ตรวจสอบ";
                        string _Color = "secondary";
                        AuditIssueMain? _AuditIssueMain = DB.AuditIssueMains.Where(w => w.AuditIssueMainId.Equals(item.AuditIssueMainId)).FirstOrDefault();
                        Faculty? _Faculty = DB.Faculties.Where(w => w.FacultyId.Equals(item.FacultyId)).FirstOrDefault();
                        WorkingPaper? _WorkingPaper = DB.WorkingPapers.Where(w => w.AuditManegementSegmentId.Equals(item.AuditManegementSegmentId)).FirstOrDefault();
                        if (item.StartDate.HasValue && item.EndDate.HasValue)
                        {
                            if (_CurrentDate >= item.StartDate.Value && _CurrentDate <= item.EndDate.Value.AddDays(1))
                            {
                                _StatusName = "กําลังตรวจสอบ";
                                _Color = "warning";
                            }
                            else if (_CurrentDate > item.EndDate.Value.AddDays(1))
                            {
                                _StatusName = "เสร็จสิ้น";
                                _Color = "success";
                            }
                        }

                        _List.Add(new
                        {
                            AuditManegementSegmentID = item.AuditManegementSegmentId,
                            AuditIssueMainName = _AuditIssueMain != null ? _AuditIssueMain.AuditIssueMainName : "",
                            FacultyName = _Faculty != null ? _Faculty.FacultyName : "",
                            StartDate = item.StartDate.HasValue ? item.StartDate.Value.ToString("dd/MM/yyyy") : "",
                            EndDate = item.EndDate.HasValue ? item.EndDate.Value.ToString("dd/MM/yyyy") : "",
                            StatusName = _StatusName,
                            Color = _Color,
                            IsExpired = item.EndDate.HasValue && item.EndDate.Value.AddDays(1) < _CurrentDate,
                            WorkingPaperID = _WorkingPaper != null ? _WorkingPaper.WorkingPaperId.ToString() : "",
                        });
                    }
                }
            }

            return Json(new { status = true, data = _List });
        }

        [HttpPost]
        public JsonResult SaveAuditManegementSegment([FromBody] AuditManegementSegmentForm model)
        {
            if (model == null) return Json(new { status = false, message = "data required" });
            if (model.FacultyID <= 0) return Json(new { status = false, message = "FacultyID required" });
            if (model.Items == null || model.Items.Count <= 0) return Json(new { status = false, message = "Items required" });

            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                if (string.IsNullOrEmpty(model.AuditManegementSegmentID))
                {
                    int _row = 0;
                    foreach (STD.Models.Form.Item Item in model.Items)
                    {
                        _row++;
                        if (DB.AuditManegementSegments.Where(w => w.FacultyId.Equals(model.FacultyID) && w.AuditIssueMainId.Equals(Item.AuditIssueMainID)).Count() > 0)
                            return Json(new { status = false, message = $"faculty duplicate" });
                        //return Json(new { status = false, message = $"Row {_row} faculty duplicate" });
                        if (Item.ListAuditManegementSegmentSub.Count == 0)
                            return Json(new { status = false, message = $"check items required" });
                        //return Json(new { status = false, message = $"Row {_row} is check items required" });
                        AuditManegementSegment _AuditManegementSegment = new Models.DB.AuditManegementSegment();
                        _AuditManegementSegment.FacultyId = model.FacultyID;
                        _AuditManegementSegment.AuditIssueMainId = Item.AuditIssueMainID;
                        _AuditManegementSegment.CreateDate = DateTime.Now;

                        try
                        {
                            DB.AuditManegementSegments.Add(_AuditManegementSegment);
                            DB.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                        }

                        if (Item.ListAuditManegementSegmentSub.Count > 0)
                        {
                            foreach (AuditManegementSegmentSubForm ItemSub in Item.ListAuditManegementSegmentSub)
                            {
                                AuditIssueSub? _AuditIssueSub = DB.AuditIssueSubs.Where(w => w.AuditIssueSubId.Equals(ItemSub.AuditIssueSubID)).FirstOrDefault();
                                if (_AuditIssueSub == null) continue;
                                AuditManegementSegmentSub _AuditManegementSegmentSub = new AuditManegementSegmentSub();
                                _AuditManegementSegmentSub.AuditManegementSegmentId = _AuditManegementSegment.AuditManegementSegmentId;
                                _AuditManegementSegmentSub.AuditIssueSubId = _AuditIssueSub.AuditIssueSubId;
                                _AuditManegementSegmentSub.AuditIssueSubDetail = _AuditIssueSub.AuditIssueSubName;
                                _AuditManegementSegmentSub.AuditIssueSubType = _AuditIssueSub.AuditIssueSubType;
                                _AuditManegementSegmentSub.CreateDate = DateTime.Now;

                                try
                                {
                                    DB.AuditManegementSegmentSubs.Add(_AuditManegementSegmentSub);
                                    DB.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                                }

                                if (ItemSub.ListActivityVerify.Count == 0) continue;
                                foreach (Guid ActivityVerifyID in ItemSub.ListActivityVerify)
                                {
                                    AuditIssueActivityVerify? _AuditIssueActivityVerify = DB.AuditIssueActivityVerifies.Where(w => w.AuditIssueActivityId.Equals(ActivityVerifyID)).FirstOrDefault();
                                    if (_AuditIssueActivityVerify == null) continue;
                                    AuditManegementSegmentSubVerify _AuditManegementSegmentSubVerify = new AuditManegementSegmentSubVerify();
                                    _AuditManegementSegmentSubVerify.AuditManegementSegmentSubId = _AuditManegementSegmentSub.AuditManegementSegmentSubId;
                                    _AuditManegementSegmentSubVerify.AuditIssueActivityId = _AuditIssueActivityVerify.AuditIssueActivityId;
                                    _AuditManegementSegmentSubVerify.AuditIssueActivityDetail = _AuditIssueActivityVerify.ActivityDetail;
                                    _AuditManegementSegmentSubVerify.CreateDate = DateTime.Now;

                                    try
                                    {
                                        DB.AuditManegementSegmentSubVerifies.Add(_AuditManegementSegmentSubVerify);
                                        DB.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                                    }
                                }
                            }
                        }
                    }
                }

                return Json(new { status = true, message = "saved successfully" });
            }
        }

        [HttpPost]
        public JsonResult EditAuditManegementSegment([FromBody] AuditManegementSegmentEditForm model)
        {
            if (model == null) return Json(new { status = false, message = "data required" });
            if (model.AuditManegementSegmentID == Guid.Empty) return Json(new { status = false, message = "AuditManegementSegmentID required" });
            if (model.FacultyID <= 0) return Json(new { status = false, message = "FacultyID required" });
            if (model.Items == null || model.Items.Count <= 0) return Json(new { status = false, message = "Items required" });

            bool IsDeleteWorkingPaper = false;
            DateOnly now = DateOnly.FromDateTime(DateTime.Now);
            AuditManegementSegment? _AuditManegementSegment = null;
            WorkingPaper? _WorkingPaper = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _AuditManegementSegment = DB.AuditManegementSegments.Where(w => w.AuditManegementSegmentId.Equals(model.AuditManegementSegmentID)).FirstOrDefault();
                if (_AuditManegementSegment == null) return Json(new { status = false, message = "AuditManegementSegment empty" });

                _WorkingPaper = DB.WorkingPapers.Where(w => w.WorkingPaperId.Equals(_AuditManegementSegment.AuditManegementSegmentId)).FirstOrDefault();
                if (DB.AuditManegementSegments.Where(w => w.AuditIssueMainId.Equals(model.AuditIssueMainID) && w.FacultyId.Equals(model.FacultyID) && !w.AuditManegementSegmentId.Equals(model.AuditManegementSegmentID)).Any()) return Json(new { status = false, message = "AuditIssueMain is exist" });

                if (!model.StartDate.HasValue && model.EndDate.HasValue) return Json(new { status = false, message = "StartDate required" });
                if (model.StartDate.HasValue && !model.EndDate.HasValue) return Json(new { status = false, message = "EndDate required" });

                if (model.StartDate.HasValue && model.EndDate.HasValue && model.StartDate.Value > model.EndDate.Value) return Json(new { status = false, message = "StartDate must be less than EndDate" });

                if (model.StartDate.HasValue)
                {
                    if (model.StartDate.Value < now) return Json(new { status = false, message = "StartDate must be greater than now" });
                    _AuditManegementSegment.StartDate = model.StartDate.Value;
                }

                if (model.EndDate.HasValue)
                {
                    if (model.EndDate.Value < now) return Json(new { status = false, message = "EndDate must be greater than now" });
                    _AuditManegementSegment.EndDate = model.EndDate.Value;
                }

                _AuditManegementSegment.FacultyId = model.FacultyID;
                _AuditManegementSegment.AuditIssueMainId = model.AuditIssueMainID;

                try
                {
                    DB.AuditManegementSegments.Update(_AuditManegementSegment);
                    DB.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.Message });
                }

                List<AuditManegementSegmentSub> _ListAuditManegementSegmentSubs = DB.AuditManegementSegmentSubs.Where(w => w.AuditManegementSegmentId.Equals(model.AuditManegementSegmentID)).ToList();
                if (_ListAuditManegementSegmentSubs != null && _ListAuditManegementSegmentSubs.Count > 0)
                {
                    _ListAuditManegementSegmentSubs.ForEach(f =>
                    {
                        List<AuditManegementSegmentSubVerify> _ListAuditManegementSegmentSubVerifies = DB.AuditManegementSegmentSubVerifies.Where(w => w.AuditManegementSegmentSubId.Equals(f.AuditManegementSegmentSubId)).ToList();
                        if (_ListAuditManegementSegmentSubVerifies != null && _ListAuditManegementSegmentSubVerifies.Count > 0)
                        {
                            DB.AuditManegementSegmentSubVerifies.RemoveRange(_ListAuditManegementSegmentSubVerifies);
                            DB.SaveChanges();
                        }
                    });
                    DB.AuditManegementSegmentSubs.RemoveRange(_ListAuditManegementSegmentSubs);
                    DB.SaveChanges();
                }

                if (model.Items != null && model.Items.Count > 0)
                {
                    foreach (AuditManegementSegmentSubForm Item in model.Items)
                    {
                        AuditIssueSub? _AuditIssueSub = DB.AuditIssueSubs.Where(w => w.AuditIssueSubId.Equals(Item.AuditIssueSubID)).FirstOrDefault();
                        if (_AuditIssueSub == null) continue;
                        AuditManegementSegmentSub _AuditManegementSegmentSub = new AuditManegementSegmentSub();
                        _AuditManegementSegmentSub.AuditManegementSegmentId = _AuditManegementSegment.AuditManegementSegmentId;
                        _AuditManegementSegmentSub.AuditIssueSubId = _AuditIssueSub.AuditIssueSubId;
                        _AuditManegementSegmentSub.AuditIssueSubDetail = _AuditIssueSub.AuditIssueSubName;
                        _AuditManegementSegmentSub.AuditIssueSubType = _AuditIssueSub.AuditIssueSubType;
                        _AuditManegementSegmentSub.CreateDate = DateTime.Now;

                        try
                        {
                            DB.AuditManegementSegmentSubs.Add(_AuditManegementSegmentSub);
                            DB.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                        }

                        if (Item.ListActivityVerify.Count == 0) continue;
                        foreach (Guid ActivityVerifyID in Item.ListActivityVerify)
                        {
                            AuditIssueActivityVerify? _AuditIssueActivityVerify = DB.AuditIssueActivityVerifies.Where(w => w.AuditIssueActivityId.Equals(ActivityVerifyID)).FirstOrDefault();
                            if (_AuditIssueActivityVerify == null) continue;
                            AuditManegementSegmentSubVerify _AuditManegementSegmentSubVerify = new AuditManegementSegmentSubVerify();
                            _AuditManegementSegmentSubVerify.AuditManegementSegmentSubId = _AuditManegementSegmentSub.AuditManegementSegmentSubId;
                            _AuditManegementSegmentSubVerify.AuditIssueActivityId = _AuditIssueActivityVerify.AuditIssueActivityId;
                            _AuditManegementSegmentSubVerify.AuditIssueActivityDetail = _AuditIssueActivityVerify.ActivityDetail;
                            _AuditManegementSegmentSubVerify.CreateDate = DateTime.Now;

                            try
                            {
                                DB.AuditManegementSegmentSubVerifies.Add(_AuditManegementSegmentSubVerify);
                                DB.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                            }
                        }
                    }
                }
            }

            if (_WorkingPaper != null && !_WorkingPaper.AuditIssueMainId.Equals(_AuditManegementSegment.AuditIssueMainId))
                IsDeleteWorkingPaper = true;

            return Json(new { status = true, message = "saved successfully", data = new { IsDeleteWorkingPaper = IsDeleteWorkingPaper } });
        }

        [HttpPost]
        public IActionResult DeleteAuditManegementSegment(Guid AuditManegementSegmentID)
        {
            if (AuditManegementSegmentID == Guid.Empty) return Json(new { status = false, message = "AuditManegementSegmentID is required" });
            AuditManegementSegment? _AuditManegementSegment = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _AuditManegementSegment = DB.AuditManegementSegments.Where(w => w.AuditManegementSegmentId.Equals(AuditManegementSegmentID)).FirstOrDefault();
                if (_AuditManegementSegment == null) return Json(new { status = false, message = "AuditManegementSegment empty" });
                DB.AuditManegementSegments.Remove(_AuditManegementSegment);

                try
                {
                    DB.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.Message });
                }
            }

            List<AuditManegementSegmentSub>? _ListAuditManegementSegmentSub = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _ListAuditManegementSegmentSub = DB.AuditManegementSegmentSubs.Where(w => w.AuditManegementSegmentId.Equals(AuditManegementSegmentID)).ToList();
                if (_ListAuditManegementSegmentSub != null && _ListAuditManegementSegmentSub.Count > 0)
                {
                    foreach (AuditManegementSegmentSub item in _ListAuditManegementSegmentSub)
                    {
                        DB.AuditManegementSegmentSubs.Remove(item);
                        try
                        {
                            DB.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return Json(new { status = false, message = ex.Message });
                        }

                        List<AuditManegementSegmentSubVerify>? _ListAuditManegementSegmentSubVerify = DB.AuditManegementSegmentSubVerifies.Where(w => w.AuditManegementSegmentSubId.Equals(item.AuditManegementSegmentSubId)).ToList();
                        if (_ListAuditManegementSegmentSubVerify != null && _ListAuditManegementSegmentSubVerify.Count > 0)
                        {
                            _ListAuditManegementSegmentSubVerify.ForEach(w => DB.AuditManegementSegmentSubVerifies.Remove(w));

                            try
                            {
                                DB.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                return Json(new { status = false, message = ex.Message });
                            }
                        }
                    }
                }
            }

            return Json(new { status = true, message = "AuditManegementSegment deleted successfully" });
        }
        #endregion
    }
}