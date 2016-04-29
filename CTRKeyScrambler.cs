using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CTR
{            
    public class KeyScrambler
    {

    	// Constant redacted for legality reasons. Insert yourself to make class functional. 
    	public static readonly byte[] SecretConstant = new byte[] { };	
    
        public static byte[] GetNormalKey(byte[] KeyX, byte[] KeyY)
        {
            if (KeyX.Length != 0x10 || KeyY.Length != 0x10)
                throw new ArgumentException("Invalid Key Length");   
            
            BigInteger Key = ToUnsignedBigInteger(XOR(RotateLeft(KeyX, 2), KeyY));
            BigInteger C = ToUnsignedBigInteger(SecretConstant);
            
            byte[] NormalKey = BigInteger.Add(Key, C).ToByteArray();
            
            if (BitConverter.IsLittleEndian)
            {
                if (NormalKey.Length > 0x10)
                    NormalKey = NormalKey.Take(0x10).ToArray();
                Array.Reverse(NormalKey);
            }
            
            return RotateLeft(NormalKey, 87);
        }
                                                                           
        private static byte[] XOR(byte[] arr1, byte[] arr2)
        {
            if (arr1.Length != arr2.Length)
                throw new ArgumentException("Array Lengths must be equal.");
            byte[] xored = new byte[arr1.Length];
            for (int i=0;i<arr1.Length;i++)
            {
                xored[i] = (byte)(arr1[i] ^ arr2[i]);    
            }
            return xored;
        }
        
        private static BigInteger ToUnsignedBigInteger(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
                return new BigInteger(data.Reverse().Concat(new byte[] { 0 }).ToArray());
            else
                return new BigInteger(new byte[] { 0 }.Concat(data).ToArray());
        }
             
        private static byte[] RotateLeft(byte[] input, int shift)
        {
            int N = (input.Length * 8) - (shift % (input.Length * 8));
            List<int> bits = new List<int>();
            byte[] output = new byte[input.Length];
            foreach (byte b in input.Reverse())
            {
                for (int i=0;i<8;i++)
                {
                    bits.Add((b >> i) & 1);   
                }
            }
            bits = bits.Skip(N).Concat(bits.Take(N)).ToList();
            for (int i=0;i<output.Length;i++)
            {
                for (int j=0;j<8;j++)
                {
                    output[i] |= (byte)(bits[i*8+j] << j);  
                }
            }
            output = output.Reverse().ToArray();
            return output;
        }
        
        private static byte[] RotateRight(byte[] input, int shift)
        {
            int N = shift % (input.Length * 8);
            List<int> bits = new List<int>();
            byte[] output = new byte[input.Length];
            foreach (byte b in input.Reverse())
            {
                for (int i=0;i<8;i++)
                {
                    bits.Add((b >> i) & 1);  
                }
            }
            bits = bits.Skip(N).Concat(bits.Take(N)).ToList();
            for (int i=0;i<output.Length;i++)
            {
                for (int j=0;j<8;j++)
                {
                    output[i] |= (byte)(bits[i*8+j] << j);  
                }
            }
            output = output.Reverse().ToArray();
            return output;
        }
    }
}
