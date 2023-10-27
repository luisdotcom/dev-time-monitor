using Community.VisualStudio.Toolkit;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DevTimeMonitor
{
    internal partial class OptionsProvider
    {
        // Register the options with this attribute on your package class:
        // [ProvideOptionPage(typeof(OptionsProvider.SettingsPageOptions), "DevTimeMonitor", "SettingsPage", 0, 0, true, SupportsProfiles = true)]
        [ComVisible(true)]
        public class SettingsPageOptions : BaseOptionPage<SettingsPage> { }
    }

    public class SettingsPage : BaseOptionModel<SettingsPage>
    {
        [Category("General")]
        [DisplayName("Autostart the extension")]
        [Description("Autostart the DevTimeMonitor extension.")]
        [DefaultValue(true)]
        public bool Autostart { get; set; } = true;

        [Category("Database")]
        [DisplayName("Connection String")]
        [Description("Connection string used for the data base.")]
        [DefaultValue("")]
        public string ConnectionString { get; set; } = "";

        [Category("User")]
        [DisplayName("Logged")]
        [Description("Status of the user account.")]
        [DefaultValue(false)]
        [ReadOnly(true)]
        public bool Logged { get; set; } = false;

        [Category("User")]
        [DisplayName("Name")]
        [Description("Name of the user logged.")]
        [DefaultValue("")]
        [ReadOnly(true)]
        public string Name { get; set; } = "";

        [Category("User")]
        [DisplayName("Username")]
        [Description("Username of the user logged.")]
        [DefaultValue("")]
        [ReadOnly(true)]
        public string UserName { get; set; } = "";

        [Category("User")]
        [DisplayName("Password")]
        [Description("Password of the user logged.")]
        [DefaultValue("")]
        [ReadOnly(true)]
        [PasswordPropertyText(true)]
        public string Password { get; set; } = "";
    }
}
