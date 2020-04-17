using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg;
using TechTools.Core.Oracle9i;

namespace jbp.core.oracle9i
{
    public class FacturaStupendoCore:BaseCore
    {
        public static string SqlInsertArchivosProcesadosStupendo()
        {
            return @"
                insert into gms.tbl_arch_proc_stupendo(TITULO_DOC_STUPENDO)
                values('{0}')";
        }
        public static string SqlNumRegArchivosProcesadosStupendoByFileName()
        {
            return @"
                select count(*)
                from gms.TBL_ARCH_PROC_STUPENDO
                where titulo_doc_stupendo like '%{0}%'";
        }
        
    }
}
