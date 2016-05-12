using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaptorDB;

namespace RaptorDBBackground
{
    public class RowSchema : RDBSchema
    {
        public string Cidade;
        public string Distancia_linha_reta_da_capital_km;
        public string Distancia_de_conducao_da_capital_km;
        public string Tempo_conducao;
    }

    [RegisterView]
    class DistanciasView : View<DistanciasToMaceio>
    {
        public DistanciasView()
        {
            this.Name = "Distancias";
            this.Description = "Uma primary View para DistanciasToMaceio";
            this.isPrimaryList = true;
            this.isActive = true;
            this.BackgroundIndexing = true;

            this.Schema = typeof(RowSchema);

            this.Mapper = (api, docid, doc) =>
            {
                api.EmitObject(docid, doc);
            };   
        }
    }
}
