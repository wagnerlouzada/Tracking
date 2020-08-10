using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Helper
{
    public static class DataHelper
    {
        private static CultureInfo standardCulture = CultureInfo.GetCultureInfo("pt-BR");

        public static IEnumerable<T> takeUntil<T>(this IEnumerable<T> data, Func<T, bool> predicate)
        {
            foreach (var item in data)
            {
                yield return item;
                if (!predicate(item))
                    break;
            }
            yield break;
        }

        /// <summary>
        ///  retorna o conteudo de um string após encontrar um padrão
        /// </summary>
        /// <param name="source"></param>
        /// <param name="From"></param>
        /// <param name="excludeKeys"></param>
        /// <returns></returns>
        // 
        //
        public static String getFrom(String source, String From, bool excludeKeys = false)
        {
            int idxI = source.IndexOf(From);
            if (idxI == -1)
                return null;
            if (excludeKeys)
                idxI = idxI + From.Length;
            String s2 = source.Substring(idxI);

            int idxF = s2.Length;
            if (idxF == -1)
                return null;

            String final = s2.Substring(0, idxF);

            return final;
        }

        public static String getFromUntil(String source, String From , String Until, bool excludeKeys = false)
        {
            int idxI = source.IndexOf(From);
            if (idxI == -1)
                return null;
            if (excludeKeys)
                idxI = idxI + From.Length;
            String s2 = source.Substring(idxI);

            int idxF = s2.IndexOf(Until);
            if (idxF == -1)
                return null;
            if (!excludeKeys)
                idxF = idxF + Until.Length;

            String final = s2.Substring(0, idxF);

            return final;
        }

        public static String removeChas(String value, params string[] strToRemove)
        {
            string ret = value.Trim();
            foreach (string s in strToRemove)
                ret = ret.Replace(s, String.Empty);
            return ret;
        }

        public static int getPos(String[] data, String TextToFind, bool atBegining = false, bool CaseSensitive = false, Int32 FromPos = 0, bool textoCompleto = false)
        {
            if (data.Length <= 0)
                return -1;

            if (FromPos > data.Length)
                return -1;

            if (!CaseSensitive)
            {
                TextToFind = TextToFind.ToLowerInvariant();
            }

            if (textoCompleto)
            {
                for (int i = FromPos; i < data.Length; i++)
                    if (@case(data[i].Trim(), CaseSensitive) == TextToFind.Trim()) // acha no inicio do conteudo
                        return i;
            }
            else
            {
                if (atBegining)
                {
                    for (int i = FromPos; i < data.Length; i++)
                        if (@case(data[i], CaseSensitive).IndexOf(TextToFind) == 0) // acha no inicio do conteudo
                            return i;
                }
                else
                {
                    for (int i = FromPos; i < data.Length; i++)
                        if (@case(data[i], CaseSensitive).IndexOf(TextToFind) != -1) // acha em qualquer parte
                            return i;
                }
            }
            return -1;
        }

        public static int getDatePos(String[] data, Int32 FromPos = 0, string dateSep = null)
        {
            if (data.Length <= 0)
                return -1;

            if (FromPos > data.Length)
                return -1;

            for (int i = FromPos; i < data.Length; i++)
                if (isDate(data[i].Trim()))
                    if (dateSep == null || data[i].IndexOf(dateSep) != -1)
                        return i;

            return -1;
        }

        public static string[] splitFixedLen(string line, int[] positions)
        {
            string[] result = null;

            List<string> splitted = new List<string>();
            int position = 0;
            int lastPosition = 0;

            bool breakFor = false;
            for (int i = 0; i <= positions.Length; i++)
            {
                if (i < positions.Length)
                {
                    position = positions[i];
                    if (position > line.Length)
                    {
                        position = line.Length;
                        breakFor = true;
                    }

                    if (lastPosition > position)
                        lastPosition = position;

                    splitted.Add(line.Substring(lastPosition, position - lastPosition).Trim());
                    if (breakFor)
                        break;
                }
                else
                {
                    position = line.Length;
                    if (lastPosition > position)
                        lastPosition = position;

                    splitted.Add(line.Substring(lastPosition, position - lastPosition).Trim());
                }
                lastPosition = position + 1;
            }

            if (splitted.Count() > 0)
                result = splitted.ToArray();

            return result;
        }

        public static string @case(String value, bool sensitive)
        {
            if (sensitive)
                return value;
            return value.ToLowerInvariant();
        }

        public static bool isInteger(String value)
        {
            if (String.IsNullOrEmpty(value))
                return false;
            Int64 i = 0;
            return Int64.TryParse(value.Trim(), out i);
        }

        public static Boolean stringMatch(String valor, String Expression, Boolean StringCompleta = true)
        {
            String Exp = null;
            if (StringCompleta)
                Exp = "^" + Expression + "$";
            else Exp = Expression;

            Match match = Regex.Match(valor, Exp);

            return match.Success;
        }

        private static string normalizeValue(String val, bool permiteNegativo)
        {
            val = val.Trim();

            if (String.IsNullOrEmpty(val))
                return val;

            char[] numeros = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] seps = new char[] { ',', '.' };
            char[] sinal = new char[] { '-' };

            bool decSep = false;
            String result = "";
            for (int i = val.Length - 1; i >= 0; i--)
            {
                if (i == 0 && val[i].In(sinal) & permiteNegativo)
                    result = val[i] + result;
                else if (val[i].In(numeros))
                    result = val[i] + result;
                else if (!decSep && val[i].In(seps))
                {
                    result = val[i] + result;
                    decSep = true;
                }
                else if (!val[i].In(numeros) && !val[i].In(seps) && !val[i].In(sinal))
                    result = val[i] + result;
            }

            return result;
        }

        public static Decimal toDecimal(String value, int nCasasDecimais = 0, bool permiteNegativo = false)
        {
            value = String.IsNullOrEmpty(value) ? "0.00" : value.Trim();
            value = value.ToUpper().Replace("R$", "").Trim();
            value = normalizeValue(value, permiteNegativo);
            CultureInfo cultInfo = standardCulture;


            int last = value.LastIndexOfAny(new[] { ',', '.' });
            if (last >= 0)
            {
                if (value[last] == '.')
                    cultInfo = CultureInfo.InvariantCulture;
            }

            Decimal valor = 0.00m;
            Decimal.TryParse(value, NumberStyles.Currency, cultInfo, out valor);

            if (nCasasDecimais > 0)
                valor = Math.Round(valor, nCasasDecimais);

            return valor;
        }

        public static bool isValue(string value, bool permiteNegativo = false)
        {
            value = value.ToUpper().Replace("R$", "").Trim();
            value = normalizeValue(value, permiteNegativo);
            CultureInfo cultInfo = standardCulture;
            int last = value.LastIndexOfAny(new[] { ',', '.' });
            if (last >= 0)
            {
                if (value[last] == '.')
                    cultInfo = CultureInfo.InvariantCulture;
            }

            Decimal valor = 0.0M;
            if (Decimal.TryParse(value, NumberStyles.Any, cultInfo, out valor))
                return true;
            return false;
        }

        public static Boolean isDecimal(String value, bool permiteNegativo = false)
        {
            value = value.ToUpper().Replace("R$", "").Trim();
            value = normalizeValue(value, permiteNegativo);
            CultureInfo cultInfo = standardCulture;

            int last = value.LastIndexOfAny(new[] { ',', '.' });
            if (last >= 0)
            {
                if (value[last] == '.')
                    cultInfo = CultureInfo.InvariantCulture;
            }
            else
                return false;

            Decimal valor = 0.0M;
            if (Decimal.TryParse(value, NumberStyles.Any, cultInfo, out valor))
                return true;
            return false;
        }

        public static String[] splitString(String[] separator, String value)
        {
            return value.Split(separator, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
        }

        public static Boolean isDate(String value)
        {
            if (String.IsNullOrEmpty(value))
                return false;

            value = value.Trim();

            DateTime dt;
            if (DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return true;

            if (DateTime.TryParseExact(value, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "d/M/yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return true;

            if (DateTime.TryParseExact(value, "d/M/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "d/M/yy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "d/M/yy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return true;

            if (DateTime.TryParseExact(value.Trim(), "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "yyyy-M-d", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "yyyy-MM-d", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return true;

            if (DateTime.TryParseExact(value, "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "M/d/yyyy hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "M/d/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "M/d/yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "MMddyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "MMddyyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "MMddyyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "MMddyyyy hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "MMddyyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value, "MMddyyyy h:m:s", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return true;
            return false;
        }

        public static DateTime? toDateExact(String value, Boolean discardHour = true, String[] Format = null)
        {
            if (Format == null)
                return toDate(value, discardHour);

            value = value.Trim();

            DateTime dt;
            foreach (String f in Format)
                if (DateTime.TryParseExact(value.Trim(), f, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    if (discardHour)
                        dt = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0, DateTimeKind.Local);
                    return dt;
                }

            return null;
        }

        public static DateTime? toDate(String value, Boolean discardHour = true)
        {
            if (String.IsNullOrEmpty(value))
                return null;

            value = value.Trim();

            DateTime dt;
            if (DateTime.TryParseExact(value.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "dd/MM/yyyy h:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                if (discardHour)
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0, DateTimeKind.Local);
                return dt;
            }
            if (DateTime.TryParseExact(value.Trim(), "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "d/M/yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "d/M/yyyy h:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "d/M/yyyy hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                if (discardHour)
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0, DateTimeKind.Local);
                return dt;
            }

            if (DateTime.TryParseExact(value.Trim(), "yyyy-M-d", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "yyyy-MM-d", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "yyyy-M-d h:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "yyyy-MM-d HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "yyyy-MM-dd hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                if (discardHour)
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0, DateTimeKind.Local);
                return dt;
            }

            if (DateTime.TryParseExact(value.Trim(), "d/M/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "d/M/yy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "d/M/yy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "d/M/yy h:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "d/M/yy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "d/M/yy hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                if (discardHour)
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0, DateTimeKind.Local);
                return dt;
            }

            if (DateTime.TryParseExact(value.Trim(), "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "ddMMyyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "ddMMyyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "ddMMyyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "ddMMyyyy hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "ddMMyyyy h:m:s", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                if (discardHour)
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0, DateTimeKind.Local);
                return dt;
            }

            if (DateTime.TryParseExact(value.Trim(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "M/d/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "M/d/yyyy hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "M/d/yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "MMddyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "MMddyyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "MMddyyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "MMddyyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "MMddyyyy hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                DateTime.TryParseExact(value.Trim(), "MMddyyyy h:m:s", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                if (discardHour)
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0, DateTimeKind.Local);
                return dt;
            }

            return null;
        }

        public static ApplicationException setError(String Mensagem, Boolean ErroTratado, Exception InnerException = null)
        {
            ApplicationException appe = null;
            if (InnerException != null)
                appe = new ApplicationException(Mensagem, InnerException);
            else
                appe = new ApplicationException(Mensagem);
            appe.Data.Add("ErroTratado", ErroTratado);
            return appe;
        }

        public static bool In<T>(this T obj, params T[] args)
        {
            return args.Contains(obj);
        }

        public static bool InStr(this String strContent, params String[] args)
        {
            foreach (String s in args)
                if (strContent.IndexOf(s) != -1) return true;

            return false;
        }

        public static void AddRange<T, S>(this IDictionary<T, S> target, IDictionary<T, S> source)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (source == null)
                throw new ArgumentNullException("source");
            foreach (var element in source)
            {
                if (target.ContainsKey(element.Key))
                    target[element.Key] = element.Value;
                else
                    target.Add(element);
            }
        }

        public static TDictionary extractAttribValue(String Source)
        {
            String[] tags = new string[] { "input" };
            //<input id="controle" name="controle" value="315579" type="hidden" value="">
            Source = Source.Replace("<", "").Replace(">", "");
            foreach (string t in tags)
                Source = Source.Replace(t, "").Trim();

            TDictionary attrs = new TDictionary();
            while (Source.Trim() != "")
            {
                String attrname = "";
                String attrvalue = "";

                attrname = String.Concat(Source.ToArray().TakeWhile(c => c != ' ' && c != '='));

                Source = Source.Substring(attrname.Length).Trim();

                if (Source.StartsWith("=")) // obtem o valor
                {
                    attrvalue = String.Concat(Source.ToArray().TakeWhile(c => c != ' '));
                    Source = Source.Substring(attrvalue.Length).Trim();
                    attrvalue = attrvalue.Replace("=", "");
                    if (attrvalue.StartsWith("\""))
                        attrvalue = attrvalue.Substring(1);

                    if (attrvalue.StartsWith("\""))
                        attrvalue = attrvalue.Substring(1);
                    if (attrvalue.EndsWith("\""))
                        attrvalue = attrvalue.Substring(0, attrvalue.Length - 1);
                }

                if (!String.IsNullOrEmpty(attrname))
                {
                    int idx = 0;
                    while (attrs.ContainsKey(attrname))
                    {
                        attrname += "_" + (++idx).ToString();
                    }

                    attrs.Add(attrname, attrvalue);
                }
            }

            return attrs;
        }

    }
}
