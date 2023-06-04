using System;

namespace lab2_2
{
    class Program
    {
        static void Main()
        {
            byte[] key = { 0x10, 0x00, 0x00, 0x08 };
            byte[] plaintext = { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };

            RC5 rc5 = new RC5(key);
            byte[] ciphertext = rc5.Encrypt(plaintext);

            Console.WriteLine("Plaintext: " + BitConverter.ToString(plaintext));
            Console.WriteLine("Ciphertext: " + BitConverter.ToString(ciphertext));

            byte[] decryptedText = rc5.Decrypt(ciphertext);
            Console.WriteLine("Decrypted Text: " + BitConverter.ToString(decryptedText));
        }
    }
}