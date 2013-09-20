using System.Collections.ObjectModel;
using Caliburn.Micro;

namespace MovieHouse
{
    public class AppViewModel : Conductor<object>, IHandle<AddMovieEvent>, IHandle<PlayMovieEvent>, IHandle<MoveMovieEvent>
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
            }
        }

        public string CurrentMovieName
        {
            get { return _mmanager.CurrentMovie.Name; }
        }

        public MovieDetail2ViewModel CurrentMovieDetails
        {
            //get { return null; }
            get { return new MovieDetail2ViewModel(new Movie(){Name = "Sample"}, null); }
        }

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
            _windowManager.ShowDialog(new MovieDetailViewModel(_eventAggregator));
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
        }

        public bool CanRemoveMovie
        {
            get { return Total > 0; }
        }

        public void PlayMovie()
        {
            _mplayer.Play(_mmanager.CurrentMovie.FileName);
        }

        public bool CanPlayMovie
        {
            get { return CurrentIndex > 0; }
        }

        public void ShowDetails()
        {
            // TODO: animate detail panel canvas.top from -690 to 0;
            
            
        }

        public bool CanShowDetails
        {
            get { return CanPlayMovie; }
        }

        #endregion

        public void Handle(AddMovieEvent message)
        {
            var movie = message.Movie;
            _mmanager.AddMovie(movie);
            Total++;
            NotifyOfPropertyChange(() => Movies);
        }

        public void Handle(PlayMovieEvent message)
        {
            var movie = message.Movie.FileName;
            _mplayer.Play(movie);
        }

        public void Handle(MoveMovieEvent message)
        {
            var movie = message.Movie;
            CurrentIndex = _mmanager.MarkAsCurrentMovie(movie.Name);
            NotifyOfPropertyChange(() => CanFindNext);
            NotifyOfPropertyChange(() => CanFindPrevious);
        }
    }
}