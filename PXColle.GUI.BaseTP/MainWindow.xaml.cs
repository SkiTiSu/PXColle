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
using PXColle.Master;
using PXColle.Master.Base;

namespace PXColle.GUI.BaseTP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            textBoxMain.Text = "";

            //var root = new PXNode
            //{
            //    Id = "0",
            //    Name = "root"
            //};

            //var people = new PXPerson
            //{
            //    Id = "1",
            //    Name = "mafumafu",
            //    Connects = new Dictionary<string, string>
            //    {
            //        { "0", "normal"}
            //    }
            //};

            //var url1 = new PXUrl
            //{
            //    Id = "1-1",
            //    Name = "https://space.bilibili.com/387994725",
            //    Connects = new Dictionary<string, string>
            //    {
            //        { "1", "prop.homepages" }
            //    }
            //};

            //var url2 = new PXUrl
            //{
            //    Id = "1-2",
            //    Name = "https://www.weibo.com/6856340946",
            //    Connects = new Dictionary<string, string>
            //    {
            //        { "1", "prop.homepages" }
            //    }
            //};

            //var url3 = new PXUrl
            //{
            //    Id = "1-3",
            //    Name = "https://www.instagram.com/mafumafu_ig",
            //    Connects = new Dictionary<string, string>
            //    {
            //        { "1", "prop.homepages" }
            //    }
            //};

            //var url4 = new PXUrl
            //{
            //    Id = "1-1-1",
            //    Name = "test",
            //    Connects = new Dictionary<string, string>
            //    {
            //        { "1-1", "prop.homepages" }
            //    }
            //};

            //List<PXNode> pxnodes = new List<PXNode>
            //{
            //    root,
            //    people,
            //    url1,
            //    url2,
            //    url3,
            //    url4,
            //};

            //int i = 0;

            //BFS(new List<PXNode> { root });

            //void BFS(List<PXNode> layer)
            //{
            //    i++;
            //    List<PXNode> nextlayer = new List<PXNode>();
            //    foreach (var node in layer)
            //    {
            //        var newlayer = pxnodes.Where(x => x.Connects?.ContainsKey(node.Id) ?? false); 
            //        foreach (var n in newlayer)
            //        {
            //            AddTextLine($"{i}: {n.Name}");
            //        }
            //        nextlayer.AddRange(newlayer);
            //    }
            //    if (nextlayer.Count() > 0)
            //    {
            //        BFS(nextlayer);
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}

            List<PXNode> pxnodes = new List<PXNode>
            {
                new PXNodeType()
                {
                    Key = "string"
                },
                new PXNodeType()
                {
                    Key = "number"
                },
                new PXNodeType()
                {
                    Key = "person",
                    Templates = new Dictionary<string, string>
                    {
                        { "birthday", "string" }
                    }
                },
                new PXNodeType()
                {
                    Key = "artist",
                    Connects = new Dictionary<string, string>
                    {
                        { "person", "inheritance" }
                    },
                    Templates = new Dictionary<string, string>
                    {
                        { "pixiv", "string" }
                    }
                }
            };

            IPXBNodeManager nodem = new PXBNodeManagerInMemory(pxnodes);
            PXBTypeManager typem = new PXBTypeManager(nodem);
            
            var res = typem.GetTemplateParams("artist");

            foreach (var kv in res)
            {
                AddTextLine($"{kv.Key}: {kv.Value}");
            }

            treeViewMain.ItemsSource = pxnodes;
        }

        public void AddTextLine(string text)
        {
            textBoxMain.Text += text + "\r\n";
        }

        public void InitTree()
        {

        }
    }
}
