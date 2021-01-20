﻿using BungeeCore.Common.Attributes;
using BungeeCore.Model.ClientBound;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace BungeeCore.Service
{
    [PacketHandler(PakcetId = 1)]
    public class EncryptionService : IService
    {
        public Type PacketTypes { get; private set; }
        private readonly ILogger Logger;
        private ICryptoTransform EncryptorTransform;
        private ICryptoTransform DecryptorTransform;
        public EncryptionService(ILogger<EncryptionService> Logger)
        {
            this.Logger = Logger;
            PacketTypes = typeof(EncryptionRequest);
        }
        public IEnumerable<bool> Handler(object obj)
        {
            EncryptionRequest encryptionRequest = (EncryptionRequest)obj;
            yield return true;
        }
        public void SetEncryption(byte[] key, byte[] iv)
        {
            RijndaelManaged RijndAes = new RijndaelManaged();
            RijndAes.Mode = CipherMode.CFB;
            RijndAes.BlockSize = 128;
            RijndAes.Key = key;
            RijndAes.IV = iv;
            EncryptorTransform = RijndAes.CreateEncryptor();
            DecryptorTransform = RijndAes.CreateDecryptor();
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] data)
        {
            byte[] resultArr = EncryptorTransform.TransformFinalBlock(data, 0, data.Length);
            return resultArr;
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">待解密数据</param>
        /// <param name="key">密钥</param>
        /// <returns>未加密原文数据</returns>
        public byte[] Decrypt(byte[] data)
        {
            byte[] resultArr = DecryptorTransform.TransformFinalBlock(data, 0, data.Length);
            return resultArr;
        }
    }
}
