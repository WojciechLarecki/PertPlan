using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using PertPlan.WebUI.Models;
using System.Diagnostics;

namespace PertPlan.WebUI.Controllers
{
    /// <summary>
    /// Kontroler obsługujący akcje związane z interakcją użytkownika na stronie domowej.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Metoda obsługująca żądanie wyświetlenia strony głównej.
        /// </summary>
        /// <returns>Widok strony głównej.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Metoda obsługująca żądanie wyświetlenia strony błędu.
        /// </summary>
        /// <returns>Widok strony błędu wraz z informacjami o błędzie.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Metoda obsługująca ustawianie języka interfejsu użytkownika.
        /// </summary>
        /// <param name="culture">Kod kultury języka, który ma zostać ustawiony.</param>
        /// <param name="returnUrl">Adres URL, do którego użytkownik zostanie przekierowany po ustawieniu języka.</param>
        /// <returns>Przekierowanie na stronę określoną przez parametr returnUrl.</returns>
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}