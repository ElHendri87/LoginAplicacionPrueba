using System.Web.Mvc;

namespace LoginAplicacion.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}

		// Nueva acción para cerrar sesión
		public ActionResult Logout()
		{
			// Eliminar cualquier variable de sesión o autenticación
			Session.Clear();
			Session.Abandon();

			// Redirigir a la página de inicio de sesión o a la página principal
			return RedirectToAction("Login", "Acceso");
		}
	}
}
