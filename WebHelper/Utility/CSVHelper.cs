using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace Helper
{
    public class CsvParser
    {

        public List<String> fields;
        private String[] fieldSeparator;
        Dictionary<String, PropertyInfo> props;
        CultureInfo Dci = new CultureInfo("pt-BR");
        CultureInfo Nci = new CultureInfo("en-US");

        Boolean ignoreLastCol = false;
        int ilc = 0;

        Regex rgx = null;

        public static byte[] GetCsvBytes<T>(IEnumerable<T> items)
        {
            String Result = "";

            String textDelimiter = "\"";
            String columnDelimiter = ";";
            String dateFormat = "dd/MM/yyyy";
            String dateTimeFormat = "dd/MM/yyyy HH:mm:ss";
            String decimalFormat = "0.00";
            String lineDelimiter = "\r\n";
            Encoding encodeOutput = Encoding.GetEncoding("ISO-8859-1");
            CultureInfo cult = CultureInfo.GetCultureInfo("pt-BR");
            Type itemType = typeof(T);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            Result = Result + (string.Join(columnDelimiter, props.Select(p => p.Name)));

            foreach (var item in items)
            {
                foreach (PropertyInfo p in props)
                {
                    Object value = p.GetValue(item, null);
                    if (value == null)
                    {
                        //writer.Write(columnDelimiter);

                        Result = Result + (columnDelimiter);

                        continue;
                    }
                    switch (value.GetType().Name.ToLower())
                    {
                        case "int16":
                        case "int32":
                            //writer.Write(String.Format("{0:0}", value));

                            Result = Result + (String.Format("{0:0}", value));

                            break;
                        case "long":
                        case "int64":
                            //writer.Write(String.Format("{0:0}", value));

                            Result = Result + (String.Format("{0:0}", value));

                            //ws.Cells[line, wsc].Value = dr[i].ToString();
                            //ws.Cells[line, i + 1].Style.Numberformat.Format = "@";
                            break;
                        case "datetime":
                            DateTime dt = Convert.ToDateTime(value);
                            if (dt.TimeOfDay.Ticks > 0)
                                //writer.Write(String.Format("{0:" + dateTimeFormat + "}", dt));
                                Result = Result + (String.Format("{0:" + dateTimeFormat + "}", dt));
                            else
                                //writer.Write(String.Format("{0:" + dateFormat + "}", dt));
                                Result = Result + (String.Format("{0:" + dateFormat + "}", dt));
                            break;

                        case "float":
                        case "decimal":
                        case "double":
                            //writer.Write(String.Format(cult, "{0:" + decimalFormat + "}", value));
                            Result = Result + (String.Format(cult, "{0:" + decimalFormat + "}", value));
                            break;

                        default:
                            if (!String.IsNullOrEmpty(value.ToString()))
                                //writer.Write(String.Format("{1}{0}{1}", value, textDelimiter));
                                Result = Result + (String.Format("{1}{0}{1}", value, textDelimiter));
                            break;
                    }
                    //writer.Write(columnDelimiter);
                    Result = Result + (columnDelimiter);
                }
                //writer.Write(lineDelimiter);
                Result = Result + (lineDelimiter);
            }

            return Encoding.GetEncoding(encodeOutput.CodePage).GetBytes(Result);
            return Result.Split('-').Select(s => Convert.ToByte(s, 16)).ToArray();
        }

        public static byte[] GetCsvBytes(List<Dictionary<String, String>> items, String encoding = "UTF8", bool sepOnLastCol = false)
        {

            String Result = "";

            String textDelimiter = "";
            String columnDelimiter = ";";
            String lineDelimiter = "\r\n";
            Encoding encodeOutput = encoding == "UTF8" ? Encoding.UTF8 : Encoding.GetEncoding(encoding);
            CultureInfo cult = CultureInfo.GetCultureInfo("pt-BR");
            String strLongNum = "";

            Boolean printHeader = true;

            var idx = items.Select((l, i) => new { Count = l.Count, Index = i })
                     .OrderByDescending(x => x.Count)
                     .First().Index;

            string lasKey = items[idx].Keys.Last();

            if (printHeader)
                //writer.WriteLine(string.Join(columnDelimiter, items[idx].Keys.Where(k => !k.StartsWith("_"))));
                Result = Result + (string.Join(columnDelimiter, items[idx].Keys.Where(k => !k.StartsWith("_")))) + "\r\n";
            foreach (var item in items)
            {

                foreach (String p in items[idx].Keys)
                {
                    if (p.StartsWith("_")) continue; // pular a coluna que começar com _

                    String value = null;
                    if (item.ContainsKey(p))
                        value = item[p];

                    if (String.IsNullOrEmpty(value))
                    {
                        if (p == lasKey)
                        {
                            if (sepOnLastCol)
                                //writer.Write(columnDelimiter);
                                Result = Result + (columnDelimiter);
                        }
                        else
                            //writer.Write(columnDelimiter);
                            Result = Result + (columnDelimiter);
                        continue;
                    }
                    //writer.Write(String.Format("{2}{1}{0}{1}", value, textDelimiter, strLongNum));
                    Result = Result + (String.Format("{2}{1}{0}{1}", value, textDelimiter, strLongNum));
                    if (p == lasKey)
                    {
                        if (sepOnLastCol)
                            //writer.Write(columnDelimiter);
                            Result = Result + (columnDelimiter);
                    }
                    else
                        //writer.Write(columnDelimiter);
                        Result = Result + (columnDelimiter);
                }
                //writer.Write(lineDelimiter);
                Result = Result + (lineDelimiter);
            }

            return Encoding.GetEncoding(encodeOutput.CodePage).GetBytes(Result);
            return Result.Split('-').Select(s => Convert.ToByte(s, 16)).ToArray();
        }

        public static byte[] GetCsvBytes(List<Dictionary<String, Object>> items, Boolean LongNumbersAsString = true, String encoding = "UTF8")
        {
            String Result = "";

            String textDelimiter = "\"";
            String columnDelimiter = ";";
            String dateFormat = "dd/MM/yyyy";
            String dateTimeFormat = "dd/MM/yyyy HH:mm:ss";
            String decimalFormat = "0.00";
            String lineDelimiter = "\r\n";
            Encoding encodeOutput = encoding == "UTF8" ? Encoding.UTF8 : Encoding.GetEncoding(encoding);
            CultureInfo cult = CultureInfo.GetCultureInfo("pt-BR");
            String strLongNum = "=";
            if (!LongNumbersAsString)
                strLongNum = null;

            Boolean printHeader = true;

            var idx = items.Select((l, i) => new { Count = l.Count, Index = i })
                     .OrderByDescending(x => x.Count)
                     .First().Index;

            if (printHeader)
                //writer.WriteLine(string.Join(columnDelimiter, items[idx].Keys.Where(k => !k.StartsWith("_"))));
                Result = Result + (string.Join(columnDelimiter, items[idx].Keys.Where(k => !k.StartsWith("_")))) + "\r\n";

            foreach (var item in items)
            {
                foreach (String p in items[idx].Keys)
                {
                    if (p.StartsWith("_")) continue; // pular a coluna que começar com _

                    Object value = null;
                    if (item.ContainsKey(p))
                        value = item[p];

                    if (value == null)
                    {
                        //writer.Write(columnDelimiter);
                        Result = Result + (columnDelimiter);
                        continue;
                    }
                    switch (value.GetType().Name.ToLower())
                    {
                        case "int16":
                        case "int32":
                            //writer.Write(String.Format("{0:0}", value));
                            Result = Result + (String.Format("{0:0}", value));
                            break;
                        case "long":
                        case "int64":
                            if (!LongNumbersAsString)
                                //writer.Write(String.Format("{0:0}", value));
                                Result = Result + (String.Format("{0:0}", value));
                            else
                                //writer.Write(String.Format("{2}{1}{0}{1}", value, textDelimiter, strLongNum));
                                Result = Result + (String.Format("{2}{1}{0}{1}", value, textDelimiter, strLongNum));
                            //ws.Cells[line, wsc].Value = dr[i].ToString();
                            //ws.Cells[line, i + 1].Style.Numberformat.Format = "@";
                            break;
                        case "datetime":
                            DateTime dt = Convert.ToDateTime(value);
                            if (dt.TimeOfDay.Ticks > 0)
                                //writer.Write(String.Format("{0:" + dateTimeFormat + "}", dt));
                                Result = Result + (String.Format("{0:" + dateTimeFormat + "}", dt));
                            else
                                //writer.Write(String.Format("{0:" + dateFormat + "}", dt));
                                Result = Result + (String.Format("{0:" + dateFormat + "}", dt));
                            break;

                        case "float":
                        case "decimal":
                        case "double":
                            //writer.Write(String.Format(cult, "{0:" + decimalFormat + "}", value));
                            Result = Result + (String.Format(cult, "{0:" + decimalFormat + "}", value));
                            break;

                        default:
                            if (!String.IsNullOrEmpty(value.ToString()))
                                //writer.Write(String.Format("{2}{1}{0}{1}", value, textDelimiter, strLongNum));
                                Result = Result + (String.Format("{2}{1}{0}{1}", value, textDelimiter, strLongNum));
                            break;
                    }
                    //writer.Write(columnDelimiter);
                    Result = Result + (columnDelimiter);
                }
                //writer.Write(lineDelimiter);
                Result = Result + (lineDelimiter);
            }

            return Encoding.GetEncoding(encodeOutput.CodePage).GetBytes(Result);
            return Result.Split('-').Select(s => Convert.ToByte(s, 16)).ToArray();
        }
 
        public static void WriteCsv<T>(IEnumerable<T> items, string path)
        {
            String textDelimiter = "\"";
            String columnDelimiter = ";";
            String dateFormat = "dd/MM/yyyy";
            String dateTimeFormat = "dd/MM/yyyy HH:mm:ss";
            String decimalFormat = "0.00";
            String lineDelimiter = "\r\n";
            Encoding encodeOutput = Encoding.GetEncoding("ISO-8859-1");
            CultureInfo cult = CultureInfo.GetCultureInfo("pt-BR");
            Type itemType = typeof(T);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            using (var writer = new StreamWriter(path, false, encodeOutput))
            {
                writer.WriteLine(string.Join(columnDelimiter, props.Select(p => p.Name)));

                foreach (var item in items)
                {
                    foreach (PropertyInfo p in props)
                    {
                        Object value = p.GetValue(item, null);
                        if (value == null)
                        {
                            writer.Write(columnDelimiter);
                            continue;
                        }
                        switch (value.GetType().Name.ToLower())
                        {
                            case "int16":
                            case "int32":
                                writer.Write(String.Format("{0:0}", value));
                                break;
                            case "long":
                            case "int64":
                                writer.Write(String.Format("{0:0}", value));

                                //ws.Cells[line, wsc].Value = dr[i].ToString();
                                //ws.Cells[line, i + 1].Style.Numberformat.Format = "@";
                                break;
                            case "datetime":
                                DateTime dt = Convert.ToDateTime(value);
                                if (dt.TimeOfDay.Ticks > 0)
                                    writer.Write(String.Format("{0:" + dateTimeFormat + "}", dt));
                                else
                                    writer.Write(String.Format("{0:" + dateFormat + "}", dt));
                                break;

                            case "float":
                            case "decimal":
                            case "double":
                                writer.Write(String.Format(cult, "{0:" + decimalFormat + "}", value));
                                break;

                            default:
                                if (!String.IsNullOrEmpty(value.ToString()))
                                    writer.Write(String.Format("{1}{0}{1}", value, textDelimiter));
                                break;
                        }
                        writer.Write(columnDelimiter);
                    }
                    writer.Write(lineDelimiter);
                }
                writer.Close();
            }
        }

        public static void WriteCsv(List<Dictionary<String, String>> items, string path, String encoding = "UTF8", bool sepOnLastCol = false, bool appendFile = false)
        {
            String textDelimiter = "";
            String columnDelimiter = ";";
            String lineDelimiter = "\r\n";
            Encoding encodeOutput = encoding == "UTF8" ? Encoding.UTF8 : Encoding.GetEncoding(encoding);
            CultureInfo cult = CultureInfo.GetCultureInfo("pt-BR");
            String strLongNum = "";

            Boolean printHeader = true;
            if (File.Exists(path) && appendFile)
                printHeader = false;

            using (var writer = new StreamWriter(path, appendFile, encodeOutput))
            {
                var idx = items.Select((l, i) => new { Count = l.Count, Index = i })
                         .OrderByDescending(x => x.Count)
                         .First().Index;

                string lasKey = items[idx].Keys.Last();

                if (printHeader)
                    writer.WriteLine(string.Join(columnDelimiter, items[idx].Keys.Where(k => !k.StartsWith("_"))));

                foreach (var item in items)
                {

                    foreach (String p in items[idx].Keys)
                    {
                        if (p.StartsWith("_")) continue; // pular a coluna que começar com _

                        String value = null;
                        if (item.ContainsKey(p))
                            value = item[p];

                        if (String.IsNullOrEmpty(value))
                        {
                            if (p == lasKey)
                            {
                                if (sepOnLastCol)
                                    writer.Write(columnDelimiter);
                            }
                            else
                                writer.Write(columnDelimiter);
                            continue;
                        }
                        writer.Write(String.Format("{2}{1}{0}{1}", value, textDelimiter, strLongNum));
                        if (p == lasKey)
                        {
                            if (sepOnLastCol)
                                writer.Write(columnDelimiter);
                        }
                        else
                            writer.Write(columnDelimiter);
                    }
                    writer.Write(lineDelimiter);
                }
                writer.Close();
            }
        }

        public static void WriteCsv(List<Dictionary<String, Object>> items, string path, Boolean LongNumbersAsString = true, String encoding = "UTF8", bool appendFile = false)
        {
            String textDelimiter = "\"";
            String columnDelimiter = ";";
            String dateFormat = "dd/MM/yyyy";
            String dateTimeFormat = "dd/MM/yyyy HH:mm:ss";
            String decimalFormat = "0.00";
            String lineDelimiter = "\r\n";
            Encoding encodeOutput = encoding == "UTF8" ? Encoding.UTF8 : Encoding.GetEncoding(encoding);
            CultureInfo cult = CultureInfo.GetCultureInfo("pt-BR");
            String strLongNum = "=";
            if (!LongNumbersAsString)
                strLongNum = null;

            Boolean printHeader = true;
            if (File.Exists(path) && appendFile)
                printHeader = false;

            using (var writer = new StreamWriter(path, appendFile, encodeOutput))
            {
                var idx = items.Select((l, i) => new { Count = l.Count, Index = i })
                         .OrderByDescending(x => x.Count)
                         .First().Index;

                if (printHeader)
                    writer.WriteLine(string.Join(columnDelimiter, items[idx].Keys.Where(k=>!k.StartsWith("_"))));

                foreach (var item in items)
                {
                    foreach (String p in items[idx].Keys)
                    {
                        if (p.StartsWith("_")) continue; // pular a coluna que começar com _

                        Object value = null;
                        if (item.ContainsKey(p))
                            value = item[p];

                        if (value == null)
                        {
                            writer.Write(columnDelimiter);
                            continue;
                        }
                        switch (value.GetType().Name.ToLower())
                        {
                            case "int16":
                            case "int32":
                                writer.Write(String.Format("{0:0}", value));
                                break;
                            case "long":
                            case "int64":
                                if (!LongNumbersAsString)
                                    writer.Write(String.Format("{0:0}", value));
                                else
                                    writer.Write(String.Format("{2}{1}{0}{1}", value, textDelimiter, strLongNum));
                                //ws.Cells[line, wsc].Value = dr[i].ToString();
                                //ws.Cells[line, i + 1].Style.Numberformat.Format = "@";
                                break;
                            case "datetime":
                                DateTime dt = Convert.ToDateTime(value);
                                if (dt.TimeOfDay.Ticks > 0)
                                    writer.Write(String.Format("{0:" + dateTimeFormat + "}", dt));
                                else
                                    writer.Write(String.Format("{0:" + dateFormat + "}", dt));
                                break;

                            case "float":
                            case "decimal":
                            case "double":
                                writer.Write(String.Format(cult, "{0:" + decimalFormat + "}", value));
                                break;

                            default:
                                if (!String.IsNullOrEmpty(value.ToString()))
                                    writer.Write(String.Format("{2}{1}{0}{1}", value, textDelimiter, strLongNum));
                                break;
                        }
                        writer.Write(columnDelimiter);
                    }
                    writer.Write(lineDelimiter);
                }
                writer.Close();
            }
        }

        public CsvParser(String Headers, String[] csvSeparator, Type typ, Boolean IgnoreLastCol = false)
        {
            fieldSeparator = csvSeparator;
            //String[] auxfields = Headers.Split(fieldSeparator, StringSplitOptions.None);
            rgx = new Regex(String.Format("\"([^\"]+?)\"{0}?|([^{0}]+){0}?|{0}", fieldSeparator.First()));
            Boolean useTypeFields = false;
            if (!String.IsNullOrEmpty(Headers))
            {
                String[] auxfields = Headers.Split(fieldSeparator, StringSplitOptions.None);
                fields = auxfields.Select(s => s.Trim().Replace(" ", "_").Replace("/", "_").Replace("(", "_").Replace(")", "_").Replace("__", "_").TrimEnd(new char[] { '_' })).ToList();
            }
            else
            {
                fields = new List<string>();
                useTypeFields = true;
            }

            ignoreLastCol = IgnoreLastCol;
            if (ignoreLastCol)
                ilc = 1;

            props = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo p in typ.GetProperties())
            {
                props.Add(p.Name, p);
                if (useTypeFields && !p.Name.StartsWith("_"))
                    fields.Add(p.Name);
            }
        }

        public CsvParser(String Headers, String csvSeparator,  Boolean IgnoreLastCol = false)
        {
            fieldSeparator = new String[] { csvSeparator };
            rgx = new Regex(String.Format("\"([^\"]+?)\"{0}?|([^{0}]+){0}?|{0}", fieldSeparator.First()));
            //Boolean useTypeFields = false;
            if (!String.IsNullOrEmpty(Headers))
            {
                String[] auxfields = Headers.Split(fieldSeparator, StringSplitOptions.None);
                fields = auxfields.ToList();

                if (fields.Last().Trim() == "")
                    fields.RemoveAt(fields.Count - 1);
                    
                //fields = auxfields.Select(s => s.Trim().Replace(" ", "_").Replace("/", "_").Replace("(", "_").Replace(")", "_").Replace("__", "_").TrimEnd(new char[] { '_' })).ToList();

            }
            else
                throw new ArgumentException("A linha de cabeçalho deve ser informada!");


            ignoreLastCol = IgnoreLastCol;
            if (ignoreLastCol)
                ilc = 1;

            props = null;
            //props = null; new Dictionary<string, PropertyInfo>();
            //foreach (PropertyInfo p in typ.GetProperties())
            //{
            //    props.Add(p.Name, p);
            //    if (useTypeFields && !p.Name.StartsWith("_"))
            //        fields.Add(p.Name);
            //}
        }

        public String GenerateSqlTable()
        {
            StringBuilder sb = new StringBuilder();
            Type t = null;
            foreach (KeyValuePair<String, PropertyInfo> kv in props)
            {
                if (kv.Value.PropertyType.Name.IndexOf("Nullable") != -1)
                    t = kv.Value.PropertyType.GenericTypeArguments[0];
                else
                    t = kv.Value.PropertyType;

                String nome = kv.Key;
                String tipo = null;
                String tamanho = null;
                String Nullable = "NOT NULL";
                switch (t.Name)
                {
                    case "String":
                        tipo = "varchar";
                        tamanho = "(200)";
                        Nullable = "NULL";
                        break;
                    case "Int32":
                        tipo = "int";
                        Nullable = "NULL";
                        break;
                    case "Int64":
                        tipo = "bigint";
                        Nullable = "NOT NULL";
                        break;
                    case "Decimal":
                        tipo = "decimal";
                        tamanho = "(15,2)";
                        Nullable = "NULL";
                        break;
                    case "DateTime":
                        tipo = "datetime";
                        Nullable = "NULL";
                        break;
                }

                sb.AppendLine(String.Format("[{0}] [{1}]{2} {3}, ", nome, tipo, tamanho, Nullable));
            }

            return sb.ToString();
        }

        public String GenerateBulkMap()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<String, PropertyInfo> kv in props)
            {
                sb.AppendLine(String.Format("bulkCopy.ColumnMappings.Add(\"{0}\", \"{1}\");", kv.Key, kv.Key));
            }
            return sb.ToString();
        }

        public List<Dictionary<String, String>> ReadAllData(String filename, Encoding fileEncoding)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException(String.Format("o arquivo {0} não foi encontrado!", filename));

            int linum = 0;
            List<Dictionary<String, String>> dados = new List<Dictionary<string, string>>();
            using (StreamReader reader = new StreamReader(filename, fileEncoding, true))
            {
                // validar o header
                String linha = reader.ReadLine();
                if (linha == null || linha.Trim() == "")
                    throw new ApplicationException("O cabecalho do arquivo não foi encontrado na primeira linha do arquivo!");
                linum++;
                while (!reader.EndOfStream)
                {
                    linha = reader.ReadLine();
                    linum++;

                    if (linha.Trim() == "")
                        continue;
                    dados.Add(ReadData(linha, linum));
                }
            }
            return dados;
        }

        public Dictionary<String,String> ReadData(String Line, Int32 linenum)
        {
            try
            {
                bool lastField = false; bool endSep = false;
                var mCol = rgx.Matches(Line);
                List<String> colunas = new List<string>();
                for (int i = 0; i < mCol.Count; i++)
                {
                    endSep = false;
                    lastField = i == mCol.Count - 1;
                    String Field = mCol[i].ToString();
                    if (Field.EndsWith(";"))
                    {
                        if (lastField) endSep = true;
                        Field = Field.Substring(0, Field.Length - 1);
                    }
                    if (Field.StartsWith("\""))
                        Field = Field.Substring(1, Field.Length - 2);
                    //if (Field.Length == 0)
                    //    Field = null;
                    colunas.Add(Field);
                    if (lastField && endSep)
                        colunas.Add("");
                }

                String[] values = colunas.ToArray();

                if ((values.Length) < fields.Count)
                    throw new ApplicationException(String.Format("A quantidade de colunas ({0}) da linha ({1}) é diferente da quantidade de colunas identificada no cabeçalho ({2}).", values.Length, linenum, fields.Count));

                String fname = null;
                Dictionary<String, String> dado = new Dictionary<string, string>();
                for (int i = 0; i < (values.Length - ilc); i++)
                {
                    fname = fields[i];
                    String sv = values[i].Trim();
                    dado[fname] = sv;
                }
                return dado;
            }
            catch(Exception ex)
            {
                throw new ApplicationException(String.Format("Erro na leitura da lina {0} - Erro: {1}", linenum, ex.Message), ex);
            }
        }

        public T ReadData<T>(String Line, Int32 linenum) where T : new()
        {
            var mCol = rgx.Matches(Line);
            List<String> colunas = new List<string>();
            foreach (var match in mCol)
            {
                String Field = match.ToString();
                if (Field.EndsWith(";"))
                    Field = Field.Substring(0, Field.Length - 1);
                if (Field.StartsWith("\""))
                    Field = Field.Substring(1, Field.Length - 2);
                //if (Field.Length == 0)
                //    Field = null;
                colunas.Add(Field);
            }

            String[] values = colunas.ToArray();//Line.Split(fieldSeparator, StringSplitOptions.None);

            if ((values.Length) < fields.Count)
                throw new ApplicationException(String.Format("A quantidade de colunas ({0}) da linha ({1}) é diferente da quantidade de colunas identificada no cabeçalho ({2}).", values.Length, linenum, fields.Count));

            T dado = new T();
            Type t = null;
            String fname = null;
            PropertyInfo pi = null;
            for (int i = 0; i < (values.Length - ilc); i++)
            {
                fname = fields[i];
                if (props.ContainsKey(fname))
                {
                    pi = props[fname];
                    if (pi.PropertyType.Name.IndexOf("Nullable") != -1)
                        t = pi.PropertyType.GenericTypeArguments[0];
                    else
                        t = pi.PropertyType;
                }
                else
                    throw new ApplicationException(String.Format("O campo {0} não foi encontrada na classe de dados {1}.", fname, typeof(T).ToString()));
                Object v = null;
                String sv = values[i].Trim();
                if (String.IsNullOrEmpty(sv))
                {
                    pi.SetValue(dado, null);
                    continue;
                }
                switch (t.Name)
                {
                    case "String":
                        pi.SetValue(dado, sv);
                        break;
                    case "Int32":
                        v = Convert.ToInt32(sv.Replace(".", "").Replace("-", ""));
                        pi.SetValue(dado, v);
                        break;
                    case "Int64":
                        v = Convert.ToInt64(sv.Replace(".", "").Replace("-", ""));
                        pi.SetValue(dado, v);
                        break;
                    case "Decimal":
                        v = Convert.ToDecimal(sv, Nci);
                        pi.SetValue(dado, v);
                        break;
                    case "DateTime":
                        v = Convert.ToDateTime(sv, Dci);

                        pi.SetValue(dado, v);
                        break;
                }
            }
            return dado;
        }

    }

}
