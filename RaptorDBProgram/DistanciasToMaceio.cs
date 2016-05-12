using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaptorDBBackground
{
    class DistanciasToMaceio
    {
        public Guid docid { get; set; }
        public string Cidade { get; set; }
        public string Distancia_linha_reta_da_capital_km { get; set; }
        public string Distancia_de_conducao_da_capital_km { get; set; }
        public string Tempo_conducao { get; set; }

        public DistanciasToMaceio()
        {
            this.docid = Guid.NewGuid();
        }
    }
}
