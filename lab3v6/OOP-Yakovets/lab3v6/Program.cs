using System;

namespace Lab3
{

    public class AudioPlayer : IDisposable
    {
        private bool _disposed = false;         
        private bool _isPlaying = false;       
        private string _trackName;         

        public string TrackName => _trackName;
        public bool IsPlaying => _isPlaying;


        public AudioPlayer(string trackName)
        {
            _trackName = trackName;
            _isPlaying = true;
            Console.WriteLine($"[AudioPlayer] Ресурс виділено. Трек '{_trackName}' відтворюється.");
        }

        public void Play()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AudioPlayer), "Неможливо відтворити: об'єкт вже звільнено!");

            _isPlaying = true;
            Console.WriteLine($"[AudioPlayer] Відтворення '{_trackName}' відновлено.");
        }

        public void Stop()
        {
            if (_disposed) return;

            _isPlaying = false;
            Console.WriteLine($"[AudioPlayer] Відтворення '{_trackName}' зупинено.");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {

                    Console.WriteLine($"[Dispose(true)] Звільнення керованих ресурсів для '{_trackName}'...");
                    _trackName = string.Empty;
                }


                if (_isPlaying)
                {
                    Console.WriteLine($"[Dispose] Звільнення аудіо ресурсу (зупинка відтворення '{_trackName}').");
                    _isPlaying = false;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true); 
            GC.SuppressFinalize(this); 
        }


        ~AudioPlayer()
        {
            Console.WriteLine($"[~AudioPlayer] Деструктор викликано для об'єкта, про який забув розробник!");
            Dispose(false); 
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Випадок 1: Використання оператора using");
            using (var player1 = new AudioPlayer("Song_1.mp3"))
            {
                player1.Stop();
            } 
            Console.WriteLine();


            Console.WriteLine("Випадок 2: Явний виклик Dispose() без using");
            var player2 = new AudioPlayer("Song_2.mp3");
            player2.Stop();
            player2.Dispose(); 
            Console.WriteLine();


            Console.WriteLine("Випадок 3: Об'єкт без Dispose() + робота GC (Деструктор)");
            CreateAndForgetObject();

            Console.WriteLine("Запуск Garbage Collector...");
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\nПрограму завершено.");
        }

        static void CreateAndForgetObject()
        {
            var player3 = new AudioPlayer("Song_3_Forgotten.mp3");

        }
    }
}
