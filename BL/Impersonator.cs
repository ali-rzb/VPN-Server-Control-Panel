using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace BL
{
    //public class Impersonator : IDisposable
    //{
    //    private readonly WindowsImpersonationContext _impersonationContext;
    //    private const int LOGON32_LOGON_INTERACTIVE = 2;
    //    private const int LOGON32_PROVIDER_DEFAULT = 0;

    //    [DllImport("advapi32.dll", SetLastError = true)]
    //    private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

    //    [DllImport("kernel32.dll", SetLastError = true)]
    //    private static extern bool CloseHandle(IntPtr hHandle);

    //    public Impersonator(string username, string password)
    //    {
    //        IntPtr userToken = IntPtr.Zero;
    //        bool success = LogonUser(username, Environment.MachineName, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out userToken);
    //        if (!success)
    //        {
    //            throw new Exception("Impersonation failed.");
    //        }

    //        WindowsIdentity windowsIdentity = new WindowsIdentity(userToken);
    //        _impersonationContext = windowsIdentity.Impersonate();
    //    }

    //    public void Dispose()
    //    {
    //        if (_impersonationContext != null)
    //        {
    //            _impersonationContext.Undo();
    //        }
    //    }
    //}

}
