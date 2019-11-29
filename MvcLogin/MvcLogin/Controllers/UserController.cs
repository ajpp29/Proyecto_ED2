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
using Metodos;
using System.IO;
using System.Text;

namespace MvcLogin.Controllers
{

    public class UserController : Controller
    {  
        DefaultConnection db = DefaultConnection.getInstance;
        UserModel Usuario = new UserModel();

        public ActionResult EliminarChat(string receptor, string emisor)
        {
            db.EliminarChat(receptor);
            return View("Index");
        }

        //ENVIAR ARCHIVO
        [HttpGet]
        public ActionResult EnviarArchivo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EnviarArchivo(HttpPostedFileBase File)
        {
            string filePath = string.Empty;
            if (File != null)
            {
                Compresion algoritmo = new Compresion();
                string path = Server.MapPath("~/UploadedFiles/");
                var ruta2= Server.MapPath("~/DownloadedFiles/");
                filePath = path + Path.GetFileName(File.FileName);
                string extension = Path.GetExtension(File.FileName);
                File.SaveAs(filePath);
                ViewBag.Message = "Archivo Cargado";

                FileInfo fileInfo = new FileInfo(filePath);

                string nombre_original = fileInfo.Name;
                long tamanio_original = fileInfo.Length;
                db.GenerarArhivoComprimido(File, File.FileName,path,ruta2);
            }

            return View();
        }





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
                if (archivos4[i].userName == email)
                {
                    bandera = true; 
                }
            }
            if(bandera != true)
            {             
                for (int i = 0; i < archivos3.Count; i++)
                {
                if(archivos3[i].userName == email)
                {
                    archivos3[i].userFriend = db.UsuarioLoggeado.Email;
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
            IEnumerable<Models.FriendModel> UsuariosDB = new List<Models.FriendModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/Friends/"+db.UsuarioLoggeado.Email);
                //HTTP GET
                var responseTask = client.GetAsync("Friends");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    UsuariosDB = JsonConvert.DeserializeObject<IList<FriendModel>>(readTask.Result);
                }
                else
                {
                    UsuariosDB = Enumerable.Empty<FriendModel>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(UsuariosDB);
        }
       

        public ActionResult AddFriend()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddFriend(Models.FriendModel us)
         {
            us.userFriend = db.UsuarioLoggeado.Email;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/Friends");


                //HTTP POST
                var postTask = client.PostAsJsonAsync<FriendModel>("Friends", us);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            //List<Models.UserModel> archivos2 = new List<Models.UserModel>();
            //archivos2 = db.ObtenerLista();
            //for (int i = 0; i < archivos2.Count; i++)
            //{               
            //        if (archivos2[i].Email == us.userName)
            //        {
            //            db.AddFriend(us);
            //        }
            //}
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
                client.BaseAddress = new Uri("http://localhost:58142/api/users");


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

            if (ModelState.IsValid)
            {
                Usuario.Email = user.Email;
                Usuario.Password = user.Password;
                db.AddUsuario(Usuario);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Login data is incorrect."); //adicionar mensaje de error al modelo 
            }
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
            //UsuariosDB = db.ObtenerLista();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/users");
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
