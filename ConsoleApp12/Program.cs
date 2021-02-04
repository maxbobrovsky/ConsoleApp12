using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ConsoleApp12
{
    class Program
    {
        static async Task Main(string[] args)

            
        {
            do
            {

                //XDocument xdoc = new XDocument();
                var path = "C:\\feeds.xml";
                XDocument xdoc;

                if (File.Exists(path))
                {
                    xdoc = XDocument.Load("C:\\feeds.xml");
                }
                else
                {
                    xdoc = new XDocument();
                }

                Console.WriteLine("Select digit which match one of the menu items:");
                Console.WriteLine("But add some feed initially to create xml-file");
                Console.WriteLine("1 - Add feed");
                Console.WriteLine("2 - Remove feed");
                Console.WriteLine("3 - Download feed(s)");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Enter the feed's name you wanna add:");
                        var feed_name = Console.ReadLine();
                        Console.WriteLine("Enter the feed's URL:");
                        var url1 = Console.ReadLine();

                        XElement feed = new XElement("feed");
                        XElement FeedName = new XElement("name", feed_name);
                        XElement Url = new XElement("url", url1);

                        feed.Add(FeedName);
                        feed.Add(Url);

                        if (xdoc.Element("feeds") == null)
                        {
                            XElement feeds1 = new XElement("feeds");
                            xdoc.Add(feeds1);
                            feeds1.Add(feed);
                            xdoc.Save("C:\\feeds.xml");
                        }
                        else
                        {
                            xdoc.Element("feeds").Add(feed);
                            xdoc.Save(path);

                        }
                        break;


                    case "2":
                        Console.WriteLine("Enter the feed's name you wanna delete:");
                        var feed_name_del = Console.ReadLine();

                        var del_node = xdoc.Elements("feeds").Elements("feed").
                            Where(x => (x.Element("name").Value.ToString().CompareTo(feed_name_del) == 0)).
                            FirstOrDefault();
                        if (del_node == null)
                        {
                            Console.WriteLine("invalid input");
                        }
                        else
                        {
                            del_node.Remove();
                            xdoc.Save(path);
                        }


                        break;
                    case "3":
                        Console.WriteLine("Enter the feed's name you wanna download:");
                        var downs = Console.ReadLine();


                        var mass = downs.Split(' ');
                        var tasks = new List<Task>();

                        if (mass[0] == "")
                        {
                            try
                            {
                                var em = xdoc.Elements("feeds").Elements("feed").Elements("url");
                                List<string> lst = new List<string>();
                                foreach (var elem in em)
                                {
                                    Console.WriteLine(elem.Value);
                                    lst.Add(elem.Value);
                                }

                                foreach (var lnk in lst)
                                {
                                    tasks.Add(Task.Run(async () =>
                                    {
                                        var feeda = await FeedReader.ReadAsync(lnk);
                                        XDocument myDoca = new XDocument();
                                        XElement myfeeda = new XElement("feed");

                                        foreach (var item in feeda.Items)
                                        {
                                            XElement myItema = new XElement("item");
                                            Console.WriteLine("HALLO");
                                            XElement myTitlea = new XElement("title", item.Title.ToString());
                                            myItema.Add(myTitlea);

                                            XElement myLinka = new XElement("link", item.Link.ToString());
                                            myItema.Add(myLinka);

                                            myfeeda.Add(myItema);
                                        }

                                        myDoca.Add(myfeeda);
                                        myDoca.Save($"C:\\{feeda.Items.Count}feed.xml");
                                    }));

                                }

                                Task t = Task.WhenAll(tasks);
                                try
                                {
                                    t.Wait();
                                }
                                catch { }

                                if (t.Status == TaskStatus.RanToCompletion)
                                {
                                    Console.WriteLine("All file downloading attempts succeeded.");
                                }
                                else if (t.Status == TaskStatus.Faulted)
                                {
                                    Console.WriteLine("Something went wrong with file downloading");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }

                        else
                        {
                            foreach (var value in mass)
                            {
                                try
                                {
                                    string url_res = xdoc.Elements("feeds").
                                                          Elements("feed").
                                                          Where(y => (y.Element("name").Value.ToString() == value)).
                                                          Elements("url").
                                                          FirstOrDefault()?.Value.
                                                          ToString();

                                    tasks.Add(Task.Run(async () =>
                                    {
                                        var feed = await FeedReader.ReadAsync(url_res);
                                        XDocument myDoc = new XDocument();
                                        XElement myfeed = new XElement("feed");

                                        foreach (var item in feed.Items)
                                        {
                                            XElement myItem = new XElement("item");
                                            //Console.WriteLine("HALLO");
                                            XElement myTitle = new XElement("title", item.Title.ToString());
                                            myItem.Add(myTitle);

                                            XElement myLink = new XElement("link", item.Link.ToString());
                                            myItem.Add(myLink);

                                            myfeed.Add(myItem);
                                        }

                                        myDoc.Add(myfeed);
                                        myDoc.Save($"C:\\{value}-feeds.xml");
                                    }));

                                }
                                catch (Exception exc)
                                {
                                    Console.WriteLine(exc.Message);
                                }
                            }

                            Task downloading = Task.WhenAll(tasks);
                            try
                            {
                                downloading.Wait();
                            }
                            catch { }

                            if (downloading.Status == TaskStatus.RanToCompletion)
                            {
                                Console.WriteLine("All file downloading attempts succeeded.");
                            }
                            else if (downloading.Status == TaskStatus.Faulted)
                            {
                                Console.WriteLine("Something went wrong with file downloading");
                            }

                        }

                        break;
                    default:
                        Console.WriteLine("invalid input");
                        break;
                }

                Console.WriteLine("Do you want to continue?(yes/no)");
                
            } while(Console.ReadLine() == "yes");


        }

        
    }
}
