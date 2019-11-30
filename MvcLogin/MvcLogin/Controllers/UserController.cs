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
using System.Net;

namespace MvcLogin.Controllers
{

    public class UserController : Controller
    {

        [HttpGet]
        public bool Validate(string username)
        {
            bool exists = db.EncontrarToken(username);
            if (!exists) return false;
            string tokenUsername = TokenManager.ValidateToken(db.ObtenerToken(username));
            if (username.Equals(tokenUsername))
                return true;
            return false;
        }


        [HttpPost]
        public ActionResult logIn(Models.UserModel user)
        {
            HttpResponseMessage hrm = new HttpResponseMessage();
            IEnumerable<Models.User> UsuariosDB = new List<Models.User>();
            //archivos2 = db.ObtenerLista();


            if (Isvalid(user.Email, user.Password))  //Verificar password del usuario
            {

                UserModel u = new Models.UserModel();
                u.Email = user.Email;
                u.Password = user.Password;
                    
                bool credentials = u.Password.Equals(user.Password);
                if (!credentials)
                {
                    hrm = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    hrm = new HttpResponseMessage(HttpStatusCode.OK);
                }
                if (db.EncontrarToken(user.Email))
                {

                }
                else { string token = TokenManager.GenerateToken(user.Email); db.GuardarToken(token, user.Email); }
               
                
                FormsAuthentication.SetAuthCookie(user.Email, false); //crea variable de usuario 
                return RedirectToAction("Index", "Home");
            }
            hrm = new HttpResponseMessage(HttpStatusCode.NotFound);
            return View(user);
        }


        public ActionResult Encontrarmensaje(string userFriend, string userName)
        {
            MensajeModel mensajeabuscar = new MensajeModel();
            mensajeabuscar.userSender = userName;
            mensajeabuscar.userRecipient = userFriend;
            return View(mensajeabuscar);
        }

        [HttpPost]
        public ActionResult Encontrarmensaje(MensajeModel mensaje)
        {
            return RedirectToAction("MostrarMensajeEncontrado", mensaje);
        }


        
        public ActionResult MostrarMensajeEncontrado(MensajeModel mensaje)
        {
            IEnumerable<Models.MensajeModel> MensajesDB = new List<Models.MensajeModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/Messages/");
                //HTTP GET
                var responseTask = client.GetAsync("GetMensaje/" + mensaje.userSender + "/" + mensaje.userRecipient+"/" + mensaje.messageSent);
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


        DefaultConnection db = DefaultConnection.getInstance;
        UserModel Usuario = new UserModel();
        [HttpGet]
        public ActionResult CambiarContraseña(string userName)
        {
            if (Validate(userName))
            {
                User modificar = new User();
                modificar.userName = userName;
                modificar.Password = "";
                return View(modificar);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
           
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
        public ActionResult logIn2(Models.UserModel user)
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

        public ActionResult Delete(string userName, string userFriend)
        {
            if (Validate(userName))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:58142/api/Friends/");
                    string Email = userName;
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
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }




        public ActionResult ObtenerChats(string userName)
        {
            if (Validate(userName))
            {
                IEnumerable<Models.Chat> UsuariosDB = new List<Models.Chat>();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:58142/api/Chats/GetChat/");
                    //HTTP GET
                    var responseTask = client.GetAsync(userName);
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
            else
            {
                return RedirectToAction("Index", "Home");
            }
           
        }
        public ActionResult AddChat(string userName, string userFriend)
        {
            Chat nuevochat = new Chat();
            nuevochat.userRecipient = userFriend;
            nuevochat.userSender = userName;
            FriendModel fr = new FriendModel();
            fr.userName = userName;
            fr.userFriend = userFriend;
            if (Validate(userName))
            {
                    if(YatieneChat(userName, userFriend) != true)
                    {
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
                    }
               

                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
                return View("Index");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
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


        public ActionResult SeeFriends(string userName)
        {
            if (Validate(userName))
            {
                IEnumerable<Models.FriendModel> UsuariosDB = new List<Models.FriendModel>();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:58142/api/Friends/");
                    //HTTP GET
                    var responseTask = client.GetAsync(userName);
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
            else
            {
                return RedirectToAction("Index", "Home");
            }
           
        }
       

        public ActionResult AddFriend(string userName)
        {
            if (Validate(userName)) { 
            return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult AddFriend(Models.FriendModel us)
         {
            
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
            IEnumerable<Models.FriendModel> amigosDB = new List<Models.FriendModel>();
            //archivos2 = db.ObtenerLista();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/Friends/");
                //HTTP GET
                var responseTask = client.GetAsync(us.userName);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    amigosDB = JsonConvert.DeserializeObject<IList<FriendModel>>(readTask.Result);
                }
                else
                {
                    amigosDB = Enumerable.Empty<FriendModel>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            IEnumerable<Models.FriendModel> Query = from prod in amigosDB
                                             where prod.userName == us.userName
                                             select prod;

            foreach (Models.FriendModel item in Query)
            {
                if (item.userName == us.userName && item.userFriend == us.userFriend)  //Verificar password del usuario
                {
                    Isvalid = true;
                }
            }
            return Isvalid;
        }



        public bool YatieneChat(string userName, string userFriend)
        {
            bool Isvalid = false;
            IEnumerable<Models.Chat> amigosDB = new List<Models.Chat>();
            //archivos2 = db.ObtenerLista();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58142/api/Chats/");
                //HTTP GET
                var responseTask = client.GetAsync("GetChat/"+userName);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    amigosDB = JsonConvert.DeserializeObject<IList<Chat>>(readTask.Result);
                }
                else
                {
                    amigosDB = Enumerable.Empty<Chat>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            IEnumerable<Models.Chat> Query = from prod in amigosDB
                                             where prod.userSender == userName
                                             select prod;

            foreach (Models.Chat item in Query)
            {
                if (item.userSender == userName && item.userRecipient == userFriend)  //Verificar password del usuario
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
