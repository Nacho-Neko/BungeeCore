using BungeeCore.Common.Attributes;
using BungeeCore.Model.ClientBound;
using BungeeCore.Model.ServerBound;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace BungeeCore.Service
{
    [PacketHandler(PakcetId = 1, Rose = Rose.Player)]
    public class EncryptionService : BaseService
    {
        public override Type PacketTypes { get; protected set; } = typeof(EncryptionRequest);
        public override object Parameter { protected get; set; }
        private readonly ILogger Logger;
        private readonly InfoService infoService;

        private ICryptoTransform EncryptorTransform;
        private ICryptoTransform DecryptorTransform;

        private EncryptionResponse encryptionResponse;
        private EncryptionRequest encryptionRequest;

        public EncryptionService(ILogger<EncryptionService> Logger, InfoService infoService)
        {
            this.Logger = Logger;
            this.infoService = infoService;
        }
        public override IEnumerable<bool> Prerouting()
        {
            encryptionResponse = (EncryptionResponse)Parameter;
            SetEncryption(encryptionResponse.SharedSecret, encryptionResponse.VerifyToken);
            yield return true;
        }
        public override IEnumerable<bool> Postrouting()
        {
            infoService.Encryption = true;
            encryptionRequest = (EncryptionRequest)Parameter;
            SetDecryption(encryptionRequest.PublicKey, encryptionRequest.VerifyToken);
            yield return true;
        }
        /// <summary>
        /// 设置加密参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        private void SetEncryption(byte[] key, byte[] iv)
        {
            RijndaelManaged RijndAes = new RijndaelManaged();
            RijndAes.Mode = CipherMode.CFB;
            RijndAes.BlockSize = 128;
            RijndAes.Key = key;
            RijndAes.IV = iv;
            DecryptorTransform = RijndAes.CreateDecryptor();
        }
        /// <summary>
        /// 设置解密参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        private void SetDecryption(byte[] key, byte[] iv)
        {
            RijndaelManaged RijndAes = new RijndaelManaged();
            RijndAes.Mode = CipherMode.CFB;
            RijndAes.BlockSize = 128;
            RijndAes.Key = key;
            RijndAes.IV = iv;
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

