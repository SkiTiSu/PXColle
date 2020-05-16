using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PXColle.Master
{
    //PXBase
    public class PXBModel
    {
    
    }

    public interface IPXBaseData
    {
        string Id { get; set; }
        string Name { get; set; }
        string Author { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }

    public class PXBaseData : IPXBaseData
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public virtual string Name { get; set; }
        public string Author { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public interface IPXNode : IPXBaseData
    {
        Dictionary<string, string> Connects { get; set; }
        //Dictionary<IPXNode, string> Templates { get; set; }
        string Get();
        void Set(string text);
    }

    public class PXNode : PXBaseData, IPXNode
    {
        public IEnumerable<string> Types { get; set; }
        // id，关系
        public Dictionary<string, string> Connects { get; set; }

        public virtual string Get()
        {
            return "use json";
        }

        public virtual void Set(string text)
        {

        }
    }

    public class PXNodeType : PXNode
    {
        // 名称，类型
        public Dictionary<string, string> Templates { get; set; }
        public override string Get()
        {
            return "node!";
        }
    }

    public class PXTypeString
    {

    }

    public abstract class PXContent : PXNode
    {
    }

    public class PXUrl : PXContent
    {
        public PXUrl(string text = null)
        {
            Set(text);
        }
        public string Url { get; set; }

        public override string Get()
        {
            return Url;
        }

        public override void Set(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Url = text;
            }
        }
    }

    public class PXWebIndex<T> : PXUrl, IPXNodeIndex<T>
    {
        public IEnumerable<T> Children { get; set; }
    }

    public class PXDatetime : PXNode
    {
        public DateTimeOffset Datetime { get; set; }
    }

    public interface IPXNodeIndex<T> : IPXNode
    {
        IEnumerable<T> Children { get; set; }
    }

    public interface IPXNodeContent
    {

    }
}
