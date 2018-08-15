using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace FloppyBirdOrigins.NEO
{
    public class CharacterContract : SmartContract
    {
        private static BigInteger characterCount
        {
            get
            {
                return Storage.Get(Storage.CurrentContext, nameof(characterCount)).AsBigInteger();
            }
            set
            {
                Storage.Put(Storage.CurrentContext, nameof(characterCount), value);
            }
        }

        private const string ownerKey = "Owner";
        private const string idKey = "ID";
        private const string dataKey = "Data";

        // 0 == sender
        // 1 == id
        // 2 == data
        public static object Main(string operation, params object[] args)
        {
            switch (operation)
            {
                case "createCharacter":
                    return CreateCharacter((byte[])args[0], (string)args[1], (string)args[2]);

                case "getCharacter":
                    return GetCharacter((string)args[1]);

                case "transferCharacter":
                    return TransferCharacter((string)args[1], (byte[])args[0]);

                case "delete":
                    return Delete((string)args[1]);

                case "getCharactersForOwner":
                    return GetCharacterIDsForOwner((byte[])args[0]);

                default:
                    return false;
            }
        }

        private static bool CreateCharacter(byte[] owner, string id, string data)
        {
            if (!Runtime.CheckWitness(owner)) { return false; }

            byte[] value = Storage.Get(Storage.CurrentContext, id);
            if (value != null) { return false; }

            characterCount += 1;

            Storage.Put(Storage.CurrentContext, characterCount.AsByteArray().AsString() + idKey, id);
            Storage.Put(Storage.CurrentContext, id + ownerKey, owner);
            Storage.Put(Storage.CurrentContext, id + dataKey, data);

            Runtime.Notify("CREATED_CHARACTER_" + id, data);
            return true;
        }

        private static byte[] GetCharacter(string id)
        {
            return Storage.Get(Storage.CurrentContext, id + dataKey);
        }

        private static bool TransferCharacter(string id, byte[] targetOwner)
        {
            if (!Runtime.CheckWitness(targetOwner)) { return false; }

            byte[] currentOwner = Storage.Get(Storage.CurrentContext, id + ownerKey);
            if (currentOwner == null) { return false; }

            if (!Runtime.CheckWitness(currentOwner)) { return false; }

            Storage.Put(Storage.CurrentContext, id + ownerKey, targetOwner);

            Runtime.Notify("TRANSFERRED_CHARACTER_" + id);

            return true;
        }

        private static bool Delete(string id)
        {
            byte[] owner = Storage.Get(Storage.CurrentContext, id + ownerKey);
            if (owner == null) { return false; }

            if (!Runtime.CheckWitness(owner)) { return false; }

            Storage.Delete(Storage.CurrentContext, characterCount.AsByteArray().AsString() + idKey);
            Storage.Delete(Storage.CurrentContext, id + ownerKey);
            Storage.Delete(Storage.CurrentContext, id + dataKey);

            characterCount -= 1;

            return true;
        }

        private static object GetCharacterIDsForOwner(byte[] targetOwner)
        {
            var datas = new string[100];
            for (var i = 0; i < characterCount; i++)
            {
                var bigI = (BigInteger)i;
                var id = Storage.Get(Storage.CurrentContext, bigI.AsByteArray().AsString() + idKey).AsString();
                var data = Storage.Get(Storage.CurrentContext, id + dataKey);
                var owner = Storage.Get(Storage.CurrentContext, id + ownerKey);

                if(owner == targetOwner)
                {
                    datas[(int)i] = id;
                }
            }
            return datas;
        }
    }
}
