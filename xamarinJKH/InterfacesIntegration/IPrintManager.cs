using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace xamarinJKH.InterfacesIntegration
{
    public interface IPrintManager
    {
        void SendFileToPrint(byte[] file);
    }
}
