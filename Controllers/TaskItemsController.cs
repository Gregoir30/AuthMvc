using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuthMvc.Data;
using AuthMvc.Models;

namespace AuthMvc.Controllers
{
    public class TaskItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TaskItems
        public async Task<IActionResult> Index()
        {
            var authMvcContext = _context.TaskItem.Include(t => t.Owner);
            return View(await authMvcContext.ToListAsync());
        }

        // GET: TaskItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.TaskItem
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(taskItem);
        }

        // GET: TaskItems/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id");
            ViewData["Statut"] = new SelectList(Enum.GetValues(typeof(StatutTache)));
            ViewData["Priorite"] = new SelectList(Enum.GetValues(typeof(PrioriteTache)));
            return View();
        }

        // POST: TaskItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titre,Description,DateEcheance,Statut,Priorite,OwnerId,Fichier")] TaskItem taskItem)
        {
            if (ModelState.IsValid)
            {
                if (taskItem.Fichier != null && taskItem.Fichier.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", taskItem.Fichier.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await taskItem.Fichier.CopyToAsync(stream);
                    }
                    // You can store the file path or name in the database if needed
                }

                _context.Add(taskItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", taskItem.OwnerId);
            ViewData["Statut"] = new SelectList(Enum.GetValues(typeof(StatutTache)), taskItem.Statut);
            ViewData["Priorite"] = new SelectList(Enum.GetValues(typeof(PrioriteTache)), taskItem.Priorite);
            return View(taskItem);
        }

        // GET: TaskItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.TaskItem.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", taskItem.OwnerId);
            return View(taskItem);
        }

        // POST: TaskItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Description,DateEcheance,Statut,Priorite,OwnerId")] TaskItem taskItem)
        {
            if (id != taskItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskItemExists(taskItem.Id))
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
            ViewData["OwnerId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", taskItem.OwnerId);
            return View(taskItem);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Aucun fichier sélectionné.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "upload", "taskitems", id.ToString());
            Directory.CreateDirectory(uploadsFolder); // Crée le dossier s’il n'existe pas

            var filePath = Path.Combine(uploadsFolder, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Optionnel : enregistrer le chemin en base dans TaskItem
            var task = await _context.TaskItem.FindAsync(id);
            if (task != null)
            {
                task.CheminFichier = Path.Combine("upload", "taskitems", id.ToString(), file.FileName);
                _context.Update(task);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id });
        }

        // GET: TaskItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.TaskItem
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(taskItem);
        }

        // POST: TaskItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskItem = await _context.TaskItem.FindAsync(id);
            if (taskItem != null)
            {
                _context.TaskItem.Remove(taskItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskItemExists(int id)
        {
            return _context.TaskItem.Any(e => e.Id == id);
        }
    }
}
