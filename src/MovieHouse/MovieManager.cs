using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;

namespace MovieHouse
{
    public class MovieManager
    {
        private int _rightNextIndex;
        private int _leftNextIndex;
        private int _currentIndex;

        private readonly ObservableCollection<Movie> _movies;

        private readonly MovieProcessQueue _queue;

        private readonly MovieConfig _config;

        private readonly IEventAggregator _eventAggregator;
       
        public ReadOnlyObservableCollection<MovieViewModel> Movies
        {
            get { return new ReadOnlyObservableCollection<MovieViewModel>(_queue.Queue); }
        }

        public int MovieCount { get { return _movies.Count; } }

        public bool CanFindNext { get { return !_queue.AlreadyReachRightEnd(); } }

        public bool CanFindPrevious { get { return !_queue.AlreadyReachLeftEnd(); } }

        public MovieManager(MovieConfig config, MovieProcessQueue queue, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _config = config;
            _queue = queue;
            _movies = new ObservableCollection<Movie>();
        }

        public void SaveConfig()
        {
            _config.Save(_movies);
        }

        public void Initialize()
        {
            LoadConfig();
            PopulateProcessQueue();
        }

        public int FindNext()
        {
            if (_queue.AlreadyReachRightEnd()) { return _currentIndex; }

            _currentIndex++;

            if (!_queue.Queue[_queue.QueueCount - 1].IsDummy)
            {
                _rightNextIndex = _queue.Queue[_queue.QueueCount - 1].SequencialNo + 1;
            }

            var newMovie = _rightNextIndex < _movies.Count ? new MovieViewModel(_movies[_rightNextIndex], _eventAggregator)
                : new MovieViewModel(new Movie(), _eventAggregator);

            _queue.InsertAtRight(newMovie);
            _queue.Animate();

            return _currentIndex;
        }

        public int FindPrevious()
        {
            if (_queue.AlreadyReachLeftEnd()) { return _currentIndex; }

            _currentIndex--;

            if (!_queue.Queue[0].IsDummy)
            {
                _leftNextIndex = _queue.Queue[0].SequencialNo - 1;
            }

            var newMovie = _leftNextIndex >= 0 ? new MovieViewModel(_movies[_leftNextIndex], _eventAggregator)
                : new MovieViewModel(new Movie(), _eventAggregator);

            _queue.InsertAtLeft(newMovie);
            _queue.Animate();

            return _currentIndex;
        }

        public void DeleteCurrentMovie()
        {
            var mvm = _queue.Queue[_queue.Capacity / 2];
            var movie = _movies.FirstOrDefault(m => m.SequencialNo == mvm.SequencialNo);
            var curIndex =  _movies.IndexOf(movie);
            for (var i =  _movies.Count - 1; i > curIndex; i--)
            {
                 _movies[i].SequencialNo =  _movies[i - 1].SequencialNo;
            }

            if (_queue.AlreadyReachRightEnd())
            {
                _queue.RemoveCenter();
                FindPrevious();
            }
            else
            {
                if (!_queue.Queue[_queue.QueueCount - 1].IsDummy)
                {
                    _rightNextIndex = _queue.Queue[_queue.QueueCount - 1].SequencialNo + 1;
                }

                var newMovie = _rightNextIndex < _movies.Count ? new MovieViewModel(_movies[_rightNextIndex], _eventAggregator)
                    : new MovieViewModel(new Movie(), _eventAggregator);

                _queue.InsertAtRightHalf(newMovie);
                _queue.Animate();
            }

            Remove(movie);

            _rightNextIndex--;

            if (_movies.Count == 0)
            {
                _currentIndex = 0;
            }
        }

        public void AddMovie(Movie movie)
        {
            movie.SequencialNo = _movies.Count;
            Add(movie);

            if (!_queue.Queue[_queue.QueueCount - 1].IsDummy) { return; }

            var lastMovie = _queue.Queue.LastOrDefault(m => !m.IsDummy);
            var index = _queue.Queue.IndexOf(lastMovie);
            var nmvm = new MovieViewModel(movie, _eventAggregator);
            var dummyView = _queue.Queue[index + 1];
            nmvm.Scale = dummyView.Scale;
            nmvm.NewScale = dummyView.NewScale;
            nmvm.CenterX = dummyView.CenterX;
            nmvm.NewCenterX = dummyView.NewCenterX;
            nmvm.CenterY = dummyView.CenterY;
            nmvm.NewCenterY = dummyView.NewCenterY;
            nmvm.Opacity = dummyView.Opacity;
            nmvm.NewOpacity = dummyView.NewOpacity;
            nmvm.ZIndex = dummyView.ZIndex;
            nmvm.NewZIndex = dummyView.NewZIndex;

            _queue.Queue[index + 1] = nmvm;
        }

        private void Add(Movie movie)
        {
            if (_movies.Contains(movie)) 
                return;
            
            _movies.Add(movie);
        }

        private void Remove(Movie movie)
        {
            if (_movies.Contains(movie))
            {
                _movies.Remove(movie);
            }
        }

        private void LoadConfig()
        {
            var movies = (ObservableCollection<Movie>)_config.Load(_movies.GetType());
            if (movies == null) 
                return;

            var sequenceNo = 0;
            foreach (var movie in movies)
            {
                movie.SequencialNo = sequenceNo++;
                Add(movie);
            }
        }

        private void PopulateProcessQueue()
        {
            while (_queue.QueueCount < _queue.Capacity)
            {
                if (_rightNextIndex >= _movies.Count)
                {
                    break;
                }

                var mvm = new MovieViewModel(_movies[_rightNextIndex], _eventAggregator);

                _queue.InsertAtRight(mvm);

                _rightNextIndex++;
            }

            if (_queue.Queue.Any(m => !m.IsDummy))
            {
                _currentIndex = 1;
            }
        }

        public int MarkAsCurrentMovie(string name)
        {
            var mvm = _queue.Queue.FirstOrDefault(m => m.Name == name);
            var index = _queue.Queue.IndexOf(mvm);

            var centerIndex = _queue.Capacity/2;
            
            if (index == centerIndex) return _currentIndex;

            if (index > centerIndex)
            {
                for (var i = 0; i < index - centerIndex; i++)
                {
                    FindNext();
                }
            }
            else
            {
                for (var i = 0; i < centerIndex - index; i++)
                {
                    FindPrevious();
                }
            }

            return _currentIndex;
        }
    }
}