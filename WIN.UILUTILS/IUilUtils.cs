using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using PRINT_CARD_CORE_VB;
using WIN.FENGEST_NAZIONALE.DOMAIN.ExcelExport;

namespace WIN.UILUTILS
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di interfaccia "IUilUtils" nel codice e nel file di configurazione contemporaneamente.
    [ServiceContract(Namespace = "http://uilwebapp.it/services")]
    public interface IUilUtils
    {
        [OperationContract]
        string CalcolaCodiceFiscale(string nome, string cognome, DateTime dataNascita, string sesso, string nomeComuneNascita, string nomeNazione);

        [OperationContract]
        Byte[] ExportDocumentToExcel(ExcelDocument document);

        [OperationContract]
        FiscalData CalcolaDatiFiscali(string codiceFiscale);

        [OperationContract]
        Byte[] ExportTessere(String settore, String provincia, Tesserato[] tesserati);

        [OperationContract]
        string SendMails(MailData maildata);


    }
      [DataContract]
    public class MailData
    {
           [DataMember]
        public string[] Tos { get; set; }
           [DataMember]
        public string Body { get; set; }
           [DataMember]
        public string Sender { get; set; }
           [DataMember]
        public string Subject { get; set; }
           [DataMember]
           public string SmtpServer { get; set; }
         [DataMember]
           public string SmtpAccount { get; set; }
           [DataMember]
         public string SmtpPassword { get; set; }
           [DataMember]
           public string SmtpMailFrom { get; set; }
           [DataMember]
           public bool EnableSSL { get; set; }
           [DataMember]
           public int Port { get; set; }

    }


    [DataContract]
    public class FiscalData
    {
        [DataMember]
        public string Provincia { get; set; }
         [DataMember]
        public string Comune { get; set; }
         [DataMember]
        public string Nazione { get; set; }
         [DataMember]
        public DateTime DataNascita { get; set; }
         [DataMember]
        public string Sesso { get; set; }

    }
}
