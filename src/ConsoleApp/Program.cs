using System;
using System.Threading;
using System.Threading.Tasks;
using Core;

namespace ConsoleApp
{
    class Program
    {
        private static async Task Main()
        {
            await InversionOfControl.Container
                .GetInstance<Bot>()
                .RunAsync();

            await Task.Delay(-1);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception) e.ExceptionObject;
            Console.WriteLine("Event code error handler");
        }
    }
}
