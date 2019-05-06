using Microsoft.Win32;
using System;

namespace SomeOneSpyingOnYou.Services
{
    public interface IWindowsService : IDisposable
    {
        void Register();
    }

    public class WindowsService : IWindowsService
    {
        public void Dispose()
        {
        }

        public void Register()
        {
            var decision = "";

            Switch:
            switch (decision)
            {
                case "y":
                case "Y":
                    RegisterAppToWindowsStart();
                    break;

                case "n":
                case "N":
                    UnRegisterAppFromWindowsStart();
                    break;

                default:
                    Console.WriteLine("Start application at Windows startup 'y' or 'n'");
                    decision = Console.ReadLine();
                    Console.Clear();
                    goto Switch;
            }

        }

        private void RegisterAppToWindowsStart()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.SetValue("SomeOneSpyingOnYou", "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"");
            }
        }

        private void UnRegisterAppFromWindowsStart()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.DeleteValue("SomeOneSpyingOnYou", false);
            }
        }
    }
}
