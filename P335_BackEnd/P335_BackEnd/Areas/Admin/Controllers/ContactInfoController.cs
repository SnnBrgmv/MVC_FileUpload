using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P335_BackEnd.Areas.Admin.Models;
using P335_BackEnd.Data;

namespace P335_BackEnd.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactInfoController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ContactInfoController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var contactInfo = _dbContext.ContactInfo.FirstOrDefault();
            if (contactInfo == null) return NotFound();

            var model = new ContactInfoIndexVM
            {
                Email = contactInfo.Email,
                PhoneNumber = contactInfo.PhoneNumber
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Index(ContactInfoIndexVM model)
        {
            var contactInfo = _dbContext.ContactInfo.FirstOrDefault();
            if (contactInfo == null) return NotFound();

            contactInfo.Email = model.Email;
            contactInfo.PhoneNumber = model.PhoneNumber;

            _dbContext.ContactInfo.Update(contactInfo);
            _dbContext.SaveChanges();

            return View(model);
        }
    }
}
