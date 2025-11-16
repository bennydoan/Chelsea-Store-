using BestStoreMVC.Models;
using BestStoreMVC.Models.ProductDTOs;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;

namespace BestStoreMVC.Controllers
{
    [Authorize(Roles ="Admin")]
  

    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment environment;

        public ProjectController(ApplicationDbContext context, IWebHostEnvironment environment )
        {
            _context = context;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            var products = _context.Products.OrderByDescending(p => p.Id).ToList();
            return View(products);
        }

        public IActionResult Create()
        {
           return View();
        }

        [HttpPost]
        public IActionResult Create(ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
            { 
                return View(productDTO);
            }

            // save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDTO.ImageFileName!.FileName);

            string imageFullPath = environment.WebRootPath + "/image/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDTO.ImageFileName.CopyTo(stream);
            }


            Product product = new Product()
            {
                Name = productDTO.Name,
                Brand = productDTO.Brand,
                Category = productDTO.Category,
                Price = productDTO.Price,
                Description = productDTO.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now,
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index", "Project");
        }


        public IActionResult Edit(int id )
        {
            var prodcut = _context.Products.Find(id);

            if (prodcut == null)
            {
                return RedirectToAction("Index", "Project");
            }

            var productDto = new ProductDTO()
            {
                Name = prodcut.Name,
                Brand = prodcut.Brand,
                Category = prodcut.Category,
                Price = prodcut.Price,
                Description = prodcut.Description,
            };

            ViewData["ProductID"] = prodcut.Id;

            ViewData["ImageFileName"] = prodcut.ImageFileName;

            ViewData["CreateAt"] = prodcut.CreatedAt;


            return View(productDto);
        }


        [HttpPost]
        public IActionResult Edit(int id,ProductDTO productDto)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Project");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ProductID"] = product.Id;

                ViewData["ImageFileName"] = product.ImageFileName;

                ViewData["CreateAt"] = product.CreatedAt;

                return View(productDto);
            }

            // save the image file

            string newFileName = product.ImageFileName;

            newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFileName!.FileName);

            string imageFullPath = environment.WebRootPath + "/image/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFileName.CopyTo(stream);
            }

            // delete the old image
            string oldImageFullPath = environment.WebRootPath + "/image/" + product.ImageFileName;
            System.IO.File.Delete(oldImageFullPath);

            //update 

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.ImageFileName = newFileName;
            product.Price = productDto.Price;   

            _context.SaveChanges();
            return RedirectToAction("Index", "project");

        }

     


        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Project");

            }

            string ImageFullPath = environment.WebRootPath + "/image/" + product.ImageFileName;
            System.IO.File.Delete(ImageFullPath);

            ViewData["ImageFileName"] = product.ImageFileName;


            _context.Products.Remove(product);
            _context.SaveChanges(true);

            return RedirectToAction("Index","Project");
        }

    }
}
