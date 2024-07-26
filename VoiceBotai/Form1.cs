using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VoiceBotai
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer s = new SpeechSynthesizer();
        Boolean wake = true;
        SpeechRecognitionEngine rec = new SpeechRecognitionEngine();
        Choices list = new Choices();
        string name = "DuoSen";
        
        List<string> toDoList = new List<string>();

        List<string> fitnessTips = new List<string>
        {
            "Remember to warm up before exercising to prevent injury.",
            "Stay hydrated and drink plenty of water throughout your workout.",
            "Maintain proper form to maximize benefits and avoid injuries.",
            "Incorporate both cardio and strength training into your routine.",
            "Listen to your body and rest when you need to.",
            "Gradually increase the intensity of your workouts to avoid burnout.",
            "Eat a balanced diet to fuel your workouts and aid recovery.",
            "Consistency is key. Stick to a regular exercise schedule.",
            "Get enough sleep to allow your body to recover and build muscle.",
            "Set realistic goals and track your progress."
        };
        public Form1()
        {
            InitializeComponent();
            list.Add(new string[] { "hello", "how are you", "what time is it", "what is today", "open chrome", "wake up", "sleep", "fine",
                "open spotify", "Yes i need some information", "no thanks", "hi", "Close", "open youtube",
                "tell me a joke", "give me a fun fact", "tell me a story", "Not Good", "How are you feeling", "good",
                "tell me a short story", "yes", "motivate me", "what's the weather", "weather in new york", "weather in london",
                "weather in tokyo", "nice " , "nice work" , "thanks for your information" , "minimize", "normal", "go", "stop", "What's my name" ,
                "do i like math", "next", "last",  "what is one plus one", "what is two plus two",
                "what is three times three", "what is one plus two", "what is five times ten", "hey juno" ,
                " hey introduce yourself", "open chat gpt", "add 'buy groceries' to my to-do list" , "add 'play valorant' to my to-do list", 
                "thanks" ,"give me a fitness tip",  "convert 10 usd to pound", "shut up", "convert 1 dollar to bdt"});



            Grammar gr = new Grammar(new GrammarBuilder(list));


            try
            {
                rec.RequestRecognizerUpdate();
                rec.LoadGrammar(gr);
                rec.SpeechRecognized += rec_SpeechRecognized;
                rec.SetInputToDefaultAudioDevice();
                rec.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing speech recognition: " + ex.Message);
            }

            // Custom voice settings
            s.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
            s.Volume = 70;

            //// Speak the initial messages
            //s.Speak("Hey! my name is Juno and I am created by Neurotech.");
            //s.Speak("Do you need any information?");
        }

        private Dictionary<string, double> exchangeRates = new Dictionary<string, double>()

                {
               { "USD_TO_POUND", 0.85 },
                  { "POUND_TO_USD", 1.18 },
            {"DOLLAR_TO_BDT", 100 },
    // Add more currency pairs as needed
                };
        private void AddToDoItem(string task)
        {
            toDoList.Add(task);
            say($"Added '{task}' to your to-do list.");
        }

        public void say(string h)
        {
            s.Speak(h);
            outputTxtBox.AppendText(h + "\n");
        }

        private void ExecuteCommand(string command)
        {
            switch (command)
            {
                case string cmd when cmd.StartsWith("add"):
                    var task = command.Substring(4).Replace("to my to-do list", "").Trim();
                    AddToDoItem(task);
                    break;

                case "give me a fitness tip":
                    ProvideFitnessTip();
                    break;

                case "shut up":
                    say("Im sorry");
                    break;
                case "wake up":
                    wake = true;
                    label4.Text = "State : Awake";
                    say("I am awake");
                    break;

                case "sleep":
                    wake = false;
                    label4.Text = "State : Sleep mode";
                    say("Going  sleep mode");
                    break;

                case "minimize":
                    this.WindowState = FormWindowState.Minimized;
                    break;

                case "normal":
                    this.WindowState = FormWindowState.Normal;
                    break;

                case "go":
                case "stop":
                    SendKeys.Send(" "); // Send a space key press to simulate play/pause
                    break;

                case "tell me a short story":
                    say("A little girl named Mia found a lost kitten in her garden. The kitten was shivering and scared. " +
                        "Mia gently picked it up and took it home. She fed it warm milk and wrapped it in a soft blanket. " +
                        "The kitten purred happily. From that day on, Mia and the kitten were best friends.");
                    break;

                case "nice":
                    say("I'm glad you liked it!.");
                    break;

                case string cmd when cmd.StartsWith("convert"):
                    var parts = cmd.Split(' ');
                    if (parts.Length == 5 && double.TryParse(parts[1], out double amount))
                    {
                        string fromCurrency = parts[2].ToUpper();
                        string toCurrency = parts[4].ToUpper();
                        try
                        {
                            double result = ConvertCurrency(amount, fromCurrency, toCurrency);
                            say($"{amount} {fromCurrency} is {result:F2} {toCurrency}");
                        }
                        catch (Exception ex)
                        {
                            say(ex.Message);
                        }
                    }
                    else
                    {
                        say("Invalid command format. Please use 'convert <amount> <from_currency> to <to_currency>'.");
                    }
                    break;

                case "thanks":
                    say("I'm glad you liked it!.");
                    break;

                case "hey introduce yourself":
                    say("I'm Juno, your virtual assistant. I'm here to help you with" +
                        " your projects, answer questions, provide information," +
                        " and assist with any tasks you might have. How can I assist you today?");
                    break;

                case "what's my name":
                    say($"Your name is {name}");
                    break;

                case "next":
                    SendKeys.Send("^{RIGHT}");
                    break;

                case "last":
                    SendKeys.Send("^{LEFT}");
                    break;

                case "You're welcome! If you have any more questions in the future, feel free to ask. Have a great day!":
                    say("I'm glad you liked it!.");
                    break;

                case "nice work":
                    say("Thanks!");
                    break;

                case "how are you feeling":
                    say("I’m doing great, thanks for asking! How about you? How’s your project coming along?");
                    break;

                case "good":
                    say("I'm glad to hear that!");
                    break;

                case "not good":
                    say("I'm sorry to hear that.");
                    break;

                case "yes i need some information":
                case "yes":
                    say("Sure, what information do you need?");
                    break;

                case "no thanks":
                    say("Alright, if you need anything in the future, feel free to reach out. Have a great day!");
                    break;

                case "hello":
                    say("Hi! How can I assist you today?");
                    break;

                case "hi":
                    say("Hello there! What can I help you with today?");
                    break;

                case "what time is it":
                    say(DateTime.Now.ToString("h:mm tt"));
                    break;

                case "what is today":
                    say(DateTime.Now.ToString("M/d/yyyy"));
                    break;

                case "how are you":
                    say("Great, and you?");
                    break;

                case "thanks for your information":
                    say("You're welcome! If you have any other questions or need further assistance, feel free to ask!");
                    break;

                case "fine":
                    say("Good to hear it.");
                    break;

                case "close":
                    say("Roger.");
                    this.Close();
                    break;

                case "tell me a joke":
                    say("Why don't scientists trust atoms? Because they make up everything!");
                    break;

                case "give me a fun fact":
                    say("Did you know? Honey never spoils. Archaeologists have found pots of honey in ancient Egyptian tombs that are over 3,000 years old and still edible.");
                    break;

                case "tell me a fun fact":
                    say("The shortest war in history lasted for only 38 minutes! It was between Britain and Zanzibar on August 27, 1896.");
                    break;

                case "open chrome":
                    try
                    {
                        Process.Start("chrome.exe");
                        say("Opening Chrome");
                    }
                    catch (Exception ex)
                    {
                        say("Unable to open Chrome.");
                        MessageBox.Show("Error opening Chrome: " + ex.Message);
                    }
                    break;

                case "open spotify":
                    try
                    {
                        Process.Start("spotify.exe");
                        say("Sure!");
                    }
                    catch (Exception ex)
                    {
                        say("Unable to open Spotify.");
                        MessageBox.Show("Error opening Spotify: " + ex.Message);
                    }
                    break;

                case "open chat gpt":
                    try
                    {
                        Process.Start("https://chatgpt.com/");
                        say("Sure!");
                    }
                    catch (Exception ex)
                    {
                        say("Unable to open Chat GPT.");
                        MessageBox.Show("Error opening Chat GPT: " + ex.Message);
                    }
                    break;

                case "open youtube":
                    try
                    {
                        Process.Start("https://www.youtube.com/");
                        say("Sure!");
                    }
                    catch (Exception ex)
                    {
                        say("Unable to open YouTube.");
                        MessageBox.Show("Error opening YouTube: " + ex.Message);
                    }
                    break;

                case "hey juno":
                    Juno();
                    break;

                case "motivate me":
                    MotivateMe();
                    break;

                case "what's the weather":
                    say("Please specify a city, for example, 'weather in New York'.");
                    break;

                case "weather in new york":
                    say("The weather in New York is sunny with a high of 85 degrees.");
                    WeatherInNewYork();
                    break;

                case "do i like math":
                    DoILikeMath();
                    break;

                case "weather in london":
                    say("The weather in London is cloudy with a chance of rain and a high of 65 degrees.");
                    break;

                case "weather in tokyo":
                    say("The weather in Tokyo is partly cloudy with a high of 75 degrees.");
                    break;

                default:
                    if (IsArithmeticQuestion(command))
                    {
                        CalculateArithmetic(command);
                    }
                    else
                    {
                        say("Sorry, I don't understand that command.");
                    }
                    break;
            }
        }

        private bool IsArithmeticQuestion(string command)
        {
            return Regex.IsMatch(command, @"\b(one|two|three|four|five|six|seven|eight|nine|ten|\d+)\b\s+(plus|minus|times|divided by)\s+\b(one|two|three|four|five|six|seven|eight|nine|ten|\d+)\b", RegexOptions.IgnoreCase);
        }

        private void CalculateArithmetic(string command)
        {
            var numberWords = new Dictionary<string, int>
            {
                { "one", 1 }, { "two", 2 }, { "three", 3 }, { "four", 4 }, { "five", 5 },
                { "six", 6 }, { "seven", 7 }, { "eight", 8 }, { "nine", 9 }, { "ten", 10 }
            };

            var match = Regex.Match(command, @"\b(one|two|three|four|five|six|seven|eight|nine|ten|\d+)\b\s+(plus|minus|times|divided by)\s+\b(one|two|three|four|five|six|seven|eight|nine|ten|\d+)\b", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                int num1 = numberWords.ContainsKey(match.Groups[1].Value.ToLower()) ? numberWords[match.Groups[1].Value.ToLower()] : int.Parse(match.Groups[1].Value);
                int num2 = numberWords.ContainsKey(match.Groups[3].Value.ToLower()) ? numberWords[match.Groups[3].Value.ToLower()] : int.Parse(match.Groups[3].Value);
                string operation = match.Groups[2].Value.ToLower();

                int result = 0;
                switch (operation)
                {
                    case "plus":
                        result = num1 + num2;
                        break;
                    case "minus":
                        result = num1 - num2;
                        break;
                    case "times":
                        result = num1 * num2;
                        break;
                    case "divided by":
                        if (num2 != 0)
                        {
                            result = num1 / num2;
                        }
                        else
                        {
                            say("Division by zero is not allowed.");
                            return;
                        }
                        break;
                }

                say($"{num1} {operation} {num2} is {result}");
            }
        }

        private void MotivateMe()
        {
            string[] quotes = new string[]
            {
                "Believe you can and you're halfway there.",
                "The only way to do great work is to love what you do.",
                "The future belongs to those who believe in the beauty of their dreams.",
                "It does not matter how slowly you go as long as you do not stop.",
                "The harder you work for something, the greater you’ll feel when you achieve it."
            };

            Random rand = new Random();
            int index = rand.Next(quotes.Length);
            say(quotes[index]);
        }

        private void Juno()
        {
            string[] quotes = new string[]
            {
                "Hi there! How can Juno assist you today?",
                "Hi! How can I assist you today?",
            };

            Random rand = new Random();
            int index = rand.Next(quotes.Length);
            say(quotes[index]);
        }

        private void DoILikeMath()
        {
            string[] quotes = new string[]
            {
                "Yes, you like math.",
                "No, you don't like math."
            };

            Random rand = new Random();
            int index = rand.Next(quotes.Length);
            say(quotes[index]);
        }

        private void WeatherInNewYork()
        {
            string[] quotes = new string[]
            {
                "Partly Cloudy.",
                "Mostly sunny in the morning, with scattered thunderstorms in the afternoon.",
                "Cloudy with a chance of rain in the morning, clearing up by the afternoon.",
                "Sunny",
                "Thunderstorms",
                "Clear",
                "Mostly Sunny",
                "Scattered Thunderstorms"
            };

            Random rand = new Random();
            int index = rand.Next(quotes.Length);
            say(quotes[index]);
        }
        private void ProvideFitnessTip()
        {
            Random rand = new Random();
            int index = rand.Next(fitnessTips.Count);
            say(fitnessTips[index]);
        }

        private void rec_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string r = e.Result.Text.ToLower();

            if (r == "wake up")
            {
                wake = true;
                label4.Text = "State : Awake";
                say("I am awake");
            }

            if (r == "sleep")
            {
                wake = false;
                label4.Text = "State : Sleep mode";
                say("Going  sleep mode");
            }

            if (wake)
            {
                ExecuteCommand(r);
            }
        }

        private void InputTxtBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void outputTxtBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void Closetn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Stopbtn_Click(object sender, EventArgs e)
        {
        }
         

        private double ConvertCurrency(double amount, string fromCurrency, string toCurrency)
        {
            string key = $"{fromCurrency.ToUpper()}_TO_{toCurrency.ToUpper()}";
            if (exchangeRates.ContainsKey(key))
            {
                return amount * exchangeRates[key];
            }
            else
            {
                throw new Exception("Conversion rate not available.");
            }
        }
    }
}
