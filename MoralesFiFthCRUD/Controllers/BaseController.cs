using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoralesFiFthCRUD.Repository;
namespace MoralesFiFthCRUD.Controllers
{
    public class BaseController : Controller
    {
        public database2Entities1 _db;
        public BaseRepository<User> _userRepo;
        public BaseRepository<UserRole> _userRole;
        public BaseRepository<Products> _productRepo;
        public BaseController()
        {
            _db = new database2Entities1();
            _userRepo = new BaseRepository<User>();
            _userRole = new BaseRepository<UserRole>();
            _productRepo = new BaseRepository<Products>();
        }
    }
}