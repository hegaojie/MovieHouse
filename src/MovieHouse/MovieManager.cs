using System.Collections.ObjectModel;

namespace MovieHouse
{
    public class MovieManager
    {
        private readonly ObservableCollection<Movie> _movies;

        private MovieConfig _config;

        public ReadOnlyObservableCollection<Movie> Movies
        {
            get { return new ReadOnlyObservableCollection<Movie>(_movies); }
        }

        public MovieManager(MovieConfig config)
        {
            _config = config;
            _movies = new ObservableCollection<Movie>();
        }

        public void Add(Movie movie)
        {
            if (_movies.Contains(movie)) 
                return;
            
            _movies.Add(movie);
        }

        public void Remove(Movie movie)
        {
            if (_movies.Contains(movie))
            {
                _movies.Remove(movie);
            }
        }

        public void SaveConfig()
        {
            _config.Save(_movies);
        }

        public void LoadConfig()
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

        public Movie RecordNewMovie()
        {
            throw new System.NotImplementedException();
        }
    }
}