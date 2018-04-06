using System;
using System.Security.Principal;
using System.Windows.Forms;
using Fiddler;

[assembly: RequiredVersion("4.0.0.0")]
namespace ProxyToggleExtension
{
    public sealed class SystemProxyToggle : IFiddlerExtension
    {
        private const string ParentMenuIdentifier = "&Rules";

        private MenuItem _rulesParentMenu;
        private MenuItem _enableMenuItem;

        private const string MenuText = "System Proxy";

        private readonly ProxySettingsController _controller;

        public SystemProxyToggle()
        {
            _controller = new ProxySettingsController();
        }

        public void OnLoad()
        {
            _rulesParentMenu = null;
            foreach (MenuItem menuItem in FiddlerApplication.UI.Menu.MenuItems)
            {
                if (string.Equals(menuItem.Text, ParentMenuIdentifier))
                    _rulesParentMenu = menuItem;
            }

            if (_rulesParentMenu != null)
            {
                var spacer = new MenuItem("-");
                _rulesParentMenu.MenuItems.Add(spacer);

                _enableMenuItem = new MenuItem(MenuText, Enable_Click);
                _rulesParentMenu.MenuItems.Add(_enableMenuItem);
            }
        }

        public void OnBeforeUnload()
        {
            _controller.DisableSystemProxy();
        }

        private void Enable_Click(object sender, EventArgs e)
        {
            var menu = (MenuItem) sender;
            if (menu == null)
                return;

            if (!IsAdministrator())
            {
                MessageBox.Show("Please launch Fiddler with administrator privileges.");
                return;
            }

            menu.Checked = !menu.Checked;

            if (menu.Checked)
            {
                //menu.Text = "Disable system proxy";
                _controller.EnableSystemProxy();
            }
            else
            {
                //menu.Text = "Enable system proxy";
                _controller.DisableSystemProxy();
            }
        }

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
