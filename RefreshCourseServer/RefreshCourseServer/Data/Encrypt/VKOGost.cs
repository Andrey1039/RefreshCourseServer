using System.Numerics;
using System.Security;
using System.Security.Cryptography;

namespace RefreshCourseServer.Data.Encrypt
{
    public class VKOGost
    {
        // Значения параметров для эллиптических кривых
        private static readonly BigInteger p = BigInteger.Parse(
                   "57896044618658097711785492504343953926634992332820282019728792003956564821041");
        private static readonly BigInteger a = BigInteger.Parse("7");
        private static readonly BigInteger b = BigInteger.Parse(
            "43308876546767276905765904595650931995942111794451039583252968842033849580414");
        private static readonly BigInteger q = BigInteger.Parse(
            "57896044618658097711785492504343953927082934583725450622380973592137631069619");
        private static readonly BigInteger xP = BigInteger.Parse("2");
        private static readonly BigInteger yP = BigInteger.Parse(
            "4018974056539037503335449422937059775635739389905545080690979365213431566280");

        // Точка генератор
        private static readonly EllipticPoint genPoint = new EllipticPoint(a, b, p, xP, yP);       

        // Соединение координат точек на эллиптической кривой
        private static byte[] ConcatCoords(EllipticPoint yP)
        {
            byte[] xPoint = yP.X.ToByteArray().Reverse().ToArray();
            byte[] yPoint = yP.Y.ToByteArray().Reverse().ToArray();

            return xPoint.Concat(yPoint).ToArray();
        }

        // Генерация приватного ключа
        private static BigInteger GenerateKey()
        {
            byte[] xRaw = new byte[32];

            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(xRaw);

            BigInteger key = new BigInteger(xRaw);

            if (key == 0)
                key = q - 1;

            key = Mathematics.Mod(key, q);

            return key;
        }

        // Генерация публичного и приватного ключей
        [SecurityCritical]
        public static (string, string) GetKeyPair()
        {
            BigInteger privKey = GenerateKey();
            EllipticPoint pubKey = privKey * genPoint;

            return (privKey.ToString(), pubKey.ToString());
        }

        // Версия алгоритма согласования ключей на 256 бит
        private static byte[] VKO_GOSTR3410_2012_256(EllipticPoint yP, BigInteger m, BigInteger q, BigInteger x)
        {
            BigInteger UKM = BigInteger.Parse("5398423985475523801");
            EllipticPoint kPoint = Mathematics.Mod((Mathematics.ExtEuclidian(m, q) * UKM * x), q) * yP;
            byte[] sumPoint = ConcatCoords(kPoint);

            return Streebog.GetHash256(sumPoint);
        }

        // Генерация хэша
        private static string GenerateHash(BigInteger privKey, BigInteger publicKeyX,
            BigInteger publicKeyY, BigInteger a, BigInteger b, BigInteger p, BigInteger q)
        {
            EllipticPoint point = new EllipticPoint(a, b, p, publicKeyX, publicKeyY);
            string hash = Convert.ToHexString(VKO_GOSTR3410_2012_256(point, p, q, privKey));

            return hash;
        }

        // Получение хэша (общего серкетного ключа)
        [SecurityCritical]
        public static string GetHash(string privKeyStr, string pubKeyStr)
        {
            BigInteger privKey = BigInteger.Parse(privKeyStr);
            BigInteger x = BigInteger.Parse(pubKeyStr.Split("-")[0]);
            BigInteger y = BigInteger.Parse(pubKeyStr.Split("-")[1]);

            return GenerateHash(privKey, x, y, a, b, p, q);
        }
    }
}