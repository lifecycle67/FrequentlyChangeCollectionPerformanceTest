using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ShareLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;

namespace UwpFrequentlyChangeCollectionPerformanceTest
{
    public class MainPageVm : ViewModelBase
    {
        private bool _useCollectionView;
        public bool UseCollectionView
        {
            get { return _useCollectionView; }
            set { Set(ref _useCollectionView, value, nameof(UseCollectionView)); }
        }

        private bool _useDispatcherInvoke = true;
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

        public IEnumerable<CoreDispatcherPriority> DispatcherPriorities
        {
            get { return Enum.GetValues(typeof(CoreDispatcherPriority)).Cast<CoreDispatcherPriority>(); }
        }

        private CoreDispatcherPriority _selectedDispatcherPriority = CoreDispatcherPriority.Normal;
        public CoreDispatcherPriority SelectedDispatcherPriority
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
            FillDispatcherMessageBox(5, _selectedDispatcherPriority);

            foreach (var messageBox in MessageBoxes)
            {
                messageBox.Start();
            }
        }

        private void FillDispatcherMessageBox(int boxCount, CoreDispatcherPriority selectedDispatcherPriority)
        {
            for (int i = 0; i < boxCount; i++)
            {
                MessageBoxes.Add(new DispatcherMessageBox(50, selectedDispatcherPriority));
            }
        }
    }
}
