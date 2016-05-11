using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Data;
using Newtonsoft.Json;

namespace RaptorDBBackground
{
    public class RaptorDBProgram
    {
        public int testSoma(int a, int b)
        {
            return a + b;
        }

        public String insertJson(string filePath)
        {
            using (StreamReader reader =  new StreamReader(filePath))
            {
                String json = reader.ReadToEnd();

                DataSet dataset = JsonConvert.DeserializeObject<DataSet>(json);
                DataTable dataTable = dataset.Tables["Distancias"];

                String cidades = "";

                foreach(DataRow row in dataTable.Rows)
                {
                    var dado = new DistanciasToMaceio();
                    dado.Municipio = row["Municipio"].ToString();
                    dado.Distancia_linha_reta_da_capital_km = row["Distancia_linha_reta_da_capital_km"].ToString();
                    dado.Distancia_de_conducao_da_capital_km = row["Distancia_de_conducao_da_capital_km"].ToString();
                    dado.Tempo_conducao = row["Tempo_conducao"].ToString();

                    cidades += dado.Distancia_de_conducao_da_capital_km+" ";
                }

                return cidades;
            }
           
        }
    }
}
