using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;


namespace WebNoticias.Helpers
{
    public static class Encriptacion
    {
        public static string StringToSHA512(string s)
        {
            using (var sha512 = SHA512.Create()) //Using: acelera la destruccion en memoria
            {
                var arreglo = Encoding.UTF8.GetBytes(s); //Lo convierte en bytes
                var hash = sha512.ComputeHash(arreglo);

                return Convert.ToHexString(hash).ToLower();
            }

        }

    }
}
