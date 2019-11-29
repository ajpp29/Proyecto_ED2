using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Metodos;
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
        public void EliminarChat(string user)
        {
            for (int i = 0; i < UsuarioLoggeado.friends.Count(); i++)
            {
                if (UsuarioLoggeado.chats[i].Email == user)
                {
                    UsuarioLoggeado.chats.Remove((UsuarioLoggeado.friends[i]));
                }

            }

        }


        public void GenerarArhivoComprimido(HttpPostedFileBase File, string fullname, string path, string path2)
        {
            string FULLNAME = path + fullname;
            const int bufferLength = 100;
            List<int> bytedecompress = new List<int>();
            StringBuilder builder = new StringBuilder();

            var buffer = new byte[bufferLength];
            using (var file = new FileStream(FULLNAME, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLength);
                        foreach (var item in buffer)
                        {
                            builder.Append(((char)item).ToString());
                        }

                        //Console.ReadKey();
                    }

                }

            }

            byte[] filedata = System.IO.File.ReadAllBytes(FULLNAME);
            Compresion algoritmo = new Compresion();
            List<int> compreso = algoritmo.Compresionn(builder.ToString());

            List<char> bytecompress = new List<char>();

            foreach (int numero in compreso)
            {
                bytecompress.Add((char)numero);
            }

            ////////
            var ruta = path2 + fullname.Split('.')[0] + ".lzw";
            using (StreamWriter outputFile = new StreamWriter(ruta))
            {
                foreach (char caracter in bytecompress)
                {
                    outputFile.Write(caracter.ToString());
                }
            }
        }
        public void eliminarAmigo(string user)
        {
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

        public int IdActual { get; set; }

        public DefaultConnection()
        {
            IdActual = 0;
        }
        public List<Models.UserModel> ObtenerLista()
        {
            return Usuarios;
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