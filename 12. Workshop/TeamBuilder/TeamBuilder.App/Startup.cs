using System;
using TeamBuilder.App.Core;

namespace TeamBuilder.App
{
    class Startup
    {
        static void Main()
        {
            var commandDispatcher = new CommandDispatcher();

            var engine = new Engine(commandDispatcher);
            engine.Run();
        }
    }
}
