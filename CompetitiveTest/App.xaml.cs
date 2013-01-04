namespace SSU.CompetitiveTest {

    using System.Windows;

    public partial class App : Application {

        public App() : base() {
            this.Dispatcher.UnhandledException += (esnder, e) => {
                MessageBox.Show("Unknown error :(");
                e.Handled = true;
            };
        }

    }

}
