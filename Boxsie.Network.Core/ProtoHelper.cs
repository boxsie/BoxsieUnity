using System;
using System.IO;
using ProtoBuf;

namespace Boxsie.Network.Core
{
    public static class ProtoHelper
    {
        public static T ProtoDeserialise<T>(this byte[] message)
        {
            using (var ms = new MemoryStream(message))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }

        public static byte[] ProtoSerialise(this object obj)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}