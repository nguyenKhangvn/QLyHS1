using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLyHS1.Data;
using QLyHS1.Models;
namespace QLyHS1.Controllers
{
    public class SubjectController : Controller
    {
        private readonly QlyHs1Context _context;

        public SubjectController(QlyHs1Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
           var sub = from st in _context.Subjects
                           select new SubjectViewModel
                           {
                               Id = st.Id,
                               Name = st.Name,
                               
                           };

            return View(sub.ToList());
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(SubjectDetailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var sub = new Subject
            {
                Name = model.Name,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                Status = true
            };

            _context.Subjects.Add(sub);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Student/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sub = await _context.Subjects.FindAsync(id);
            if (sub == null)
            {
                return NotFound();
            }

            // Ánh xạ dữ liệu từ `Student` sang `StudentDetailToEditViewModel`
            var subViewModel = new SubjectViewModel
            {
                Id = sub.Id,          
                Name = sub.Name, 
                CreateAt = sub.CreateAt,
                UpdateAt = sub.UpdateAt,
                Status = sub.Status
            };
            return View(subViewModel);
        }


        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status")] SubjectViewModel subViewModel)
        {
            if (id != subViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var student = await _context.Subjects.FindAsync(id);
                    if (student == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các thuộc tính cần thiết
                
                    student.Name = subViewModel.Name;
                    student.CreateAt = subViewModel.CreateAt;
                    student.UpdateAt = DateTime.Now;
                    student.Status = subViewModel.Status;

                    _context.Update(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(subViewModel);
        }



        private bool StudentExists(int id)
        {
            return _context.Subjects.Any(e => e.Id == id);
        }

        //detail
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Subjects.FindAsync(id);
            if (student != null)
            {
                _context.Subjects.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
