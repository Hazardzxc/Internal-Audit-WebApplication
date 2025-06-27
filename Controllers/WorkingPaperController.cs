using Microsoft.AspNetCore.Mvc;
using STD.Models.DB;
using STD.Models.Form;
using PuppeteerSharp;
using PuppeteerSharp.BrowserData;
using PuppeteerSharp.Media;
using Razor.Templating.Core;
using System.Web;

namespace STD.Controllers
{
    public class WorkingPaperController : Controller
    {
        #region WorkingPaper
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/WorkingPaper/Index.cshtml");
        }

        [HttpGet]
        public IActionResult GetDraftPage(Guid AuditManegementSegmentID)
        {
            AuditManegementSegment? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.AuditManegementSegments.Where(w => w.AuditManegementSegmentId.Equals(AuditManegementSegmentID)).FirstOrDefault();
            }

            return View("~/Views/WorkingPaper/Draft/Index.cshtml", _Obj);
        }

        [HttpGet]
        public IActionResult GetChooseAuditIssuePage()
        {
            return View("~/Views/WorkingPaper/ChooseAuditIssue.cshtml");
        }

        [HttpGet]
        public IActionResult GetWorkingPaperForm(Guid? WorkingPaperID, int FacultyID = 0, int AuditIssueMainID = 0)
        {
            WorkingPaper? Model = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                Model = DB.WorkingPapers.Where(w => w.WorkingPaperId.Equals(WorkingPaperID)).FirstOrDefault();
            }

            ViewBag.FacultyID = FacultyID;
            ViewBag.AuditIssueMainID = AuditIssueMainID;
            return View("~/Views/WorkingPaper/WorkingPaperForm.cshtml", Model);
        }

        [HttpGet]
        public IActionResult GetWorkingPaper()
        {
            List<WorkingPaper> _ListWorkingPaper = new List<WorkingPaper>();
            List<object> _List = new List<object>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _ListWorkingPaper = DB.WorkingPapers.OrderByDescending(o => o.CreateDate).ToList();
                if (_ListWorkingPaper != null && _ListWorkingPaper.Count > 0)
                {
                    foreach (WorkingPaper Item in _ListWorkingPaper)
                    {
                        Faculty? _Faculty = DB.Faculties.Where(w => w.FacultyId.Equals(Item.FacultyId)).FirstOrDefault();
                        AuditIssueMain? _AuditIssueMain = DB.AuditIssueMains.Where(w => w.AuditIssueMainId.Equals(Item.AuditIssueMainId)).FirstOrDefault();
                        UserAuditor? _AuditorBy = DB.UserAuditors.Where(w => w.AuditorId.Equals(Item.AuditorBy)).FirstOrDefault();
                        UserAuditor? _OperationsControllerAuditorBy = DB.UserAuditors.Where(w => w.AuditorId.Equals(Item.OperationsControllerAuditorBy)).FirstOrDefault();
                        UserAuditor? _ReviewerBy = DB.UserAuditors.Where(w => w.AuditorId.Equals(Item.ReviewerBy)).FirstOrDefault();
                        UserAuditor? _ProducerBy = DB.UserAuditors.Where(w => w.AuditorId.Equals(Item.ProducerBy)).FirstOrDefault();
                        UserAuditor? _ApproveBy = DB.UserAuditors.Where(w => w.AuditorId.Equals(Item.ApproveBy)).FirstOrDefault();

                        _List.Add(new
                        {
                            WorkingPaperID = Item.WorkingPaperId,
                            No = Item.No,
                            ReviewNo = Item.ReviewNo,
                            AuditIssueMainID = Item.AuditIssueMainId,
                            AuditIssueMainName = _AuditIssueMain != null ? _AuditIssueMain.AuditIssueMainName : "",
                            FacultyID = Item.FacultyId,
                            FacultyName = _Faculty != null ? _Faculty.FacultyName : "",
                            AuditorBy = Item.AuditorBy,
                            AuditorName = _AuditorBy != null ? $"{_AuditorBy.FirstName} {_AuditorBy.LastName}" : "",
                            OperationsControllerAuditorBy = Item.OperationsControllerAuditorBy,
                            OperationsControllerAuditorName = _OperationsControllerAuditorBy != null ? $"{_OperationsControllerAuditorBy.FirstName} {_OperationsControllerAuditorBy.LastName}" : "",
                            ReviewerBy = Item.ReviewerBy,
                            ReviewerName = _ReviewerBy != null ? $"{_ReviewerBy.FirstName} {_ReviewerBy.LastName}" : "",
                            ProducerBy = Item.ProducerBy,
                            ProducerName = _ProducerBy != null ? $"{_ProducerBy.FirstName} {_ProducerBy.LastName}" : "",
                            ApproveBy = Item.ApproveBy,
                            ApproveName = _ApproveBy != null ? $"{_ApproveBy.FirstName} {_ApproveBy.LastName}" : "",
                            StartDate = Item.StartDate.ToString("dd/MM/yyyy"),
                            EndDate = Item.EndDate.ToString("dd/MM/yyyy"),
                            ApproveDate = Item.ApproveDate.ToString("dd/MM/yyyy"),
                            AuditManegementSegmentID = Item.AuditManegementSegmentId,
                        });
                    }
                }
            }

            return Json(new { status = true, data = _List });
        }

        [HttpPost]
        public IActionResult SaveWorkingPaper([FromBody] WorkingPaperForm model)
        {
            if (model == null) return Json(new { status = false, message = "WorkingPaper required" });
            if (model.No == 0) return Json(new { status = false, message = "No required" });
            if (model.ReviewNo == 0) return Json(new { status = false, message = "ReviewNo required" });
            if (model.AuditIssueMainID == 0) return Json(new { status = false, message = "AuditIssueMainID required" });
            if (model.FacultyID == 0) return Json(new { status = false, message = "FacultyID required" });
            if (model.AuditorBy == Guid.Empty) return Json(new { status = false, message = "AuditorBy required" });
            if (model.OperationsControllerAuditorBy == Guid.Empty) return Json(new { status = false, message = "OperationsControllerAuditorBy required" });
            if (model.ReviewerBy == Guid.Empty) return Json(new { status = false, message = "ReviewerBy required" });
            if (model.ProducerBy == Guid.Empty) return Json(new { status = false, message = "ProducerBy required" });
            if (model.ApproveBy == Guid.Empty) return Json(new { status = false, message = "ApproveBy required" });
            if (model.StartDate == DateOnly.MinValue) { return Json(new { status = false, message = "StartDate required" }); }
            if (model.EndDate == DateOnly.MinValue) { return Json(new { status = false, message = "EndDate required" }); }
            if (model.ApproveDate == DateTime.MinValue) { return Json(new { status = false, message = "ApproveDate required" }); }

            WorkingPaper? _Obj = null;
            DateOnly now = DateOnly.FromDateTime(DateTime.Now);
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Now);
            DateOnly endDate = DateOnly.FromDateTime(DateTime.Now);
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.WorkingPapers.Where(w => w.WorkingPaperId.Equals(model.WorkingPaperID)).FirstOrDefault();
                if (_Obj == null)
                {
                    //if (DB.WorkingPapers.Where(w => w.AuditIssueMainId.Equals(model.AuditIssueMainID) && w.FacultyId.Equals(model.FacultyID)).Count() > 0) return Json(new { status = false, message = "Data duplicate" });
                    if (model.StartDate < now) return Json(new { status = false, message = "StartDate must be greater than now" });
                    if (model.EndDate < now) return Json(new { status = false, message = "EndDate must be greater than now" });
                    if (model.EndDate < model.StartDate) return Json(new { status = false, message = "EndDate must be greater than StartDate" });
                    if (DateOnly.FromDateTime(model.ApproveDate) >= now) return Json(new { status = false, message = "ApproveDate must be less than now" });

                    _Obj = new WorkingPaper();
                    _Obj.No = model.No;
                    _Obj.ReviewNo = model.ReviewNo;
                    _Obj.AuditIssueMainId = model.AuditIssueMainID;
                    _Obj.FacultyId = model.FacultyID;
                    _Obj.AuditorBy = model.AuditorBy;
                    _Obj.OperationsControllerAuditorBy = model.OperationsControllerAuditorBy;
                    _Obj.ReviewerBy = model.ReviewerBy;
                    _Obj.ProducerBy = model.ProducerBy;
                    _Obj.ApproveBy = model.ApproveBy;
                    _Obj.StartDate = model.StartDate;
                    _Obj.EndDate = model.EndDate;
                    _Obj.ApproveDate = model.ApproveDate;
                    _Obj.CreateDate = DateTime.Now;
                    DB.WorkingPapers.Add(_Obj);
                }
                else
                {
                    DateOnly _ModelApproveDate = DateOnly.FromDateTime(model.ApproveDate);
                    if (DateOnly.FromDateTime(_Obj.ApproveDate) != _ModelApproveDate && _ModelApproveDate >= now) return Json(new { status = false, message = "ApproveDate must be less than now" });
                    if (_Obj.StartDate != model.StartDate && model.StartDate < now) return Json(new { status = false, message = "StartDate must be greater than now" });
                    if (_Obj.EndDate != model.EndDate && model.EndDate < now) return Json(new { status = false, message = "EndDate must be greater than now" });
                    if (_Obj.EndDate != model.EndDate && model.EndDate < model.StartDate) return Json(new { status = false, message = "EndDate must be greater than StartDate" });

                    _Obj.No = model.No;
                    _Obj.ReviewNo = model.ReviewNo;
                    _Obj.AuditIssueMainId = model.AuditIssueMainID;
                    _Obj.FacultyId = model.FacultyID;
                    _Obj.AuditorBy = model.AuditorBy;
                    _Obj.OperationsControllerAuditorBy = model.OperationsControllerAuditorBy;
                    _Obj.ReviewerBy = model.ReviewerBy;
                    _Obj.ProducerBy = model.ProducerBy;
                    _Obj.ApproveBy = model.ApproveBy;
                    _Obj.StartDate = model.StartDate;
                    _Obj.EndDate = model.EndDate;
                    _Obj.ApproveDate = model.ApproveDate;
                    DB.WorkingPapers.Update(_Obj);
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

            return Json(new { status = true, message = "WorkingPaper saved successfully", data = new { FacultyID = _Obj.FacultyId, WorkingPaperID = _Obj.WorkingPaperId, AuditIssueMainID = _Obj.AuditIssueMainId } });
        }

        [HttpPost]
        public IActionResult SaveWorkingPaperForDraft([FromBody] WorkingPaperFormDraft model)
        {
            if (model == null) return Json(new { status = false, message = "WorkingPaper required" });
            if (model.No == 0) return Json(new { status = false, message = "No required" });
            if (model.ReviewNo == 0) return Json(new { status = false, message = "ReviewNo required" });
            if (model.AuditorBy == Guid.Empty) return Json(new { status = false, message = "AuditorBy required" });
            if (model.OperationsControllerAuditorBy == Guid.Empty) return Json(new { status = false, message = "OperationsControllerAuditorBy required" });
            if (model.ReviewerBy == Guid.Empty) return Json(new { status = false, message = "ReviewerBy required" });
            if (model.ProducerBy == Guid.Empty) return Json(new { status = false, message = "ProducerBy required" });
            if (model.ApproveBy == Guid.Empty) return Json(new { status = false, message = "ApproveBy required" });
            if (model.StartDate == DateOnly.MinValue) { return Json(new { status = false, message = "StartDate required" }); }
            if (model.EndDate == DateOnly.MinValue) { return Json(new { status = false, message = "EndDate required" }); }
            if (model.ApproveDate == DateTime.MinValue) { return Json(new { status = false, message = "ApproveDate required" }); }

            DateOnly now = DateOnly.FromDateTime(DateTime.Now);
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Now);
            DateOnly endDate = DateOnly.FromDateTime(DateTime.Now);
            WorkingPaper? _ObjWorkingPaper = null;
            AuditManegementSegment? _ObjSegment = null;
            List<AuditManegementSegmentSub> _ListAuditManegementSegmentSub = new List<AuditManegementSegmentSub>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _ObjSegment = DB.AuditManegementSegments.Where(w => w.AuditManegementSegmentId.Equals(model.AuditManegementSegmentID)).FirstOrDefault();
                if (_ObjSegment == null) return Json(new { status = false, message = "AuditManegementSegment empty" });
                _ListAuditManegementSegmentSub = DB.AuditManegementSegmentSubs.Where(w => w.AuditManegementSegmentId.Equals(_ObjSegment.AuditManegementSegmentId)).ToList();

                if (!model.WorkingPaperID.HasValue)
                {
                    if (model.StartDate < now) return Json(new { status = false, message = "StartDate must be greater than now" });
                    if (model.EndDate < now) return Json(new { status = false, message = "EndDate must be greater than now" });
                    if (model.EndDate < model.StartDate) return Json(new { status = false, message = "EndDate must be greater than StartDate" });
                    if (DateOnly.FromDateTime(model.ApproveDate) >= now) return Json(new { status = false, message = "ApproveDate must be less than now" });

                    _ObjWorkingPaper = new WorkingPaper();
                    _ObjWorkingPaper.WorkingPaperId = Guid.NewGuid();
                    _ObjWorkingPaper.No = model.No;
                    _ObjWorkingPaper.ReviewNo = model.ReviewNo;
                    _ObjWorkingPaper.AuditIssueMainId = _ObjSegment.AuditIssueMainId;
                    _ObjWorkingPaper.FacultyId = _ObjSegment.FacultyId;
                    _ObjWorkingPaper.AuditorBy = model.AuditorBy;
                    _ObjWorkingPaper.OperationsControllerAuditorBy = model.OperationsControllerAuditorBy;
                    _ObjWorkingPaper.ReviewerBy = model.ReviewerBy;
                    _ObjWorkingPaper.ProducerBy = model.ProducerBy;
                    _ObjWorkingPaper.ApproveBy = model.ApproveBy;
                    _ObjWorkingPaper.StartDate = model.StartDate;
                    _ObjWorkingPaper.EndDate = model.EndDate;
                    _ObjWorkingPaper.ApproveDate = model.ApproveDate;
                    _ObjWorkingPaper.CreateDate = DateTime.Now;
                    _ObjWorkingPaper.AuditManegementSegmentId = _ObjSegment.AuditManegementSegmentId;
                    DB.WorkingPapers.Add(_ObjWorkingPaper);
                }
                else
                {
                    _ObjWorkingPaper = DB.WorkingPapers.Where(w => w.WorkingPaperId.Equals(model.WorkingPaperID) && w.AuditManegementSegmentId.Equals(_ObjSegment.AuditManegementSegmentId)).FirstOrDefault();
                    if (_ObjWorkingPaper == null) return Json(new { status = false, message = "WorkingPaper not found" });
                    DateOnly _ModelApproveDate = DateOnly.FromDateTime(model.ApproveDate);
                    if (DateOnly.FromDateTime(_ObjWorkingPaper.ApproveDate) != _ModelApproveDate && _ModelApproveDate >= now) return Json(new { status = false, message = "ApproveDate must be less than now" });
                    if (_ObjWorkingPaper.StartDate != model.StartDate && model.StartDate < now) return Json(new { status = false, message = "StartDate must be greater than now" });
                    if (_ObjWorkingPaper.EndDate != model.EndDate && model.EndDate < now) return Json(new { status = false, message = "EndDate must be greater than now" });
                    if (_ObjWorkingPaper.EndDate != model.EndDate && model.EndDate < model.StartDate) return Json(new { status = false, message = "EndDate must be greater than StartDate" });
                    _ObjWorkingPaper.No = model.No;
                    _ObjWorkingPaper.ReviewNo = model.ReviewNo;
                    _ObjWorkingPaper.AuditIssueMainId = _ObjSegment.AuditIssueMainId;
                    _ObjWorkingPaper.FacultyId = _ObjSegment.FacultyId;
                    _ObjWorkingPaper.AuditorBy = model.AuditorBy;
                    _ObjWorkingPaper.OperationsControllerAuditorBy = model.OperationsControllerAuditorBy;
                    _ObjWorkingPaper.ReviewerBy = model.ReviewerBy;
                    _ObjWorkingPaper.ProducerBy = model.ProducerBy;
                    _ObjWorkingPaper.ApproveBy = model.ApproveBy;
                    _ObjWorkingPaper.StartDate = model.StartDate;
                    _ObjWorkingPaper.EndDate = model.EndDate;
                    _ObjWorkingPaper.ApproveDate = model.ApproveDate;
                    DB.WorkingPapers.Update(_ObjWorkingPaper);
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

            if (_ListAuditManegementSegmentSub != null && _ListAuditManegementSegmentSub.Count > 0)
            {
                using (AuditManager_Connect DB = new AuditManager_Connect())
                {
                    foreach (AuditManegementSegmentSub Item in _ListAuditManegementSegmentSub.Where(w => w.AuditIssueSubType.Equals(3)))
                    {
                        List<AuditManegementSegmentSubVerify> _AuditManegementSegmentSubVerify = DB.AuditManegementSegmentSubVerifies.Where(w => w.AuditManegementSegmentSubId.Equals(Item.AuditManegementSegmentSubId)).ToList();
                        WorkingPaperActivity? _ObjActivity = null;
                        if (model.WorkingPaperID.HasValue)
                            _ObjActivity = DB.WorkingPaperActivities.Where(w => w.WorkingPaperId.Equals(model.WorkingPaperID.Value) && w.AuditIssueSubId.Equals(Item.AuditIssueSubId)).FirstOrDefault();
                        if (_ObjActivity != null) continue;
                        _ObjActivity = new WorkingPaperActivity();
                        _ObjActivity.WorkingPaperActivityId = Guid.NewGuid();
                        _ObjActivity.AuditIssueSubId = Item.AuditIssueSubId;
                        _ObjActivity.WorkingPaperId = _ObjWorkingPaper.WorkingPaperId;
                        _ObjActivity.CreateDate = DateTime.Now;

                        try
                        {
                            DB.WorkingPaperActivities.Add(_ObjActivity);
                            DB.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                        }

                        if (_AuditManegementSegmentSubVerify == null || _AuditManegementSegmentSubVerify.Count == 0) continue;

                        foreach (AuditManegementSegmentSubVerify ItemVerify in _AuditManegementSegmentSubVerify)
                        {
                            WorkingPaperActivityVerify? _ObjActivityVerify = null;
                            if (_ObjActivity.WorkingPaperActivityId != Guid.Empty)
                                _ObjActivityVerify = DB.WorkingPaperActivityVerifies.Where(w => w.WorkingPaperActivityId.Equals(_ObjActivity.WorkingPaperActivityId) && w.AuditIssueActivityId.Equals(ItemVerify.AuditIssueActivityId)).FirstOrDefault();
                            if (_ObjActivityVerify != null) continue;
                            _ObjActivityVerify = new WorkingPaperActivityVerify();
                            _ObjActivityVerify.WorkingPaperActivityVerifyId = Guid.NewGuid();
                            _ObjActivityVerify.WorkingPaperActivityId = _ObjActivity.WorkingPaperActivityId;
                            _ObjActivityVerify.AuditIssueActivityId = ItemVerify.AuditIssueActivityId;
                            _ObjActivityVerify.CreateDate = DateTime.Now;

                            try
                            {
                                DB.WorkingPaperActivityVerifies.Add(_ObjActivityVerify);
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

            return Json(new { status = true, message = "WorkingPaper saved successfully", data = new { FacultyID = _ObjWorkingPaper.FacultyId, WorkingPaperID = _ObjWorkingPaper.WorkingPaperId, AuditIssueMainID = _ObjWorkingPaper.AuditIssueMainId, AuditManegementSegmentID = _ObjSegment.AuditManegementSegmentId } });
        }

        [HttpPost]
        public IActionResult DeleteWorkingPaper(Guid WorkingPaperID)
        {
            if (WorkingPaperID == Guid.Empty) return Json(new { status = false, message = "Invalid WorkingPaperID" });
            WorkingPaper? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.WorkingPapers.Where(w => w.WorkingPaperId.Equals(WorkingPaperID)).FirstOrDefault();
                if (_Obj == null) return Json(new { status = false, message = "WorkingPaper not found" });
                DB.WorkingPapers.Remove(_Obj);

                try
                {
                    DB.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                }
            }

            List<WorkingPaperActivity>? _ListWorkingPaperActivity = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _ListWorkingPaperActivity = DB.WorkingPaperActivities.Where(w => w.WorkingPaperId.Equals(WorkingPaperID)).ToList();
                if (_ListWorkingPaperActivity != null && _ListWorkingPaperActivity.Count > 0)
                {
                    _ListWorkingPaperActivity.ForEach(f =>
                    {
                        List<WorkingPaperActivityVerify>? _ListVerify = DB.WorkingPaperActivityVerifies.Where(w => w.WorkingPaperActivityId.Equals(f.WorkingPaperActivityId)).ToList();
                        if (_ListVerify != null && _ListVerify.Count > 0) DB.WorkingPaperActivityVerifies.RemoveRange(_ListVerify);

                        List<WorkingPaperActivitySub> _ListWorkingPaperActivitySub = DB.WorkingPaperActivitySubs.Where(w => w.WorkingPaperActivityId.Equals(f.WorkingPaperActivityId)).ToList();
                        if (_ListWorkingPaperActivitySub != null && _ListWorkingPaperActivitySub.Count > 0)
                        {
                            _ListWorkingPaperActivitySub.ForEach(fSub =>
                            {
                                List<WorkingPaperActivitySubVerify>? _ListjVerifySub = DB.WorkingPaperActivitySubVerifies.Where(w => w.WorkingPaperActivitySubId.Equals(fSub.WorkingPaperActivitySubId)).ToList();
                                if (_ListjVerifySub != null && _ListjVerifySub.Count > 0) DB.WorkingPaperActivitySubVerifies.RemoveRange(_ListjVerifySub);
                            });

                            DB.WorkingPaperActivitySubs.RemoveRange(_ListWorkingPaperActivitySub);
                        }
                    });

                    DB.WorkingPaperActivities.RemoveRange(_ListWorkingPaperActivity);

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

            return Json(new { status = true, message = "WorkingPaper deleted successfully" });
        }

        [HttpGet]
        public IActionResult ViewsWorkingPaper(Guid WorkingPaperID)
        {
            WorkingPaper? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.WorkingPapers.Where(w => w.WorkingPaperId.Equals(WorkingPaperID)).FirstOrDefault();
            }

            ViewBag.Logo = $"{Request.Scheme}://{Request.Host}/AdminLTE/dist/img/AdminLTELogo.png";

            return View("~/Views/WorkingPaper/Views.cshtml", _Obj);
        }

        [HttpGet]
        public async Task<IActionResult> ViewsWorkingPaperPDF(Guid WorkingPaperID)
        {
            WorkingPaper? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.WorkingPapers.Where(w => w.WorkingPaperId.Equals(WorkingPaperID)).FirstOrDefault();
            }

            if (_Obj == null) return NotFound(new { status = false, code = 404, message = "File not found" });

            Dictionary<string, object> _ViewBag = new Dictionary<string, object>
            {
                { "Logo", ViewBag.Logo = $"{Request.Scheme}://{Request.Host}/AdminLTE/dist/img/AdminLTELogo.png" },
            };

            Stream? _PDF = null;
            string _HtmlCS = await RenderHtml("~/Views/WorkingPaper/Views.cshtml", _Obj, _ViewBag);
            if (string.IsNullOrEmpty(_HtmlCS)) return NotFound(new { status = false, code = 404, message = "File not found" });

            try
            {
                BrowserFetcher _BrowserFetcher = await InitializeBrowser();
                await using var _Browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    ExecutablePath = _BrowserFetcher.GetExecutablePath(Chrome.DefaultBuildId),
                    Args = new[]
                    {
                         "--no-sandbox",
                         //"--disable-setuid-sandbox",
                         //"--disable-dev-shm-usage",
                    }
                });
                await using var _Page = await _Browser.NewPageAsync();
                await _Page.EmulateMediaTypeAsync(PuppeteerSharp.Media.MediaType.Print);
                await _Page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 1200,
                    Height = 800
                });
                PdfOptions _PdfOptions = new PdfOptions
                {
                    Format = PaperFormat.A4,
                    DisplayHeaderFooter = true,
                    PrintBackground = true,
                    MarginOptions = new MarginOptions { Top = "1cm", Bottom = "1cm", Left = "1cm", Right = "1cm" },
                    Scale = decimal.Parse("1.5"),
                    //FooterTemplate = $"<div style='font-size:16px!important; color:#808080; padding-left:20px; padding-right:20px; font-weight:500;'>Copyright © {DateTime.Now.Year}</div>"
                };

                //await _Page.WaitForFunctionAsync("window.innerWidth < 100");
                await _Page.SetContentAsync(_HtmlCS);
                _PDF = await _Page.PdfStreamAsync(_PdfOptions);

                await _Browser.CloseAsync();
                Response.Headers.Append("Content-Disposition", string.Format("inline; filename*=UTF-8''{0}; filename={0}", HttpUtility.UrlEncode($"GeneratePDF_{Guid.NewGuid()}.pdf")));
                Response.Headers.Append("Content-Transfer-Encoding", "binary");
                Response.Headers.Append("Accept-Ranges", "bytes");
                return File(_PDF, "application/pdf");
            }
            catch (Exception ex)
            {
                return NotFound(new { status = false, code = 400, message = ex?.InnerException?.Message ?? ex?.Message });
            }
        }
        #endregion

        #region WorkingPaperActivity
        [HttpGet]
        public IActionResult GetWorkingPaperActivityStartVerifyPage(Guid WorkingPaperActivityID)
        {
            WorkingPaperActivity? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.WorkingPaperActivities.Where(w => w.WorkingPaperActivityId.Equals(WorkingPaperActivityID)).FirstOrDefault();
            }

            return View("~/Views/WorkingPaper/Activity/ActivityStartVerify.cshtml", _Obj);
        }

        [HttpGet]
        public IActionResult GetWorkingPaperActivityDetailPage(Guid WorkingPaperActivityID)
        {
            WorkingPaperActivity? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.WorkingPaperActivities.Where(w => w.WorkingPaperActivityId.Equals(WorkingPaperActivityID)).FirstOrDefault();
            }

            return View("~/Views/WorkingPaper/Activity/ActivityDetail.cshtml", _Obj);
        }

        [HttpPost]
        public IActionResult SaveWorkingPaperActivityStartVerify(Guid WorkingPaperActivityID, int Issue, string Notes, string InspectionResults)
        {
            if (WorkingPaperActivityID == Guid.Empty) return Json(new { status = false, message = "Invalid WorkingPaperActivityID" });
            if (string.IsNullOrEmpty(InspectionResults)) return Json(new { status = false, message = "InspectionResults required" });
            if (string.IsNullOrEmpty(Notes)) return Json(new { status = false, message = "Note required" });

            WorkingPaperActivity? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.WorkingPaperActivities.Where(w => w.WorkingPaperActivityId.Equals(WorkingPaperActivityID)).FirstOrDefault();
                if (_Obj == null) return Json(new { status = false, message = "WorkingPaperActivity not found" });

                _Obj.Issue = Issue;
                _Obj.Notes = Notes;
                _Obj.InspectionResults = InspectionResults;
                DB.WorkingPaperActivities.Update(_Obj);

                try
                {
                    DB.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                }
            }

            return Json(new { status = true, message = "WorkingPaperActivity saved successfully" });
        }

        [HttpGet]
        public IActionResult GetWorkingPaperActivityTable(Guid WorkingPaperID)
        {
            WorkingPaper? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.WorkingPapers.Where(w => w.WorkingPaperId.Equals(WorkingPaperID)).FirstOrDefault();
            }

            return View("~/Views/WorkingPaper/Activity/ActivityTable.cshtml", _Obj);
        }

        [HttpGet]
        public IActionResult GetWorkingPaperActivityForm(Guid WorkingPaperID)
        {
            WorkingPaper? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.WorkingPapers.Where(w => w.WorkingPaperId.Equals(WorkingPaperID)).FirstOrDefault();
            }

            return View("~/Views/WorkingPaper/Activity/ActivityForm.cshtml", _Obj);
        }

        [HttpPost]
        public IActionResult SaveWorkingPaperActivity([FromBody] List<WorkingPaperActivityForm> model)
        {
            if (model == null || model.Count == 0) return Json(new { status = false, message = "Please WorkingPaperActivity" });

            WorkingPaperActivity? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                foreach (WorkingPaperActivityForm Item in model)
                {
                    _Obj = DB.WorkingPaperActivities.Where(w => w.WorkingPaperActivityId.Equals(Item.WorkingPaperActivityID)).FirstOrDefault();
                    if (_Obj == null)
                    {
                        _Obj = new WorkingPaperActivity();
                        _Obj.WorkingPaperId = Item.WorkingPaperID;
                        _Obj.AuditIssueSubId = Item.AuditIssueSubID;
                        _Obj.CreateDate = DateTime.Now;
                        DB.WorkingPaperActivities.Add(_Obj);
                    }
                    else
                    {
                        _Obj.WorkingPaperId = Item.WorkingPaperID;
                        _Obj.AuditIssueSubId = Item.AuditIssueSubID;
                        DB.WorkingPaperActivities.Update(_Obj);
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

            return Json(new { status = true, message = "WorkingPaperActivity saved successfully" });
        }

        [HttpPost]
        public IActionResult SaveUpdateWorkingPaperActivity(int AuditIssueSubID, Guid WorkingPaperActivityID, List<Guid> ListAuditIssueActivityVerify, Guid? WorkingPaperID)
        {
            if (AuditIssueSubID == 0) return Json(new { status = false, message = "Invalid AuditIssueSubID" });

            List<WorkingPaperActivityVerify> _ListWorkingPaperActivityVerify = new List<WorkingPaperActivityVerify>();
            WorkingPaperActivity? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.WorkingPaperActivities.Where(w => w.WorkingPaperActivityId.Equals(WorkingPaperActivityID)).FirstOrDefault();
                if (_Obj == null)
                {
                    if (!WorkingPaperID.HasValue) return Json(new { status = false, message = "Invalid WorkingPaperID" });
                    if (DB.WorkingPaperActivities.Where(w => w.AuditIssueSubId.Equals(AuditIssueSubID) && w.WorkingPaperId.Equals(WorkingPaperID.Value)).Any())
                        return Json(new { status = false, message = "WorkingPaperActivity already exist" });
                    _Obj = new WorkingPaperActivity();
                    _Obj.WorkingPaperActivityId = Guid.NewGuid();
                    _Obj.WorkingPaperId = WorkingPaperID.Value;
                    _Obj.AuditIssueSubId = AuditIssueSubID;
                    _Obj.CreateDate = DateTime.Now;
                    DB.WorkingPaperActivities.Add(_Obj);
                }
                else
                {
                    if (_Obj == null) return Json(new { status = false, message = "WorkingPaperActivity empty" });
                    //if (ListAuditIssueActivityVerify == null || ListAuditIssueActivityVerify.Count == 0) return Json(new { status = false, message = "Please select WorkingPaperActivity" });
                    if (WorkingPaperActivityID == Guid.Empty) return Json(new { status = false, message = "Invalid WorkingPaperActivityID" });
                    if (DB.WorkingPaperActivities.Where(w => w.WorkingPaperId.Equals(_Obj.WorkingPaperId) && w.AuditIssueSubId.Equals(AuditIssueSubID) && !w.WorkingPaperActivityId.Equals(WorkingPaperActivityID)).Any())
                        return Json(new { status = false, message = "WorkingPaperActivity already exist" });

                    _Obj.AuditIssueSubId = AuditIssueSubID;
                    DB.WorkingPaperActivities.Update(_Obj);
                }

                _ListWorkingPaperActivityVerify = DB.WorkingPaperActivityVerifies.Where(w => w.WorkingPaperActivityId.Equals(_Obj.WorkingPaperActivityId)).ToList();
                if (_ListWorkingPaperActivityVerify != null && _ListWorkingPaperActivityVerify.Count > 0)
                    DB.WorkingPaperActivityVerifies.RemoveRange(_ListWorkingPaperActivityVerify);

                if (ListAuditIssueActivityVerify != null && ListAuditIssueActivityVerify.Count > 0)
                {
                    foreach (Guid Item in ListAuditIssueActivityVerify)
                    {
                        WorkingPaperActivityVerify _ObjVerify = new WorkingPaperActivityVerify();
                        _ObjVerify.WorkingPaperActivityId = _Obj.WorkingPaperActivityId;
                        _ObjVerify.AuditIssueActivityId = Item;
                        _ObjVerify.CreateDate = DateTime.Now;
                        DB.WorkingPaperActivityVerifies.Add(_ObjVerify);
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

            return Json(new { status = true, message = "updated successfully", data = new { WorkingPaperActivityID = _Obj.WorkingPaperActivityId } });
        }

        [HttpPost]
        public IActionResult DeleteWorkingPaperActivity(Guid WorkingPaperActivityID)
        {
            if (WorkingPaperActivityID == Guid.Empty) return Json(new { status = false, message = "Invalid WorkingPaperActivityID" });
            WorkingPaperActivity? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.WorkingPaperActivities.Where(w => w.WorkingPaperActivityId.Equals(WorkingPaperActivityID)).FirstOrDefault();
                if (_Obj == null) return Json(new { status = false, message = "WorkingPaperActivity not found" });

                List<WorkingPaperActivityVerify> _ListWorkingPaperActivityVerify = DB.WorkingPaperActivityVerifies.Where(w => w.WorkingPaperActivityId.Equals(WorkingPaperActivityID)).ToList();
                if (_ListWorkingPaperActivityVerify != null && _ListWorkingPaperActivityVerify.Count > 0)
                    DB.WorkingPaperActivityVerifies.RemoveRange(_ListWorkingPaperActivityVerify);

                List<WorkingPaperActivitySub> _ListWorkingPaperActivitySub = DB.WorkingPaperActivitySubs.Where(w => w.WorkingPaperActivityId.Equals(WorkingPaperActivityID)).ToList();
                if (_ListWorkingPaperActivitySub != null && _ListWorkingPaperActivitySub.Count > 0)
                {
                    _ListWorkingPaperActivitySub.ForEach(f =>
                    {
                        List<WorkingPaperActivitySubVerify>? _ListVerifySub = DB.WorkingPaperActivitySubVerifies.Where(w => w.WorkingPaperActivitySubId.Equals(f.WorkingPaperActivitySubId)).ToList();
                        if (_ListVerifySub == null || _ListVerifySub.Count == 0) return;
                        DB.WorkingPaperActivitySubVerifies.RemoveRange(_ListVerifySub);
                    });

                    DB.WorkingPaperActivitySubs.RemoveRange(_ListWorkingPaperActivitySub);
                }

                DB.WorkingPaperActivities.Remove(_Obj);

                try
                {
                    DB.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                }
            }

            return Json(new { status = true, message = "deleted successfully" });
        }
        #endregion

        #region WorkingPaperActivityVerify
        [HttpGet]
        public IActionResult GetListWorkingPaperActivityVerify(Guid WorkingPaperActivityID)
        {
            List<WorkingPaperActivityVerify> _List = new List<WorkingPaperActivityVerify>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                if (WorkingPaperActivityID != Guid.Empty)
                    _List = DB.WorkingPaperActivityVerifies.Where(w => w.WorkingPaperActivityId.Equals(WorkingPaperActivityID)).OrderBy(o => o.CreateDate).ToList();
                else
                    _List = DB.WorkingPaperActivityVerifies.OrderBy(o => o.CreateDate).ToList();
            }

            return Json(new { status = true, data = _List });
        }
        #endregion

        #region WorkingPaperActivitySub
        [HttpGet]
        public IActionResult GetWorkingPaperActivityStartSubVerifyPage(Guid WorkingPaperActivitySubID)
        {
            WorkingPaperActivitySub? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.WorkingPaperActivitySubs.Where(w => w.WorkingPaperActivitySubId.Equals(WorkingPaperActivitySubID)).FirstOrDefault();
            }

            return View("~/Views/WorkingPaper/Activity/ActivitySub/ActivityStartSubVerify.cshtml", _Obj);
        }

        [HttpGet]
        public IActionResult GetWorkingPaperActivitySubForm(Guid WorkingPaperActivityID, Guid? WorkingPaperActivitySubID)
        {
            WorkingPaperActivity? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.WorkingPaperActivities.Where(w => w.WorkingPaperActivityId.Equals(WorkingPaperActivityID)).FirstOrDefault();
            }

            ViewBag.WorkingPaperActivitySubID = WorkingPaperActivitySubID.HasValue ? WorkingPaperActivitySubID : Guid.Empty;

            return View("~/Views/WorkingPaper/Activity/ActivitySub/ActivitySubForm.cshtml", _Obj);
        }

        [HttpGet]
        public IActionResult GetWorkingPaperActivitySubTable(Guid WorkingPaperActivityID)
        {
            WorkingPaperActivity? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.WorkingPaperActivities.Where(w => w.WorkingPaperActivityId.Equals(WorkingPaperActivityID)).FirstOrDefault();
            }

            return View("~/Views/WorkingPaper/Activity/ActivitySub/ActivitySubTable.cshtml", _Obj);
        }

        [HttpPost]
        public IActionResult SaveWorkingPaperActivityStartSubVerify(Guid WorkingPaperActivitySubID, int Issue, string Notes, string InspectionResults)
        {
            if (WorkingPaperActivitySubID == Guid.Empty) return Json(new { status = false, message = "Invalid WorkingPaperActivitySubID" });
            if (string.IsNullOrEmpty(InspectionResults)) return Json(new { status = false, message = "InspectionResults required" });
            if (string.IsNullOrEmpty(Notes)) return Json(new { status = false, message = "Note required" });

            WorkingPaperActivitySub? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.WorkingPaperActivitySubs.Where(w => w.WorkingPaperActivitySubId.Equals(WorkingPaperActivitySubID)).FirstOrDefault();
                if (_Obj == null) return Json(new { status = false, message = "WorkingPaperActivitySub empty" });

                _Obj.Issue = Issue;
                _Obj.Notes = Notes;
                _Obj.InspectionResults = InspectionResults;
                DB.WorkingPaperActivitySubs.Update(_Obj);

                try
                {
                    DB.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                }
            }

            return Json(new { status = true, message = "WorkingPaperActivitySub saved successfully" });
        }

        [HttpPost]
        public IActionResult SaveUpdateWorkingPaperActivitySub(int AuditIssueSubID, Guid WorkingPaperActivityID, List<Guid> ListAuditIssueActivityVerifySub, Guid? WorkingPaperActivitySubID)
        {
            if (AuditIssueSubID == 0) return Json(new { status = false, message = "Invalid AuditIssueSubID" });
            if (WorkingPaperActivityID == Guid.Empty) return Json(new { status = false, message = "Invalid WorkingPaperActivityID" });

            WorkingPaperActivitySub? _Obj = null;
            List<WorkingPaperActivitySubVerify> _ListWorkingPaperActivitySubVerify = new List<WorkingPaperActivitySubVerify>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                if (!WorkingPaperActivitySubID.HasValue)
                {
                    if (DB.WorkingPaperActivitySubs.Where(w => w.AuditIssueSubId.Equals(AuditIssueSubID) && w.WorkingPaperActivityId.Equals(WorkingPaperActivityID)).Any())
                        return Json(new { status = false, message = "WorkingPaperActivity already exist" });

                    _Obj = new WorkingPaperActivitySub();
                    _Obj.WorkingPaperActivitySubId = Guid.NewGuid();
                    _Obj.WorkingPaperActivityId = WorkingPaperActivityID;
                    _Obj.AuditIssueSubId = AuditIssueSubID;
                    _Obj.CreateDate = DateTime.Now;
                    DB.WorkingPaperActivitySubs.Add(_Obj);
                }
                else
                {
                    _Obj = DB.WorkingPaperActivitySubs.Where(w => w.WorkingPaperActivitySubId.Equals(WorkingPaperActivitySubID.Value)).FirstOrDefault();
                    if (_Obj == null) return Json(new { status = false, message = "WorkingPaperActivitySub empty" });
                    if (DB.WorkingPaperActivitySubs.Where(w => !w.WorkingPaperActivitySubId.Equals(_Obj.WorkingPaperActivitySubId) && w.AuditIssueSubId.Equals(AuditIssueSubID) && w.WorkingPaperActivityId.Equals(WorkingPaperActivityID)).Any())
                        return Json(new { status = false, message = "WorkingPaperActivity already exist" });

                    _Obj.AuditIssueSubId = AuditIssueSubID;
                    DB.WorkingPaperActivitySubs.Update(_Obj);
                }

                _ListWorkingPaperActivitySubVerify = DB.WorkingPaperActivitySubVerifies.Where(w => w.WorkingPaperActivitySubId.Equals(_Obj.WorkingPaperActivitySubId)).ToList();
                if (_ListWorkingPaperActivitySubVerify != null && _ListWorkingPaperActivitySubVerify.Count > 0)
                    DB.WorkingPaperActivitySubVerifies.RemoveRange(_ListWorkingPaperActivitySubVerify);

                if (ListAuditIssueActivityVerifySub != null && ListAuditIssueActivityVerifySub.Count > 0)
                {
                    foreach (Guid item in ListAuditIssueActivityVerifySub)
                    {
                        WorkingPaperActivitySubVerify _ObjVerify = new WorkingPaperActivitySubVerify();
                        _ObjVerify.WorkingPaperActivitySubId = _Obj.WorkingPaperActivitySubId;
                        _ObjVerify.AuditIssueActivityId = item;
                        _ObjVerify.CreateDate = DateTime.Now;
                        DB.WorkingPaperActivitySubVerifies.Add(_ObjVerify);
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

            return Json(new { status = true, message = "WorkingPaperActivity updated successfully", data = new { WorkingPaperActivitySubID = _Obj.WorkingPaperActivitySubId } });
        }

        [HttpPost]
        public IActionResult DeleteWorkingPaperActivitySub(Guid WorkingPaperActivitySubID)
        {
            if (WorkingPaperActivitySubID == Guid.Empty) return Json(new { status = false, message = "Invalid WorkingPaperActivitySubID" });
            WorkingPaperActivitySub? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.WorkingPaperActivitySubs.Where(w => w.WorkingPaperActivitySubId.Equals(WorkingPaperActivitySubID)).FirstOrDefault();
                if (_Obj == null) return Json(new { status = false, message = "WorkingPaperActivitySub empty" });

                List<WorkingPaperActivitySubVerify> _ListWorkingPaperActivitySubVerify = DB.WorkingPaperActivitySubVerifies.Where(w => w.WorkingPaperActivitySubId.Equals(WorkingPaperActivitySubID)).ToList();
                if (_ListWorkingPaperActivitySubVerify != null && _ListWorkingPaperActivitySubVerify.Count > 0)
                    DB.WorkingPaperActivitySubVerifies.RemoveRange(_ListWorkingPaperActivitySubVerify);

                DB.WorkingPaperActivitySubs.Remove(_Obj);

                try
                {
                    DB.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                }
            }

            return Json(new { status = true, message = "WorkingPaperActivitySub deleted successfully" });
        }
        #endregion

        #region WorkingPaperActivitySubVerify
        [HttpGet]
        public IActionResult GetListWorkingPaperActivitySubVerify(Guid WorkingPaperActivitySubID)
        {
            List<WorkingPaperActivitySubVerify> _List = new List<WorkingPaperActivitySubVerify>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                if (WorkingPaperActivitySubID != Guid.Empty)
                    _List = DB.WorkingPaperActivitySubVerifies.Where(w => w.WorkingPaperActivitySubId.Equals(WorkingPaperActivitySubID)).OrderBy(o => o.CreateDate).ToList();
                else
                    _List = DB.WorkingPaperActivitySubVerifies.OrderBy(o => o.CreateDate).ToList();
            }

            return Json(new { status = true, data = _List });
        }
        #endregion

        private async Task<string> RenderHtml(string path, object? model = null, Dictionary<string, object>? ViewBag = null)
        {
            return !string.IsNullOrEmpty(path) ? await RazorTemplateEngine.RenderAsync(path, model, ViewBag) : "";
        }

        private async Task<BrowserFetcher> InitializeBrowser()
        {
            BrowserFetcher _BrowserFetcher = new BrowserFetcher();
            await _BrowserFetcher.DownloadAsync(Chrome.DefaultBuildId);
            return _BrowserFetcher;
        }
    }
}