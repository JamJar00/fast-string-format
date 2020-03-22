using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastStringFormat.Test
{
    [TestClass]
    public class CultureInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture =  new CultureInfo("en-GB");
        }
    }
}
