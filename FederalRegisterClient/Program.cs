using System;

namespace FederalRegisterClient
{
    class Program
    {
        static void Main() {
            DocumentHandler.RunAsync().GetAwaiter().GetResult();
        }        
    }
}
