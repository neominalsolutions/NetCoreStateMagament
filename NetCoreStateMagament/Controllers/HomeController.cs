using Microsoft.AspNetCore.Mvc;
using NetCoreStateMagament.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace NetCoreStateMagament.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private readonly IMemoryCache _cache;

    public HomeController(ILogger<HomeController> logger, IMemoryCache cache)
    {
      _logger = logger;
      _cache = cache;
    }

    [HttpGet("create-cookie", Name ="createCookie")]
    public IActionResult CookieSample()
    {
      CookieOptions c = new CookieOptions();
      c.HttpOnly = false; // js üzerinden cookie erişimi kapadık.
      c.Expires = DateTime.Now.AddDays(30); // 30 gün cookie değerini kullanabiliriz.
      c.Secure = true; // Cookie normalde http protokü üzerinden çalışır. Cookieler http üzerinden request de okunanbildiğinden dolayı güvenlik açığı oluştururz. Bundan dolayı sadece https protokü ile ssl ile çalış dedik.

      c.SameSite = SameSiteMode.Strict; // cookie sadece kendi domainden gelen isteklerde geçerli olsun. 

      // cookie de sadece keyvalue cinsinden string değer saklarız.
      Response.Cookies.Append("username", "ali", c);
      Response.Cookies.Append("email", "test@test.com", c);

      // cookie oluşturma sonrasında redirect ile genelde anasayfaya yönleniriz. 1 redirect sonrası cookie oluşmuş olur.


      return RedirectToRoute("getCookie",new {id=2,name="ali"});
    }

    [HttpGet("get-cookie", Name ="getCookie")]
    public IActionResult GetCookies([FromQuery] int id, [FromQuery] string name)
    {
      var username = Request.Cookies["username"];
      var email = Request.Cookies["email"];

      ViewBag.Message = "Mesaj";

      return View();
    }

    public IActionResult SetSession()
    {
      HttpContext.Session.SetString("username", "ali");
      // object tabanlı çalışmak için JSONSerializer paketini kullanrak objeyi jsonString çeviririz.
      // HttpContext.Session.Id; sistem üretir her bir oturum için oturumu tak,p etmek amaçlı unique bir id üretir.
      var sessionModel = new SessionModel();
      sessionModel.SessionId = HttpContext.Session.Id;
      // obje ile çalışmak için modelimizi jsonString çevirip sonra string olarak saklıyoruz
      var sessionModelJson = System.Text.Json.JsonSerializer.Serialize(sessionModel);
      HttpContext.Session.SetString("sessionModel", sessionModelJson);

      //HttpContext.Response.Cookies.add

      // eğer route tanımı yapılmaz ise aynı controller seviyesindeysek sadece action ismi yazmamız yeterli olucaktır.
      return RedirectToAction("GetSession");
    }

    public IActionResult GetSession()
    {
      var sessionJson = HttpContext.Session.GetString("sessionModel");

      if(sessionJson is not null)
      {
        // json stringini objeye dönüştürmüş olduk.
        var m = System.Text.Json.JsonSerializer.Deserialize<SessionModel>(sessionJson);

        return View(m);
      }


      return View();
    }

    // Cache Application bazlı çalışır. Cookie ve Session Oturum bazlı çalışır.
    // Bütün herkesi ilgilendiren veriler performans amaçlı cache de tutulur.
    public IActionResult SetCache()
    {
      // Ids keyinde veri yoksa cachle
      if(_cache.Get("Ids") == null)
      {
        var cacheIds = new List<CacheModel>();
        cacheIds.Add(new CacheModel { Id = Guid.NewGuid().ToString() });
        cacheIds.Add(new CacheModel { Id = Guid.NewGuid().ToString() });

        // Ids key ile 1 günlüğüne cacheIds verisini in memory olarak sunucunun raminde saklamak istiyoruz.
        _cache.Set("Ids", cacheIds, DateTime.Now.AddMinutes(1));
      }

     
      return Redirect("GetCache");
    }

    public IActionResult GetCache()
    {
      // cacheden veri okuma yöntemi
     var model = _cache.Get<List<CacheModel>>("Ids");

      return View(model);
    }

    // kategoriler/1
    // kategoriler/1?renk=beyaz
    //[HttpGet("kategoriler/{id}")]
    public IActionResult Index(int id)
    {
      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}