/*
 * Speech to text command line utility
 * Uses Microsoft Cognitive Services api for speech recognition
 * Based on example code from https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/quickstart-csharp-dotnetcore-windows
 * 
 * Usage: speechtotext <mediafilename.wav>
 * 
 * Remember to put your API key (free) from microsoft in the accompanying speechtotext config file. (speechtotext_config_file.json)
 *
 * Written by P Lishman for the dissertation of Jon Lishman, April 2019. License: GPL3
*/

using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Configuration;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace speechtotext
{
    class Program
    {
        public static async Task RecognizeSpeechAsync(String subscription_key, String endpoint, String filename, String outfilename, String recognition_language, String region)
        {
            var source = new TaskCompletionSource<int>();
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key // and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription(subscription_key, region);
            config.SpeechRecognitionLanguage = recognition_language;
            
            // Creates a speech recognizer.
            using (var audioInput = AudioConfig.FromWavFileInput(filename))
            {
                using (var recognizer = new SpeechRecognizer(config, audioInput))
                {
                    //Console.WriteLine("Say something...");

                    // Starts speech recognition, and returns after a single utterance is recognized. The end of a
                    // single utterance is determined by listening for silence at the end or until a maximum of 15
                    // seconds of audio is processed.  The task returns the recognition text as result. 
                    // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                    // shot recognition like command or query. 
                    // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead (PLL 2019 now uses this and event handlers)
                    EventHandler<SpeechRecognitionEventArgs> recognizedHandler = (sender, e) => RecognizedEventHandler(e, outfilename);
                    EventHandler<SpeechRecognitionCanceledEventArgs> canceledHandler = (sender, e) => CanceledEventHandler(e, source);
                    EventHandler<SessionEventArgs> sessionStartedHandler = (sender, e) => SessionStartedEventHandler(e);
                    EventHandler<SessionEventArgs> sessionStoppedHandler = (sender, e) => SessionStoppedEventHandler(e, source);
                    EventHandler<RecognitionEventArgs> speechStartDetectedHandler = (sender, e) => SpeechDetectedEventHandler(e, "start");
                    EventHandler<RecognitionEventArgs> speechEndDetectedHandler = (sender, e) => SpeechDetectedEventHandler(e, "end");
                    recognizer.Recognized += recognizedHandler;
                    recognizer.Canceled += canceledHandler;
                    recognizer.SessionStarted += sessionStartedHandler;
                    recognizer.SessionStopped += sessionStoppedHandler;
                    recognizer.SpeechStartDetected -= speechStartDetectedHandler;
                    recognizer.SpeechEndDetected -= speechEndDetectedHandler;

                    //await recognizer.StartContinuousRecognitionAsync();
                    await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
                    await source.Task.ConfigureAwait(false);
                    await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);

                    recognizer.Recognized -= recognizedHandler;
                    recognizer.Canceled -= canceledHandler;
                    recognizer.SessionStarted -= sessionStartedHandler;
                    recognizer.SessionStopped -= sessionStoppedHandler;
                    recognizer.SpeechStartDetected -= speechStartDetectedHandler;
                    recognizer.SpeechEndDetected -= speechEndDetectedHandler;

                }
            }
        }

        /// <summary>
        /// Logs the final recognition result
        /// </summary>
        private static void RecognizedEventHandler(SpeechRecognitionEventArgs e, string outfilename)
        {
            //this.WriteLine(log);
            //this.WriteLine(log, $" --- Final result received. Reason: {e.Result.Reason.ToString()}. --- ");
            if (!string.IsNullOrEmpty(e.Result.Text))
            {
                Console.WriteLine(e.Result.Text);
                Console.WriteLine();

                // Write the text to a new file named "WriteFile.txt".
                String output = e.Result.Text + Environment.NewLine + Environment.NewLine;
                File.AppendAllText(outfilename, output);
            }
        }

        /// <summary>
        /// Logs Canceled events
        /// And sets the TaskCompletionSource to 0, in order to trigger Recognition Stop
        /// </summary>
        private static void CanceledEventHandler(SpeechRecognitionCanceledEventArgs e, TaskCompletionSource<int> source)
        {
            source.TrySetResult(0);
            if (e.Reason == CancellationReason.EndOfStream)
            {
                Console. WriteLine("--- recognition completed (end of stream) ---");
            }
            else
            {

                Console.WriteLine("--- recognition canceled ---");
                Console.WriteLine($"CancellationReason: {e.Reason.ToString()}. ErrorDetails: {e.ErrorDetails}.");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Session started event handler.
        /// </summary>
        private static void SessionStartedEventHandler(SessionEventArgs e)
        {
            Console.WriteLine(String.Format(CultureInfo.InvariantCulture, "Speech recognition: Session started event: {0}.", e.ToString()));
        }

        /// <summary>
        /// Session stopped event handler. Set the TaskCompletionSource to 0, in order to trigger Recognition Stop
        /// </summary>
        private static void SessionStoppedEventHandler(SessionEventArgs e, TaskCompletionSource<int> source)
        {
            Console.WriteLine(String.Format(CultureInfo.InvariantCulture, "Speech recognition: Session stopped event: {0}.", e.ToString()));
            source.TrySetResult(0);
        }

        private static void SpeechDetectedEventHandler(RecognitionEventArgs e, string eventType)
        {
            Console.WriteLine(String.Format(CultureInfo.InvariantCulture, "Speech recognition: Speech {0} detected event: {1}.", eventType, e.ToString()));
        }

        static void Main()
        {
            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("speechtotext_config_file.json");
                var configuration = builder.Build();

                var subscription_key = configuration["subscription_key"];
                var endpoint = configuration["endpoint"];
                var recognition_language = configuration["recognition_language"];
                var region = configuration["region"];

                String[] arguments = Environment.GetCommandLineArgs();
                if (arguments.Length != 2)
                {
                    Console.WriteLine("Incorrect number of arguments");
                    return;
                }

                var filename = arguments[1];
                var outfilename = arguments[1] + ".txt";

                if (File.Exists(outfilename)) {
                    File.Delete(outfilename);
                }

                RecognizeSpeechAsync(subscription_key, endpoint, filename, outfilename, recognition_language, region).Wait();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Please press a key to continue.");
            Console.ReadLine();
        }
    }
}