using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace WpfFrequentlyChangeCollectionPerformanceTest
{
    public class MainWindowVm : ViewModelBase
    {
        private bool _useCollectionView = true;
        public bool UseCollectionView
        {
            get { return _useCollectionView; }
            set { Set(ref _useCollectionView, value, nameof(UseCollectionView)); }
        }

        private bool _useDispatcherInvoke;
        public bool UseDispatcherInvoke
        {
            get { return _useDispatcherInvoke; }
            set { Set(ref _useDispatcherInvoke, value, nameof(UseDispatcherInvoke)); }
        }

        private bool _useDispatcherBeginInvoke;
        public bool UseDispatcherBeginInvoke
        {
            get { return _useDispatcherBeginInvoke; }
            set { Set(ref _useDispatcherBeginInvoke, value, nameof(UseDispatcherBeginInvoke)); }
        }

        public IEnumerable<DispatcherPriority> DispatcherPriorities
        {
            get { return Enum.GetValues(typeof(DispatcherPriority)).Cast<DispatcherPriority>(); }
        }

        private DispatcherPriority _selectedDispatcherPriority = DispatcherPriority.Normal;
        public DispatcherPriority SelectedDispatcherPriority
        {
            get { return _selectedDispatcherPriority; }
            set { Set(ref _selectedDispatcherPriority, value, nameof(SelectedDispatcherPriority)); }
        }

        public ObservableCollection<IMessageBox> MessageBoxes { get; set; } = new ObservableCollection<IMessageBox>();

        private ICommand _startCommand;
        public ICommand StartCommand
        {
            get
            {
                if (_startCommand == null)
                    _startCommand = new RelayCommand(StartCommandAction);
                return _startCommand;
            }
        }

        private ICommand _stopCommand;
        public ICommand StopCommand
        {
            get
            {
                if (_stopCommand == null)
                    _stopCommand = new RelayCommand(StopCommandAction);
                return _stopCommand;
            }
        }

        public MainWindowVm()
        {
        }

        private void StopCommandAction()
        {
            foreach (var messageVm in MessageBoxes)
            {
                messageVm.Stop();
            }
        }

        private void StartCommandAction()
        {
            if (MessageBoxes.Any(messageBox => messageBox.IsRunning == true))
                return;

            MessageBoxes.Clear();

            if (UseCollectionView)
                FillCollectionViewMessageBox(5);
            else if (UseDispatcherInvoke)
                FillDispatcherMessageBox(5, true, _selectedDispatcherPriority);
            else if (UseDispatcherBeginInvoke)
                FillDispatcherMessageBox(5, false, _selectedDispatcherPriority);

            foreach (var messageBox in MessageBoxes)
            {
                messageBox.Start();
            }
        }

        private void FillCollectionViewMessageBox(int boxCount)
        {
            for (int i = 0; i < boxCount; i++)
            {
                MessageBoxes.Add(new CollectionViewMessageBox(50));
            }
        }

        private void FillDispatcherMessageBox(int boxCount, bool useInvoke, DispatcherPriority selectedDispatcherPriority)
        {
            for (int i = 0; i < boxCount; i++)
            {
                MessageBoxes.Add(new DispatcherMessageBox(50, useInvoke, selectedDispatcherPriority));
            }
        }
    }
}
