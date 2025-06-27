using Microsoft.AspNetCore.Mvc;
using STD.Models.DB;
using STD.Models.Form;

namespace STD.Controllers
{
    public class AuditIssueMainController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/AuditIssueMain/Index.cshtml");
        }

        [HttpGet]
        public JsonResult GetAuditIssueMain()
        {
            List<AuditIssueMain> _ListAuditIssueMain = new List<AuditIssueMain>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _ListAuditIssueMain = DB.AuditIssueMains.ToList();
            }

            return Json(new { status = true, data = _ListAuditIssueMain });
        }

        [HttpGet]
        public IActionResult GetAuditIssueMainForm(int AuditIssueMainID)
        {
            AuditIssueMain? Model = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                Model = DB.AuditIssueMains.Where(w => w.AuditIssueMainId.Equals(AuditIssueMainID)).FirstOrDefault();
            }

            return View("~/Views/AuditIssueMain/AuditIssueMainForm.cshtml", Model);
        }

        [HttpPost]
        public JsonResult SaveAuditIssueMain([FromForm] AuditIssueMainForm Model)
        {
            if (string.IsNullOrEmpty(Model.AuditIssueMainName)) return Json(new { status = false, message = "Please Enter Audit Issue Main Name" });

            if (Model.ListAuditIssueSub == null || Model.ListAuditIssueSub.Count == 0) return Json(new { status = false, message = "Please Select Audit Issue Sub" });

            AuditIssueMain? _AuditIssueMain = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _AuditIssueMain = DB.AuditIssueMains.Where(w => w.AuditIssueMainId.Equals(Model.AuditIssueMainID)).FirstOrDefault();
                if (_AuditIssueMain == null)
                {
                    if (DB.AuditIssueMains.Where(w => w.AuditIssueMainName.Equals(Model.AuditIssueMainName.Trim())).Any())
                        return Json(new { status = false, message = $"AuditIssueMainName {Model.AuditIssueMainName} already exists" });

                    _AuditIssueMain = new AuditIssueMain();
                    _AuditIssueMain.AuditIssueMainName = Model.AuditIssueMainName;
                    _AuditIssueMain.CreateDate = DateTime.Now;
                    DB.AuditIssueMains.Add(_AuditIssueMain);
                }
                else
                {
                    if (DB.AuditIssueMains.Where(w => w.AuditIssueMainId != _AuditIssueMain.AuditIssueMainId && w.AuditIssueMainName.Equals(Model.AuditIssueMainName.Trim())).Any())
                        return Json(new { status = false, message = $"AuditIssueMainName {Model.AuditIssueMainName} already exists" });

                    _AuditIssueMain.AuditIssueMainName = Model.AuditIssueMainName;
                    DB.AuditIssueMains.Update(_AuditIssueMain);
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

            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                foreach (AuditIssueSubForm Item in Model.ListAuditIssueSub)
                {
                    AuditIssueSub? _Obj = null;
                    _Obj = DB.AuditIssueSubs.Where(w => w.AuditIssueSubId.Equals(Item.AuditIssueSubID)).FirstOrDefault();
                    if (_Obj == null)
                    {
                        _Obj = new AuditIssueSub();
                        _Obj.AuditIssueMainId = _AuditIssueMain.AuditIssueMainId;
                        _Obj.AuditIssueSubName = Item.AuditIssueSubName;
                        _Obj.AuditIssueSubType = Item.Ref;
                        _Obj.CreateDate = DateTime.Now;
                        DB.AuditIssueSubs.Add(_Obj);
                    }
                    else
                    {
                        _Obj.AuditIssueSubName = Item.AuditIssueSubName;
                        _Obj.AuditIssueSubType = Item.Ref;
                        DB.AuditIssueSubs.Update(_Obj);
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
        public JsonResult DeleteAuditIssueMain(int AuditIssueMainID)
        {
            if (AuditIssueMainID <= 0) return Json(new { status = false, message = "Invalid Audit Issue Main ID" });

            AuditIssueMain? _AuditIssueMain = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _AuditIssueMain = DB.AuditIssueMains.Where(w => w.AuditIssueMainId.Equals(AuditIssueMainID)).FirstOrDefault();
            }

            if (_AuditIssueMain == null) return Json(new { status = false, message = "Audit Issue Main empty" });

            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                DB.AuditIssueMains.Remove(_AuditIssueMain);

                try
                {
                    DB.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                }
            }

            List<AuditIssueSub>? _ListAuditIssueSub = null;
            List<AuditIssueActivityVerify>? _ListAuditIssueActivityVerify = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _ListAuditIssueSub = DB.AuditIssueSubs.Where(w => w.AuditIssueMainId.Equals(AuditIssueMainID)).ToList();
                if (_ListAuditIssueSub.Count > 0)
                {
                    foreach (AuditIssueSub Item in _ListAuditIssueSub)
                    {
                        _ListAuditIssueActivityVerify = DB.AuditIssueActivityVerifies.Where(w => w.AuditIssueSubId.Equals(Item.AuditIssueSubId)).ToList();
                        if (_ListAuditIssueActivityVerify.Count == 0) continue;
                        DB.AuditIssueActivityVerifies.RemoveRange(_ListAuditIssueActivityVerify);
                    }

                    DB.AuditIssueSubs.RemoveRange(_ListAuditIssueSub);
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

            return Json(new { status = true, message = "Deleted Successfully" });
        }

        #region AuditIssueSub
        [HttpGet]
        public JsonResult GetAuditIssueSub(int AuditIssueMainID)
        {
            List<AuditIssueSub> _ListAuditIssueSub = new List<AuditIssueSub>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _ListAuditIssueSub = DB.AuditIssueSubs.Where(w => w.AuditIssueMainId.Equals(AuditIssueMainID)).ToList();
            }

            return Json(new { status = true, data = _ListAuditIssueSub });
        }

        [HttpGet]
        public JsonResult GetAuditIssueSubDetail(int AuditIssueSubID)
        {
            AuditIssueSub? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.AuditIssueSubs.Where(w => w.AuditIssueSubId.Equals(AuditIssueSubID)).FirstOrDefault();
            }

            return Json(new { status = true, data = _Obj });
        }

        [HttpPost]
        public JsonResult DeleteAuditIssueSub(int AuditIssueSubID)
        {
            if (AuditIssueSubID <= 0) return Json(new { status = false, message = "Invalid Audit Issue Sub ID" });

            AuditIssueSub? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.AuditIssueSubs.Where(w => w.AuditIssueSubId.Equals(AuditIssueSubID)).FirstOrDefault();
            }

            if (_Obj == null) return Json(new { status = false, message = "Audit Issue Sub empty" });

            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                DB.AuditIssueSubs.Remove(_Obj);

                try
                {
                    DB.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.InnerException?.Message ?? ex.Message });
                }
            }

            List<AuditIssueActivityVerify>? _ListAuditIssueActivityVerify = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _ListAuditIssueActivityVerify = DB.AuditIssueActivityVerifies.Where(w => w.AuditIssueSubId.Equals(AuditIssueSubID)).ToList();
                if (_ListAuditIssueActivityVerify.Count > 0)
                {
                    DB.AuditIssueActivityVerifies.RemoveRange(_ListAuditIssueActivityVerify);
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
        public JsonResult GetAuditIssueSubByAuditIssueMain(int AuditIssueMainID)
        {
            List<object> _List = new List<object>();
            List<AuditIssueSub> _ListAuditIssueSub = new List<AuditIssueSub>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _ListAuditIssueSub = DB.AuditIssueSubs.Where(w => w.AuditIssueMainId.Equals(AuditIssueMainID)).OrderBy(w => w.AuditIssueSubName).ToList();
                if (_ListAuditIssueSub.Count > 0)
                {
                    foreach (AuditIssueSub Item in _ListAuditIssueSub)
                    {
                        List<AuditIssueActivityVerify> _ListAuditIssueActivityVerify = new List<AuditIssueActivityVerify>();
                        if (Item.AuditIssueSubType.Equals(3))
                            _ListAuditIssueActivityVerify = DB.AuditIssueActivityVerifies.Where(w => w.AuditIssueSubId.Equals(Item.AuditIssueSubId)).ToList();

                        _List.Add(new
                        {
                            AuditIssueSubID = Item.AuditIssueSubId,
                            AuditIssueSubName = Item.AuditIssueSubName,
                            AuditIssueSubType = Item.AuditIssueSubType,
                            ListAuditIssueActivityVerify = _ListAuditIssueActivityVerify,
                        });
                    }
                }
            }

            return Json(new { status = true, data = _List });
        }
        #endregion

        #region AuditIssueActivityVerify
        [HttpGet]
        public IActionResult GetAuditIssueActivityVerifyForm(int AuditIssueSubID)
        {
            AuditIssueSub? Model = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                Model = DB.AuditIssueSubs.Where(w => w.AuditIssueSubId.Equals(AuditIssueSubID)).FirstOrDefault();
            }

            return View("~/Views/AuditIssueMain/AuditIssueActivityVerifyForm.cshtml", Model);
        }

        [HttpPost]
        public JsonResult SaveAuditIssueActivityVerify([FromBody] List<AuditIssueActivityVerifyForm> ListModel)
        {
            if (ListModel == null || ListModel.Count == 0) return Json(new { status = false, message = "Please AuditIssueActivityVerify" });

            AuditIssueActivityVerify? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                foreach (AuditIssueActivityVerifyForm Item in ListModel)
                {
                    if (string.IsNullOrEmpty(Item.ActivityDetail)) return Json(new { status = false, message = "Please Enter Activity Detail" });

                    _Obj = DB.AuditIssueActivityVerifies.Where(w => w.AuditIssueActivityId.Equals(Item.AuditIssueActivityID)).FirstOrDefault();
                    if (_Obj == null)
                    {
                        _Obj = new AuditIssueActivityVerify();
                        _Obj.AuditIssueSubId = Item.AuditIssueSubID;
                        _Obj.ActivityDetail = Item.ActivityDetail;
                        _Obj.CreateDate = DateTime.Now;
                        DB.AuditIssueActivityVerifies.Add(_Obj);
                    }
                    else
                    {
                        _Obj.ActivityDetail = Item.ActivityDetail;
                        DB.AuditIssueActivityVerifies.Update(_Obj);
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
        }

        [HttpGet]
        public JsonResult GetAuditIssueActivityVerifyDetail(Guid AuditIssueActivityID)
        {
            if (AuditIssueActivityID == Guid.Empty) return Json(new { status = false, message = "Invalid AuditIssueActivityID" });

            AuditIssueActivityVerify? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.AuditIssueActivityVerifies.Where(w => w.AuditIssueActivityId.Equals(AuditIssueActivityID)).FirstOrDefault();
            }

            return Json(new { status = true, data = _Obj });
        }

        [HttpGet]
        public JsonResult GetListAuditIssueActivityVerify(int AuditIssueSubID = 0)
        {
            List<AuditIssueActivityVerify> _List = new List<AuditIssueActivityVerify>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                if (AuditIssueSubID > 0)
                    _List = DB.AuditIssueActivityVerifies.Where(w => w.AuditIssueSubId.Equals(AuditIssueSubID)).OrderBy(w => w.CreateDate).ToList();
                else
                    _List = DB.AuditIssueActivityVerifies.OrderBy(w => w.CreateDate).ToList();
            }

            return Json(new { status = true, data = _List });
        }

        [HttpPost]
        public JsonResult DeleteAuditIssueActivityVerify(Guid AuditIssueActivityID)
        {
            if (AuditIssueActivityID == Guid.Empty) return Json(new { status = false, message = "Invalid AuditIssueActivityID" });

            AuditIssueActivityVerify? _Obj = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _Obj = DB.AuditIssueActivityVerifies.Where(w => w.AuditIssueActivityId.Equals(AuditIssueActivityID)).FirstOrDefault();
                if (_Obj == null) return Json(new { status = false, message = "Audit Issue Activity empty" });

                DB.AuditIssueActivityVerifies.Remove(_Obj);

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
    #endregion
}