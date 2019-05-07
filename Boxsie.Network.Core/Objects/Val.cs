//using System;
//using System.Text;

//namespace Boxsie.Network.Core.Objects
//{
//    public class Guid : Val<Guid>
//    {
//        public Guid(Guid val, bool update = false) : base(val, update) { }
        
//        public static implicit operator Guid(Guid x)
//        {
//            return x.Val;
//        }

//        public static implicit operator Guid(Guid x)
//        {
//            return new Guid(x);
//        }

//        public static implicit operator byte[] (Guid x)
//        {
//            return Encoding.ASCII.GetBytes(Convert.ToBase64String(((Guid)x).ToByteArray()));
//        }

//        public static implicit operator Guid(byte[] x)
//        {
//            return new Guid(new Guid(Convert.FromBase64String(Encoding.ASCII.GetString(x))));
//        }
//    }

//    public class Bool : Val<bool>
//    {
//        public Bool(bool val, bool update = false) : base(val, update) { }

//        public static implicit operator bool(Bool x)
//        {
//            return x.Val;
//        }

//        public static implicit operator Bool(bool x)
//        {
//            return new Bool(x);
//        }

//        public static implicit operator byte[] (Bool x)
//        {
//            return new[] { Convert.ToByte(x) };
//        }

//        public static implicit operator Bool(byte[] x)
//        {
//            return new Bool(Convert.ToBoolean(x[0]));
//        }
//    }

//    public class String : Val<string>
//    {
//        public String(string val, bool update = false) : base(val, update) { }

//        public static implicit operator string(String x)
//        {
//            return x.Val;
//        }

//        public static implicit operator String(string x)
//        {
//            return new String(x);
//        }

//        public static implicit operator byte[] (String x)
//        {
//            return Encoding.ASCII.GetBytes(Convert.ToBase64String(Encoding.ASCII.GetBytes(x)));
//        }

//        public static implicit operator String(byte[] x)
//        {
//            return new String(Encoding.ASCII.GetString(Convert.FromBase64String(Encoding.ASCII.GetString(x))));
//        }
//    }

//    public class Val<T> : IVal
//    {
//        public bool NeedsUpdate { get; private set; }

//        public T Val
//        {
//            get { return _val; }
//            set
//            {
//                _val = value;
//                NeedsUpdate = true;
//            }
//        }

//        private T _val;

//        public Val(T val, bool update = false)
//        {
//            _val = val;
//            NeedsUpdate = update;
//        }
//    }
//}
