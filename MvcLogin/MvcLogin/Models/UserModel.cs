using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcLogin.Models
{

    public class UserModel
    {
        [Required] //Dato requerido
        [EmailAddress] //Validar que se ingrese un email valido
        [StringLength(150)] //longitud maxima del campo
        [Display(Name="Email address ")] //Mensaje indicar obligatorio
        public string Email { get; set; }

        [Required] //Dato requerido
        [DataType(DataType.Password)] //Indicar dato  tipo password
        [StringLength(20,MinimumLength=6)] //Longitud minima y maxima
        [Display(Name = "Password ")] //Mensaje indicar obligatorio
        public string Password { get; set; }

        public  List<FriendModel> friends = new List<FriendModel>();
        public List<FriendModel> chats = new List<FriendModel>();
        public List<MensajeModel> mensajes = new List<MensajeModel>();

        public void AddMensaje(MensajeModel mensaje)
        {
            mensajes.Add(mensaje);
        }
        public void AddChat(FriendModel Usuario)
        {
            chats.Add(Usuario);
        }

        public List<FriendModel> ObtenerChat(string Usuario)
        {
            return chats;
        }

        public void AddFriend(FriendModel Usuario)
        {
            friends.Add(Usuario);
        }

        public List<FriendModel> obtenerfriends()
        {
            return friends;
        }

        //public List<Models.MensajeModel> obtenerMensajes()
        //{
        //    return  ;
        //}
        //public void AddMensaje(string emisor, string receptor, string contenido)
        //{
        //    Models.MensajeModel msj = new MensajeModel();
        //    msj.Contenido = contenido;
        //    msj.emisor = emisor;
        //    msj.receptor = receptor;
        //    mensajes.Add(msj);

        //}
    }
}