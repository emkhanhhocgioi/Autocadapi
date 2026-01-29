using Autodesk.AutoCAD.Windows;
using System;
using System.Drawing;
using System.Windows.Forms.Integration;
using test.View;


namespace test.Actions
{
    internal class DBPallete
    {
        private PaletteSet _paletteSet;
        public void execute(DynamicBlockControl userControl)
        {
            if (_paletteSet == null)
            {
                string paletteName = "CustomPalette";

                _paletteSet = new PaletteSet(
                    paletteName
          
                );

                _paletteSet.Style =
                    PaletteSetStyles.ShowAutoHideButton |
                    PaletteSetStyles.ShowCloseButton |
                    PaletteSetStyles.Snappable;
                     
                _paletteSet.Opacity = 100;
                _paletteSet.Size = new Size(500, 800);
                _paletteSet.MinimumSize = new Size(250, 200);

                _paletteSet.DockEnabled = DockSides.Left | DockSides.Right;

               
                _paletteSet.Dock = DockSides.Right;

                ElementHost host = new ElementHost();
                host.AutoSize = true;
                host.Dock = System.Windows.Forms.DockStyle.Fill;
                host.Child = userControl;

                _paletteSet.Add(paletteName, host);
            }

            _paletteSet.Visible = true;
        }
    }
}
