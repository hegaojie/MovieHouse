using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace MovieHouse
{
    public class Movie
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public int Year { get; set; }
        public string Details { get; set; }
        public string FileName { get; set; }
        public string PosterName { get; set; }

        public Movie()
        {
            Name = string.Empty;
            Country = string.Empty;
            Details = string.Empty;
            FileName = string.Empty;
            PosterName = string.Empty;
        }

        [XmlIgnore]
        public BitmapImage Poster { get; private set; }

        [XmlIgnore]
        public int SequencialNo { get; set; }

        [XmlIgnore]
        public bool IsPosterChanged { get; set; }

        public void LoadPoster()
        {
            if (File.Exists(PosterName))
            {
                Poster = new BitmapImage(new Uri(PosterName));
            }
            else
            {
                //TODO:load default poster
            }
        }

        public override bool Equals(object obj)
        {
            var movie = obj as Movie;
            return movie != null && string.Equals(Name, movie.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}