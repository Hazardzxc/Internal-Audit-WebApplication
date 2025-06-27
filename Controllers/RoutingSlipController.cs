using Microsoft.AspNetCore.Mvc;
using STD.Models.DB;
using STD.Models.Form;

namespace STD.Controllers
{
    public class RoutingSlipController : Controller
    {
        [HttpGet]
        public IActionResult Index(int AuditManagementID)
        {
            ViewBag.AuditManagementID = AuditManagementID;
            return View("~/Views/RoutingSlip/Index.cshtml");
        }

        [HttpGet]
        public IActionResult GetRoutingSlipForm(int RoutingSlipID)
        {
            RoutingSlip? Model = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                Model = DB.RoutingSlips.Where(w => w.RoutingSlipId.Equals(RoutingSlipID)).FirstOrDefault();
            }

            return View("~/Views/RoutingSlip/RoutingSlipForm.cshtml", Model);
        }

        [HttpPost]
        public IActionResult GetRoutingSlip(int AuditManagementID)
        {
            List<object> _List = new List<object>();
            List<RoutingSlip> _ListRoutingSlip = new List<RoutingSlip>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _ListRoutingSlip = DB.RoutingSlips.Where(w => w.AuditManagementId.Equals(AuditManagementID)).ToList();
                if (_ListRoutingSlip != null && _ListRoutingSlip.Count > 0)
                {
                    foreach (RoutingSlip Item in _ListRoutingSlip)
                    {
                        _List.Add(new
                        {
                            RoutingSlipID = Item.RoutingSlipId,
                            AuditManagementID = Item.AuditManagementId,
                            AuditPlan = Item.AuditPlan,
                            RoutingSlipDetail = Item.RoutingSlipDetail,
                            Practicality = Item.Practicality,
                            Comment = Item.Comment,
                        });
                    }
                }
            }

            return Json(new { status = true, data = _List });
        }

        [HttpPost]
        public IActionResult SaveRoutingSlip([FromForm] RoutingSlipForm Model)
        {
            if (Model == null) return Json(new { status = false, message = "Invalid Data" });
            if (Model.AuditManagementID <= 0) return Json(new { status = false, message = "Audit Management is required" });
            if (string.IsNullOrEmpty(Model.AuditPlan)) return Json(new { status = false, message = "Audit Plan is required" });
            if (string.IsNullOrEmpty(Model.RoutingSlipDetail)) return Json(new { status = false, message = "Routing Slip Detail is required" });
            if (string.IsNullOrEmpty(Model.Practicality)) return Json(new { status = false, message = "Practicality is required" });

            RoutingSlip? _RoutingSlip = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _RoutingSlip = DB.RoutingSlips.Where(w => w.RoutingSlipId.Equals(Model.RoutingSlipID)).FirstOrDefault();
                if (_RoutingSlip == null)
                {
                    _RoutingSlip = new RoutingSlip();
                    _RoutingSlip.AuditManagementId = Model.AuditManagementID;
                    _RoutingSlip.AuditPlan = Model.AuditPlan;
                    _RoutingSlip.RoutingSlipDetail = Model.RoutingSlipDetail;
                    _RoutingSlip.Practicality = Model.Practicality;
                    _RoutingSlip.Comment = Model.Comment;
                    _RoutingSlip.CreateDate = DateTime.Now;
                    DB.RoutingSlips.Add(_RoutingSlip);
                }
                else
                {
                    _RoutingSlip.AuditManagementId = Model.AuditManagementID;
                    _RoutingSlip.AuditPlan = Model.AuditPlan;
                    _RoutingSlip.RoutingSlipDetail = Model.RoutingSlipDetail;
                    _RoutingSlip.Practicality = Model.Practicality;
                    _RoutingSlip.Comment = Model.Comment;
                    DB.RoutingSlips.Update(_RoutingSlip);
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
        public IActionResult DeleteRoutingSlip(int RoutingSlipID)
        {
            if (RoutingSlipID <= 0) return Json(new { status = false, message = "RoutingSlipID is required" });

            RoutingSlip? _RoutingSlip = null;
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _RoutingSlip = DB.RoutingSlips.Where(w => w.RoutingSlipId.Equals(RoutingSlipID)).FirstOrDefault();
                if (_RoutingSlip == null) return Json(new { status = false, message = "RoutingSlip empty" });
                DB.RoutingSlips.Remove(_RoutingSlip);

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
