using L_AI.Options;
using L_AI.TextGeneration;
using L_AI.UI.Impl;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace L_AI.UI.ToolWindows.ViewModels
{
    public enum ConnectionStatusEnum
    {
        Unchecked,
        Success,
        Failure,
    }

    public class FirstSetupViewModel : ObservableObject
    {
        #region General

        public ICommand GoBackCommand => new RelayCommand(() =>
        {
            _backActions[CurrentStep]?.Invoke();
            CurrentStep--;
            NotifyPropertyChanged(nameof(CanNavigate));
        });
        public ICommand ContinueCommand => new RelayCommand(() =>
        {
            CurrentStep++;
            NotifyPropertyChanged(nameof(CanNavigate));
        });

        private const int MaxSteps = 2;
        public bool CanNavigateBack => CurrentStep > 0;

        private int _currentStep;
        public int CurrentStep
        {
            get => _currentStep;
            set
            {
                SetProperty(ref _currentStep, value);
                NotifyPropertyChanged(nameof(CanNavigateBack));
                NotifyPropertyChanged(nameof(CanNavigate));
            }
        }

        public bool CanNavigate
        {
            get
            {
                switch (CurrentStep)
                {
                    case 0:
                        return (_isOoga || _isKobold) && CurrentStep < MaxSteps;
                    case 1:
                        return ConnectionStatus == ConnectionStatusEnum.Success && CurrentStep < MaxSteps;
                    default:
                        return false;
                }
            }

        }

        private readonly Dictionary<int, Action> _backActions;

        #endregion

        #region Step 0

        private bool _isKobold;
        public bool IsKobold
        {
            get => _isKobold;
            set
            {
                SetProperty(ref _isKobold, value);
                NotifyPropertyChanged(nameof(CanNavigate));
                Options.ApiProvider = GenerationProviderType.Kobold;
            }
        }

        private bool _isOoga;
        public bool IsOoga
        {
            get => _isOoga;
            set
            {
                SetProperty(ref _isOoga, value);
                NotifyPropertyChanged(nameof(CanNavigate));
                Options.ApiProvider = GenerationProviderType.OogaBooga;
            }
        }

        public ICommand SelectKoboldCommand => new RelayCommand(() => IsKobold = true);
        public ICommand SelectOogaCommand => new RelayCommand(() => IsOoga = true);

        #endregion

        #region Step 1

        public ICommand TestConnectionCommand => new RelayCommand(() => Task.Run(TestConnectionAsync).Forget());

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                SetProperty(ref _isBusy, value);
                NotifyPropertyChanged(nameof(ShouldShowButton));
            }
        }

        private ConnectionStatusEnum _connectionStatus;
        public ConnectionStatusEnum ConnectionStatus
        {
            get => _connectionStatus;
            set
             {
                SetProperty(ref _connectionStatus, value);
                NotifyPropertyChanged(nameof(ShouldShowButton));
             }
        }

        public bool ShouldShowButton => ConnectionStatus != ConnectionStatusEnum.Success && !IsBusy;

        private async Task TestConnectionAsync()
        {
            ConnectionStatus = ConnectionStatusEnum.Unchecked;
            IsBusy = true;
            await Task.Delay(300);
            var canConnect = await GenerationManager.TestConnection(Options);
            ConnectionStatus = canConnect ? ConnectionStatusEnum.Success : ConnectionStatusEnum.Failure;
            IsBusy = false;
            NotifyPropertyChanged(nameof(CanNavigate));
        }

        #endregion

        private readonly GeneralOptions _options;
        public GeneralOptions Options => _options;

        public FirstSetupViewModel()
        {
            _options = GeneralOptions.Instance.CreateCopy();
            _backActions = new Dictionary<int, Action>()
            {
                { 0, null },
                {
                    1, () => {
                        IsBusy = false;
                        ConnectionStatus = ConnectionStatusEnum.Unchecked;
                    }
                },
                { 2, null },
            };
        }
    }
}
