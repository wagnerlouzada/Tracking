using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class ExceptionHelper
    {

        /// <summary>
        /// Gera um relatório padronizado sobre o erro.
        /// </summary>
        /// <param name="exception">Erro capturado, e fonte para geração do relatório.</param>
        /// <param name="ExtraInfo">Texo com informações especificas que podem ser fornecidas pela aplicação que consome esta classe.</param>
        /// <returns>Relatório detalhado sobre o erro.</returns>
        /// <exception cref="ArgumentNullException">O parametro exception não deve ser nulo.</exception>
        public static String GetDetailedExceptionMessage(Exception exception, String ExtraInfo)
        {
            if (exception == null)
                throw new ArgumentNullException("execption", "O parametro não deve ser nulo.");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Informações Gerais");
            sb.AppendLine("-------------------------------------------------------------------------");
            sb.AppendLine("Data: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            sb.AppendLine("Maquina: " + Environment.MachineName);
            sb.AppendLine("SO : " + Environment.OSVersion.VersionString);
            sb.AppendLine("Dominio\\Usuario: " + Environment.UserDomainName + "\\" + Environment.UserName);
            sb.AppendLine("Applicação: " + AppDomain.CurrentDomain.FriendlyName);
            try
            {
                sb.AppendLine("Modulo: " + exception.TargetSite.Module.Assembly.FullName);
            }
            catch
            {
                sb.AppendLine("Modulo: Informação não disponível.");
            }

            //Assembly assem = Assembly.GetEntryAssembly();
            //sb.AppendLine("Aplicação iniciada por: " + assem.Location);
            //sb.AppendLine("Cultura da aplicação: " + assem.GetName().CultureInfo.DisplayName);
            //sb.AppendLine("Nome do Produto: " + ((AssemblyProductAttribute)assem.GetCustomAttribute(typeof(AssemblyProductAttribute))).Product);
            //sb.AppendLine("Versão: " + ((AssemblyFileVersionAttribute)assem.GetCustomAttribute(typeof(AssemblyFileVersionAttribute))).Version);
            if (ExtraInfo != null && ExtraInfo != "")
            {
                sb.AppendLine("");
                sb.AppendLine("Informações Extras");
                sb.AppendLine("-------------------------------------------------------------------------");
                sb.AppendLine(ExtraInfo);
            }

            sb.AppendLine("");
            sb.AppendLine("Informações sobre o Erro ");
            sb.AppendLine("-------------------------------------------------------------------------");
            try
            {
                sb.AppendLine("Tipo: " + exception.GetType());
                //if (exception.TargetSite != null)
                //    sb.AppendLine("Target: " + exception.TargetSite.Name);
                //sb.AppendLine("Fonte: " + exception.Source);
                sb.AppendLine("Mensagem: ");
                sb.AppendLine(exception.Message);
            }
            catch
            {
                sb.AppendLine("Informações não disponíveis.");
            }

            try
            {
                if (exception.InnerException != null)
                {

                    Exception exi = exception.InnerException;

                    while (exi != null)
                    {
                        sb.AppendLine("");
                        sb.AppendLine("Inner Exception: " + exi.GetType());
                        sb.AppendLine("Mensagem Interna: " + exi.Message);
                        sb.AppendLine("Stack Trace: " + exi.StackTrace);
                        exi = exi.InnerException;
                    }
                }
            }
            catch
            {
                sb.AppendLine("Informações não disponíveis.");
            }

            sb.AppendLine("");
            sb.AppendLine("Informações para Rastreamento ");
            sb.AppendLine("-------------------------------------------------------------------------");
            try
            {
                sb.AppendLine(exception.StackTrace);
            }
            catch
            {
                sb.AppendLine("Informações não disponíveis.");
            }
            return sb.ToString();
        }

    }
}
