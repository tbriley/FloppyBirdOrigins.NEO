using System;
using System.Numerics;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace FloppyBirdOrigins.NEO
{
    public class CharacterContract : SmartContract
    {
        public static byte[] Main(string operation, object[] args)
        {
            var characterID = (string)args[0];
            if (operation.Equals("get"))
            {
                byte[] data = Storage.Get(Storage.CurrentContext, characterID);
                Runtime.Notify("CHARACTER_" + characterID, data.AsString());
                return data;
            }
            else if(operation.Equals("transfer"))
            {
                return new BigInteger(0).AsByteArray();
            }
            return new BigInteger(0).AsByteArray();
        }
    }
}
