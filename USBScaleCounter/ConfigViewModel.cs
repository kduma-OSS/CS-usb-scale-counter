using System.ComponentModel;

namespace USBScaleCounter
{
    public class ConfigViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double _expectedWeight;

        public double ExpectedWeight
        {
            get => _expectedWeight;
            set
            {
                if (_expectedWeight == value) return;
                _expectedWeight = value;

                OnPropertyChanged(nameof(ExpectedWeight));
            }
        }

        private int _expectedCount;

        public int ExpectedCount
        {
            get => _expectedCount;
            set
            {
                if (_expectedCount == value) return;
                _expectedCount = value;

                OnPropertyChanged(nameof(ExpectedCount));
            }
        }

        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}