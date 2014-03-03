using System.Linq;
using System.Web.Mvc;
using WuzlStats.Models;
using WuzlStats.ViewModels.Delete;

namespace WuzlStats.Controllers
{
    public class DeleteController : Controller
    {
        private readonly Db _db = new Db();

        public ActionResult Index(int? id)
        {
            if (id.HasValue)
            {
                var score = _db.Scores.FirstOrDefault(x => x.Id == id);
                if (score != null)
                {
                    _db.Scores.Remove(score);
                    _db.SaveChanges();
                }
            }

            var model = new IndexViewModel().Build(_db.Scores.OrderByDescending(x => x.Date).Take(20));
            return View(model);
        }
    }
}
