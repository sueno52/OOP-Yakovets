using System;

namespace Lab6
{

    public class MediaItem
    {

        private string _title;
        private int _duration; 

        public string Title => _title;
        public int Duration => _duration;

        public MediaItem(string title, int duration)
        {
            _title = title;
            _duration = duration;
        }

        public virtual void Play()
        {
            Console.WriteLine($"[MediaItem] Відтворення медіафайлу: '{_title}' (тривалість/обсяг: {_duration})");
        }

        public string GetMediaType()
        {
            return "Загальний медіа-ресурс (MediaItem)";
        }
    }


    public class BookMedia : MediaItem
    {
        private string _author;

        public string Author => _author;


        public BookMedia(string title, int pages, string author) 
            : base(title, pages)
        {
            _author = author;
        }

        public override void Play()
        {
            Console.WriteLine($"[BookMedia] Читання книги: '{Title}', Автор: {_author}, Сторінок: {Duration}");
        }

        public void ReadSample()
        {
            Console.WriteLine($"[BookMedia] Відкрито уривок книги '{Title}' для ознайомлення.");
        }

        public new string GetMediaType()
        {
            return "Книжкове видання (BookMedia)";
        }
    }
    public class MovieMedia : MediaItem
    {
        private string _director;

        public string Director => _director;

        public MovieMedia(string title, int durationMinutes, string director) 
            : base(title, durationMinutes)
        {
            _director = director;
        }

        public override void Play()
        {
            Console.WriteLine($"[MovieMedia] Показ фільму: '{Title}', Режисер: {_director}, Тривалість: {Duration} хв.");
        }

        public void ShowTrailer()
        {
            Console.WriteLine($"[MovieMedia] Запуск трейлера до фільму '{Title}'!");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Створення об'єктів та виклик унікальних методів");
            MediaItem baseMedia = new MediaItem("Деяке медіа", 100);
            BookMedia book = new BookMedia("Кобзар", 350, "Тарас Шевченко");
            MovieMedia movie = new MovieMedia("Тіні забутих предків", 97, "Сергій Параджанов");

            book.ReadSample();
            movie.ShowTrailer();
            Console.WriteLine();


            Console.WriteLine("Поліморфізм");
            MediaItem[] playlist = new MediaItem[] { baseMedia, book, movie };

            foreach (var item in playlist)
            {

                item.Play();
            }
            Console.WriteLine();


            Console.WriteLine("Різниця між override і new");

            BookMedia bookRef = new BookMedia("1984", 328, "Джордж Орвелл");
            Console.WriteLine($"Через посилання BookMedia -> GetMediaType(): {bookRef.GetMediaType()}");

            MediaItem mediaRef = bookRef;
            Console.WriteLine($"Через посилання MediaItem -> GetMediaType(): {mediaRef.GetMediaType()}");

            Console.WriteLine("\nПояснення: При 'new' метод обирається за типом посилання (MediaItem викликає метод базового класу, а BookMedia — новий прихований).");
        }
    }
}