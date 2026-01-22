using Autodesk.AutoCAD.Runtime;
using System;
using System.IO;
using System.Reflection;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace test.Application
{
    public class PluginEntry : IExtensionApplication
    {
        public void Initialize()
        {
            // Register assembly resolver for dependencies
            AppDomain.CurrentDomain.AssemblyResolve += ResolveHandler;

            // Initialize ribbon after AutoCAD is ready
            AcadApp.Idle += OnIdle;

            AcadApp.DocumentManager.MdiActiveDocument?
                .Editor.WriteMessage("\nTest plugin loaded successfully.");
        }

        private void OnIdle(object sender, EventArgs e)
        {
            AcadApp.Idle -= OnIdle;
            RibbonManager.InitializeRibbon();
        }

        private Assembly ResolveHandler(object sender, ResolveEventArgs e)
        {
            string shortName = e.Name.Split(',')[0];
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in loadedAssemblies)
            {
                string name1 = assembly.GetName().Name;
                if (string.Compare(shortName, name1, true) == 0)
                    return assembly;
            }

            try
            {
                string path = Assembly.GetExecutingAssembly().Location;
                string directory = Path.GetDirectoryName(path);
                string finalPath = Path.Combine(directory, shortName + ".dll");

                if (File.Exists(finalPath))
                {
                    Assembly retval = Assembly.LoadFrom(finalPath);
                    return retval;
                }
            }
            catch (System.Exception )
            {
                AcadApp.DocumentManager.MdiActiveDocument?
             .Editor.WriteMessage("Eror");
            }

            return null;
        }

        public void Terminate()
        {
            // Cleanup
            AppDomain.CurrentDomain.AssemblyResolve -= ResolveHandler;
        }
    }
}