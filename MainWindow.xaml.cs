using COSHH_Generator.Scrapers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace COSHH_Generator
{
    class SubstanceEntry : INotifyPropertyChanged
    {
        public SubstanceEntry()
        {
            DisplayName = "";
        }
        public string _DisplayName = "";
        public string DisplayName { 
            get 
            {
                return _DisplayName;
            }
            set 
            {
                _DisplayName = value;
                OnPropertyChanged("DisplayName");

            } 
        }
        public string query;
        public ObservableCollection<Result> _Results = new ObservableCollection<Result>();
        public ObservableCollection<Result> Results
        {
            get
            {
                return _Results;
            }
        }

        public string Amount { get; set; }
        public string AmountUnit { get; set; }

        public Result? _SelectedResult = null;
        public Result SelectedResult
        {
            set
            {
                Extract(value);
                _SelectedResult = value;
                DisplayName = value.SubstanceName;
            }
        }


        public void Search(in string query)
        {
            if (string.IsNullOrEmpty(query) || this.query == query) return;
            SigmaAldrich.SearchAsync(query, SetResults);
            
            //Trace.WriteLine("searhcing");

        }
        void SetResults(List<SigmaAldrich.Result> results)
        {
            _Results.Clear();
            if (results.Count == 0)
            {
                _Results.Add(new Result { ProductName = "No Results" });
                return;
            }

            foreach (var result in results)
            {
                _Results.Add(new Result
                {
                    ProductName = result.name,
                    Link = null
                });

                for (int j = 0; j < result.products.Count; j++)
                {
                    SigmaAldrich.Result.Product product = result.products[j];
                    _Results.Add(new Result
                    {
                        ProductName = $"{j + 1}. {product.description}",
                        SubstanceName = result.name,
                        Link = product.link,
                    });
                }
            }
            OnPropertyChanged("Results");
        }

        public void Bind(ref TextBox amount, ref ComboBox amountUnit, ref ComboBox resultsComboBox, ref TextBox substance, ref TextBlock substanceName)
        {
            amount.TextChanged += (sender, e) => {
                TextBox? textBox = sender as TextBox;
                if (textBox != null)
                {
                    //MessageBox.Show(textBox.Text, textBox.Text, MessageBoxButton.OK);
                    Amount = textBox.Text;
                }

            };

            amountUnit.SelectionChanged += (sender, args) =>
            {
                //MessageBox.Show("dsads", ((Result)args.AddedItems[0]!).Name, MessageBoxButton.OK);
                AmountUnit = args.AddedItems[0]!.ToString()!;
            };

            resultsComboBox.SetBinding(ComboBox.ItemsSourceProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath("Results"),
                Mode = BindingMode.OneWay

            });

            resultsComboBox.ItemContainerStyle = new Style(typeof(ComboBoxItem))
            {
                Setters =
                  {
                    new Setter(ComboBoxItem.IsEnabledProperty, new Binding("IsSelectable"))
                  }
            };

            resultsComboBox.SelectionChanged += (sender, args) =>
            {
                var addedItems = args.AddedItems;
                if(addedItems.Count > 0)
                {
                    SelectedResult = (Result)args.AddedItems[0]!;
                    
                }
 
            };

            substance.KeyDown += new KeyEventHandler((object sender, KeyEventArgs e) => {
                if (e.Key == Key.Enter)
                {
                    Search(((TextBox)sender).Text);
                }
            });

            substance.LostFocus += (object sender, RoutedEventArgs e) =>
            {
                Search(((TextBox)sender).Text);
                
            };

        }

        public void Extract(in Result substance)
        {
            extractionTask = SDSParser.Extract(substance.Link);
            DisplayName = substance.SubstanceName;
        }
        public Task<SafetyData>? extractionTask = null;
        public event PropertyChangedEventHandler? PropertyChanged;
        internal void OnPropertyChanged([CallerMemberName] string propName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));


        //public CancellationToken cancellationToken;   
    }
    struct Result
    {
        public string SubstanceName { get; set; }
        public string ProductName { get; set; }
        public string? Link { get; set; }
        public bool IsSelectable { get
            {
                return Link != null;
            }
        }

    }
    
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AddNewSubstance(null,null);
            dateTextBox.Text = DateTime.Today.ToString("dd/MM/yyyy");
            configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, ".config");
            try
            {
                var text = File.ReadAllText(configPath).Trim().Split(';');
                nameTextBox.Text = text[0];
                collegeTextBox.Text = text[1];
                yearTextBox.Text = text[2];
            }catch(Exception ex) { }
            //Generate(null, null);
        }

        List<SubstanceEntry> substanceEntries = new List<SubstanceEntry>();

        private void AddNewSubstance(object? sender, RoutedEventArgs? e)
        {
            substanceEntries.Add(new SubstanceEntry());
            
            var index = substanceEntries.Count -1 ;
            
            ListBoxItem item = new ListBoxItem();
            
            var grid = new Grid();
            //grid.VerticalAlignment = VerticalAlignment.Stretch;
            //grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.Width = 690;
            grid.Margin = new Thickness(10);
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(0.5, GridUnitType.Star)
            });
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(0.20, GridUnitType.Star)
            });
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(0.1, GridUnitType.Star)
            });
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(0.2, GridUnitType.Star)
            });


            Button deleteSubstanceButton = new Button()
            {
                Content = "Delete",
                IsTabStop = false
            };
            var substance = substanceEntries.Last();
            deleteSubstanceButton.Click += (sender, e) =>
            {
                substanceEntries.Remove(substance);
                substanceListBox.Items.Remove(item);
            };
            

            var substanceQuery = new TextBox();
            
            
            var amount = new TextBox();
            var amountUnit = new ComboBox();
            
            var substanceName = new TextBlock();

            amountUnit.IsTabStop = false;
            amountUnit.Items.Add("mg");
            amountUnit.Items.Add("mL");
            amountUnit.Items.Add("g");
            amountUnit.Items.Add("cm³");
            amountUnit.Items.Add("L");

            var resultsComboBox = new ComboBox();

            resultsComboBox.IsTabStop = false;
            resultsComboBox.DisplayMemberPath = "ProductName";
            substanceEntries.Last().Bind(ref amount, ref amountUnit, ref resultsComboBox,ref substanceQuery,ref substanceName);
            amount.PreviewGotKeyboardFocus += (sender, e) =>
            {
                if (index + 2 > substanceEntries.Count)
                {
                    AddNewSubstance(null, null);
                }
            };
            substanceQuery.Margin = new Thickness(0, 0, 5, 5);
            grid.Children.Add(substanceQuery);
            Grid.SetRow(substanceQuery, 0);
            Grid.SetColumn(substanceQuery, 0);


            amount.Margin = new Thickness(0, 0, 5, 5);
            grid.Children.Add(amount);
            Grid.SetRow(amount, 0);
            Grid.SetColumn(amount, 1);

            amountUnit.Margin = new Thickness(0, 0, 5, 5);
            grid.Children.Add(amountUnit);
            Grid.SetRow(amountUnit, 0);
            Grid.SetColumn(amountUnit, 2);

            resultsComboBox.Margin = new Thickness(0, 0, 5, 5);
            grid.Children.Add(resultsComboBox);
            Grid.SetRow(resultsComboBox, 1);
            Grid.SetColumn(resultsComboBox, 0);
            Grid.SetColumnSpan(resultsComboBox, 3);

            var displayNameGrid = new Grid();
            displayNameGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            displayNameGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(0.15, GridUnitType.Star)
            });
            displayNameGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(0.85, GridUnitType.Star)
            });
            var displayNameLabel = new TextBlock { Text = "Display Name:" };
            displayNameGrid.Children.Add(displayNameLabel);
            Grid.SetColumn(displayNameLabel, 0);

            var displayName = new TextBox();
            displayName.Margin = new Thickness(0, 0, 5, 0);
            displayNameGrid.Children.Add(displayName);
            Grid.SetColumn(displayName, 1);
            Binding binding = new Binding();
            binding.Path = new PropertyPath("DisplayName");
            binding.Source = substance;
            binding.Mode = BindingMode.TwoWay;
            
            displayName.SetBinding(TextBox.TextProperty, binding);




            grid.Children.Add(displayNameGrid);
            Grid.SetColumn(displayNameGrid, 0);
            Grid.SetRow(displayNameGrid, 2);
            Grid.SetColumnSpan(displayNameGrid, 3);

            grid.Children.Add(deleteSubstanceButton);
            Grid.SetColumn(deleteSubstanceButton, 3);
            Grid.SetRow(deleteSubstanceButton, 0);
            Grid.SetRowSpan(deleteSubstanceButton, 3);



            item.Content = grid;
           
            substanceListBox.Items.Add(item);

        }

        private void Clear(object? sender, RoutedEventArgs? e)
        {
            substanceEntries.Clear();
            substanceListBox.Items.Clear();
            AddNewSubstance(null,null);
        }

        Task? generateTask = null;
        string configPath;
        private void Generate(object? sender, RoutedEventArgs? e)
        {
            generateButton.IsEnabled = false;
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "output.docx");
            generateTask = COSHHForm.Generate(titleTextBox.Text, nameTextBox.Text, collegeTextBox.Text, yearTextBox.Text, dateTextBox.Text,
                fireExplosionCheckbox.IsChecked, thermalRunawayCheckbox.IsChecked, gasReleaseCheckbox.IsChecked, malodorousSubstancesCheckbox.IsChecked, specialMeasuresCheckbox.IsChecked,
                halogenatedCheckBox.IsChecked, hydrocarbonCheckBox.IsChecked, contaminatedCheckBox.IsChecked, aqueousCheckBox.IsChecked, namedWasteCheckBox.IsChecked, silicaTLCCheckBox.IsChecked,
                substanceEntries, path, () =>
                {
                    generateButton.IsEnabled = true;
                    generateTask = null;
                });
            using (StreamWriter sw = new StreamWriter(File.Open(configPath, FileMode.OpenOrCreate)))
            {
                sw.WriteLine($"{nameTextBox.Text};{collegeTextBox.Text};{yearTextBox.Text}");
            }
        }

    }
}
