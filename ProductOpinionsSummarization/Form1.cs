using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.Utils.Win;
using DevExpress.XtraCharts;
using HtmlAgilityPack;
using VaderSharp;

namespace ProductOpinionsSummarization
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        HtmlAgilityPack.HtmlDocument htmlSnippet = new HtmlAgilityPack.HtmlDocument();
        string con1, con2, url2;
        int lastIndex = 0;
        int index = 0;
        int count = 0, cunnrent = 0;
        private List<string> reviewsList;
        private int positive = 0, negative = 0, neutral = 0;
        private int positiveBattery = 0, negativeBattery = 0, neutralBattery = 0;
        private int positiveDisplay = 0, negativeDisplay = 0, neutralDisplay = 0;
        private int positiveDesign = 0, negativeDesign = 0, neutralDesign = 0;
        private int positiveUpdates = 0, negativeUpdates = 0, neutralUpdates = 0;

        private string deviceURL = "";
        private string deviceReviewsURL = "";

        public Form1()
        {
            InitializeComponent();

            webBrowser1.ScriptErrorsSuppressed = true;

            selectDevice.Focus();
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (splashScreenManager1.IsSplashFormVisible)
            {
                splashScreenManager1.CloseWaitForm();

            }
        }

        private void browserFlyoutPanel_ButtonClick(object sender, DevExpress.Utils.FlyoutPanelButtonClickEventArgs e)
        {
            if (e.Button.Caption == "Refresh")
            {
                webBrowser1.Refresh();
            }
            else if (e.Button.Caption == "Close")
            {
                webBrowser1.Stop();
                browserFlyoutPanel.HidePopup();
            }
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            
            if ((e.Url.AbsolutePath.Contains("makers")) || e.Url.AbsolutePath.Contains("phones"))
            {
                
            }
            else
            {
                browserFlyoutPanel.HidePopup();
                splashScreenManager1.ShowWaitForm();

                deviceURL = e.Url.AbsoluteUri;
                
                webBrowser1.Stop();
                

                if (deviceURL.Count(c => Char.IsNumber(c))>=4)
                {
                    HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                    html.LoadHtml(new WebClient().DownloadString(deviceURL));
                    var root = html.DocumentNode;
                    var descriptionHeader = root.Descendants("h1")
                        .Where(n => n.GetAttributeValue("class", "")
                        .Equals("specs-phone-name-title"))
                        .FirstOrDefault();
                    deviceNameLabel.Text = descriptionHeader.InnerText;
                    //MessageBox.Show(descriptionHeader.InnerText);

                    /*string[] s = e.Url.AbsolutePath.Split('-');
                    string img = Regex.Replace(s[0], "_", "-");
                    img += ".jpg";
                    pictureEdit1.LoadAsync("https://cdn2.gsmarena.com/vv/bigpic/" + img);*/
                    //MessageBox.Show(img);
                    //HtmlAgilityPack.HtmlDocument html2 = new HtmlAgilityPack.HtmlDocument();
                    


                    deviceReviewsURL = deviceURL.Insert(deviceURL.Length - 9, "-reviews");
                    //listBox1.Items.Add(deviceReviewsURL);
                    //MessageBox.Show(deviceReviewsURL);
                    

                    html.LoadHtml(new WebClient().DownloadString(deviceURL));
                    var nn = html.DocumentNode.SelectNodes("//img[@src]");

                    int c = 0;
                    foreach (var ss in nn)
                    {

                        if (ss.Attributes["src"] == null)
                            continue;
                        c++;

                        if (c == 2)
                        {
                            //listBox1.Items.Add(ss.Attributes["src"].Value);
                            splashScreenManager1.CloseWaitForm();
                            pictureEdit1.LoadAsync(ss.Attributes["src"].Value);
                            break;
                        }


                    }

                    if (splashScreenManager1.IsSplashFormVisible)
                    {
                        splashScreenManager1.CloseWaitForm();

                    }

                }
                
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsolutePath.Contains("makers"))
            {
                StringBuilder sb = new StringBuilder();
                foreach (HtmlElement elm in webBrowser1.Document.All)
                    if (elm.GetAttribute("className") == "main main-makers l-box col float-right")
                        sb.Append(elm.InnerHtml);
                System.Windows.Forms.HtmlDocument doc = webBrowser1.Document;
                doc.Body.InnerHtml = sb.ToString();
            }
            else if (e.Url.AbsolutePath.Contains("phones"))
            {
                StringBuilder sbb = new StringBuilder();
                foreach (HtmlElement elm in webBrowser1.Document.All)
                    if (elm.GetAttribute("className") == "makers")
                        sbb.Append(elm.InnerHtml);
                System.Windows.Forms.HtmlDocument docc = webBrowser1.Document;
                docc.Body.InnerHtml = sbb.ToString();
            }

            if (splashScreenManager1.IsSplashFormVisible)
            {
                splashScreenManager1.CloseWaitForm();

            }
        }

        private void selectDevice_Click_1(object sender, EventArgs e)
        {
            //browserFlyoutPanel.Visible = true;
            browserFlyoutPanel.Options.AnchorType = PopupToolWindowAnchor.Center;
            browserFlyoutPanel.OwnerControl = this;
            browserFlyoutPanel.ShowPopup();
            

            webBrowser1.Navigate("https://www.gsmarena.com/makers.php3");

            //Declare the URL
            /*var url = "https://www.gsmarena.com/alcatel_5-9012.php";
            // HtmlWeb - A Utility class to get HTML document from http
            var web = new HtmlWeb();
            //Load() Method download the specified HTML document from an Internet resource.
            var doc = web.Load(url);

            var rootNode = doc.DocumentNode;

            var nodes = doc.DocumentNode.SelectNodes("//img");
            foreach (var src in nodes)
            {
                var links = src.Attributes["src"].Value;
                listBox1.Items.Add(links);
            }*/
            
            
            
            //Console.ReadLine();
        }

        Series series1 = new Series("", ViewType.Pie);
        Series series2 = new Series("", ViewType.Pie);
        Series series3 = new Series("", ViewType.Pie);
        Series series4 = new Series("", ViewType.Pie);
        Series series5 = new Series("", ViewType.Pie);

        private void anaylizeButton_Click(object sender, EventArgs e)
        {
            string model;
            //if (modelTextEdit.Text!="")
            if(deviceNameLabel.Text!= "Device Name")
            {
                //model = modelTextEdit.Text;
                //GetLink(model);
                splashScreenManager1.ShowWaitForm();
                ExtractReviews();
                PerformSentimentAnalysis();
                //splashScreenManager1.CloseWaitForm();
            }
            else
            {
                MessageBox.Show("Select device first!", "Device not selected", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        void GetLink(string model)
        {
            listBox1.Items.Clear();
            StringBuilder sb = new StringBuilder();
            byte[] ResultsBuffer = new byte[8192];
            string SearchResults = "http://google.com/search?q=" + "site:gsmarena.com " + model + " user opinions";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SearchResults);
            //request.UserAgent = @"Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.0.4) Gecko/20060508 Firefox/1.5.0.4";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();

                string tempString = null;
                int count = 0;
                do
                {
                    count = resStream.Read(ResultsBuffer, 0, ResultsBuffer.Length);
                    if (count != 0)
                    {
                        tempString = Encoding.ASCII.GetString(ResultsBuffer, 0, count);
                        sb.Append(tempString);
                    }
                }

                while (count > 0);
            }
            catch (WebException e)
            {
                //MessageBox.Show(e.ToString());
                using (var sr = new StreamReader(e.Response.GetResponseStream()))
                {
                    var html = sr.ReadToEnd();
                }
            }
            

            try
            {
                string sbb = sb.ToString();

                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                html.OptionOutputAsXml = true;
                html.LoadHtml(sbb);
                HtmlNode doc = html.DocumentNode;
                foreach (HtmlNode link in doc.SelectNodes("//a[@href]"))
                {
                    //HtmlAttribute att = link.Attributes["href"];
                    string hrefValue = link.GetAttributeValue("href", string.Empty);
                    if (!hrefValue.ToString().ToUpper().Contains("GOOGLE") && hrefValue.ToString().Contains("/url?q=") && hrefValue.ToString().ToUpper().Contains("HTTPS://"))
                    {
                        int index = hrefValue.IndexOf("&");
                        if (index > 0)
                        {
                            hrefValue = hrefValue.Substring(0, index);
                            listBox1.Items.Add(hrefValue.Replace("/url?q=", ""));
                        }
                    }
                    if (listBox1.Items.Count == 1)
                        break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            
        }

        void ExtractReviews()
        {
            //listBox2.Items.Clear();
            reviewsList = new List<string>();
            reviewsList.Clear();

            //string url = listBox1.Items[0].ToString();
            string url = deviceReviewsURL;
            string nURL = "";

            nURL = url.Substring(0, url.LastIndexOf('.'));

            //MessageBox.Show(nURL);

            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            for (int i = 0; i < 7; i++)
            {

                if (i > 0)
                {
                    url = nURL + "p" + i + ".php";
                }

                List<HtmlNode> HeaderNames;

                try
                {
                    HtmlAgilityPack.HtmlDocument doc = web.Load(url);

                    HeaderNames = doc.DocumentNode
                        .SelectNodes("//p[@class='uopin']").ToList();
                    count = HeaderNames.Count;
                }
                catch (Exception e)
                {
                    break;
                }

                

                //MessageBox.Show(listBox1.Items[0].ToString());

                foreach (var item in HeaderNames)
                {


                    try
                    {
                        //connection.Open();
                        //OleDbCommand command = new OleDbCommand();
                        //command.Connection = connection;
                        con1 = url.Replace("https://www.gsmarena.com/", string.Empty);
                        lastIndex = con1.IndexOf('-');
                        index = lastIndex;
                        con2 = con1.Substring(index);
                        url2 = con1.Replace(con2, string.Empty);
                        //command.CommandText = "INSERT INTO Review (Rev,Url) values('" + item.InnerText.Replace("'", "^") + "','" + url2.Replace("_", " ") + "')";

                        //listBox2.Items.Add(item.InnerText);
                        reviewsList.Add(item.InnerText);
                        
                        //command.ExecuteNonQuery();
                        //connection.Close();
                        //cunnrent++;
                        //pbTime.Value = cunnrent / count * 30 + 70;
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }


                }
            }
            totalLabel.Text = "Total: "+reviewsList.Count.ToString();

        
    }

        void PerformSentimentAnalysis()
        {
            SentimentIntensityAnalyzer analyzer = new SentimentIntensityAnalyzer();
            SentimentAnalysisResults score=null;
            if (reviewsList.Count > 0)
            {
                foreach (var review in reviewsList)
                {
                    score = analyzer.PolarityScores(review);
                    //listBox1.Items.Add(score.Compound);
                    if (score.Compound > 0)
                    {
                        positive++;
                    }
                    else if (score.Compound < 0)
                    {
                        negative++;
                    }
                    else
                    {
                        neutral++;
                    }

                    if (review.IndexOf("battery", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (score.Compound > 0)
                        {
                            positiveBattery++;
                        }
                        else if (score.Compound < 0)
                        {
                            negativeBattery++;
                        }
                        else
                        {
                            neutralBattery++;
                        }
                    }

                    if (review.IndexOf("display", StringComparison.OrdinalIgnoreCase) >= 0 || review.IndexOf("screen", StringComparison.OrdinalIgnoreCase) >= 0 || review.IndexOf("lcd", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (score.Compound > 0)
                        {
                            positiveDisplay++;
                        }
                        else if (score.Compound < 0)
                        {
                            negativeDisplay++;
                        }
                        else
                        {
                            neutralDisplay++;
                        }
                    }

                    if (review.IndexOf("design", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (score.Compound > 0)
                        {
                            positiveDesign++;
                        }
                        else if (score.Compound < 0)
                        {
                            negativeDesign++;
                        }
                        else
                        {
                            neutralDesign++;
                        }
                    }

                    if (review.IndexOf("updates", StringComparison.OrdinalIgnoreCase) >= 0 || review.IndexOf("update", StringComparison.OrdinalIgnoreCase) >= 0 || review.IndexOf("upgrade", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (score.Compound > 0)
                        {
                            positiveUpdates++;
                        }
                        else if (score.Compound < 0)
                        {
                            negativeUpdates++;
                        }
                        else
                        {
                            neutralUpdates++;
                        }
                    }
                }
            }
            series1.Points.Clear();
            
            chartControl1.Series.Clear();
            //chartControl1.SeriesTemplate.
            
            double total = positive + negative + neutral;

            double postivePercentage = (positive / total) * 100;
            double negativePercentage = (negative / total) * 100;
            double neutralPercentage = (neutral / total) * 100;

            
            series1.Points.Add(new SeriesPoint("Positive", postivePercentage));
            series1.Points.Add(new SeriesPoint("Negative", negativePercentage));
            series1.Points.Add(new SeriesPoint("Neutral", neutralPercentage));

            chartControl1.Series.Add(series1);
            // Format the the series labels.
            series1.Label.TextPattern = "{A}: {VP:p0}";

            // Adjust the position of series labels. 
            ((PieSeriesLabel)series1.Label).Position = PieSeriesLabelPosition.TwoColumns;

            // Detect overlapping of series labels.
            ((PieSeriesLabel)series1.Label).ResolveOverlappingMode = ResolveOverlappingMode.Default;

            // Access the view-type-specific options of the series.
            PieSeriesView myView = (PieSeriesView)series1.View;

            myView.Titles.Clear();
            // Show a title for the series.
            myView.Titles.Add(new SeriesTitle());
            myView.Titles[0].Text = series1.Name;


            series2.Points.Clear();
            chartControl2.Series.Clear();
            double totalDisplay = positiveDisplay + negativeDisplay + neutralDisplay;

            double postiveDisplayPercentage = (positiveDisplay / totalDisplay) * 100;
            double negativeDisplayPercentage = (negativeDisplay / totalDisplay) * 100;
            double neutralDisplayPercentage = (neutralDisplay / totalDisplay) * 100;


            series2.Points.Add(new SeriesPoint("Positive", postiveDisplayPercentage));
            series2.Points.Add(new SeriesPoint("Negative", negativeDisplayPercentage));
            series2.Points.Add(new SeriesPoint("Neutral", neutralDisplayPercentage));

            chartControl2.Series.Add(series2);
            // Format the the series labels.
            series2.Label.TextPattern = "{A}: {VP:p0}";

            // Adjust the position of series labels. 
            ((PieSeriesLabel)series2.Label).Position = PieSeriesLabelPosition.TwoColumns;

            // Detect overlapping of series labels.
            ((PieSeriesLabel)series2.Label).ResolveOverlappingMode = ResolveOverlappingMode.Default;

            // Access the view-type-specific options of the series.
            PieSeriesView myView2 = (PieSeriesView)series2.View;

            myView2.Titles.Clear();
            // Show a title for the series.
            myView2.Titles.Add(new SeriesTitle());
            myView2.Titles[0].Text = series2.Name;
            //chartControl1.Dock = DockStyle.Fill;

            chartControl3.Series.Clear();
            series3.Points.Clear();
            double totalBattery = positiveBattery + negativeBattery + neutralBattery;

            double postiveBatteryPercentage = (positiveBattery / totalBattery) * 100;
            double negativeBatteryPercentage = (negativeBattery / totalBattery) * 100;
            double neutralBatteryPercentage = (neutralBattery / totalBattery) * 100;


            series3.Points.Add(new SeriesPoint("Positive", postiveBatteryPercentage));
            series3.Points.Add(new SeriesPoint("Negative", negativeBatteryPercentage));
            series3.Points.Add(new SeriesPoint("Neutral", neutralBatteryPercentage));

            chartControl3.Series.Add(series3);
            // Format the the series labels.
            series3.Label.TextPattern = "{A}: {VP:p0}";

            // Adjust the position of series labels. 
            ((PieSeriesLabel)series3.Label).Position = PieSeriesLabelPosition.TwoColumns;

            // Detect overlapping of series labels.
            ((PieSeriesLabel)series3.Label).ResolveOverlappingMode = ResolveOverlappingMode.Default;

            // Access the view-type-specific options of the series.
            PieSeriesView myView3 = (PieSeriesView)series3.View;

            myView3.Titles.Clear();
            // Show a title for the series.
            myView3.Titles.Add(new SeriesTitle());
            myView3.Titles[0].Text = series3.Name;


            series4.Points.Clear();

            chartControl4.Series.Clear();
            double totalDesign = positiveDesign + negativeDesign + neutralDesign;

            double postiveDesignPercentage = (positiveDesign / totalDesign) * 100;
            double negativeDesignPercentage = (negativeDesign / totalDesign) * 100;
            double neutralDesignPercentage = (neutralDesign / totalDesign) * 100;


            series4.Points.Add(new SeriesPoint("Positive", postiveDesignPercentage));
            series4.Points.Add(new SeriesPoint("Negative", negativeDesignPercentage));
            series4.Points.Add(new SeriesPoint("Neutral", neutralDesignPercentage));

            chartControl4.Series.Add(series4);
            // Format the the series labels.
            series4.Label.TextPattern = "{A}: {VP:p0}";

            // Adjust the position of series labels. 
            ((PieSeriesLabel)series4.Label).Position = PieSeriesLabelPosition.TwoColumns;

            // Detect overlapping of series labels.
            ((PieSeriesLabel)series4.Label).ResolveOverlappingMode = ResolveOverlappingMode.Default;

            // Access the view-type-specific options of the series.
            PieSeriesView myView4 = (PieSeriesView)series4.View;

            myView4.Titles.Clear();
            // Show a title for the series.
            myView4.Titles.Add(new SeriesTitle());
            myView4.Titles[0].Text = series4.Name;


            series5.Points.Clear();
            chartControl5.Series.Clear();

            double totalUpdates = positiveUpdates + negativeUpdates + neutralUpdates;

            double postiveUpdatesPercentage = (positiveUpdates / totalUpdates) * 100;
            double negativeUpdatesPercentage = (negativeUpdates / totalUpdates) * 100;
            double neutralUpdatesPercentage = (neutralUpdates / totalUpdates) * 100;


            series5.Points.Add(new SeriesPoint("Positive", postiveUpdatesPercentage));
            series5.Points.Add(new SeriesPoint("Negative", negativeUpdatesPercentage));
            series5.Points.Add(new SeriesPoint("Neutral", neutralUpdatesPercentage));

            chartControl5.Series.Add(series5);
            // Format the the series labels.
            series5.Label.TextPattern = "{A}: {VP:p0}";

            // Adjust the position of series labels. 
            ((PieSeriesLabel)series5.Label).Position = PieSeriesLabelPosition.TwoColumns;

            // Detect overlapping of series labels.
            ((PieSeriesLabel)series5.Label).ResolveOverlappingMode = ResolveOverlappingMode.Default;

            // Access the view-type-specific options of the series.
            PieSeriesView myView5 = (PieSeriesView)series5.View;

            myView5.Titles.Clear();
            // Show a title for the series.
            myView5.Titles.Add(new SeriesTitle());
            myView5.Titles[0].Text = series5.Name;

            splashScreenManager1.CloseWaitForm();
        }

    }
}
