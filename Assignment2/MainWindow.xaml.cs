﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Assignment2
{
    //public class Article
    //{
    //    [Required]
    //    public string Title { get; set; }
    //    public DateTime Date { get; set; }
    //    public string URLTitle { get; set; }

    //}
    public partial class MainWindow : Window
    {
        private Thickness spacing = new Thickness(5);
        private HttpClient http = new HttpClient();
        // We will need these as instance variables to access in event handlers.
        private TextBox addFeedTextBox;
        private Button addFeedButton;
        private ComboBox selectFeedComboBox;
        private Button loadArticlesButton;
        private StackPanel articlePanel;

        // Do we need this list here?
        private List<string> urls = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Added region for convenience of hiding this section
            #region Design
            // Window options
            Title = "Feed Reader";
            Width = 800;
            Height = 400;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            var root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            var grid = new Grid();
            root.Content = grid;
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var addFeedLabel = new Label
            {
                Content = "Feed URL:",
                Margin = spacing
            };
            grid.Children.Add(addFeedLabel);

            addFeedTextBox = new TextBox
            {
                Margin = spacing,
                Padding = spacing
            };
            grid.Children.Add(addFeedTextBox);
            Grid.SetColumn(addFeedTextBox, 1);

            addFeedButton = new Button
            {
                Content = "Add Feed",
                Margin = spacing,
                Padding = spacing
            };
            grid.Children.Add(addFeedButton);
            Grid.SetColumn(addFeedButton, 2);

            addFeedButton.Click += AddFeed;

            var selectFeedLabel = new Label
            {
                Content = "Select Feed:",
                Margin = spacing
            };
            grid.Children.Add(selectFeedLabel);
            Grid.SetRow(selectFeedLabel, 1);

            selectFeedComboBox = new ComboBox
            {
                Margin = spacing,
                Padding = spacing,
                IsEditable = false
            };
            grid.Children.Add(selectFeedComboBox);
            Grid.SetRow(selectFeedComboBox, 1);
            Grid.SetColumn(selectFeedComboBox, 1);

            // Added this for default option with all feeds
            selectFeedComboBox.SelectedIndex = 0;
            selectFeedComboBox.Items.Add("All Feeds");

            loadArticlesButton = new Button
            {
                Content = "Load Articles",
                Margin = spacing,
                Padding = spacing
            };
            grid.Children.Add(loadArticlesButton);
            Grid.SetRow(loadArticlesButton, 1);
            Grid.SetColumn(loadArticlesButton, 2);

            // A method for the button-click event that i made
            loadArticlesButton.Click += LoadArticles;

            articlePanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = spacing
            };
            grid.Children.Add(articlePanel);
            Grid.SetRow(articlePanel, 2);
            Grid.SetColumnSpan(articlePanel, 3);
            #endregion

            // These are just placeholders.
            // Replace them with your own code that shows actual articles.
            for (int i = 0; i < 3; i++)
            {
                var articlePlaceholder = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = spacing
                };
                articlePanel.Children.Add(articlePlaceholder);

                var articleTitle = new TextBlock
                {
                    Text = "2021-01-02 12:34 - Placeholder for an actual article title #" + (i + 1),
                    FontWeight = FontWeights.Bold,
                    TextTrimming = TextTrimming.CharacterEllipsis
                };
                articlePlaceholder.Children.Add(articleTitle);

                var articleWebsite = new TextBlock
                {
                    Text = "Website name #" + (i + 1)
                };
                articlePlaceholder.Children.Add(articleWebsite);
            }
        }

        // Example/testmethods
        private void LoadArticles(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This button should load 5 articles");
            // EXAMPLE FROM XML - IS THIS THE RIGHT PLACE TO PUT IT?
            //var document = XDocument.Load("Example.xml");

            //// Get the title of the first movie as a string.
            //string firstTitle = document.Descendants("title").First().Value;

            //// Get all titles as an array of strings.
            //string[] allTitles = document.Descendants("title").Select(t => t.Value).ToArray();

            //// We can loop over Descendants with foreach.
            //var eightiesTitles = new List<string>();
            //foreach (var movie in document.Descendants("movie"))
            //{
            //    string title = movie.Descendants("title").First().Value;
            //    int year = int.Parse(movie.Descendants("year").First().Value);
            //    if (year >= 1980 && year < 1990)
            //    {
            //        eightiesTitles.Add(title);
            //    }
            //}

            // We should use the current added url here to list 5 articles from it in the interface/GUI
        }

        // Example/testmethods
        private async void AddFeed(object sender, RoutedEventArgs e)
        {
            // Button is disabled when it is clicked
            addFeedButton.IsEnabled = false;
            loadArticlesButton.IsEnabled = false;

            // Should we use the pre-made LoadDocumentAsync-method here instead of this delay?

            string text = addFeedTextBox.Text;
            await LoadDocumentAsync(text);

            string firstTitle = LoadDocumentAsync(text).Descendants("title").First().Value;


            // Button is active again after the delay
            addFeedButton.IsEnabled = true;
            loadArticlesButton.IsEnabled = true;



            if (addFeedTextBox.Text == "")
            {
                MessageBox.Show("Please enter a valid URL");
            }
            else
            {
                // Clears the container after we have clicked the button and added them to the combobox
                addFeedTextBox.Clear();

                MessageBox.Show("This button should add the given URL to the feed and add it to the combobox.");
                // Maybe we should add the urls like this to this list so that we can loop them in the GUI-interface down below?
                urls.Add(text);
                
                selectFeedComboBox.Items.Add(text);
                selectFeedComboBox.SelectedItem = text;
            }
        }

        // We should use this premade method to simulate the async-time
        private async Task<XDocument> LoadDocumentAsync(string url)
        {
            // This is just to simulate a slow/large data transfer and make testing easier.
            // Remove it if you want to.
            await Task.Delay(1000);
            var response = await http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            var feed = XDocument.Load(stream);
            return feed;
        }
    }
}
