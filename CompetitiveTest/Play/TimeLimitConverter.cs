namespace SSU.CompetitiveTest.Play {

    using System;
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof(TimeSpan), typeof(String))]
    public class TimeLimitConverter : IValueConverter {

        #region Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return String.Format("{0:F1} с", ((TimeSpan)value).TotalSeconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion

    }

}
