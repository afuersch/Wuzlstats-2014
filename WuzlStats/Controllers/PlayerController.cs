using System.Web.Mvc;
using WuzlStats.Models;
using WuzlStats.ViewModels.Player;

namespace WuzlStats.Controllers
{
    public class PlayerController : Controller
    {
        private readonly Db _db = new Db();

        [Route("Player/{playerName}")]
        public ActionResult Index(string playerName)
        {
            return View(new IndexViewModel().Build(_db, playerName));
        }
    }
}