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
        static RaptorDB.RaptorDB rdb;//1 instancia

        public RaptorDBProgram()
        {
            rdb = RaptorDB.RaptorDB.Open("data"); //cria uma pasta "data" ao lado do executavel   
            RaptorDB.Global.RequirePrimaryView = false;

            rdb.RegisterView(new DistanciasView());
                
        }

        public string insertJson(string filePath)
        {
            using (StreamReader reader =  new StreamReader(filePath))
            {
                string json = reader.ReadToEnd();

                DataSet dataset = JsonConvert.DeserializeObject<DataSet>(json);
                DataTable dataTable = dataset.Tables["Distancias"];

                string texto = "";

                foreach(DataRow row in dataTable.Rows)
                {
                    var dado = new DistanciasToMaceio();
                    dado.Cidade = row["Cidade"].ToString();
                    dado.Distancia_linha_reta_da_capital_km = row["Distancia_linha_reta_da_capital_km"].ToString();
                    dado.Distancia_de_conducao_da_capital_km = row["Distancia_de_conducao_da_capital_km"].ToString();
                    dado.Tempo_conducao = row["Tempo_conducao"].ToString();
                    

                    bool isSalvo =rdb.Save(dado.docid, dado);
                    //rdb.Delete(dado.docid);

                    //teste
                    //    var result = rdb.Query<RowSchema>(x => x.Municipio == "Maceio");

                    if (isSalvo)
                    {
                        texto = "O documento foi inserido com sucesso!";
                    }
                    //texto = isSalvo.ToString();//fastJSON.JSON.ToNiceJSON(result.Rows, new fastJSON.JSONParameters { UseExtensions = false, UseFastGuid = false });

                    /* var q = rdb.Query(typeof(DistanciasToMaceio), // call by the view type or the primary document type
                 (DistanciasToMaceio s) => ( s.Municipio == "3"));

                     var result = rdb.Query(typeof(DistanciasToMaceio), (DistanciasToMaceio distancia) => (distancia.Municipio == "Maceio"));
               */

                }

                return texto;
            }
           
        }

        public string query(string query)
        {
          //  var result = rdb.Query<RowSchema>(x => x.Municipio == "Maceio");
          var result = rdb.Query<RowSchema>(query);

            return fastJSON.JSON.ToNiceJSON(result.Rows, new fastJSON.JSONParameters { UseExtensions = false, UseFastGuid = false });

        }


        public void shutdown()
        {
            rdb.Shutdown();
        }
    }
}
