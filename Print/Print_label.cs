using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.InteropServices;

namespace Print
{
    public class Print_label
    {
        const int _RC_NEAR = 0x00000000;
        const int _PC_53 = 0x00010000;
        const int _EM_INVALID = 0x00000010;
        const int _EM_UNDERFLOW = 0x00000002;
        const int _EM_ZERODIVIDE = 0x00000008;
        const int _EM_OVERFLOW = 0x00000004;
        const int _EM_INEXACT = 0x00000001;
        const int _EM_DENORMAL = 0x00080000;
        const int _CW_DEFAULT = _RC_NEAR + _PC_53 + _EM_INVALID + _EM_ZERODIVIDE + _EM_OVERFLOW + _EM_UNDERFLOW + _EM_INEXACT + _EM_DENORMAL;

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _controlfp(int newControl, int mask);

        [DllImport("PrintLabel.dll", CharSet = CharSet.Unicode)]
        private static extern void Print(string jsonstr, bool show_dialog);
        [DllImport("PrintLabel.dll", CharSet = CharSet.Unicode)]
        private static extern void _Left(int Value);
        [DllImport("PrintLabel.dll", CharSet = CharSet.Unicode)]
        private static extern void _Top(int Value);
        [DllImport("PrintLabel.dll", CharSet = CharSet.Unicode)]
        private static extern void _Right(int Value);
        [DllImport("PrintLabel.dll", CharSet = CharSet.Unicode)]
        private static extern void _Bottom(int Value);
        [DllImport("PrintLabel.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string GetErrorMessage();

        public void Left(int Value)
        {
            _Left(Value);
            _controlfp(_CW_DEFAULT, 0xfffff);
        }
        public void Top(int Value)
        {
            _Top(Value);
            _controlfp(_CW_DEFAULT, 0xfffff);
        }
        public void Right(int Value)
        {
            _Right(Value);
            _controlfp(_CW_DEFAULT, 0xfffff);
        }
        public void Bottom(int Value)
        {
            _Bottom(Value);
            _controlfp(_CW_DEFAULT, 0xfffff);
        }

        public void Print(OrderPrintInfo orderPrintInfo, bool ShowSelectPrint)
        {
            MemoryStream Mstream = new MemoryStream();

            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(OrderPrintInfo), new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("dd.MM.yyyy HH:mm:ss")
            });

            Serializer.WriteObject(Mstream, orderPrintInfo);

            Mstream.Position = 0;
            StreamReader sr = new StreamReader(Mstream);

            string json_str = sr.ReadToEnd();

            Print(json_str, ShowSelectPrint);
            _controlfp(_CW_DEFAULT, 0xfffff);

        }
    }
}
