using Business.Core;
using Business.Files;
using Business.Process;
using Business.Repositories;
using GUI.Config;
using Logger;
using Logger.Log4Net;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GUI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private IAppSettings _AppConfig;
        private ILocalRepository _LocalRepository;
        private IProcessManager _ProcessManager;
        private Process _CurrentProcess;

        public MainWindowViewModel()
        {
            _AppConfig = BaseFactory.GetFactory().GetConfiguration<AppSettings>();
            _ProcessManager = BaseFactory.GetFactory().GetService<IProcessManager>();

            InitRepository();
        }

        public void Refresh()
        {
            OnPropertyChanged("LocalVersion");
            OnPropertyChanged("LocalLastChange");
            OnPropertyChanged("RemoteVersion");
            OnPropertyChanged("RemoteLastChange");
        }

        protected void InitRepository()
        {
            _LocalRepository = new LocalRepository(_AppConfig.RepositoryName, _AppConfig.RepositoriesPath);
            _LocalRepository.Refresh();

            if(!String.IsNullOrEmpty(_LocalRepository.ProcessName))
            {
                _ProcessManager.WatchForProcessByName(_LocalRepository.ProcessName, async p => await _ProcessManager.WaitForExit(p, OnProcessClosing));
            }
        }

        public async Task RunSynchronisationAsync(Action actionToRun)
        {
            await RunHandleActionAsync(actionToRun);
        }

        protected void RunHandleAction(Action actionToRun)
        {
            try
            {
                actionToRun.Invoke();
                Refresh();
            }
            catch (ApplicationException ex)
            {
                ShowErrorMessageBox(ex);
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox(ex);
            }
        }

        protected async Task RunHandleActionAsync(Action actionToRun)
        {
            try
            {
                await Task.Run(() => actionToRun.Invoke());
                Refresh();
            }
            catch (ApplicationException ex)
            {
                ShowErrorMessageBox(ex);
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox(ex);
            }
        }

        protected void RunHandleAction(Func<Task> actionToRun)
        {
            Task.Run(() =>
            {
                try
                {
                    actionToRun.Invoke();
                    Refresh();
                }
                catch (ApplicationException ex)
                {
                    ShowErrorMessageBox(ex);
                }
                catch (Exception ex)
                {
                    ShowErrorMessageBox(ex);
                }
            });
        }

        protected MessageBoxResult ShowErrorMessageBox(ApplicationException exception)
        {
            return MessageBox.Show(exception.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);

        }
        protected MessageBoxResult ShowErrorMessageBox(Exception exception)
        {
            return MessageBox.Show("Unbekannter Fehler ist aufgetreten: " + Environment.NewLine + Environment.NewLine +
                exception.StackTrace, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected MessageBoxResult ShowSynPreviewMessageBox(string direction, string title, string importantMessage, MessageBoxImage image)
        {
            string message = "Folgende Synchronisationsrichtung wird durchgeführt:" + Environment.NewLine + Environment.NewLine +
                    $"Synchronisationsrichtung: {direction}" + Environment.NewLine +
                    $"Lokales Repository: Version {LocalVersion} vom {LocalLastChange.ToString("dd.MM.yyyy HH:mm:ss")}" + Environment.NewLine +
                    $"Remote Repository: Version {RemoteVersion} vom {RemoteLastChange.ToString("dd.MM.yyyy HH:mm:ss")}" + Environment.NewLine + Environment.NewLine +
                    "Soll die Synchronisation durchgeführt werden?";

            if (!String.IsNullOrEmpty(importantMessage))
            {
                message += Environment.NewLine + Environment.NewLine + "ACHTUNG" + Environment.NewLine + importantMessage;
            }

            return MessageBox.Show(message, title, MessageBoxButton.OKCancel, image);
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if(_CurrentProcess != null)
            {
                if(!_CurrentProcess.HasExited)
                {
                    MessageBoxResult res = MessageBox.Show($"Der Prozess '{_LocalRepository.ProcessName}' ist noch nicht beendet." + Environment.NewLine + Environment.NewLine +
                        "Soll die App wirklich beendet werden?", "Prozess läuft noch", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if(res != MessageBoxResult.Yes)
                    {
                        return;
                    }
                }
            }

            OnProcessClosing();
        }

        public void OnProcessClosing()
        {
            _CurrentProcess = null;

            MessageBoxResult res;
            MessageBoxImage messageImage = MessageBoxImage.Question;
            string importantMessge = null;

            _LocalRepository.ValidateVersion();

            if (_LocalRepository.IsNotNewerThanRemote())
            {
                return;
            }

            if (_LocalRepository.IsOlderThanRemote())
            {
                importantMessge = "Der remote Spielstand ist NEUER als der lokale Spielstand!";
                messageImage = MessageBoxImage.Warning;
            }

            res = ShowSynPreviewMessageBox("Mein PC -> Cloud (PUSH)", "Mein PC -> Cloud (PUSH)", importantMessge, messageImage);

            if (res == MessageBoxResult.OK)
            {
                Task.Run(() => RunSynchronisationAsync(_LocalRepository.PushToRemote));
                return;
            }
        }

        #region Properties

        public int LocalVersion
        {
            get { return _LocalRepository.Version; }
            set
            {
                _LocalRepository.Version = value;
                OnPropertyChanged("LocalVersion");
            }
        }

        public DateTime LocalLastChange
        {
            get { return _LocalRepository.LastChange; }
            set
            {
                OnPropertyChanged("LocalLastChange");
            }
        }

        public int RemoteVersion
        {
            get { return _LocalRepository.RemoteRepository.Version; }
            set
            {
                _LocalRepository.RemoteRepository.Version = value;
                OnPropertyChanged("RemoteVersion");
            }
        }

        public DateTime RemoteLastChange
        {
            get { return _LocalRepository.RemoteRepository.LastChange; }
            set
            {
                OnPropertyChanged("RemoteLastChange");
            }
        }

        private ICommand _PullCommand;
        public ICommand PullCommand
        {
            get
            {
                if (_PullCommand == null)
                {
                    _PullCommand = new DelegateCommand(() => RunHandleAction(HandlePullClick));
                }

                return _PullCommand;
            }
            set
            {
                _PullCommand = value;
                OnPropertyChanged("PullCommand");
            }
        }

        private ICommand _PushCommand;
        public ICommand PushCommand
        {
            get
            {
                if (_PushCommand == null)
                {
                    _PushCommand = new DelegateCommand(() => RunHandleAction(HandlePushClick));
                }

                return _PushCommand;
            }
            set
            {
                _PushCommand = value;
                OnPropertyChanged("PushCommand");
            }
        }

        private ICommand _StartCommand;
        public ICommand StartCommand
        {
            get
            {
                if (_StartCommand == null)
                {
                    _StartCommand = new DelegateCommand(() => RunHandleAction(HandleStartClick));
                }

                return _StartCommand;
            }
            set
            {
                _StartCommand = value;
                OnPropertyChanged("StartCommand");
            }
        }

        private ICommand _SynchronizeCommand;
        public ICommand SynchronizeCommand
        {
            get
            {
                if (_SynchronizeCommand == null)
                {
                    _SynchronizeCommand = new DelegateCommand(() => RunHandleAction(HandleSynchronizeClick));
                }

                return _SynchronizeCommand;
            }
            set
            {
                _SynchronizeCommand = value;
                OnPropertyChanged("SynchronizeCommand");
            }
        }

        private ICommand _RefreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (_RefreshCommand == null)
                {
                    _RefreshCommand = new DelegateCommand(() => RunHandleAction(HandleRefreshClick));
                }

                return _RefreshCommand;
            }
            set
            {
                _RefreshCommand = value;
                OnPropertyChanged("RefreshCommand");
            }
        }

        private ICommand _RepositoryInfoClick;
        public ICommand RepositoryInfoClick
        {
            get
            {
                if (_RepositoryInfoClick == null)
                {
                    _RepositoryInfoClick = new DelegateCommand(() => RunHandleAction(HandleRepositoryInfoClick));
                }

                return _RepositoryInfoClick;
            }
            set
            {
                _RepositoryInfoClick = value;
                OnPropertyChanged("RepositoryInfoClick");
            }
        }

        #endregion

        #region Handle Interactions

        protected async Task HandleStartClick()
        {
            if (String.IsNullOrEmpty(_LocalRepository.StartCommandLine))
            {
                throw new ApplicationException("Option 'StartCommandLine' ist im aktuellen Repository nicht konfiguriert.");
            }

            if(String.IsNullOrEmpty(_LocalRepository.ProcessName))
            {
                throw new ApplicationException("Option 'ProcessName' ist im aktuellen Repository nicht konfiguriert.");
            }

            _LocalRepository.ValidateVersion();

            if (_LocalRepository.IsOlderThanRemote())
            {
                MessageBoxResult res = ShowSynPreviewMessageBox("Cloud -> Mein PC (PULL)", "Mein PC -> Cloud (PULL)",
                "Der lokale Spielstand ist älter als der remote Spielstand!" + Environment.NewLine + Environment.NewLine +
                "Abbruch führt zu einem späteren Konflikt!", MessageBoxImage.Question);

                if (res == MessageBoxResult.OK)
                {
                    await RunSynchronisationAsync (_LocalRepository.PullFromRemote);
                    return;
                }
            }

            _ProcessManager.StartProcess(Processes.POWERSHELL, _LocalRepository.StartCommandLine);
            _CurrentProcess = await _ProcessManager.WatchForProcessByName(_LocalRepository.ProcessName);
            await _ProcessManager.WaitForExit(_CurrentProcess, OnProcessClosing);
        }


        protected async Task HandlePullClick()
        {
            _LocalRepository.ValidateVersion();

            string importantMessage = null;
            MessageBoxImage image = MessageBoxImage.Information;

            if (_LocalRepository.IsNewerThanRemote())
            {
                importantMessage = "Der lokale Spielstand ist NEUER als der remote Spielstand!";
                image = MessageBoxImage.Warning;
            }

            MessageBoxResult res = ShowSynPreviewMessageBox("Cloud -> Mein PC (PULL)", "Cloud -> Mein PC (PULL)",
                importantMessage, image);

            if (res == MessageBoxResult.OK)
            {
                await RunSynchronisationAsync (_LocalRepository.PullFromRemote);
                return;
            }
        }

        protected async Task HandlePushClick()
        {
            _LocalRepository.ValidateVersion();

            string importantMessage = null;
            MessageBoxImage image = MessageBoxImage.Information;

            if (_LocalRepository.IsOlderThanRemote())
            {
                importantMessage = "Der remote Spielstand ist NEUER als der lokale Spielstand!";
                image = MessageBoxImage.Warning;
            }

            MessageBoxResult res = ShowSynPreviewMessageBox("Mein PC -> Cloud (PUSH)", "Mein PC -> Cloud (PUSH)",
                importantMessage, image);

            if (res == MessageBoxResult.OK)
            {
                await RunSynchronisationAsync (_LocalRepository.PushToRemote);
                return;
            }
        }

        protected async Task HandleSynchronizeClick()
        {
            SyncPreview syncPreview = _LocalRepository.GetSynchronizePreview();

            if (syncPreview == SyncPreview.Nothing)
            {
                MessageBox.Show("Keine ausstehenden Änderungen gefunden." + Environment.NewLine +
                    "Alles auf dem aktuellsten Stand", "Aktuellster Stand", MessageBoxButton.OK, MessageBoxImage.Information);
                Refresh();
                return;
            }

            string direction = string.Empty;
            if (syncPreview == SyncPreview.PushToRemote)
            {
                direction = "Mein PC -> Cloud (PUSH)";
            }
            else if (syncPreview == SyncPreview.PullFromRemote)
            {
                direction = "Cloud -> Mein PC (PULL)";
            }

            MessageBoxResult res = ShowSynPreviewMessageBox(direction, direction, null, MessageBoxImage.Information);

            if (res == MessageBoxResult.OK)
            {
                await RunSynchronisationAsync (_LocalRepository.Synchronize);
                return;
            }

            Refresh();
        }

        public void HandleRefreshClick()
        {
            _LocalRepository.Refresh();
            Refresh();
        }

        public void HandleRepositoryInfoClick()
        {
            StringBuilder infoMsgBuilder = new StringBuilder();
            _LocalRepository.ToString().Split("|").ToList().ForEach(c => infoMsgBuilder.Append(c + Environment.NewLine));

            MessageBox.Show(infoMsgBuilder.ToString(), _LocalRepository.Name, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region INotifyPropertyChanged Members 
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                    new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

}
