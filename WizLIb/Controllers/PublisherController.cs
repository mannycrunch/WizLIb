using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WizLib_DataAccess.Data;
using WizLib_Model.Models;

namespace WizLib.Controllers
{
    public class PublisherController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PublisherController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Publisher> objList = _db.Publishers.ToList();
            return View(objList);
        }

        public IActionResult AddEdit(int? id)
        {
            Publisher obj = new Publisher();
            if (id == null)
            {   //Add
                return View(obj);
            }
            obj = _db.Publishers.FirstOrDefault(u => u.Publisher_Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEdit(Publisher obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Publisher_Id == 0)
                {
                    //Create new Category
                    _db.Publishers.Add(obj);
                }
                else
                {
                    //Update Category
                    _db.Publishers.Update(obj);
                }
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        public IActionResult Delete(int id)
        {
            var objFromDb = _db.Publishers.FirstOrDefault(c => c.Publisher_Id == id);
            _db.Publishers.Remove(objFromDb);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
