using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{

    public static class Enumerables
    {
        public static String getValue(this Dictionary<String,String> dic, String key)// ToTDictionary<tsource>(this IEnumerable<tsource> source, Func<tsource,tkey=""> keySelector, Func<tsource> elementSelector)
        {
            if (dic.ContainsKey(key))
                return dic[key];
            return "";
        }
    }

    public class TKeyValue
    {
        public String Key { get; set; }
        public String Value { get; set; }
        public TKeyValue(String key, String value)
        {
            Key = key;
            Value = value;
        }

    }

    public class TDictionary : IList<TKeyValue>
    {
        private List<TKeyValue> collection;

        public int Count => collection.Count;

        public bool IsReadOnly => false;


        public TDictionary()
        {
            collection = new List<TKeyValue>();
        }

        public TDictionary(HtmlNode[] itens)
        {
            collection = new List<TKeyValue>();
            AddFromHtmlNodes(itens);
        }

        public TDictionary(HtmlNode[] itens, Boolean allowDuplicates )
        {
            collection = new List<TKeyValue>();
            AddFromHtmlNodes(itens, true, allowDuplicates);
        }

        public TDictionary(String url, Boolean urlEncoded = false)
        {
            collection = new List<TKeyValue>();
            AddFromUrl(url, urlEncoded);
        }

        public TDictionary(TKeyValue[] items)
        {
            collection = new List<TKeyValue>();
            collection.AddRange(items);
        }

        public bool setValueParcialKey(String parcialKey, String value)
        {
            try
            {
                TKeyValue kv = collection.Where(k => k.Key.ToLower().Contains(parcialKey.ToLower())).FirstOrDefault();
                if (kv != null)
                {
                    kv.Value = value;
                    return true;
                }
            } catch
            {
            }
            return false;
        }

        public String this[string key]
        {
            get
            {
                TKeyValue kv = collection.Where(k => k.Key == key).FirstOrDefault();

                if (kv!=null)
                    return kv.Value;

                return "";
            }
            set
            {
                TKeyValue kv = collection.Where(k => k.Key == key).FirstOrDefault();
                if (kv==null)
                    Add(key, value);
                else
                    kv.Value = value;
            }
        }

        public bool ContainsKey(String key)
        {
            TKeyValue  kv = collection.Where(k => k.Key == key).FirstOrDefault();
            return kv != null;
        }

        public KeyValuePair<String,String>[] ToKeyValuePair()
        {
            if (collection.Count > 0)
                return collection.Select(x => new KeyValuePair<String, String>(x.Key, x.Value)).ToArray();

            return null;
        }

        public String ToUrlGet(bool urlEncode = false)
        {
            string url = "";
            if (collection.Count > 0)
            {
                if (!urlEncode)
                    url = string.Join("&", collection.Select(x => String.Format("{0}={1}", x.Key, x.Value)));
                else
                {
                    url = string.Join("&", collection.Select(x => String.Format("{0}={1}", System.Net.WebUtility.UrlEncode(x.Key), System.Net.WebUtility.UrlEncode(x.Value))));
                }
            }
            return url;
        }

        public StringContent toFormUrlEncoded(Encoding contentEncoding)
        {
            String formUrlEnc = ToUrlGet(true).Replace("%20", "+");
            StringContent sc = new StringContent(formUrlEnc, contentEncoding, "application/x-www-form-urlencoded");
            return sc;
        }

        public bool AddFromUrl(String url, Boolean appendItens = true, Boolean urlEncoded = false)
        {
            if (url == null && url.Trim() == "")
                return false;
            if (urlEncoded)
                url = System.Net.WebUtility.UrlDecode(url);

            int i = url.IndexOf('?');
            if (i != -1)
                url = url.Substring(i + 1);

            if (!appendItens)
                Clear();

            String[] pars = url.Split('&');
            foreach(string s in pars)
            {
                string[] kv = s.Split('=');
                Add(kv[0], kv[1], true);
            }
            return true;
        }

        public bool AddFromHtmlNodes(HtmlNode[] itens, Boolean appendItens = true, Boolean allowDuplicates = true)
        {
            if (itens == null || itens.Length == 0)
                return false;

            if (!appendItens)
                Clear();

            if (allowDuplicates)
                foreach (HtmlNode item in itens)
                {
                    String tipo = item.GetAttributeValue("type", "").ToLower().Trim();
                    if (tipo == "")
                        tipo = item.Name;


                    switch(tipo)
                    {
                        case "select":
                            String nome = item.GetAttributeValue("name", "");
                            String valor = "";
                            var opt = item.Descendants("option").Where(x => x.GetAttributeValue("selected", "NA") != "NA").LastOrDefault(); // ultima ocorrencia do options com selected (no chrome funciona assim!)
                            if (opt == null) // pegar o primeiro
                                opt = item.Descendants("option").FirstOrDefault();
                            if (opt != null)
                                valor = opt.GetAttributeValue("value", "");
                            this[nome] = valor;
                            break;
                        case "image":
                            this[item.GetAttributeValue("name", "") + ".x"] = item.GetAttributeValue("value", "");
                            this[item.GetAttributeValue("name", "") + ".y"] = item.GetAttributeValue("value", "");
                            break;
                        case "radio":
                            if (item.GetAttributeValue("checked", "nao achou") != "nao achou")
                            {
                                this[item.GetAttributeValue("name", "")] = item.GetAttributeValue("value", "");
                            }
                            break;
                        default:
                            string item_nome = item.GetAttributeValue("name", item.GetAttributeValue("id", ""));
                            if (item_nome != "") // nao inserir itens que nao tem nome nem id
                                Add(item_nome, item.GetAttributeValue("value", ""));
                            break;
                    }
                }
            else
                foreach (HtmlNode item in itens)
                {
                    String tipo = item.GetAttributeValue("type", "").ToLower().Trim();
                    if (tipo == "")
                        tipo = item.Name;

                    switch (tipo)
                    {
                        case "select":
                            String nome = item.GetAttributeValue("name", "");
                            String valor = "";
                            var opt = item.Descendants("option").Where(x => x.GetAttributeValue("selected", "NA") != "NA").FirstOrDefault();
                            if (opt == null) // pegar o primeiro
                                opt = item.Descendants("option").FirstOrDefault();
                            if (opt != null)
                                valor = opt.GetAttributeValue("value", "");
                            this[nome] = valor;
                            break;
                        case "image":
                            this[item.GetAttributeValue("name", "") + ".x"] = item.GetAttributeValue("value", "");
                            this[item.GetAttributeValue("name", "") + ".y"] = item.GetAttributeValue("value", "");
                            break;
                        case "radio":
                            if (item.GetAttributeValue("checked", "nao achou") != "nao achou")
                            {
                                this[item.GetAttributeValue("name", "")] = item.GetAttributeValue("value", "");
                            }
                            break;
                        default:
                            this[item.GetAttributeValue("name", "")] = item.GetAttributeValue("value", "");
                            break;
                    }              
                }


            return true;
        }

        public TKeyValue this[int index] {
            get
            {
                if (index < collection.Count)
                    return collection[index];
                throw new IndexOutOfRangeException("Indice fora de faixa");
            }
            set
            {
                if (index < collection.Count)
                    collection[index] = value;
                else
                    throw new IndexOutOfRangeException("Indice fora de faixa");
            }
        }

        public void Add(String key, String value, bool insertDuplicateKeys = true)
        {
            if (insertDuplicateKeys)
                collection.Add(new TKeyValue(key, value));
            else
            {
                this[key] = value; // assim nao será possivel inserir chaves duplicadas
            }
        }

        public void AddRange(TDictionary tdic, bool insertDuplicateKeys = true)
        {
            foreach (TKeyValue kv in tdic)
            {
                if (insertDuplicateKeys)
                    collection.Add(new TKeyValue(kv.Key, kv.Value));
                else
                {
                    this[kv.Key] = kv.Value; // assim nao será possivel inserir chaves duplicadas
                }
            }
        }

        public void Add(TKeyValue item)
        {
            collection.Add(item);
        }

        public void Clear()
        {
            collection.Clear();
        }

        public bool Contains(TKeyValue item)
        {
            return collection.Contains(item);
        }

        public void CopyTo(TKeyValue[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TKeyValue> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        public int IndexOf(TKeyValue item)
        {
            return IndexOf(item);
        }

        public void Insert(int index, TKeyValue item)
        {
            if (this.ContainsKey(item.Key))
                this[item.Key] = item.Value;
            else
                collection.Insert(index, item);
        }

        public void Insert(int index, String key, String value)
        {
            if (this.ContainsKey(key))
                this[key] = value;
            else
                collection.Insert(index, new TKeyValue(key, value));
        }

        public bool Remove(TKeyValue item)
        {
            return collection.Remove(item);
        }

        public bool Remove(String key)
        {
            TKeyValue kv = collection.Where(k => k.Key == key).FirstOrDefault();
            if (kv!=null)
                return collection.Remove(kv);
            return false;
        }

        public void ReplaceOrAdd(String oldKey, String newKey, String newValue)
        {
            TKeyValue kv = collection.Where(k => k.Key == oldKey).FirstOrDefault();
            if (kv != null)
            {
                kv.Key = newKey;
                if (newValue != null)
                    kv.Value = newValue;
            }
            else
            {
                collection.Add(new TKeyValue(newKey, newValue));
            }
        }

        public void RemoveAt(int index)
        {
            collection.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }
    }

}
