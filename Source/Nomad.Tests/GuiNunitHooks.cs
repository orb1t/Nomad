using NUnit.Framework;
using TestsShared;
using TestsShared.FunctionalTests;

namespace Nomad.Tests
{
    /// <summary>
    ///     Called by NUnit after all fixtures from test assembly
    ///     were executed. Ensures that test application is properly
    ///     shutdown before NUnit tries to exit.
    /// </summary>
    [SetUpFixture]
    [FunctionalTests]
    public class GuiNunitHooks
    {
        /// <summary>
        ///     Shutdowns application if it was launched
        /// </summary>
        [TearDown]
        public void ensure_application_is_shutdown_standard_tear_down()
        {
            GuiApplicationContainer.ShutdownApplication();
        }
    }
}