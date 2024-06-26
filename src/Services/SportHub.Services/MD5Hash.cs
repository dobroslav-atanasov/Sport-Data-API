﻿namespace SportHub.Services;

using System.Security.Cryptography;
using System.Text;

using SportHub.Services.Interfaces;

public class MD5Hash : IMD5Hash
{
    public string Hash(byte[] data)
    {
        using var md5 = MD5.Create();

        var hashBytes = md5.ComputeHash(data);

        var sb = new StringBuilder();
        for (var i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(hashBytes[i].ToString("X2"));
        }

        return sb.ToString();
    }
}