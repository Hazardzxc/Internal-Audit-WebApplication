using Microsoft.AspNetCore.Mvc;
using STD.Models.DB;

namespace STD.Controllers
{
    [Route("Json")]
    public class JsonController : Controller
    {
        [Route("GetAuditIssueSubByAuditIssueMainID")]
        [HttpGet]
        public JsonResult GetAuditIssueSubByAuditIssueMainID(int AuditIssueMainID)
        {
            List<AuditIssueSub> _ListAuditIssueSub = new List<AuditIssueSub>();
            using (AuditManager_Connect DB = new AuditManager_Connect())
            {
                _ListAuditIssueSub = DB.AuditIssueSubs.Where(w => w.AuditIssueMainId.Equals(AuditIssueMainID)).ToList();
            }

            return Json(new { status = true, data = _ListAuditIssueSub });
        }
    }
}