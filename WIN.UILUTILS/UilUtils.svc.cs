using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using PRINT_CARD_CORE_VB;
using WIN.BASEREUSE;
using WIN.FENGEST_NAZIONALE.DOMAIN.Compression;
using WIN.FENGEST_NAZIONALE.DOMAIN.ExcelExport;
using WIN.TECHNICAL.PERSISTENCE;

namespace WIN.UILUTILS
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di classe "UilUtils" nel codice, nel file svc e nel file di configurazione contemporaneamente.
    // NOTA: per avviare il client di prova WCF per testare il servizio, selezionare UilUtils.svc o UilUtils.svc.cs in Esplora soluzioni e avviare il debug.
    [ServiceBehavior(Namespace = "http://uilwebapp.it/services")]
    public class UilUtils : IUilUtils
    {
         IPersistenceFacade f;
            WIN.BASEREUSE.GeoLocationFacade g;

        public string CalcolaCodiceFiscale(string nome, string cognome, DateTime dataNascita, string sesso, string nomeComuneNascita, string nomeNazione)
        {

            
                f = DataAccessServices.SimplePersistenceFacadeInstance();
                GeoLocationFacade.InitializeInstance(new GeoHandler(f));
                g = GeoLocationFacade.Instance();

                int sex = 1;

                if (sesso.Equals("MASCHIO"))
                {

                    sex = 1;
                }
                else if (sesso.Equals("FEMMINA"))
                {
                    sex = 2;
                }
                else
                {
                    sex = 1;
                }

                if (!nomeNazione.ToUpper().Equals("ITALIA") && !String.IsNullOrEmpty(nomeNazione))
                {
                    Nazione nazione = g.GetGeoHandler().GetNazioneByName(nomeNazione);
                    return CodiceFiscaleCalculator.CalcolaCodiceFiscale(nome, cognome, dataNascita, sex, nazione.CodiceFiscale, "");
                }
                else
                {
                    Comune comune = g.GetGeoHandler().GetComuneByName(nomeComuneNascita);
                    return CodiceFiscaleCalculator.CalcolaCodiceFiscale(nome, cognome, dataNascita, sex, "", comune.CodiceFiscale);
                }
            
            
            
        }

        public byte[] ExportDocumentToExcel(FENGEST_NAZIONALE.DOMAIN.ExcelExport.ExcelDocument document)
        {
            string file = ExcelDocumentExporter.CreateExcelFile("test.xlsx", document);

            //verifico se esiste il path se non esiste  ritorno
            if (!File.Exists(file))
                return new Byte[] { };



            FileStream objfilestream = new FileStream(file, FileMode.Open, FileAccess.Read);
            int len = (int)objfilestream.Length;
            Byte[] documentcontents = new Byte[len];
            objfilestream.Read(documentcontents, 0, len);
            objfilestream.Close();

            return documentcontents;
        }

        public FiscalData CalcolaDatiFiscali(string codiceFiscale)
        {
            f = DataAccessServices.SimplePersistenceFacadeInstance();
            GeoLocationFacade.InitializeInstance(new GeoHandler(f));
            g = GeoLocationFacade.Instance();

            DatiFiscali d = CodiceFiscaleCalculator.GetDatiFiscali(ref codiceFiscale, new GeoHandler(f)) as DatiFiscali;

            if (d == null)
                return new FiscalData();


            FiscalData fis = new FiscalData();
            fis.Comune = d.Comune.Descrizione;
            fis.DataNascita = d.DataNascita;
            fis.Nazione = d.Nazione.Descrizione;
            fis.Provincia = d.Provincia.Descrizione;
            if (d.SessoPersona.Equals("0"))
                fis.Sesso = "MASCHIO";
            else
                fis.Sesso = "FEMMINA";

            return fis;
        }


        public byte[] ExportTessere(string settore, string provincia, PRINT_CARD_CORE_VB.Tesserato[] tesserati)
        {
            string result = Path.GetTempPath();
            string filename = result + "StampaTessere_" + provincia.Replace(" ", "").Replace(".", "") + ".pdf";

            
            IList a = new ArrayList();
            foreach (Tesserato item in tesserati)
            {
                a.Add(item);
            }

            PRINT_CARD_CORE_VB.PrintCardFacade.StampaTessereUil(filename, provincia, settore, a);

            //recuperato il file pdf posso creare il file excel per le etichette
            string etichietteFilename = "EtichetteTessere" + provincia.Replace(" ", "").Replace(".", "") + ".xlsx";
            string zipFilename = "ZipTessere_" + provincia.Replace(" ", "").Replace(".", "") + ".zip";
            string etichette = CreateFileForEtichette(a, etichietteFilename);


            //adesso posso zippare i file etichette e filename
            string[] data = new string[] { filename, etichette };
            string pathTofile = Archiver.ArchiveFiles(data, zipFilename);



            FileStream objfilestream = new FileStream(pathTofile, FileMode.Open, FileAccess.Read);
            int len = (int)objfilestream.Length;
            Byte[] documentcontents = new Byte[len];
            objfilestream.Read(documentcontents, 0, len);
            objfilestream.Close();

            return documentcontents;

        }



        private string CreateFileForEtichette(IList a, string etichietteFilename)
        {
            //per prima cosa creo un documento excel da stampare
            ExcelDocument doc = new ExcelDocument();
            doc.Rows = new List<ExcelRow>();



            foreach (Tesserato item in a)
            {
                //creo una riga da inserire nel doc
                ExcelRow row = new ExcelRow();
                row.Properties = new List<ExcelProperty>();

                //aggiungo le proprietà che mi necessitano
                row.Properties.Add(new ExcelProperty("Cognome", item.Cognome, 1));
                row.Properties.Add(new ExcelProperty("Nome", item.Nome, 2));
                row.Properties.Add(new ExcelProperty("Provincia", item.Provincia, 3));
                row.Properties.Add(new ExcelProperty("Comune", item.Comune, 4));
                row.Properties.Add(new ExcelProperty("Indirizzo", item.Via, 5));
                row.Properties.Add(new ExcelProperty("Cap", item.Cap, 6));
                doc.Rows.Add(row);
            }

            //creato il documento posso stamparlo su excel
            return ExcelDocumentExporter.CreateExcelFile(etichietteFilename, doc);

        }

        public string SendMails(MailData maildata)
        {
            try
            {
                foreach (string item in maildata.Tos)
	            {
                    Mailer s = new Mailer();
                    s.SendMail(maildata.SmtpMailFrom, maildata.Sender, item, "", maildata.Subject, maildata.Body, true, new List<String>());
                    
	            }


                return "";
            }
            catch ( Exception ex)
            {

                return ex.Message;
            }
        }
    }
}
