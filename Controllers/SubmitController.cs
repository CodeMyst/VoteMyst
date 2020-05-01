using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace VoteMyst.Controllers
{
    public class SubmitController : Controller
    {
        public const int MaxFileSize = 4000000;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormFile file)
        {
            if (file.Length > MaxFileSize) {
                ViewBag.Message = "File too large!";
                return View();
            }

            string fileName = System.IO.Path.GetFileName(file.FileName);
            string filePath = GetSubmissionPath(1, 1, System.IO.Path.GetExtension(file.FileName));
            using (var localFile = System.IO.File.OpenWrite(filePath))
            using (var uploadedFile = file.OpenReadStream())
            {
                uploadedFile.CopyTo(localFile);
            }

            ViewBag.Message = "File successfully uploaded!";

            return View();
        }

        private string GetSubmissionPath(ulong eventId, ulong userId, string extension) 
        {
            System.IO.Directory.CreateDirectory($"./assets/{eventId}");
            return $"./assets/{eventId}/{userId}{extension}";
        }
    }
}