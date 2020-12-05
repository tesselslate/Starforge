using Eto.Drawing;
using Eto.Forms;
using System;

namespace Starforge.UI.Main {
    public partial class MainWindow : Form {
        public MainWindow() {
            Icon = new Icon(1f, new Bitmap("./assets/logo_256.png"));
        }

        private void CreateMenu() {
            Menu = new MenuBar
            {
                Items =
                {
                    new ButtonMenuItem
                    {
                        Text = "&File",
                        Items =
                        {
                            new NewMapCommand(),
                            new OpenMapCommand(),
                            new SaveMapCommand(),
                            new SaveAsCommand(),
                            new QuitCommand()
                        }
                    },
                    new ButtonMenuItem
                    {
                        Text = "&Edit",
                        Items =
                        {

                        }
                    },
                    new ButtonMenuItem
                    {
                        Text = "&View",
                        Items =
                        {

                        }
                    },
                    new ButtonMenuItem
                    {
                        Text = "&Help",
                        Items =
                        {

                        }
                    }
                }
            };
        }

        public void DisplayMainWindow() {
            Content = null;

            Title = "Starforge";
            WindowStyle = WindowStyle.Default;

            CreateMenu();
            Maximize();
        }

        protected override void OnShown(EventArgs e) {
            base.OnShown(e);

            if(!Core.Starforge.Loaded) {
                ClientSize = new Size(768, 256);
                Location = new Point((int)(Screen.WorkingArea.Width - Width) / 2, (int)(Screen.WorkingArea.Height - Height) / 2);

                Bitmap SplashImage = new Bitmap("./assets/banner_256.png");
                WindowStyle = WindowStyle.None;

                Content = SplashImage;

                Core.Starforge.Load();
            } else {
                DisplayMainWindow();
            }
        }
    }
}