using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WIN.BASEREUSE;
using WIN.TECHNICAL.PERSISTENCE;

namespace WIN.UILUTILS
{
    public class GeoHandler : IGeoLocationLoader
    {
        private IPersistenceFacade _ps;

        public GeoHandler(IPersistenceFacade facade)
        {
            if (facade == null)
                throw new ArgumentException("Servizio di persistenza non impostato");


            _ps = facade;
        }



        #region IGeoLocationLoader Membri di

        public System.Collections.IList GetComuneByFiscalCode(string code)
        {
            Query query = _ps.CreateNewQuery("MapperComune");
            query.SetTable("TB_COMUNI");
            AbstractBoolCriteria crit = Criteria.Equal("CODICE_FISCALE", "'" + code.Replace("'", "''") + "'");
            query.AddWhereClause(crit);
            IList l = query.Execute(_ps);
            return l;
        }

        public Comune GetComuneById(int id)
        {
            return _ps.GetObject("Comune", id) as Comune;
        }

        public Comune GetComuneByName(string name)
        {
            Query query = _ps.CreateNewQuery("MapperComune");
            query.SetTable("TB_COMUNI");
            string s = name.Replace("'", "''");
            AbstractBoolCriteria crit = Criteria.Equal("Descrizione", "'" + s + "'");
            query.AddWhereClause(crit);
            IList l = query.Execute(_ps);
            //'questa impostazione è necessaria per quei comuni 
            //'che sono passati da una provincia all'altra
            if (l.Count > 0)
            {
                return l[l.Count - 1] as Comune;
            }
            return null;
        }

        public Nazione GetNazionByFiscalCode(string code)
        {

            Query query = _ps.CreateNewQuery("MapperNazione");
            query.SetTable("TB_NAZIONI");


            AbstractBoolCriteria crit = Criteria.Equal("CODICE_CF", "'" + code.Replace("'", "''") + "'");
            query.AddWhereClause(crit);
            IList l = query.Execute(_ps);
            if (l.Count > 0)
            {
                return l[0] as Nazione;
            }
            return new NazioneNulla();
        }

        public System.Collections.IList GetNazioni()
        {
            return _ps.GetAllObjects("Nazione");
        }

        public Provincia GetProvinciaById(int id)
        {
            Provincia p = _ps.GetObject("Provincia", id) as Provincia;
            if (p == null)
                return new ProvinciaNulla();

            return p;
        }

        public System.Collections.IList GetProvincie()
        {
            return _ps.GetAllObjects("Provincia");
        }

        public System.Collections.IList GetRegioni()
        {
            return _ps.GetAllObjects("Regione");
        }

        #endregion
    }
}
