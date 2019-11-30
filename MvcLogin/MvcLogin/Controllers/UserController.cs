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
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MvcLogin.Controllers
{

    public class UserController : Controller
    {




        [HttpPost]
        public HttpResponseMessage Login(User user)
        {
            bool Isvalid = false;
            IEnumerable<Models.User> UsuariosDB = new List<Models.User>();
            //archivos2 = db.ObtenerLista();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/users");
                //HTTP GET
                var responseTask = client.GetAsync("Users");
                //var responseTask1 = client.GetAsync("Users");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    UsuariosDB = JsonConvert.DeserializeObject<IList<User>>(readTask.Result);
                }

                IEnumerable<Models.User> Query = from prod in UsuariosDB
                                                 where prod.userName == user.userName
                                                 select prod;

                foreach (Models.User item in Query)
                {
                    if (item.userName == user.userName)  //Verificar password del usuario
                    {
                        User u = item;
                        if (u == null)
                            return Request.CreateResponse(HttpStatusCode.NotFound,
                                 "The user was not found.");
                        bool credentials = u.Password.Equals(user.Password);
                        if (!credentials) return Request.CreateResponse(HttpStatusCode.Forbidden,
                            "The username/password combination was wrong.");
                        return Request.CreateResponse(HttpStatusCode.OK,
                             TokenManager.GenerateToken(user.userName));
                    }
                }
            }
        }





        DefaultConnection db = DefaultConnection.getInstance;
        UserModel Usuario = new UserModel();
        [HttpGet]
        public ActionResult CambiarContraseña()
        {
            User modificar = new User();
            modificar.userName = db.UsuarioLoggeado.Email;
            modificar.Password = db.UsuarioLoggeado.Password;
            return View(modificar);
        }

        [HttpPost]
        public ActionResult CambiarContraseña(User user)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/users/edit");

                //HTTP POST
                var putTask = client.PutAsJsonAsync<User>("edit", user);
                putTask.Wait();

                var result = putTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        public bool Obtenerusuario(string usuario)
        {
            bool Isvalid = false;
            IEnumerable<Models.User> UsuariosDB = new List<Models.User>();
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
                                             where prod.userName == usuario
                                             select prod;

            foreach (Models.User item in Query)
            {
                if (item.userName == usuario)  //Verificar password del usuario
                {
                    Isvalid = true;
                }
            }
            return Isvalid;
        }
       

        public ActionResult EliminarChat(string userFriend, string userName)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/Messages/");
                string parametros = "DeleteMessages/" + userName + "/" + userFriend;
                //HTTP DELETE
                var deleteTask = client.DeleteAsync(parametros.ToString());
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    Eliminarchat2(userFriend, userName);
                }
            }
            return View("Index");
        }

        public void Eliminarchat2(string userFriend, string userName)
        {
            using (var client2 = new HttpClient())
            {
                client2.BaseAddress = new Uri("http://localhost:58142/api/Chats/");
                string parametros2 = "Delete/" + userName + "/" + userFriend;
                //HTTP DELETE
                var deleteTask2 = client2.DeleteAsync(parametros2.ToString());
                deleteTask2.Wait();
            }
        }
        public ActionResult EliminarMensajes(string userFriend, string userName)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/Messages/");
                string parametros = "DeleteMessages/" + userName + "/" + userFriend;
                //HTTP DELETE
                var deleteTask = client.DeleteAsync(parametros.ToString());
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("Index");
                }
            }
            return View();
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

        public ActionResult Delete(string email)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/Friends/");
                string Email = db.UsuarioLoggeado.Email;
                string userFriend = email;
                string parametros = "Delete/" + Email + "/" + userFriend;
                //HTTP DELETE
                var deleteTask = client.DeleteAsync(parametros.ToString());
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("Index");
                }
            }

            return View("Index");
        }




        public ActionResult ObtenerChats()
        {
            IEnumerable<Models.Chat> UsuariosDB = new List<Models.Chat>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/Chats/GetChat/");
                //HTTP GET
                var responseTask = client.GetAsync(db.UsuarioLoggeado.Email);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    UsuariosDB = JsonConvert.DeserializeObject<IList<Chat>>(result.Content.ReadAsStringAsync().Result);
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    UsuariosDB = JsonConvert.DeserializeObject<IList<Chat>>(readTask.Result);
                }
                else
                {
                    UsuariosDB = Enumerable.Empty<Chat>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(UsuariosDB);
        }
        public ActionResult AddChat(string email)
        {
            Chat nuevochat = new Chat();
            nuevochat.userRecipient = email;
            nuevochat.userSender = db.UsuarioLoggeado.Email;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/Chats/Create");


                //HTTP POST
                var postTask = client.PostAsJsonAsync<Chat>("Create", nuevochat);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            return View("Index");
        }



        MensajeModel Nuevo = new MensajeModel();
        public ActionResult NuevoMensaje(string emisor, string receptor)
        {
            Nuevo.userSender = emisor;
            Nuevo.userRecipient = receptor;
            return View(Nuevo);
        }
        [HttpPost]
        public ActionResult NuevoMensaje(MensajeModel nuevo)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/Messages/SendMessage");


                //HTTP POST
                var postTask = client.PostAsJsonAsync<MensajeModel>("SendMessage", nuevo);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            return View("Index");
        }
        public ActionResult ObtenerMensajes(string receptor, string emisor )
        {
            IEnumerable<Models.MensajeModel> MensajesDB = new List<Models.MensajeModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/Messages/");
                //HTTP GET
                var responseTask = client.GetAsync("GetConversation/" + emisor + "/" + receptor);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    MensajesDB = JsonConvert.DeserializeObject<IList<MensajeModel>>(result.Content.ReadAsStringAsync().Result);
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                }
                else
                {
                    MensajesDB = Enumerable.Empty<MensajeModel>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(MensajesDB);
        }


        public ActionResult SeeFriends()
        {
            IEnumerable<Models.FriendModel> UsuariosDB = new List<Models.FriendModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/Friends/");
                //HTTP GET
                var responseTask = client.GetAsync(db.UsuarioLoggeado.Email);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    UsuariosDB = JsonConvert.DeserializeObject<IList<FriendModel>>(result.Content.ReadAsStringAsync().Result);
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
            us.userFriend = us.userName;
            us.userName = db.UsuarioLoggeado.Email;
            if (Obtenerusuario(us.userFriend) == true )
            {
                if (YasonAmigos(us)!=true)
                {
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
            if (Obtenerusuario(user.Email) != true)
            {
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
            }
            
            return View();
        }

        public ActionResult LogOut()
        {

            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public bool YasonAmigos(FriendModel us)
        {
            bool Isvalid = false;
            IEnumerable<Models.User> amigosDB = new List<Models.User>();
            //archivos2 = db.ObtenerLista();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/Friends/");
                //HTTP GET
                var responseTask = client.GetAsync(us.userFriend);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    amigosDB = JsonConvert.DeserializeObject<IList<User>>(readTask.Result);
                }
                else
                {
                    amigosDB = Enumerable.Empty<User>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            IEnumerable<Models.User> Query = from prod in amigosDB
                                             where prod.userName == db.UsuarioLoggeado.Email
                                             select prod;

            foreach (Models.User item in Query)
            {
                if (item.userName == db.UsuarioLoggeado.Email)  //Verificar password del usuario
                {
                    Isvalid = true;
                }
            }
            return Isvalid;
        }

        private bool Isvalid(string Email, string password)
        {
            bool Isvalid = false;
            IEnumerable<Models.User> UsuariosDB = new List<Models.User>();
            //archivos2 = db.ObtenerLista();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/users");
                //HTTP GET
                var responseTask = client.GetAsync("Users");
                //var responseTask1 = client.GetAsync("Users");
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
