using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Caliburn.Micro;

namespace MovieHouse
{
    public class AppViewModel : Screen, IHandle<MoveMovieEvent>, IHandle<CloseDetailEvent>, IHandle<CancelDetailEvent>
    {
        private readonly IWindowManager _windowManager;

        private readonly IEventAggregator _eventAggregator;

        private readonly MovieManager _mmanager;

        private readonly MoviePlayer _mplayer;

        private int _total;
        private int _currentIndex;
        private string _triggerDetailsAnimation;

        public AppViewModel(MovieManager mmanager, MoviePlayer mplayer, IEventAggregator eventAggregator, IWindowManager windowManager)
        {
            _windowManager = windowManager;

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _mplayer = mplayer;

            _mmanager = mmanager;
            _mmanager.Initialize();

            Total = _mmanager.MovieCount;
            if (Total > 0) { CurrentIndex = 1; }

            TriggerDetailsAnimation = string.Empty;
        }

        #region Binding Properties

        public ReadOnlyObservableCollection<MovieViewModel> Movies { get { return _mmanager.Movies; } }

        public int Total
        {
            get { return _total; }
            set
            {
                if (value == _total) return;
                _total = value;
                NotifyOfPropertyChange(() => Total);
                NotifyOfPropertyChange(() => CanRemoveMovie);
            }
        }

        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                if (value == _currentIndex) return;
                _currentIndex = value;
                NotifyOfPropertyChange(() => CurrentIndex);
                NotifyOfPropertyChange(() => CurrentMovieName);
                NotifyOfPropertyChange(() => CanFindNext);
                NotifyOfPropertyChange(() => CanFindPrevious);
                NotifyOfPropertyChange(() => CanPlayMovie);
                NotifyOfPropertyChange(() => CanShowDetails);
                NotifyOfPropertyChange(() => CurrentMovieDetails);
            }
        }

        public string CurrentMovieName
        {
            get { return _mmanager.CurrentMovie.Name; }
        }

        private MovieDetailViewModel _newMovieViewModel;
        public MovieDetailViewModel CurrentMovieDetails
        {
            get
            {
                return AddNew ? _newMovieViewModel
                    : new MovieDetailViewModel(_mmanager.CurrentMovie, _eventAggregator);
            }
        }

        private bool AddNew { get; set; }

        public string TriggerDetailsAnimation
        {
            get { return _triggerDetailsAnimation; }
            set
            {
                if (value == _triggerDetailsAnimation) return;
                _triggerDetailsAnimation = value;
                NotifyOfPropertyChange(() => TriggerDetailsAnimation);
            }
        }

        #endregion

        #region Binding Methods

        public bool CanFindNext
        {
            get { return _mmanager.CanFindNext; }
        }

        public void FindNext()
        {
            CurrentIndex = _mmanager.FindNext();
        }

        public bool CanFindPrevious
        {
            get { return _mmanager.CanFindPrevious; }
        }

        public void FindPrevious()
        {
            CurrentIndex = _mmanager.FindPrevious();
        }

        public void AddMovie()
        {
            AddNew = true;

            _newMovieViewModel = new MovieDetailViewModel(new Movie(), _eventAggregator);

            NotifyOfPropertyChange(() => CurrentMovieDetails);

            TriggerDetailsAnimation = Guid.NewGuid().ToString();
        }

        public void RemoveMovie()
        {
            Total--;

            if (!_mmanager.CanFindNext)
            {
                CurrentIndex--;
            }

            if (Total == 0)
            {
                CurrentIndex = 0;
            }

            _mmanager.DeleteCurrentMovie();

            NotifyOfPropertyChange(() => CurrentMovieName);
            NotifyOfPropertyChange(() => CurrentMovieDetails);
        }

        public bool CanRemoveMovie
        {
            get { return Total > 0; }
        }

        public void PlayMovie()
        {
            try
            {
                _mplayer.Play(_mmanager.CurrentMovie.FileName);
            }
            catch (Win32Exception exc)
            {
                MessageBox.Show("Can not find video. Please check the file path.", "MovieHouse - Error", MessageBoxButton.OK, MessageBoxImage.Error,
                                MessageBoxResult.OK);
            }
        }

        public bool CanPlayMovie
        {
            get { return CurrentIndex > 0; }
        }

        public void ShowDetails()
        {
            NotifyOfPropertyChange(() => CurrentMovieDetails);

            TriggerDetailsAnimation = Guid.NewGuid().ToString();
        }

        public bool CanShowDetails
        {
            get { return CanPlayMovie; }
        }

        #endregion

        public void Handle(MoveMovieEvent message)
        {
            var movie = message.Movie;
            CurrentIndex = _mmanager.MarkAsCurrentMovie(movie.Name);
            NotifyOfPropertyChange(() => CanFindNext);
            NotifyOfPropertyChange(() => CanFindPrevious);
        }

        public void Handle(CloseDetailEvent message)
        {
            TriggerDetailsAnimation = Guid.NewGuid().ToString();

            if (AddNew)
            {
                var movie = CurrentMovieDetails.ToMovie();
                _mmanager.AddMovie(movie);
                Total++;

                AddNew = false;
                _newMovieViewModel = null;
                NotifyOfPropertyChange(() => CanFindNext);
            }
            else
            {
                if (CurrentMovieDetails.IsPosterNameChanged)
                {
                    CurrentMovieDetails.RefreshPoster();
                }
            }

            NotifyOfPropertyChange(() => CurrentMovieDetails);
            NotifyOfPropertyChange(() => CurrentMovieName);
            NotifyOfPropertyChange(() => Movies);
        }

        public void Handle(CancelDetailEvent message)
        {
            TriggerDetailsAnimation = Guid.NewGuid().ToString();
            AddNew = false;
            _newMovieViewModel = null;
        }

        protected override void OnDeactivate(bool close)
        {
            _mmanager.SaveConfig();
            base.OnDeactivate(close);
        }

    }
}