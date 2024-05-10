using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MoralesFiFthCRUD.ViewModels;
using MoralesFiFthCRUD.Repository;
using MoralesFiFthCRUD.Contracts;

namespace MoralesFiFthCRUD.Controllers
{

    public class HomeController : BaseController
    {
        // GET: Home
        private readonly database2Entities1 _dbContext;
       
        public HomeController()
        {
            _dbContext = new database2Entities1();
           
        }

        public ActionResult Index()
        {
            return View(_userRepo.GetAll());
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Login");
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(User u)
        {
            var user = _userRepo._table.Where(m => m.username == u.username).FirstOrDefault();
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(u.username, false);
                return RedirectToAction("Dashboard");
            }
            ModelState.AddModelError("", "User not Exist or Incorrect Password");

            return View(u);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(User u, string SelectedRole)
        {
            _userRepo.Create(u);

            var userAdded = _userRepo._table.FirstOrDefault(m => m.username == u.username);

            if (userAdded == null)
            {
                // Handle case where user creation failed
                ModelState.AddModelError("", "Failed to create user.");
                return View(u); // Redisplay the form with an error message
            }

            if (string.IsNullOrEmpty(SelectedRole))
            {
                // Handle case where role is not selected
                ModelState.AddModelError("", "Role not selected.");
                return View(u); // Redisplay the form with an error message
            }

            var role = _db.Role.FirstOrDefault(r => r.roleName == SelectedRole);

            if (role == null)
            {
                // Handle case where role is not found (invalid selection)
                ModelState.AddModelError("", "Invalid role selected.");
                return View(u); // Redisplay the form with an error message
            }

            var userRole = new UserRole
            {
                userId = userAdded.id,
                roleId = role.id // Assign the retrieved roleId
            };

            _userRole.Create(userRole);

            TempData["Msg"] = $"User {u.username} added!";
            return RedirectToAction("LandingPage");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int id)
        {
            return View(_userRepo.Get(id));
        }
        [Authorize(Roles = "Tutor")]
        public ActionResult Edit(int id)
        {

            return View(_userRepo.Get(id));
        }
        [HttpPost]
        public ActionResult Edit(User u)
        {
            _userRepo.Update(u.id, u);
            TempData["Msg"] = $"User {u.username} updated!";

            return RedirectToAction("index");

        }
        
        public ActionResult Delete(int id)
        {
            _userRepo.Delete(id);
            TempData["Msg"] = $"User deleted!";
            return RedirectToAction("index");
        }
        public ActionResult LandingPage()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult ContactUs()
        {
            return View();
        }
        public ActionResult Shop(string searchTerm, string sellerName)
        {
            // Fetch all products from the database
            var products = _dbContext.Products.ToList();

            // Filter the products based on the search term if it's not null or empty
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Filter the products based on the search term
                products = products.Where(p => p.ProductName.Contains(searchTerm)).ToList();
            }

            // Filter the products based on the seller name if it's not null or empty
            if (!string.IsNullOrWhiteSpace(sellerName))
            {
                // Filter the products based on the seller name
                products = products.Where(p => p.User.username == sellerName).ToList();
            }

            // Map the list of Products to a list of ProductViewModel
            var productViewModels = products
                .Where(p => p.Category != null && p.ProductImg != null)
                .Select(p => new ProductViewModel
                {
                    ProductName = p.ProductName,
                    Category = p.Category != null ? p.Category.CategoryName : "N/A", // Add null check for Category
            ProductImg = p.ProductImg,
                    Quantity = p.Quantity ?? 0,
                    Description = p.description,
                    Price = p.price != null ? (decimal)p.price : 0,
                    sellerName = p.User.username
                })
                .ToList();

            // Pass the list of ProductViewModel to the view
            return View(productViewModels);
        }




        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(User u, string SelectedRole)
        {
            _userRepo.Create(u);

            var userAdded = _userRepo._table.FirstOrDefault(m => m.username == u.username);

            if (userAdded == null)
            {
                // Handle case where user creation failed
                ModelState.AddModelError("", "Failed to create user.");
                return View(u); // Redisplay the form with an error message
            }

            if (string.IsNullOrEmpty(SelectedRole))
            {
                // Handle case where role is not selected
                ModelState.AddModelError("", "Role not selected.");
                return View(u); // Redisplay the form with an error message
            }

            var role = _db.Role.FirstOrDefault(r => r.roleName == SelectedRole);

            if (role == null)
            {
                // Handle case where role is not found (invalid selection)
                ModelState.AddModelError("", "Invalid role selected.");
                return View(u); // Redisplay the form with an error message
            }

            var userRole = new UserRole
            {
                userId = userAdded.id,
                roleId = role.id // Assign the retrieved roleId
            };

            _userRole.Create(userRole);

            TempData["SuccessMsg"] = $"User {u.username} added!";
            return RedirectToAction("LandingPage");
        }
        public ActionResult SellerView()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public ActionResult SellerView(string productName, int categoryId, HttpPostedFileBase productImage, string productDescription, int productPrice, int productQuantity)
        {
            // Get the username of the currently logged-in user
            string userName = User.Identity.Name;

            // Retrieve the user from the repository based on the username
            var user = _userRepo._table.FirstOrDefault(u => u.username == userName);

            if (user == null)
            {
                // Handle the case where the user is not found
                ModelState.AddModelError("", "User not found.");
                return View();
            }

            // Retrieve the category from the database based on the categoryId provided in the form
            var category = _db.Category.FirstOrDefault(c => c.id == categoryId);

            if (category == null)
            {
                // Handle the case where the category is not found
                ModelState.AddModelError("", "Category not found.");
                return View(); // You may redirect to an error page or display an error message
            }

            // Check if a file was uploaded
            if (productImage == null || productImage.ContentLength == 0)
            {
                ModelState.AddModelError("", "Please select a product image.");
                return View(); // Return to the view to display the error message
            }

            // Check if the file type is valid
            if (!productImage.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError("", "Please upload a valid image file.");
                return View(); // Return to the view to display the error message
            }

            // Check if the file size is within the limit (e.g., 5MB)
            if (productImage.ContentLength > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("", "The image size exceeds the limit (5MB). Please upload a smaller image.");
                return View(); // Return to the view to display the error message
            }

            // Read the file data and convert it to a byte array
            byte[] imageData;
            using (var binaryReader = new BinaryReader(productImage.InputStream))
            {
                imageData = binaryReader.ReadBytes(productImage.ContentLength);
            }

            // Create a new Product object with the provided data
            var product = new Products
            {
                ProductName = productName,
                CategoryId = categoryId,
                UserId = user.id,
                price = productPrice,
                description = productDescription,
                Quantity = productQuantity, // Assign the quantity
                ProductImg = imageData // Assign the image data
            };

            // Add the product to the repository
            _productRepo.Create(product);

            TempData["SuccessMsg"] = "Product added successfully!";

            return RedirectToAction("SellerView"); // You may redirect to the product list page or any other appropriate page
        }




        public ActionResult MessageUs()
        {
            return View();

        }
        [Authorize(Roles = "Buyer")]
        public ActionResult Userprofile()
        {
            return View();
        }
        [Authorize(Roles = "Seller")]
        public ActionResult Resellerprofile()
        {
            // Get the username of the currently logged-in user
            string userName = User.Identity.Name;

            // Retrieve the user from the repository based on the username
            var user = _dbContext.User.FirstOrDefault(u => u.username == userName);

            if (user == null)
            {

                return View("Error");
            }


            var products = _dbContext.Products
              .Where(p => p.UserId == user.id && p.Category != null)
              .ToList() // Fetch the data from the database
              .Select(p => new ProductViewModel
              {
                  ProductID = p.ProductID,
                  ProductName = p.ProductName,
                  Category = p.Category.CategoryName,
                  ProductImg = p.ProductImg,
                  Description = p.description,
                  Quantity = p.Quantity ?? 0,
                  Price = p.price != null ? (decimal)p.price : 0,
              })
              .ToList();


           
            return View(products);
        }



        public ActionResult test(string username)
        {
            // Retrieve the user from the repository based on the username
            var user = _dbContext.User.FirstOrDefault(u => u.username == username);

            if (user == null)
            {
                return View("Error");
            }

            var products = _dbContext.Products
                .Where(p => p.UserId == user.id && p.Category != null)
                .ToList() // Fetch the data from the database
                .Select(p => new ProductViewModel
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Category = p.Category.CategoryName,
                    ProductImg = p.ProductImg,
                    Description = p.description,
                    sellerName = p.User.username,
                    ProductOwner = p.User.username,
                    Price = p.price != null ? (decimal)p.price : 0
                })
                .ToList();

            return View(products);
        }




        public ActionResult DeleteProduct(int id)
        {
            var result = _productRepo.Delete(id);

            if (result == ErrorCode.Success)
            {
                // Product deleted successfully
                TempData["SuccessMsg"] = "Product deleted successfully!";
            }
            else
            {
                // Failed to delete product
                TempData["ErrorMsg"] = "Failed to delete product.";
            }

            return RedirectToAction("ResellerProfile");
        }


        }




}


