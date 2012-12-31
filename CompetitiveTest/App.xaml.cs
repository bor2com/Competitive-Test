namespace SSU.CompetitiveTest {

    using System.Windows;

    public partial class App : Application {

        public App() : base() {
            this.Dispatcher.UnhandledException += (esnder, e) => {
                MessageBox.Show("Вибачте, але у програмі виникла помилка. Будь-ласка напишіть на dibrov.bor@gmail.com яким чином це сталося.", "Помилка виконання :(", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
            };
        }

    }

}
