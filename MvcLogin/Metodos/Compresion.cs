using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metodos
{
    public class Compresion
    {
        public List<int> Compresionn(string Descomprimido)
        {
            // Crando Diccionario
            List<int> indices = new List<int>();
            Dictionary<int, string> dictionary = new Dictionary<int, string>();

            for (int i = 0; i < 256; i++)
                dictionary.Add(i, new string((char)i, 1));

            char c = '\0';
            int index = 1, n = Descomprimido.Length, nextKey = 256;
            string s = new string(Descomprimido[0], 1), sc = string.Empty;

            while (index < n)
            {
                c = Descomprimido[index++];
                sc = s + c;

                if (dictionary.ContainsValue(sc))
                    s = sc;

                else
                {
                    foreach (KeyValuePair<int, string> kvp in dictionary)
                    {
                        if (kvp.Value == s)
                        {
                            indices.Add(kvp.Key);
                            break;
                        }
                    }

                    dictionary.Add(nextKey++, sc);
                    s = new string(c, 1);
                }
            }

            foreach (KeyValuePair<int, string> kvp in dictionary)
            {
                if (kvp.Value == s)
                {
                    indices.Add(kvp.Key);
                    break;
                }
            }

            return indices;
        }

        public string Descompresion(List<int> Comprimido)
        {
            // Crando Diccionario
            Dictionary<int, string> Diccionario = new Dictionary<int, string>();
            for (int i = 0; i < 256; i++)
                Diccionario.Add(i, ((char)i).ToString());

            string w = Diccionario[Comprimido[0]];
            Comprimido.RemoveAt(0);
            StringBuilder Descomprimido = new StringBuilder(w);

            foreach (int k in Comprimido)
            {
                string entry = null;
                if (Diccionario.ContainsKey(k))
                    entry = Diccionario[k];
                else if (k == Diccionario.Count)
                    entry = w + w[0];

                Descomprimido.Append(entry);

                // Agregando al diccionario la entrada
                Diccionario.Add(Diccionario.Count, w + entry[0]);
                w = entry;
            }

            return Descomprimido.ToString();
        }
    }
}
