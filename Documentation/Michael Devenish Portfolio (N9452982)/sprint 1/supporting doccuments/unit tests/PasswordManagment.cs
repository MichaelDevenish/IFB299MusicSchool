using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{/// <summary>
/// This class is in charge of generating a SHA-3 hash and a 16 character salt for a supplied value
/// </summary>
    public class PasswordManagment
    {
        Random random;
        private static int SALT_LENGTH = 16;

        /// <summary>
        /// Constructor to set up variables such as the random number generator
        /// </summary>
        public PasswordManagment()
        {
            random = new Random();
        }

        /// <summary>
        /// Constructor to set up variables such as the random number generator using a seeded number
        /// </summary>
        public PasswordManagment(int seededRandom)
        {
            random = new Random(seededRandom);
        }

        /// <summary>
        /// Generates a SHA-3 hash using the supplied variables
        /// </summary>
        /// <param name="password">the password to hash</param>
        /// <param name="salt">the salt that will be appended</param>
        /// <returns></returns>
        public byte[] GenerateHash(string password, string salt)
        {
            string plaintext = password + salt;
            System.Security.Cryptography.SHA512Managed crypt = new System.Security.Cryptography.SHA512Managed();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(plaintext), 0, Encoding.UTF8.GetByteCount(plaintext));
            return crypto;
        }

        /// <summary>
        /// Generates a 16 character salt
        /// </summary>
        /// <returns></returns>
        public string GenerateSalt()
        {
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder(SALT_LENGTH);

            for (int i = 0; i < SALT_LENGTH; i++)
                result.Append(characters[random.Next(characters.Length)]);

            return result.ToString();
        }
    }
}