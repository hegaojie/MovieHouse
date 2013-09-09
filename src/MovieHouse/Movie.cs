using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
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

        [XmlIgnore]
        public BitmapImage Poster { get; private set; }

        public int SequencialNo { get; set; }

        public void LoadPoster()
        {
            if (File.Exists(PosterName))
            {
                Poster = new BitmapImage(new Uri(PosterName));
            }
        }

        public override bool Equals(object obj)
        {
            var movie = obj as Movie;
            if (movie != null)
            {
                return string.Equals(Name, movie.Name) && string.Equals(FileName, movie.FileName);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() | FileName.GetHashCode();
        }
    }
}