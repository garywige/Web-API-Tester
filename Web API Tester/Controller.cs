using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Web_API_Tester
{
    public class Controller
    {
        // singleton
        private Controller()
        {
            _client = new();
        }

        ~Controller()
        {
            _client.Dispose();
            if (Response != null) Response.Dispose();
        }

        // public interface
        public static Controller Instance
        {
            get
            {
                if (_controller == null)
                    _controller = new();
                return _controller;
            }
        }

        public enum HttpMethod
        {
            Get,
            Post,
            Put,
            Delete,
            Patch
        }

        public async Task RunLoop()
        {
            // enter loop
            do
            {
                // prompt user for URL
                URL = PromptUrl();

                // prompt user what HTTP method to use
                Method = PromptMethod();

                // prompt user for parameters for method
                //Parameter = PromptParameter();

                // send request
                await SendRequest();

                // output response
                if(Response != null)
                    Console.WriteLine(Response);

            } while (PromptContinue()); // prompt user to continue
        }

        // private interface
        private HttpMethod Method { get; set; }
        private string URL { get; set; }
        private string Parameter { get; set; }
        private HttpResponseMessage Response { get; set; }

        private async Task SendRequest()
        {
            try
            {
                // behavior depends on method
                Response = Method switch 
                {
                    HttpMethod.Get => await _client.GetAsync(URL),
                    _ => throw new NotImplementedException()
                };
            }
            catch(HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static bool PromptContinue()
        {
            while (true)
            {
                // output and get key press
                Console.Write("Would you like to run another test (y/n)? ");
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                Console.WriteLine();

                // validate
                if (keyInfo.Key != ConsoleKey.Y && keyInfo.Key != ConsoleKey.N)
                    continue;

                // process input
                if (keyInfo.Key == ConsoleKey.N)
                    return false;

                // prompt was successful
                return true;
            }
        }

        private HttpMethod PromptMethod()
        {
            bool isSuccessful = false;
            HttpMethod method = HttpMethod.Get;
            while(!isSuccessful)
            {
                Console.Write("Which HTTP method to you wish to test? ");
                string input = Console.ReadLine();

                switch(input.ToLower())
                {
                    case "get":
                        method = HttpMethod.Get;
                        isSuccessful = true;
                        break;
                    default:
                        continue;
                }                
            }

            return method;
        }

        private static string PromptUrl()
        {
            bool isSuccessful = false;
            string input = null;
            while(!isSuccessful)
            {
                Console.Write("Which URL would you like to use for the request? ");
                input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) continue;

                isSuccessful = true;
            }

            return input;
        }

        private static string PromptParameter()
        {
            Console.Write("What parameter will be sent along with the method, if any? ");
            return Console.ReadLine();
        }

        // private members
        private static Controller _controller;
        private readonly HttpClient _client;
    }
}
