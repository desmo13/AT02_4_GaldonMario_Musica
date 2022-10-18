using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicaAut_GaldonMario.Models;
using System.Data;

namespace MusicaAut_GaldonMario.Controllers
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
            List<Album> listaCanciones = _context.Albums.Include(Album => Album.Artist).OrderByDescending(Album => Album.AlbumId).Take(15).ToList();
            return View(listaCanciones);
        }

        [HttpGet]
        [Authorize(Roles = "Manager,Administrator")]
        public IActionResult CreateAlbum()
        {
            ViewBag.ArtistId = new SelectList(_context.Artists, "ArtistId", "Name");
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Manager,Administrator")]
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

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }
            var albumTracks = await _context.Albums.Include(a => a.Tracks).FirstOrDefaultAsync(a => a.AlbumId == id);

            var artist = await _context.Artists.FindAsync(albumTracks.ArtistId);
            if (albumTracks == null)
            {
                return NotFound();
            }

            return View(albumTracks);
        }

        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }
            ViewBag.ArtistId = new SelectList(_context.Artists, "ArtistId", "Name");
            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            return View(album);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("AlbumId,Title,ArtistId")] Album album)
        {
            if (id != album.AlbumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(album);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExist(album.AlbumId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(album);
        }
        private bool AlbumExist(int id)
        {
            return _context.Albums.Any(e => e.AlbumId == id);
        }

        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .FirstOrDefaultAsync(m => m.AlbumId == id);
            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.ArtistId == album.ArtistId);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Albums == null)
            {
                return Problem("Entity set 'ChinookContext.Artists'  is null.");
            }




            try
            {

                var album = await _context.Albums.FirstOrDefaultAsync(a => a.AlbumId == id);
                var tracks = await _context.Tracks.FirstOrDefaultAsync(a => a.AlbumId == album.AlbumId);


                if (tracks != null)
                {
                    _context.Tracks.Remove(tracks);
                    await _context.SaveChangesAsync();





                }

                if (album != null)
                {
                    _context.Albums.Remove(album);
                    await _context.SaveChangesAsync();



                }







            }
            catch
            {
                ViewData["Error"] = "Error no se puede eliminar un album que tenga factura";
                return View();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
