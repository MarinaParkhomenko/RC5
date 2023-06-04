using System;

namespace lab2_2
{
    public class RC5
    {
        private const int WORD_SIZE = 32;
        private const int ROUNDS = 12;
        private const int KEY_SIZE = 16;
        private const int BLOCK_SIZE = 8;

        private uint[] S;

        public RC5(byte[] key)
        {
            InitializeKeySchedule(key);
        }

        private void InitializeKeySchedule(byte[] key)
        {
            int c = KEY_SIZE / (WORD_SIZE / 8); // Number of key bytes to words
            uint[] L = new uint[c];

            for (int m = 0; m < c; m++)
            {
                L[m] = 0;
            }

            for (int n = 0; n < key.Length; n++)
            {
                L[n / WORD_SIZE] = (L[n / WORD_SIZE] << 8) + key[n];
            }

            S = new uint[2 * (ROUNDS + 1)];
            S[0] = 0xB7E15163;

            for (int k = 1; k < S.Length; k++)
            {
                S[k] = S[k - 1] + 0x9E3779B9;
            }

            uint A = 0, B = 0;
            int i = 0, j = 0;

            for (int k = 0; k < 3 * S.Length; k++)
            {
                A = S[i] = RotateLeft(S[i] + (A + B), 3);
                B = L[j] = RotateLeft(L[j] + (A + B), (int)(A + B));

                i = (i + 1) % S.Length;
                j = (j + 1) % c;
            }
        }

        public byte[] Encrypt(byte[] plaintext)
        {
            if (plaintext.Length != BLOCK_SIZE)
            {
                throw new ArgumentException("Invalid block size");
            }

            uint A = BitConverter.ToUInt32(plaintext, 0);
            uint B = BitConverter.ToUInt32(plaintext, 4);

            A += S[0];
            B += S[1];

            for (int i = 1; i <= ROUNDS; i++)
            {
                A = RotateLeft(A ^ B, (int)B) + S[2 * i];
                B = RotateLeft(B ^ A, (int)A) + S[2 * i + 1];
            }

            byte[] ciphertext = new byte[BLOCK_SIZE];
            Buffer.BlockCopy(BitConverter.GetBytes(A), 0, ciphertext, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(B), 0, ciphertext, 4, 4);

            return ciphertext;
        }

        public byte[] Decrypt(byte[] ciphertext)
        {
            if (ciphertext.Length != BLOCK_SIZE)
            {
                throw new ArgumentException("Invalid block size");
            }

            uint A = BitConverter.ToUInt32(ciphertext, 0);
            uint B = BitConverter.ToUInt32(ciphertext, 4);

            for (int i = ROUNDS; i >= 1; i--)
            {
                B = RotateRight(B - S[2 * i + 1], (int)A) ^ A;
                A = RotateRight(A - S[2 * i], (int)B) ^ B;
            }

            B -= S[1];
            A -= S[0];

            byte[] plaintext = new byte[BLOCK_SIZE];
            Buffer.BlockCopy(BitConverter.GetBytes(A), 0, plaintext, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(B), 0, plaintext, 4, 4);

            return plaintext;
        }

        private uint RotateLeft(uint value, int shift)
        {
            return (value << shift) | (value >> (WORD_SIZE - shift));
        }

        private uint RotateRight(uint value, int shift)
        {
            return (value >> shift) | (value << (WORD_SIZE - shift));
        }
    }
}
