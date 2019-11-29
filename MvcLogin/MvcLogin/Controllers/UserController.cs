using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MvcLogin.DBContext;
using MvcLogin.Models;
using Newtonsoft.Json;

namespace MvcLogin.Controllers
{
    public class UserController : Controller
    {
        DefaultConnection db = DefaultConnection.getInstance;
        UserModel Usuario = new UserModel();
        [HttpGet]
        public ActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public ActionResult logIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult logIn(Models.UserModel user)
        {
            if (ModelState.IsValid) //Verificar que el modelo de datos sea valido en cuanto a la definición de las propiedades
            {
                if (Isvalid(user.Email, user.Password))//Verificar que el email y clave exista utilizando el método privado 
                {
                    Usuario.Email = user.Email;
                    Usuario.Password = user.Password;
                    db.UsuarioLoggeado = user;
                    FormsAuthentication.SetAuthCookie(user.Email, false); //crea variable de usuario 
                    return RedirectToAction("Index", "Home");  //dirigir controlador home vista Index una vez se a autenticado en el sistema
                }
                else
                {
                    ModelState.AddModelError("", "Login data in incorrect"); //adicionar mensaje de error al model 
                }
            }
            return View(user);
        }

        public ActionResult Delete(string usuario)
        {
            db.eliminarAmigo(usuario);
            return View("Index");
        }
        public ActionResult ObtenerChats()
        {
            return View(db.ObtenerChats());
        }
        public ActionResult AddChat(string email)
        {
            List<Models.FriendModel> archivos3 = new List<Models.FriendModel>();
            archivos3 = db.ObtenerFriends();
            List<Models.FriendModel> archivos4 = new List<Models.FriendModel>();
            archivos4 = db.ObtenerChats();
            bool bandera = false;
            if (email != null)
            {
                for (int i = 0; i < archivos4.Count; i++)
            {
                if (archivos4[i].Email == email)
                {
                    bandera = true; 
                }
            }
            if(bandera != true)
            {             
                for (int i = 0; i < archivos3.Count; i++)
                {
                if(archivos3[i].Email == email)
                {
                    archivos3[i].UsuarioLoggeado = db.UsuarioLoggeado.Email;
                    db.AddChat(archivos3[i]);
                }
                }
                }
            }
            return View("Index");
        }
        MensajeModel nuevo = new MensajeModel();
        public ActionResult NuevoMensaje(string emisor, string receptor)
        {
            nuevo.emisor = emisor;
            nuevo.receptor = receptor;
            return View("NuevoMensaje");
        }
        [HttpPost]
        public ActionResult NuevoMensaje(MensajeModel Nuevo)
        {
            nuevo.Contenido = Nuevo.Contenido;
            nuevo.emisor = Nuevo.emisor;
            nuevo.receptor = Nuevo.receptor;
            db.addmensaje(nuevo);
            return View("Index");
        }
        public ActionResult ObtenerMensajes()
        {
            List<MensajeModel> listaMensajes = new List<MensajeModel>();
            listaMensajes = db.ObtenerMensajes();
            return View(listaMensajes);
        }


        public ActionResult SeeFriends()
        {
            List<Models.FriendModel> archivos3 = new List<Models.FriendModel>();
            archivos3 = db.ObtenerFriends();
            return View(archivos3);
        }
        [HttpPost]
        public ActionResult SeeFriends(Models.FriendModel us)
        {
            List<Models.FriendModel> archivos3 = new List<Models.FriendModel>();
            archivos3 = db.ObtenerFriends();
            return View(archivos3);
        }

        public ActionResult AddFriend()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddFriend(Models.FriendModel us)
         {
            List<Models.UserModel> archivos2 = new List<Models.UserModel>();
            archivos2 = db.ObtenerLista();
            for (int i = 0; i < archivos2.Count; i++)
            {               
                    if (archivos2[i].Email == us.Email)
                    {
                        db.AddFriend(us);
                    }
            }
            return View("Index");
        }
        public ActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Registration(Models.UserModel user)
        {
            User newuser = new User();
            newuser.userName = user.Email;
            newuser.Password = user.Password;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44300/api/users");


                //HTTP POST
                var postTask = client.PostAsJsonAsync<User>("Users", newuser);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            //if (ModelState.IsValid)
            //{
            //    Usuario.Email = user.Email;
            //    Usuario.Password = user.Password;
            //    db.AddUsuario(Usuario);
            //    return RedirectToAction("Index", "Home");
            //}
            //else
            //{
            //    ModelState.AddModelError("", "Login data is incorrect."); //adicionar mensaje de error al modelo 
            //}
            return View();
        }

        public ActionResult LogOut()
        {

            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private bool Isvalid(string Email, string password)
        {
            bool Isvalid = false; 
            IEnumerable<Models.User> UsuariosDB = new List<Models.User>();
            //archivos2 = db.ObtenerLista();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44300/api/users");
                //HTTP GET
                var responseTask = client.GetAsync("Users"); 
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    UsuariosDB = JsonConvert.DeserializeObject<IList<User>>(readTask.Result);
                }
                else
                {
                    UsuariosDB = Enumerable.Empty<User>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            IEnumerable<Models.User> Query = from prod in UsuariosDB
                                                  where prod.userName == Email
                                                  select prod;

            foreach (Models.User item in Query)
            {
                if (item.Password == password)  //Verificar password del usuario
                {
                    Isvalid = true;
                }
            }
            return Isvalid;
        }
    }
}
