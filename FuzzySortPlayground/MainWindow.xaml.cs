using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FuzzySortLib;

namespace FuzzySortPlayground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<ListItem> InputList { get; set; }

        private bool _appInit = false;
        FuzzySorter mainSorter;

        public MainWindow()
        {
            InitializeComponent();
            InputListTextBox.Text = "";
            _appInit = true;
            mainSorter = new FuzzySorter(analyzeData: true, detectReverseOrderOriginals: false, descendingOrder: false, zeroIsSortable: true);
            ReadOptions();
            SortAndUpdate();
        }

        private void SortAndUpdate(bool updateAnalyzer = true)
        {
            if (_appInit) {
                ReadOptions();
                List<ListItem> content = DataParser.ParseListString(InputListTextBox.Text);
                List<ListItem> sortedContent = mainSorter.Sorted(content);

                ShowOriginalOrResultList(OriginalView, content);
                ShowOriginalOrResultList(ResultView, sortedContent);
                if (mainSorter.LastSortingData != null) {

                    TrendValueLabel.Content = mainSorter.LastSortingData.OriginalTrend.ToString();
                    OriginalOrderValueLabel.Content = mainSorter.LastSortingData.OriginalAscending ? "Ascending" : "Descending";
                    QualityValueLabel.Content = mainSorter.LastSortingData.Quality.ToString();
                    SlopeValueLabel.Content = mainSorter.LastSortingData.OriginalSlope.ToString();
                    EQValueLabel.Content = mainSorter.LastSortingData.OverallEvaluationQuality.ToString();

                    if (mainSorter.LastSortingData.DeterminativeTrend < mainSorter.LastSortingData.TrendThreshold) {
                        TrendValueLabel.Foreground = Brushes.Red;
                    } else {
                        TrendValueLabel.Foreground = Brushes.Black;
                    }
                    if (mainSorter.LastSortingData.Quality < mainSorter.LastSortingData.QualityThreshold) {
                        QualityValueLabel.Foreground = Brushes.Red;
                    } else {
                        QualityValueLabel.Foreground = Brushes.Black;
                    }
                    if (mainSorter.LastSortingData.OverallEvaluationQuality < mainSorter.LastSortingData.EvaluationQualityThreshold) {
                        EQValueLabel.Foreground = Brushes.Red;
                    } else {
                        EQValueLabel.Foreground = Brushes.Black;
                    }

                    //double test = mainSorter.LastSortingData.OrderTrend;

                    //if (test < 0)
                    //    TrendValueLabel.Content = "negative";
                    if (updateAnalyzer)
                        ShowAnalyzerData();
                }
            }
        }

        // kaksi nopeasti päivittyvää listaa
        private void ShowOriginalOrResultList(WrapPanel panel, List<ListItem> list)
        {
            if (!_appInit)
                return;

            panel.Children.Clear();
            foreach (ListItem item in list) {
                Label newLabel = MakeListItemLabel(item.TextValue);
                panel.Children.Add(newLabel);
            }
        }

        private Label MakeListItemLabel(string value, bool border = false, Brush color = null, bool underline = false, bool doubleUnderline = false, bool bold = false, bool italic = false, bool strikethrough = false)
        {
            Label myLabel = new Label();
            Brush itemColor = Brushes.Black;
            if (color != null)
                itemColor = color;
            myLabel.MinWidth = 38;
            myLabel.Height = 20;
            myLabel.HorizontalAlignment = HorizontalAlignment.Left;
            myLabel.HorizontalContentAlignment = HorizontalAlignment.Right;
            myLabel.VerticalContentAlignment = VerticalAlignment.Center;
            myLabel.Background = Brushes.Transparent;
            myLabel.Padding = new Thickness(5, 0, 5, 0);
            myLabel.Margin = new Thickness(3);
            myLabel.BorderBrush = itemColor;
            myLabel.BorderThickness = new Thickness(border ? 1 : 0);
            myLabel.Foreground = itemColor;
            myLabel.FontSize = 14;
            myLabel.FontWeight = bold ? FontWeights.Bold : FontWeights.Normal;
            myLabel.FontStyle = italic ? FontStyles.Italic : FontStyles.Normal;
            TextBlock labelBlock = new TextBlock();

            if (underline || doubleUnderline) {
                // Create a linear gradient pen for the text decoration.
                Pen myPen = new Pen();
                myPen.Brush = itemColor;
                //Brush transparentBrush = new Brush();
                //myPen.Brush.Opacity = 0.5;
                myPen.Thickness = 1;

                //myPen.DashStyle = DashStyles.Dash;
                TextDecoration myUnderline = new TextDecoration();
                myUnderline.Pen = myPen;
                myUnderline.PenThicknessUnit = TextDecorationUnit.FontRecommended;
                TextDecorationCollection myCollection = new TextDecorationCollection();

                myUnderline.PenOffset = 0;
                myCollection.Add(myUnderline);
                if (doubleUnderline) {
                    TextDecoration myUnderline2 = new TextDecoration();
                    myUnderline2.PenThicknessUnit = TextDecorationUnit.FontRecommended;
                    myUnderline2.PenOffset = 2;
                    myCollection.Add(myUnderline2);
                }

                labelBlock.TextDecorations = myCollection;
            }

            if (strikethrough) {
                // Create a linear gradient pen for the text decoration.
                Pen myPen = new Pen();
                myPen.Brush = itemColor;
                //Brush transparentBrush = new Brush();
                //myPen.Brush.Opacity = 0.5;
                myPen.Thickness = 1;
                //myPen.DashStyle = DashStyles.Dash;
                TextDecoration myStrikeThrough = new TextDecoration();
                myStrikeThrough.Pen = myPen;
                myStrikeThrough.PenThicknessUnit = TextDecorationUnit.FontRecommended;
                myStrikeThrough.Location = TextDecorationLocation.Strikethrough;
                myStrikeThrough.PenOffset = 0;
                TextDecorationCollection myCollection = new TextDecorationCollection();
                myCollection.Add(myStrikeThrough);
                labelBlock.TextDecorations = myCollection;
            }

            labelBlock.Text = value;
            myLabel.Content = labelBlock;

            return myLabel;
        }

        private WrapPanel MakeAnalyzerListArea(EvaluationData dropData, bool after = false)
        {
            WrapPanel panel = new WrapPanel();
            //panel.Name = "";
            List<SortAgent> sortList = after ? dropData.ListAfter : dropData.ListBefore;

            foreach (SortAgent agent in sortList) {

                int index = agent.OriginalIndex;
                ListItem originalItem = (ListItem)agent.OriginalItem;
                string strValue = originalItem.TextValue;

                bool bold = false;
                bool underline = false;
                bool doubleUnderline = false;
                bool border = false;
                bool italic = false;
                bool strikeThrough = false;

                Brush color = null;
                if (index == dropData.Target.OriginalIndex) {
                    border = true;
                    if (after) {
                        strValue = dropData.ListAfter[index].SortValue.ToString();
                    }
                }

                if (agent.Droppable)
                    bold = true;

                if (agent.ValueExceeds && (index != dropData.Target.OriginalIndex || !agent.Evaluated)) {
                    strikeThrough = true; ;
                }

                if (agent.UnSortable) {
                    color = Brushes.Gray;
                    italic = true;
                }
                if (agent.Dropped)
                    color = Brushes.LightGray;

                if (!after) {
                    if (IsInSortAgentList(dropData.HighMin, agent) || IsInSortAgentList(dropData.LowMax, agent)) {
                        if (dropData.DeterminativeCriterion != null) {
                            color = Brushes.Blue;
                            if (dropData.DeterminativeCriterion.Side == DropSide.Low && IsInSortAgentList(dropData.LowMax, agent))
                                color = Brushes.Red;
                            if (dropData.DeterminativeCriterion.Side == DropSide.High && IsInSortAgentList(dropData.HighMin, agent))
                                color = Brushes.Red;
                        }
                        doubleUnderline = true;
                    }

                    if (IsInSortAgentList(dropData.HighMin2, agent) || IsInSortAgentList(dropData.LowMax2, agent))
                        underline = true;
                } else {
                    if (IsInSortAgentList(dropData.HighMin, agent) || IsInSortAgentList(dropData.LowMax, agent)) {
                        doubleUnderline = true;
                    }
                }

                panel.Children.Add(MakeListItemLabel(strValue, border, color, underline, doubleUnderline, bold, italic, strikeThrough));
            }

            // gap between lines
            if (!after)
                panel.Margin = new Thickness(5, 0, 0, 0);
            else
                panel.Margin = new Thickness(5, 0, 0, 15);

            return panel;
        }

        private void ShowAnalyzerData()
        {
            if (!_appInit)
                return;

            AnalyzerPanel.Children.Clear();
            if (mainSorter.LastSortingData == null) {
                return;
            }

            // go through all evaluated values
            foreach (Evaluation evaluation in mainSorter.LastSortingData.Evaluations) {
                int index = 0;
                if (evaluation.Drops.Count > 0) {
                    // go through all dropped values
                    foreach (EvaluationData dropData in evaluation.Drops) {
                        AnalyzerPanel.Children.Add(MakeAnalyzerInfoArea("Dropping", dropData, after: false));
                        AnalyzerPanel.Children.Add(MakeAnalyzerListArea(dropData, after: false));
                        if (index == evaluation.Drops.Count - 1) {
                            AnalyzerPanel.Children.Add(MakeAnalyzerInfoArea("Result", evaluation.EvaluationData, after: true));
                            AnalyzerPanel.Children.Add(MakeAnalyzerListArea(evaluation.EvaluationData, after: true));
                        }
                        index++;
                    }
                } else {
                    AnalyzerPanel.Children.Add(MakeAnalyzerInfoArea("Simple Evaluation", evaluation.EvaluationData, after: false));
                    AnalyzerPanel.Children.Add(MakeAnalyzerListArea(evaluation.EvaluationData, after: false));

                    AnalyzerPanel.Children.Add(MakeAnalyzerInfoArea("Result", evaluation.EvaluationData, after: true));
                    AnalyzerPanel.Children.Add(MakeAnalyzerListArea(evaluation.EvaluationData, after: true));
                }
            }
        }

        private StackPanel MakeAnalyzerInfoArea(string message, EvaluationData dropData, bool after)
        {
            StackPanel stackPanel = new StackPanel();

            if (dropData.DroppingDone) {
                // data about drop
                string infotxt = "";
                string detRule = DropCriterion.ToString(dropData.DeterminativeCriterion.Type);

                infotxt = ListUsedCriteria(dropData);

                stackPanel.Children.Add(MakeMakeAnalyzerInfoLabel(message + " by " + detRule + ": " + DropCriterion.ToString(dropData.Side).ToUpper() +
                    " with Quality: " + Math.Round(dropData.DroppingQuality, 3)));

                stackPanel.Children.Add(MakeMakeAnalyzerInfoLabel("Criteria: " + infotxt));

            } else {
                if (after) {
                    stackPanel.Children.Add(MakeMakeAnalyzerInfoLabel(message + " with Overall Quality: " + Math.Round(dropData.Evaluation.EvaluationQuality,3)));
                } else {
                    stackPanel.Children.Add(MakeMakeAnalyzerInfoLabel(message));
                }

            }

            return stackPanel;

        }

        private string ListUsedCriteria(EvaluationData dropData)
        {
            string result = "";
            foreach (DropCriterion criterion in dropData.Criteria) {
                string title = DropCriterion.ToString(criterion.Type);
                string side = DropCriterion.ToString(criterion.Side).ToUpper();
                string score = "";
                if (criterion.Type == DropCriterionType.Displacement) {
                    score = " [" + ScoreString(dropData.DisplacementLow, dropData.DisplacementHigh) + "]";
                }
                if (criterion.Type == DropCriterionType.Distance) {
                    score = " [" + ScoreString(dropData.DistanceLow, dropData.DistanceHigh) + "]";
                }
                if (criterion.Type == DropCriterionType.Droppables) {
                    score = " [" + ScoreString(dropData.DroppablesLow, dropData.DroppablesHigh) + "]";
                }
                if (criterion.Type == DropCriterionType.OrderQuality) {
                    score = " [" + ScoreString(dropData.QualityDroppingLow, dropData.QualityDroppingHigh, 3) + "]";
                }
                if (dropData.DeterminativeCriterion.Type == DropCriterionType.Vote && !criterion.UsedForVoting) {
                        result += "(" + title + score + ": " + side + ") / ";
                } else {
                    result += title + score + ": " + side + " / ";
                }
            }
            return result.Trim().Trim(',').Trim('/').Trim();
        }

        private Label MakeMakeAnalyzerInfoLabel(string text)
        {
            Label label = new Label();
            label.Content = text;
            label.FontSize = 11;
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.Margin = new Thickness(0);
            label.Padding = new Thickness(0);
            label.Foreground = new SolidColorBrush(Color.FromArgb(255, 110,110,110));
            return label;
        }

        private string ScoreString(object value1, object value2, int decimals = 1)
        {
            return NumericValueToString(value1, decimals) + ":" + NumericValueToString(value2, decimals);
        }

        private string NumericValueToString(object value, int decimals = 1)
        {
            if (value.GetType() == typeof(double) || value.GetType() == typeof(float)) {
                double val = (double)value;
                return Math.Round(val, decimals).ToString();
            } else {
                return value.ToString();
            }
        }

        private void ReadOptions()
        {
            if (!_appInit)
                return;
            mainSorter.LowLimit = DataParser.ParseNumericInput(LowLimitTextBox.Text, System.Double.NegativeInfinity);
            mainSorter.HighLimit = DataParser.ParseNumericInput(HighLimitTextBox.Text, System.Double.PositiveInfinity);
            mainSorter.MaxRange = DataParser.ParseNumericInput(RangeTextBox.Text, System.Double.PositiveInfinity);
            mainSorter.MaxDeviation = DataParser.ParseNumericInput(DeviationTextBox.Text, System.Double.PositiveInfinity);
            mainSorter.DescendingOrder = OrderComboBox.SelectedIndex == 1;
            mainSorter.DetectReverseOrderOriginals = ReverseCheckBox.IsChecked ?? false;
            mainSorter.SortUnsortablesLast = UnsortableComboBox.SelectedIndex == 1;
            mainSorter.TrendThreshold = DataParser.ParseNumericInput(TrendThresholdTextBox.Text, double.NegativeInfinity);
            mainSorter.OrderQualityThreshold = DataParser.ParseNumericInput(QualityThresholdTextBox.Text, double.NegativeInfinity);
            mainSorter.EvaluationQualityThreshold = DataParser.ParseNumericInput(EQThresholdTextBox.Text, double.NegativeInfinity);
            mainSorter.ExceedingAsUnsortables = ExceedingComboBox.SelectedIndex == 1;
        }

        private void InputListField_TextChanged(object sender, TextChangedEventArgs e)
        {
            SortAndUpdate(false);
        }

        private void InputListField_LostFocus(object sender, EventArgs e)
        {
            SortAndUpdate();
        }

        private void OrderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortAndUpdate();
        }

        private void ReverseCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SortAndUpdate();
        }

        private void ReverseCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SortAndUpdate();
        }

        private void UnsortableComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortAndUpdate();
        }

        private void ExceedingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortAndUpdate();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SortAndUpdate(false);
        }

        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            SortAndUpdate();
        }

        private bool IsInSortAgentList(List<SortAgent> sortAgents, SortAgent testAgent)
        {

            if (sortAgents == null)
                return false;
            if (sortAgents.Count == 0)
                return false;
            foreach (SortAgent testItem in sortAgents) {
                if (testItem.OriginalIndex == testAgent.OriginalIndex)
                    return true;
            }
            return false;
        }
    }
}
