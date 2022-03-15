using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Timers;
using System.Diagnostics;

namespace simple_reaction_test_game
{
    
    public partial class MainWindow : Window
    {
        private bool game;
        private Random rand;
        int bestTime;
        Timer timerToGreen; // shows green rectangle after it elapses
        Timer verySlow; // used to end the game if player takes too long
        Stopwatch timeFromGreen; // Calculates reaction speed

        public MainWindow()
        {
            InitializeComponent();
            game = false;
            rand = new Random();
            bestTime = -1;
        }

        private void GameRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!game)
            {
                this.Dispatcher.Invoke(() =>
                {
                    GameRectangle.Fill = Brushes.Red;
                    Instructions.Text = "Click on rectangle when it becomes green!";
                });
                
                if(timerToGreen != null)
                {
                    timerToGreen.Enabled = false; // for resetting
                }

                timerToGreen = new Timer(GenerateTime(2, 6)); // green rectangle appears after some time between 2 and 6 seconds
                timerToGreen.Elapsed += TimerToGreen_Elapsed;
                timerToGreen.AutoReset = false;
                timerToGreen.Enabled = true;
                game = true;
            } else
            {
                if(timeFromGreen != null) // if player clicked on rectangle AFTER it became green
                {
                    timeFromGreen.Stop(); // stopping the stopwatch
                    verySlow.Enabled = false; // player is not too slow
                    int tfg = (int)timeFromGreen.ElapsedMilliseconds;
                    LastTime.Text = $"Last time: {tfg}ms";
                    if(bestTime == -1 || tfg < bestTime) // update best time
                    {
                        bestTime = tfg;
                        BestTime.Text = $"Best time: {tfg}ms";
                    }
                    timeFromGreen = null; // resetting stopwatch

                    if(tfg < 220)
                    {
                        MessageBox.Show($"You took {tfg}ms. You are very fast!", "Very fast");
                    } else if(tfg < 310)
                    {
                        MessageBox.Show($"You took {tfg}ms. You have average reaction speed!", "Average");
                    } else
                    {
                        MessageBox.Show($"You took {tfg}ms. You are very slow", "Very slow");
                    }
                }
                else // if player clicked on rectangle BEFORE it became green
                {
                    timerToGreen.Enabled = false;
                    MessageBox.Show("You clicked early. Try again!", "You lost");
                }
                game = false;
                // resetting game
                this.Dispatcher.Invoke(() =>
                {
                    GameRectangle.Fill = Brushes.Gray;
                    Instructions.Text = "Click on gray rectangle to start the game!";
                });
            }
        }

        private void TimerToGreen_Elapsed(object sender, ElapsedEventArgs e)
        {
            // rectangle becomes green, player musk very quickly click on it
            this.Dispatcher.Invoke(() =>
            {
                GameRectangle.Fill = Brushes.Green;
                Instructions.Text = "CLICK NOW!";
            });
            timeFromGreen = new Stopwatch();
            timeFromGreen.Start(); // Starting stopwatch
            verySlow = new Timer(2000); // Starting 2s timer. If player doesn't click rectangle in that time, stop the game
            verySlow.Elapsed += VerySlow_Elapsed;
            verySlow.AutoReset = false;
            verySlow.Enabled = true;
        }

        private void VerySlow_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Resetting stuff and preventing the stopwatch to count to infinity
            timeFromGreen = null;
            game = false;
            this.Dispatcher.Invoke(() =>
            {
                GameRectangle.Fill = Brushes.Gray;
                Instructions.Text = "Click on gray rectangle to start the game!";
                LastTime.Text = "Last time: /";
            });
            MessageBox.Show("You took too long! Try again.", "Took too long");
        }

        private int GenerateTime(int min, int max)
        {
            int randomMs = (int)((rand.NextDouble() * (max - min) + min) * 1000); // Random miliseconds between min and max seconds
            return randomMs;
        }
    }
}
