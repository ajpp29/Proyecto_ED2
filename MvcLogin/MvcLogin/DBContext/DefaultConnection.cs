using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MvcLogin.Models;

namespace MvcLogin.DBContext
{
    public class DefaultConnection
    {
        private static volatile DefaultConnection Instance;
        private static object syncRoot = new Object();
        public UserModel UsuarioLoggeado = new UserModel();
        public List<Models.UserModel> Usuarios = new List<Models.UserModel>();
        public int Email { get; set; }
        public int Contraseña { get; set; }

        public List<Models.MensajeModel> ObtenerMensajes()
        {
            return UsuarioLoggeado.mensajes;

        }

        public void eliminarAmigo(string user)
        {
            FriendModel usEliminar = new FriendModel();
            usEliminar.Email = user;
            for (int i = 0; i < UsuarioLoggeado.friends.Count(); i++)
            {
                if (UsuarioLoggeado.friends[i].Email == user)
                {
                    UsuarioLoggeado.friends.Remove((UsuarioLoggeado.friends[i]));
                }
               
            }
           
        }
        public void addmensaje(MensajeModel mensaje)
        {
            UsuarioLoggeado.AddMensaje(mensaje);
        }
        public void AddFriend(Models.FriendModel chat)
        {
            UsuarioLoggeado.AddFriend(chat);
        }

        public List<Models.FriendModel> ObtenerFriends()
        {
            return UsuarioLoggeado.friends;

        }

        public List<Models.FriendModel> ObtenerChats()
        {
            return UsuarioLoggeado.chats;

        }
        public void  AddChat(FriendModel usuario)
        {
            UsuarioLoggeado.AddChat(usuario);
        }

        public void AddUsuario(Models.UserModel user)
        {
            Usuarios.Add(user);
        }
        private static FileInfo fileInfo = default(FileInfo);

        public int IdActual { get; set; }

        public DefaultConnection()
        {
            IdActual = 0;
        }
        public List<Models.UserModel> ObtenerLista()
        {
            return Usuarios;
        }
        public void AsignarRuta(FileInfo file)
        {
            fileInfo = file;
        }

        public FileInfo ObtenerRuta()
        {
            return fileInfo;
        }

        public static DefaultConnection getInstance
        {
            get
            {
                if (Instance == null)
                {
                    lock (syncRoot)
                    {
                        if (Instance == null)
                        {
                            Instance = new DefaultConnection();
                        }
                    }
                }
                return Instance;
            }
        }
    }
}