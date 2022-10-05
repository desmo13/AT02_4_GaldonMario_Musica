using AT02_4_GaldonMario_Musica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AT02_4_GaldonMario_Musica.Controllers
{
    public class AlbumController : Controller
    {
        private readonly ChinookContext _context;

        public AlbumController(ChinookContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Album> listaCanciones = _context.Albums.Include(Album => Album.Artist).OrderByDescending(Album => Album.Title).Take(15).ToList();
            return View(listaCanciones);
        }

        [HttpGet]
        public IActionResult CreateAlbum()
        {
            ViewBag.ArtistId = new SelectList(_context.Artists, "ArtistId", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult CreateAlbum(Album album)
        {
            album.AlbumId = _context.Albums.Max(Album => Album.AlbumId) + 1;
            if (ModelState.IsValid)
            {
                _context.Albums.Add(album); 
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
            
        }
    }
}
