using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metodos
{
    public class Cifrado
    {
        private int[] P10;
        private int[] P8;
        private int[] P4;
        private int[] EP;
        private int[] IP;
        private int[] Swap;
        private int[] IP1;
        private string[,] S0;
        private string[,] S1;
        private int[] Key1;
        private int[] Key2;

        public Cifrado()
        {
            P10 = new int[10];
            P8 = new int[8];
            P4 = new int[4];
            EP = new int[8];
            IP = new int[8];
            Swap = new int[8];
            IP1 = new int[8];
            S0 = new string[4, 4];
            S1 = new string[4, 4];
            Key1 = new int[8];
            Key2 = new int[8];
        }

        public Cifrado(int uno)
        {
            P10 = new int[] { 8, 5, 3, 7, 9, 2, 6, 0, 1, 4 };
            P8 = new int[] { 7, 9, 3, 5, 8, 2, 1, 6 };
            P4 = new int[] { 0, 3, 2, 1 };
            EP = new int[] { 0, 1, 3, 2, 3, 2, 1, 0 };
            IP = new int[] { 6, 3, 5, 7, 2, 0, 1, 4 };
            Swap = new int[] { 4, 5, 6, 7, 0, 1, 2, 3 };
            IP1 = new int[] { 5, 6, 4, 1, 7, 2, 0, 3 };
            S0 = new string[,] { { "01", "00", "11", "10" }, { "11", "10", "01", "00" }, { "00", "10", "01", "11" }, { "11", "01", "11", "10" } };
            S1 = new string[,] { { "00", "01", "10", "11" }, { "10", "00", "01", "11" }, { "11", "00", "01", "00" }, { "10", "01", "00", "11" } };
            Key1 = new int[8];
            Key2 = new int[8];
        }


        public int GenerarNumeroCifrado(string value)
        {
            int numeroCifrado = default(int);

            //foreach (var item in value.ToArray())
            //{
            //    numeroCifrado += (int)item;
            //}

            numeroCifrado = value.GetHashCode();
            numeroCifrado = (numeroCifrado % 1024);
            numeroCifrado = Math.Abs(numeroCifrado);

            return numeroCifrado;
        }

        public int GenerarNumeroCifrado(string value, string value2)
        {
            int numeroCifrado = default(int);

            //foreach (var item in value.ToArray())
            //{
            //    numeroCifrado += (int)item;
            //}

            var num1 = value.GetHashCode();
            var num2 = value.GetHashCode();

            num1 = (num1 % 1024);
            num2 = (num2 % 1024);

            numeroCifrado = num1 + num2;
            numeroCifrado = (numeroCifrado % 1024);

            numeroCifrado = Math.Abs(numeroCifrado);

            return numeroCifrado;
        }

        public string LeerArchivo(string textoObtenido, int numbertoCipher, bool operacion)
        {
            GenerarKeys(numbertoCipher);

            int bufferLength = 10;
            StringBuilder builder = new StringBuilder();
            int auxIndex = 0;
            var buffer = new char[bufferLength];

            for (int i = 0; i < textoObtenido.ToString().Length; i++)
            {
                if ((textoObtenido.ToString().Length - i) >= 10)
                {
                    buffer = textoObtenido.ToString().Substring(i, 10).ToCharArray();
                    i += 9;
                }
                else
                {
                    buffer = textoObtenido.ToString().Substring(i, (textoObtenido.ToString().Length - i)).ToCharArray();
                    i += (textoObtenido.ToString().Length - i);
                }

                if (operacion)
                {
                    foreach (var item in buffer)
                    {
                        builder.Append(Cipher(item));
                    }
                }
                else
                {
                    foreach (var item in buffer)
                    {
                        builder.Append(Descrifrado(item));
                    }
                }
            }

            //using (var file = new FileStream(db.ObtenerRuta().FullName, FileMode.Open))
            //using (var file = new TextReader())
            //{
            //    using (var reader = new BinaryReader(file))
            //    {
            //        while (reader.BaseStream.Position != reader.BaseStream.Length)
            //        {
            //            buffer = reader.ReadChars(bufferLength);
            //            foreach (var item in buffer)
            //            {
            //                builder.Append(cifradoSDes.Cifrado((char)item).ToString());
            //            }
            //        }

            //    }

            //}

            return builder.ToString();
        }

        private char Cipher(char num_caracter)
        {
            var caracter = GenerarCaracterInicial((int)num_caracter);
            var caracterIP = ObtenerIP(caracter);
            var caracter_p1 = new int[] { caracterIP[0], caracterIP[1], caracterIP[2], caracterIP[3] };
            var caracter_p2 = new int[] { caracterIP[4], caracterIP[5], caracterIP[6], caracterIP[7] };
            var caracter2EP = ObtenerEP(caracter_p2);
            var caracterXORK1 = ObtenerXORKey(caracter2EP, Key1);
            var caracterSBox = ObtenerSBox(caracterXORK1);
            var caracterP4 = ObtenerP4(caracterSBox);
            var caracterXORP = ObtenerXORP1(caracterP4, caracter_p1);
            var caracterUnion = Union(caracterXORP, caracter_p2);
            var caracterSwap = ObtenerSwap(caracterUnion);
            caracter_p1 = new int[] { caracterSwap[0], caracterSwap[1], caracterSwap[2], caracterSwap[3] };
            caracter_p2 = new int[] { caracterSwap[4], caracterSwap[5], caracterSwap[6], caracterSwap[7] };
            caracter2EP = ObtenerEP(caracter_p2);
            var caracterXORK2 = ObtenerXORKey(caracter2EP, Key2);
            caracterSBox = ObtenerSBox(caracterXORK2);
            caracterP4 = ObtenerP4(caracterSBox);
            caracterXORP = ObtenerXORP1(caracterP4, caracter_p1);
            caracterUnion = Union(caracterXORP, caracter_p2);
            var caractercifrado = ObtenerIP1(caracterUnion);

            return ObtenerCaracterCifrado(caractercifrado);
        }

        private char ObtenerCaracterCifrado(int[] caracter_cifrado)
        {
            int inicio = 128;
            int caracter = 0;

            for (int i = 0; i < 8; i++)
            {
                caracter += (inicio * caracter_cifrado[i]);
                inicio = inicio / 2;
            }

            return (char)caracter;
        }

        private int[] ObtenerIP(int[] caracter_inicial)
        {
            int[] aux = new int[8];
            for (int i = 0; i < 8; i++)
            {
                aux[i] = caracter_inicial[IP[i]];
            }

            return aux;
        }

        private int[] ObtenerIP1(int[] caracter_union)
        {
            int[] aux = new int[8];
            for (int i = 0; i < 8; i++)
            {
                aux[i] = caracter_union[IP1[i]];
            }

            return aux;
        }

        private int[] ObtenerSwap(int[] caracter_union)
        {
            int[] aux = new int[8];
            for (int i = 0; i < 8; i++)
            {
                aux[i] = caracter_union[Swap[i]];
            }

            return aux;
        }

        private int[] Union(int[] caracter_XORP, int[] caracterp2)
        {
            int[] aux = new int[8];
            for (int i = 0; i < 8; i++)
            {
                if (i < 4)
                {
                    aux[i] = caracter_XORP[i];
                }
                else
                {
                    aux[i] = caracterp2[i - 4];
                }
            }

            return aux;
        }

        private int[] ObtenerXORP1(int[] caracter_p4, int[] caracterp2)
        {
            int[] aux = new int[4];
            for (int i = 0; i < 4; i++)
            {
                if (caracter_p4[i] == caracterp2[i])
                {
                    aux[i] = 0;
                }
                else
                {
                    aux[i] = 1;
                }
            }

            return aux;
        }

        private int[] ObtenerP4(int[] caracter_SBox)
        {
            int[] aux = new int[4];
            for (int i = 0; i < 4; i++)
            {
                aux[i] = caracter_SBox[P4[i]];
            }

            return aux;
        }

        private int[] ObtenerSBox(int[] caracter_XORK1)
        {
            int filaS0 = (2 * caracter_XORK1[0]) + caracter_XORK1[3];
            int columnaS0 = (2 * caracter_XORK1[1]) + caracter_XORK1[2];
            int filaS1 = (2 * caracter_XORK1[4]) + caracter_XORK1[7];
            int columnaS1 = (2 * caracter_XORK1[5]) + caracter_XORK1[6];
            string temp = S0[filaS0, columnaS0] + S1[filaS1, columnaS1];
            int[] aux = new int[4];
            for (int i = 0; i < temp.Length; i++)
            {
                aux[i] = int.Parse(temp.Substring(i, 1));
            }

            return aux;
        }

        private int[] ObtenerEP(int[] caracter_IP)
        {
            int[] aux = new int[8];
            for (int i = 0; i < 8; i++)
            {
                aux[i] = caracter_IP[EP[i]];
            }

            return aux;
        }

        private int[] ObtenerXORKey(int[] caracterEP, int[] key)
        {
            int[] aux = new int[8];
            for (int i = 0; i < 8; i++)
            {
                if (caracterEP[i] == key[i])
                {
                    aux[i] = 0;
                }
                else
                {
                    aux[i] = 1;
                }
            }

            return aux;
        }

        private int[] GenerarCaracterInicial(int caracterInicial)
        {
            var binario = Convert.ToString(caracterInicial, 2);
            int[] aux = new int[8];
            int contador = 7;

            for (int i = binario.Length - 1; i >= 0; i--)
            {
                aux[contador] = int.Parse(binario.Substring(i, 1));
                contador--;
            }

            return aux;
        }
        /// <summary>
        /// //////////////////////////////////////////////////////
        /// </summary>
        private char Descrifrado(char num_caracter)
        {
            var caracter = GenerarCaracterInicial((int)num_caracter);
            var caracterIP = ObtenerIP(caracter);
            var caracter_p1 = new int[] { caracterIP[0], caracterIP[1], caracterIP[2], caracterIP[3] };
            var caracter_p2 = new int[] { caracterIP[4], caracterIP[5], caracterIP[6], caracterIP[7] };
            var caracter2EP = ObtenerEP(caracter_p2);
            var caracterXORK1 = ObtenerXORKey(caracter2EP, Key2);
            var caracterSBox = ObtenerSBox(caracterXORK1);
            var caracterP4 = ObtenerP4(caracterSBox);
            var caracterXORP = ObtenerXORP1(caracterP4, caracter_p1);
            var caracterUnion = Union(caracterXORP, caracter_p2);
            var caracterSwap = ObtenerSwap(caracterUnion);
            caracter_p1 = new int[] { caracterSwap[0], caracterSwap[1], caracterSwap[2], caracterSwap[3] };
            caracter_p2 = new int[] { caracterSwap[4], caracterSwap[5], caracterSwap[6], caracterSwap[7] };
            caracter2EP = ObtenerEP(caracter_p2);
            var caracterXORK2 = ObtenerXORKey(caracter2EP, Key1);
            caracterSBox = ObtenerSBox(caracterXORK2);
            caracterP4 = ObtenerP4(caracterSBox);
            caracterXORP = ObtenerXORP1(caracterP4, caracter_p1);
            caracterUnion = Union(caracterXORP, caracter_p2);
            var caractercifrado = ObtenerIP1(caracterUnion);

            return ObtenerCaracterCifrado(caractercifrado);
        }
        /// <summary>
        /// ///////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="num"></param>
        public void GenerarKeys(int num)
        {
            var key = GenerarKeyInicial(num);
            var keyP10 = ObtenerP10(key);
            var KeyLS1 = ObtenerLS1(keyP10);
            Key1 = ObtenerP8(KeyLS1);
            var KeyLS2 = ObtenerLS2(KeyLS1);
            Key2 = ObtenerP8(KeyLS2);
        }


        private int[] GenerarKeyInicial(int keyInicial)
        {
            var binario = Convert.ToString(keyInicial, 2);
            int[] aux = new int[10];
            int contador = 9;

            for (int i = binario.Length - 1; i >= 0; i--)
            {
                aux[contador] = int.Parse(binario.Substring(i, 1));
                contador--;
            }

            return aux;
        }

        private int[] ObtenerP10(int[] key_inicial)
        {
            int[] aux = new int[10];
            for (int i = 0; i < 10; i++)
            {
                aux[i] = key_inicial[P10[i]];
            }

            return aux;
        }

        private int[] ObtenerLS1(int[] key_p10)
        {
            int[] aux = new int[10];
            int n1 = key_p10[0];
            int n2 = key_p10[5];
            for (int i = 0; i < 9; i++)
            {
                aux[i] = key_p10[i + 1];
            }

            aux[4] = n1;
            aux[9] = n2;

            return aux;
        }

        private int[] ObtenerP8(int[] key_LS1)
        {
            int[] aux = new int[8];
            for (int i = 0; i < 8; i++)
            {
                aux[i] = key_LS1[P8[i]];
            }

            return aux;
        }

        private int[] ObtenerLS2(int[] key_LS1)
        {
            int[] aux = new int[10];
            int n1 = key_LS1[0];
            int n2 = key_LS1[1];
            int n3 = key_LS1[5];
            int n4 = key_LS1[6];
            for (int i = 0; i < 8; i++)
            {
                aux[i] = key_LS1[i + 2];
            }

            aux[3] = n1;
            aux[4] = n2;

            aux[8] = n3;
            aux[9] = n4;

            return aux;
        }

        private void LeerPermutaciones()
        {
        }
    }
}
