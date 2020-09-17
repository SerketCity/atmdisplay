/**
 * Program: CSCI293 Final Project
 * Author: John Rice
 * Program Description/Checklist: 
 * 1.Stores ATM#s and PINs in "users.txt", with PIN correlating to ATM# on the same line, separated by whitespace
 * 2.Stores user account data in [ATM#].txt, First line is balance, second is access date, third is daily withdraw limit,
 * all following are successful withdraw transactions
 * (users.xt and [ATM#].txt stored in C:\..\Final Project\bin\DebugFinal Project\bin\Debug)
 * 3.Validates ATM# entered correctly (error popups if too long, too short, or not in users directory)
 * 4.Validates PIN for ATM# (error popups if too long, too short, or if not correct pin)
 * 5.On log in, displays balance and previous 5 transactions for account
 * 6.Able to withdraw at most $1000 at a time, with 10 withdraws a day (error popups if amount is too high, too low (0),
 * isn't a number, has too many decimals, is more than available balance, and if daily limit is reached)
**/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Final_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int screenState = 0;

        string userNum;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LogIn(object sender, RoutedEventArgs e) //function for when log in is clicked
        {
            if(atmBox.Text.Length != 5 || pinBox.Text.Length != 4) //verifies that the atm# and pin are correct size
            {
                if(atmBox.Text.Length > 5)
                {
                    MessageBox.Show("Invalid Input: ATM# Too Long");
                } else if(atmBox.Text.Length < 5)
                {
                    MessageBox.Show("Invalid Input: ATM# Too Short");
                }

                if (pinBox.Text.Length > 4)
                {
                    MessageBox.Show("Invalid Input: PIN# Too Long");
                }else if (pinBox.Text.Length < 4)
                {
                    MessageBox.Show("Invalid Input: PIN# Too Short");
                }
            }
            else
            {
                userNum = atmBox.Text;
                if (!System.IO.File.Exists("users.txt")) //makes sure the pin# directory exists
                {
                    MessageBox.Show("Error: Could not find users directory on system");
                }
                else
                {
                    string[] readText = File.ReadAllLines("users.txt"); //users.txt
                    int counter = 0;
                    while((!readText[counter].Contains(userNum)) && (counter < readText.Length-1)) //looks for entered pin
                    {
                        ++counter;
                    }

                    if(readText[counter].Contains(userNum)) //checks to make sure pin is correct
                    {
                        if(readText[counter].Contains(pinBox.Text)) //changes to a UI for the users account and retrieves account info
                        {
                            AccountSetup();
                            ScreenChange();
                        }
                        else
                        {
                            MessageBox.Show("Incorrect pin!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("ATM# not in directory!");
                    }
                }
            }
        }

        private void ScreenChange() //function for swapping screens
        {
            if(screenState == 0) //sets up the accounts screen if on log in screen
            {
                atmLabel.Visibility = Visibility.Hidden;
                atmBox.Visibility = Visibility.Hidden;
                pinLabel.Visibility = Visibility.Hidden;
                pinBox.Visibility = Visibility.Hidden;
                loginButton.Visibility = Visibility.Hidden;
                logoutButton.Visibility = Visibility.Visible;
                withdrawButton.Visibility = Visibility.Visible;
                withdrawBox.Visibility = Visibility.Visible;
                balanceLabel.Visibility = Visibility.Visible;
                balanceTotalLabel.Visibility = Visibility.Visible;
                transactionsLabel.Visibility = Visibility.Visible;
                transactionsList.Visibility = Visibility.Visible;
                screenState = 1;
            }
            else //sets up the log in screen if on account screen
            {
                atmLabel.Visibility = Visibility.Visible;
                atmBox.Visibility = Visibility.Visible;
                pinLabel.Visibility = Visibility.Visible;
                pinBox.Visibility = Visibility.Visible;
                loginButton.Visibility = Visibility.Visible;
                logoutButton.Visibility = Visibility.Hidden;
                withdrawButton.Visibility = Visibility.Hidden;
                withdrawBox.Visibility = Visibility.Hidden;
                balanceLabel.Visibility = Visibility.Hidden;
                balanceTotalLabel.Visibility = Visibility.Hidden;
                transactionsLabel.Visibility = Visibility.Hidden;
                transactionsList.Visibility = Visibility.Hidden;
                screenState = 0;
            }
        }

        private void LogOut(object sender, RoutedEventArgs e) //goes to log in screen when log out is clicked
        {
            atmBox.Text = null;
            pinBox.Text = null;
            ScreenChange();
        }

        private void Withdraw(object sender, RoutedEventArgs e) //actions for when withdraw is clicked
        {
            double i;
            if(double.TryParse(withdrawBox.Text, out i)) //checks to make sure a number was entered
            {
                var num = decimal.Parse(withdrawBox.Text);
                if (decimal.Round(num, 2) == num) //checks to make sure input doesnt have more than two decimals
                {
                    if (num > 1000 || num == 0) //checks that number is at most 1000 and at least 0.01
                    {
                        MessageBox.Show("Cannot withdraw more than $1000 or less than $0.01!");
                    }
                    else
                    {
                        string[] readText = File.ReadAllLines(userNum + ".txt"); 
                        if (int.Parse(readText[2]) == 0) //checks to make sure not at withdraw limit
                        {
                            MessageBox.Show("Only 10 withdraws allowed per day!");
                        }
                        else
                        {
                            WithdrawFunc(num);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Error: Too many decimals.");
                }
            }
            else
            {
                MessageBox.Show("Error: Withdraw amount not a number.");
            }
            
        }

        private void AccountSetup() //retrieves information on user accounts from user file
        {
            if (!System.IO.File.Exists(userNum + ".txt")) //if the user file doesnt exist, it creates one
            {
                string[] lines = { "100000", DateTime.Now.ToString("MM/dd/yyyy"), "10"};
                System.IO.File.WriteAllLines(userNum + ".txt", lines);
            }
            else
            {
                string[] readText = File.ReadAllLines(userNum + ".txt");
                balanceTotalLabel.Content = "$" + readText[0];
                if(!readText[1].Contains(DateTime.Now.ToString("MM/dd/yyyy"))) //checks to make sure access date is correct, if not it refreshes date and withdraw limi
                {
                    readText[1] = DateTime.Now.ToString("MM/dd/yyyy");
                    readText[2] = "10";
                    System.IO.File.WriteAllLines(userNum + ".txt", readText);
                }
                int stop = 5;
                if ((readText.Length - 4) < 5) //prep for obtaining transactions
                {
                    stop = readText.Length - 3;
                }
                transactionsList.Content = readText[readText.Length - 1] + "\n";
                for (int i = 0; i < stop - 1; i++) //retrieves newest 5 transactions
                {
                    transactionsList.Content += readText[(readText.Length - 2) - i] + "\n";
                }
            }
        }

        private void WithdrawFunc(decimal n) //function for handling withdraws
        {
            string[] readText = File.ReadAllLines(userNum + ".txt");
            var num = decimal.Parse(readText[0]);
            if((num - n) < 0) //makes sure withdraw amount is available in balance
            {
                MessageBox.Show("Cannot withdraw more than balance amount!");
            }
            else
            {
                readText[0] = (num - n).ToString();
                balanceTotalLabel.Content = "$" + readText[0];
                readText[2] = (int.Parse(readText[2]) - 1).ToString();
                System.IO.File.WriteAllLines(userNum + ".txt", readText); //adds newest transaction to file
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(userNum + ".txt", true)) //adds transaction to transaction list
                {
                    file.WriteLine(DateTime.Now.ToString("MM/dd/yyyy") + " $" + n.ToString());
                }
                readText = File.ReadAllLines(userNum + ".txt"); //retrieves edited file
                int stop = 5;
                if ((readText.Length - 4) < 5)
                {
                    stop = readText.Length - 3;
                }
                transactionsList.Content = readText[readText.Length - 1] + "\n";
                for (int i = 0; i < stop - 1; i++) //makes sure new transaction is added to ui
                {
                    transactionsList.Content += readText[(readText.Length - 2) - i] + "\n"; 
                }
            }
        }
    }
}
