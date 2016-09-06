using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{/// <summary>
/// This class is in charge of generating a SHA-3 hash and a 16 character salt for a supplied value
/// </summary>
    class PasswordManagment
    {
        /// <summary>
        /// Constructor to set up variables such as the random number generator
        /// </summary>
        public PasswordManagment()
        {


        }

        /// <summary>
        /// Constructor to set up variables such as the random number generator using a seeded number
        /// </summary>
        public PasswordManagment(int seededRandom)
        {

        }

        /// <summary>
        /// Generates a SHA-3 hash using the supplied variables
        /// </summary>
        /// <param name="password">the password to hash</param>
        /// <param name="salt">the salt that will be appended</param>
        /// <returns></returns>
        public byte[] GenerateHash(String password, String salt)
        {
            return null;
        }

        /// <summary>
        /// Generates a 16 character salt
        /// </summary>
        /// <returns></returns>
        public string GenerateSalt()
        {
            return null;
        }
    }
}
